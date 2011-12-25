using Associativy.Models;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using System;

namespace Associativy.FrontendEngines.Engines.Graphviz
{
    [OrchardFeature("Associativy")]
    public class GraphvizDriver<TNode> : FrontendEngineDriver<TNode>, IGraphvizDriver<TNode>
        where TNode : INode
    {
        protected override string Name
        {
            get { return "Graphviz"; }
        }

        public GraphvizDriver(
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor)
            : base(orchardServices, shapeFactory, workContextAccessor)
        {
        }
    }
}