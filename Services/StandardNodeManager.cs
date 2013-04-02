using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Indexing;
using QuickGraph;

namespace Associativy.Services
{
    public interface IStandardNodeManager : INodeManager, IGraphAwareService, ITransientDependency
    {
    }

    public class StandardNodeManager : GraphAwareServiceBase, IStandardNodeManager
    {
        protected readonly IContentManager _contentManager;
        protected readonly IGraphEditor _graphEditor;
        protected readonly INodeIndexingService _indexingService;
        protected readonly INodeManagerEventHander _eventHandler;


        public StandardNodeManager(
            IGraphDescriptor graphDescriptor,
            IContentManager contentManager,
            IGraphEditor graphEditor,
            INodeIndexingService indexingService,
            INodeManagerEventHander eventHandler)
            : base(graphDescriptor)
        {
            _contentManager = contentManager;
            _graphEditor = graphEditor;
            _indexingService = indexingService;
            _eventHandler = eventHandler;
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

        public virtual IContentQuery<ContentItem> GetBySimilarLabelQuery(string labelSnippet)
        {
            // The max count is hard-coded now but should be somehow configurable
            return GetSearchHitQuery(_indexingService.Search(_graphDescriptor.Name, labelSnippet + "*", 50));
        }

        public virtual IContentQuery<ContentItem> GetByLabelQuery(params string[] labels)
        {
            var hits = new List<ISearchHit>();

            foreach (var label in labels)
            {
                var hit = _indexingService.SearchExact(_graphDescriptor.Name, label).FirstOrDefault();
                if (hit != null) hits.Add(hit);
            }

            return GetSearchHitQuery(hits);
        }


        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            var nodes = GetManyQuery(idGraph.Vertices).List().ToDictionary(node => node.Id);

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


        private IContentQuery<ContentItem> GetSearchHitQuery(IEnumerable<ISearchHit> hits)
        {
            if (!hits.Any()) return GetQuery().Where<CommonPartRecord>(record => false).ForPart<ContentItem>();
            return GetManyQuery(hits.Select(hit => hit.ContentItemId));
        }
    }
}