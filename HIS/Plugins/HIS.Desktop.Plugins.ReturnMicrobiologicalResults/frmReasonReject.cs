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
using DevExpress.XtraEditors;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Validation;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults
{
    public partial class frmReasonReject : Form
    {
        int positionHandle = -1;
        V_LIS_SAMPLE sample = null;
        public frmReasonReject(V_LIS_SAMPLE data)
        {
            InitializeComponent();
            this.sample = data;
            try
            {
                string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmReasonReject_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.ValidControl();
                lblBarcode.Text = sample.BARCODE ?? "";
                lblPatientCode.Text = sample.PATIENT_CODE ?? "";
                lblPatientName.Text = (sample.LAST_NAME ?? "") + " " + (sample.FIRST_NAME ?? "");
                lblServiceReqCode.Text = sample.SERVICE_REQ_CODE ?? "";
                lblTreatmentCode.Text = sample.TREATMENT_CODE ?? "";
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ReasonRejectValidationRule rule = new ReasonRejectValidationRule();
                rule.txtReason = txtReason;
                dxValidationProvider1.SetValidationRule(txtReason, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!btnSave.Enabled || this.sample == null) return;
                if (!dxValidationProvider1.Validate()) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                sdo.SampleId = sample.ID;
                sdo.RejectReason = txtReason.Text;
                var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Reject", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                if (curentSTT != null)
                {
                    success = true;
                    sample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                    sample.SAMPLE_ORDER = null;
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
