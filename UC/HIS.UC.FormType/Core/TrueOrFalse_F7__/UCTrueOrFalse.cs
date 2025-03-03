﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.UC.FormType.Core.TrueOrFalse.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.Core.TrueOrFalse
{
    public partial class UCTrueOrFalse : System.Windows.Forms.UserControl
    {
        TrueOrFalseFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        const string StrOutput0 = "_OUTPUT0:";
        string Output0 = "";
        string JsonOutput = "";


        public UCTrueOrFalse(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 30;

                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.config = config;
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void Init()
        {
            try
            {
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    SetValidation();
                    
                }
                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                JsonOutput = this.config.JSON_OUTPUT;
                RemoveStrOutput0(ref JsonOutput);
                if (!string.IsNullOrWhiteSpace(Output0))
                {
                    if (Output0 == "true") radTrue.Checked = true;
                    else if (Output0 == "false") radFalse.Checked = true;
                }
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetValueOutput0(string JSON_OUTPUT, ref string Output0)
        {
            try
            {
                //string JSON_OUTPUT = "sdfsdf_OUTPUT0:2x";
                int lastIndex = JSON_OUTPUT.LastIndexOf(StrOutput0);
                if (lastIndex >= 0)
                {
                    Output0 = JSON_OUTPUT.Substring(lastIndex + StrOutput0.Length);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RemoveStrOutput0(ref string JsonOutput)
        {
            try
            {
                //string JSON_OUTPUT = "sdfsdf_OUTPUT0:2x";
                int lastIndex = JsonOutput.LastIndexOf(StrOutput0);
                if (lastIndex >= 0)
                {
                    JsonOutput = JsonOutput.Substring(0, lastIndex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        } 

        void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    radTrue.Text = this.config.DESCRIPTION.Split(',').ToList().First();
                    radFalse.Text = this.config.DESCRIPTION.Split(',').ToList().Last();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public string GetValue()
        {
            string value = "";
            try
            {
                string TrueOrFalseId ="";
                if (radTrue.Checked) TrueOrFalseId = "true";
                else if (radFalse.Checked) TrueOrFalseId = "false";
                else TrueOrFalseId = "null";
                value = String.Format(this.JsonOutput, TrueOrFalseId);
            }
            catch (Exception ex)
            {
                value = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }

        public void SetValue()
        {
            try
            {
                if (this.JsonOutput != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.JsonOutput, this.report.JSON_FILTER);
                    if (value == "true") radTrue.Checked = true;
                    else if (value == "false") radFalse.Checked = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Validation
        private void SetValidateRad()
        {
            try
            {
                TrueOrFalseValidationRule validRule = new TrueOrFalseValidationRule();
                validRule.radTrue = radTrue;
                validRule.radFalse = radFalse;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(radTrue, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValidation()
        {
            try
            {
                SetValidateRad();
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    this.positionHandleControl = -1;
                    result = dxValidationProvider1.Validate();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        
        }
    }
}
