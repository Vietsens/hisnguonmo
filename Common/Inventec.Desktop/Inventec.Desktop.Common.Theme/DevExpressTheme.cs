using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using Microsoft.Win32;
using System;

namespace Inventec.Desktop.Common.Theme
{
    public class DevExpressTheme
    {
        private static string themeName = "";
        public static string THEME_NAME
        {
            get
            {
                try
                {
                    RegistryKey hfsFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);
                    themeName = (string)hfsFolder.GetValue(RegistryConstant.THEME_KEY);
                    return themeName;
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    RegistryKey hfsFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);
                    hfsFolder.SetValue(RegistryConstant.THEME_KEY, value);
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                }
            }
        }
    }
}
