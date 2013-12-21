using System;
using Orchard.Data.Conventions;

namespace OrchardPros.Careers.Models {
    public class Position {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string Title { get; set; }
        public virtual string Location { get; set; }
        public virtual int? PeriodStartYear { get; set; }
        public virtual int? PeriodStartMonth { get; set; }
        public virtual int? PeriodEndYear { get; set; }
        public virtual int? PeriodEndMonth { get; set; }
        public virtual bool IsCurrentPosition { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual bool IsArchived { get; set; }
        public virtual DateTime? ArchivedUtc { get; set; }
    }
}