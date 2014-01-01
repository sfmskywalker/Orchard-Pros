using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace OrchardProsTheme.Helpers {
    public static class UrlHelpers {
        public static string FileIcon(this UrlHelper url, string fileName) {
            const string defaultType = "document";
            var extension = Path.GetExtension(fileName) ?? "";
            var dictionary = new Dictionary<string, string[]> {
                {"docx", new[] {".docx", ".doc", ".rtf"}},
                {"xlsx", new[] {".xlsx", ".xls", ".csv"}},
                {"image", new[] {".jpg", ".png", ".gif", ".tiff", ".tif", ".svg", ".psd", ".bmp"}},
                {"video", new[] {".mpg", ".mpeg", ".mp4", ".wmv"}},
                {"pdf", new[] {".pdf"}},
                {"pptx", new[] {".pptx", "ppt"}}
            };
            var type = dictionary.Where(x => x.Value.Contains(extension, StringComparer.OrdinalIgnoreCase)).Select(x => x.Key).FirstOrDefault() ?? defaultType;
            return url.Content(String.Format("~/themes/orchardprostheme/images/file-{0}.png", type));
        }
    }
}