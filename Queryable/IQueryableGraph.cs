using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using QuickGraph;

namespace Associativy.Queryable
{
    public delegate dynamic ValueFactory<TNode>(IExecutionParams parameters);


    public interface IQueryableGraph<TNode>
    {
        ValueFactory<TNode> ValueFactory { get; } // Is this and copying necessary?
        IQueryParams Params { get; } 

        IQueryableGraph<TNode> Zoom(int level, int count);
        IQueryableGraph<TNode> SkipConnections  (int count);
        IQueryableGraph<TNode> TakeConnections(int count);
        int NodeCount();
        int ConnectionCount();
        int ZoomLevelCount(int maximalZoomLevelCount);
        IUndirectedGraph<TNode, IUndirectedEdge<TNode>> ToGraph();
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
    }

    public interface IPaging
    {
        int SkipConnections { get; }
        int TakeConnections { get; }
    }
}
