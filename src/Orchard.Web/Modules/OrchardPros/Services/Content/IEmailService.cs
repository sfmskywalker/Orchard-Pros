using Orchard;
using Orchard.Localization;

namespace OrchardPros.Services.Content {
    public interface IEmailService : IDependency {
        void Queue(LocalizedString subject, string recipientEmail, string templateName, object templateProperties);
    }
}