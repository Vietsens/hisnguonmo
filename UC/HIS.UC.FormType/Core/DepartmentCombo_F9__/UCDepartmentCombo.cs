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
using Inventec.Common.Logging;
using HIS.UC.FormType.Loader;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using His.UC.LibraryMessage;
using HIS.UC.FormType.HisMultiGetString;
using DevExpress.XtraEditors.Controls;

namespace HIS.UC.FormType.DepartmentCombo
{
    public partial class UCDepartmentCombo : DevExpress.XtraEditors.XtraUserControl
    {
        DepartmentComboFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        string FDO = null;
        string[] limitCodes = null;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        string Output0 = "";
        string JsonOutput = "";
        public UCDepartmentCombo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
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
                LogSystem.Warn(ex);
            }
        }

        private void LoadDefault(List<DataGet> listData)
        {
            try
            {
                if (listData.Count == 1 && this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    txtDepartmentCode.Text = listData.First().CODE;
                    cboDepartment.EditValue = listData.First().ID;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadBranch(List<DataGet> listData)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.config.JSON_OUTPUT) && (this.config.JSON_OUTPUT.Contains("BRANCH_ID") || this.config.JSON_OUTPUT.Contains("BRANCH__ID")) && this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    var currentBranch = listData.FirstOrDefault(o => o.ID == FormTypeConfig.BranchId);
                    if (currentBranch != null)
                    {
                        txtDepartmentCode.Text = currentBranch.CODE;
                        cboDepartment.EditValue = currentBranch.ID;
                    }
                    else
                    {
                        txtDepartmentCode.Text = listData.First().CODE;
                        cboDepartment.EditValue = listData.First().ID;
                    }
                }
                else
                {
                    cboDepartment.Focus();
                    //cboDepartment.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void Init()
        {
            try
            {
                FormTypeConfig.ReportTypeCode = config.REPORT_TYPE_CODE;

                FilterConfig.GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                JsonOutput = this.config.JSON_OUTPUT;
                FilterConfig.RemoveStrOutput0(ref JsonOutput);
                //this.config.JSON_OUTPUT = JsonOutput;

                FDO = FilterConfig.HisFilterTypes(this.config.JSON_OUTPUT);
                limitCodes = FilterConfig.GetLimitCodes(this.config.JSON_OUTPUT);
                cboDepartment.Properties.DataSource = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0);
                cboDepartment.Properties.DisplayMember = "NAME";
                var dataCombo = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0);
                var defaultValue = new DataGet();
                if (!string.IsNullOrWhiteSpace(Output0) && dataCombo != null)
                {
                    defaultValue = dataCombo.FirstOrDefault(o => o.CODE == Output0);
                }
                string[] Filters = null;
                FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                {
                    cboDepartment.Properties.ValueMember = "ID";
                    if (defaultValue != null && defaultValue.ID > 0)
                    {
                        cboDepartment.EditValue = defaultValue.ID;
                        txtDepartmentCode.Text = defaultValue.CODE;
                    }
                }
                else
                {
                    cboDepartment.Properties.ValueMember = "CODE";
                    if (defaultValue != null)
                    {
                        cboDepartment.EditValue = defaultValue.CODE;
                        txtDepartmentCode.Text = defaultValue.CODE;
                    }
                }

                cboDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboDepartment.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboDepartment.Properties.ImmediatePopup = true;
                cboDepartment.ForceInitialize();
                cboDepartment.Properties.View.Columns.Clear();
                cboDepartment.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboDepartment.Properties.View.Columns.AddField("CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = false;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboDepartment.Properties.View.Columns.AddField("NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = false;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;


                LoadDefault(HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0));
                LoadBranch(HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0));

                aColumnCode.Visible = true;
                aColumnName.Visible = true;
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    Validation();
                    lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

       

        void SetTitle()
        {
            try
            {
                //if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                //{
                //    lciTitleName.Text = this.config.DESCRIPTION;
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}
                //else
                //{
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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

        private void txtDepartmentCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtDepartmentCode.Text.Trim()))
                    {
                        string code = txtDepartmentCode.Text.Trim().ToLower();
                        var listData = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).Where(o => o.CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtDepartmentCode.Text = result.First().CODE;

                            string[] Filters = null;
                            FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                            if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                            {
                                cboDepartment.EditValue = result.First().ID;
                            }
                            else
                                cboDepartment.EditValue = result.First().CODE;
                        }
                    }
                    if (showCbo)
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var department = new DataGet();
                        string[] Filters = null;
                        FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                        if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                        {
                            department = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).FirstOrDefault(f => f.ID == long.Parse(cboDepartment.EditValue.ToString()));
                        }
                        else
                        {
                            department = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).FirstOrDefault(f => f.CODE == cboDepartment.EditValue.ToString());
                        }
                        if (department != null)
                        {
                            txtDepartmentCode.Text = department.CODE;
                        }
                        if (this.config != null && this.config.IS_REQUIRE != IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            cboDepartment.Properties.Buttons[1].Visible = true;
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var department = HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).FirstOrDefault(f => f.ID == long.Parse(cboDepartment.EditValue.ToString()));
                        if (department != null)
                        {
                            txtDepartmentCode.Text = department.CODE;
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
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
                string[] Filters = null;
                FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0])))
                {
                    long? departmentId = (long?)cboDepartment.EditValue;
                    value = String.Format(this.JsonOutput, ConvertUtils.ConvertToObjectFilter(departmentId));
                }
                else
                {
                    string departmentId = "\"" + (string)cboDepartment.EditValue + "\"";
                    if (departmentId == "\"\"") departmentId = "null";
                    value = String.Format(this.JsonOutput, ConvertUtils.ConvertToObjectFilter(departmentId));
                }
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
                if (this.JsonOutput != null && this.report.JSON_FILTER != null)
                {

                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.JsonOutput, this.report.JSON_FILTER);

                    string[] Filters = null;
                    FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref Filters);
                    
                    if (Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[0]) && value != null && value != "null")
                    {
                        txtDepartmentCode.Text = value.Replace("\"", "");
                        cboDepartment.EditValue = (HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).FirstOrDefault(f => f.CODE == txtDepartmentCode.Text) ?? new DataGet()).CODE;
                    }
                    else if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                    {
                        //cboDepartment.Properties.DataSource = HisMultiGetByString.GetByStringLimit(FDO, limitCodes);
                        txtDepartmentCode.Text = (HisMultiGetByString.GetByStringLimit(FDO, limitCodes, ref Output0).FirstOrDefault(f => f.ID == Inventec.Common.TypeConvert.Parse.ToInt64(value)) ?? new DataGet()).CODE;
                        cboDepartment.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
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

        #region Validation
        private void ValidateDepartment()
        {
            try
            {
                HIS.UC.FormType.DepartmentCombo.Validation.DepartmentValidationRule validRule = new HIS.UC.FormType.DepartmentCombo.Validation.DepartmentValidationRule();
                validRule.cboDepartment = cboDepartment;
                validRule.txtDepartmentCode = txtDepartmentCode;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDepartmentCode, validRule);
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
                ValidateDepartment();
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

        private void UCDepartmentCombo_Load(object sender, EventArgs e)
        {
            try
            {
                lciTitleName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_DEPARTMENT_COMBO_LCI_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCDepartmentCombo, Base.LanguageManager.GetCulture());

                SetTitle();
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

        //private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataGet data = null;
        //        if (cboDepartment.EditValue != null && cboDepartment.EditValue.GetType() == typeof(string))
        //        {
        //            data = HisMultiGetByString.GetByStringLimit(FDO, limitCodes).FirstOrDefault(o => o.CODE == cboDepartment.EditValue.ToString());
        //        }
        //        else
        //        {
        //            data = HisMultiGetByString.GetByStringLimit(FDO, limitCodes).FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? 0).ToString()));
        //        }
        //        if (data != null)
        //        {
        //            txtDepartmentCode.Text = data.CODE;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                    cboDepartment.Properties.Buttons[1].Visible = false;
                    txtDepartmentCode.Text = null;
                    txtDepartmentCode.Focus();
                    txtDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
