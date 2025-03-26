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

namespace Inventec.UC.ChangePassword.Process
{
    internal class EventLogUtil
    {
        private static string lang = "VietNamese";

        public static string SetLog(EventLog.EventLog.Enum EventLogEnum)
        {
            try
            {
                string result = "";
                EventLog.EventLog EventLog = Inventec.UC.ChangePassword.EventLog.EventLogFrontend.Get(lang, EventLogEnum);
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
                EventLog.EventLog EventLog = Inventec.UC.ChangePassword.EventLog.EventLogFrontend.Get(lang, eventLogEnum);
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
