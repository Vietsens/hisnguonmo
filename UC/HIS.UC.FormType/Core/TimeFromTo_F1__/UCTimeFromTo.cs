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

namespace HIS.UC.FormType.TimeFromTo
{
    public partial class UCTimeFromTo : DevExpress.XtraEditors.XtraUserControl
    {
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        bool isValidData = false;
        TimeFromToFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCTimeFromTo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
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
                SetDefaultTime();
                FormatView();
                if (this.isValidData)
                {
                    //lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutTimeFrom.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutTimeTo.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //lblTitleName.ForeColor = Color.Maroon;
                    Validation();
                }
                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormatView()
        {
            try
            {
                string js = "";

                if (this.config.JSON_OUTPUT.Contains("ONLY_DATE_FROM") && this.config.JSON_OUTPUT.Contains("ONLY_DATE_TO"))
                {
                    this.dtTimeFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.MonthView;
                    this.dtTimeTo.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.MonthView;
                    js = "dd/MM/yyyy";
                }
                else if (this.config.JSON_OUTPUT.Contains("ONLY_MONTH_FROM") && this.config.JSON_OUTPUT.Contains("ONLY_MONTH_TO"))
                {
                    this.dtTimeFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    this.dtTimeTo.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    js = "MM/yyyy";
                }
                else if (this.config.JSON_OUTPUT.Contains("ONLY_YEAR_FROM") && this.config.JSON_OUTPUT.Contains("ONLY_YEAR_TO"))
                {
                    this.dtTimeFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView;
                    this.dtTimeTo.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView;
                    js = "yyyy";
                }
                else
                {
                    this.dtTimeFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.All;
                    this.dtTimeTo.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.All;
                    js = "dd/MM/yyyy HH:mm:ss";
                }

                this.dtTimeFrom.Properties.DisplayFormat.FormatString = js;
                this.dtTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.dtTimeFrom.Properties.EditFormat.FormatString = js;
                this.dtTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.dtTimeFrom.Properties.Mask.EditMask = js;

                this.dtTimeTo.Properties.DisplayFormat.FormatString = js;
                this.dtTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.dtTimeTo.Properties.EditFormat.FormatString = js;
                this.dtTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.dtTimeTo.Properties.Mask.EditMask = js;
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
                //if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                //{
                //    this.layoutTimeFrom.Text = this.config.DESCRIPTION + " Từ:";
                //    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}
                //else
                //{
                //    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //}
                if (this.config != null)
                {
                    layoutTimeFrom.Text = (this.config.DESCRIPTION ?? " ") + " Từ:";
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
                if (this.generateRDO != null && (this.generateRDO.FromTimeDefault.HasValue || this.generateRDO.ToTimeDefault.HasValue))
                {
                    if (this.generateRDO.FromTimeDefault.HasValue)
                        dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.generateRDO.FromTimeDefault.Value);
                    if (this.generateRDO.ToTimeDefault.HasValue)
                        dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.generateRDO.ToTimeDefault.Value);
                }
                else if (this.config != null && this.config.JSON_OUTPUT.Contains("DATE_OF_WEEK"))
                {
                    System.DateTime baseDate = System.DateTime.Today;

                    //var today = baseDate;
                    //var yesterday = baseDate.AddDays(-1);
                    var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                    var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                    //var lastWeekStart = thisWeekStart.AddDays(-7);
                    //var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                    //var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                    //var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                    //var lastMonthStart = thisMonthStart.AddMonths(-1);
                    //var lastMonthEnd = thisMonthStart.AddSeconds(-1);
                    dtTimeFrom.EditValue = thisWeekStart;
                    dtTimeTo.EditValue = thisWeekEnd;
                }
                else if (this.config != null && this.config.JSON_OUTPUT.Contains("NOW"))
                {
                    dtTimeFrom.EditValue = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 0, 0, 0);
                    dtTimeTo.EditValue = System.DateTime.Now;
                }
                else if (this.config != null && this.config.FORM_FIELD_CODE == "FTHIS000001")
                {
                    dtTimeFrom.EditValue = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
                    dtTimeTo.EditValue = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 23, 59, 59);
                }
                else
                {
                    dtTimeFrom.EditValue = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 0, 0, 0);
                    dtTimeTo.EditValue = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 23, 59, 59);
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
            long? FROM_TIME = null;
            long? TO_TIME = null;
            try
            {
                if (!String.IsNullOrWhiteSpace(dtTimeFrom.Text) && dtTimeFrom.DateTime != System.DateTime.MinValue)
                    FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeFrom.DateTime);

                if (!String.IsNullOrWhiteSpace(dtTimeTo.Text) && dtTimeTo.DateTime != System.DateTime.MinValue)
                    TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeTo.DateTime);

                value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(FROM_TIME), ConvertUtils.ConvertToObjectFilter(TO_TIME));
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

                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, jsOutputSub[0], this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(value));

                    if (jsOutputSub.Count() > 1)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, jsOutputSub[1], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                            dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(value));
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
        private void ValidateFromTime()
        {
            try
            {
                HIS.UC.FormType.TimeFromTo.Validation.FromTimeValidationRule validRule = new HIS.UC.FormType.TimeFromTo.Validation.FromTimeValidationRule();
                validRule.dtFromTime = dtTimeFrom;
                validRule.dtToTime = dtTimeTo;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtTimeFrom, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateToTime()
        {
            try
            {
                HIS.UC.FormType.TimeFromTo.Validation.ToTimeValidationRule validRule = new HIS.UC.FormType.TimeFromTo.Validation.ToTimeValidationRule();
                validRule.dtFromTime = dtTimeFrom;
                validRule.dtToTime = dtTimeTo;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtTimeTo, validRule);
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
                ValidateFromTime();
                ValidateToTime();
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

        private void UCTimeFromTo_Load(object sender, EventArgs e)
        {
            try
            {
                //lblTitleName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_TIME_FROM_TO_LBL_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCTimeFromTo, Base.LanguageManager.GetCulture());
                //layoutTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_TIME_FROM_TO_LAYOUT_TIME_FROM", Resources.ResourceLanguageManager.LanguageUCTimeFromTo, Base.LanguageManager.GetCulture());
                //layoutTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_TIME_FROM_TO_LAYOUT_TIME_TO", Resources.ResourceLanguageManager.LanguageUCTimeFromTo, Base.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
