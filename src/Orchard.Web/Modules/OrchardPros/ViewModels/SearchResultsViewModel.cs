using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace OrchardPros.ViewModels {
    public class SearchResultsViewModel {
        public LocalizedString IndexDisplayName { get; set; }
        public IList<IContent> ContentItems { get; set; }
        public IList<dynamic> ContentItemShapes { get; set; }
        public dynamic Pager { get; set; }
    }
}