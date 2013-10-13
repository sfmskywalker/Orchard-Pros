using Orchard.Environment.Extensions;
using Orchard.UI.Navigation;

namespace Orchard.Messaging {
    [OrchardFeature("Orchard.Messaging.Queuing")]
    public class AdminMenu : Component, INavigationProvider {
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("Messaging"), "5.0", item => item
                .Action("Index", "AdminQueue", new { area = "Orchard.Messaging" }));
        }
    }
}