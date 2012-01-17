using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard.Caching;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class PathFinder<TGraphDescriptor> : AssociativyServiceBase, IPathFinder<TGraphDescriptor>
        where TGraphDescriptor : IGraphDescriptor
    {
        protected readonly IGraphEventMonitor _associativeGraphEventMonitor;
        protected readonly ICacheManager _cacheManager;

        public PathFinder(
            TGraphDescriptor graphDescriptor,
            IGraphEventMonitor associativeGraphEventMonitor,
            ICacheManager cacheManager)
            : base(graphDescriptor)
        {
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

        public virtual IEnumerable<IEnumerable<int>> FindPaths(int startNodeId, int targetNodeId, IMindSettings settings)
        {
            // It could be that this is the only caching that's really needed and can work:
            // - With this tens of database queries can be saved.
            // - Caching the whole graph is nice, but caching parts and their records could cause problems. None is yet known,
            //   but it can happen.
            if (settings.UseCache)
            {
                return _cacheManager.Get("Associativy." + startNodeId.ToString() + targetNodeId.ToString() + settings.MaxDistance, ctx =>
                {
                    _associativeGraphEventMonitor.MonitorChanged(ctx, GraphDescriptor);
                    settings.UseCache = false;
                    return FindPaths(startNodeId, targetNodeId, settings);
                });
            }

            var explored = new Dictionary<int, PathNode>();
            var succeededPaths = new List<IList<int>>();
            var frontier = new Stack<FrontierNode>();

            explored[startNodeId] = new PathNode(startNodeId) { MinDistance = 0 };
            frontier.Push(new FrontierNode { Node = explored[startNodeId] });

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
                    if (GraphDescriptor.ConnectionManager.AreNeighbours(currentNode.Id, targetNodeId))
                    {
                        if (!explored.ContainsKey(targetNodeId)) explored[targetNodeId] = new PathNode(targetNodeId);
                        if (explored[targetNodeId].MinDistance > currentDistance + 1)
                        {
                            explored[targetNodeId].MinDistance = currentDistance + 1;
                        }

                        currentNode.Neighbours.Add(explored[targetNodeId]);
                        currentPath.Add(targetNodeId);
                        succeededPaths.Add(currentPath);
                    }
                }
                // We can traverse the graph further
                else
                {
                    // If we haven't already fetched current's neighbours, fetch them
                    if (currentNode.Neighbours.Count == 0)
                    {
                        var neighbourIds = GraphDescriptor.ConnectionManager.GetNeighbourIds(currentNode.Id);
                        currentNode.Neighbours = new List<PathNode>(neighbourIds.Count());
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
                        if (neighbour.Id == targetNodeId)
                        {
                            var succeededPath = new List<int>(currentPath) { targetNodeId }; // Since we will use currentPath in further iterations too
                            succeededPaths.Add(succeededPath);
                        }
                        // We can traverse further, push the neighbour onto the stack
                        else if (neighbour.Id != startNodeId)
                        {
                            neighbour.MinDistance = currentDistance + 1;
                            frontier.Push(new FrontierNode { Distance = currentDistance + 1, Path = new List<int>(currentPath), Node = neighbour });
                        }

                        // If this is the shortest path to the node, overwrite its minDepth
                        if (neighbour.Id != startNodeId && neighbour.MinDistance > currentDistance + 1)
                        {
                            neighbour.MinDistance = currentDistance + 1;
                        }
                    }
                }
            }


            return succeededPaths;
        }
    }

    [OrchardFeature("Associativy")]
    public class PathFinder : PathFinder<IGraphDescriptor>, IPathFinder
    {
        public PathFinder(
            IGraphDescriptor graphDescriptor,
            IGraphEventMonitor associativeGraphEventMonitor,
            ICacheManager cacheManager)
            : base(graphDescriptor, associativeGraphEventMonitor, cacheManager)
        {
        }
    }
}