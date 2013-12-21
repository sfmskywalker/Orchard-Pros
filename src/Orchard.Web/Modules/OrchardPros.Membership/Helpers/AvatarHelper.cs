using System;
using System.Web;
using System.Web.Mvc;
using NGravatar;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Users.Models;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Helpers {
    public static class AvatarHelper {
        public const string DefaultAvatarUrl = "~/Themes/OrchardPros/Images/temp-profile.png";

        public static string AvatarUrl<T>(this UrlHelper urlHelper, UserProfilePart profile, Orchard.Mvc.ViewEngines.Razor.WebViewPage<T> view, int size) {
            return profile.AvatarType == AvatarType.Gravatar ? urlHelper.GravatarUrl(profile, size) : urlHelper.UploadedAvatarUrl(profile, view, size);
        }

        public static string UploadedAvatarUrl<T>(this UrlHelper urlHelper, UserProfilePart profile, Orchard.Mvc.ViewEngines.Razor.WebViewPage<T> view, int size) {
            var resizedMediaUrl = !String.IsNullOrWhiteSpace(profile.Avatar) ? view.Display.ResizeMediaUrl(Path: profile.Avatar, Width: size, Height: size, Mode: "crop").ToString() : default(string);
            return resizedMediaUrl ?? urlHelper.Content(String.Format(DefaultAvatarUrl, size));
        }

        public static string GravatarUrl(this UrlHelper urlHelper, UserProfilePart profile, int size) {
            return urlHelper.GravatarUrl(profile.As<UserPart>().Email, size);
        }

        public static string GravatarUrl(this UrlHelper urlHelper, string email, int size) {
            if (String.IsNullOrWhiteSpace(email))
                return null;

            var workContext = urlHelper.RequestContext.GetWorkContext();
            return new Gravatar {
                DefaultImage = VirtualPathUtility.AppendTrailingSlash(workContext.CurrentSite.BaseUrl) + urlHelper.Content(String.Format(DefaultAvatarUrl, size)),
                MaxRating = Rating.PG,
                Size = size
            }.GetImageSource(email);
        }
    }
}