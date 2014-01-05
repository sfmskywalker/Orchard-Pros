using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;

namespace OrchardPros.Models {
    public class RepliesPart : ContentPart {
        internal LazyField<IEnumerable<ReplyPart>> RepliesField = new LazyField<IEnumerable<ReplyPart>>();

        public IEnumerable<ReplyPart> Replies {
            get { return RepliesField.Value; }
        }
    }
}