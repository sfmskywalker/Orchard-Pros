using System.Collections.Generic;
using OrchardPros.Models;

namespace OrchardPros.Helpers {
    public static class EnumerableHelpers {
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> collection, long totalCount) {
            return new PagedList<T>(collection, totalCount);
        }
    }
}