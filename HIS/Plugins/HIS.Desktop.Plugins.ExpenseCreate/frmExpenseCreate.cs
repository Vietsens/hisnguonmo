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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExpenseCreate.ADO;
using HIS.Desktop.Plugins.ExpenseCreate.Validation;
using HTC.EFMODEL.DataModels;
using HTC.Filter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpenseCreate
{
    public partial class frmExpenseCreate : HIS.Desktop.Utility.FormBase
    {
        private int positionHandleControl = -1;

        List<HTC_EXPENSE_TYPE> listExpenseType;

        List<HtcExpenseADO> listExpense = new List<HtcExpenseADO>();

        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmExpenseCreate(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExpenseCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.ValidControl();
                this.LoadDataToComboPeriod();
                this.LoadDataToComboExpenseType();
                this.ResetGridExpense();
                this.SetDefaultPeriod();
                this.SetEnableBtnSave(true);
                txtExpenseTypeCode.Focus();
                txtExpenseTypeCode.SelectAll();
                SetCaptionByLanguageKey();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExpenseCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpenseCreate.frmExpenseCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmExpenseCreate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmExpenseCreate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmExpenseCreate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmExpenseCreate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmExpenseCreate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmExpenseCreate.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExpenseType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmExpenseCreate.cboExpenseType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPeriod.Properties.NullText = Inventec.Common.Resource.Get.Value("frmExpenseCreate.cboPeriod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutPeriod.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.layoutPeriod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutExpenseType.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.layoutExpenseType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutExpenseTime.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.layoutExpenseTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmExpenseCreate.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCRefresh.Caption = Inventec.Common.Resource.Get.Value("frmExpenseCreate.bbtnRCRefresh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalPrice.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.layoutTotalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmExpenseCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void LoadDataToComboExpenseType()
        {
            try
            {
                listExpenseType = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HTC_EXPENSE_TYPE>>("api/HtcExpenseType/Get", ApiConsumers.HtcConsumer, new HtcExpenseTypeFilter(), null);
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

        private void ResetGridExpense()
        {
            try
            {
                listExpense = new List<HtcExpenseADO>();
                var listDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().OrderBy(o => o.NUM_ORDER).ToList();
                listDepart = listDepart.OrderBy(o => o.NUM_ORDER).ToList();
                foreach (var item in listDepart)
                {
                    HtcExpenseADO ado = new HtcExpenseADO(item);
                    listExpense.Add(ado);
                }
                gridControlExpense.BeginUpdate();
                gridControlExpense.DataSource = listExpense;
                gridControlExpense.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultPeriod()
        {
            try
            {
                var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__TRUE);
                if (period != null)
                {
                    txtPeriodCode.Text = period.PERIOD_CODE;
                    cboPeriod.EditValue = period.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableBtnSave(bool enable)
        {
            try
            {
                btnSave.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlPeriod();
                ValidControlExpenseType();
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

        private void txtPeriodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPeriodCode.Text))
                    {
                        string key = txtPeriodCode.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HTC_PERIOD>().Where(o => o.PERIOD_CODE.ToLower().Contains(key) || o.PERIOD_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            txtPeriodCode.Text = listData.First().PERIOD_CODE;
                            cboPeriod.EditValue = listData.First().ID;
                            txtExpenseTypeCode.Focus();
                            txtExpenseTypeCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboPeriod.Focus();
                        cboPeriod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPeriod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboPeriod.EditValue != null)
                    {
                        var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                        if (period != null)
                        {
                            txtPeriodCode.Text = period.PERIOD_CODE;
                        }
                    }
                    txtExpenseTypeCode.Focus();
                    txtExpenseTypeCode.Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpenseTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtExpenseTypeCode.Text))
                    {
                        string key = txtExpenseTypeCode.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HTC_EXPENSE_TYPE>().Where(o => o.EXPENSE_TYPE_CODE.ToLower().Contains(key) || o.EXPENSE_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            txtExpenseTypeCode.Text = listData.First().EXPENSE_TYPE_CODE;
                            cboExpenseType.EditValue = listData.First().ID;
                            dtExpenseTime.Focus();
                            dtExpenseTime.ShowPopup();
                        }
                    }
                    if (!valid)
                    {
                        cboExpenseType.Focus();
                        cboExpenseType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpenseType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboExpenseType.EditValue != null)
                    {
                        var expenseType = BackendDataWorker.Get<HTC_EXPENSE_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpenseType.EditValue));
                        if (expenseType != null)
                        {
                            txtExpenseTypeCode.Text = expenseType.EXPENSE_TYPE_CODE;
                        }
                    }
                    dtExpenseTime.Focus();
                    dtExpenseTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpenseTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpense_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.ListSourceRowIndex >= 0 && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HtcExpenseADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpense_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (HtcExpenseADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsError)
                        {
                            e.Appearance.ForeColor = Color.Yellow;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpense_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "PRICE")
                {
                    CalculatorTotalPrice();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewExpense.PostEditor();
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || listExpense == null || listExpense.Count == 0)
                    return;
                WaitingManager.Show();
                gridViewExpense.PostEditor();
                CommonParam param = new CommonParam();
                bool success = false;
                var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                var expenseType = listExpenseType.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpenseType.EditValue));
                long expenseTime = 0;
                if (dtExpenseTime.EditValue != null && dtExpenseTime.DateTime != DateTime.MinValue)
                {
                    expenseTime = Convert.ToInt64(dtExpenseTime.DateTime.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    expenseTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                }

                if (period != null && expenseType != null)
                {
                    int count = 0;
                    foreach (var data in listExpense)
                    {
                        if (data.PRICE < 0)
                        {
                            data.IsError = true;
                            data.DESCRIPTION = "Số tiền nhỏ hơn 0";
                            count++;
                            continue;
                        }
                        if (!String.IsNullOrEmpty(data.DEPARTMENT_CODE))
                        {
                            data.PERIOD_ID = period.ID;
                            data.EXPENSE_TYPE_ID = expenseType.ID;
                            data.EXPENSE_TIME = expenseTime;
                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HTC_EXPENSE>("api/HtcExpense/Create", ApiConsumers.HtcConsumer, data, param);
                            if (rs != null)
                            {
                                success = true;
                                data.DEPARTMENT_CODE = rs.DEPARTMENT_CODE;
                                data.DEPARTMENT_NAME = rs.DEPARTMENT_NAME;
                                data.PRICE = rs.PRICE;
                            }
                            else
                            {
                                count++;
                                data.IsError = true;
                                data.DESCRIPTION = !String.IsNullOrEmpty(param.GetMessage()) ? param.GetMessage() : "Tạo dữ liệu thất bại";
                                param = new CommonParam();
                            }
                        }
                    }
                    if (count > 0)
                    {
                        //MessageUtil.SetMessage(param, LibraryMessage.Message.Enum.Plugin_ExpenseCreate__CoDuLieuKhongTaoDuoc, count + "");
                    }
                }
                gridControlExpense.BeginUpdate();
                gridControlExpense.DataSource = listExpense;
                gridControlExpense.EndUpdate();
                if (success)
                {
                    SetEnableBtnSave(false);
                }
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRefresh.Enabled)
                    return;
                ResetGridExpense();
                dtExpenseTime.EditValue = null;
                txtExpenseTypeCode.Text = "";
                cboExpenseType.EditValue = null;
                txtExpenseTypeCode.Focus();
                txtExpenseTypeCode.SelectAll();
                SetEnableBtnSave(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewExpense.FocusedRowHandle >= 0)
                {
                    var data = (HtcExpenseADO)gridViewExpense.GetFocusedRow();
                    if (data != null)
                    {
                        listExpense.Remove(data);
                        gridControlExpense.BeginUpdate();
                        gridControlExpense.DataSource = listExpense;
                        gridControlExpense.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void bbtnRCRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefresh_Click(null, null);
        }

        private void CalculatorTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                if (listExpense != null && listExpense.Count > 0)
                {
                    totalPrice = listExpense.Sum(s => s.PRICE);
                }
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
