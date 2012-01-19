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
    public class GraphService : IGraphService
    {
        public virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> GraphFactory()
        {
            return new UndirectedGraph<IContent, IUndirectedEdge<IContent>>(false);
        }

        public virtual IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> CreateZoomedGraph(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int zoomLevel, int maxZoomLevel)
        {
            /// Removing all nodes that are above the specified zoom level
            IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> zoomedGraph = GraphFactory();
            // Copying the original graph
            zoomedGraph.AddVertexRange(graph.Vertices); // With AddVerticesAndEdgeRange() nodes without edges wouldn't be copied
            zoomedGraph.AddEdgeRange(graph.Edges);

            var zoomPartitions = CalculateZoomPartitions(graph, maxZoomLevel);
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

        public virtual int CalculateZoomLevelCount(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int maxZoomLevel)
        {
            return CalculateZoomPartitions(graph, maxZoomLevel).Count;
        }

        protected virtual List<List<IContent>> CalculateZoomPartitions(IMutableUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, int maxZoomLevel)
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
            int approxVerticesInPartition = (int)Math.Round((double)(nodes.Count / maxZoomLevel), 0);
            if (approxVerticesInPartition == 0) approxVerticesInPartition = nodes.Count; // Too little number of nodes
            int currentRealZoomLevel = 0;
            int previousRealZoomLevel = -1;
            int nodeCountTillThisLevel = 0;
            var zoomPartitions = new List<List<IContent>>(maxZoomLevel); // Nodes partitioned by zoom level, filled up continuously
            // Iterating backwards as nodes with higher neighbourCount are on the top
            // I.e.: with zoomlevel 0 only the nodes with the highest neighbourCount will be returned, on MaxZoomLevel
            // all the nodes.
            var reversedAdjacentDegreeGroups = adjacentDegreeGroups.Reverse();
            foreach (var nodeGroup in reversedAdjacentDegreeGroups)
            {
                nodeCountTillThisLevel += nodeGroup.Value.Count;
                currentRealZoomLevel = (int)Math.Floor((double)(nodeCountTillThisLevel / approxVerticesInPartition));

                if (previousRealZoomLevel != currentRealZoomLevel) zoomPartitions.Add(nodeGroup.Value); // We've reached a new zoom level
                else zoomPartitions[zoomPartitions.Count - 1].AddRange(nodeGroup.Value);

                previousRealZoomLevel = currentRealZoomLevel;
            }


            return zoomPartitions;
        }
    }
}