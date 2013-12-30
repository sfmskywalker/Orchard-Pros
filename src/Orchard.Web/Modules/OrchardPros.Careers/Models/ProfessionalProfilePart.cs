﻿using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;

namespace OrchardPros.Careers.Models {
    public class ProfessionalProfilePart : ContentPart {
        internal LazyField<IEnumerable<Position>> PositionsField = new LazyField<IEnumerable<Position>>();
        internal LazyField<IEnumerable<Skill>> SkillsField = new LazyField<IEnumerable<Skill>>();
        internal LazyField<IEnumerable<RecommendationPart>> RecommendationsField = new LazyField<IEnumerable<RecommendationPart>>();
        internal LazyField<IEnumerable<Experience>> ExperienceField = new LazyField<IEnumerable<Experience>>();

        public IEnumerable<Position> Positions {
            get { return PositionsField.Value; }
        }

        public IEnumerable<Skill> Skills {
            get { return SkillsField.Value; }
        }

        public IEnumerable<RecommendationPart> Recommendations {
            get { return RecommendationsField.Value; }
        }

        public IEnumerable<Experience> Experience {
            get { return ExperienceField.Value; }
        }
    }
}