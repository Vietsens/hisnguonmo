using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Base
{
    internal class MessageUtil
    {
        public static string GetMessage(Message.Message.Enum MessageCaseEnum)
        {
            string result = "";
            try
            {
                Message.Message messageCase = Message.FrontendMessage.Get(Base.LanguageWorker.GetLanguage(), MessageCaseEnum);
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

        public static string GetTextLabel(Label.ManagerLanguage.Enum textCaseEnum)
        {
            string result = "";
            try
            {
                Label.ManagerLanguage textLabelCase = Label.FrontendTextLabel.Get(Base.LanguageWorker.GetLanguage(), textCaseEnum);
                if (textLabelCase != null)
                {
                    result = textLabelCase.textLabel;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Co exception khi GetTextLabel.", ex);                
            }

            return result;
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
                LogSystem.Error("Co exception khi GetMessageAlert.", ex);
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
                LogSystem.Error("Co exception khi SetResultParam.", ex);
            }
        }

        public static void SetParamFirstPostion(CommonParam param, Message.Message.Enum MessageCaseEnum)
        {
            try
            {
                Message.Message MessageCase = Message.FrontendMessage.Get(Base.LanguageWorker.GetLanguage(), MessageCaseEnum);
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
    }
}
