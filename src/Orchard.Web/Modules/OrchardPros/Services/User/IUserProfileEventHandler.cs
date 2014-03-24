using Orchard.Events;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public interface IUserProfileEventHandler : IEventHandler {
        void XpReceived(XpReceivedContext context);
        void LevelUp(LevelUpContext context);
    }
}