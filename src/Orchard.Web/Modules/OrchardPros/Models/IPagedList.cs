using System.Collections.Generic;

namespace OrchardPros.Models {
    public interface IPagedList<out T> : IEnumerable<T> {
        long TotalItemCount { get; }
    }
}