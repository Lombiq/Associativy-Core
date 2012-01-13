using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Associativy.Models
{
    /// <summary>
    /// Describes the graphDescriptor in which Associativy services are run, i.e. it stores information about the purpose and database
    /// of associations
    /// </summary>
    [OrchardFeature("Associativy")]
    public abstract class AssociativyGraphDescriptorBase : IAssociativyGraphDescriptor
    {
        public LocalizedString GraphName { get; protected set; }
        public string TechnicalGraphName { get; protected set; }
        public string[] ContentTypes { get; protected set; }

        public abstract IConnectionManager ConnectionManager
        {
            get;
        }

        public Localizer T { get; set; }

        public AssociativyGraphDescriptorBase()
        {
            T = NullLocalizer.Instance;
        }
    }

    [OrchardFeature("Associativy")]
    public abstract class AssociativyGraphDescriptorBase<TNodeToNodeConnectorRecord> : AssociativyGraphDescriptorBase
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
            : base()
        {
            _connectionManagerResolver = connectionManagerResolver;
        }
    }
}