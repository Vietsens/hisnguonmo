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
using AutoMapper;
using DevExpress.Skins;
using DevExpress.UserSkins;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Aup.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Core;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.Library.ConnectBloodPressure;
namespace HIS.Desktop
{
    internal class CloseAllApp
    {
        internal static void CloseAllApps()
        {
            try
            {
                var processLOGVPlus = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains("LOG.VPlus")).ToList();
                if (processLOGVPlus != null && processLOGVPlus.Count() > 0)
                {
                    for (int i = 0; i < processLOGVPlus.Count(); i++)
                    {
                        try
                        {
                            processLOGVPlus[i].Kill();
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Warn(ex);
                        }
                    }
                }

                var processHLS = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains("HLS.WCFClient")).ToList();
                if (processHLS != null && processHLS.Count() > 0)
                {
                    for (int i = 0; i < processHLS.Count(); i++)
                    {
                        try
                        {
                            processHLS[i].Kill();
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Warn(ex);
                        }
                    }
                }

                string exeNameSignProcessor = "EMR.SignProcessor";
                var processSignService = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName == exeNameSignProcessor || o.ProcessName == String.Format("{0}.exe", exeNameSignProcessor) || o.ProcessName == String.Format("{0} (32 bit)", exeNameSignProcessor) || o.ProcessName == String.Format("{0}.exe (32 bit)", exeNameSignProcessor)).ToList();
                if (processSignService != null && processSignService.Count() > 0)
                {
                    for (int i = 0; i < processSignService.Count(); i++)
                    {
                        try
                        {
                            processSignService[i].Kill();
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Warn(ex);
                        }
                    }
                }
                string exeNameWCF = "WCF";
                var processWCF = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName == exeNameWCF || o.ProcessName == String.Format("{0}.exe", exeNameWCF) || o.ProcessName == String.Format("{0} (32 bit)", exeNameWCF) || o.ProcessName == String.Format("{0}.exe (32 bit)", exeNameWCF)).ToList();
                if (processWCF != null && processWCF.Count() > 0)
                {
                    for (int i = 0; i < processWCF.Count(); i++)
                    {
                        try
                        {
                            processWCF[i].Kill();
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Warn(ex);
                        }
                    }
                }

                try
                {
                    StartAppPrintBartenderProcessor.KillAppPrintBartender();
                    ConnectBloodPressureProcessor.KillAppBloodPressure();
                }
                catch (Exception exx)
                {
                    LogSystem.Warn(exx);
                }
                

                if (!IsProcessHasOpen("HIS.Desktop.Notify.exe"))
                {
                    var processNotify = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains("HIS.Desktop.Notify")).ToList();
                    if (processNotify != null && processNotify.Count() > 0)
                    {
                        for (int i = 0; i < processNotify.Count(); i++)
                        {
                            try
                            {
                                processNotify[i].Kill();
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Warn(ex);
                            }
                        }
                    }
                }

                try
                {
                    string exeNameUAS = "UAS";
                    var processUAS = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName == exeNameUAS || o.ProcessName == String.Format("{0}.exe", exeNameUAS) || o.ProcessName == String.Format("{0} (32 bit)", exeNameUAS) || o.ProcessName == String.Format("{0}.exe (32 bit)", exeNameUAS)).ToList();
                    if (processUAS != null && processUAS.Count() > 0)
                    {
                        for (int i = 0; i < processUAS.Count(); i++)
                        {
                            try
                            {
                                processUAS[i].Kill();
                            }
                            catch (Exception ex)
                            {
                                LogSystem.Warn(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                Application.Exit();
            }
            catch (Exception ex)
            {
                LogSystem.Debug("Kill process HIS.Desktop.Notify | HLS.WCFClient that bai. ", ex);
            }
        }
        internal static bool IsProcessOpen(string name)
        {
			try
			{
                LogSystem.Debug(String.Format("Ứng dụng {0}.", name));
                foreach (Process clsProcess in Process.GetProcesses())
                {
                    if (clsProcess.ProcessName.Contains(name))
                    {
                        return true;
                    }
                }
            }
			catch (Exception ex)
			{
                LogSystem.Debug(String.Format("Xảy ra lỗi khi kiểm tra ứng dụng {0}.",name), ex);
            }

            return false;
        }

        internal static bool IsProcessOpenExact(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsProcessHasOpen(string name)
        {
            var processByNames = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains(name)).ToList();
            if (processByNames != null && processByNames.Count >= 2)
            {
                return true;
            }
            return false;
        }
    }
}
