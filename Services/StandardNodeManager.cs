using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using QuickGraph;

namespace Associativy.Services
{
    public interface IStandardNodeManager : INodeManager, IGraphAwareService, ITransientDependency
    {
    }

    public class StandardNodeManager : GraphAwareServiceBase, IStandardNodeManager
    {
        protected readonly IContentManager _contentManager;
        protected readonly INodeManagerEventHander _eventHandler;
        protected readonly IGraphEditor _graphEditor;


        public StandardNodeManager(
            IGraphDescriptor graphDescriptor,
            IContentManager contentManager,
            INodeManagerEventHander eventHandler,
            IGraphEditor graphEditor)
            : base(graphDescriptor)
        {
            _contentManager = contentManager;
            _eventHandler = eventHandler;
            _graphEditor = graphEditor;
        }


        public IContentQuery<ContentItem> GetQuery()
        {
            var query = _contentManager.Query(_graphDescriptor.ContentTypes.ToArray());
            _eventHandler.QueryBuilt(new QueryBuiltContext(_graphDescriptor, query));
            return query;
        }

        public IContentQuery<ContentItem> GetManyQuery(IEnumerable<int> ids)
        {
            // Otherwise an exception with message "Expression argument must be of type ICollection." is thrown from
            // Orchard.ContentManagement.DefaultContentQuery on line 90.
            var idsList = ids.ToList();
            return GetQuery().Where<CommonPartRecord>(r => idsList.Contains(r.Id));
        }

        public virtual IContentQuery<ContentItem> GetSimilarNodesQuery(string labelSnippet)
        {
            labelSnippet = labelSnippet.ToUpperInvariant();
            return GetQuery().Where<AssociativyNodeLabelPartRecord>(r => r.UpperInvariantLabel.StartsWith(labelSnippet));
        }


        public virtual IContentQuery<ContentItem> GetManyByLabelQuery(IEnumerable<string> labels)
        {
            var labelsArray = labels.ToArray();
            for (int i = 0; i < labelsArray.Length; i++)
            {
                labelsArray[i] = labelsArray[i].ToUpperInvariant();
            }

            return GetQuery().Where<AssociativyNodeLabelPartRecord>(r => labelsArray.Contains(r.UpperInvariantLabel));
        }

        public virtual IContentQuery<ContentItem> GetByLabelQuery(string label)
        {
            label = label.ToUpperInvariant();
            return GetQuery().Where<AssociativyNodeLabelPartRecord>(r => r.UpperInvariantLabel == label);
        }


        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            var query = GetManyQuery(idGraph.Vertices);
            var nodes = query.List().ToDictionary(node => node.Id);

            var graph = _graphEditor.GraphFactory<IContent>();
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
    }
}