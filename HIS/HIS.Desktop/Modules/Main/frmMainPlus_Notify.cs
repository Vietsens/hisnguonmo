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
using DevExpress.XtraBars.Ribbon;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        int timeNotify;
        int count;
        string optionFormNotify;

        public V_HIS_EMPLOYEE Employee { get; private set; }

        private string SubStringConfig(string config, int index)
        {
            string str = "";
            try
            {
                var strArray = config.Split(',');
                str = strArray[index];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
            return str;
        }

        private void RunNotify()
        {
            try
            {
                string configResult = ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__AUTO_SHOW_NOTIFY);
                timeNotify = Inventec.Common.TypeConvert.Parse.ToInt32(SubStringConfig(configResult, 0));
                count = Inventec.Common.TypeConvert.Parse.ToInt32(SubStringConfig(configResult, 1));
                optionFormNotify = ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__OPTION_FORM_NOTIFY);
                if (timeNotify > 0)
                {
                    Employee = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().User.LoginName);
                    System.Windows.Forms.Timer timerNotify = new System.Windows.Forms.Timer();
                    timerNotify.Interval = timeNotify;
                    //timerNotify.Enabled = true;
                    timerNotify.Tick += TimerNotify_Tick;
                    timerNotify.Start();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void TimerNotify_Tick(object sender, EventArgs e)
        {
            try
            {
                TheadProcessNotify();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void TheadProcessNotify()
        {
            try
            {
                Task.Factory.StartNew(ProcessNotify);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessNotify()
        {
            try
            {
                if (!CloseAllApp.IsProcessOpen("HIS.Desktop.Notify"))
                {
                    var tc = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                    string cmdLn = "";
                    cmdLn += "SdaBaseUri|" + ConfigSystems.URI_API_SDA;
                    cmdLn += "|ApplicationCode|" + GlobalVariables.APPLICATION_CODE;
                    cmdLn += "|TokenCode|" + (tc != null ? tc.TokenCode : "");
                    cmdLn += "|Time|" + timeNotify.ToString();
                    cmdLn += "|Count|" + count.ToString();
                    cmdLn += "|Loginname|" + (tc != null ? tc.User.LoginName : "");
                    cmdLn += "|OptionFormNotify|" + (optionFormNotify ?? "");
                    cmdLn += "|ReceiverDepartment|" + (Employee != null ? Employee.DEPARTMENT_CODE : "");
                    cmdLn += "|BranchName|" + BranchDataWorker.Branch.BRANCH_NAME;
                    Inventec.Common.Logging.LogSystem.Debug("cmdLn = " + cmdLn);

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Application.StartupPath + @"\Integrate\Notify\HIS.Desktop.Notify.exe";
                    startInfo.Arguments = "\"" + cmdLn + "\"";
                    Process.Start(startInfo);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
