﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using JetBrains.Annotations;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Environment.Features;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.ContentManagement;
using Orchard.Themes.Models;

namespace Orchard.Themes.Services {
    public interface IThemeService : IDependency {
        void DisableThemeFeatures(string themeName);
        void EnableThemeFeatures(string themeName);
    }

    [UsedImplicitly]
    public class ThemeService : IThemeService {
        private readonly IExtensionManager _extensionManager;
        private readonly IFeatureManager _featureManager;
        private readonly IEnumerable<IThemeSelector> _themeSelectors;

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IOrchardServices _orchardServices;
        private readonly IShellDescriptorManager _shellDescriptorManager;

        public ThemeService(
            IShellDescriptorManager shellDescriptorManager,
            IExtensionManager extensionManager,
            IFeatureManager featureManager,
            IEnumerable<IThemeSelector> themeSelectors,

            IWorkContextAccessor workContextAccessor,
            ShellDescriptor shellDescriptor,
            IOrchardServices orchardServices) {
            _shellDescriptorManager = shellDescriptorManager;
            _extensionManager = extensionManager;
            _featureManager = featureManager;
            _themeSelectors = themeSelectors;

            _workContextAccessor = workContextAccessor;
            _shellDescriptor = shellDescriptor;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }



        private bool AllBaseThemesAreInstalled(string baseThemeName) {
            var themesSeen = new List<string>();
            while (!string.IsNullOrWhiteSpace(baseThemeName)) {
                //todo: (heskew) need a better way to protect from recursive references
                if (themesSeen.Contains(baseThemeName))
                    throw new InvalidOperationException(T("The theme \"{0}\" was already seen - looks like we're going around in circles.", baseThemeName).Text);
                themesSeen.Add(baseThemeName);

                var baseTheme = _extensionManager.GetExtension(baseThemeName);
                if (baseTheme == null)
                    return false;
                baseThemeName = baseTheme.BaseTheme;
            }

            return true;
        }

        public void DisableThemeFeatures(string themeName) {
            var themes = new Queue<string>();
            while (themeName != null) {
                if (themes.Contains(themeName))
                    throw new InvalidOperationException(T("The theme \"{0}\" is already in the stack of themes that need features disabled.", themeName).Text);
                var theme = _extensionManager.GetExtension(themeName);
                if (theme == null)
                    break;
                themes.Enqueue(themeName);

                themeName = !string.IsNullOrWhiteSpace(theme.BaseTheme)
                    ? theme.BaseTheme
                    : null;
            }

            while (themes.Count > 0)
                _featureManager.DisableFeatures(new[] { themes.Dequeue() });
        }

        public void EnableThemeFeatures(string themeName) {
            var themes = new Stack<string>();
            while(themeName != null) {
                if (themes.Contains(themeName))
                    throw new InvalidOperationException(T("The theme \"{0}\" is already in the stack of themes that need features enabled.", themeName).Text);
                themes.Push(themeName);

                var theme = _extensionManager.GetExtension(themeName);
                themeName = !string.IsNullOrWhiteSpace(theme.BaseTheme)
                    ? theme.BaseTheme
                    : null;
            }

            while (themes.Count > 0)
                _featureManager.EnableFeatures(new[] {themes.Pop()});
        }

        private bool DoEnableTheme(string themeName) {
            if (string.IsNullOrWhiteSpace(themeName))
                return false;

            //todo: (heskew) need messages given in addition to all of these early returns so something meaningful can be presented to the user
            var themeToEnable = _extensionManager.GetExtension(themeName);
            if (themeToEnable == null)
                return false;

            // ensure all base themes down the line are present and accounted for
            //todo: (heskew) dito on the need of a meaningful message
            if (!AllBaseThemesAreInstalled(themeToEnable.BaseTheme))
                return false;

            // enable all theme features
            EnableThemeFeatures(themeToEnable.Name);
            return true;
        }

        public ExtensionDescriptor GetRequestTheme(RequestContext requestContext) {
            var requestTheme = _themeSelectors
                .Select(x => x.GetTheme(requestContext))
                .Where(x => x != null)
                .OrderByDescending(x => x.Priority);

            if (requestTheme.Count() < 1)
                return null;

            foreach (var theme in requestTheme) {
                var t = _extensionManager.GetExtension(theme.ThemeName);
                if (t != null)
                    return t;
            }

            return _extensionManager.GetExtension("SafeMode");
        }

        /// <summary>
        /// Loads only installed themes
        /// </summary>
        public IEnumerable<ExtensionDescriptor> GetInstalledThemes() {
            return GetThemes(_extensionManager.AvailableExtensions());
        }

        private IEnumerable<ExtensionDescriptor> GetThemes(IEnumerable<ExtensionDescriptor> extensions) {
            var themes = new List<ExtensionDescriptor>();
            foreach (var descriptor in extensions) {

                if (!string.Equals(descriptor.ExtensionType, "Theme", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                ExtensionDescriptor theme = descriptor;

                if (!theme.Tags.Contains("hidden")) {
                    themes.Add(theme);
                }
            }
            return themes;
        }

        private static string TryLocalize(string key, string original, Localizer localizer) {
            var localized = localizer(key).Text;

            if ( key == localized ) {
                // no specific localization available
                return original;
            }

            return localized;
        }

        private bool IsThemeEnabled(ExtensionDescriptor descriptor) {
            return (descriptor.Name == "TheAdmin" || descriptor.Name == "SafeMode") ||
                _shellDescriptorManager.GetShellDescriptor().Features.Any(sf => sf.Name == descriptor.Name);
        }

        //private ITheme CreateTheme(ExtensionDescriptor descriptor) {

        //    var localizer = LocalizationUtilities.Resolve(_workContextAccessor.GetContext(), String.Concat(descriptor.Location, "/", descriptor.Name, "/Theme.txt"));

        //    return new Theme {
        //        //Author = TryLocalize("Author", descriptor.Author, localizer) ?? "",
        //        //Description = TryLocalize("Description", descriptor.Description, localizer) ?? "",
        //        DisplayName = TryLocalize("Name", descriptor.DisplayName, localizer) ?? "",
        //        //HomePage = TryLocalize("Website", descriptor.WebSite, localizer) ?? "",
        //        ThemeName = descriptor.Name,
        //        //Version = descriptor.Version ?? "",
        //        Tags = TryLocalize("Tags", descriptor.Tags, localizer) ?? "",
        //        Zones = descriptor.Zones ?? "",
        //        BaseTheme = descriptor.BaseTheme ?? "",
        //        Enabled = IsThemeEnabled(descriptor)
        //    };
        //}
    }

}