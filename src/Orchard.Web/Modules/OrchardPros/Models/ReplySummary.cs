using System;

namespace OrchardPros.Models {
    public class ReplySummary {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime ModifiedUtc { get; set; }
    }
}