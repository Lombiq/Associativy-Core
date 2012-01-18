using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;
using Associativy.GraphDescription;
using System.Collections.Generic;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public abstract class GraphDescriptorBase<TNodeToNodeConnectorRecord> : IGraphDescriptor
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
                connectionManager.GraphDescriptor = this;
                return connectionManager;
            }
        }

        IGraphContext SupportedGraphContext { get; protected set; }
        IEnumerable<IContentContext> SupportedContentContexts { get; protected set; }

        public Localizer T { get; set; }

        public GraphDescriptorBase(IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> connectionManagerResolver)
        {
            _connectionManagerResolver = connectionManagerResolver;

            T = NullLocalizer.Instance;
        }
    }
}