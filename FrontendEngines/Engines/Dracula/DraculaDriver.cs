﻿using Associativy.Models;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using System;

namespace Associativy.FrontendEngines.Engines.Dracula
{
    [OrchardFeature("Associativy")]
    public class DraculaDriver<TNode> : FrontendEngineDriver<TNode>, IDraculaDriver<TNode>
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