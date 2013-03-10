using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;
using QuickGraph;

namespace Associativy.Queryable
{
    public static class GraphExtensions
    {
        public static IUndirectedGraph<IContent, IUndirectedEdge<IContent>> ToContentGraph(this IUndirectedGraph<int, IUndirectedEdge<int>> idGraph, IGraphDescriptor graphDescriptor)
        {
            return graphDescriptor.Services.NodeManager.MakeContentGraph(idGraph);
        }
    }
}