using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;
using Associativy.GraphDiscovery;
using System.Collections.Generic;
using Associativy.Models;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public abstract class GraphProviderBase<TNodeToNodeConnectorRecord> : IGraphProvider
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> _connectionManagerResolver;
        protected IConnectionManager ConnectionManager
        {
            get
            {
                // Cache the resolved service maybe?
                var connectionManager = _connectionManagerResolver.Value;
                return connectionManager;
            }
        }

        public Localizer T { get; set; }

        public GraphProviderBase(IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> connectionManagerResolver)
        {
            _connectionManagerResolver = connectionManagerResolver;

            T = NullLocalizer.Instance;
        }

        public virtual void Describe(GraphDescriptor descriptor)
        {
            descriptor.ConnectionManager = ConnectionManager;
        }
    }
}