using Orchard.Caching;
using Orchard.ContentManagement.Handlers;
using Orchard.Taxonomies.Models;

namespace OrchardPros.Tickets.Handlers {
    public class CategoryTermPartHandler : ContentHandler {
        private readonly ISignals _signals;

        public CategoryTermPartHandler(ISignals signals) {
            _signals = signals;
            OnUpdated<TermPart>(ClearCategoryDictionaryCache);
            OnRemoved<TermPart>(ClearCategoryDictionaryCache);
        }

        private void ClearCategoryDictionaryCache(ContentContextBase context, TermPart part) {
            if (part.ContentItem.ContentType != "CategoryTerm")
                return;

            _signals.Trigger(Signals.CategoryDictionary);
        }
    }
}