using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.GraphDiscovery;
using Orchard.ContentManagement;

namespace Associativy.Services
{
    public static class Extensions
    {
        public static void Connect<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node1, IContent node2)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.Connect(graphContext, node1.Id, node2.Id);
        }

        public static void DeleteFromNode<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.DeleteFromNode(graphContext, node.Id);
        }

        public static void Disconnect<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node1, IContent node2)
            where TConnectionManager : IConnectionManager
        {
            connectionManager.Disconnect(graphContext, node1.Id, node2.Id);
        }

        public static IEnumerable<int> GetNeighbourIds<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node)
            where TConnectionManager : IConnectionManager
        {
            return connectionManager.GetNeighbourIds(graphContext, node.Id);
        }

        public static int GetNeighbourCount<TConnectionManager>(this TConnectionManager connectionManager, IGraphContext graphContext, IContent node)
            where TConnectionManager : IConnectionManager
        {
            return connectionManager.GetNeighbourCount(graphContext, node.Id);
        }
    }
}