using Orchard;
using Orchard.Security;

namespace OrchardPros.Membership.Services {
    public interface IAccountServices : IDependency {
        void ChangeUserName(IUser user, string newUserName);
        void ChangeUserEmail(IUser user, string newEmail);
    }
}