using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;

namespace OrchardPros.Models {
    public class VotablePart : ContentPart {
        internal LazyField<double> VoteCountField = new LazyField<double>();

        public double VoteCount {
            get { return VoteCountField.Value; }
        }
    }
}