using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using OrchardPros.Helpers;

namespace OrchardPros.Models {
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