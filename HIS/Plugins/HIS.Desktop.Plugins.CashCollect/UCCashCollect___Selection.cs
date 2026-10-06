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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.CashCollect.Resources;
using HIS.Desktop.Utility;
using HIS.UC.CashCollect;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.CashCollect
{
    /// <summary>
    /// Deposit list (right grid) - task 55505.
    /// Selected transactions are kept by ID independently of the current page, so a selection survives paging
    /// and "select all by filter" can pick every page. The right grid always equals what will be sent to the server.
    /// </summary>
    public partial class UCCashCollect : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>Transactions newly selected for deposit (not in any cashout yet). Key = transaction ID.</summary>
        Dictionary<long, CashCollectADO> dicSelectedTransaction = new Dictionary<long, CashCollectADO>();

        /// <summary>Filter of the last search; reused by paging and by "select all" so both match the grid content.</summary>
        HisTransactionViewFilter currentTransactionFilter;

        private void checkAll_Click(bool isCheckAll)
        {
            try
            {
                if (isCheckAll)
                {
                    SelectAllTransactionByFilter();
                }
                else
                {
                    dicSelectedTransaction.Clear();
                    SetCheckAllState(false);
                    ApplySelectionToCurrentPage();
                    RefreshCollectGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Selects every not-yet-deposited transaction matching the last search (all pages).</summary>
        private void SelectAllTransactionByFilter()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                HisTransactionViewFilter filter = this.currentTransactionFilter ?? BuildTransactionFilter();
                List<V_HIS_TRANSACTION> transactions = null;
                // Only not-yet-deposited transactions can be selected. The search filter is changed for this call only
                // (not copied: a mapper copy turns null ID lists into empty lists, and the backend then returns nothing).
                bool? searchHasCashout = filter.HAS_CASHOUT;
                try
                {
                    filter.HAS_CASHOUT = false;
                    Inventec.Common.Logging.LogSystem.Debug("SelectAllTransactionByFilter" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    transactions = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>(
                        HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_TRANSACTION_GETVIEW,
                        ApiConsumers.MosConsumer,
                        filter,
                        param);
                }
                finally
                {
                    filter.HAS_CASHOUT = searchHasCashout;
                }
                WaitingManager.Hide();

                if (transactions == null)
                {
                    SetCheckAllState(false);
                    MessageManager.Show(this.ParentForm, param, false);
                    SessionManager.ProcessTokenLost(param);
                    return;
                }

                // A backend older than 02/10/2026 ignores CASHIER_ROOM_IDs: check the room again here
                // so that another room's cash can never be deposited by "select all".
                HashSet<long> roomIds = (filter.CASHIER_ROOM_IDs != null && filter.CASHIER_ROOM_IDs.Count > 0) ? new HashSet<long>(filter.CASHIER_ROOM_IDs) : null;
                List<V_HIS_TRANSACTION> selectables = transactions
                    .Where(o => !o.CASHOUT_ID.HasValue && (roomIds == null || roomIds.Contains(o.CASHIER_ROOM_ID)))
                    .ToList();
                Inventec.Common.Logging.LogSystem.Info("SelectAllTransactionByFilter: api returned " + transactions.Count + " transactions, selectable " + selectables.Count);

                if (selectables.Count == 0)
                {
                    SetCheckAllState(false);
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongCoGiaoDichChuaNopQuyTheoDieuKienLoc,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                foreach (V_HIS_TRANSACTION item in selectables)
                {
                    CashCollectADO ado = new CashCollectADO(item);
                    ado.check = true;
                    dicSelectedTransaction[item.ID] = ado;
                }
                SetCheckAllState(true);
                ApplySelectionToCurrentPage();
                RefreshCollectGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                SetCheckAllState(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Copies the check state of the current page into the selection (called after a row check changed).</summary>
        private void SyncSelectionWithCurrentPage()
        {
            try
            {
                List<CashCollectADO> pageData = CashCollectProcessor.GetDataGridView(ucGridControl) as List<CashCollectADO>;
                if (pageData == null) return;

                bool hasDeselected = false;
                foreach (CashCollectADO item in pageData)
                {
                    if (item.CASHOUT_ID.HasValue) continue;
                    if (item.check)
                    {
                        dicSelectedTransaction[item.ID] = item;
                    }
                    else if (dicSelectedTransaction.Remove(item.ID))
                    {
                        hasDeselected = true;
                    }
                }
                if (hasDeselected)
                {
                    SetCheckAllState(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Shows the selection on the rows of the current page.</summary>
        private void ApplySelectionToCurrentPage()
        {
            try
            {
                List<CashCollectADO> pageData = CashCollectProcessor.GetDataGridView(ucGridControl) as List<CashCollectADO>;
                if (pageData == null) return;

                foreach (CashCollectADO item in pageData)
                {
                    item.check = !item.CASHOUT_ID.HasValue && dicSelectedTransaction.ContainsKey(item.ID);
                }
                // New list instance so that the grid rebinds and repaints every check box
                CashCollectProcessor.Reload(ucGridControl, new List<CashCollectADO>(pageData));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Header select-all checkbox state (kept by the grid UC).</summary>
        private void SetCheckAllState(bool isCheckAll)
        {
            try
            {
                if (CashCollectProcessor != null && ucGridControl != null)
                {
                    CashCollectProcessor.SetCheckAll(ucGridControl, isCheckAll);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Clears the newly selected transactions (transactions already in the edited cashout are kept).</summary>
        private void ClearSelectedTransactions()
        {
            try
            {
                dicSelectedTransaction.Clear();
                SetCheckAllState(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Content of the right grid = newly selected transactions + transactions kept in the edited cashout.</summary>
        private List<CashCollectADO> GetTransactionsToCollect()
        {
            List<CashCollectADO> result = new List<CashCollectADO>();
            try
            {
                result.AddRange(dicSelectedTransaction.Values.OrderByDescending(o => o.CREATE_TIME ?? 0));
                if (dataClick != null)
                {
                    foreach (V_HIS_TRANSACTION item in dataClick)
                    {
                        if (!dicSelectedTransaction.ContainsKey(item.ID))
                        {
                            result.Add(new CashCollectADO(item));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void RefreshCollectGrid()
        {
            try
            {
                List<CashCollectADO> collects = GetTransactionsToCollect();
                gridControlCollect.BeginUpdate();
                gridControlCollect.DataSource = collects;
                gridControlCollect.EndUpdate();

                totalPay = CalcCashoutAmount(collects);
                if (collects.Count > 0)
                {
                    txtAmountSum.Text = Inventec.Common.Number.Convert.NumberToStringRoundMax4(totalPay);
                }
                else
                {
                    txtAmountSum.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cashout amount, same formula as MOS HisCashoutCreate/HisCashoutUpdate (the server rejects the request when they differ):
        /// payment (TT) and deposit (TU) are added, repayment (HU) is subtracted, then exemption, carried amount (KC) and bill fund are subtracted.
        /// </summary>
        internal static decimal CalcCashoutAmount(IEnumerable<V_HIS_TRANSACTION> transactions)
        {
            decimal result = 0;
            if (transactions == null) return result;
            foreach (V_HIS_TRANSACTION item in transactions)
            {
                if (item.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || item.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                {
                    result += item.AMOUNT;
                }
                else if (item.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                {
                    result -= item.AMOUNT;
                }
                result -= (item.EXEMPTION ?? 0) + (item.KC_AMOUNT ?? 0) + (item.TDL_BILL_FUND_AMOUNT ?? 0);
            }
            return result;
        }

        /// <summary>Leaves cashout edit mode and empties the deposit list. The chosen deposit time is kept.</summary>
        private void ResetToAddMode()
        {
            try
            {
                cashoutId = 0;
                currentData = null;
                dataClick = new List<V_HIS_TRANSACTION>();
                ClearSelectedTransactions();
                btnEdit.Enabled = false;
                btnAdd.Enabled = true;
                RefreshCollectGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Deposit time from "Ngày nộp" as yyyyMMddHHmmss; 0 when empty.</summary>
        private long GetCashoutTimeInput()
        {
            long result = 0;
            try
            {
                if (dtCashOutTime.EditValue != null && dtCashOutTime.DateTime != DateTime.MinValue)
                {
                    result = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtCashOutTime.DateTime) ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
