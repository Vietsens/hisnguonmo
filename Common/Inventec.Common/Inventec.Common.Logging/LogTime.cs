using log4net;
using System;

namespace Inventec.Common.Logging
{
    public static class LogTime
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
                    lf = LogManager.GetLogger(typeof(LogTime));
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
