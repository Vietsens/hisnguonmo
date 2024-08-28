using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Base
{
    public class LanguageWorker
    {
        public static string languageVi = "vi";
        public static string languageEn = "en";
        public static string languageMy = "my";
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
    }
}
