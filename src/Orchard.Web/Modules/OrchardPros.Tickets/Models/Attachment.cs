using System;
using System.ComponentModel.DataAnnotations;

namespace OrchardPros.Tickets.Models {
    public class Attachment {
        public virtual int Id { get; set; }
        public virtual Ticket Ticket { get; set; }
        
        [StringLength(256)]
        public virtual string FileName { get; set; }
        public virtual int DownloadCount { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}