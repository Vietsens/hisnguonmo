using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Base
{
    internal class LanguageWorker
    {
        public static string languageVi = "vi";
        public static string languageEn = "en";
        private static string language = "";
        public static bool SetLanguage(string lang)
        {
            bool result = false;
            try
            {
                lang = String.IsNullOrEmpty(lang) ? languageVi : lang;
                //var ro = ApiConsumerStore.MosConsumer.Post<Inventec.Core.ApiResultObject<bool>>("/api/Token/UpdateLanguage", new Inventec.Core.CommonParam(), lang);
                //if (ro != null)
                //{
                //    result = ro.Data;
                //}
                result = true;
                language = lang;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static string GetLanguage()
        {
            string lang = "";
            try
            {
                if (String.IsNullOrEmpty(language))
                {
                    lang = languageVi;
                }
                else
                    lang = language;
            }
            catch (Exception)
            {
            }
            return lang;
        }

        public static CultureInfo GetCulture()
        {
            CultureInfo result = null;
            try
            {
                if (String.IsNullOrWhiteSpace(language))
                {
                    result = new CultureInfo(languageVi);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
