﻿using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Orchard.MultiTenancy {
    public class AdminMenu : INavigationProvider {
        private readonly ShellSettings _shellSettings;

        public AdminMenu(ShellSettings shellSettings) {
            _shellSettings = shellSettings;
        }

        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            if ( _shellSettings.Name != "Default" )
                return;

            builder.Add(T("Tenants"), "100",
                menu => menu.Add(T("List"), "0", item => item.Action("Index", "Admin", new { area = "Orchard.MultiTenancy" })
                    .Permission(Permissions.ManageTenants)));
        }
    }
}