using System;
using System.Linq;
using NGM.OpenAuthentication.Events;
using Orchard.ContentManagement;
using OrchardPros.Models;

namespace OrchardPros.Events {
    public class OpenAuthUserEventHandler : IOpenAuthUserEventHandler {
        public void Creating(CreatingOpenAuthUserContext context) {}

        public void Created(CreatedOpenAuthUserContext context) {
            var profile = context.User.As<UserProfilePart>();

            switch (context.ProviderName) {
                case "facebook":
                    var facebookUrl = context.ExtraData["link"];
                    var name = context.ExtraData["name"];
                    var gender = context.ExtraData["gender"];
                    var nameParts = name.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    profile.FirstName = nameParts.Length >= 1 ? nameParts[0] : null;
                    profile.LastName = nameParts.Length >= 2 ? nameParts[nameParts.Length - 1] : null;
                    profile.MiddleName = nameParts.Length > 2 ? String.Join(" ", nameParts.Skip(1).Take(nameParts.Length - 2)) : null;
                    break;
                case "twitter":
                    break;
            }
        }
    }
}