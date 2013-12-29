using Orchard.Caching;
using Orchard.ContentManagement.Handlers;
using Orchard.Taxonomies.Models;

namespace OrchardPros.Tickets.Handlers {
    public class TermPartHandler : ContentHandler {
        private readonly ISignals _signals;

        public TermPartHandler(ISignals signals) {
            _signals = signals;
            OnUpdated<TermPart>(ClearTermsDictionaryCache);
            OnRemoved<TermPart>(ClearTermsDictionaryCache);
        }

        private void ClearTermsDictionaryCache(ContentContextBase context, TermPart part) {
            switch (part.ContentItem.ContentType) {
                case "CategoryTerm":
                    _signals.Trigger(Signals.CategoryDictionary);
                    break;
                case "TagTerm":
                    _signals.Trigger(Signals.TagDictionary);
                    break;
            }
        }
    }
}