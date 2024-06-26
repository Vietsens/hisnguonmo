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
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.SampleCollectionRoom.ADO;
using HIS.Desktop.Plugins.SampleCollectionRoom.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.SampleCollectionRoom
{
    public partial class frmCancelReason : HIS.Desktop.Utility.FormBase
    {
        DelegateRefreshData delegateSelectData;
        SampleListViewADO lisSample;
        int positionHandle = -1;
        string roomCode;
        public frmCancelReason()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        Inventec.Desktop.Common.Modules.Module currentModule;
        public frmCancelReason(Inventec.Desktop.Common.Modules.Module module, SampleListViewADO _lisSample, string _roomCode, DelegateRefreshData _delegateSelectData) :
            base(module)
        {
            InitializeComponent();
            this.lisSample = _lisSample;
            this.roomCode = _roomCode;
            this.delegateSelectData = _delegateSelectData;
            this.currentModule = module;
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmCancelReason_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                ValidateForm();
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultValue()
        {
            try
            {
                lblServiceReqCode.Text = lisSample.SERVICE_REQ_CODE;
                lblBarcode.Text = lisSample.BARCODE;
                lblTreatmentCode.Text = lisSample.TREATMENT_CODE;
                lblPatientCode.Text = lisSample.PATIENT_CODE;
                lblPatientName.Text = lisSample.VIR_PATIENT_NAME;
                txtCancelReason.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidateForm()
        {
            try
            {
                ControlMaxLengthValidationRule reasonValidate = new ControlMaxLengthValidationRule();
                reasonValidate.editor = txtCancelReason;
                reasonValidate.maxLength = 1000;
                reasonValidate.IsRequired = false;
                reasonValidate.ErrorText = "Trường dữ liệu vượt quá 1000 ký tự";
                reasonValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtCancelReason, reasonValidate);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                sdo.SampleId = lisSample.ID;
                sdo.RequestRoomCode = roomCode;
                sdo.CancelReason = txtCancelReason.Text.Trim();
                var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Unsample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                if (curentSTT != null)
                {
                    result = true;
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                if (result && this.delegateSelectData != null)
                {
                    this.delegateSelectData();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
    }
}
