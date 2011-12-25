using Associativy.Models;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.ViewModels;
using Associativy.FrontendEngines.Engines.JIT.ViewModels;
using Piedone.HelpfulLibraries.Serialization;
using QuickGraph;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Associativy.FrontendEngines.Engines.JIT
{
    [OrchardFeature("Associativy")]
    public class JITDriver<TNode> : FrontendEngineDriver<TNode>, IJITDriver<TNode>
        where TNode : INode
    {
        private readonly ISimpleSerializer _simpleSerializer;

        protected override string Name
        {
            get { return "JIT"; }
        }

        protected override string SearchResultShapeTemplateName
        {
            get { return "FrontendEngines/Engines/JIT/SearchResult"; }
        }

        public JITDriver(
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor,
            ISimpleSerializer simpleSerializer)
            : base(orchardServices, shapeFactory, workContextAccessor)
        {
            _simpleSerializer = simpleSerializer;
        }

        public override dynamic GraphShape(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
        {
            return null;
        }

        public string GraphJson(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph)
        {
            IEnumerable<string> jsonNodes = from node in BuildViewNodes<IJITGraphNodeViewModel<TNode>>(graph).Values
                                            select _simpleSerializer.JsonSerialize(node);

            return "[" + String.Join(",", jsonNodes) + "]";
        }
    }
}