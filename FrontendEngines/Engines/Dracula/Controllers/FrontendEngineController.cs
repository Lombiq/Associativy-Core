using Associativy.FrontendEngines.Controllers;
using Associativy.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.Shapes;

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
            IFrontendShapes frontendShapes,
            IShapeFactory shapeFactory)
            : base(associativyServices, orchardServices, frontendShapes, shapeFactory)
        {
        }
    }
}