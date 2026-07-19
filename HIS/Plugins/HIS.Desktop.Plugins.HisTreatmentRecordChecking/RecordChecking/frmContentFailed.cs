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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    public partial class frmContentFailed : FormBase
    {

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long? treatmentId;
        private Action<bool> dlgIsSuccess;
        public frmContentFailed(Inventec.Desktop.Common.Modules.Module _moduleData, long? treatmentId, Action<bool> dlgIsSuccess)
            : base(_moduleData)
        {
            InitializeComponent();
            this.moduleData = _moduleData;
            this.treatmentId = treatmentId;
            this.dlgIsSuccess = dlgIsSuccess;
            SetIcon();
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmContentFailed_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                txtRejectReason.Text = "";

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }


        private void btnAgree_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;

                HisTreatmentRejectStoreSDO RejectStoreSDO = new HisTreatmentRejectStoreSDO();
                RejectStoreSDO.RejectReason = txtRejectReason.Text;
                RejectStoreSDO.TreatmentId = (long)this.treatmentId;

                var resultData = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/RejectStore", ApiConsumers.MosConsumer, RejectStoreSDO, param);

                if (resultData != null)
                {
                    success = true;

                }
                if (dlgIsSuccess != null)
                {
                    dlgIsSuccess(success);
                }
                this.Close();

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnToIgnore_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

    }
}
