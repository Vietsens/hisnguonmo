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

namespace Inventec.UC.ChangePassword.EventLog
{
    internal class EventLogFrontend
    {
        private static Dictionary<EventLog.LanguageEnum, Dictionary<EventLog.Enum, EventLog>> dicMultiLanguage = new Dictionary<EventLog.LanguageEnum, Dictionary<EventLog.Enum, EventLog>>();
        private static Dictionary<EventLog.Enum, EventLog> dic = new Dictionary<EventLog.Enum, EventLog>();
        private static Object thisLock = new Object();

        public static EventLog Get(string languageName, EventLog.Enum enumBC)
        {
            EventLog result = null;
            Dictionary<EventLog.Enum, EventLog> dic = null;
            EventLog.LanguageEnum languageEnum = EventLog.GetLanguageEnum(languageName);
            if (!dicMultiLanguage.TryGetValue(languageEnum, out dic))
            {
                lock (thisLock)
                {
                    dic = new Dictionary<EventLog.Enum, EventLog>();
                    result = new EventLog(languageEnum, enumBC);
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
                        result = new EventLog(languageEnum, enumBC);
                    }
                    dic.Add(enumBC, result);
                }
            }
            return result;
        }
    }
}
