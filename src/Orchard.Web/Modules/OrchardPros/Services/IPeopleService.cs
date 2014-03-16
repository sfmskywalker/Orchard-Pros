using Orchard;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IPeopleService : IDependency {
        IPagedList<IUser> GetPeople(int? skip = null, int? take = null, PeopleCriteria criteria = PeopleCriteria.Activity, string countryCode = null, string term = null);
    }
}