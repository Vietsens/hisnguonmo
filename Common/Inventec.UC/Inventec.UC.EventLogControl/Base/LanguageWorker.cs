/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Base
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
