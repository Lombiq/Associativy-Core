using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Associativy.Models
{
    /// <summary>
    /// Describes the context in which Associativy services are run, i.e. it stores information about the purpose and database
    /// of associations
    /// </summary>
    [OrchardFeature("Associativy")]
    public abstract class AssociativyContext : IAssociativyContext
    {
        public LocalizedString GraphName { get; protected set; }
        public string TechnicalGraphName { get; protected set; }
        public string[] ContentTypes { get; protected set; }

        public abstract IConnectionManager ConnectionManager
        {
            get;
        }

        public Localizer T { get; set; }

        public AssociativyContext()
        {
            T = NullLocalizer.Instance;
        }
    }

    [OrchardFeature("Associativy")]
    public abstract class AssociativyContext<TNodeToNodeConnectorRecord> : AssociativyContext
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected readonly IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> _connectionManagerResolver;

        public override IConnectionManager ConnectionManager
        {
            get
            {
                var connectionManager = _connectionManagerResolver.Value;
                connectionManager.Context = this;
                return connectionManager;
            }
        }

        public AssociativyContext(IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> connectionManagerResolver)
            : base()
        {
            _connectionManagerResolver = connectionManagerResolver;
        }
    }
}