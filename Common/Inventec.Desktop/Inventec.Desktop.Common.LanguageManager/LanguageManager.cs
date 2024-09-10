using DevExpress.XtraEditors.Controls;
using Inventec.Common.Logging;
using Microsoft.Win32;
using System;
using System.Globalization;

namespace Inventec.Desktop.Common.LanguageManager
{
    public class LanguageManager
    {
        public enum LanguageEnum
        {
            VI,
            EN,
            MY
        }

        private static string languageViTitle = "Tiếng Việt";
        private static string languageEnTitle = "English";
        private static string languageMyTitle = "Myanmar";
        public static string LanguageVi = "vi";
        public static string LanguageEn = "en";
        public static string LanguageMy = "my";
        private static CultureInfo cultureInfo;
        private static string language;
        private static RegistryKey hfsFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);

        public static bool Init()
        {
            bool result = false;
            try
            {
                string temp = (hfsFolder.GetValue(RegistryConstant.LANGUAGE_KEY) ?? "").ToString();
                if (temp == LanguageEn)
                {
                    Change(LanguageEnum.EN);
                }
                else if (temp == LanguageVi)
                {
                    Change(LanguageEnum.VI);
                }
                else if (temp == LanguageMy)
                {
                    Change(LanguageEnum.MY);
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
                    case LanguageEnum.MY:
                        ChangeMy();
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
                    System.Threading.Thread.CurrentThread.CurrentCulture = LanguageManager.GetCulture();
                    //System.Threading.Thread.CurrentThread.CurrentUICulture = LanguageManager.GetCulture();
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = LanguageManager.GetCulture();
                    //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = LanguageManager.GetCulture();
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
            language = LanguageEn;
            hfsFolder.SetValue(RegistryConstant.LANGUAGE_KEY, language);
            cultureInfo = new CultureInfo("en");
            MessageBoxManager.OK = "OK";
            MessageBoxManager.Cancel = "Cancel";
            MessageBoxManager.Yes = "Yes";
            MessageBoxManager.No = "No";

            Localizer.Active = new NewLocalizerDX();
        }

        private static void ChangeMy()
        {
            language = LanguageMy;
            hfsFolder.SetValue(RegistryConstant.LANGUAGE_KEY, language);
            cultureInfo = new CultureInfo("my");
            MessageBoxManager.OK = "အဆင်ပြေလား";
            MessageBoxManager.Cancel = "မလုပ်တော့";
            MessageBoxManager.Yes = "ဟုတ်ကဲ့";
            MessageBoxManager.No = "မရှိ";

            Localizer.Active = new NewLocalizerDX();
        }

        private static void ChangeVi()
        {
            language = LanguageVi;
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
                result = LanguageVi;
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
                result = LanguageEn;
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Error(ex);
            }
            return result;
        }

        public static string GetLanguageMy()
        {
            string result = "";
            try
            {
                result = LanguageMy;
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
