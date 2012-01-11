using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Associativy
{
    [OrchardFeature("Associativy")]
    public class Routes : IRouteProvider
    {
        private readonly RouteCollection _routeCollection;

        public Routes(RouteCollection routeCollection)
        {
            _routeCollection = routeCollection;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new List<RouteDescriptor>();
        }
    }
}