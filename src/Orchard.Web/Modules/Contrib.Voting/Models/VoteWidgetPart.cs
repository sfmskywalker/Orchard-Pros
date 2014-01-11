using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Contrib.Voting.Models {
    public class VoteWidgetPart : ContentPart<VoteWidgetPartRecord> {
        [Required]
        public string FunctionName {
            get { return Retrieve(x => x.FunctionName); }
            set { Store(x => x.FunctionName, value); }
        }

        public string ContentType {
            get { return Retrieve(x => x.ContentType); }
            set { Store(x => x.ContentType, value); }
        }

        [Range(1, int.MaxValue)]
        public int Count {
            get { return Retrieve(x => x.Count); }
            set { Store(x => x.Count, value); }
        }

        public bool Ascending {
            get { return Retrieve(x => x.Ascending);  }
            set { Store(x => x.Ascending, value);  }
        }

        public string Dimension {
            get { return Retrieve(x => x.Dimension); }
            set { Store(x => x.Dimension, value); }
        }
    }
}