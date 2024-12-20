using DevExpress.XtraEditors.Controls;
using Inventec.Common.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Base
{
    public class LanguageManager
    {
        public enum LanguageEnum
        {
            VI,
            EN,
            LO,
        }
        public static string LanguageVi = "vi";
        public static string LanguageEn = "en";
        public static string LanguageLo = "lo";
        
        private static LanguageEnum currentEnum;
        
        private static string language;
        private static RegistryKey hfsFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.MOS_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);

        
        private static CultureInfo cultureInfo { get; set; }

        public static bool Init(CultureInfo current)
        {
            bool result = false;
            try
            {
                cultureInfo = current;
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }

        public static CultureInfo GetCulture()
        {
            CultureInfo result = null;
            try
            {
                if (cultureInfo == null)
                {
                    cultureInfo = new CultureInfo("vi");
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

        internal static object GetLanguage()
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
        public static bool Change(LanguageEnum languageEnum)
        {
            bool result = false;
            try
            {
                switch (languageEnum)
                {
                    case LanguageEnum.VI:
                        currentEnum = LanguageEnum.VI;
                        ChangeVi();
                        break;
                    case LanguageEnum.EN:
                        currentEnum = LanguageEnum.EN;
                        ChangeEn();
                        break;
                    case LanguageEnum.LO:
                        currentEnum = LanguageEnum.LO;
                        ChangeLo();
                        break;
                    default:
                        currentEnum = LanguageEnum.VI;
                        ChangeVi();
                        break;
                }

                try
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = LanguageManager.GetCulture();
                    // System.Threading.Thread.CurrentThread.CurrentUICulture = EXE.UTILITY.LanguageManager.GetCulture();
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = LanguageManager.GetCulture();
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


        private static void ChangeLo()
        {
            language = LanguageLo;
            hfsFolder.SetValue(RegistryConstant.LANGUAGE_KEY, language);
            cultureInfo = new CultureInfo("lo");
            MessageBoxManager.OK = "OK";
            MessageBoxManager.Cancel = "Cancel";
            MessageBoxManager.Yes = "Yes";
            MessageBoxManager.No = "No";

            Localizer.Active = new NewLocalizerDX();
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

        
    }
}
