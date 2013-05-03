using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Associativy
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes()) routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
			{
				new RouteDescriptor
				{
					Name = "AssociativyNodeLabelAdmin",
					Route = new Route(
						"Admin/Associativy/NodeLabel/{action}",
						new RouteValueDictionary
						{
							{"area", "Associativy"},
							{"controller", "AssociativyNodeLabelAdmin"},
							{"action", "RefreshLabels"}
						},
						new RouteValueDictionary(),
						new RouteValueDictionary
						{
							{"area", "Associativy"}
						},
						new MvcRouteHandler())
				}
			};
        }
    }
}