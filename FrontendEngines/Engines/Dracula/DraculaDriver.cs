using Associativy.Models;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using System;

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

        public override string GraphJson(QuickGraph.IUndirectedGraph<TNode, QuickGraph.IUndirectedEdge<TNode>> graph)
        {
            throw new NotImplementedException();
        }
    }
}