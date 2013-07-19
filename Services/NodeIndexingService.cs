using System;
using System.Linq;
using Associativy.GraphDiscovery;
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
        private readonly IContentDefinitionManager _contentDefinitionManager;


        public NodeIndexingService(
            IIndexManager indexManager,
            IIndexingService indexingService,
            IGraphManager graphManager,
            IContentDefinitionManager contentDefinitionManager)
        {
            _indexManager = indexManager;
            _indexingService = indexingService;
            _graphManager = graphManager;
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
                if (typeDefinition == null) throw new ApplicationException("The type definition for the type \"" + type + "\" in the Associativy graph \"" + graphName + "\" was not found.");
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
            return "Associativy" + graphName;
        }
    }
}