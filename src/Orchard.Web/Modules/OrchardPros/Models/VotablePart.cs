using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;

namespace OrchardPros.Models {
    public class VotablePart : ContentPart {
        internal LazyField<int> VoteCountField = new LazyField<int>();

        public int VoteCount {
            get { return VoteCountField.Value; }
        }
    }
}