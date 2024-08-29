using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.MessageLang
{
    class FrontendMessage
    {
        private static Dictionary<Message.LanguageEnum, Dictionary<Message.Enum, Message>> dicMultiLanguage = new Dictionary<Message.LanguageEnum, Dictionary<Message.Enum, Message>>();
        private static Dictionary<Message.Enum, Message> dic = new Dictionary<Message.Enum, Message>();
        private static Object thisLock = new Object();

        public static Message Get(string languageName, Message.Enum enumBC)
        {
            Message result = null;
            Dictionary<Message.Enum, Message> dic = null;
            Message.LanguageEnum languageEnum = Message.GetLanguageEnum(languageName);
            if (!dicMultiLanguage.TryGetValue(languageEnum, out dic))
            {
                lock (thisLock)
                {
                    dic = new Dictionary<Message.Enum, Message>();
                    result = new Message(languageEnum, enumBC);
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
                        result = new Message(languageEnum, enumBC);
                    }
                    dic.Add(enumBC, result);
                }
            }
            return result;
        }
    }
}
