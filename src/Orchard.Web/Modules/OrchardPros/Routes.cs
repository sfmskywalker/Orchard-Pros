using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace OrchardPros {
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
                        {"area", "OrchardPros"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        {"area", "OrchardPros"}
                    },
                    new MvcRouteHandler())
            };

            yield return new RouteDescriptor {
                Name = "UserProfile",
                Priority = 1,
                Route = new Route(
                    "profile/{userName}/{action}/{id}",
                    new RouteValueDictionary {
                        {"id", UrlParameter.Optional},
                        {"action", "Index"},
                        {"userName", UrlParameter.Optional},
                        {"controller", "Profile"},
                        {"area", "OrchardPros"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        {"area", "OrchardPros"}
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
