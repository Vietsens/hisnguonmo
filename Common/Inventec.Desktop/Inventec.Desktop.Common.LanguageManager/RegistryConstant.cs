
using System.Configuration;
namespace Inventec.Desktop.Common.LanguageManager
{
    public class RegistryConstant
    {
        public const string SOFTWARE_FOLDER = "SOFTWARE";
        public const string COMPANY_FOLDER = "INVENTEC";
        public static readonly string APP_FOLDER = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];

        public const string THEME_KEY = "ThemeName";
        public const string LANGUAGE_KEY = "Language";
    }
}
