﻿using System;

namespace OrchardPros.Tickets.Models {
    public class ReplySummary {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}