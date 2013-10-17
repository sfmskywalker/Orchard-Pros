﻿using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Templating.Models;

namespace Orchard.Templating.Handlers {
    public class TemplatePartHandler : ContentHandler {
        public TemplatePartHandler(IRepository<TemplatePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            OnGetContentItemMetadata<TemplatePart>((c, p) => c.Metadata.DisplayText = p.Name);
        }
    }
}