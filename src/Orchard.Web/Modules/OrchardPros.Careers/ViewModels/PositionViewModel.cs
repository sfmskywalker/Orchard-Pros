using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.ViewModels {
    public class PositionViewModel {
        [StringLength(64)] public string CompanyName { get; set; }
        [StringLength(64)] public string Title { get; set; }
        [StringLength(64)] public string Location { get; set; }
        [UIHint("YearPicker")] public int? PeriodStartYear { get; set; }
        [UIHint("MonthPicker")] public int? PeriodStartMonth { get; set; }
        [UIHint("YearPicker")] public int? PeriodEndYear { get; set; }
        [UIHint("MonthPicker")] public int? PeriodEndMonth { get; set; }
        public bool IsCurrentPosition { get; set; }
        public string Description { get; set; }

        public IList<int> Years { get; set; }
        public IList<int> Months { get; set; }
        public ProfessionalProfilePart Profile { get; set; }
    }
}