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
using log4net;
using System;

namespace Inventec.Common.Logging
{
    public static class LogSystem
    {
        private static ILog lf;
        private static ILog logFile
        {
            get
            {
                //load cau hinh
                if (!log4net.LogManager.GetRepository().Configured)
                {
                    log4net.Config.XmlConfigurator.Configure();
                }

                if (lf == null)
                {
                    lf = LogManager.GetLogger(typeof(LogSystem));
                }
                return lf;
            }
        }

        public static bool IsDebugEnabled()
        {
            return logFile.IsDebugEnabled;
        }

        public static bool IsInfoEnabled()
        {
            return logFile.IsInfoEnabled;
        }

        public static void Debug(string message)
        {
            Debug(message, null);
        }

        public static void Debug(Exception ex)
        {
            logFile.Debug(null, ex);
        }

        public static void Debug(string message, Exception ex)
        {
            logFile.Debug(message, ex);
        }

        public static void Info(string message)
        {
            logFile.Info(message);
        }

        public static void Warn(string message)
        {
            Warn(message, null);
        }

        public static void Warn(Exception ex)
        {
            Warn(null, ex);
        }

        public static void Warn(string message, Exception ex)
        {
            logFile.Warn(message, ex);
        }

        public static void Error(string message)
        {
            Error(message, null);
        }

        public static void Error(Exception ex)
        {
            Error(null, ex);
        }

        public static void Error(string message, Exception ex)
        {
            logFile.Error(message, ex);
        }

        public static void Fatal(string message)
        {
            Fatal(message, null);
        }

        public static void Fatal(Exception ex)
        {
            Fatal(null, ex);
        }

        public static void Fatal(string message, Exception ex)
        {
            logFile.Fatal(message, ex);
        }
    }
}
