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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His.UC.LibraryMessage;
using Inventec.Common.Logging;

namespace HIS.UC.PlusInfo.Validate
{
    public class MessageUtil
    {
        public static string GetMessage(Message.Enum MessageCaseEnum)
        {
            string result = "";
            try
            {
                Message messageCase = His.UC.LibraryMessage.FontendMessage.Get(TokenStore.language, MessageCaseEnum);
                if (messageCase != null)
                {
                    result = messageCase.message;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi GetMessage.", ex);
            }
            return result;
        }
    }

    public class TokenStore
    {
        internal static string language { get; set; }
    }
}
