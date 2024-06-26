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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
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

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class frmUnSample : Form
    {
        LisSampleADO ado;
        Action<LIS_SAMPLE> action;
        Action<bool> IsShowMessage;
        bool IsClickSave;
        string roomCode;
        private int positionHandle;

        public frmUnSample(LisSampleADO ado, Action<LIS_SAMPLE> action, string roomCode, Action<bool> IsShowMessage = default(Action<bool>))
        {
            InitializeComponent();
            try
            {
                this.ado = ado;
                this.action = action;
                this.IsShowMessage = IsShowMessage;
                this.roomCode = roomCode;
                SetIconFrm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmUnSample_Load(object sender, EventArgs e)
        {
            try
            {
                lblPatientCode.Text = ado.PATIENT_CODE; 
                lblPatientName.Text = ado.VIR_PATIENT_NAME;
                lblServiceReqCode.Text = ado.SERVICE_REQ_CODE;
                lblTreatmentCode.Text = ado.TREATMENT_CODE;
                lblBarcode.Text = ado.BARCODE;
                HIS.Desktop.Plugins.ConnectionTest.Validation.ValidateMaxLength valid = new HIS.Desktop.Plugins.ConnectionTest.Validation.ValidateMaxLength();
                valid.memoEdit = txtReason;
                valid.maxLength = 1000;
                valid.ErrorText = "Trường dữ liệu vượt quá 1000 ký tự";
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtReason, valid);
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
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                sdo.SampleId = ado.ID;
                sdo.RequestRoomCode = roomCode;
                sdo.CancelReason = txtReason.Text.Trim();
                var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Unsample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                this.action(curentSTT);
                IsClickSave = true;
                WaitingManager.Hide();
                #region Show message

                MessageManager.Show(this.ParentForm, param, curentSTT != null);
                #endregion
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                if (curentSTT != null)
                    this.Close();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmUnSample_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (IsShowMessage != null)
                    IsShowMessage(IsClickSave);
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

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null,null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
