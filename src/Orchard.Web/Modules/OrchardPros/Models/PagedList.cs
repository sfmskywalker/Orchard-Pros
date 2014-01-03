using System.Collections.Generic;

namespace OrchardPros.Models {
    public class PagedList<T> : List<T>, IPagedList<T> {
        public PagedList(IEnumerable<T> collection, long totalItemCount) : base(collection) {
            TotalItemCount = totalItemCount;
        }

        public long TotalItemCount { get; private set; }
    }
}