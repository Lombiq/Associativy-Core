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
        public virtual string GraphName { get; protected set; }
        public virtual LocalizedString DisplayName { get; protected set; }
        public virtual string[] ContentTypes { get; protected set; }

        protected readonly IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> _connectionManagerResolver;
        public IConnectionManager ConnectionManager
        {
            get
            {
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
    }
}