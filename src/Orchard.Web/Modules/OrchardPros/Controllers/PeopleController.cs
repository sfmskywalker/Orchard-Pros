using System;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Themes;
using Orchard.UI.Navigation;
using OrchardPros.Models;
using OrchardPros.Services.User;

namespace OrchardPros.Controllers {
    [Themed]
    public class PeopleController : Controller {
        private readonly IOrchardServices _services;
        private readonly IRepository<Country> _countryRepository;
        private readonly IPeopleService _peopleService;

        public PeopleController(
            IOrchardServices services, 
            IRepository<Country> countryRepository, 
            IPeopleService peopleService) {

            _services = services;
            _countryRepository = countryRepository;
            _peopleService = peopleService;
        }

        public ActionResult Index(PagerParameters pagerParameters, string countryCode = null, PeopleCriteria criteria = PeopleCriteria.Activity, string term = null) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var users = _peopleService.GetPeople(pager.GetStartIndex(), pager.PageSize, criteria, countryCode, term);
            var country = !String.IsNullOrWhiteSpace(countryCode) ? _countryRepository.Get(x => x.Code == countryCode) : default(Country);
            var pagerShape = _services.New.Pager(pager).TotalItemCount(users.TotalItemCount);
            var countries = _services.ContentManager.Query<UserProfilePart>().Where<UserProfilePartRecord>(x => x.Country != null).List().Select(x => x.Country).Distinct().ToArray();
            var viewModel = _services.New.ViewModel(
                Users: users,
                Countries: countries,
                Country: country,
                Criteria: criteria,
                Pager: pagerShape);

            return View(viewModel);
        }
    }
}