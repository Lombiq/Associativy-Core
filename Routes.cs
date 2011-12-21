using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using Orchard.Environment.Extensions;

namespace Associativy
{
    [OrchardFeature("Associativy")]
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Name = "Test",
                    Route = new Route(
                        "AssociativyTest",
                        new RouteValueDictionary {
                                                    {"area", "Associativy"},
                                                    {"controller", "Test"},
                                                    {"action", "Index"}
                                                },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                                                    {"area", "Associativy"}
                                                },
                        new MvcRouteHandler())
                }
            };
        }
    }
}