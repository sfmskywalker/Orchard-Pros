using System;
using System.ComponentModel.DataAnnotations;

namespace Orchard.Messaging.ViewModels {
    public class MessageQueueViewModel {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public TimeSpan UpdateFrequency { get; set; }
        public TimeSpan TimeSlice { get; set; }

        public string ReturnUrl { get; set; }
    }
}