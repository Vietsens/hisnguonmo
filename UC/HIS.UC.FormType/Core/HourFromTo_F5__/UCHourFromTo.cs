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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.HourFromTo
{
    public partial class UCHourFromTo : DevExpress.XtraEditors.XtraUserControl
    {
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        bool isValidData = false;
        HourFromToFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCHourFromTo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            InitializeComponent();
            //FormTypeConfig.ReportHight += 25;
            try
            {
                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Init()
        {
            try
            {
                SetDefaultTime();//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                if (this.report != null)
                {
                    SetValue();
                }
                if (this.isValidData)
                {
                    //lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutHourFrom.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutTimeTo.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //lblTitleName.ForeColor = Color.Maroon;
                    Validation();
                }
                SetTitle();
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
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    //lblTitleName.Text = this.config.DESCRIPTION;
                    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
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
                if (this.generateRDO != null && (this.generateRDO.FromHourDefault.HasValue || this.generateRDO.ToHourDefault.HasValue))
                {
                    if (this.generateRDO.FromHourDefault.HasValue)
                        dtHourFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.generateRDO.FromHourDefault.Value);
                    if (this.generateRDO.ToHourDefault.HasValue)
                        dtHourTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.generateRDO.ToHourDefault.Value);
                }
                else
                {
                    dtHourFrom.EditValue = new System.DateTime(1, 1, 1, 0, 0, 0);
                    dtHourTo.EditValue = new System.DateTime(1, 1, 1, 23, 59, 59);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValue()
        {
            string value = "";
            long? FROM_TIME = 0;
            long? TO_TIME = 0;
            try
            {
                if (dtHourFrom.Time != System.DateTime.MinValue)
                    FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHourFrom.Time);

                if (dtHourTo.Time != System.DateTime.MinValue)
                    TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHourTo.Time);

                value = String.Format(this.config.JSON_OUTPUT, FROM_TIME % 1000000, TO_TIME % 1000000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }

        public void SetValue()
        {
            try
            {
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    var jsOutputSub = this.config.JSON_OUTPUT.Split(new string[] { "," }, StringSplitOptions.None);

                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[0], this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        dtHourFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(value));

                    if (jsOutputSub.Count() > 1)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[1], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                            dtHourTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(value));
                    }
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
                if (this.isValidData)
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
        private void ValidateFromHour()
        {
            try
            {
                HIS.UC.FormType.HourFromTo.Validation.FromHourValidationRule validRule = new HIS.UC.FormType.HourFromTo.Validation.FromHourValidationRule();
                validRule.dtFromHour = dtHourFrom;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtHourFrom, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateToHour()
        {
            try
            {
                HIS.UC.FormType.HourFromTo.Validation.ToHourValidationRule validRule = new HIS.UC.FormType.HourFromTo.Validation.ToHourValidationRule();
                validRule.dtFromHour = dtHourFrom;
                validRule.dtToHour = dtHourTo;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtHourTo, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Validation()
        {
            try
            {
                ValidateFromHour();
                ValidateToHour();
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

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void UCHourFromTo_Load(object sender, EventArgs e)
        {
            try
            {
                //lblTitleName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_TIME_FROM_TO_LBL_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCHourFromTo, Base.LanguageManager.GetCulture());
                //layoutHourFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_TIME_FROM_TO_LAYOUT_TIME_FROM", Resources.ResourceLanguageManager.LanguageUCHourFromTo, Base.LanguageManager.GetCulture());
                //layoutTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_TIME_FROM_TO_LAYOUT_TIME_TO", Resources.ResourceLanguageManager.LanguageUCHourFromTo, Base.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
