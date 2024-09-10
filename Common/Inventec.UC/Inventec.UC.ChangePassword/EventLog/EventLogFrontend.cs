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
