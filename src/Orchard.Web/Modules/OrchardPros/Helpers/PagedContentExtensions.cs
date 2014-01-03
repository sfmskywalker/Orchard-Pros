using System.Linq;
using Orchard.ContentManagement;
using OrchardPros.Models;

namespace OrchardPros.Helpers {
    public static class PagedContentExtensions {
        public static IPagedList<T> As<T>(this IPagedList<IContent> contentItems) where T : IContent {
            return contentItems.Where(x => x.Is<T>()).Select(x => x.As<T>()).ToPagedList(contentItems.TotalItemCount);
        }
    }
}