﻿using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using QuickGraph;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class GraphEditor : IGraphEditor
    {
        private readonly INodeManager _nodeManager;


        public GraphEditor(INodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }


        public IMutableUndirectedGraph<TNode, IUndirectedEdge<TNode>> GraphFactory<TNode>()
        {
            return new UndirectedGraph<TNode, IUndirectedEdge<TNode>>(false);
        }

        public virtual IUndirectedGraph<TNode, IUndirectedEdge<TNode>> CreateZoomedGraph<TNode>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph, int zoomLevel, int zoomLevelCount)
        {
            /// Removing all nodes that are above the specified zoom level
            var zoomedGraph = GraphFactory<TNode>();
            // Copying the original graph
            zoomedGraph.AddVertexRange(graph.Vertices); // With AddVerticesAndEdgeRange() nodes without edges wouldn't be copied
            zoomedGraph.AddEdgeRange(graph.Edges);

            var zoomPartitions = CalculateZoomPartitions(graph, zoomLevelCount);
            int i = zoomPartitions.Count - 1;
            while (i >= 0 && i > zoomLevel)
            {
                foreach (var node in zoomPartitions[i])
                {
                    zoomedGraph.RemoveVertex(node);
                }

                i--;
            }


            return zoomedGraph;
        }

        public virtual int CalculateZoomLevelCount<TNode>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph, int maximalZoomLevelCount)
        {
            return CalculateZoomPartitions(graph, maximalZoomLevelCount).Count;
        }

        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(IGraphContext graphContext, IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            var query = _nodeManager.GetManyQuery(graphContext, idGraph.Vertices);
            var nodes = query.List().ToDictionary(node => node.Id);

            var graph = GraphFactory<IContent>();
            graph.AddVertexRange(nodes.Values);

            foreach (var edge in idGraph.Edges)
            {
                // Since the query can be modified in an event handler and it could have removed items, this check is necessary
                if (nodes.ContainsKey(edge.Source) && nodes.ContainsKey(edge.Target))
                {
                    graph.AddEdge(new UndirectedEdge<IContent>(nodes[edge.Source], nodes[edge.Target]));
                }
            }

            return graph;
        }


        protected virtual List<List<TNode>> CalculateZoomPartitions<TNode>(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph, int zoomLevelCount)
        {
            // Caching doesn't work, fails at graph.AdjacentEdges(node), the graph can't find the object. Caused most likely
            // because the objects are not the same. But it seems that although the calculation to the last block is repeated
            // with the same numbers for a graph when calculating zoomed graphs, there is no gain in caching: the algorithm runs
            // freaking fast: ~100 ticks on i7@3,2Ghz, running sequentially. That's very far from even one ms...


            /// Grouping vertices by the number of their neighbours (= adjacentDegree)
            var nodes = graph.Vertices.ToList();
            var adjacentDegreeGroups = new SortedList<int, List<TNode>>();
            foreach (var node in nodes)
            {
                var adjacentDegree = graph.AdjacentDegree(node);
                if (!adjacentDegreeGroups.ContainsKey(adjacentDegree)) adjacentDegreeGroups[adjacentDegree] = new List<TNode>();
                adjacentDegreeGroups[adjacentDegree].Add(node);
            }


            /// Partitioning nodes into continuous zoom levels
            int approxVerticesInPartition = (int)Math.Round((double)(nodes.Count / zoomLevelCount), 0);
            if (approxVerticesInPartition == 0) approxVerticesInPartition = nodes.Count; // Too little number of nodes
            int currentRealZoomLevel = 0;
            int previousRealZoomLevel = -1;
            int nodeCountTillThisLevelInclusive = 0; // Including the current level
            var zoomPartitions = new List<List<TNode>>(zoomLevelCount); // Nodes partitioned by zoom level, filled up continuously
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