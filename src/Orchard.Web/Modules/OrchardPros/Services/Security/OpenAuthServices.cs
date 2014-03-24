using System.Collections.Generic;
using System.Linq;
using NGM.OpenAuthentication.Models;
using NGM.OpenAuthentication.Services;
using NGM.OpenAuthentication.Services.Clients;

namespace OrchardPros.Services.Security {
    public class OpenAuthServices : IOpenAuthServices {
        private readonly IEnumerable<IExternalAuthenticationClient> _openAuthClients;
        private readonly IOrchardOpenAuthClientProvider _openAuthService;

        public OpenAuthServices(IEnumerable<IExternalAuthenticationClient> openAuthClients, IOrchardOpenAuthClientProvider openAuthService) {
            _openAuthClients = openAuthClients;
            _openAuthService = openAuthService;
        }

        public IEnumerable<OrchardAuthenticationClientData> GetProviders() {
            return _openAuthClients.Select(client => _openAuthService.GetClientData(client.ProviderName)).Where(x => x != null);
        }
    }
}