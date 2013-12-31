using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace OrchardPros.Tickets {
    public class Routes : IRouteProvider {
        public IEnumerable<RouteDescriptor> GetRoutes() {
            yield return new RouteDescriptor {
                Name = "Tickets",
                Priority = 1,
                Route = new Route(
                    "tickets/{action}/{id}",
                    new RouteValueDictionary {
                        {"id", UrlParameter.Optional},
                        {"action", "Index"},
                        {"controller", "Ticket"},
                        {"area", "OrchardPros.Tickets"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        {"area", "OrchardPros.Tickets"}
                    },
                    new MvcRouteHandler())
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var route in GetRoutes()) {
                routes.Add(route);
            }
        }
    }
}
