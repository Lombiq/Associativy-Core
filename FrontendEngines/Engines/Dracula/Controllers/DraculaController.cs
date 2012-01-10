using Associativy.FrontendEngines.Controllers;
using Associativy.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.Shapes;
using Associativy.FrontendEngines.Services;
using Associativy.FrontendEngines.Engines.Dracula.Models;

namespace Associativy.FrontendEngines.Engines.Dracula.Controllers
{
    [OrchardFeature("Associativy")]
    public class DraculaController : FrontendEngineBaseController
    {
        protected readonly IDraculaSetup _setup;

        protected override string FrontendEngine
        {
            get { return "Dracula"; }
        }

        public DraculaController(
            IAssociativyServices associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes frontendShapes,
            IShapeFactory shapeFactory,
            IGraphFilterer graphFilterer,
            IDraculaSetup setup)
            : base(associativyServices, orchardServices, frontendShapes, shapeFactory, graphFilterer, setup)
        {
            _setup = setup;
        }
    }
}