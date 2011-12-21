using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.DisplayManagement;
using Associativy.FrontendEngines;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.ViewModels;
using Orchard.ContentManagement;
using QuickGraph;
using Associativy.Models;

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