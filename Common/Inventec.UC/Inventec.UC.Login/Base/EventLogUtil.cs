using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Base
{
    internal class EventLogUtil
    {
        private static string lang = "VietNamese";

        public static string SetLog(EventLog.EventLog.Enum EventLogEnum)
        {
            try
            {
                string result = "";
                EventLog.EventLog EventLog = Inventec.UC.Login.EventLog.FrontendEventLog.Get(lang, EventLogEnum);
                if (EventLog != null && !String.IsNullOrEmpty(EventLog.Message))
                {
                    result = EventLog.Message;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Thu vien chua khai bao key.");
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "";
            }
        }

        public static string SetLog(EventLog.EventLog.Enum eventLogEnum, string[] extraMessage)
        {
            string result = "";
            try
            {
                EventLog.EventLog EventLog = Inventec.UC.Login.EventLog.FrontendEventLog.Get(lang, eventLogEnum);
                if (EventLog != null && !String.IsNullOrEmpty(EventLog.Message))
                {
                    result = EventLog.Message;
                    try
                    {
                        result = String.Format(EventLog.Message, extraMessage);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong format duoc string.", ex);
                        result = EventLog.Message;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Thu vien chua khai bao key.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }
}
