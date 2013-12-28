using System;
using Orchard.UI.Navigation;

namespace OrchardPros.Tickets.Helpers {
    public static class PagerHelpers {
        public static int TotalPageCount(this Pager pager, long totalCount) {
            return (int)Math.Ceiling(((double)totalCount / pager.PageSize));
        }
    }
}