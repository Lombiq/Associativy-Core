using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Associativy
{
    [OrchardFeature("Associativy")]
    public abstract class RoutesBase : IRouteProvider
    {
        public string FrontendEngine { get; set; }
        abstract public string ModuleName { get; }

        public RoutesBase()
        {
            FrontendEngine = "JIT";
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Name = ModuleName + " Associations",
                    Route = new Route(
                        ModuleName +"/Associations/{action}",
                        new RouteValueDictionary {
                                                    {"area", "Associativy"},
                                                    {"controller", FrontendEngine},
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