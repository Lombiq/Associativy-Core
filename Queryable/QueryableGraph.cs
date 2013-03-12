using System;
using QuickGraph;

namespace Associativy.Queryable
{
    public abstract class QueryableGraph
    {
        public static dynamic ThrowNotSupported(ExecutionMethod method)
        {
            throw new NotSupportedException("The operation " + method + " was not supported for this query.");
        }
    }

    public class QueryableGraph<TNode> : QueryableGraph, IQueryableGraph<TNode>
    {
        private readonly ValueFactory<TNode> _valueFactory;
        private readonly ExecutionParamsImpl _executionParams;

        public ValueFactory<TNode> ValueFactory { get { return _valueFactory; } }
        public IQueryParams Params { get { return _executionParams; } }


        public QueryableGraph(ValueFactory<TNode> valueFactory)
        {
            _valueFactory = valueFactory;
            _executionParams = new ExecutionParamsImpl();
        }


        public IQueryableGraph<TNode> Zoom(int level, int count)
        {
            _executionParams.ZoomImpl.Level = level;
            _executionParams.ZoomImpl.Count = count;

            return this;
        }

        public IQueryableGraph<TNode> SkipConnections(int count)
        {
            _executionParams.PagingImpl.SkipConnections = count;

            return this;
        }

        public IQueryableGraph<TNode> TakeConnections(int count)
        {
            _executionParams.PagingImpl.TakeConnections = count;

            return this;
        }

        public int NodeCount()
        {
            _executionParams.Method = ExecutionMethod.NodeCount;

            return Value();
        }

        public int ConnectionCount()
        {
            _executionParams.Method = ExecutionMethod.ConnectionCount;

            return Value();
        }

        public int ZoomLevelCount(int maximalZoomLevelCount)
        {
            _executionParams.Method = ExecutionMethod.ZoomLevelCount;
            _executionParams.ZoomImpl.Count = maximalZoomLevelCount;

            return Value();
        }

        public IUndirectedGraph<TNode, IUndirectedEdge<TNode>> ToGraph()
        {
            _executionParams.Method = ExecutionMethod.ToGraph;

            return Value();
        }


        private dynamic Value()
        {
            return _valueFactory(_executionParams);
        }


        private class ExecutionParamsImpl : IExecutionParams
        {
            public ExecutionMethod Method { get; set; }

            public ZoomImpl ZoomImpl { get; private set; }
            public IZoom Zoom { get { return ZoomImpl; } }

            public PagingImpl PagingImpl { get; private set; }
            public IPaging Paging { get { return PagingImpl; } }


            public ExecutionParamsImpl()
            {
                ZoomImpl = new ZoomImpl();
                PagingImpl = new PagingImpl();
            }
        }

        private class ZoomImpl : IZoom
        {
            public int Level { get; set; }
            public int Count { get; set; }

            public ZoomImpl()
            {
                Level = int.MinValue;
                Count = int.MinValue;
            }
        }

        private class PagingImpl : IPaging
        {
            public int SkipConnections { get; set; }
            public int TakeConnections { get; set; }

            public PagingImpl()
            {
                TakeConnections = int.MaxValue;
            }
        }
    }
}