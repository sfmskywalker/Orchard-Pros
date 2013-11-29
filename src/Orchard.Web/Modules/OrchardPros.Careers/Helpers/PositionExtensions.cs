using System;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Helpers {
    public static class PositionExtensions {
        
        public static string Start(this Position position) {
            return Date(position.PeriodStartYear, position.PeriodStartMonth);
        }

        public static string End(this Position position) {
            return position.IsCurrentPosition ? "Current" : Date(position.PeriodEndYear, position.PeriodEndMonth);
        }

        private static string Date(int? year, int? month) {
            if (year == null) return null;
            if (month == null) return year.ToString();
            return new DateTime(year.Value, month.Value, 1).ToString("Y");
        }
    }
}