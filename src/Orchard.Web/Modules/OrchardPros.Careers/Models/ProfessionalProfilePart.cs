using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;

namespace OrchardPros.Careers.Models {
    public class ProfessionalProfilePart : ContentPart {
        internal LazyField<IEnumerable<Position>> PositionsField = new LazyField<IEnumerable<Position>>();

        public IEnumerable<Position> Positions {
            get { return PositionsField.Value; }
        }
    }
}