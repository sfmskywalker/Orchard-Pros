using System.Collections.Generic;
using NGM.OpenAuthentication.Models;
using Orchard;

namespace OrchardPros.Services.Security {
    public interface IOpenAuthServices : IDependency {
        IEnumerable<OrchardAuthenticationClientData> GetProviders();
    }
}