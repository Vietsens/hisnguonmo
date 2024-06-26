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
using DevExpress.XtraEditors.ViewInfo;
using HTC.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.PeriodList.Validation;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HTC.Filter;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PeriodList.MrsFilter;
using DevExpress.XtraBars;
using HIS.Desktop.Plugins.PeriodList.Process;

namespace HIS.Desktop.Plugins.PeriodList
{
    public partial class UCPeriodList : HIS.Desktop.Utility.UserControlBase
    {
        private int positionHandleControl = -1;
        List<HTC_PERIOD> listHtcPeriod = null;
        HTC_PERIOD period = null;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public UCPeriodList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _moduleData;
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCPeriodList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyUcLanguage();
                this.ValidControl();
                ResetValueControl();
                SetEnableButton(true);
                LoadDataSourceGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidControl()
        {
            try
            {
                this.ValidControlYear();
                this.ValidControlMonth();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidControlYear()
        {
            try
            {
                PeriodYearValidationRule yearRule = new PeriodYearValidationRule();
                yearRule.cboYear = cboYear;
                dxValidationProvider1.SetValidationRule(cboYear, yearRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidControlMonth()
        {
            try
            {
                PeriodMonthValidationRule monthRule = new PeriodMonthValidationRule();
                monthRule.cboMonth = cboMonth;
                dxValidationProvider1.SetValidationRule(cboMonth, monthRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControl()
        {
            try
            {
                period = null;
                txtPeriodCode.Text = "";
                txtPeriodName.Text = "";
                cboMonth.SelectedIndex = -1;
                cboYear.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButton(bool isEnable)
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

        private void LoadDataSourceGridControl()
        {
            try
            {
                listHtcPeriod = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HTC_PERIOD>>("api/HtcPeriod/Get", ApiConsumers.HtcConsumer, new HtcPeriodFilter(), null);
                gridControlPeriodList.BeginUpdate();
                gridControlPeriodList.DataSource = listHtcPeriod;
                gridControlPeriodList.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueControlUpdate()
        {
            try
            {
                if (period != null)
                {
                    txtPeriodCode.Text = period.PERIOD_CODE;
                    txtPeriodName.Text = period.PERIOD_NAME;
                    cboYear.SelectedValue = period.YEAR;
                    cboMonth.SelectedValue = period.MONTH;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPeriodName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboYear.Focus();
                    cboYear.Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cboMonth.Focus();
                cboMonth.Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SendKeys.Send("{TAB}");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled || !dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HTC_PERIOD data = new HTC_PERIOD();
                if (!String.IsNullOrEmpty(txtPeriodName.Text))
                {
                    data.PERIOD_NAME = txtPeriodName.Text;
                }

                if (cboYear.SelectedIndex >= 0)
                {
                    data.YEAR = Convert.ToInt16(cboYear.SelectedItem);
                }
                if (cboMonth.SelectedIndex >= 0)
                {
                    data.MONTH = Convert.ToInt16(cboMonth.SelectedItem);
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HTC_PERIOD>(HtcRequestUriStore.HTC_PERIOD__CREATE, ApiConsumers.HtcConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    LoadDataSourceGridControl();
                }
            End:
                WaitingManager.Hide();
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnEdit.Enabled || !dxValidationProvider1.Validate() || period == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                if (!String.IsNullOrEmpty(txtPeriodName.Text))
                {
                    period.PERIOD_NAME = txtPeriodName.Text;
                }

                if (cboYear.SelectedIndex >= 0)
                {
                    period.YEAR = Convert.ToInt16(cboYear.SelectedItem);
                }
                if (cboMonth.SelectedIndex >= 0)
                {
                    period.MONTH = Convert.ToInt16(cboMonth.SelectedItem);
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HTC_PERIOD>(HtcRequestUriStore.HTC_PERIOD__UPDATE, ApiConsumers.HtcConsumer, period, param);
                if (rs != null)
                {
                    success = true;
                    LoadDataSourceGridControl();
                }
            End:
                WaitingManager.Hide();
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ResetValueControl();
                SetEnableButton(true);
                txtPeriodName.Focus();
                txtPeriodName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPeriodList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HTC_PERIOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            try
                            {
                                if (data.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    e.Value = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.DuLieuDangMo);
                                }
                                else
                                {
                                    e.Value = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.DuLieuDangKhoa);
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TIME_FROM_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FROM_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TIME_TO_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TO_TIME ?? 0
);
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

        private void gridViewPeriodList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (HTC_PERIOD)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "CHANGE_LOCK")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = repositoryItemBtnLock;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnUnLock;
                            }
                        }
                        else if (e.Column.FieldName == "RepartitionRatio")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = repositoryItemBtnRepartitionEnable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnRepartitionDisable;
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

        private void gridViewPeriodList_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    period = (HTC_PERIOD)gridViewPeriodList.GetFocusedRow();
                    if (period != null)
                    {
                        SetValueControlUpdate();
                        SetEnableButton(false);
                    }
                    else
                    {
                        ResetValueControl();
                        SetEnableButton(true);
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

        private void repositoryItemBtnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HTC_PERIOD)gridViewPeriodList.GetFocusedRow();
                if (data != null)
                {
                    ChangeLock(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HTC_PERIOD)gridViewPeriodList.GetFocusedRow();
                if (data != null)
                {
                    ChangeLock(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeLock(HTC_PERIOD data)
        {
            try
            {
                if (data != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HTC_PERIOD>(HtcRequestUriStore.HTC_PERIOD__CHANGELOCK, ApiConsumers.HtcConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        data.IS_ACTIVE = rs.IS_ACTIVE;
                        data.MODIFIER = rs.MODIFIER;
                        data.MODIFY_TIME = rs.MODIFY_TIME;
                        data.APP_MODIFIER = rs.APP_MODIFIER;
                        LoadDataSourceGridControl();
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

        private void LoadKeyUcLanguage()
        {
            try
            {
                //Button
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__BTN_ADD", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__BTN_EDIT", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__BTN_NEW", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutPeriodCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__LAYOUT_PERIOD_CODE", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPeriodName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__LAYOUT_PERIOD_NAME", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__LAYOUT_TIME_FROM", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__LAYOUT_TIME_TO", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //gridControl Column
                this.gridColumn_Period_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_CREATE_TIME", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Period_Creator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_CREATOR", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.gridColumn_Period_Year.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_FROM_TIME", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Period_Modifier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_MODIFIER", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Period_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_MODIFY_TIME", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Period_PeriodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_PERIOD_CODE", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Period_PeriodName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_PERIOD_NAME", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Period_PeriodStatus.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_PERIOD_STATUS", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.gridColumn_Period_Month.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_PERIOD_LIST__GRID_CONTROL__COLUMN_TO_TIME", Base.ResourceLangManager.LanguageUCPeriodList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        private void repositoryItemBtnCreateReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                period = (HTC_PERIOD)gridViewPeriodList.GetFocusedRow();
                if (period != null)
                {
                    BarManager bar = new BarManager();
                    bar.Form = this;
                    MrsCreateReportPopupMenuProcessor processor = new MrsCreateReportPopupMenuProcessor(CreateReport_Click, bar);
                    processor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void repositoryItemBtnAllocation_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HTC_PERIOD)gridViewPeriodList.GetFocusedRow();
                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AllocationIndrectFee").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AllocationIndrectFee'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(moduleData);
                        listArgs.Add(data);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnCreateReportExe_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HTC_PERIOD)gridViewPeriodList.GetFocusedRow();
                if (data != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    MRS.SDO.CreateReportSDO sdo = new MRS.SDO.CreateReportSDO();
                    sdo.ReportTemplateCode = "MRS0026501";
                    sdo.ReportTypeCode = "MRS00265";
                    Mrs00265Filter filter = new Mrs00265Filter();
                    filter.PERIOD_ID = data.ID;
                    sdo.Filter = filter;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/MrsReport/Create", ApiConsumers.MrsConsumer, sdo, param);
                    WaitingManager.Hide();
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnRepartitionEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (HTC_PERIOD)gridViewPeriodList.GetFocusedRow();
                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RepartitionRatioCreate").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RepartitionRatioCreate'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateReport_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Tag is MrsCreateReportPopupMenuProcessor.PrintType)
                {
                    var tag = (MrsCreateReportPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (tag)
                    {
                        case MrsCreateReportPopupMenuProcessor.PrintType.TaoBaoCaoHoachToanDoanhThu:
                            this.TaoBaoCaoHoachToanThuChi();
                            break;
                        case MrsCreateReportPopupMenuProcessor.PrintType.TaoBaoCaoTheoKhoaChiDinh:
                            this.TaoBaoCaoTheoKhoaChiDinh();
                            break;
                        case MrsCreateReportPopupMenuProcessor.PrintType.TaoBaoCaoTheoKhoaThucHien:
                            this.TaoBaoCaoTheoKhoaThucHien();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TaoBaoCaoHoachToanThuChi()
        {
            try
            {
                if (period == null) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                MRS.SDO.CreateReportSDO sdo = new MRS.SDO.CreateReportSDO();
                sdo.ReportTemplateCode = "MRS0026601";
                sdo.ReportTypeCode = "MRS00266";
                Mrs00266Filter filter = new Mrs00266Filter();
                filter.PERIOD_ID = period.ID;
                sdo.Filter = filter;
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/MrsReport/Create", ApiConsumers.MrsConsumer, sdo, param);
                WaitingManager.Hide();
                MessageManager.Show(param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TaoBaoCaoTheoKhoaChiDinh()
        {
            try
            {
                if (period == null) return;
                Report.frmCreateReportByDepartment frm = new Report.frmCreateReportByDepartment(period, "MRS00492", this.moduleData);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TaoBaoCaoTheoKhoaThucHien()
        {
            try
            {
                if (period == null) return;
                Report.frmCreateReportByDepartment frm = new Report.frmCreateReportByDepartment(period, "MRS00491", this.moduleData);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
