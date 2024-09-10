namespace Inventec.Desktop.Common.LibraryMessage
{
    public partial class Message
    {
        public LanguageEnum Language;
        public string message;
        public Enum EnumBC;

        private static string defaultViMessage = "Không xác định được thông báo của phần mềm.";
        private static string defaultEnMessage = "Can not found message case.";

        public Message(LanguageEnum language, Enum en)
        {
            Language = language;
            EnumBC = en;
            message = GetMessage(en);
        }

        public enum LanguageEnum
        {
            Vietnamese,
            English,
            Mianmar
        }

        internal class LanguageName
        {
            internal const string VI = "vi";
            internal const string EN = "en";
            internal const string MY = "my";
        }

        public static string GetLanguageName(LanguageEnum type)
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
                case LanguageEnum.Mianmar:
                    result = LanguageName.MY;
                    break;
                default:
                    break;
            }
            return result;
        }

        public static LanguageEnum GetLanguageEnum(string languageName)
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
                case LanguageName.MY:
                    result = LanguageEnum.Mianmar;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
