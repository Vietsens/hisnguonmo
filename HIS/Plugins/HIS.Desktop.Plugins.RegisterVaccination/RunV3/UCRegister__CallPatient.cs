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
using HIS.Desktop.Utility;
using System.Threading;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private void CreateThreadCallPatient()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CallPatientNewThread));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void CallPatientNewThread()
        {
            try
            {
                this.CallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallPatient()
        {
            try
            {
                if (!this.btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrEmpty(this.txtStepNumber.Text) || String.IsNullOrEmpty(this.txtGateNumber.Text))
                    return;
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonGoiBenhNhanBangCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.CallNumOrder(int.Parse(txtGateNumber.Text), int.Parse(this.txtStepNumber.Text));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReCallPatient()
        {
            try
            {
                if (!btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrEmpty(txtStepNumber.Text) || String.IsNullOrEmpty(txtGateNumber.Text))
                    return;
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonGoiBenhNhanBangCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.RecallNumOrder(int.Parse(txtGateNumber.Text), int.Parse(txtStepNumber.Text));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
