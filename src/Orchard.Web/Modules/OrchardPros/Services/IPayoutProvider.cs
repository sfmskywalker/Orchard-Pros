using Orchard;
using Orchard.DisplayManagement;

namespace OrchardPros.Services {
    public interface IPayoutProvider : IDependency {
        string Name { get; }
        string DisplayName { get; }
        IShape BuildDisplay(IShapeFactory shapeFactory);
    }
}