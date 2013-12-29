using System;
using System.Collections.Generic;
using System.Linq;

namespace OrchardPros.Tickets.Helpers {
    public static class StringHelpers {
        public static IEnumerable<T> Split<T>(this string value, string separator = ",") {
            if (string.IsNullOrWhiteSpace(value))
                return Enumerable.Empty<T>();

            var items = value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            return items.Select(x => Convert.ChangeType(x, typeof (T))).Cast<T>();
        }

        public static string Join<T>(this IEnumerable<T> value, string separator = ",") {
            if (value == null)
                return null;

            return String.Join(separator, value);
        }
    }
}