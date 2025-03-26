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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Logging;
using Inventec.Core;

namespace Inventec.Desktop.Plugins.EventLog
{
    public partial class UCEventLog : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.UC.EventLogControl.ProcessHasException processHasException;
        private string treatmentCode;
        private string patientCode;
        private string serviceRequestCode;
        private string impMestCode;
        private string expMestCode;
        public UCEventLog(Inventec.UC.EventLogControl.ProcessHasException _processHasException, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            processHasException = _processHasException;
        }

        public UCEventLog(UC.EventLogControl.ProcessHasException exceptionApi, string treatmentCode, string patientCode, string serviceRequestCode, string impMestCode, string expMestCode, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.processHasException = exceptionApi;
            this.treatmentCode = treatmentCode;
            this.patientCode = patientCode;
            this.serviceRequestCode = serviceRequestCode;
            this.impMestCode = impMestCode;
            this.expMestCode = expMestCode;
        }

        private void UCEventLog_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.UC.EventLogControl.MainEventLog ucEventLog = new Inventec.UC.EventLogControl.MainEventLog();
                Inventec.UC.EventLogControl.Data.DataInit initData = new Inventec.UC.EventLogControl.Data.DataInit(EventLogConfig.SdaConsumer, EventLogConfig.NumPageSize, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());

                if (treatmentCode != null || patientCode != null || serviceRequestCode != null || impMestCode != null || expMestCode != null)
                {
                    Inventec.UC.EventLogControl.Data.DataInit initData1 = new Inventec.UC.EventLogControl.Data.DataInit(EventLogConfig.NumPageSize, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), treatmentCode, patientCode, serviceRequestCode, impMestCode, expMestCode);

                    var uc = ucEventLog.Init(Inventec.UC.EventLogControl.MainEventLog.EnumTemplate.TEMPLATE2, initData1);

                    uc.Dock = DockStyle.Fill;
                    this.Controls.Add(uc);

                    ucEventLog.MeShow(uc);
                    ucEventLog.SetDelegateHasException(uc, processHasException);
                }
                else
                {
                    var uc = ucEventLog.Init(Inventec.UC.EventLogControl.MainEventLog.EnumTemplate.TEMPLATE2, initData);

                    uc.Dock = DockStyle.Fill;
                    this.Controls.Add(uc);

                    ucEventLog.MeShow(uc);
                    ucEventLog.SetDelegateHasException(uc, processHasException);
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
