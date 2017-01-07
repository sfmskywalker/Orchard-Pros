using System;
using System.Web;
using System.Web.Mvc;
using NGravatar;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Helpers {
    public static class AvatarHelper {
        public const string DefaultAvatarUrl = "~/Themes/OrchardProsTheme/Images/temp-profile.png";

        public static string AvatarUrl<T>(this UrlHelper urlHelper, IUser user, Orchard.Mvc.ViewEngines.Razor.WebViewPage<T> view, int size) {
            var profile = user.As<UserProfilePart>();
            return profile.AvatarType == AvatarType.Gravatar ? urlHelper.GravatarUrl(user, size) : urlHelper.UploadedAvatarUrl(user, view, size);
        }

        public static string UploadedAvatarUrl<T>(this UrlHelper urlHelper, IUser user, Orchard.Mvc.ViewEngines.Razor.WebViewPage<T> view, int size) {
            var profile = user.As<UserProfilePart>();
            var resizedMediaUrl = !String.IsNullOrWhiteSpace(profile.Avatar) ? (string)view.Display.ResizeMediaUrl(Path: profile.Avatar, Width: size, Height: size, Mode: "crop").ToString() : default(string);
            return urlHelper.Content(resizedMediaUrl ?? String.Format(DefaultAvatarUrl, size));
        }

        public static string GravatarUrl(this UrlHelper urlHelper, IUser user, int size) {
            return urlHelper.GravatarUrl(user.Email, size);
        }

        public static string GravatarUrl(this UrlHelper urlHelper, string email, int size) {
            if (String.IsNullOrWhiteSpace(email))
                return null;

            var workContext = urlHelper.RequestContext.GetWorkContext();
            return new Gravatar {
                Default = VirtualPathUtility.AppendTrailingSlash(workContext.CurrentSite.BaseUrl) + urlHelper.Content(String.Format(DefaultAvatarUrl, size)),
                Rating = GravatarRating.PG,
                Size = size
            }.GetUrl(email);
        }
    }
}