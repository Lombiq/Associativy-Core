﻿using System;
using System.Linq;
using Associativy.GraphDiscovery;
using Associativy.Services;
using QuickGraph;

namespace Associativy.Queryable
{
    public static class QueryableGraphHelper
    {
        public static dynamic LastStepsWithPaging(LastStepParams parameters)
        {
            var paging = parameters.ExecutionParameters.Paging;
            if (paging.SkipConnections != 0 || paging.TakeConnections < parameters.Graph.EdgeCount)
            {
                var graph = parameters.Graph;
                var pagedGraph = parameters.GraphEditor.GraphFactory<int>();
                pagedGraph.AddVerticesAndEdgeRange(graph.Edges
                    .OrderByDescending(edge => Math.Min(graph.AdjacentDegree(edge.Source), graph.AdjacentDegree(edge.Target)))
                    .Skip(paging.SkipConnections)
                    .Take(paging.TakeConnections));
                parameters.Graph = pagedGraph;
            }

            return LastSteps(parameters);
        }

        public static dynamic LastSteps(LastStepParams parameters)
        {
            var method = parameters.ExecutionParameters.Method;
            var zoom = parameters.ExecutionParameters.Zoom;
            var graph = parameters.Graph;

            if (zoom.IsSet() && !zoom.IsFlat())
            {
                graph = parameters.CacheService.GetMonitored(
                    parameters.GraphDescriptor,
                    MakeCacheKey(parameters.BaseCacheKey + ".ZoomedGraph", parameters.ExecutionParameters, true),
                    () => parameters.GraphEditor.CreateZoomedGraph(graph, zoom.Level, zoom.Count));
            }

            switch (method)
            {
                case ExecutionMethod.NodeCount:
                    return graph.VertexCount;
                case ExecutionMethod.ConnectionCount:
                    return graph.EdgeCount;
                case ExecutionMethod.ZoomLevelCount:
                    return parameters.CacheService.GetMonitored(
                        parameters.GraphDescriptor, MakeCacheKey(parameters.BaseCacheKey + ".ZoomLevelCount", parameters.ExecutionParameters, true),
                        () => parameters.GraphEditor.CalculateZoomLevelCount(graph, zoom.Count));
                case ExecutionMethod.ToGraph:
                    return graph;
                default:
                    return QueryableGraph.ThrowNotSupported(method);
            }
        }


        public static string MakeCacheKey(string name, IExecutionParams parameters, bool includeZoom = false)
        {
            var paging = parameters.Paging;
            var zoom = parameters.Zoom;

            var key = name + ".Params:" + paging.SkipConnections + "/" + paging.TakeConnections;
            if (includeZoom) key += "/" + zoom.Level + "/" + zoom.Count;

            return key;
        }

    }

    public class LastStepParams
    {
        public IGraphCacheService CacheService { get; set; }
        public IGraphEditor GraphEditor { get; set; }
        public IGraphDescriptor GraphDescriptor { get; set; }
        public IExecutionParams ExecutionParameters { get; set; }
        public IUndirectedGraph<int, IUndirectedEdge<int>> Graph { get; set; }
        public string BaseCacheKey { get; set; }
        public bool UseCache { get; set; }
    }
}