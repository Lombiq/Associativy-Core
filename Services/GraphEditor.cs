using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.Models;
using QuickGraph;
using Orchard.ContentManagement;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class GraphEditor : IGraphEditor
    {
        public IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GraphFactory()
        {
            return new UndirectedGraph<IContent, IUndirectedEdge<IContent>>(false);
        }

        public virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> CreateZoomedGraph(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int zoomLevel, int zoomLevelCount)
        {
            /// Removing all nodes that are above the specified zoom level
            IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> zoomedGraph = GraphFactory();
            // Copying the original graph
            zoomedGraph.AddVertexRange(graph.Vertices); // With AddVerticesAndEdgeRange() nodes without edges wouldn't be copied
            zoomedGraph.AddEdgeRange(graph.Edges);

            var zoomPartitions = CalculateZoomPartitions(graph, zoomLevelCount);
            int i = zoomPartitions.Count - 1;
            while (i >= 0 && i > zoomLevel)
            {
                foreach (var node in zoomPartitions[i])
                {
                    // Rewiring all edges so that nodes previously connected through this nodes now get directly connected
                    // Looks unneeded and wrong
                    //if (zoomedGraph.AdjacentDegree(node) > 1)
                    //{
                    //    foreach (var edge in zoomedGraph.AdjacentEdges(node))
                    //    {

                    //    }
                    //}
                    zoomedGraph.RemoveVertex(node);
                }

                i--;
            }


            return zoomedGraph;
        }

        public virtual int CalculateZoomLevelCount(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int zoomLevelCount)
        {
            return CalculateZoomPartitions(graph, zoomLevelCount).Count;
        }

        protected virtual List<List<IContent>> CalculateZoomPartitions(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int zoomLevelCount)
        {
            // Caching doesn't work, fails at graph.AdjacentEdges(node), the graph can't find the object. Caused most likely
            // because the objects are not the same. But it seems that although the calculation to the last block is repeated
            // with the same numbers for a graph when calculating zoomed graphs, there is no gain in caching: the algorithm runs
            // freaking fast: ~100 ticks on i7@3,2Ghz, running sequentially. That's very far from even one ms...


            /// Grouping vertices by the number of their neighbours (= adjacentDegree)
            var nodes = graph.Vertices.ToList();
            var adjacentDegreeGroups = new SortedList<int, List<IContent>>();
            foreach (var node in nodes)
            {
                var adjacentDegree = graph.AdjacentDegree(node);
                if (!adjacentDegreeGroups.ContainsKey(adjacentDegree)) adjacentDegreeGroups[adjacentDegree] = new List<IContent>();
                adjacentDegreeGroups[adjacentDegree].Add(node);
            }


            /// Partitioning nodes into continuous zoom levels
            int approxVerticesInPartition = (int)Math.Round((double)(nodes.Count / zoomLevelCount), 0);
            if (approxVerticesInPartition == 0) approxVerticesInPartition = nodes.Count; // Too little number of nodes
            int currentRealZoomLevel = 0;
            int previousRealZoomLevel = -1;
            int nodeCountTillThisLevelInclusive = 0; // Including the current level
            var zoomPartitions = new List<List<IContent>>(zoomLevelCount); // Nodes partitioned by zoom level, filled up continuously
            // Iterating backwards as nodes with higher neighbourCount are on the top
            // I.e.: with zoomlevel 0 only the nodes with the highest neighbourCount will be returned, on ZoomLevelCount
            // all the nodes.
            var reversedAdjacentDegreeGroups = adjacentDegreeGroups.Reverse();
            foreach (var nodeGroup in reversedAdjacentDegreeGroups)
            {
                nodeCountTillThisLevelInclusive += nodeGroup.Value.Count;

                // +1 so that filled up partitions don't invoke a start of a new one, e.g. if nodeCountTillThisLevelInclusive = approxVerticesInPartition = 5
                // this ensures that there will still be only one partition.
                currentRealZoomLevel = (int)Math.Floor((double)(nodeCountTillThisLevelInclusive / (approxVerticesInPartition + 1)));

                if (previousRealZoomLevel != currentRealZoomLevel) zoomPartitions.Add(nodeGroup.Value); // We've reached a new zoom level
                else zoomPartitions[zoomPartitions.Count - 1].AddRange(nodeGroup.Value);

                previousRealZoomLevel = currentRealZoomLevel;
            }


            return zoomPartitions;
        }
    }
}