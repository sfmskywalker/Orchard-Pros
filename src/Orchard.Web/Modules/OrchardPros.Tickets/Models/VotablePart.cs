using System.Collections.Generic;
using Orchard.ContentManagement;

namespace OrchardPros.Tickets.Models {
    public class VotablePart : ContentPart {
        public IList<int> VoteIds {
            get { return this.Retrieve(x => x.VoteIds); }
            set { this.Store(x => x.VoteIds, value); }
        }
    }
}