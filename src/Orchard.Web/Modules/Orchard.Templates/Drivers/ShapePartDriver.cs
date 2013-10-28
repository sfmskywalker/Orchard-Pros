using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Templates.Helpers;
using Orchard.Templates.Models;
using Orchard.Templates.Services;
using Orchard.Templates.ViewModels;

namespace Orchard.Templates.Drivers {
    public class ShapePartDriver : ContentPartDriver<ShapePart> {
        private readonly IEnumerable<ITemplateProcessor> _processors;
        private readonly ITemplateService _service;
        private readonly ITemplateCache _cache;
        private readonly ITransactionManager _transactions;

        public ShapePartDriver(IEnumerable<ITemplateProcessor> processors, ITemplateService service, ITemplateCache cache, ITransactionManager transactions) {
            _processors = processors;
            _service = service;
            _cache = cache;
            _transactions = transactions;
            T = NullLocalizer.Instance;
        }

        Localizer T { get; set; }

        protected override DriverResult Editor(ShapePart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(ShapePart part, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new ShapePartViewModel {
                Name = part.Name,
                Template = part.Template,
                Language = part.Language,
                AvailableLanguages = _processors.Select(x => x.Type).Distinct().ToArray()
            };
            if (updater != null) {
                if (updater.TryUpdateModel(viewModel, Prefix, null, new[] { "AvailableLanguages" })) {
                    part.Name = viewModel.Name.TrimSafe();
                    part.Language = viewModel.Language;
                    part.Template = viewModel.Template;

                    try {
                        var processor = _processors.FirstOrDefault(x => String.Equals(x.Type, part.Language, StringComparison.OrdinalIgnoreCase));
                        processor.Verify(part.Template);

                        _cache.Set(part.Name, part.Template);
                    }
                    catch (Exception ex) {
                        updater.AddModelError("", T("Template processing error: {0}", ex.Message));
                        _transactions.Cancel();
                    }
                }
            }
            return ContentShape("Parts_Shape_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts.Shape", Model: viewModel, Prefix: Prefix));
        }
    }
}