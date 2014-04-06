using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Search.Services;
using Orchard.Themes;
using Orchard.UI.Navigation;
using OrchardPros.Models;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Themed]
    public class SearchController : Controller {
        private readonly ISearchService _searchService;
        private readonly IOrchardServices _services;

        public SearchController(ISearchService searchService, IOrchardServices services) {
            _searchService = searchService;
            _services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index(string term, SearchIndex? index, PagerParameters pagerParameters) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            string indexName;
            LocalizedString indexDisplayName;
            string[] searchFields;

            index = index ?? SearchIndex.All;
            switch (index) {
                case SearchIndex.Tickets:
                    indexName = "Tickets";
                    indexDisplayName = T("Tickets");
                    searchFields = new[] {"author", "body", "title", "username", "firstname", "lastname", "bio", "Categories", "Tags"};
                    break;
                case SearchIndex.People:
                    indexName = "People";
                    indexDisplayName = T("People");
                    searchFields = new[] { "body", "title", "username", "firstname", "lastname", "bio" };
                    break;
                case SearchIndex.Pages:
                    indexName = "Pages";
                    indexDisplayName = T("Pages");
                    searchFields = new[] { "body", "title" };
                    break;
                default:
                    indexName = "All";
                    indexDisplayName = T("All");
                    searchFields = new[] { "author", "body", "title", "username", "firstname", "lastname", "bio", "Categories", "Tags" };
                    break;
            }

            var searchHits = _searchService.Query(
                query: term,
                filterCulture: false,
                skip: pager.GetStartIndex(),
                take: pager.PageSize,
                index: indexName,
                searchFields: searchFields,
                shapeResult: searchHit => searchHit);

            var foundIds = searchHits.Select(searchHit => searchHit.ContentItemId).ToArray();
            var foundItems = _services.ContentManager.GetMany<IContent>(foundIds, VersionOptions.Published, QueryHints.Empty).ToArray();
            var foundItemShapes = foundItems.Select(x => _services.ContentManager.BuildDisplay(x, "SearchResult")).ToArray();
            
            searchHits.TotalItemCount -= foundIds.Count() - foundItems.Count();
            var pagerShape = _services.New.Pager(pager).TotalItemCount(searchHits.TotalItemCount);
            var viewModel = new SearchResultsViewModel {
                Term = term,
                Index = index.Value,
                IndexDisplayName = indexDisplayName,
                ContentItems = foundItems,
                ContentItemShapes = foundItemShapes,
                Pager = pagerShape
            };
            return View(viewModel);
        }
    }
}