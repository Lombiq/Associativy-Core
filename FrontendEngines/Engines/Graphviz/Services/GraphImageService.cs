using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.FileSystems.Media;
using Orchard.Environment.Extensions;
using QuickGraph;
using Associativy.Models;
using QuickGraph.Graphviz;
using QuickGraph.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using System.Net;
using System.Diagnostics;
using Associativy.EventHandlers;
using Orchard.Caching;

namespace Associativy.FrontendEngines.Engines.Graphviz.Services
{
    [OrchardFeature("Associativy")]
    public class GraphImageService<TNode> : Associativy.FrontendEngines.Engines.Graphviz.Services.IGraphImageService<TNode>
        where TNode : INode
    {
        protected readonly IStorageProvider _storageProvider;
        protected readonly ICacheManager _cacheManager;
        protected readonly IAssociativeGraphEventMonitor _graphEventMonitor;

        protected readonly string _storagePath = "Associativy/Graphs-" + typeof(TNode).Name + "/";

        public GraphImageService(
            IStorageProvider storageProvider,
            ICacheManager cacheManager,
            IAssociativeGraphEventMonitor graphEventMonitor)
        {
            _storageProvider = storageProvider;
            _cacheManager = cacheManager;
            _graphEventMonitor = graphEventMonitor;
        }

        public virtual string ToSvg(IUndirectedGraph<TNode, IUndirectedEdge<TNode>> graph, Action<GraphvizAlgorithm<TNode, IUndirectedEdge<TNode>>> initialization)
        {
            //var stringBuilder = new StringBuilder();
            //using (var xmlWriter = XmlWriter.Create(stringBuilder))
            //{
            //    // This may also need attributes on models, see: http://quickgraph.codeplex.com/wikipage?title=GraphML%20Serialization&referringTitle=Documentation
            //    graph.SerializeToGraphML<TNode, IUndirectedEdge<TNode>, IUndirectedGraph<TNode, IUndirectedEdge<TNode>>>(
            //        xmlWriter,
            //        node => node.Label,
            //        edge => edge.Source.Id.ToString() + edge.Target.Id.ToString());
            //}
            //var graphML = stringBuilder.ToString();

            var filePath = _storagePath + graph.GetHashCode().ToString() + initialization.GetHashCode() + ".svg";
            var sw = new Stopwatch();
            sw.Start();
            var z = typeof(TNode).GetCustomAttributes(true);
            sw.Stop();

            return _cacheManager.Get("Associativy.GraphImages." + filePath, ctx =>
            {
                //_graphEventMonitor.MonitorChangedSignal(ctx);

                // Since there is no method for checking the existance of a file, we use this ugly technique
                try
                {
                    _storageProvider.DeleteFile(filePath);
                }
                catch (Exception)
                {
                }

                var dotData = graph.ToGraphviz(algorithm =>
                {
                    initialization(algorithm);
                });

                var wc = new WebClient();
                var svgData = wc.UploadString("http://rise4fun.com/services.svc/ask/agl", dotData);

                using (var stream = _storageProvider.CreateFile(filePath).OpenWrite())
                {
                    var bytes = Encoding.UTF8.GetBytes(svgData);
                    stream.Write(bytes, 0, bytes.Length);
                }

                return _storageProvider.GetPublicUrl(filePath);
            });
        }
    }
}