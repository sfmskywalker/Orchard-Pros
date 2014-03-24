using Orchard;

namespace OrchardPros.Services.System {
    public interface IHandleGenerator : IDependency {
        string Generate();
    }
}