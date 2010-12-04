﻿using System.Linq;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Environment.Features;
using Orchard.Mvc.Filters;
using Orchard.Themes.ViewModels;

namespace Orchard.Themes.Preview {
    public class PreviewThemeFilter : FilterProvider, IResultFilter {
        private readonly IThemeManager _themeManager;
        private readonly IPreviewTheme _previewTheme;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly dynamic _shapeFactory;
        private readonly IFeatureManager _featureManager;

        public PreviewThemeFilter(
            IThemeManager themeManager,
            IPreviewTheme previewTheme,
            IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory,
            IFeatureManager featureManager) {
            _themeManager = themeManager;
            _previewTheme = previewTheme;
            _workContextAccessor = workContextAccessor;
            _shapeFactory = shapeFactory;
            _featureManager = featureManager;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            var previewThemeName = _previewTheme.GetPreviewTheme();
            if (string.IsNullOrEmpty(previewThemeName))
                return;

            var installedThemes = _featureManager.GetEnabledFeatures()
                .Select(x => x.Extension)
                .Where(x => x.ExtensionType == "Theme")
                .Distinct();

            var themeListItems = installedThemes
                .Select(theme => new SelectListItem {
                    Text = theme.DisplayName,
                    Value = theme.Name,
                    Selected = theme.Name == previewThemeName
                })
                .ToList();

            _workContextAccessor.GetContext(filterContext).Layout.Zones["Body"].Add(_shapeFactory.ThemePreview(Themes: themeListItems), ":before");
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) { }
    }
}