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
using Inventec.Core;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.LocalStorage.LocalData;
using System.IO;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Utilities
{
    public class LogThreadSessionProcess
    {
        Action sessionProcess;
        string sessionThreadName;
        string ModuleLink;

        public LogThreadSessionProcess(Action _sessionProcess, string _sessionThreadName, string _moduleLink)
        {
            this.sessionProcess = _sessionProcess;
            this.sessionThreadName = _sessionThreadName;
            this.ModuleLink = _moduleLink;
        }

        public void Run()
        {
            try
            {
                if (this.sessionProcess != null)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    this.sessionProcess();

                    // Thời gian kết thúc
                    watch.Stop();

                    //Tổng thời gian thực hiện 
                    LogSystem.Debug(ModuleLink + "____" + sessionThreadName + ": ElapsedMilliseconds = " + watch.ElapsedMilliseconds);
                    //INFO 2022-06-24 10:05:47,232 [1] - HIS____2.181.0____0,188____HIS.Desktop.Plugins.AssignPrescriptionPK____ActionLink____Loginname____192.168.1.18____CustomerCode
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), this.ModuleLink, sessionThreadName, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }
    }
}
