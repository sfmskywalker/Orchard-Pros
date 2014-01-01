using System.Collections.Generic;
using NGM.OpenAuthentication.Models;
using Orchard;

namespace OrchardPros.Services {
    public interface IOpenAuthServices : IDependency {
        IEnumerable<OrchardAuthenticationClientData> GetProviders();
    }
}