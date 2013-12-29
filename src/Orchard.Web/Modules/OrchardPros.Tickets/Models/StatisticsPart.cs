﻿using Orchard.ContentManagement;

namespace OrchardPros.Tickets.Models {
    public class StatisticsPart : ContentPart<StatisticsPartRecord> {
        public int ViewCount {
            get { return Retrieve(x => x.ViewCount); }
            set { Store(x => x.ViewCount, value); }
        }
    }
}