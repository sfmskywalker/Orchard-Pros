﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.Core.Common.Models;
using Orchard.Core.Settings.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Models;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Upgrade.Services;

namespace Upgrade.Controllers {
    [Admin]
    public class InfosetController : Controller {
        private readonly IOrchardServices _orchardServices;
        private readonly IUpgradeService _upgradeService;

        private const int BATCH = 50;

        public InfosetController(
            IOrchardServices orchardServices,
            IUpgradeService upgradeService) {
            _orchardServices = orchardServices;
            _upgradeService = upgradeService;

            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        
        public ActionResult Index() {
            return View();
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost() {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var site = _orchardServices.WorkContext.CurrentSite.As<SiteSettingsPart>();

            #region SiteSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Settings_SiteSettingsPartRecord"),
                (reader, connection) => {
                    site.HomePage = (string)reader["HomePage"];
                    site.PageSize = (int)reader["PageSize"];
                    site.PageTitleSeparator = (string)reader["PageTitleSeparator"];
                    site.ResourceDebugMode = (ResourceDebugMode)reader["ResourceDebugMode"];
                    site.SiteCulture = (string)reader["SiteCulture"];
                    site.SiteName = (string)reader["SiteName"];
                    site.SiteSalt = (string)reader["SiteSalt"];
                    site.SiteTimeZone = (string)reader["SiteTimeZone"];
                    site.SuperUser = (string)reader["SuperUser"];
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Settings_SiteSettingsPartRecord"), null);

            #endregion

            #region SiteSettings2PartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Settings_SiteSettings2PartRecord"),
                (reader, connection) => {
                    site.BaseUrl = (string)reader["BaseUrl"];
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Settings_SiteSettings2PartRecord"), null);

            #endregion

            #region ThemeSiteSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Themes_ThemeSiteSettingsPartRecord"),
                (reader, connection) => site.As<InfosetPart>().Store("ThemeSiteSettingsPart", "CurrentThemeName", (string)reader["CurrentThemeName"]));

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_Themes_ThemeSiteSettingsPartRecord"), null);


            #endregion

            #region AkismetSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_AntiSpam_AkismetSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("AkismetSettingsPart", "TrustAuthenticatedUsers", (bool)reader["TrustAuthenticatedUsers"]);
                    site.As<InfosetPart>().Store("AkismetSettingsPart", "ApiKey", reader["ApiKey"].ToString());
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_AntiSpam_AkismetSettingsPartRecord"), null);

            #endregion

            #region ReCaptchaSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_AntiSpam_ReCaptchaSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("ReCaptchaSettingsPart", "PublicKey", reader["PublicKey"].ToString());
                    site.As<InfosetPart>().Store("ReCaptchaSettingsPart", "PrivateKey", reader["PrivateKey"].ToString());
                    site.As<InfosetPart>().Store("ReCaptchaSettingsPart", "TrustAuthenticatedUsers", (bool)reader["TrustAuthenticatedUsers"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_AntiSpam_ReCaptchaSettingsPartRecord"), null);

            #endregion

            #region TypePadSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_AntiSpam_TypePadSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("TypePadSettingsPart", "ApiKey", reader["ApiKey"].ToString());
                    site.As<InfosetPart>().Store("TypePadSettingsPart", "TrustAuthenticatedUsers", (bool)reader["TrustAuthenticatedUsers"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_AntiSpam_TypePadSettingsPartRecord"), null);

            #endregion

            #region CacheSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_OutputCache_CacheSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("CacheSettingsPart", "DefaultCacheDuration", (int)reader["DefaultCacheDuration"]);
                    site.As<InfosetPart>().Store("CacheSettingsPart", "DefaultMaxAge", (int)reader["DefaultMaxAge"]);
                    site.As<InfosetPart>().Store("CacheSettingsPart", "VaryQueryStringParameters", (string)reader["VaryQueryStringParameters"]);
                    site.As<InfosetPart>().Store("CacheSettingsPart", "VaryRequestHeaders", (string)reader["VaryRequestHeaders"]);
                    site.As<InfosetPart>().Store("CacheSettingsPart", "IgnoredUrls", (string)reader["IgnoredUrls"]);
                    site.As<InfosetPart>().Store("CacheSettingsPart", "ApplyCulture", (bool)reader["ApplyCulture"]);
                    site.As<InfosetPart>().Store("CacheSettingsPart", "DebugMode", (bool)reader["DebugMode"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_OutputCache_CacheSettingsPartRecord"), null);
            #endregion

            #region CommentSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Comments_CommentSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("CommentSettingsPart", "ModerateComments", (bool)reader["ModerateComments"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_Comments_CommentSettingsPartRecord"), null);
            #endregion

            #region  MessageSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Messaging_MessageSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("MessageSettingsPart", "DefaultChannelService", (bool)reader["DefaultChannelService"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_Messaging_MessageSettingsPartRecord"), null);

            #endregion

            #region SearchSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Search_SearchSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("SearchSettingsPart", "SearchedFields", (string)reader["SearchedFields"]);
                    site.As<InfosetPart>().Store("SearchSettingsPart", "FilterCulture", (bool)reader["FilterCulture"]);
                    site.As<InfosetPart>().Store("SearchSettingsPart", "SearchIndex", (string)reader["SearchIndex"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_Search_SearchSettingsPartRecord"), null);

            #endregion

            #region RegistrationSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Users_RegistrationSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "UsersCanRegister", (bool)reader["UsersCanRegister"]);
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "UsersMustValidateEmail", (bool)reader["UsersMustValidateEmail"]);
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "UsersCanRegister", (bool)reader["UsersCanRegister"]);
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "ValidateEmailRegisteredWebsite", (bool)reader["ValidateEmailRegisteredWebsite"]);
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "ValidateEmailContactEMail", (bool)reader["ValidateEmailContactEMail"]);
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "UsersAreModerated", (bool)reader["UsersAreModerated"]);
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "NotifyModeration", (bool)reader["NotifyModeration"]);
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "NotificationsRecipients", (bool)reader["NotificationsRecipients"]);
                    site.As<InfosetPart>().Store("RegistrationSettingsPart", "EnableLostPassword", (bool)reader["EnableLostPassword"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_Users_RegistrationSettingsPartRecord"), null);

            #endregion

            #region SmtpSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Email_SmtpSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("SmtpSettingsPart", "Address", (string)reader["Address"]);
                    site.As<InfosetPart>().Store("SmtpSettingsPart", "Host", (string)reader["Host"]);
                    site.As<InfosetPart>().Store("SmtpSettingsPart", "Port", (int)reader["Port"]);
                    site.As<InfosetPart>().Store("SmtpSettingsPart", "EnableSsl", (bool)reader["EnableSsl"]);
                    site.As<InfosetPart>().Store("SmtpSettingsPart", "RequireCredentials", (bool)reader["RequireCredentials"]);
                    site.As<InfosetPart>().Store("SmtpSettingsPart", "UserName", (string)reader["UserName"]);
                    site.As<InfosetPart>().Store("SmtpSettingsPart", "Password", (string)reader["Password"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_Email_SmtpSettingsPartRecord"), null);

            #endregion

            #region WarmupSettingsPartRecord
            _upgradeService.ExecuteReader("SELECT * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Warmup_WarmupSettingsPartRecord"),
                (reader, connection) => {
                    site.As<InfosetPart>().Store("WarmupSettingsPart", "Urls", (string)reader["Urls"]);
                    site.As<InfosetPart>().Store("WarmupSettingsPart", "Scheduled", (bool)reader["Scheduled"]);
                    site.As<InfosetPart>().Store("WarmupSettingsPart", "Delay", (int)reader["Delay"]);
                    site.As<InfosetPart>().Store("WarmupSettingsPart", "OnPublish", (bool)reader["OnPublish"]);
                });

            _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_Warmup_WarmupSettingsPartRecord"), null);
            #endregion

            // todo: user records

            _orchardServices.Notifier.Information(T("Site Settings migrated successfully"));

            return View();
        }

        [HttpPost]
        public JsonResult MigrateBody(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var parts = _orchardServices.ContentManager
                .Query<BodyPart, BodyPartRecord>()
                .Where(x => x.Id > id)
                .OrderBy(x => x.Id)
                .Slice(0, BATCH).ToList();

            var lastContentItemId = id;

            foreach (var part in parts) {
                part.Text = part.Text;
                lastContentItemId = part.Id;
            }

            return new JsonResult { Data = lastContentItemId };
        }

        [HttpPost]
        public JsonResult MigrateMedia(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var parts = _orchardServices.ContentManager
                .Query<MediaPart, MediaPartRecord>()
                .Where(x => x.Id > id)
                .OrderBy(x => x.Id)
                .Slice(0, BATCH).ToList();

            var lastContentItemId = id;

            foreach (var part in parts) {
                part.MimeType = part.MimeType;
                part.Caption = part.Caption;
                part.AlternateText = part.AlternateText;
                part.FolderPath = part.FolderPath;
                part.FileName = part.FileName;
                lastContentItemId = part.Id;
            }

            return new JsonResult { Data = lastContentItemId };
        }
        
        [HttpPost]
        public JsonResult MigrateContentPermissionsPart(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var lastContentItemId = id;
            
            _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " * FROM " + _upgradeService.GetPrefixedTableName("Orchard_ContentPermissions_ContentPermissionsPartRecord") + " WHERE Id > " + id,
                (reader, connection) => {
                    lastContentItemId = (int) reader["Id"];
                    var contentPermissionPart = _orchardServices.ContentManager.Get(lastContentItemId);

                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "Enabled", reader["Enabled"].ToString());
                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "ViewContent", reader["ViewContent"].ToString());
                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "ViewOwnContent", reader["ViewOwnContent"].ToString());
                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "PublishContent", reader["PublishContent"].ToString());
                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "PublishOwnContent", reader["PublishOwnContent"].ToString());
                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "EditContent", reader["EditContent"].ToString());
                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "EditOwnContent", reader["EditOwnContent"].ToString());
                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "DeleteContent", reader["DeleteContent"].ToString());
                    contentPermissionPart.As<InfosetPart>().Store("ContentPermissionsPart", "DeleteOwnContent", reader["DeleteOwnContent"].ToString());
                });

            if (lastContentItemId == id) {
                // delete the table only when there is no more content to process
                _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Orchard_ContentPermissions_ContentPermissionsPartRecord"), null);
            }

            return new JsonResult { Data = lastContentItemId };
        }

        [HttpPost]
        public JsonResult MigrateContentMenuItemPart(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var lastContentItemId = id;
            
            _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " * FROM " + _upgradeService.GetPrefixedTableName("Orchard_ContentPicker_ContentMenuItemPartRecord") + " WHERE Id > " + id,
                (reader, connection) => {
                    lastContentItemId = (int)reader["Id"];
                    var contentPermissionPart = _orchardServices.ContentManager.Get(lastContentItemId);

                    contentPermissionPart.As<InfosetPart>().Store("ContentMenuItemPart", "ContentItemId", (int)reader["ContentMenuItemRecord_id"]);
                });

            return new JsonResult { Data = lastContentItemId };
        }

        [HttpPost]
        public JsonResult MigrateTagsPart(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var lastContentItemId = id;
            
            _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Tags_TagsPartRecord") + " WHERE Id > " + id,
                (reader, connection) => {
                    lastContentItemId = (int)reader["Id"];
                    var contentPermissionPart = _orchardServices.ContentManager.Get(lastContentItemId);

                    var tagNames = new List<string>();
                    _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " TR.TagName as TagName FROM "
                                                  + _upgradeService.GetPrefixedTableName("Orchard_Tags_ContentTagRecord") + " as CTR "
                                                  + " INNER JOIN " + _upgradeService.GetPrefixedTableName("Orchard_Tags_TagRecord") + " as TR "
                                                  + " ON CTR.TagRecord_Id = TR.Id"
                                                  + " WHERE TagsPartRecord_id = " + lastContentItemId, (r, c) => tagNames.Add((string) r["TagName"]));


                    contentPermissionPart.As<InfosetPart>().Store("TagsPart", "CurrentTags", String.Join(",", tagNames));
                });

            return new JsonResult { Data = lastContentItemId };
        }

        [HttpPost]
        public JsonResult MigrateWidgetPart(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var lastContentItemId = id;

            _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Widgets_WidgetPartRecord") + " WHERE Id > " + id,
                (reader, connection) => {
                    lastContentItemId = (int)reader["Id"];
                    var contentPermissionPart = _orchardServices.ContentManager.Get(lastContentItemId);

                    contentPermissionPart.As<InfosetPart>().Store("WidgetPart", "Title", (string)reader["Title"]);
                    contentPermissionPart.As<InfosetPart>().Store("WidgetPart", "Position", (string)reader["Position"]);
                    contentPermissionPart.As<InfosetPart>().Store("WidgetPart", "Zone", (string)reader["Zone"]);
                    contentPermissionPart.As<InfosetPart>().Store("WidgetPart", "RenderTitle", (bool)reader["RenderTitle"]);
                    contentPermissionPart.As<InfosetPart>().Store("WidgetPart", "Name", (string)reader["Name"]);
                });

            return new JsonResult { Data = lastContentItemId };
        }

        [HttpPost]
        public JsonResult MigrateLayerPart(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var lastContentItemId = id;

            _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " * FROM " + _upgradeService.GetPrefixedTableName("Orchard_Widgets_LayerPartRecord") + " WHERE Id > " + id,
                (reader, connection) => {
                    lastContentItemId = (int)reader["Id"];
                    var contentPermissionPart = _orchardServices.ContentManager.Get(lastContentItemId);

                    contentPermissionPart.As<InfosetPart>().Store("LayerPart", "Name", (string)reader["Name"]);
                    contentPermissionPart.As<InfosetPart>().Store("LayerPart", "Description", (string)reader["Description"]);
                    contentPermissionPart.As<InfosetPart>().Store("LayerPart", "LayerRule", (string)reader["LayerRule"]);
                });

            return new JsonResult { Data = lastContentItemId };
        }

        [HttpPost]
        public JsonResult MigrateMenuWidgetPart(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var lastContentItemId = id;

            _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " * FROM " + _upgradeService.GetPrefixedTableName("Navigation_MenuWidgetPartRecord") + " WHERE Id > " + id,
                (reader, connection) => {
                    lastContentItemId = (int)reader["Id"];
                    var contentPermissionPart = _orchardServices.ContentManager.Get(lastContentItemId);

                    contentPermissionPart.As<InfosetPart>().Store("MenuWidgetPart", "StartLevel", (int)reader["StartLevel"]);
                    contentPermissionPart.As<InfosetPart>().Store("MenuWidgetPart", "Levels", (int)reader["Levels"]);
                    contentPermissionPart.As<InfosetPart>().Store("MenuWidgetPart", "Breadcrumb", (bool)reader["Breadcrumb"]);
                    contentPermissionPart.As<InfosetPart>().Store("MenuWidgetPart", "AddHomePage", (bool)reader["AddHomePage"]);
                    contentPermissionPart.As<InfosetPart>().Store("MenuWidgetPart", "AddCurrentPage", (bool)reader["AddCurrentPage"]);
                    contentPermissionPart.As<InfosetPart>().Store("MenuWidgetPart", "MenuContentItemId", (int)reader["Menu_id"]);
                });

            if (lastContentItemId == id) {
                // delete the table only when there is no more content to process
                _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Navigation_MenuWidgetPartRecord"), null);
            }

            return new JsonResult { Data = lastContentItemId };
        }

        [HttpPost]
        public JsonResult MigrateShapeMenuItemPart(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var lastContentItemId = id;

            _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " * FROM " + _upgradeService.GetPrefixedTableName("Navigation_ShapeMenuItemPartRecord") + " WHERE Id > " + id,
                (reader, connection) => {
                    lastContentItemId = (int)reader["Id"];
                    var contentPermissionPart = _orchardServices.ContentManager.Get(lastContentItemId);

                    contentPermissionPart.As<InfosetPart>().Store("ShapeMenuItemPart", "ShapeType", (string)reader["ShapeType"]);
                });

            if (lastContentItemId == id) {
                // delete the table only when there is no more content to process
                _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Navigation_ShapeMenuItemPartRecord"), null);
            }

            return new JsonResult { Data = lastContentItemId };
        }


        [HttpPost]
        public JsonResult MigrateMenuItemPart(int id) {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
                throw new AuthenticationException("");

            var lastContentItemId = id;

            _upgradeService.ExecuteReader("SELECT TOP " + BATCH + " * FROM " + _upgradeService.GetPrefixedTableName("Navigation_MenuItemPartRecord") + " WHERE Id > " + id,
                (reader, connection) => {
                    lastContentItemId = (int)reader["Id"];
                    var contentPermissionPart = _orchardServices.ContentManager.Get(lastContentItemId);

                    contentPermissionPart.As<InfosetPart>().Store("MenuItemPart", "Url", (string)reader["Url"]);
                });

            if (lastContentItemId == id) {
                // delete the table only when there is no more content to process
                _upgradeService.ExecuteReader("DROP TABLE " + _upgradeService.GetPrefixedTableName("Navigation_MenuItemPartRecord"), null);
            }

            return new JsonResult { Data = lastContentItemId };
        }
    }
}
