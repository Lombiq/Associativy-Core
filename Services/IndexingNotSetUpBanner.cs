using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Associativy.GraphDiscovery;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.UI.Admin.Notification;
using Orchard.UI.Notify;

namespace Associativy.Services
{
    public class IndexingNotSetUpBanner : INotificationProvider
    {
        private readonly INodeIndexingService _indexingService;
        private readonly IGraphManager _graphManager;
        private readonly IWorkContextAccessor _wca;
        private readonly dynamic _shapeFactory;

        public Localizer T { get; set; }


        public IndexingNotSetUpBanner(
            INodeIndexingService indexingService,
            IGraphManager graphManager,
            IWorkContextAccessor wca,
            IShapeFactory shapeFactory)
        {
            _indexingService = indexingService;
            _graphManager = graphManager;
            _wca = wca;
            _shapeFactory = shapeFactory;

            T = NullLocalizer.Instance;
        }



        public IEnumerable<NotifyEntry> GetNotifications()
        {
            var workContext = _wca.GetContext();
            var request = workContext.HttpContext.Request;
            var urlHelper = new UrlHelper(request.RequestContext);
            var i = 0;

            foreach (var graph in _graphManager.FindDistinctGraphs(GraphContext.Empty))
            {
                if (!_indexingService.IsIndexingSetupForGraph(graph.Name))
                {
                    if (i == 0)
                    {
                        workContext.Layout.Tail.Add(_shapeFactory.NodeIndexingAntiforgeryToken());
                    }
                    i++;
                    var url = urlHelper.Action("SetupIndexingForGraph", "Admin", new { Area = "Associativy", GraphName = graph.Name, ReturnUrl = request.RawUrl });
                    yield return new NotifyEntry { Message = T("Node indexing is not set up for the graph {0}. This means that nodes can't be fetched by their labels. If you have an indexing implementation like Lucene enabled you can set up node indexing for this graph <a href=\"{1}\" itemprop=\"UnsafeUrl\">here</a>.", graph.DisplayName, url), Type = NotifyType.Warning };
                }
            }
        }
    }
}