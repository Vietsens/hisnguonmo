using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.UC.Feedback.Design.Template1.Validation;
using Inventec.UC.Feedback.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Design.Template1
{
    internal partial class Template1
    {
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

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void ValidationFeedbackTitle()
        {
            try
            {
                Title__ValidationRule titleRule = new Title__ValidationRule();
                titleRule.txtTitle = txtTitle;
                titleRule.ErrorText = MessageUtil.GetMessage(Message.Message.Enum.TruongDuLieuBatBuoc);
                titleRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTitle, titleRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationFeedbackContent()
        {
            try
            {
                Content__ValidationRule contentRule = new Content__ValidationRule();
                contentRule.txtContent = txtContent;
                contentRule.ErrorText = MessageUtil.GetMessage(Message.Message.Enum.TruongDuLieuBatBuoc);
                contentRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtContent, contentRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationFeedbackAuthor()
        {
            try
            {
                Author__ValidationRule authorRule = new Author__ValidationRule();
                authorRule.txtAuthor = txtAuthor;
                authorRule.ErrorText = MessageUtil.GetMessage(Message.Message.Enum.TruongDuLieuBatBuoc);
                authorRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtAuthor, authorRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
