using Associativy.Models;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.Engines.Dracula
{
    [OrchardFeature("Associativy")]
    public class DraculaDriver<TNode> : FrontendEngineDriver<TNode>
        where TNode : INode
    {
        protected override string Name
        {
            get { return "Dracula"; }
        }

        public DraculaDriver(
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor)
            : base(orchardServices, shapeFactory, workContextAccessor)
        {
        }
    }
}