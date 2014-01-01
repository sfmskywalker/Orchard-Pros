using Orchard;
using Orchard.Security;

namespace OrchardPros.Services {
    public interface IAccountServices : IDependency {
        void ChangeUserName(IUser user, string newUserName);
        void ChangeUserEmail(IUser user, string newEmail);
    }
}