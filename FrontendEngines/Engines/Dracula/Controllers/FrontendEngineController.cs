using Associativy.FrontendEngines.Controllers;
using Associativy.Services;
using Associativy.Shapes;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.Engines.Dracula.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineController : FrontendEngineBaseController, IDiscoverableFrontendEngineController
    {
        protected override string FrontendEngine
        {
            get { return "Dracula"; }
        }

        public FrontendEngineController(
            IAssociativyServices associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes shapes,
            IShapeFactory shapeFactory)
            : base(associativyServices, orchardServices, shapes, shapeFactory)
        {
        }
    }
}