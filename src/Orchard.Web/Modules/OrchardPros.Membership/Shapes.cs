using System.Linq;
using Orchard.DisplayManagement.Descriptors;
using OrchardPros.Membership.Services;

namespace OrchardPros.Membership {
    public class Shapes : IShapeTableProvider {
        private readonly IOpenAuthServices _openAuthServices;

        public Shapes(IOpenAuthServices openAuthServices) {
            _openAuthServices = openAuthServices;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("OAuthLogin").OnDisplaying(displaying => {
                displaying.Shape.Providers = _openAuthServices.GetProviders().ToArray();
            });
        }
    }
}
