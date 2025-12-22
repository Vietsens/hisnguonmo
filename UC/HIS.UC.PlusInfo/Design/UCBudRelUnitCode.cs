using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.WorkPlace;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.PlusInfo.Design
{
    // Delegate để nhận thông báo khi workplace thay đổi
    public delegate void DelegateWorkPlaceValueChanged(long? workPlaceId);

    public partial class UCBudRelUnitCode : UserControlBase
    {
        bool patientOld = false;
        DelegateWorkPlaceValueChanged workPlaceValueChanged;
        public UCBudRelUnitCode()
            : base("UCPlusInfo", "UCBudRelUnitCode")
        {
            try
            {
                InitializeComponent();
                //SetCaptionByLanguageKey();
                SetMaxlength(this.dvqhnsCode, 7);
                this.dvqhnsCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dvqhnsCode_KeyPress);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void dvqhnsCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region Get - Set Value

        internal string GetValue()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.dvqhnsCode.Text))
                    return this.dvqhnsCode.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        internal void SetValueDelegate(object dataSet)
        {
            try
            {
                if (dataSet != null)
                {
                    if (dataSet is MOS.EFMODEL.DataModels.HIS_WORK_PLACE)
                    {
                        MOS.EFMODEL.DataModels.HIS_WORK_PLACE data = (MOS.EFMODEL.DataModels.HIS_WORK_PLACE)dataSet;
                        if (data.BUD_REL_UNIT_CODE != null)
                        {
                            this.dvqhnsCode.EditValue = data.BUD_REL_UNIT_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        internal void SetValue(UCPlusInfoADO dvqhns, long patientID)
        {
            try
            {
                this.dvqhnsCode.EditValue = dvqhns.BUD_REL_UNIT_CODE;
                if (!string.IsNullOrEmpty(dvqhns.BUD_REL_UNIT_CODE))
                {
                    Inventec.Common.Logging.LogSystem.Debug(string.Format("SetValue: Đã gán BUD_REL_UNIT_CODE={0} từ dvqhns (fallback)", dvqhns.BUD_REL_UNIT_CODE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValueAddress(object address)
        {
            try
            {
                if (this.patientOld == false)
                    this.dvqhnsCode.Text = (string)address ?? "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Set delegate để nhận thông báo khi workplace thay đổi
        /// </summary>
        internal void SetDelegateWorkPlaceValueChanged(DelegateWorkPlaceValueChanged dlg)
        {
            try
            {
                this.workPlaceValueChanged = dlg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xử lý khi workplace thay đổi - tự động cập nhật BUD_REL_UNIT_CODE
        /// </summary>
        internal void OnWorkPlaceValueChanged(long? workPlaceId)
        {
            try
            {
                if (workPlaceId.HasValue && workPlaceId.Value > 0)
                {
                    // Lấy thông tin workplace từ BackendData
                    var workplace = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>()
                        .FirstOrDefault(o => o.ID == workPlaceId.Value);
                    
                    if (workplace != null && !string.IsNullOrEmpty(workplace.BUD_REL_UNIT_CODE))
                    {
                        // Chỉ gán nếu workplace có BUD_REL_UNIT_CODE
                        this.dvqhnsCode.EditValue = workplace.BUD_REL_UNIT_CODE;
                        Inventec.Common.Logging.LogSystem.Info(string.Format("OnWorkPlaceValueChanged: Đã cập nhật BUD_REL_UNIT_CODE={0} từ WorkPlace ID={1}", 
                            workplace.BUD_REL_UNIT_CODE, workPlaceId.Value));
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info(string.Format("OnWorkPlaceValueChanged: WorkPlace ID={0} không có BUD_REL_UNIT_CODE", workPlaceId.Value));
                    }
                }
                else
                {
                    // Xóa giá trị khi workplace bị xóa
                    if (this.patientOld == false)
                    {
                        this.dvqhnsCode.EditValue = null;
                        Inventec.Common.Logging.LogSystem.Info("OnWorkPlaceValueChanged: Đã xóa BUD_REL_UNIT_CODE vì workplace bị xóa");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            DisposeControl();
        }
        internal void DisposeControl()
        {
            try
            {
                patientOld = false;
                //dlgFocusNextUserControl = null;
                positionHandle = 0;
                this.dvqhnsCode.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.dvqhnsCode_KeyPress);
                //this.txtAddressNow.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtAddressNow_KeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.UCBudRelUnitCode_Load);
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                layoutControlItem1 = null;
                dvqhnsCode = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        private void SetMaxlength(BaseEdit control, int maxlenght)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = false;
                string message = string.Format("Vượt quá {0} ký tự", maxlenght);
                validate.ErrorText = message;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal bool ValidateRequiredField()
        {
            bool result = true;
            try
            {
                positionHandle = -1;
                result = dxValidationProvider1.Validate();
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        internal void ResetRequiredField()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UCBudRelUnitCode_Load(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        int positionHandle = -1;
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
