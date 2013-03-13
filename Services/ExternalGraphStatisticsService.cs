using System.Linq;
using Associativy.GraphDiscovery;
using Associativy.Models;
using Associativy.Models.Services;
using Orchard.Caching.Services;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy.GraphStatisticsServices")]
    public class ExternalGraphStatisticsService : GraphAwareServiceBase, IExternalGraphStatisticsService
    {
        private readonly ICacheService _cacheService;
        private readonly IRepository<GraphStatisticsRecord> _repository;

        private const string CacheKeyPrefix = "Associativy.GraphStatistics.";
        private const string GraphInfoCacheKey = CacheKeyPrefix + "GraphInfo";


        public ExternalGraphStatisticsService(
            IGraphDescriptor graphDescriptor,
            ICacheService cacheService,
            IRepository<GraphStatisticsRecord> repository)
            : base(graphDescriptor)
        {
            _cacheService = cacheService;
            _repository = repository;
        }


        public IGraphInfo GetGraphInfo()
        {
            return _cacheService.Get(GraphInfoCacheKey, () =>
                {
                    var record = GetOrCreateGraphInfoRecord();

                    return new GraphInfo
                    {
                        NodeCount = record.NodeCount,
                        ConnectionCount = record.ConnectionCount
                    };
                });
        }

        public void SetNodeCount(int count)
        {
            GetOrCreateGraphInfoRecord().NodeCount = count;
            InvalidateGraphInfoCache();
        }

        public void SetConnectionCount(int count)
        {
            GetOrCreateGraphInfoRecord().ConnectionCount = count;
            InvalidateGraphInfoCache();
        }

        public void SetCentralNodeId(int id)
        {
            GetOrCreateGraphInfoRecord().CentralNodeId = id;
            InvalidateGraphInfoCache();
        }


        private GraphStatisticsRecord GetOrCreateGraphInfoRecord()
        {
            var record = _repository.Table.Where(r => r.GraphName == _graphDescriptor.Name).SingleOrDefault();

            if (record == null)
            {
                record = new GraphStatisticsRecord { GraphName = _graphDescriptor.Name };
                _repository.Create(record);
            }

            return record;
        }

        private void InvalidateGraphInfoCache()
        {
            _cacheService.Remove(GraphInfoCacheKey);
            _repository.Flush();
        }


        private class GraphInfo : IGraphInfo
        {
            public int NodeCount { get; set; }
            public int ConnectionCount { get; set; }
            public int CentralNodeId { get; set; }
        }
    }
}