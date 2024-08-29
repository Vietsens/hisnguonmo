using DevExpress.XtraEditors.Controls;
using Inventec.Common.Logging;
using Microsoft.Win32;
using System;
using System.Globalization;

namespace Inventec.Common.RichEditor.Base
{
    public class LanguageManager
    {
        public enum LanguageEnum
        {
            VI,
            EN,
        }

        private static string languageVi = "Tiếng Việt";
        private static string languageEn = "English";
        private static CultureInfo cultureInfo;
        private static string language;
        private static RegistryKey hfsFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.MOS_FOLDER).CreateSubKey(RegistryConstant.HFS_FOLDER);

        public static bool Init()
        {
            bool result = false;
            try
            {
                string temp = (hfsFolder.GetValue(RegistryConstant.LANGUAGE_KEY) ?? "").ToString();
                if (temp == languageEn)
                {
                    Change(LanguageEnum.EN);
                }
                else if (temp == languageVi)
                {
                    Change(LanguageEnum.VI);
                }
                else
                {
                    Change(LanguageEnum.VI); //default if registry is empty
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }

        public static bool Change(LanguageEnum languageEnum)
        {
            bool result = false;
            try
            {
                switch (languageEnum)
                {
                    case LanguageEnum.VI:
                        ChangeVi();
                        break;
                    case LanguageEnum.EN:
                        ChangeEn();
                        break;
                    default:
                        ChangeVi();
                        break;
                }

                try
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = EXE.MANAGER.Base.LanguageManager.GetCulture();
                    // System.Threading.Thread.CurrentThread.CurrentUICulture = EXE.UTILITY.LanguageManager.GetCulture();
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = EXE.MANAGER.Base.LanguageManager.GetCulture();
                    //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = EXE.UTILITY.LanguageManager.GetCulture();
                }
                catch (Exception ex)
                {
                    LogSystem.Warn("Set CurrentCulture loi: " + ex);
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }

        private static void ChangeEn()
        {
            language = languageEn;
            hfsFolder.SetValue(RegistryConstant.LANGUAGE_KEY, language);
            cultureInfo = new CultureInfo("en");
            MessageBoxManager.OK = "OK";
            MessageBoxManager.Cancel = "Cancel";
            MessageBoxManager.Yes = "Yes";
            MessageBoxManager.No = "No";

            Localizer.Active = new NewLocalizerDX();
        }

        private static void ChangeVi()
        {
            language = languageVi;
            hfsFolder.SetValue(RegistryConstant.LANGUAGE_KEY, language);
            cultureInfo = new CultureInfo("vi");
            MessageBoxManager.OK = "Đồng ý";
            MessageBoxManager.Cancel = "Hủy bỏ";
            MessageBoxManager.Yes = "Có";
            MessageBoxManager.No = "Không";

            Localizer.Active = new NewLocalizerDX();
        }

        public static string GetLanguage()
        {
            string result = "";
            try
            {
                if (String.IsNullOrWhiteSpace(language))
                {
                    Change(LanguageEnum.VI);
                }
                result = language;
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Error(ex);
            }
            return result;
        }

        public static string GetLanguageVi()
        {
            string result = "";
            try
            {
                result = languageVi;
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Error(ex);
            }
            return result;
        }

        public static string GetLanguageEn()
        {
            string result = "";
            try
            {
                result = languageEn;
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Error(ex);
            }
            return result;
        }

        public static CultureInfo GetCulture()
        {
            CultureInfo result = null;
            try
            {
                if (String.IsNullOrWhiteSpace(language))
                {
                    Change(LanguageEnum.VI);
                }
                result = cultureInfo;
            }
            catch (Exception ex)
            {
                result = null;
                LogSystem.Error(ex);
            }
            return result;
        }
    }
}
