using System;
using System.Collections.Generic;
using System.Linq;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Helpers {
    public static class PositionExtensions {
        
        public static string Start(this Position position) {
            return Date(position.PeriodStartYear, position.PeriodStartMonth);
        }

        public static string End(this Position position) {
            return position.IsCurrentPosition ? "Current" : Date(position.PeriodEndYear, position.PeriodEndMonth);
        }

        public static Position Current(this IEnumerable<Position> positions) {
            return positions
                .OrderByDescending(x => x.IsCurrentPosition)
                .ThenByDescending(x => x.PeriodStartYear.GetValueOrDefault())
                .ThenByDescending(x => x.PeriodStartMonth.GetValueOrDefault())
                .FirstOrDefault();
        }

        private static string Date(int? year, int? month) {
            if (year == null) return null;
            if (month == null) return year.ToString();
            return new DateTime(year.Value, month.Value, 1).ToString("Y");
        }
    }
}