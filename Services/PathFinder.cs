﻿using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard.Caching;
using Orchard.Environment.Extensions;
using Associativy.GraphDiscovery;
using QuickGraph;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class PathFinder : AssociativyServiceBase, IPathFinder
    {
        protected readonly IGraphEditor _graphEditor;
        protected readonly IGraphEventMonitor _associativeGraphEventMonitor;
        protected readonly ICacheManager _cacheManager;

        public PathFinder(
            IGraphManager graphManager,
            IGraphEditor graphEditor,
            IGraphEventMonitor associativeGraphEventMonitor,
            ICacheManager cacheManager)
            : base(graphManager)
        {
            _graphEditor = graphEditor;
            _associativeGraphEventMonitor = associativeGraphEventMonitor;
            _cacheManager = cacheManager;
        }


        #region FindPaths() auxiliary classes
        protected class PathNode
        {
            public int Id { get; private set; }
            public int MinDistance { get; set; }
            public List<PathNode> Neighbours { get; set; }

            public PathNode(int id)
            {
                Id = id;
                MinDistance = int.MaxValue;
                Neighbours = new List<PathNode>();
            }
        }

        protected class FrontierNode
        {
            public int Distance { get; set; }
            public List<int> Path { get; set; }
            public PathNode Node { get; set; }

            public FrontierNode()
            {
                Distance = 0;
                Path = new List<int>();
            }
        }
        #endregion

        public virtual PathResult FindPaths(IGraphContext graphContext, int startNodeId, int targetNodeId, int maxDistance = 3, bool useCache = false)
        {
            // It could be that this is the only caching that's really needed and can work:
            // - With this tens of database queries can be saved (when the connection manager uses DB storage without in-memory caching).
            // - Caching the whole graph would be nice, but caching parts and their records cause problems.
            if (useCache)
            {
                return _cacheManager.Get("Associativy.Paths." + graphContext.GraphName + startNodeId.ToString() + targetNodeId.ToString() + maxDistance, ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChanged(graphContext, ctx);
                    return FindPaths(graphContext, startNodeId, targetNodeId, maxDistance, false);
                });
            }

            // This below is a depth-first search that tries to find all paths to the target node that are within the maximal length (maxDistance) and
            // keeps track of the paths found.
            var connectionManager = _graphManager.FindGraph(graphContext).ConnectionManager;

            var explored = new Dictionary<int, PathNode>();
            var succeededPaths = new List<List<int>>();
            var traversedGraph = _graphEditor.GraphFactory<int>();
            var frontier = new Stack<FrontierNode>();

            explored[startNodeId] = new PathNode(startNodeId) { MinDistance = 0 };
            frontier.Push(new FrontierNode { Node = explored[startNodeId] });
            traversedGraph.AddVertex(startNodeId);

            FrontierNode frontierNode;
            PathNode currentNode;
            List<int> currentPath;
            int currentDistance;
            while (frontier.Count != 0)
            {
                frontierNode = frontier.Pop();
                currentNode = frontierNode.Node;
                currentPath = frontierNode.Path;
                currentPath.Add(currentNode.Id);
                currentDistance = frontierNode.Distance;

                // We can't traverse the graph further
                if (currentDistance == maxDistance - 1)
                {
                    // Target will be only found if it's the direct neighbour of current
                    if (connectionManager.AreNeighbours(graphContext, currentNode.Id, targetNodeId))
                    {
                        if (!explored.ContainsKey(targetNodeId))
                        {
                            explored[targetNodeId] = new PathNode(targetNodeId);
                            traversedGraph.AddVertex(targetNodeId);
                        }

                        if (explored[targetNodeId].MinDistance > currentDistance + 1)
                        {
                            explored[targetNodeId].MinDistance = currentDistance + 1;
                        }

                        currentNode.Neighbours.Add(explored[targetNodeId]);
                        currentPath.Add(targetNodeId);
                        succeededPaths.Add(currentPath);
                        traversedGraph.AddEdge(new UndirectedEdge<int>(currentNode.Id, targetNodeId));
                    }
                }
                // We can traverse the graph further
                else
                {
                    // If we haven't already fetched current's neighbours, fetch them
                    if (currentNode.Neighbours.Count == 0)
                    {
                        var neighbourIds = connectionManager.GetNeighbourIds(graphContext, currentNode.Id);
                        currentNode.Neighbours = new List<PathNode>(neighbourIds.Count());
                        foreach (var neighbourId in neighbourIds)
                        {
                            currentNode.Neighbours.Add(new PathNode(neighbourId));
                        }
                    }

                    foreach (var neighbour in currentNode.Neighbours)
                    {
                        // Target is a neighbour
                        if (neighbour.Id == targetNodeId)
                        {
                            var succeededPath = new List<int>(currentPath) { targetNodeId }; // Since we will use currentPath in further iterations too
                            succeededPaths.Add(succeededPath);
                        }
                        // We can traverse further, push the neighbour onto the stack
                        else if (neighbour.Id != startNodeId)
                        {
                            neighbour.MinDistance = currentDistance + 1;
                            if (!explored.ContainsKey(neighbour.Id) || neighbour.MinDistance >= currentDistance + 1)
                            {
                                frontier.Push(new FrontierNode { Distance = currentDistance + 1, Path = new List<int>(currentPath), Node = neighbour });
                            }
                        }

                        if (neighbour.Id != startNodeId)
                        {
                            // If this is the shortest path to the node, overwrite its minDepth
                            if (neighbour.MinDistance > currentDistance + 1)
                            {
                                neighbour.MinDistance = currentDistance + 1;
                            }

                            explored[neighbour.Id] = neighbour;
                            traversedGraph.AddVertex(neighbour.Id);
                            traversedGraph.AddEdge(new UndirectedEdge<int>(currentNode.Id, neighbour.Id)); 
                        }
                    }
                }
            }


            return new PathResult(succeededPaths, traversedGraph);
        }
    }
}