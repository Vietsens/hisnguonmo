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
using HTC.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using HTC.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpenseByPeriod.Validation;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.ExpenseByPeriod
{
    public partial class UCExpenseByPeriod : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        List<HtcExpenseByPeriodSDO> listSdo = new List<HtcExpenseByPeriodSDO>();
        BindingList<HtcExpenseByPeriodSDO> records;

        private int positionHandleControl = -1;

        public UCExpenseByPeriod(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpenseByPeriod_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ValidControl();
                LoadDataToComboPeriod();
                FillDataToTreeList();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExpenseByPeriod.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpenseByPeriod.UCExpenseByPeriod).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_ExpenseTypeCode.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_ExpenseTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_ExpenseTypeName.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_ExpenseTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_ExpenseCode.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_ExpenseCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_Department.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_Department.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_Price.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_ExpenseTime.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_ExpenseTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_CreateTime.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_CreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_Creator.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_Creator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_ModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ExpenseSdo_Modifier.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.treeListColumn_ExpenseSdo_Modifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnShow.Text = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.btnShow.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPeriod.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.cboPeriod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutPeriod.Text = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.layoutPeriod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.radioClose.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.radioClose.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.radioOpen.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpenseByPeriod.radioOpen.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void FillDataToTreeList()
        {
            try
            {
                records = new BindingList<HtcExpenseByPeriodSDO>(listSdo);
                treeListExpenseSdo.DataSource = records;
                if (radioClose.Checked)
                {
                    treeListExpenseSdo.CollapseAll();
                }
                else
                {
                    treeListExpenseSdo.ExpandAll();
                }
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

        private void treeListExpenseSdo_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Node != null && e.Column.UnboundType != DevExpress.XtraTreeList.Data.UnboundColumnType.Bound)
                {
                    HtcExpenseByPeriodSDO data = (HtcExpenseByPeriodSDO)e.Row;
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EXPENSE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXPENSE_TIME);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListExpenseSdo_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = treeListExpenseSdo.GetDataRecordByNode(e.Node);
                if (data != null && data is HtcExpenseByPeriodSDO)
                {
                    var noteData = (HtcExpenseByPeriodSDO)data;
                    if (String.IsNullOrEmpty(noteData.ParentId))
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
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
                        string key = txtPeriodCode.Text.ToLower();
                        var listData = BackendDataWorker.Get<HTC_PERIOD>().Where(o => o.PERIOD_CODE.ToLower().Contains(key) || (!String.IsNullOrEmpty(o.PERIOD_NAME) && o.PERIOD_NAME.ToLower().Contains(key))).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboPeriod.EditValue = listData.First().ID;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
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
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPeriod_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                listSdo = new List<HtcExpenseByPeriodSDO>();
                txtPeriodCode.Text = "";
                if (cboPeriod.EditValue != null)
                {
                    var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                    if (period != null)
                    {
                        txtPeriodCode.Text = period.PERIOD_CODE;
                    }
                }
                FillDataToTreeList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioClose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioClose.Checked)
                {
                    treeListExpenseSdo.CollapseAll();
                }
                else
                {
                    treeListExpenseSdo.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnShow.Enabled || !dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                if (period != null)
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HtcExpenseByPeriodSDO>>("api/HtcExpense/GetSdoByPeriod", ApiConsumers.HtcConsumer, period.ID, param);
                    if (rs != null)
                    {
                        success = true;
                        listSdo = rs;
                        FillDataToTreeList();
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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
    }
}
