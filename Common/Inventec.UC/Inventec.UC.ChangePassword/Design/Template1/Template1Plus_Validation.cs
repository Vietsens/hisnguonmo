using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.UC.ChangePassword.Validate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Design.Template1
{
    internal partial class Template1
    {

        #region Validation

        private void ValidControl()
        {
            try
            {
                ValidtxtOldPass();
                ValidtxtNewPass();
                ValidtxtRetypePass();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidtxtOldPass()
        {
            try
            {
                OldPass__ValidationRule oldPassRule = new OldPass__ValidationRule();
                oldPassRule.txtOldPass = txtPreviousPass;
                oldPassRule.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                oldPassRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtPreviousPass, oldPassRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidtxtNewPass()
        {
            try
            {
                NewPass__ValidationRule newPassRule = new NewPass__ValidationRule();
                newPassRule.txtNewPass = txtNewPass;
                newPassRule.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                newPassRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtNewPass, newPassRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidtxtRetypePass()
        {
            try
            {
                RetypePass__ValidationRule retypePassRule = new RetypePass__ValidationRule();
                retypePassRule.txtRetypePass = txtRetypePass;
                retypePassRule.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                retypePassRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtRetypePass, retypePassRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

    }
}
