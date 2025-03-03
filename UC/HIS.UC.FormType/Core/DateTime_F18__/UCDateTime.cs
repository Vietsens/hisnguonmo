﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.DateTime
{
    public partial class UCDateTime : DevExpress.XtraEditors.XtraUserControl
    {
        DateTimeFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCDateTime(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 25;

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
                    lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                SetDefaultTime();
                SetTitle();//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultTime()
        {
            try
            {
                if (this.generateRDO != null && (this.generateRDO.TimeDefault.HasValue))
                {
                    if (this.generateRDO.TimeDefault.HasValue)
                        dtTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.generateRDO.TimeDefault.Value);
                }
                else dtTime.EditValue = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetTitle()
        {
            try
            {
                //if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                //{
                //    lciTitleName.Text = this.config.DESCRIPTION;
                //    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}
                //else
                //{
                //    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //}
                if (this.config != null)
                {
                    lciTitleName.Text = this.config.DESCRIPTION ?? " ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string GetValue()
        {
            string result = "";
            long? value = 0;
            try
            {
                if (dtTime.DateTime != System.DateTime.MinValue)
                    value = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime);
                result = String.Format(this.config.JSON_OUTPUT, value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        public void SetValue()
        {
            try
            {
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {

                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,this.config.JSON_OUTPUT, this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        dtTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(value));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.generateRDO != null && this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
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

        #region Validation
        private void SetValidateDateTime()
        {
            try
            {
                HIS.UC.FormType.DateTime.Validation.TimeValidationRule validRule = new HIS.UC.FormType.DateTime.Validation.TimeValidationRule();
                validRule.dtTime = dtTime;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtTime, validRule);
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
                SetValidateDateTime();
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

        private void UCDateTime_Load(object sender, EventArgs e)
        {
            try
            {
                layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_DATE_TIME_LAYOUT_CONTROL_GROUP1", Resources.ResourceLanguageManager.LanguageUCDateTime, Base.LanguageManager.GetCulture());
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
    }
}
