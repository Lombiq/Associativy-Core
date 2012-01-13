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
        protected readonly IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> _connectionManagerResolver;

        public override IConnectionManager ConnectionManager
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