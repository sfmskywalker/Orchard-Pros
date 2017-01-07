using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Orchard;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    public interface ILocalizationManagementService : IDependency {
        void InstallTranslation(byte[] zippedTranslation, string sitePath);
        byte[] PackageTranslations(string cultureCode, string sitePath);
        byte[] PackageTranslations(string cultureCode, string sitePath, IEnumerable<string> extensionNames);
        byte[] ExtractDefaultTranslation(string sitePath);
        byte[] ExtractDefaultTranslation(string sitePath, IEnumerable<string> extensionNames);
        void SyncTranslation(string sitePath, string cultureCode);
    }

    [OrchardFeature("Vandelay.TranslationManager")]
    public class LocalizationManagementService : ILocalizationManagementService
    {
        public byte[] ExtractDefaultTranslation(string sitePath)
        {
            throw new NotImplementedException();
        }

        public byte[] ExtractDefaultTranslation(string sitePath, IEnumerable<string> extensionNames)
        {
            throw new NotImplementedException();
        }

        public void InstallTranslation(byte[] zippedTranslation, string sitePath)
        {
            throw new NotImplementedException();
        }

        public byte[] PackageTranslations(string cultureCode, string sitePath)
        {
            throw new NotImplementedException();
        }

        public byte[] PackageTranslations(string cultureCode, string sitePath, IEnumerable<string> extensionNames)
        {
            throw new NotImplementedException();
        }

        public void SyncTranslation(string sitePath, string cultureCode)
        {
            throw new NotImplementedException();
        }
    }
}