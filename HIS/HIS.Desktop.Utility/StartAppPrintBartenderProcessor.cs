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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    public class StartAppPrintBartenderProcessor
    {
        public static bool OpenAppPrintBartender()
        {
            try
            {
                if (IsProcessOpen("Bartender.Print"))
                {
                    return true;
                }
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Application.StartupPath + @"\Integrate\Bartender\Bartender.Print.exe";
                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private static bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool KillAppPrintBartender()
        {
            try
            {
                var processBartender = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains("Bartender.Print")).ToList();
                if (processBartender != null && processBartender.Count() > 0)
                {
                    for (int i = 0; i < processBartender.Count(); i++)
                    {
                        try
                        {
                            processBartender[i].Kill();
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }
    }
}
