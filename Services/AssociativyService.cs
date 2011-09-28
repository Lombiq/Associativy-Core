using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.Models;
using Orchard.Data;
using Orchard.ContentManagement;
using QuickGraph;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyService<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : IAssociativyService<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : NodePart<TNodePartRecord>
        where TNodePartRecord : NodePartRecord //ContentPartRecord
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        #region Dependencies
        private readonly IContentManager _contentManager;
        private readonly IRepository<TNodeToNodeConnectorRecord> _nodeToNodeRecordRepository;
        private readonly IRepository<TNodePartRecord> _nodePartRecordRepository;
        #endregion

        public AssociativyService(IContentManager contentManager, IRepository<TNodeToNodeConnectorRecord> nodeToNodeRecordRepository, IRepository<TNodePartRecord> nodePartRecordRepository)
        {
            _contentManager = contentManager;
            _nodeToNodeRecordRepository = nodeToNodeRecordRepository;
            _nodePartRecordRepository = nodePartRecordRepository;
        }

        //public IList<NodePartRecord> GetNeighbours(int nodeId)
        //{
        //    var neighbourIds = GetNeighbourIds(nodeId);

        //    return _nodePartRecordRepository.Fetch(node => neighbourIds.Contains(node.Id)).ToList();
        //    //var z = _contentManager.Query<NodePart, NodePartRecord>().Where(node => neighbourIds.Contains(node.Id)).List(); // ez valszeg azért nem megy, mert nem a contentmanagerrel hoztuk létre a node-okat
        //}

        //public TNodePart CreateNode(INode node)
        //{
        //    var notion1 = _contentManager.New<TNodePart>("Notion");
        //    notion1 = node;
        //    notion1.Label = "forró";
        //    _contentManager.Create(notion1);

        //    return notion1;
        //}

        public void AddConnection(TNodePart node1, TNodePart node2)
        {
            if (!AreConnected(node1.Id, node2.Id))
            {
                _nodeToNodeRecordRepository.Create(new TNodeToNodeConnectorRecord() { Record1Id = node1.Id, Record2Id = node2.Id });
            }
        }

        public void AddConnection(int nodeId1, int nodeId2)
        {
            if (!AreConnected(nodeId1, nodeId2))
            {
                _nodeToNodeRecordRepository.Create(new TNodeToNodeConnectorRecord() { Record1Id = nodeId1, Record2Id = nodeId2 });
            }
        }

        public List<string> GetSimilarTerms(string snippet)
        {
            return _nodePartRecordRepository.Fetch(node => node.Label.StartsWith(snippet)).Select(node => node.Label).ToList();
        }

        // Valami ContentItem leszármazott legyen a T NodePartRecord helyett?
        public UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>> MakeAssociations(IList<string> terms, bool simpleAlgorithm = false)
        {
            // Check cache
            var graph = new UndirectedGraph<TNodePart, UndirectedEdge<TNodePart>>();

            //graph.AddVerticesAndEdge(new UndirectedEdge<NodePartRecord>(1, 2));

            return graph;
        }

        private IList<int> GetNeighbourIds(int nodeId)
        {
            // Measure performance with large datasets, as .AsParallel() queries tend to be slower
            return _nodeToNodeRecordRepository.
                Fetch(connector => connector.Record1Id == nodeId || connector.Record2Id == nodeId).
                Select(connector => connector.Record1Id == nodeId ? connector.Record2Id : connector.Record1Id).ToList();
        }

        private bool AreConnected(int nodeId1, int nodeId2)
        {
            return _nodeToNodeRecordRepository.Count(connector =>
                connector.Record1Id == nodeId1 && connector.Record2Id == nodeId2 ||
                connector.Record1Id == nodeId2 && connector.Record2Id == nodeId1) != 0;
        }

        private int neighbourCount(int nodeId)
        {
            return _nodeToNodeRecordRepository.
                Count(connector => connector.Record1Id == nodeId || connector.Record2Id == nodeId);
        }

        private IEnumerable<TNodePart> GetSucceededNodes(List<List<int>> succeededPaths)
        {
            var succeededNodeIds = new List<int>();
            succeededPaths.ForEach(row => succeededNodeIds = succeededNodeIds.Union(row).ToList());
            _contentManager.Query<TNodePart, TNodePartRecord>();
            return _contentManager.Query<TNodePart, TNodePartRecord>().Where(node => succeededNodeIds.Contains(node.Id)).List();
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

        public List<List<int>> CalculatePaths(int startId, int targetId, int maxDepth = 3)
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
                    if (AreConnected(currentNode.Id, targetId))
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

                        foreach (var neighbourId in GetNeighbourIds(currentNode.Id))
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