using Orchard;
using Orchard.ContentManagement;

namespace OrchardPros.Services.Content {
    public interface ICurrentContentAccessor : IDependency {
        ContentItem CurrentContentItem { get; }
    }
}