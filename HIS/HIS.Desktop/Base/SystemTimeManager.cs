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
using ACS.SDO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Base
{
    class SystemTimeManager
    {
        private static long timeSyncConfig;
        public static long TimeSyncConfig
        {
            get
            {
                if (timeSyncConfig <= 0)
                {
                    timeSyncConfig = Inventec.Common.TypeConvert.Parse.ToInt64(ConfigurationManager.AppSettings["His.Desktop.TimeSyncConfig"] ?? "300");
                }
                return timeSyncConfig;
            }
        }

        public static void SetTime(TimerSDO timerSDO)
        {
            try
            {
                bool isUpdateDate = false;
                bool isUpdateTimeZone = false;
                bool isUpdateTime = false;

                TimeZoneInfo timeZone = TimeZoneInfo.Local;
                if (timerSDO.TimeZoneId != timeZone.Id)
                {
                    isUpdateTimeZone = true;
                }
                LogSystem.Info("isUpdateTimeZone: " + isUpdateTimeZone);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timerSDO.LocalTime), timerSDO.LocalTime)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timerSDO.TimeZoneId), timerSDO.TimeZoneId)
                     + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timerSDO.DateNow), timerSDO.DateNow)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timerSDO.UniversalTime), timerSDO.UniversalTime));
                if (isUpdateTimeZone)
                {
                    LogSystem.Info("Dong bo TimeZone: local.timeZone.Id: " + timeZone.Id + "; server.TimeZoneId = " + timerSDO.TimeZoneId);
                    string timeZoneId = timerSDO.TimeZoneId;
                    SetTimezone(timeZoneId); 
                }
                else
                {
                    DateTime dateTime = DateTime.Now;
                    var dateTimeUnspec = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
                    DateTime universalTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
                    DateTime localTime = TimeZoneInfo.ConvertTime(universalTime, timeZone);
                    if (timerSDO.DateNow.Date != localTime.Date)
                    {
                        isUpdateDate = true;
                    }

                    long dtNow = Inventec.Common.TypeConvert.Parse.ToInt64(localTime.ToString("yyyyMMddHHmmss"));
                    long dtSv = timerSDO.LocalTime;

                    var a = dtNow - dtSv;
                    long lech = Math.Abs(dtNow - dtSv);
                    LogSystem.Info("Truoc khi dong bo thoi gian: dtNow = " + dtNow + "; dtSv = " + dtSv + "; lech = " + lech + "; timeZone.Id = " + timeZone.Id + "; server.TimeZoneId = " + timerSDO.TimeZoneId);
                    if (lech > TimeSyncConfig)
                    {
                        isUpdateTime = true;
                    }
                    if (isUpdateDate || isUpdateTime)
                    {
                        LogSystem.Info("Truoc khi dong bo thoi gian: dtNow = " + dtNow + "; dtSv = " + dtSv + "; lech = " + lech + "; timeZone.Id = " + timeZone.Id + "; server.TimeZoneId = " + timerSDO.TimeZoneId);

                        //if (MessageBox.Show(HIS.Desktop.Resources.ResourceCommon.ThongBaoBanCoMuonDongBoGioMayChu, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        //{
                        if (isUpdateDate)
                        {
                            LogSystem.Debug("Dong bo ngay: dtNow = " + dtNow + "; dtSv = " + dtSv + "; lech = " + lech + "; isUpdateDate = " + isUpdateDate + "; isUpdateTime = " + isUpdateTime + "; cau hinh gioi han thoi gian lech timeSyncConfig =" + timeSyncConfig);

                            string sysUIFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
                            if (sysUIFormat.Contains('/'))
                                sysUIFormat = sysUIFormat.Replace('/', '-');
                            string dateInYourSystemFormat = timerSDO.DateNow.ToString(sysUIFormat);
                            SetDate(dateInYourSystemFormat);
                        }

                        if (isUpdateTime)
                        {
                            LogSystem.Debug("Dong bo gio: dtNow = " + dtNow + "; dtSv = " + dtSv + "; lech = " + lech + "; isUpdateDate = " + isUpdateDate + "; isUpdateTime = " + isUpdateTime + "; cau hinh gioi han thoi gian lech timeSyncConfig =" + timeSyncConfig);

                            string timeInYourSystemFormat = timerSDO.DateNow.ToString("HH:mm:ss");
                            SetTime(timeInYourSystemFormat);
                        }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void SetTimezone(string timeZoneId)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeZoneId), timeZoneId));
                var proc = new System.Diagnostics.ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = @"C:\Windows\System32";
                proc.CreateNoWindow = false;
                proc.FileName = @"C:\Windows\System32\cmd.exe";
                proc.Verb = "runas";
                proc.Arguments = "/k tzutil.exe /s \"" + timeZoneId + "\"";
                proc.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.LoadUserProfile = true;
                System.Diagnostics.Process.Start(proc);                            

                System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();
                Info.Arguments = "/C ping 127.0.0.1 -n 2 && \"" + Application.ExecutablePath + "\"";
                Info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                System.Diagnostics.Process.Start(Info);
                Application.Exit(); 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void SetDate(string dateInYourSystemFormat)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dateInYourSystemFormat), dateInYourSystemFormat)
                       );
                var proc = new System.Diagnostics.ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = @"C:\Windows\System32";
                proc.CreateNoWindow = false;
                proc.FileName = @"C:\Windows\System32\cmd.exe";
                proc.Verb = "runas";
                proc.Arguments = "/C date " + dateInYourSystemFormat;
                proc.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.LoadUserProfile = true;

                System.Diagnostics.Process.Start(proc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void SetTime(string timeInYourSystemFormat)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeInYourSystemFormat), timeInYourSystemFormat));
                var proc = new System.Diagnostics.ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = @"C:\Windows\System32";
                proc.CreateNoWindow = false;
                proc.FileName = @"C:\Windows\System32\cmd.exe";
                proc.Verb = "runas";
                proc.LoadUserProfile = true;
                proc.Arguments = "/C time " + timeInYourSystemFormat;
                proc.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                System.Diagnostics.Process.Start(proc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
