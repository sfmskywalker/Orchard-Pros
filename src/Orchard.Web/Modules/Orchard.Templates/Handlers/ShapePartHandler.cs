using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Templates.Models;

namespace Orchard.Templates.Handlers {
    public class ShapePartHandler : ContentHandler {
        public ShapePartHandler(IRepository<ShapePartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            OnGetContentItemMetadata<ShapePart>((c, p) => c.Metadata.DisplayText = p.Name);
        }
    }
}