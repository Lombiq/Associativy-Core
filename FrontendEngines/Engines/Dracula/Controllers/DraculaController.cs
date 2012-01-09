using Associativy.FrontendEngines.Controllers;
using Associativy.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.Shapes;
using Associativy.FrontendEngines.Services;

namespace Associativy.FrontendEngines.Engines.Dracula.Controllers
{
    [OrchardFeature("Associativy")]
    public class DraculaController : FrontendEngineBaseController, IDiscoverableFrontendEngineController
    {
        protected override string FrontendEngine
        {
            get { return "Dracula"; }
        }

        public DraculaController(
            IAssociativyServices associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes frontendShapes,
            IShapeFactory shapeFactory,
            IGraphFilterer graphFilterer)
            : base(associativyServices, orchardServices, frontendShapes, shapeFactory, graphFilterer)
        {
        }
    }
}