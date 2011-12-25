using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Environment.Extensions;
using QuickGraph;
using System.Web.Mvc;
using Associativy.Models;
using Associativy.Models.Mind;
using Associativy.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using QuickGraph.Graphviz;

namespace Associativy.FrontendEngines.Engines.Graphviz.Controllers
{
    [OrchardFeature("Associativy")]
    public abstract class AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : Associativy.FrontendEngines.Controllers.AssociationsController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TAssocociativyServices : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
    {
        protected IGraphvizDriver<TNodePart> _graphvizDriver;

        protected override string FrontendEngineDriver
        {
            get { return "Graphviz"; }
        }

        protected AssociationsController(
            TAssocociativyServices associativyService,
            IOrchardServices orchardServices,
            IGraphvizDriver<TNodePart> graphvizDriver)
            : base(associativyService, orchardServices, graphvizDriver)
        {
            _graphvizDriver = graphvizDriver;
        }

        public virtual void Render()
        {
            var graph = _associativyServices.Mind.GetAllAssociations();
            var renderGraph = new EdgeListGraph<TNodePart, IEdge<TNodePart>>(false, false);
            var graphviz = graph.ToGraphviz(algorithm =>
                {
                });

            var svg = graph.ToSvg(algorithm =>
            {
                algorithm.FormatVertex +=
                    (sender, e) =>
                    {
                        e.VertexFormatter.Label = e.Vertex.Label;
                        e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.Diamond;
                        e.VertexFormatter.Url = "http://pyrocenter.hu";
                    };
            });
            int z = 5 + 5;
        }
    }
}