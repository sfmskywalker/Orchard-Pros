using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Localization;
using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class SearchResultsViewModel {
        public string Term { get; set; }
        public SearchIndex Index { get; set; }
        public LocalizedString IndexDisplayName { get; set; }
        public IList<IContent> ContentItems { get; set; }
        public IList<dynamic> ContentItemShapes { get; set; }
        public dynamic Pager { get; set; }
    }
}