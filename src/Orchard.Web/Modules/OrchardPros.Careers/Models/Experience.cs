using System;
using Orchard.Data.Conventions;

namespace OrchardPros.Careers.Models {
    public class Experience {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual Position Position { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}