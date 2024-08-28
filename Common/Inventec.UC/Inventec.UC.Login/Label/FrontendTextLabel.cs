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
