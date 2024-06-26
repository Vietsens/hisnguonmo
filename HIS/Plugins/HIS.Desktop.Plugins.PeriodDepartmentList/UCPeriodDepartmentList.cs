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
using System.Windows.Forms;
using HTC.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using Inventec.Core;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HTC.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.PeriodDepartmentList.Validation;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.PeriodDepartmentList
{
    public partial class UCPeriodDepartmentList : HIS.Desktop.Utility.UserControlBase
    {
        private int positionHandleControl = -1;
        int rowCount = 0;
        int dataTotal = 0;

        V_HTC_PERIOD_DEPARTMENT periodDepartment = null;
        List<V_HTC_PERIOD_DEPARTMENT> ListPeriodDepartment = new List<V_HTC_PERIOD_DEPARTMENT>();

        public UCPeriodDepartmentList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCPeriodDepartmentList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyUcLanguage();
                LoadDataToComboFilterPeriod();
                LoadDataToComboPeriod();
                LoadDataToComboDepartment();
                SetDefaultFilterControl();
                ResetValueControlCreate();
                SetEnableButtonCreate(true);
                ValidControl();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultFilterControl()
        {
            try
            {
                txtKeyword.Text = "";
                dtCreateTimeFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtCreateTimeTo.DateTime = DateTime.Now;
                cboFilterPeriod.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlCreate()
        {
            try
            {
                periodDepartment = null;
                txtPeriodCode.Text = "";
                cboPeriod.EditValue = null;
                txtDepartmentCode.Text = "";
                cboDepartment.EditValue = null;
                txtEndTreatmentAmount.Text = "";
                txtFromExamClinicalAmount.Text = "";
                txtClinicalAmount.Text = "";
                txtClinicalDayAmount.Text = "";
                txtLaborAmount.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButtonCreate(bool isEnable)
        {
            try
            {
                btnAdd.Enabled = isEnable;
                btnEdit.Enabled = !isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboFilterPeriod()
        {
            try
            {
                cboFilterPeriod.Properties.DataSource = BackendDataWorker.Get<HTC_PERIOD>().OrderByDescending(o => o.CREATE_TIME).ToList();
                cboFilterPeriod.Properties.DisplayMember = "PERIOD_NAME";
                cboFilterPeriod.Properties.ValueMember = "ID";
                cboFilterPeriod.Properties.ForceInitialize();
                cboFilterPeriod.Properties.Columns.Clear();
                cboFilterPeriod.Properties.Columns.Add(new LookUpColumnInfo("PERIOD_CODE", "Mã", 50));
                cboFilterPeriod.Properties.Columns.Add(new LookUpColumnInfo("PERIOD_NAME", "Tên", 140));
                cboFilterPeriod.Properties.ShowHeader = true;
                cboFilterPeriod.Properties.ImmediatePopup = true;
                cboFilterPeriod.Properties.DropDownRows = 10;
                cboFilterPeriod.Properties.PopupWidth = 190;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboPeriod()
        {
            try
            {
                cboPeriod.Properties.DataSource = BackendDataWorker.Get<HTC_PERIOD>().OrderByDescending(o => o.CREATE_TIME).ToList();
                cboPeriod.Properties.DisplayMember = "PERIOD_NAME";
                cboPeriod.Properties.ValueMember = "ID";
                cboPeriod.Properties.ForceInitialize();
                cboPeriod.Properties.Columns.Clear();
                cboPeriod.Properties.Columns.Add(new LookUpColumnInfo("PERIOD_CODE", "Mã", 50));
                cboPeriod.Properties.Columns.Add(new LookUpColumnInfo("PERIOD_NAME", "Tên", 140));
                cboPeriod.Properties.ShowHeader = true;
                cboPeriod.Properties.ImmediatePopup = true;
                cboPeriod.Properties.DropDownRows = 10;
                cboPeriod.Properties.PopupWidth = 190;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboDepartment()
        {
            try
            {
                cboDepartment.Properties.DataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().OrderByDescending(o => o.DEPARTMENT_CODE).ToList();
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                cboDepartment.Properties.ForceInitialize();
                cboDepartment.Properties.Columns.Clear();
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "Mã", 50));
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "Tên", 200));
                cboDepartment.Properties.ShowHeader = true;
                cboDepartment.Properties.ImmediatePopup = true;
                cboDepartment.Properties.DropDownRows = 10;
                cboDepartment.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                FillDataToGridExpense(new CommonParam(0, (int)100));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridExpense, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExpense(object param)
        {
            try
            {
                ListPeriodDepartment = new List<V_HTC_PERIOD_DEPARTMENT>();
                gridControlPeriodDepartmentList.DataSource = null;
                ResetValueControlCreate();
                SetEnableButtonCreate(true);
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HtcPeriodDepartmentViewFilter filter = new HtcPeriodDepartmentViewFilter();

                if (!String.IsNullOrEmpty(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }
                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (cboFilterPeriod.EditValue != null)
                {
                    filter.PERIOD_ID = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboFilterPeriod.EditValue)).ID;
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HTC_PERIOD_DEPARTMENT>>(HtcRequestUriStore.HTC_PERIOD_DEPARTMENT__GETVIEW, ApiConsumers.HtcConsumer, filter, paramCommon);
                if (result != null)
                {
                    ListPeriodDepartment = (List<V_HTC_PERIOD_DEPARTMENT>)result.Data;
                    rowCount = (ListPeriodDepartment == null ? 0 : ListPeriodDepartment.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlPeriodDepartmentList.BeginUpdate();
                    gridControlPeriodDepartmentList.DataSource = ListPeriodDepartment;
                    gridControlPeriodDepartmentList.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPeriodDepartmentList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HTC_PERIOD_DEPARTMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPeriodDepartmentList_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    periodDepartment = (V_HTC_PERIOD_DEPARTMENT)gridViewPeriodDepartmentList.GetFocusedRow();
                    if (periodDepartment != null)
                    {
                        SetEnableButtonCreate(false);
                        SetValueControlUpdate();
                    }
                    else
                    {
                        ResetValueControlCreate();
                        SetEnableButtonCreate(true);
                    }
                }
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
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlPeriod();
                ValidControlDepartment();
                ValidControlEndTreatment();
                ValidControlFromExamClinical();
                ValidControlClinical();
                ValidControlClinicalDay();
                ValidControlLabor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPeriod()
        {
            try
            {
                PeriodValidationRule periodRule = new PeriodValidationRule();
                periodRule.txtPeriodCode = txtPeriodCode;
                periodRule.cboPeriod = cboPeriod;
                dxValidationProvider1.SetValidationRule(txtPeriodCode, periodRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDepartment()
        {
            try
            {
                DepartmentValidationRule departRule = new DepartmentValidationRule();
                departRule.txtDepartmentCode = txtDepartmentCode;
                departRule.cboDepartment = cboDepartment;
                dxValidationProvider1.SetValidationRule(txtDepartmentCode, departRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlEndTreatment()
        {
            try
            {
                EndTreatmentAmountValidationRule endTreatRule = new EndTreatmentAmountValidationRule();
                endTreatRule.txtEndTreatmentAmount = txtEndTreatmentAmount;
                dxValidationProvider1.SetValidationRule(txtEndTreatmentAmount, endTreatRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlFromExamClinical()
        {
            try
            {
                FromExamClinicalAmountValidationRule fromExamRule = new FromExamClinicalAmountValidationRule();
                fromExamRule.txtFromExamClinicalAmount = txtFromExamClinicalAmount;
                dxValidationProvider1.SetValidationRule(txtFromExamClinicalAmount, fromExamRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlClinical()
        {
            try
            {
                ClinicalAmountValidationRule clinicalRule = new ClinicalAmountValidationRule();
                clinicalRule.txtClinicalAmount = txtClinicalAmount;
                dxValidationProvider1.SetValidationRule(txtClinicalAmount, clinicalRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlClinicalDay()
        {
            try
            {
                ClinicalDayAmountValidationRule clinicalDayRule = new ClinicalDayAmountValidationRule();
                clinicalDayRule.txtClinicalDayAmount = txtClinicalDayAmount;
                dxValidationProvider1.SetValidationRule(txtClinicalDayAmount, clinicalDayRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlLabor()
        {
            try
            {
                LaborAmountValidationRule laborRule = new LaborAmountValidationRule();
                laborRule.txtLaborAmount = txtLaborAmount;
                dxValidationProvider1.SetValidationRule(txtLaborAmount, laborRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnFind()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnRefresh()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnAdd()
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnEdit()
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnNew()
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyUcLanguage()
        {
            try
            {
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__BTN_FIND", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__BTN_ADD", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__BTN_EDIT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__BTN_NEW", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__BTN_REFRESH", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutClinicalAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_CLINICAL_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutClinicalDayAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_CLINICAL_DAY_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_TIME_FROM", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_TIME_TO", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDepartment.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_DEPARTMENT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutEndTreatmentAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_END_TREATMENT_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutFromExamClinicalAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_FROM_EXAM_CLINICAL_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutLaborAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_LABOR_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPeriod.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__LAYOUT_PERIOD", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //navbargroup
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__NAV_BAR_CONTROL__GROUP_CREATE_TIME", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.navBarGroupPeriod.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__NAV_BAR_CONTROL__GROUP_PERIOD", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //gridControl column
                this.gridColumn_Department_ClinicalAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_CLINICAL_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_ClinicalDayAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_CLINICAL_DAY_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_CREATE_TIME", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_Creator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_CREATOR", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_DepartmentCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_DEPARTMENT_CODE", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_DepartmentName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_EndTreatmentAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_END_TREATMENT_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_FromExamClinicalAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_FROM_EXAM_CLINICAL_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_LaborAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_LABOR_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_Modifier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_MODIFIER", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_MODIFY_TIME", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_PeriodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_PERIOD_CODE", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_PeriodName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_PERIOD_NAME", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_VirFromOtherClinicalAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_VIR_FROM_OTHER_CLINICAL_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Department_VirNotEndTreatmentAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__GRID_CONTROL__COLUMN_VIR_NOT_END_TREATMENT_AMOUNT", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());


                //Repository
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_DEPARTMENT_LIST__BTN_REPOSITORY_DELETE", Base.ResourceLangManager.LanguageUCPeriodDepartmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
