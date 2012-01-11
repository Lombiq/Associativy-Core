using Associativy.FrontendEngines.Controllers;
using Associativy.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.Shapes;
using Associativy.FrontendEngines.Engines.Dracula.Models;
using QuickGraph;
using Orchard.ContentManagement;
using Associativy.FrontendEngines.Engines.Dracula.ViewModels;
using System.Collections.Generic;

namespace Associativy.FrontendEngines.Engines.Dracula.Controllers
{
    [OrchardFeature("Associativy")]
    public class DraculaFrontendEngineController : FrontendEngineBaseController
    {
        protected readonly IDraculaSetup _setup;

        protected override string FrontendEngine
        {
            get { return "Dracula"; }
        }

        public DraculaFrontendEngineController(
            IAssociativyServices associativyServices,
            IOrchardServices orchardServices,
            IFrontendShapes frontendShapes,
            IShapeFactory shapeFactory,
            IDraculaSetup setup)
            : base(associativyServices, orchardServices, frontendShapes, shapeFactory, setup)
        {
            _setup = setup;
        }

        protected override dynamic GraphShape(IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph)
        {
            var nodes = new Dictionary<int, NodeViewModel>(graph.VertexCount);

            foreach (var node in graph.Vertices)
            {
                nodes[node.Id] = _setup.SetViewModel(node, new NodeViewModel() { ContentItem = node });
            }

            return GraphShape(new GraphViewModel() { Graph = graph, Nodes = nodes });
        }
    }
}