using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;
using Associativy.GraphDiscovery;
using System.Collections.Generic;
using Associativy.Models;
using System.Diagnostics;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public abstract class GraphProviderBase<TNodeToNodeConnectorRecord> : IGraphProvider
        where TNodeToNodeConnectorRecord : INodeToNodeConnector, new()
    {
        public virtual string GraphName { get; protected set; }
        public virtual LocalizedString DisplayGraphName { get; protected set; }
        public virtual IEnumerable<string> ContentTypes { get; protected set; }

        private readonly IResolve<IDatabaseConnectionManager<TNodeToNodeConnectorRecord>> _connectionManagerResolver;
        public virtual IConnectionManager ConnectionManager
        {
            get
            {
                return _connectionManagerResolver.Value;
            }
        }

        public Localizer T { get; set; }

        public GraphProviderBase(IResolve<IDatabaseConnectionManager<TNodeToNodeConnectorRecord>> connectionManagerResolver)
        {
            _connectionManagerResolver = connectionManagerResolver;

            T = NullLocalizer.Instance;
        }
    }
}