using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Models.Mind;
using Associativy.EventHandlers;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;

namespace Associativy.Services
{
    public class PathFinder<TNodeToNodeConnectorRecord> : IPathFinder<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IConnectionManager<TNodeToNodeConnectorRecord> _connectionManager;
        protected readonly IAssociativeGraphEventMonitor _associativeGraphEventMonitor;
        protected readonly ICacheManager _cacheManager;
        protected readonly string GraphSignal = "Associativy.Graph.Connections." + typeof(TNodeToNodeConnectorRecord).Name;

        public PathFinder(
            IConnectionManager<TNodeToNodeConnectorRecord> connectionManager,
            IAssociativeGraphEventMonitor associativeGraphEventMonitor,
            ICacheManager cacheManager)
        {
            _connectionManager = connectionManager;
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

        public virtual IList<IList<int>> FindPaths(INode startNode, INode targetNode, IMindSettings settings)
        {
            var startId = startNode.Id;
            var targetId = targetNode.Id;

            if (settings.UseCache)
            {
                return _cacheManager.Get("Associativy." + startId.ToString() + targetId.ToString() + settings.MaxDistance, ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChangedSignal(ctx, GraphSignal);
                    settings.UseCache = false;
                    return FindPaths(startNode, targetNode, settings);
                });
            }

            var explored = new Dictionary<int, PathNode>();
            var succeededPaths = new List<IList<int>>();
            var frontier = new Stack<FrontierNode>();

            explored[startId] = new PathNode(startId) { MinDistance = 0 };
            frontier.Push(new FrontierNode { Node = explored[startId] });

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
                if (currentDistance == settings.MaxDistance - 1)
                {
                    // Target will be only found if it's the direct neighbour of current
                    if (_connectionManager.AreNeighbours(currentNode.Id, targetId))
                    {
                        if (!explored.ContainsKey(targetId)) explored[targetId] = new PathNode(targetId);
                        if (explored[targetId].MinDistance > currentDistance + 1)
                        {
                            explored[targetId].MinDistance = currentDistance + 1;
                        }

                        currentNode.Neighbours.Add(explored[targetId]);
                        currentPath.Add(targetId);
                        succeededPaths.Add(currentPath);
                    }
                }
                // We can traverse the graph further
                else
                {
                    // If we haven't already fetched current's neighbours, fetch them
                    if (currentNode.Neighbours.Count == 0)
                    {
                        var neighbourIds = _connectionManager.GetNeighbourIds(currentNode.Id);
                        currentNode.Neighbours = new List<PathNode>(neighbourIds.Count);
                        foreach (var neighbourId in neighbourIds)
                        {
                            if (!explored.ContainsKey(neighbourId))
                            {
                                explored[neighbourId] = new PathNode(neighbourId);
                            }
                            currentNode.Neighbours.Add(explored[neighbourId]);
                        }
                    }

                    foreach (var neighbour in currentNode.Neighbours)
                    {
                        // Target is a neighbour
                        if (neighbour.Id == targetId)
                        {
                            var succeededPath = new List<int>(currentPath) { targetId }; // Since we will use currentPath in further iterations too
                            succeededPaths.Add(succeededPath);
                        }
                        // We can traverse further, push the neighbour onto the stack
                        else if (neighbour.Id != startId)
                        {
                            neighbour.MinDistance = currentDistance + 1;
                            frontier.Push(new FrontierNode { Distance = currentDistance + 1, Path = new List<int>(currentPath), Node = neighbour });
                        }

                        // If this is the shortest path to the node, overwrite its minDepth
                        if (neighbour.Id != startId && neighbour.MinDistance > currentDistance + 1)
                        {
                            neighbour.MinDistance = currentDistance + 1;
                        }
                    }
                }
            }


            return succeededPaths;
        }
    }
}