using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using QuickGraph;
using Orchard.Data;
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
        protected readonly IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> _connectionManager;
        protected readonly INodeManager<TNodePart, TNodePartRecord> _nodeManager;
        protected readonly ICacheManager _cacheManager;
        protected readonly IClock _clock;

        public Mind(
            IConnectionManager<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> connectionManager,
            INodeManager<TNodePart, TNodePartRecord> nodeManager,
            ICacheManager cacheManager,
            IClock clock)
        {
            _connectionManager = connectionManager;
            _nodeManager = nodeManager;
            _cacheManager = cacheManager;
            _clock = clock;
        }

        protected const int CacheLifetimeMin = 0;

        //class GraphToken : IVolatileToken
        //{
        //    public bool IsCurrent
        //    {
        //        get { valahogy kitalálni, módosult-e a tábla, talán NHibernate.ISession.cs }
        //    }
        //}

        public UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetAllAssociations(int zoomLevel = 0)
        {
            return _cacheManager.Get("Assciativy Whole Graph", ctx =>
            {
                ctx.Monitor(_clock.When(TimeSpan.FromMinutes(CacheLifetimeMin)));
                return GetAllAssociationsWithoutCaching(zoomLevel);
            });
        }

        protected UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetAllAssociationsWithoutCaching(int zoomLevel = 0)
        {
            var graph = new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();

            var nodes = _nodeManager.ContentQuery.List().ToDictionary<TNodePart, int>(node => node.Id);

            foreach (var node in _nodeManager.ContentQuery.List().ToDictionary<TNodePart, int>(node => node.Id))
            {
                graph.AddVertex(node.Value);
            }

            foreach (var connection in _connectionManager.GetAll())
            {
                graph.AddEdge(new UndirectedEdge<TNodePart>(nodes[connection.Record1Id], nodes[connection.Record2Id]));
            }

            // Leaves out nodes that don't have any neighbours
            //foreach (var connection in _nodeManager.NodeToNodeRecordRepository.Table.ToList())
            //{
            //    graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(nodes[connection.Record1Id], nodes[connection.Record2Id]));
            //}

            return graph;
        }

        public UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociations(IList<TNodePart> nodes, bool simpleAlgorithm = false, int zoomLevel = 0)
        {
            return _cacheManager.Get("Assciativy Whole Graph", ctx =>
            {
                ctx.Monitor(_clock.When(TimeSpan.FromMinutes(CacheLifetimeMin)));
                return MakeAssociationsWithoutCaching(nodes, simpleAlgorithm);
            });
        }

        protected UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociationsWithoutCaching(IList<TNodePart> nodes, bool simpleAlgorithm = false, int zoomLevel = 0)
        {
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

        protected UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetNeighboursGraph(TNodePart node)
        {
            var graph = new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();

            graph.AddVertex(node);

            foreach (var currentNode in _nodeManager.GetMany(_connectionManager.GetNeighbourIds(node.Id)))
            {
                graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(node, currentNode));
            }

            return graph;
        }

        protected UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeSimpleAssocations(IList<TNodePart> nodes)
        {
            var graph = new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();

            // Simply calculate the intersection of the neighbours of the nodes
            var commonNeighbourIds = _connectionManager.GetNeighbourIds(nodes[0].Id);
            var remainingNodes = new List<TNodePart>(nodes); // Maybe later we will need all the searched nodes
            remainingNodes.RemoveAt(0);
            foreach (var node in remainingNodes)
            {
                commonNeighbourIds = commonNeighbourIds.Intersect(_connectionManager.GetNeighbourIds(node.Id)).ToList();
            }

            if (commonNeighbourIds.Count == 0) return null;

            var commonNeighbours = _nodeManager.GetMany(commonNeighbourIds);

            foreach (var node in nodes)
            {
                foreach (var neighbour in commonNeighbours)
                {
                    graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(node, neighbour));
                }
            }

            return graph;
        }

        protected UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeSophisticatedAssociations(IList<TNodePart> nodes)
        {
            if (nodes.Count < 2) throw new ArgumentException("The count of nodes should be at least two.");

            var graph = new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();

            if (nodes.Count == 2)
            {
                var succeededPaths = CalculatePaths(nodes[0].Id, nodes[1].Id);
                if (succeededPaths.Count == 0) return null;

                var succeededNodes = GetSucceededNodes(succeededPaths);

                foreach (var path in succeededPaths)  // To parallel?
                {
                    for (int i = 1; i < path.Count; i++)
                    {
                        graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(succeededNodes[path[i - 1]], succeededNodes[path[i]]));
                    }
                }
            }
            // Calculate the routes between every nodes pair, then calculate the intersection of the routes
            else if (nodes.Count > 2)
            {
                //var allPairSucceededPaths = new List<List<int>>();
                //var succeededNodeIds = new List<int>(); // Without those nodes that were not present in all paths

                //for (int i = 0; i < nodes.Count - 1; i++)
                //{
                //    for (int n = i + 1; n < nodes.Count; n++)
                //    {
                //        var pairSucceededPaths = CalculatePaths(nodes[i].Id, nodes[n].Id);
                //        succeededNodeIds = 
                //        allPairSucceededPaths = allPairSucceededPaths.Union(pairSucceededPaths).ToList();
                //    }
                //}

                //if (allPairSucceededPaths.Count == 0) return null;

                //var allSucceededNodeIds = GetSucceededNodeIds(allPairSucceededPaths);

                //var succeededPaths = new List<List<int>>(); 
                //succeededPaths.ForEach(row => succeededNodeIds = succeededNodeIds.Union(row).ToList()); // To parallel?

                int z;
            }

            return graph;
        }

        protected Dictionary<int, TNodePart> GetSucceededNodes(List<List<int>> succeededPaths)
        {
            return _nodeManager.GetMany(GetSucceededNodeIds(succeededPaths)).ToDictionary<TNodePart, int>(node => node.Id);
        }

        protected List<int> GetSucceededNodeIds(List<List<int>> succeededPaths)
        {
            var succeededNodeIds = new List<int>();
            succeededPaths.ForEach(row => succeededNodeIds = succeededNodeIds.Union(row).ToList()); // To parallel?
            return succeededNodeIds;
        }

        #region CalculatePaths() auxiliary classes
        protected class PathNode
        {
            public int Id { get; set; }
            public int MinimumDepth { get; set; }
            public bool IsDeadEnd { get; set; }
            public Dictionary<int, PathNode> Neighbours { get; set; }

            public PathNode(int id)
            {
                Id = id;
                MinimumDepth = int.MaxValue;
                IsDeadEnd = false;
                Neighbours = new Dictionary<int, PathNode>();
            }
        }

        protected class StackItem
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

        protected List<List<int>> CalculatePaths(int startId, int targetId, int maxDepth = 3)
        {
            // Cache itt is.
            var found = false; // Maybe can be removed
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
                    if (_connectionManager.AreConnected(currentNode.Id, targetId))
                    {
                        found = true;
                        if (visitedNodes[targetId].MinimumDepth > currentDepth + 1)
                        {
                            visitedNodes[targetId].MinimumDepth = currentDepth + 1;
                        }

                        currentNode.Neighbours[targetId] = visitedNodes[targetId];
                        currentPath.Add(targetId);
                        succeededPaths.Add(currentPath);
                        // if ($this->debugMode) echo "##found from ".$node->loadById($currentNode->id)->notion."\n";
                    }
                    // else if ($this->debugMode) echo "<<-maxdepth backtrack (not found in neighbours)\n";
                }
                // We can traverse the graph further
                else if (!currentNode.IsDeadEnd)
                {
                    var neighbours = new Dictionary<int, PathNode>();

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
                        //        neighbours[neighbourId] = visitedNodes[neighbourId];
                        //    });

                        foreach (var neighbourId in _connectionManager.GetNeighbourIds(currentNode.Id))
                        {
                            if (!visitedNodes.ContainsKey(neighbourId))
                            {
                                visitedNodes[neighbourId] = new PathNode(neighbourId);
                            }
                            neighbours[neighbourId] = visitedNodes[neighbourId];
                        }

                        // The only path to this node is where we have come from
                        if (neighbours.Count == 1)
                        {
                            currentNode.IsDeadEnd = true;
                            // if ($this->debugMode) echo "///dead end\n";
                        }
                    }
                    else
                    {
                        neighbours = visitedNodes[currentNode.Id].Neighbours;
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
                        foreach (var neighbourItem in neighbours)
                        {
                            var neighbour = neighbourItem.Value;
                            currentNode.Neighbours[neighbour.Id] = neighbour;

                            // Target is a neighbour
                            if (neighbour.Id == targetId)
                            {
                                found = true;
                                var succeededPath = new List<int>(currentPath); // Since we will use currentPath in further iterations too
                                succeededPath.Add(targetId);
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
    }
}