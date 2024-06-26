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
using His.EventLog;
using Inventec.Common.Logging;
using System;

namespace HIS.Desktop.EventLog
{
    public class EventLogUtil
    {
        private static string lang = "VietNamese";

        public static string SetLog(His.EventLog.Message.Enum EventLogEnum)
        {
            try
            {
                string result = "";
                Message EventLog = His.EventLog.FrontendMessage.Get(lang, EventLogEnum);
                if (EventLog != null && !String.IsNullOrEmpty(EventLog.message))
                {
                    result = EventLog.message;
                }
                else
                {
                    LogSystem.Error("Thu vien chua khai bao key.");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return "";
            }
        }

        public static string SetLog(His.EventLog.Message.Enum eventLogEnum, string[] extraMessage)
        {
            string result = "";
            try
            {
                Message EventLog = His.EventLog.FrontendMessage.Get(lang, eventLogEnum);
                if (EventLog != null && !String.IsNullOrEmpty(EventLog.message))
                {
                    result = EventLog.message;
                    try
                    {
                        result = String.Format(EventLog.message, extraMessage);
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error("Khong format duoc string.", ex);
                        result = EventLog.message;
                    }
                }
                else
                {
                    LogSystem.Error("Thu vien chua khai bao key.");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }
}
