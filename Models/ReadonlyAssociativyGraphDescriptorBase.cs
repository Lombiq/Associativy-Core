using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public abstract class ReadonlyAssociativyGraphDescriptorBase<TNodeToNodeConnectorRecord> : AssociativyGraphDescriptorBase<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        public LocalizedString GraphName { get; protected set; }
        public string TechnicalGraphName { get; protected set; }
        public string[] ContentTypes { get; protected set; }

        public abstract IConnectionManager ConnectionManager
        {
            get;
        }

        public Localizer T { get; set; }

        public ReadonlyAssociativyGraphDescriptorBase(IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> connectionManagerResolver)
            : base(connectionManagerResolver)
        {
            T = NullLocalizer.Instance;
        }
    }
}