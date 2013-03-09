using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using QuickGraph;

namespace Associativy.Queryable
{
    public delegate dynamic ValueFactory<TNode>(IExecutionParams parameters);


    public interface IQueryableGraph<TNode> : ITransientDependency
    {
        ValueFactory<TNode> ValueFactory { get; }
        IQueryableGraph<TNode> Zoom(int level, int count);
        IQueryableGraph<TNode> Skip(int count);
        IQueryableGraph<TNode> Take(int count);
        int NodeCount();
        int ConnectionCount();
        int ZoomLevelCount(int maximalZoomLevelCount);
        IUndirectedGraph<TNode, IUndirectedEdge<TNode>> ToGraph();
    }


    public enum ExecutionMethod
    {
        NodeCount,
        ConnectionCount,
        ZoomLevelCount,
        ToGraph
    }

    public interface IExecutionParams
	{
        ExecutionMethod Method { get; }
        IZoom Zoom { get; }
        IPaging Paging { get; }
	}

    public interface IZoom
    {
        int Level { get; }
        int Count { get; }
    }

    public interface IPaging
    {
        int Skip { get; }
        int Take { get; }
    }
}
