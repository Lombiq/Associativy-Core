using Orchard.Localization;
using Associativy.Services;
using Piedone.HelpfulLibraries.DependencyInjection;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    /// <summary>
    /// Describes the context in which Associativy services are run, i.e. it stores information about the purpose and database
    /// of associations
    /// </summary>
    [OrchardFeature("Associativy")]
    public abstract class AssociativyContext : IAssociativyContext
    {
        protected LocalizedString _graphName;
        public LocalizedString GraphName
        {
            get { return _graphName; }
        }

        protected string _technicalGraphName;
        public string TechnicalGraphName
        {
            get { return _technicalGraphName; }
        }

        protected string[] _contentTypeNames;
        public string[] ContentTypeNames
        {
            get { return _contentTypeNames; }
        }

        protected int _maxZoomLevel = 10;
        public int MaxZoomLevel
        {
            get { return _maxZoomLevel; }
        }

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