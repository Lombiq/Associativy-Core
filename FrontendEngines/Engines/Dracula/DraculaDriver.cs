using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Associativy.ViewModels;
using Orchard.DisplayManagement;
using Associativy.FrontendEngines;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.ViewModels;
using Orchard.ContentManagement;
using QuickGraph;
using Associativy.Models;

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
    }
}