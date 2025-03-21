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
namespace HIS.Desktop.LibraryMessage
{
    public partial class Message
    {
        public LanguageEnum Language;
        public string message;
        public Enum EnumBC;

        private static string defaultViMessage = "HIS00001";
        private static string defaultEnMessage = "HIS00001";

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
