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
using DevExpress.XtraEditors;
using HTC.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HTC.Filter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.ExpenseList.Validation;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ExpenseList
{
    public partial class UCExpenseList : UserControl
    {
        private int positionHandleControl = -1;
        int rowCount = 0;
        int dataTotal = 0;

        List<HTC_EXPENSE_TYPE> listExpenseType;

        List<V_HTC_EXPENSE> ListHtcExpense = new List<V_HTC_EXPENSE>();
        V_HTC_EXPENSE expense = null;
        int start;
        int limit;

        const string BTN_ADD_CONTROL_CODE = "BTN000001";
        const string BTN_EDIT_CONTROL_CODE = "BTN000002";
        const string BTN_REFRESH_CONTROL_CODE = "BTN000003";
        const string BTN_CREATE_LIST_CODE = "BTN000004";
        const string BTN_CREATE_TYPE_CODE = "BTN000005";

        public UCExpenseList()
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

        private void UCExpenseList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyUcLanguage();
                LoadDataToComboFilterPeriod();
                LoadDataToComboPeriod();
                LoadDataToComboFilterExpenseType();
                LoadDataToComboExpenseType();
                LoadDataToComboDepartment();
                InitControl();
                SetDefaultControl();
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

        private void InitControl()
        {
            try
            {
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                btnNew.Enabled = false;
                btnCreateList.Enabled = false;
                btnCreateType.Enabled = false;
                
                if (GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null || GlobalVariables.AcsAuthorizeSDO.ControlInRoles.Count > 0)
                {
                    var controlAdd = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN_ADD_CONTROL_CODE);
                    if (controlAdd != null)
                    {
                        btnAdd.Enabled = true;
                    }

                    var controlEdit = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN_EDIT_CONTROL_CODE);
                    if (controlEdit != null)
                    {
                        btnEdit.Enabled = true;
                    }
                    else
                    {
                        gridCol_ExpenseDelete.Visible = false;
                    }

                    var controlRefresh = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN_REFRESH_CONTROL_CODE);
                    if (controlRefresh != null)
                    {
                        btnNew.Enabled = true;
                    }

                    var controlCreateList = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN_CREATE_LIST_CODE);
                    if (controlCreateList != null)
                    {
                        btnCreateList.Enabled = true;
                    }

                    var controlCreateType = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN_CREATE_TYPE_CODE);
                    if (controlCreateType != null)
                    {
                        btnCreateType.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                cboFilterPeriod.EditValue = null;
                cboFilterExpenseType.EditValue = null;
                dtCreateTimeFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtCreateTimeTo.DateTime = DateTime.Now;
                dtExpenseTimeFrom.EditValue = null;
                dtExpenseTimeTo.EditValue = null;
                txtKeyword.Text = "";
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

        private void LoadDataToComboFilterExpenseType()
        {
            try
            {
                cboFilterExpenseType.Properties.DataSource = BackendDataWorker.Get<HTC_EXPENSE_TYPE>().Where(o => o.IS_ALLOW_EXPENSE == IMSys.DbConfig.HTC_RS.HTC_EXPENSE_TYPE.IS_ALLOW_EXPENSE__TRUE).OrderByDescending(o => o.EXPENSE_TYPE_CODE).ToList();
                cboFilterExpenseType.Properties.DisplayMember = "EXPENSE_TYPE_NAME";
                cboFilterExpenseType.Properties.ValueMember = "ID";
                cboFilterExpenseType.Properties.ForceInitialize();
                cboFilterExpenseType.Properties.Columns.Clear();
                cboFilterExpenseType.Properties.Columns.Add(new LookUpColumnInfo("EXPENSE_TYPE_CODE", "Mã", 50));
                cboFilterExpenseType.Properties.Columns.Add(new LookUpColumnInfo("EXPENSE_TYPE_NAME", "Tên", 200));
                cboFilterExpenseType.Properties.ShowHeader = true;
                cboFilterExpenseType.Properties.ImmediatePopup = true;
                cboFilterExpenseType.Properties.DropDownRows = 10;
                cboFilterExpenseType.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboExpenseType()
        {
            try
            {
                listExpenseType = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HTC_EXPENSE_TYPE>>(HtcRequestUriStore.HTC_EXPENSE_TYPE__GET, ApiConsumers.HtcConsumer, new HtcExpenseTypeFilter(), null);
                cboExpenseType.Properties.DataSource = listExpenseType.Where(o => o.IS_ALLOW_EXPENSE == IMSys.DbConfig.HTC_RS.HTC_EXPENSE_TYPE.IS_ALLOW_EXPENSE__TRUE).OrderByDescending(o => o.EXPENSE_TYPE_CODE).ToList();
                cboExpenseType.Properties.DisplayMember = "EXPENSE_TYPE_NAME";
                cboExpenseType.Properties.ValueMember = "ID";
                cboExpenseType.Properties.ForceInitialize();
                cboExpenseType.Properties.Columns.Clear();
                cboExpenseType.Properties.Columns.Add(new LookUpColumnInfo("EXPENSE_TYPE_CODE", "Mã", 50));
                cboExpenseType.Properties.Columns.Add(new LookUpColumnInfo("EXPENSE_TYPE_NAME", "Tên", 200));
                cboExpenseType.Properties.ShowHeader = true;
                cboExpenseType.Properties.ImmediatePopup = true;
                cboExpenseType.Properties.DropDownRows = 10;
                cboExpenseType.Properties.PopupWidth = 250;
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
                ListHtcExpense = new List<V_HTC_EXPENSE>();
                gridControlExpenseList.DataSource = null;
                ResetValueControlCreate();
                SetEnableButtonCreate(true);
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HtcExpenseViewFilter filter = new HtcExpenseViewFilter();
                if (!String.IsNullOrEmpty(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text;
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
                if (cboFilterExpenseType.EditValue != null)
                {
                    filter.EXPENSE_TYPE_ID = BackendDataWorker.Get<HTC_EXPENSE_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboFilterExpenseType.EditValue)).ID;
                }
                if (dtExpenseTimeFrom.EditValue != null && dtExpenseTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.EXPENSE_TIME_FROM = Convert.ToInt64(dtExpenseTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtExpenseTimeTo.EditValue != null && dtExpenseTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.EXPENSE_TIME_TO = Convert.ToInt64(dtExpenseTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HTC_EXPENSE>>(HtcRequestUriStore.HTC_EXPENSE__GETVIEW, ApiConsumers.HtcConsumer, filter, paramCommon);
                if (result != null)
                {
                    ListHtcExpense = (List<V_HTC_EXPENSE>)result.Data;
                    rowCount = (ListHtcExpense == null ? 0 : ListHtcExpense.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlExpenseList.BeginUpdate();
                    gridControlExpenseList.DataSource = ListHtcExpense;
                    gridControlExpenseList.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpenseList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HTC_EXPENSE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "EXPENSE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXPENSE_TIME);
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

        private void gridViewExpenseList_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    expense = (V_HTC_EXPENSE)gridViewExpenseList.GetFocusedRow();
                    if (expense != null)
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

        private void repositoryItemBtnDeleteExpense_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HTC_EXPENSE)gridViewExpenseList.GetFocusedRow();
                if (data != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HtcRequestUriStore.HTC_EXPENSE__DELETE, ApiConsumers.HtcConsumer, data.ID, param);
                    if (success)
                    {
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlPeriod();
                ValidControlExpenseType();
                ValidControlDepartment();
                ValidControlPrice();
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

        private void ValidControlExpenseType()
        {
            try
            {
                ExpenseTypeValidationRule expenseTypeRule = new ExpenseTypeValidationRule();
                expenseTypeRule.txtExpenseTypeCode = txtExpenseTypeCode;
                expenseTypeRule.cboExpenseType = cboExpenseType;
                dxValidationProvider1.SetValidationRule(txtExpenseTypeCode, expenseTypeRule);
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
                DepartmentValidationRule departmentRule = new DepartmentValidationRule();
                departmentRule.txtDepartmentCode = txtDepartmentCode;
                departmentRule.cboDepartment = cboDepartment;
                dxValidationProvider1.SetValidationRule(txtDepartmentCode, departmentRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPrice()
        {
            try
            {
                PriceValidationRule priceRule = new PriceValidationRule();
                priceRule.txtPrice = txtPrice;
                dxValidationProvider1.SetValidationRule(txtPrice, priceRule);
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

        private void LoadKeyUcLanguage()
        {
            try
            {
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__BTN_FIND", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__BTN_ADD", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__BTN_EDIT", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__BTN_NEW", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__BTN_REFRESH", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_TIME_FROM", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_TIME_TO", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutExpenseTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_TIME_FROM", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutExpenseTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_TIME_TO", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutExpenseType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_EXPENSE_TYPE", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPeriod.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_PERIOD", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutExpenseTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_EXPENSE_TIME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDepartment.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_DEPARTMENT", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__LAYOUT_PRICE", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //navbargroup
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__NAV_BAR_CONTROL__GROUP_CREATE_TIME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.navBarGroupExpenseTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__NAV_BAR_CONTROL__GROUP_EXPENSE_TIME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.navBarGroupExpenseType.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__NAV_BAR_CONTROL__GROUP_EXPENSE_TYPE", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.navBarGroupPeriod.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__NAV_BAR_CONTROL__GROUP_PERIOD", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //gridControl column
                this.gridColumn_Expense_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_CREATE_TIME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_Creator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_CREATOR", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_DepartmentName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_ExpenseCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_EXPENSE_CODE", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_ExpenseTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_EXPENSE_TIME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_ExpenseTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_EXPENSE_TYPE_CODE", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_ExpenseTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_EXPENSE_TYPE_NAME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_Modifier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_MODIFIER", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_MODIFY_TIME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_PeriodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_PERIOD_CODE", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_PeriodName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_PERIOD_NAME", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Expense_Price.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__GRID_CONTROL__COLUMN_PRICE", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Repository
                this.repositoryItemBtnDeleteExpense.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPENSE_LIST__BTN_REPOSITORY_DELETE", Base.ResourceLangManager.LanguageUCExpenseList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreateType_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCreateType.Enabled)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpenseTypeList").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ExpenseTypeList'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    LoadDataToComboFilterExpenseType();
                    LoadDataToComboExpenseType();
                }
                else
                {
                    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreateList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCreateList.Enabled)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpenseCreate").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ExpenseCreate'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToGridExpense(new CommonParam(start, limit));
                }
                else
                {
                    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
