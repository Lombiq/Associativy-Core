using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Associativy.EventHandlers;
using Associativy.GraphDiscovery;
using Orchard;
using Orchard.ContentManagement;
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

        public virtual IContentQuery<ContentItem> GetBySimilarLabelQuery(string labelSnippet)
        {
            // The max count is hard-coded now but should be somehow configurable
            return GetSearchHitQuery(_indexingService.Search(_graphDescriptor.Name, EscapeSearchCharacters(labelSnippet.Trim()) + "*", 50));
        }

        public virtual IContentQuery<ContentItem> GetByLabelQuery(params string[] labels)
        {
            var hits = new List<ISearchHit>();

            foreach (var label in labels)
            {
                var subHits = _indexingService.SearchExact(_graphDescriptor.Name, EscapeSearchCharacters(label));
                foreach (var subHit in subHits)
                {
                    if (subHit != null && string.Compare(subHit.GetString("nodeLabel"), label, System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        hits.Add(subHit);
                    } 
                }
            }

            return GetSearchHitQuery(hits);
        }


        public virtual IUndirectedGraph<IContent, IUndirectedEdge<IContent>> MakeContentGraph(IUndirectedGraph<int, IUndirectedEdge<int>> idGraph)
        {
            var nodes = GetQuery().ForContentItems(idGraph.Vertices).List().ToDictionary(node => node.Id);

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
            if (!hits.Any()) return _contentManager.Query("ősőőfőwőeoeworőőeŰŰŰÍÍÍíűrőooeoerő"); // An empty query
            return GetQuery().ForContentItems(hits.Select(hit => hit.ContentItemId));
        }


        private static string EscapeSearchCharacters(string query)
        {
            // Escapes Lucene special characters (see http://lucene.apache.org/core/2_9_4/queryparsersyntax.html#Escaping%20Special%20Characters)
            // without directly depending on Lucene. This is a workaround.
            return Regex.Replace(query, @"((\&\&)|(\|\|)|([!(){}[\]^\\""~*?:]))", match => "\\" + match.Groups[1]);
        }
    }
}