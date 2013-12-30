using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using OrchardPros.Tickets.Helpers;

namespace OrchardPros.Tickets.Models {
    public class VotablePart : ContentPart {
        public IEnumerable<int> VoteIds {
            get { return Retrieve<string>("Votes").Split<int>(); }
            set { Store("Votes", value.Join()); }
        }

        public int VoteCount {
            get { return VoteIds.Count(); }
        }
    }
}