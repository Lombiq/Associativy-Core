using QuickGraph;

namespace Associativy.Queryable
{
    public delegate dynamic ValueFactory<TNode>(IExecutionParams parameters);


    public interface IQueryableGraph<TNode>
    {
        ValueFactory<TNode> ValueFactory { get; } // Is this and copying necessary?
        IQueryParams Params { get; } 

        IQueryableGraph<TNode> Zoom(int level, int count);
        IQueryableGraph<TNode> SkipConnections(int count);
        IQueryableGraph<TNode> TakeConnections(int count);
        int NodeCount();
        int ConnectionCount();
        int ZoomLevelCount(int maximalZoomLevelCount);
        IUndirectedGraph<TNode, IUndirectedEdge<TNode>> ToGraph();
    }


    public static class QueryableGraphExtensions
    {
        public static void CopyParamsTo<TNode1, TNode2>(this IQueryableGraph<TNode1> graph, IQueryableGraph<TNode2> otherGraph)
        {
            graph.CopyParamsFrom(otherGraph.Params);
        }

        public static void CopyParamsFrom<TNode>(this IQueryableGraph<TNode> graph, IQueryParams parameters)
        {
            if (parameters.Zoom.IsSet()) graph.Zoom(parameters.Zoom.Level, parameters.Zoom.Count);
            graph.SkipConnections(parameters.Paging.SkipConnections);
            graph.TakeConnections(parameters.Paging.TakeConnections);
        }

        public static dynamic ExecuteWithParams<TNode>(this IQueryableGraph<TNode> graph, IExecutionParams parameters)
        {
            graph.CopyParamsFrom(parameters);

            switch (parameters.Method)
            {
                case ExecutionMethod.NodeCount:
                    return graph.NodeCount();
                case ExecutionMethod.ConnectionCount:
                    return graph.ConnectionCount();
                case ExecutionMethod.ZoomLevelCount:
                    return graph.ZoomLevelCount(parameters.Zoom.Count);
                case ExecutionMethod.ToGraph:
                    return graph.ToGraph();
                default:
                    return QueryableGraph.ThrowNotSupported(parameters.Method);
            }
        }

    }


    public interface IQueryParams
    {
        IZoom Zoom { get; }
        IPaging Paging { get; }
    }

    public enum ExecutionMethod
    {
        NodeCount,
        ConnectionCount,
        ZoomLevelCount,
        ToGraph
    }

    public interface IExecutionParams : IQueryParams
    {
        ExecutionMethod Method { get; }
    }

    public interface IZoom
    {
        int Level { get; }
        int Count { get; }
    }

    public static class ZoomExtensions
    {
        public static bool IsFlat(this IZoom zoom)
        {
            return zoom.Level == zoom.Count - 1;
        }

        public static bool IsSet(this IZoom zoom)
        {
            return zoom.Level >= 0 && zoom.Count >= 0;
        }
    }

    public interface IPaging
    {
        int SkipConnections { get; }
        int TakeConnections { get; }
    }
}
