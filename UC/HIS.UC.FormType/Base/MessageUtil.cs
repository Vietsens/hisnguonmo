using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His.UC.LibraryMessage;
using HIS.UC.FormType;

namespace HIS.UC.FormType.Base
{
    internal class MessageUtil
    {
        public static string GetMessage(Message.Enum MessageCaseEnum)
        {
            string result = "";
            try
            {
                Message messageCase =FontendMessage.Get(FormTypeConfig.Language, MessageCaseEnum);
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

        public static string GetMessage(Message.Enum MessageCaseEnum, string[] extraMessage)
        {
            string result = "";
            try
            {
                Message MessageCase = FontendMessage.Get(FormTypeConfig.Language, MessageCaseEnum);
                if (MessageCase != null)
                {
                    try
                    {
                        result = String.Format(MessageCase.message, extraMessage);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Co exception khi set message vao param.Messages co tham so phu.", ex);
                        result = String.Format(MessageCase.message);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi SetParam co tham so phu.", ex);
            }
            return result;
        }

        public static void SetMessage(CommonParam param, Message.Enum en)
        {
            try
            {
                Message message = FontendMessage.Get(FormTypeConfig.Language, en);
                if (message != null)
                {
                    param.Messages.Add(message.message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi SetParam.", ex);
            }
        }

        public static void SetMessage(CommonParam param, Message.Enum en, string extraMessage)
        {
            try
            {
                Message message = FontendMessage.Get(FormTypeConfig.Language, en);
                if (message != null)
                {
                    try
                    {
                        param.Messages.Add(String.Format(message.message, extraMessage));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Co exception khi set message vao param.Messages co tham so phu.", ex);
                        param.Messages.Add(message.message);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi SetParam co tham so phu.", ex);
            }
        }

        public static void SetMessage(CommonParam param,Message.Enum en, string[] extraMessage)
        {
            try
            {
                Message message = FontendMessage.Get(FormTypeConfig.Language, en);
                if (message != null)
                {
                    try
                    {
                        param.Messages.Add(String.Format(message.message, extraMessage));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Co exception khi set message vao param.Messages co tham so phu.", ex);
                        param.Messages.Add(message.message);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi SetParam co tham so phu.", ex);
            }
        }

        public static void SetParam(CommonParam param, Message.Enum MessageCaseEnum)
        {
            try
            {
                Message MessageCase = FontendMessage.Get(FormTypeConfig.Language, MessageCaseEnum);
                if (MessageCase != null)
                {
                    param.Messages.Add(MessageCase.message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi SetParam.", ex);
            }
        }

        public static void SetParamFirstPostion(CommonParam param, Message.Enum MessageCaseEnum)
        {
            try
            {
                Message MessageCase = FontendMessage.Get(FormTypeConfig.Language, MessageCaseEnum);
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
                    MessageUtil.SetParamFirstPostion(param, Message.Enum.HeThongTBKQXLYCCuaFrontendThanhCong);
                else
                    MessageUtil.SetParamFirstPostion(param, Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Co exception khi SetResultParam.", ex);
            }
        }
    }
}
