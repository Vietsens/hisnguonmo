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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Label
{
    internal class FrontendTextLabel
    {
        private static Dictionary<ManagerLanguage.LanguageEnum, Dictionary<ManagerLanguage.Enum, ManagerLanguage>> dicMultiLanguage = new Dictionary<ManagerLanguage.LanguageEnum, Dictionary<ManagerLanguage.Enum, ManagerLanguage>>();
        private static Dictionary<ManagerLanguage.Enum, ManagerLanguage> dic = new Dictionary<ManagerLanguage.Enum, ManagerLanguage>();
        private static Object thisLock = new Object();

        public static ManagerLanguage Get(string languageName, ManagerLanguage.Enum enumBC)
        {
            ManagerLanguage result = null;
            Dictionary<ManagerLanguage.Enum, ManagerLanguage> dic = null;
            ManagerLanguage.LanguageEnum languageEnum = ManagerLanguage.GetLanguageEnum(languageName);
            if (!dicMultiLanguage.TryGetValue(languageEnum, out dic))
            {
                lock (thisLock)
                {
                    dic = new Dictionary<ManagerLanguage.Enum, ManagerLanguage>();
                    result = new ManagerLanguage(languageEnum, enumBC);
                    dic.Add(enumBC, result);
                }
                dicMultiLanguage.Add(languageEnum, dic);
            }
            else
            {
                if (!dic.TryGetValue(enumBC, out result))
                {
                    lock (thisLock)
                    {
                        result = new ManagerLanguage(languageEnum, enumBC);
                    }
                    dic.Add(enumBC, result);
                }
            }
            return result;
        }    
    }
}
