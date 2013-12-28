using System.Collections.Generic;

namespace OrchardPros.Tickets.Models {
    public class PagedList<T> : List<T>, IPagedList<T> {
        public PagedList(IEnumerable<T> collection, int totalItemCount) : base(collection) {
            TotalItemCount = totalItemCount;
        }

        public long TotalItemCount { get; private set; }
    }
}