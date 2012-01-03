using Associativy.FrontendEngines.Controllers;
using Associativy.Models;
using Associativy.Services;
using Associativy.Shapes;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.Engines.Dracula.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        : FrontendEngineBaseController, IDiscoverableFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        protected override string FrontendEngine
        {
            get { return "Dracula"; }
        }

        public FrontendEngineController(
            IAssociativyServices<TNodeToNodeConnectorRecord, TAssociativyContext> associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes shapes,
            IShapeFactory shapeFactory)
            : base(associativyServices, orchardServices, shapes, shapeFactory)
        {
        }
    }
}