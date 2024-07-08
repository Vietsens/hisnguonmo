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
using HIS.UC.FormType.Core.ComboBoxAtComboBox_F4__.Validation;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.UC.FormType.Core.ComboBoxAtComboBox_F4__
{
    public partial class UCComboBoxAtComboBox : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        string[] limitCodes = null;
        string[] Filters = null;

        private Dictionary<int, List<DataGet>> dicDataAll = new Dictionary<int, List<DataGet>>();

        public UCComboBoxAtComboBox(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
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
        void Init()
        {
            try
            {
                FormTypeConfig.ReportTypeCode = this.config.REPORT_TYPE_CODE;
                string jsonOutput = this.config.JSON_OUTPUT;
                string Output0 = "";
                FilterConfig.GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                FilterConfig.RemoveStrOutput0(ref jsonOutput);
                this.config.JSON_OUTPUT = jsonOutput;

                this.limitCodes = FilterConfig.GetLimitCodes(this.config.JSON_OUTPUT);
                FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref this.Filters);
                string[] jsonDescriptions = (this.config.DESCRIPTION ?? "").Split(',');

                if (this.Filters != null && this.Filters.Length > 0)
                {
                    var FDO = FilterConfig.HisFilterTypes("\"" + this.Filters[0]);
                    dicDataAll[0] = HisMultiGetByString.GetByStringLimit(FDO, this.limitCodes, ref Output0);
                    string iDescription = "";
                    if (jsonDescriptions != null && jsonDescriptions.Length > 0)
                    {
                        iDescription = jsonDescriptions[0];
                    }
                    this.InitCombo(this.cboServiceType, this.layoutControlItem2, iDescription, 0);
                }
                else
                {
                    throw new Exception("this.Filters.Length < 1");
                }

                if (this.Filters != null && this.Filters.Length > 1)
                {
                    var FDO = FilterConfig.HisFilterTypes("\"" + this.Filters[1]);
                    dicDataAll[1] = HisMultiGetByString.GetByStringLimit(FDO, null, ref Output0);
                    string iDescription = "";
                    if (jsonDescriptions != null && jsonDescriptions.Length > 1)
                    {
                        iDescription = jsonDescriptions[1];
                    }
                    this.InitCombo(this.cboService, this.layoutControlItem1, iDescription, 1);
                }
                else
                {
                    throw new Exception("this.Filters.Length < 2");
                }


                LoadBranch(dicDataAll[0]);

                CboSetDefaultValue(this.cboServiceType,this.dicDataAll[0],Output0,0);

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

        private void InitCombo(GridLookUpEdit gridLookUpEdit, DevExpress.XtraLayout.LayoutControlItem layoutControlItem, string description, int i)
        {
            try
            {

                InitComboParent(gridLookUpEdit, description, dicDataAll[i]);
                layoutControlItem.Text = description;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboParent(DevExpress.XtraEditors.GridLookUpEdit cbo, string description, List<DataGet> data)
        {


            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("CODE", "Mã " + description, 100, 1));
            columnInfos.Add(new ColumnInfo("NAME", "Tên " + description, 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cbo, data, controlEditorADO);
        }

        private void CboSetDefaultValue(GridLookUpEdit cbo, List<DataGet> data, string Output0, int indexFilter)
        {
            try
            {
                var defaultValue = new DataGet();
                if (!string.IsNullOrWhiteSpace(Output0) && data != null)
                {
                    defaultValue = data.FirstOrDefault(o => o.CODE == Output0);
                }

                if (!(Filters != null && Filters.Length > 0 && FilterConfig.IsCodeField(Filters[indexFilter])))
                {
                    cbo.Properties.ValueMember = "ID";
                    if (defaultValue != null && defaultValue.ID > 0)
                    {
                        cbo.EditValue = defaultValue.ID;
                    }
                }
                else
                {
                    cbo.Properties.ValueMember = "CODE";
                    if (defaultValue != null)
                    {
                        cbo.EditValue = defaultValue.CODE;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBranch(List<DataGet> datas)
        {
            try
            {
                if (this.config != null && this.config.JSON_OUTPUT!=null&& this.config.JSON_OUTPUT.Contains( "\"BRANCH_ID\":{0}") && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    DataGet branch = datas.FirstOrDefault(o => o.ID == FormTypeConfig.BranchId);
                    if (branch != null)
                    {
                        cboServiceType.EditValue = branch.ID;
                    }
                   
                }
                
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                    layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateService();
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
                HIS.UC.FormType.Core.ComboBoxAtComboBox_F4__.Validation.ComboBoxAtComboBoxServiceTypeValidationRule validRule = new HIS.UC.FormType.Core.ComboBoxAtComboBox_F4__.Validation.ComboBoxAtComboBoxServiceTypeValidationRule();
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
                ComboBoxAtComboBoxServiceValidationRule validRule = new ComboBoxAtComboBoxServiceValidationRule();
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
                        cboService.Properties.DataSource = dicDataAll[1].Where(o => (long)cboServiceType.EditValue == o.PARENT).ToList();
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
                        cboServiceType.Properties.DataSource = this.dicDataAll[0];
                        cboServiceType.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                    }
                    if (jsOutputSub.Count() > 1)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, jsOutputSub[1], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        {
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

        private void cboServiceType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceType.EditValue != null)
                {
                    cboService.Properties.DataSource = dicDataAll[1].Where(o => (long)cboServiceType.EditValue == o.PARENT).ToList();
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
                else
                {
                    cboService.Properties.DataSource = dicDataAll[1].ToList();
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
