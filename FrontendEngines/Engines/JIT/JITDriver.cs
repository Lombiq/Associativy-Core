using Associativy.Models;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.Engines.JIT
{
    [OrchardFeature("Associativy")]
    public class JITDriver<TNode> : FrontendEngineDriver<TNode>
        where TNode : INode
    {
        protected override string Name
        {
            get { return "JIT"; }
        }

        public JITDriver(
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor)
            : base(orchardServices, shapeFactory, workContextAccessor)
        {
        }
    }
}