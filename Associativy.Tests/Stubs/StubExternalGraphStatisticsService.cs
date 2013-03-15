using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.Models.Services;
using Associativy.Services;

namespace Associativy.Tests.Stubs
{
    public class StubExternalGraphStatisticsService : IExternalGraphStatisticsService
    {
        private readonly GraphInfo _graphInfo = new GraphInfo();


        public IGraphInfo GetGraphInfo()
        {
            return _graphInfo;
        }

        public void SetNodeCount(int count)
        {
            _graphInfo.NodeCount = count;
        }

        public void SetConnectionCount(int count)
        {
            _graphInfo.ConnectionCount = count;
        }

        public void SetCentralNodeId(int id)
        {
            _graphInfo.CentralNodeId = id;
        }


        private class GraphInfo : IGraphInfo
        {
            public int NodeCount { get; set; }
            public int ConnectionCount { get; set; }
            public int CentralNodeId { get; set; }
        }
    }
}
