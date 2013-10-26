using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Orchard.Templates.Models {
    public class ShapePart : ContentPart<ShapePartRecord> {
        public string Name {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        public string Body {
            get { return Record.Body; }
            set { Record.Body = value; }
        }
    }

    public class ShapePartRecord : ContentPartRecord {
        public virtual string Name { get; set; }

        [StringLengthMax]
        public virtual string Body { get; set; }
    }
}