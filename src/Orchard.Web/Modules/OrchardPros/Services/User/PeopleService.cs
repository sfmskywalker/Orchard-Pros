using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Search.Services;
using Orchard.Security;
using Orchard.Users.Models;
using OrchardPros.Helpers;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public class PeopleService : IPeopleService {
        private readonly IContentManager _contentManager;
        private readonly ISearchService _searchService;
        private readonly IRepository<Country> _countryRepository;

        public PeopleService(
            IContentManager contentManager,
            ISearchService searchService, 
            IRepository<Country> countryRepository) {

            _contentManager = contentManager;
            _searchService = searchService;
            _countryRepository = countryRepository;
        }

        public IPagedList<IUser> GetPeople(int? skip = null, int? take = null, PeopleCriteria criteria = PeopleCriteria.Activity, string countryCode = null, string term = null) {
            var baseQuery = _contentManager.Query<UserProfilePart>(VersionOptions.Published);

            if (!String.IsNullOrWhiteSpace(term)) {
                var foundUserIds = SearchPeople(term, skip.GetValueOrDefault(), take);
                baseQuery = baseQuery.ForContentItems(foundUserIds);
            }

            baseQuery = baseQuery.Join<UserProfilePartRecord>();
            IContentQuery<UserProfilePart, UserProfilePartRecord> commonQuery;

            switch (criteria) {
                default:
                    commonQuery = baseQuery.OrderByDescending<UserProfilePartRecord>(x => x.ActivityPoints);
                    break;
                case PeopleCriteria.Experience:
                    commonQuery = baseQuery.OrderByDescending<UserProfilePartRecord>(x => x.ExperiencePoints);
                    break;
                case PeopleCriteria.Name:
                    commonQuery = baseQuery.OrderBy<UserPartRecord>(x => x.UserName).Join<UserProfilePartRecord>();
                    break;
                case PeopleCriteria.RegistrationDate:
                    commonQuery = baseQuery.OrderByDescending<UserProfilePartRecord>(x => x.CreatedUtc);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(countryCode)) {
                var country = _countryRepository.Get(x => x.Code == countryCode);
                commonQuery = commonQuery.Where<UserProfilePartRecord>(x => x.Country.Id == country.Id);
            }

            var peopleQuery = commonQuery;
            var totalCount = peopleQuery.Count();
            var pagedQuery = skip != null && take != null ? peopleQuery.Slice(skip.Value, take.Value) : peopleQuery.List();
            var people = pagedQuery.ToArray();

            return people.Select(x => x.As<IUser>()).ToPagedList(totalCount);
        }

        private IEnumerable<int> SearchPeople(string term, int skip, int? take) {
            var searchHits = _searchService.Query(
                query: term, 
                filterCulture: false,
                page: skip,
                pageSize: take,
                index: "People",
                searchFields: new[]{"author", "body", "title", "Categories", "Tags"},
                shapeResult: searchHit => searchHit);

            var foundIds = searchHits.Select(searchHit => searchHit.ContentItemId).ToList();
            return foundIds;
        }
    }
}