using System;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Navigation;
using OrchardPros.Models;

namespace OrchardPros.Controllers {
    [Themed]
    public class PeopleController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
        private readonly IRepository<UserProfilePartRecord> _userProfileRepository;
        private readonly IRepository<Country> _countryRepository;

        public PeopleController(
            IOrchardServices services, 
            IRepository<UserProfilePartRecord> userProfileRepository, 
            IRepository<Country> countryRepository) {

            _contentManager = services.ContentManager;
            _services = services;
            _userProfileRepository = userProfileRepository;
            _countryRepository = countryRepository;
        }

        public ActionResult Index(PagerParameters pagerParameters, string countryCode) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var country = !String.IsNullOrWhiteSpace(countryCode) ? _countryRepository.Table.SingleOrDefault(x => x.Code == countryCode) : default(Country);
            var query = _userProfileRepository.Table;

            if (country != null) {
                var code = country.Code;
                query = query.Where(x => x.Country.Code == code);
            }

            var totalItemCount = query.Count();
            var userIds = query.Skip(pager.GetStartIndex()).Take(pager.PageSize).Select(x => x.Id).ToArray();
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty).ToArray();
            var pagerShape = _services.New.Pager(pager).TotalItemCount(totalItemCount);
            var countries = _userProfileRepository.Table.Where(x => x.Country != null).Select(x => x.Country).Distinct().ToArray();
            var viewModel = _services.New.ViewModel(
                Users: users,
                Countries: countries,
                Country: country,
                Pager: pagerShape);

            return View(viewModel);
        }
    }
}