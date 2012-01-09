using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using QuickGraph;
using Orchard.ContentManagement;
using Associativy.FrontendEngines.NodeFilters;
using Piedone.HelpfulLibraries.DependencyInjection;
using Associativy.FrontendEngines.ViewModels;
using System.Diagnostics;

namespace Associativy.FrontendEngines.Services
{
    [OrchardFeature("Associativy")]
    public class GraphFilterer : IGraphFilterer
    {
        protected readonly IEnumerable<INodeFilter> _nodeFilters;
        protected readonly IResolve<INodeViewModel> _viewModelResolve;

        public GraphFilterer(IEnumerable<INodeFilter> nodeFilters, IResolve<INodeViewModel> viewModelResolve)
        {
            _nodeFilters = nodeFilters;
            _viewModelResolve = viewModelResolve;
        }

        public IDictionary<int, INodeViewModel> RunFilters(IUndirectedGraph<IContent, IUndirectedEdge<IContent>> graph, string frontendEngine)
        {
            var filters = _nodeFilters.ToList();
            filters.Sort();

            var models = new Dictionary<int, INodeViewModel>(graph.VertexCount);
            
            foreach (var node in graph.Vertices)
            {
                var viewModel = _viewModelResolve.Value;
                viewModel.ContentItem = node;

                foreach (var filter in filters)
                {
                    viewModel = filter.Apply(node, viewModel, frontendEngine);
                }

                models[node.Id] = viewModel;
            }
            
            return models;
        }
    }
}