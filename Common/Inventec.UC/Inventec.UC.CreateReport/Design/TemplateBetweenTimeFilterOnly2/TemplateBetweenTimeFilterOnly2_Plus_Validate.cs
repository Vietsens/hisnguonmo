﻿using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.UC.CreateReport.Base;
using Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly2.Validate;
using Inventec.UC.CreateReport.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly2
{
    internal partial class TemplateBetweenTimeFilterOnly2
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
        private void ValidReportTemplate()
        {
            try
            {
                ReportTemplateValidationRule editValidationRuleCombo = new ReportTemplateValidationRule();
                editValidationRuleCombo.cboReportTemplate = cboReportTemplate;
                editValidationRuleCombo.txtReportTemplateCode = txtReportTemplateCode;
                editValidationRuleCombo.ErrorText = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
                editValidationRuleCombo.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtReportTemplateCode, editValidationRuleCombo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidReportTimeFrom()
        {
            try
            {
                TimeFromValidationRule editValidationRuleCombo = new TimeFromValidationRule();
                editValidationRuleCombo.deTimeFromDate = dtTimeFrom;
                editValidationRuleCombo.deTimeToDate = dtTimeFrom;
                editValidationRuleCombo.ErrorText = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
                editValidationRuleCombo.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtTimeFrom, editValidationRuleCombo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidReportTimeTo()
        {
            try
            {
                TimeToValidationRule editValidationRuleCombo = new TimeToValidationRule();
                editValidationRuleCombo.deTimeFromDate = dtTimeTo;
                editValidationRuleCombo.deTimeToDate = dtTimeTo;
                editValidationRuleCombo.ErrorText = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
                editValidationRuleCombo.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtTimeTo, editValidationRuleCombo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void ValidControl()
        {
            try
            {
                ValidReportTemplate();
                ValidReportTimeFrom();
                ValidReportTimeTo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
