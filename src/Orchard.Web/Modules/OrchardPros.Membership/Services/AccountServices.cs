using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Security;
using Orchard.Users.Models;

namespace OrchardPros.Membership.Services {
    public class AccountServices : IAccountServices {
        private readonly IRepository<UserPartRecord> _userRepository;

        public AccountServices(IRepository<UserPartRecord> userRepository) {
            _userRepository = userRepository;
        }

        public void ChangeUserName(IUser user, string newUserName) {
            var record = _userRepository.Get(user.Id);
            user.As<UserPart>().UserName = newUserName;
            record.NormalizedUserName = newUserName.ToLowerInvariant();
        }

        public void ChangeUserEmail(IUser user, string newEmail) {
            user.As<UserPart>().Email = newEmail;
        }
    }
}