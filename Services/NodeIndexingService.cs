using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Indexing;
using Orchard.Indexing.Services;
using Orchard.Indexing.Settings;

namespace Associativy.Services
{
    public class NodeIndexingService : INodeIndexingService
    {
        private readonly IIndexManager _indexManager;
        private readonly IIndexingService _indexingService;
        private readonly IGraphManager _graphManager;
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;


        public NodeIndexingService(
            IIndexManager indexManager,
            IIndexingService indexingService,
            IGraphManager graphManager,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager)
        {
            _indexManager = indexManager;
            _indexingService = indexingService;
            _graphManager = graphManager;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
        }


        public bool IsIndexingSetupForGraph(string graphName)
        {
            var provider = GetIndexProvider();

            if (provider == null) return false;

            var indexName = IndexNameForGraph(graphName);

            // TODO: cache this
            if (!provider.Exists(indexName)) return false;

            foreach (var type in _graphManager.FindGraphByName(graphName).ContentTypes)
            {
                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(type);
                if (typeDefinition == null) continue;
                var indexingSettings = typeDefinition.Settings.TryGetModel<TypeIndexing>();

                if (indexingSettings == null || !indexingSettings.List.Contains(indexName)) return false;
            }

            return true;
        }

        public void SetupIndexingForGraph(string graphName)
        {
            if (IsIndexingSetupForGraph(graphName)) return;

            var provider = GetIndexProvider();

            if (provider == null) throw new InvalidOperationException("No search index provider was found. Thus indexing can't be set up for the graph " + graphName + ".");

            var indexName = IndexNameForGraph(graphName);
            provider.CreateIndex(indexName);

            foreach (var type in _graphManager.FindGraphByName(graphName).ContentTypes)
            {
                var settings = _contentDefinitionManager.GetTypeDefinition(type).Settings;
                var indexingSettings = settings.TryGetModel<TypeIndexing>();

                if (indexingSettings == null)
                {
                    _contentDefinitionManager.AlterTypeDefinition(type,
                        cfg => cfg
                            .WithSetting("TypeIndexing.Indexes", indexName)
                        );
                }
                else
                {
                    _contentDefinitionManager.AlterTypeDefinition(type,
                        cfg => cfg
                            .WithSetting("TypeIndexing.Indexes", indexingSettings.Indexes + "," + indexName)
                        );
                }
            }

            _indexingService.RebuildIndex(IndexNameForGraph(graphName));
        }

        public void RemoveIndexForGraph(string graphName)
        {
            var provider = GetIndexProvider();

            if (provider == null) throw new InvalidOperationException("No search index provider was found. Thus index can't be deleted for the graph " + graphName + ".");

            var indexName = IndexNameForGraph(graphName);
            if (!provider.Exists(indexName)) return;
            provider.DeleteIndex(indexName);
        }

        public void IndexNodesForGraph(string graphName, IEnumerable<IContent> nodes)
        {
            if (!IsIndexingSetupForGraph(graphName)) return;

            var indexProvider = GetIndexProvider();

            var documents = new List<IDocumentIndex>();

            foreach (var node in nodes)
            {
                var document = indexProvider.New(node.ContentItem.Id);
                _contentManager.Index(node.ContentItem, document);
                documents.Add(document);
            }

            indexProvider.Store(IndexNameForGraph(graphName), documents);
        }

        public ISearchBuilder GetSearchBuilder(string graphName)
        {
            if (!IsIndexingSetupForGraph(graphName)) return null;

            return GetIndexProvider().CreateSearchBuilder(IndexNameForGraph(graphName));
        }


        private IIndexProvider GetIndexProvider()
        {
            if (!_indexManager.HasIndexProvider()) return null;
            return _indexManager.GetSearchIndexProvider();
        }


        private static string IndexNameForGraph(string graphName)
        {
            return "Associativy_" + graphName;
        }
    }
}