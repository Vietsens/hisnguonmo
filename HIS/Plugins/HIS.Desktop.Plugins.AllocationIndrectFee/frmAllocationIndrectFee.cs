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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.AllocationIndrectFee.Validation;
using HTC.EFMODEL.DataModels;
using HTC.SDO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AllocationIndrectFee
{
    public partial class frmAllocationIndrectFee : HIS.Desktop.Utility.FormBase
    {
        HTC_PERIOD period = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        private int positionHandleControl = -1;

        public frmAllocationIndrectFee(Inventec.Desktop.Common.Modules.Module module, HTC_PERIOD data)
		:base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                period = data;
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

        private void frmAllocationIndrectFee_Load(object sender, EventArgs e)
        {
            try
            {
                ValidControl();
                this.LoadKeyFrmLanguage();
                if (period != null)
                {
                    lblPeriod.Text = period.PERIOD_CODE + " - " + period.PERIOD_NAME;
                    spinTotalAmount.Focus();
                    spinTotalAmount.SelectAll();
                }
                else
                {
                    btnSave.Enabled = false;
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
                ValidControlTotalAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlTotalAmount()
        {
            try
            {
                TotalAmountValidationRule amountRule = new TotalAmountValidationRule();
                amountRule.spinTotalAmount = spinTotalAmount;
                dxValidationProvider1.SetValidationRule(spinTotalAmount, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpenseType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (V_HTC_EXPENSE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void spinTotalAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.period == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HtcAllocationIndrectFeeSDO data = new HtcAllocationIndrectFeeSDO();
                data.PeriodId = period.ID;
                data.Amount = spinTotalAmount.Value;
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<V_HTC_EXPENSE>>("api/HtcPeriod/AllocationIndrectFee", ApiConsumers.HtcConsumer, data, param);
                if (rs != null && rs.Count > 0)
                {
                    success = true;
                    gridControlExpenseType.BeginUpdate();
                    gridControlExpenseType.DataSource = rs;
                    gridControlExpenseType.EndUpdate();
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

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
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

        private void LoadKeyFrmLanguage()
        {
            try
            {
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var lang = Base.ResourceLangManager.LanguageFrmAllocationIndrectFee;
                //Button

                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__BTN_SAVE", lang, cul);               
                //Layout
                this.layoutPeriod.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__LAYOUT_PERIOD", lang, cul);
                this.layoutTotalAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__LAYOUT_TOTAL_AMOUNT", lang, cul);

                //grid Expense
                this.gridColumn_Expense_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_CREATE_TIME", lang, cul);
                this.gridColumn_Expense_Creator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_CREATOR", lang, cul);
                this.gridColumn_Expense_Department.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_DEPARTMENT", lang, cul);
                this.gridColumn_Expense_ExpenseCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_EXPENSE_CODE", lang, cul);
                this.gridColumn_Expense_ExpenseType.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_EXPENSE_TYPE", lang, cul);
                this.gridColumn_Expense_Modifier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_MODIFIER", lang, cul);
                this.gridColumn_Expense_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_MODIFY_TIME", lang, cul);
                this.gridColumn_Expense_PeriodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_PERIOD_CODE", lang, cul);
                this.gridColumn_Expense_PeriodName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_PERIOD_NAME", lang, cul);
                this.gridColumn_Expense_price.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_PRICE", lang, cul);
                this.gridColumn_Expense_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ALLOCATION_INDRECT_FEE__GRID_EXPENSE_TYPE__COLUMN_STT", lang, cul);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
