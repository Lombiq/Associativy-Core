using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using QuickGraph;
using Orchard.Services;
using Orchard.Caching;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    /// <summary>
    /// All suitable methods protected to aid inheritence.
    /// </summary>
    /// <typeparam name="TNodePart"></typeparam>
    /// <typeparam name="TNodePartRecord"></typeparam>
    /// <typeparam name="TNodeToNodeConnectorRecord"></typeparam>
    [OrchardFeature("Associativy")]
    public class Mind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IMind<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> connectionManager;
        protected readonly INodeManager<TNodePart, TNodePartRecord> nodeManager;
        protected readonly ICacheManager cacheManager;
        protected readonly IClock clock;

        public Mind(
            IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> connectionManager,
            INodeManager<TNodePart, TNodePartRecord> nodeManager,
            ICacheManager cacheManager,
            IClock clock)
        {
            this.connectionManager = connectionManager;
            this.nodeManager = nodeManager;
            this.cacheManager = cacheManager;
            this.clock = clock;
        }

        protected const int CacheLifetimeMin = 0;

        //class GraphToken : IVolatileToken
        //{
        //    public bool IsCurrent
        //    {
        //        get { valahogy kitalálni, módosult-e a tábla, talán NHibernate.ISession.cs }
        //    }
        //}

        public UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetAllAssociations(int zoomLevel = 0, bool useCache = true)
        {
            if (useCache)
            {
                return cacheManager.Get("Assciativy Whole Graph", ctx =>
                    {
                        ctx.Monitor(clock.When(TimeSpan.FromMinutes(CacheLifetimeMin)));
                        return GetAllAssociations(zoomLevel, false);
                    });
            }

            var graph = GraphFactory();

            var nodes = nodeManager.ContentQuery.List().ToDictionary<TNodePart, int>(node => node.Id);

            foreach (var node in nodes)
            {
                graph.AddVertex(node.Value);
            }

            var connections = connectionManager.GetAll();
            for (int i = 0; i < connections.Count; i++)
            {
                graph.AddEdge(new UndirectedEdge<TNodePart>(nodes[connections[i].Record1Id], nodes[connections[i].Record2Id]));
            }

            // Leaves out nodes that don't have any neighbours
            //foreach (var connection in _nodeManager.NodeToNodeRecordRepository.Table.ToList())
            //{
            //    graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(nodes[connection.Record1Id], nodes[connection.Record2Id]));
            //}

            return graph;
        }

        public UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociations(IList<TNodePart> nodes, bool simpleAlgorithm = false, int zoomLevel = 0, bool useCache = true)
        {
            if (useCache)
            {
                string cacheKey = "";
                nodes.ToList().ForEach(node => cacheKey += node.Id.ToString() + ", ");
                return cacheManager.Get(cacheKey, ctx =>
                    {
                        ctx.Monitor(clock.When(TimeSpan.FromMinutes(CacheLifetimeMin)));
                        return MakeAssociations(nodes, simpleAlgorithm, zoomLevel, false);
                    });
            }

            if (nodes == null) throw new ArgumentNullException("The list of searched nodes can't be empty");
            if (nodes.Count == 0) throw new ArgumentException("The list of searched nodes can't be empty");

            // If there's only one node, return its neighbours
            if (nodes.Count == 1)
            {
                return GetNeighboursGraph(nodes[0]);
            }
            // Simply calculate the intersection of the neighbours of the nodes
            else if (simpleAlgorithm)
            {
                return MakeSimpleAssocations(nodes);
            }
            // Calculate the routes between two nodes
            else
            {
                return MakeSophisticatedAssociations(nodes);
            }
        }

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetNeighboursGraph(TNodePart node)
        {
            var graph = GraphFactory();

            graph.AddVertex(node);

            var nodes = nodeManager.GetMany(connectionManager.GetNeighbourIds(node.Id));
            for (int i = 0; i < nodes.Count; i++)
            {
                graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(node, nodes[i]));
            }

            return graph;
        }

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeSimpleAssocations(IList<TNodePart> nodes)
        {
            // Simply calculate the intersection of the neighbours of the nodes

            var graph = GraphFactory();

            var commonNeighbourIds = connectionManager.GetNeighbourIds(nodes[0].Id);
            var remainingNodes = new List<TNodePart>(nodes); // Maybe later we will need all the searched nodes
            remainingNodes.RemoveAt(0);
            commonNeighbourIds = remainingNodes.Aggregate(commonNeighbourIds, (current, node) => current.Intersect(connectionManager.GetNeighbourIds(node.Id)).ToList());
            // Same as
            //foreach (var node in remainingNodes)
            //{
            //    commonNeighbourIds = commonNeighbourIds.Intersect(connectionManager.GetNeighbourIds(node.Id)).ToList();
            //}

            if (commonNeighbourIds.Count == 0) return null;

            var commonNeighbours = nodeManager.GetMany(commonNeighbourIds);

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int n = 0; n < commonNeighbours.Count; n++)
                {
                    graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(nodes[i], commonNeighbours[n]));
                }
            }

            return graph;
        }

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeSophisticatedAssociations(IList<TNodePart> nodes)
        {
            if (nodes.Count < 2) throw new ArgumentException("The count of nodes should be at least two.");

            var graph = GraphFactory();
            List<List<int>> succeededPaths;

            var allPairSucceededPaths = CalculatePaths(nodes[0].Id, nodes[1].Id);

            if (allPairSucceededPaths.Count == 0) return null;

            if (nodes.Count == 2)
            {
                succeededPaths = allPairSucceededPaths;
            }
            // Calculate the routes between every nodes pair, then calculate the intersection of the routes
            else
            {
                // We have to preserve the searched node ids in the succeeded paths despite the intersections
                var searchedNodeIds = new List<int>(nodes.Count);
                nodes.ToList().ForEach(
                        node => searchedNodeIds.Add(node.Id)
                    );

                var commonSucceededNodeIds = GetSucceededNodeIds(allPairSucceededPaths).Union(searchedNodeIds).ToList();

                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    int n = i + 1;
                    if (i == 0) n = 2; // Because of the calculation of intersections the first iteration is already done above

                    while (n < nodes.Count)
                    {
                        var pairSucceededPaths = CalculatePaths(nodes[i].Id, nodes[n].Id);
                        commonSucceededNodeIds = commonSucceededNodeIds.Intersect(GetSucceededNodeIds(pairSucceededPaths).Union(searchedNodeIds)).ToList();
                        allPairSucceededPaths = allPairSucceededPaths.Union(pairSucceededPaths).ToList();

                        n++;
                    }
                }

                if (allPairSucceededPaths.Count == 0 || commonSucceededNodeIds.Count == 0) return null;

                succeededPaths = new List<List<int>>(allPairSucceededPaths.Count); // We are oversizing, but it's worth the performance gain

                foreach (var path in allPairSucceededPaths)
                {
                    var succeededPath = path.Intersect(commonSucceededNodeIds);
                    if (succeededPath.Count() > 2) succeededPaths.Add(succeededPath.ToList()); // Only paths where intersecting nodes are present
                }

                if (succeededPaths.Count == 0) return null;
            }


            var succeededNodes = GetSucceededNodes(succeededPaths);

            foreach (var path in succeededPaths)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(succeededNodes[path[i - 1]], succeededNodes[path[i]]));
                }
            }

            return graph;
        }

        private Dictionary<int, TNodePart> GetSucceededNodes(List<List<int>> succeededPaths)
        {
            return nodeManager.GetMany(GetSucceededNodeIds(succeededPaths)).ToDictionary(node => node.Id);
        }

        private List<int> GetSucceededNodeIds(List<List<int>> succeededPaths)
        {
            var succeededNodeIds = new List<int>(succeededPaths.Count); // An incorrect estimate, but enhaces performance
            succeededPaths.ForEach(row => succeededNodeIds = succeededNodeIds.Union(row).ToList()); // To parallel?
            return succeededNodeIds;
        }

        #region CalculatePaths() auxiliary classes
        private class PathNode
        {
            public int Id { get; private set; }
            public int MinimumDepth { get; set; }
            public bool IsDeadEnd { get; set; }
            public List<PathNode> Neighbours { get; set; }

            public PathNode(int id)
            {
                Id = id;
                MinimumDepth = int.MaxValue;
                IsDeadEnd = false;
                Neighbours = new List<PathNode>();
            }
        }

        private class StackItem
        {
            public int Depth { get; set; }
            public List<int> Path { get; set; }
            public PathNode Node { get; set; }

            public StackItem()
            {
                Depth = 0;
                Path = new List<int>();
            }
        }
        #endregion

        private List<List<int>> CalculatePaths(int startId, int targetId, int maxDepth = 3, bool useCache = true)
        {
            if (useCache)
            {
                return cacheManager.Get(startId.ToString() + targetId.ToString() + maxDepth.ToString(), ctx =>
                {
                    ctx.Monitor(clock.When(TimeSpan.FromMinutes(CacheLifetimeMin)));
                    return CalculatePaths(startId, targetId, maxDepth, false);
                });
            }

            var found = false; // Maybe can be removed?
            var visitedNodes = new Dictionary<int, PathNode>();
            var succeededPaths = new List<List<int>>();
            var stack = new Stack<StackItem>();

            visitedNodes[startId] = new PathNode(startId) { MinimumDepth = 0 };
            stack.Push(new StackItem { Node = visitedNodes[startId] });
            visitedNodes[targetId] = new PathNode(targetId);

            StackItem stackItem;
            PathNode currentNode;
            List<int> currentPath;
            int currentDepth;
            while (stack.Count != 0)
            {
                stackItem = stack.Pop();
                currentNode = stackItem.Node;
                currentPath = stackItem.Path;
                currentPath.Add(currentNode.Id);
                currentDepth = stackItem.Depth;

                // We can't traverse the graph further
                if (currentDepth == maxDepth - 1)
                {
                    // Target will be only found if it's the direct neighbour of current
                    if (connectionManager.AreNeighbours(currentNode.Id, targetId))
                    {
                        found = true;
                        if (visitedNodes[targetId].MinimumDepth > currentDepth + 1)
                        {
                            visitedNodes[targetId].MinimumDepth = currentDepth + 1;
                        }

                        currentNode.Neighbours.Add(visitedNodes[targetId]);
                        currentPath.Add(targetId);
                        succeededPaths.Add(currentPath);
                        // if ($this->debugMode) echo "##found from ".$node->loadById($currentNode->id)->notion."\n";
                    }
                    // else if ($this->debugMode) echo "<<-maxdepth backtrack (not found in neighbours)\n";
                }
                // We can traverse the graph further
                else if (!currentNode.IsDeadEnd)
                {
                    // If we haven't already fetched current's neighbours, fetch them
                    if (currentNode.Neighbours.Count == 0)
                    {
                        // Measure performance with large datasets, as Parallel.ForEach tends to be slower
                        //Parallel.ForEach(GetNeighbourIds(currentNode.Id), neighbourId =>
                        //    {
                        //        if (!visitedNodes.ContainsKey(neighbourId))
                        //        {
                        //            visitedNodes[neighbourId] = new GraphNode(neighbourId);
                        //        }
                        //        neighbours.Add(visitedNodes[neighbourId]);
                        //    });

                        var neighbourIds = connectionManager.GetNeighbourIds(currentNode.Id);
                        currentNode.Neighbours = new List<PathNode>(neighbourIds.Count);
                        foreach (var neighbourId in neighbourIds)
                        {
                            if (!visitedNodes.ContainsKey(neighbourId))
                            {
                                visitedNodes[neighbourId] = new PathNode(neighbourId);
                            }
                            currentNode.Neighbours.Add(visitedNodes[neighbourId]);
                        }

                        // The only path to this node is where we have come from
                        if (currentNode.Neighbours.Count == 1)
                        {
                            currentNode.IsDeadEnd = true;
                            // if ($this->debugMode) echo "///dead end\n";
                        }
                    }

                    if (!currentNode.IsDeadEnd)
                    {
                        // Measure performance with large datasets, as Parallel.ForEach tends to be slower
                        //Parallel.ForEach(neighbours, neighbourItem =>
                        //    {
                        //        var neighbour = neighbourItem.Value;
                        //        currentNode.Neighbours[neighbour.Id] = neighbour;

                        //        // Target is a neighbour
                        //        if (neighbour.Id == targetId)
                        //        {
                        //            found = true;
                        //            var succeededPath = new List<int>(currentPath); // Since we will use currentPath in further iterations too
                        //            succeededPath.Add(targetId);
                        //            succeededPaths.Add(succeededPath);
                        //            // if ($this->debugMode) echo "##found from ".$node->loadById($currentNode->id)->notion."\n";
                        //        }
                        //        // We can traverse further, push the neighbour onto the stack
                        //        else if (neighbour.Id != startId &&
                        //            currentDepth + 1 + visitedNodes[targetId].MinimumDepth - currentNode.MinimumDepth <= maxDepth)
                        //        {
                        //            neighbour.MinimumDepth = currentDepth + 1;
                        //            stack.Push(new StackItem { Depth = currentDepth + 1, Path = currentPath, Node = neighbour });
                        //        }

                        //        // If this is the shortest path to the node, overwrite its minDepth
                        //        if (neighbour.Id != startId && neighbour.MinimumDepth > currentDepth + 1)
                        //        {
                        //            neighbour.MinimumDepth = currentDepth + 1;
                        //        }
                        //    });
                        foreach (var neighbour in currentNode.Neighbours)
                        {
                            // Target is a neighbour
                            if (neighbour.Id == targetId)
                            {
                                found = true;
                                var succeededPath = new List<int>(currentPath) { targetId }; // Since we will use currentPath in further iterations too
                                succeededPaths.Add(succeededPath);
                                // if ($this->debugMode) echo "##found from ".$node->loadById($currentNode->id)->notion."\n";
                            }
                            // We can traverse further, push the neighbour onto the stack
                            else if (neighbour.Id != startId &&
                                currentDepth + 1 + visitedNodes[targetId].MinimumDepth - currentNode.MinimumDepth <= maxDepth)
                            {
                                neighbour.MinimumDepth = currentDepth + 1;
                                stack.Push(new StackItem { Depth = currentDepth + 1, Path = new List<int>(currentPath), Node = neighbour });
                            }

                            // If this is the shortest path to the node, overwrite its minDepth
                            if (neighbour.Id != startId && neighbour.MinimumDepth > currentDepth + 1)
                            {
                                neighbour.MinimumDepth = currentDepth + 1;
                            }
                        }
                    }

                }
                // else if ($this->debugMode) echo "///was a dead end\n";
            }


            return succeededPaths;
        }

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GraphFactory()
        {
            return new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();
        }
    }
}