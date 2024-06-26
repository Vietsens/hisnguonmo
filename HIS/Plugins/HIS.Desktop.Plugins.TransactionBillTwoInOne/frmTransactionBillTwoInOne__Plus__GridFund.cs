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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Config;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne : HIS.Desktop.Utility.FormBase
    {
        private void gridViewTransaction_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "LOCK")
                {
                    var data = (V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IS_CANCEL == HisConfig.IS_TRUE)
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = repositoryItemBtnLockDisable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnUnLockDisable;
                            }
                        }
                        else
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = repositoryItemBtnLock;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnUnlock;
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

        private void gridViewTransaction_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "CREATE_TIME_STR")
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
                        else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.AMOUNT, ConfigApplications.NumberSeperator);
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

        private void repositoryItemBtnLock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewTransaction.FocusedRowHandle >= 0)
                {
                    var data = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                    if (data != null && data.IS_CANCEL != HisConfig.IS_TRUE && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        ProcessChangeLock(data, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnUnlock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewTransaction.FocusedRowHandle >= 0)
                {
                    var data = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                    if (data != null && data.IS_CANCEL != HisConfig.IS_TRUE && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                    {
                        ProcessChangeLock(data, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChangeLock(V_HIS_TRANSACTION data, bool isLock)
        {
            try
            {
                if (data != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HIS_TRANSACTION rs = null;
                    if (isLock)
                    {
                        rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/DepositLock", ApiConsumers.MosConsumer, data.ID, param);
                    }
                    else
                    {
                        rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/DepositUnlock", ApiConsumers.MosConsumer, data.ID, param);
                    }
                    if (rs != null)
                    {
                        success = true;
                        data.IS_ACTIVE = rs.IS_ACTIVE;
                        data.MODIFY_TIME = rs.MODIFY_TIME;
                        data.MODIFIER = rs.MODIFIER;
                        this.CalcuHienDu();
                        this.CalcuCanThu(true);
                        gridControlTransaction.BeginUpdate();
                        gridControlTransaction.DataSource = listTransaction;
                        gridControlTransaction.EndUpdate();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridViewFund_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.Column.FieldName == "AMOUNT")
                    {

                        CalcuCanThu(true);
                        if (Convert.ToDecimal((gridViewFund.GetRowCellValue(e.RowHandle, gridViewFund.Columns["AMOUNT"]) ?? "").ToString()) % 1 == 0)
                        {
                            FormatControl(0, repositoryItemSpinFundAmount);
                            e.Column.DisplayFormat.FormatString = "#,##0";
                            e.Column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

                        }
                        else
                        {
                            string format = FormatControl(ConfigApplications.NumberSeperator, repositoryItemSpinFundAmount);
                            e.Column.DisplayFormat.FormatString = format;
                            e.Column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFund_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                bool valid = false;
                if (e.RowHandle >= 0)
                {
                    var data = (VHisBillFundADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        valid = true;
                        if (e.Column.FieldName == "FUND_NAME")
                        {
                            if (data.IsNotEdit)
                            {
                                e.RepositoryItem = repositoryItemTxtFundName;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCboFund;
                            }
                        }
                        else if (e.Column.FieldName == "AMOUNT")
                        {
                            if (data.IsNotEdit)
                            {
                                e.RepositoryItem = repositoryItemSpinFundAmountDisable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemSpinFundAmount;
                            }
                        }
                        else if (e.Column.FieldName == "Delete")
                        {
                            if (data.IsNotEdit)
                            {
                                e.RepositoryItem = repositoryItemBtnDeleteFundDisable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnDeleteFund;
                            }
                        }
                    }
                }
                if (!valid)
                {
                    if (e.Column.FieldName == "FUND_NAME")
                    {
                        e.RepositoryItem = repositoryItemCboFund;
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        e.RepositoryItem = repositoryItemSpinFundAmount;
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = repositoryItemBtnDeleteFund;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFund_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisBillFundADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void gridViewFund_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFund_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Base.ColumnView view = (DevExpress.XtraGrid.Views.Base.ColumnView)sender;
                if (view.FocusedColumn.FieldName == "FUND_NAME")
                {
                    gridViewFund.ShowEditor();
                    if (gridViewFund.ActiveEditor is LookUpEdit)
                        ((LookUpEdit)gridViewFund.ActiveEditor).ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFund_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                int count = 0;
                GridView view = sender as GridView;
                if (view.GetRowCellValue(e.RowHandle, view.Columns["FUND_NAME"]) == null || view.GetRowCellValue(e.RowHandle, view.Columns["FUND_CODE"]) == null)
                {
                    if (count == 0) count = 1;
                    e.Valid = false;
                    view.SetColumnError(view.Columns["FUND_NAME"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__GRID_BILL_FUND__VALID_FUND_NOT_FOUND", Base.ResourceLangManager.LanguageFrmTransactionBillTwoInOne, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                }

                if (Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, view.Columns["AMOUNT"])) <= 0)
                {
                    if (count == 0) count = 2;
                    e.Valid = false;
                    view.SetColumnError(view.Columns["AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__GRID_BILL_FUND__VALID_AMOUNT_GREAT_THAN_ZERO", Base.ResourceLangManager.LanguageFrmTransactionBillTwoInOne, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                }
                if (count == 1)
                {
                    gridViewFund.FocusedColumn = view.Columns["FUND_NAME"];
                }
                else if (count == 2)
                {
                    gridViewFund.FocusedColumn = view.Columns["AMOUNT"];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboFund_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewFund.PostEditor();
                long fundId = Convert.ToInt64(gridViewFund.EditingValue);
                if (fundId > 0)
                {
                    var fund = BackendDataWorker.Get<HIS_FUND>().FirstOrDefault(o => o.ID == fundId);
                    if (fund != null)
                    {
                        gridViewFund.SetFocusedRowCellValue("FUND_CODE", fund.FUND_CODE);
                        gridViewFund.SetFocusedRowCellValue("FUND_ID", fund.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDeleteFund_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                gridViewFund.PostEditor();
                gridViewFund.DeleteRow(gridViewFund.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
