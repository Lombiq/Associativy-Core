using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public abstract class ReadonlyGraphDescriptorBase<TNodeToNodeConnectorRecord> : GraphDescriptorBase<TNodeToNodeConnectorRecord>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        public Localizer T { get; set; }

        public ReadonlyGraphDescriptorBase(IResolve<IConnectionManager<TNodeToNodeConnectorRecord>> connectionManagerResolver)
            : base(connectionManagerResolver)
        {
            T = NullLocalizer.Instance;
        }
    }
}