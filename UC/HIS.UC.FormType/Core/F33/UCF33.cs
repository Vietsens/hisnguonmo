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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using HIS.UC.FormType.Loader;
using HIS.UC.FormType.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using MOS.EFMODEL.DataModels;
using His.UC.LibraryMessage;
using HIS.UC.FormType.HisMultiGetString;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Logging;
using HIS.UC.FormType.Core.F33.Validation;

namespace HIS.UC.FormType.Core.F33
{
    public partial class UCF33 : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        string FDO1 = null;
        string FDO2 = null;

        public UCF33(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 60;

                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Init()
        {
            try
            {
                //Chọn combo cha
                FormTypeConfig.ReportTypeCode = this.config.REPORT_TYPE_CODE;
                string JSON_OUTPUT_PARENT = this.config.JSON_OUTPUT.Split(',').ToList().First();
                FDO1 = FilterConfig.HisFilterTypes(JSON_OUTPUT_PARENT);
                cboServiceType.Properties.DataSource = HisMultiGetByString.GetByString(FDO1, null);
                cboServiceType.Properties.DisplayMember = "NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();
                cboServiceType.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;

                //Chọn combo con
                string JSON_OUTPUT_CHILD = this.config.JSON_OUTPUT.Split(',').ToList().Last();
                FDO2 = FilterConfig.HisFilterTypes(JSON_OUTPUT_CHILD);
                //cboService.Properties.DataSource = HisMultiGetByString.GetByString(FDO2,null);
                cboService.Properties.DisplayMember = "NAME";
                cboService.Properties.ValueMember = "ID";

                cboService.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboService.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboService.Properties.ImmediatePopup = true;
                cboService.ForceInitialize();
                cboService.Properties.View.Columns.Clear();
                cboService.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode1 = cboService.Properties.View.Columns.AddField("CODE");
                aColumnCode1.Caption = "Mã";
                aColumnCode1.Visible = true;
                aColumnCode1.VisibleIndex = 1;
                aColumnCode1.Width = 50;

                GridColumn aColumnName1 = cboService.Properties.View.Columns.AddField("NAME");
                aColumnName1.Caption = "Tên";
                aColumnName1.Visible = true;
                aColumnName1.VisibleIndex = 2;
                aColumnName1.Width = 100;

                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                if (this.report != null)
                {
                    SetValue();
                }

                Validation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    layoutControlItem1.Text = this.config.DESCRIPTION.Split(',').ToList().Last();
                    layoutControlItem2.Text = this.config.DESCRIPTION.Split(',').ToList().First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Validation()
        {
            try
            {
                if (this.isValidData)
                {
                    layoutControlItem2.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateServiceType();
                    //layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //ValidateService();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateServiceType()
        {
            try
            {
                HIS.UC.FormType.Core.F33.Validation.F33GrandParentValidationRule validRule = new HIS.UC.FormType.Core.F33.Validation.F33GrandParentValidationRule();
                validRule.cboServiceType = cboServiceType;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboServiceType, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateService()
        {
            try
            {
                F33ChildValidationRule validRule = new F33ChildValidationRule();
                validRule.cboService = cboService;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboService, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        cboService.Properties.DataSource = HisMultiGetByString.GetByString(FDO2, null).Where(o => (long)cboServiceType.EditValue == o.GRAND_PARENT).ToList();
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboServiceType.Focus();
                        cboServiceType.ShowPopup();
                    }
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
            try
            {
                long? ServiceId = (long?)cboService.EditValue;
                long? ServiceTypeId = (long?)cboServiceType.EditValue;
                value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(ServiceTypeId), ConvertUtils.ConvertToObjectFilter(ServiceId));
            }
            catch (Exception ex)
            {
                value = null;
                LogSystem.Warn(ex);
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
                    {
                        cboServiceType.Properties.DataSource = HisMultiGetByString.GetByString(FDO1, null);
                        cboServiceType.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                    }
                    if (jsOutputSub.Count() > 1)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, jsOutputSub[1], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        {
                            cboService.Properties.DataSource = HisMultiGetByString.GetByString(FDO2, null);
                            cboService.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                        }
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

        private void InitGridCheckMarksSelection()
        {
            try
            {
                //GridCheckMarksSelection gridCheckMarksSA = new GridCheckMarksSelection(cboService.Properties);
                //gridCheckMarksSA.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(gridCheckMarks_SelectionChanged);
                //cboService.Properties.Tag = gridCheckMarksSA;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridCheckMarks_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                //if (ActiveControl is GridLookUpEdit)
                //{
                //    StringBuilder sb = new StringBuilder();
                //    foreach (MOS.EFMODEL.DataModels.V_HIS_SERVICE rv in (sender as GridCheckMarksSelection).Selection)
                //    {
                //        if (sb.ToString().Length > 0) { sb.Append(", "); }
                //        sb.Append(rv.SERVICE_NAME.ToString());
                //    }
                //    (ActiveControl as GridLookUpEdit).Text = sb.ToString();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                //StringBuilder sb = new StringBuilder();
                //GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                //if (gridCheckMark == null) return;
                //foreach (MOS.EFMODEL.DataModels.V_HIS_SERVICE rv in gridCheckMark.Selection)
                //{
                //    if (sb.ToString().Length > 0) { sb.Append(", "); }
                //    if (true)
                //    {

                //    }
                //    sb.Append(rv.SERVICE_NAME.ToString());
                //}
                //e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed_1(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void cboService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboService.EditValue != null)
                    {
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboService.Focus();
                        cboService.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
