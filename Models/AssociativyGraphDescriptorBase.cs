using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public abstract class AssociativyGraphDescriptorBase<TNodeToNodeConnectorRecord> : IAssociativyGraphDescriptor
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        public virtual LocalizedString GraphName { get; protected set; }
        public virtual string TechnicalGraphName { get; protected set; }
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

        public AssociativyGraphDescriptorBase(IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> connectionManagerResolver)
        {
            _connectionManagerResolver = connectionManagerResolver;
        }

        
    }
}