using System;

namespace OrchardPros.Careers.Models {
    public class Recommendation {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual int RecommendingUserId { get; set; }
        public virtual string Text { get; set; }
        public virtual bool Approved { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}