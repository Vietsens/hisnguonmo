using System;
using System.Configuration;
using System.IO;

namespace HIS.Desktop.MIMS.Integration.Core
{
    public static class MimsConfig
    {
        public static string Username
        {
            get { return Get("MIMS.Username"); }
        }

        public static string Password
        {
            get { return Get("MIMS.Password"); }
        }

        public static string CdsApiUrl
        {
            get { return Get("MIMS.CDS.ApiUrl"); }
        }

        public static string VnContraApiUrl
        {
            get { return Get("MIMS.VNContra.ApiUrl"); }
        }

        public static string ResourceBasePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Get("MIMS.Resource.BasePath")); }
        }

        public static string StyleSheetPath
        {
            get { return Path.Combine(ResourceBasePath, Get("MIMS.StyleSheet.File")); }
        }

        private static string Get(string key)
        {
            var value = ConfigurationSettings.AppSettings[key];

            if (string.IsNullOrEmpty(value))
                throw new Exception(
                    string.Format("Thiếu cấu hình bắt buộc: {0}", key));

            return value;
        }
    }
}
