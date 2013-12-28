using System.Collections.Generic;

namespace OrchardPros.Tickets.Models {
    public interface IPagedList<T> : IList<T> {
        long TotalItemCount { get; }
    }
}