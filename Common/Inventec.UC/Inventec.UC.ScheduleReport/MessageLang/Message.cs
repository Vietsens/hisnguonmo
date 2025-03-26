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

namespace Inventec.UC.ScheduleReport.MessageLang
{
    internal partial class Message
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
        }

        internal class LanguageName
        {
            internal const string VI = "VietNamese";
            internal const string EN = "English";
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
                default:
                    break;
            }
            return result;
        }
    }
}
