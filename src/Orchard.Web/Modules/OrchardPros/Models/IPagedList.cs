using System.Collections.Generic;

namespace OrchardPros.Models {
    public interface IPagedList<T> : IList<T> {
        long TotalItemCount { get; }
    }
}