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

namespace Associativy.Services
{
    public class Mind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> : IMind<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        #region Dependencies
        private readonly IContentManager _contentManager;
        private readonly INodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> _nodeManager;
        private readonly ICacheManager _cacheManager;
        private readonly IClock _clock;
        #endregion

        public Mind(
            IContentManager contentManager,
            INodeManager<TNodePart, TNodePartRecord, TNodeParams, TNodeToNodeConnectorRecord> nodeManager,
            ICacheManager cacheManager,
            IClock clock)
        {
            _contentManager = contentManager;
            _nodeManager = nodeManager;
            _cacheManager = cacheManager;
            _clock = clock;
        }

        private const int CacheLifetimeMin = 0;

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

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> GetAllAssociationsWithoutCaching(int zoomLevel = 0)
        {
            var graph = new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();

            var nodes = _nodeManager.ContentQuery.List().ToDictionary<TNodePart, int>(node => node.Id);

            foreach (var node in _nodeManager.ContentQuery.List().ToDictionary<TNodePart, int>(node => node.Id))
            {
                graph.AddVertex(node.Value);
            }

            foreach (var connection in _nodeManager.GetAllConnections())
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

        public UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociations(IList<string> terms, bool simpleAlgorithm = false)
        {
            return _cacheManager.Get("Assciativy Whole Graph", ctx =>
            {
                ctx.Monitor(_clock.When(TimeSpan.FromMinutes(CacheLifetimeMin)));
                return MakeAssociationsWithoutCaching(terms, simpleAlgorithm);
            });
        }

        private UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociationsWithoutCaching(IList<string> terms, bool simpleAlgorithm = false)
        {
            var graph = new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();

            //graph.AddVerticesAndEdge(new UndirectedEdge<TNodePart>(1, 2));

            var succeededPaths = CalculatePaths(1, 2);
            var succeededNodeIds = new List<int>();
            succeededPaths.ForEach(row => succeededNodeIds = succeededNodeIds.Union(row).ToList());
            var succeededNodes = _nodeManager.ContentQuery.Where(node => succeededNodeIds.Contains(node.Id)).List();

            return graph;
        }

        #region CalculatePaths() auxiliary classes
        private class PathNode
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

        private List<List<int>> CalculatePaths(int startId, int targetId, int maxDepth = 3)
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
                    if (_nodeManager.AreConnected(currentNode.Id, targetId))
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

                        foreach (var neighbourId in _nodeManager.GetNeighbourIds(currentNode.Id))
                        {
                            if (!visitedNodes.ContainsKey(neighbourId))
                            {
                                visitedNodes[neighbourId] = new PathNode(neighbourId);
                            }
                            neighbours[neighbourId] = visitedNodes[neighbourId];
                        }

                        // The only path to this node is where we have come from
                        if (neighbours.Count == 0)
                        {
                            currentNode.IsDeadEnd = true;
                            // if ($this->debugMode) echo "///dead end\n";
                        }
                    }
                    else
                    {
                        neighbours = visitedNodes[currentNode.Id].Neighbours;
                    }

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
                            stack.Push(new StackItem { Depth = currentDepth + 1, Path = currentPath, Node = neighbour });
                        }

                        // If this is the shortest path to the node, overwrite its minDepth
                        if (neighbour.Id != startId && neighbour.MinimumDepth > currentDepth + 1)
                        {
                            neighbour.MinimumDepth = currentDepth + 1;
                        }
                    }
                }
                // else if ($this->debugMode) echo "///was a dead end\n";
            }


            return succeededPaths;
        }
    }
}