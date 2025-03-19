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
using His.UC.LibraryMessage;
using Inventec.Common.Logging;
using Inventec.Core;
using System;

namespace Inventec.Common.WordContent.Base
{
    public class MessageUtil
    {
        public static string GetMessage(Message.Enum MessageCaseEnum)
        {
            string result = "";
            try
            {
                Message messageCase = His.UC.LibraryMessage.FontendMessage.Get(LanguageWorker.language, MessageCaseEnum);
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

        public static string GetMessage(Message.Enum MessageCaseEnum, string[] extraMessage)
        {
            string result = "";
            try
            {
                Message MessageCase = His.UC.LibraryMessage.FontendMessage.Get(LanguageWorker.language, MessageCaseEnum);
                if (MessageCase != null)
                {
                    try
                    {
                        result = String.Format(MessageCase.message, extraMessage);
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error("Co exception khi set message vao param.listMessage co tham so phu.", ex);
                        result = String.Format(MessageCase.message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi SetParam co tham so phu.", ex);
            }
            return result;
        }

        public static void SetMessage(CommonParam param, His.UC.LibraryMessage.Message.Enum en)
        {
            try
            {
                Message message = His.UC.LibraryMessage.FontendMessage.Get(LanguageWorker.language, en);
                if (message != null)
                {
                    param.Messages.Add(message.message);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi SetParam.", ex);
            }
        }

        public static void SetMessage(CommonParam param, His.UC.LibraryMessage.Message.Enum en, string extraMessage)
        {
            try
            {
                Message message = His.UC.LibraryMessage.FontendMessage.Get(LanguageWorker.language, en);
                if (message != null)
                {
                    try
                    {
                        param.Messages.Add(String.Format(message.message, extraMessage));
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error("Co exception khi set message vao param.Messages co tham so phu.", ex);
                        param.Messages.Add(message.message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi SetParam co tham so phu.", ex);
            }
        }

        public static void SetMessage(CommonParam param, His.UC.LibraryMessage.Message.Enum en, string[] extraMessage)
        {
            try
            {
                Message message = His.UC.LibraryMessage.FontendMessage.Get(LanguageWorker.language, en);
                if (message != null)
                {
                    try
                    {
                        param.Messages.Add(String.Format(message.message, extraMessage));
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error("Co exception khi set message vao param.Messages co tham so phu.", ex);
                        param.Messages.Add(message.message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi SetParam co tham so phu.", ex);
            }
        }

        public static void SetParam(CommonParam param, Message.Enum MessageCaseEnum)
        {
            try
            {
                Message MessageCase = His.UC.LibraryMessage.FontendMessage.Get(LanguageWorker.language, MessageCaseEnum);
                if (MessageCase != null)
                {
                    param.Messages.Add(MessageCase.message);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi SetParam.", ex);
            }
        }

        public static void SetParamFirstPostion(CommonParam param, Message.Enum MessageCaseEnum)
        {
            try
            {
                Message MessageCase = His.UC.LibraryMessage.FontendMessage.Get(LanguageWorker.language, MessageCaseEnum);
                if (MessageCase != null)
                {
                    param.Messages.Insert(0, MessageCase.message);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi SetParam.", ex);
            }
        }

        public static void SetParam(CommonParam param, Message.Enum MessageCaseEnum, string[] extraMessage)
        {
            try
            {
                Message MessageCase = His.UC.LibraryMessage.FontendMessage.Get(LanguageWorker.language, MessageCaseEnum);
                if (MessageCase != null)
                {
                    try
                    {
                        param.Messages.Add(String.Format(MessageCase.message, extraMessage));
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error("Co exception khi set message vao param.listMessage co tham so phu.", ex);
                        param.Messages.Add(MessageCase.message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi SetParam co tham so phu.", ex);
            }
        }

        public static string GetMessageAlert(CommonParam param)
        {
            string result = "";
            try
            {
                if (param.Messages != null && param.Messages.Count > 0)
                {
                    result = result + param.GetMessage();
                }
                if (param.BugCodes != null && param.BugCodes.Count > 0)
                {
                    result = result + "\r\nMã sự cố: " + param.GetBugCode();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi GetMessageAlert.", ex);
            }
            return result;
        }

        public static void SetResultParam(CommonParam param, bool success)
        {
            try
            {
                if (success)
                    MessageUtil.SetParamFirstPostion(param, His.UC.LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThanhCong);
                else
                    MessageUtil.SetParamFirstPostion(param, His.UC.LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai);
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi SetResultParam.", ex);
            }
        }
    }
}
