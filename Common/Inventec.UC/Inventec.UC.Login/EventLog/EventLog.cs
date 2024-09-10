using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.EventLog
{
    internal partial class EventLog
    {
        public LanguageEnum Language;
        public string Message;
        public Enum EnumBC;

        private static string defaultViMessage = "[].";
        private static string defaultEnMessage = "[].";

        internal EventLog(LanguageEnum language, Enum en)
        {
            Language = language;
            EnumBC = en;
            Message = GetMessage(en);
        }

        internal enum LanguageEnum
        {
            Vietnamese,
            English,
        }

        internal class LanguageName
        {
            internal const string VI = "VietNamese";
            internal const string EN = "English";
        }

        internal static string GetLanguageName(LanguageEnum type)
        {
            string result = LanguageName.VI;
            switch (type)
            {
                case LanguageEnum.Vietnamese:
                    result = LanguageName.VI;
                    break;
                case LanguageEnum.English:
                    result = LanguageName.EN;
                    break;
                default:
                    break;
            }
            return result;
        }

        internal static LanguageEnum GetLanguageEnum(string languageName)
        {
            LanguageEnum result = LanguageEnum.Vietnamese;
            switch (languageName)
            {
                case LanguageName.VI:
                    result = LanguageEnum.Vietnamese;
                    break;
                case LanguageName.EN:
                    result = LanguageEnum.English;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
