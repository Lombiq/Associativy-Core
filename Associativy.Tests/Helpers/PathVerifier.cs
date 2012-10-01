using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using QuickGraph;

namespace Associativy.Tests.Helpers
{
    static class PathVerifier
    {
        public static bool PathExistsInGraph(IUndirectedGraph<int, IUndirectedEdge<int>> graph, IEnumerable<IContent> path)
        {
            var pathList = path.ToList();
            if (!graph.ContainsVertex(pathList[0].Id)) return false;

            var nextIndex = 1;
            var node = pathList[0].Id;
            while (node != pathList.Last().Id)
            {
                var nextNode = pathList[nextIndex].Id;
                if (!graph.AdjacentEdges(node).Any(edge => edge.Target == nextNode || edge.Source == nextNode)) return false;
                node = nextNode;
                nextIndex++;
            }

            return true;
        }

        public static bool VerifyPath(IEnumerable<int> path, IEnumerable<IContent> verification)
        {
            var pathList = path.ToList();
            var verificationList = verification.ToList();

            var i = 0;
            while (i < pathList.Count && verificationList[i].Id == pathList[i])
            {
                i++;
            }

            return i == pathList.Count;
        }
    }
}
