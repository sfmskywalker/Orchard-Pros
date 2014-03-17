using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Search.Services;
using Orchard.Security;
using Orchard.Users.Models;
using OrchardPros.Helpers;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class PeopleService : IPeopleService {
        private readonly IContentManager _contentManager;
        private readonly ISearchService _searchService;

        public PeopleService(
            IContentManager contentManager,
            ISearchService searchService) {

            _contentManager = contentManager;
            _searchService = searchService;
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
                commonQuery = commonQuery.Where<UserProfilePartRecord>(x => x.Country.Code == countryCode);
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
                skip: skip,
                take: take,
                index: "People",
                searchFields: new[]{"author", "body", "title", "Categories", "Tags"},
                shapeResult: searchHit => searchHit);

            var foundIds = searchHits.Select(searchHit => searchHit.ContentItemId).ToList();
            return foundIds;
        }
    }
}