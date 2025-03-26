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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Process
{
    class MessageUtil
    {
        public static string GetMessage(Message.Message.Enum MessageCaseEnum)
        {
            string result = "";
            try
            {
                Message.Message messageCase = Message.FrontendMessage.Get(LanguageWorker.GetLanguage(), MessageCaseEnum);
                if (messageCase != null)
                {
                    result = messageCase.message;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi GetMessage.", ex);
            }
            return result;
        }

        public static void SetParamFirstPostion(CommonParam param, Message.Message.Enum MessageCaseEnum)
        {
            try
            {
                Message.Message MessageCase = Message.FrontendMessage.Get(LanguageWorker.GetLanguage(), MessageCaseEnum);
                if (MessageCase != null)
                {
                    param.Messages.Insert(0, MessageCase.message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi SetParam.", ex);
            }
        }

        public static string GetMessageAlert(CommonParam param)
        {
            string result = "";
            try
            {
                if (param.Messages != null && param.Messages.Count > 0)
                {
                    param.Messages = param.Messages.Distinct().ToList();
                    result = result + param.GetMessage();
                }
                if (param.BugCodes != null && param.BugCodes.Count > 0)
                {
                    param.BugCodes = param.BugCodes.Distinct().ToList();
                    result = result + "\r\nMã sự cố: " + param.GetBugCode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi GetMessageAlert.", ex);
            }
            return result;
        }

        public static void SetResultParam(CommonParam param, bool success)
        {
            try
            {
                if (success)
                    MessageUtil.SetParamFirstPostion(param, Message.Message.Enum.HeThongTBKQXLYCCuaFrontendThanhCong);
                else
                    MessageUtil.SetParamFirstPostion(param, Message.Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi SetResultParam.", ex);
            }
        }
    }
}
