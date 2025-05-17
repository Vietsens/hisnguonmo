using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ApprovaleDebateList.Resources;
using Inventec.Desktop.Common.Controls.ValidationRule;
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

namespace HIS.Desktop.Plugins.ApprovaleDebateList
{
    public partial class frmRejectApproval : HIS.Desktop.Utility.FormBase
    {
        public string RejectReason { get; set; }
        int positionHandle;

        public frmRejectApproval()
        {
            InitializeComponent();
        }

        private void frmRejectApproval_Load(object sender, EventArgs e)
        {
            SetIcon();
            txtReason.Text = RejectReason;
            //ValidContent();
            SetMaxlength(txtReason, 4000, true);
        }

        //private void ValidContent()
        //{
        //    MemoEditValidationRule spin = new MemoEditValidationRule();
        //    spin.txtTextEdit = txtReason;
        //    this.dxValidationProvider1.SetValidationRule(txtReason, spin);
        //}
        private void SetMaxlength(BaseEdit control, int maxlenght, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = IsRequired;
                validate.ErrorText = string.Format(ResourceMessage.NhapQuaMaxlength, maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
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

        //private void Validation(BaseEdit control)
        //{
        //    try
        //    {
        //        ControlEditValidationRule validRule = new ControlEditValidationRule();
        //        validRule.editor = control;
        //        validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //        validRule.ErrorType = ErrorType.Warning;
        //        dxValidationProvider1.SetValidationRule(control, validRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void ValidMaxlength(BaseEdit control)
        //{
        //    try
        //    {
        //        ControlMaxLengthValidationRule maxLength = new ControlMaxLengthValidationRule();
        //        maxLength.editor = control;
        //        maxLength.maxLength = 10;
        //        maxLength.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
        //        maxLength.ErrorType = ErrorType.Warning;
        //        dxValidationProvider1.SetValidationRule(control, maxLength);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dxValidationProvider1.Validate())
            {
                return;
            }
            RejectReason = txtReason.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
