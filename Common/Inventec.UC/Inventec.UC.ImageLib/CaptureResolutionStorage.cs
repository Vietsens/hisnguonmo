using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ImageLib
{
    internal class CaptureResolutionStorage
    {
        internal const string SOFTWARE_FOLDER = "SOFTWARE";
        internal const string COMPANY_FOLDER = "INVENTEC";
        internal static readonly string APP_FOLDER = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];

        internal const string RESOLUTION_KEY = "CaptureResolution";

        private static RegistryKey appFolder = Registry.CurrentUser.CreateSubKey(SOFTWARE_FOLDER).CreateSubKey(COMPANY_FOLDER).CreateSubKey(APP_FOLDER);

        internal static void ChangeCaptureResolution(string resolution)
        {
            try
            {
                appFolder.SetValue(RESOLUTION_KEY, resolution);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static string GetCaptureResolution()
        {
            string resolution = "";
            try
            {
                resolution = (appFolder.GetValue(RESOLUTION_KEY, "") ?? "").ToString();
            }
            catch (Exception ex)
            {
                resolution = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return resolution;
        }
    }
}
