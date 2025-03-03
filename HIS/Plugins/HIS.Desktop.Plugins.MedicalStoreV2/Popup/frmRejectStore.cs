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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MedicalStoreV2.ADO;
using HIS.Desktop.Plugins.MedicalStoreV2.Config;
using HIS.Desktop.Plugins.MedicalStoreV2.Validtion;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
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
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalStoreV2.Popup
{
    public partial class frmRejectStore : Form
    {
        internal TreatmentADO currentTreatment { get; set; }
        HIS.Desktop.Common.DelegateSelectData refeshData;
        Inventec.Desktop.Common.Modules.Module currentModule;
        int positionHandle = -1;

        public frmRejectStore(Inventec.Desktop.Common.Modules.Module currentModule, TreatmentADO currentMediRecord, DelegateSelectData refeshData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentTreatment = currentMediRecord;
                this.refeshData = refeshData;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void frmDataStore_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                ValidateForm();
                AssignLabel();
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
                //ValidateTextEdit(txtRejectStoreReason);
                ValidMaxlengthTxtReason();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AssignLabel()
        {
            try
            {
                lblPatientCode.Text = this.currentTreatment.TDL_PATIENT_CODE;
                lblPatientName.Text = this.currentTreatment.TDL_PATIENT_NAME;
                lblTreatmentCode.Text = this.currentTreatment.TREATMENT_CODE;
                txtRejectStoreReason.Text = this.currentTreatment.REJECT_STORE_REASON;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateTextEdit(BaseEdit textEdit)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                validRule.editor = textEdit;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtReason()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtRejectStoreReason;
                validateMaxLength.maxLength = 4000;
                dxValidationProvider1.SetValidationRule(txtRejectStoreReason, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource_frmRejectStore = new ResourceManager("HIS.Desktop.Plugins.MedicalStoreV2.Resources.Lang", typeof(frmRejectStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmRejectStore.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmRejectStore.bar1.Text", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.barButtonItem_Save.Caption = Inventec.Common.Resource.Get.Value("frmRejectStore.barButtonItem_Save.Caption", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.barButtonItem__Print.Caption = Inventec.Common.Resource.Get.Value("frmRejectStore.barButtonItem__Print.Caption", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.btnSaveData.Text = Inventec.Common.Resource.Get.Value("frmRejectStore.btnSaveData.Text", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmRejectStore.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmRejectStore.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmRejectStore.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmRejectStore.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmRejectStore.Text", Resources.ResourceLanguageManager.LanguageResource_frmRejectStore, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                CommonParam param = new CommonParam();

                if (!dxValidationProvider1.Validate())
                    return;

                HisTreatmentRejectStoreSDO treatmentStoreSDO = new HisTreatmentRejectStoreSDO();

                bool result = false;
                HIS_TREATMENT treatmentResult = null;
                if (currentTreatment != null && currentTreatment != null)
                {
                    treatmentStoreSDO.TreatmentId = currentTreatment.ID;
                    treatmentStoreSDO.RejectReason = txtRejectStoreReason.Text.Trim();
                    treatmentResult = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/RejectStore", ApiConsumers.MosConsumer, treatmentStoreSDO, param);
                    if (treatmentResult != null)
                    {
                        result = true;
                        if (this.refeshData != null)
                        {
                            this.refeshData(result);
                        }
                    }
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (result)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //PrintProcessByMediRecordCode(PrintTypeMediRecord.IN_BARCODE_MEDI_RECORD_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveData_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
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
    }
}
