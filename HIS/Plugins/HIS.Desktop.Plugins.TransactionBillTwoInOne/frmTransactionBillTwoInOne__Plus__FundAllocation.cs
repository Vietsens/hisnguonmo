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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    /// <summary>
    /// Fund allocation (commercial guarantee insurance) of the treatment, same rules as the one-book screen (TransactionBill):
    /// - allocation saved but not finalized: billing is not allowed
    /// - allocation finalized but a line allocated to the insurer changed its cost afterwards: billing is not allowed
    /// - allocation finalized: the fund line takes the allocated insurer amount, deposits are not offset
    ///   and the whole remaining deposit is refunded in the same payment (auto repay).
    ///   The fund amount is split over the two books: the receipt book first, the rest goes to the invoice book
    /// </summary>
    public partial class frmTransactionBillTwoInOne
    {
        private const short FUND_ALLOCATION_IS_FINISH = 1;
        private const short FUND_ALLOCATION_IS_COST_CHANGED = 1;
        private const short FUND_ALLOCATION_LINE_CHANGED = 2;
        private const short FUND_ALLOCATION_LINE_REMOVED = 3;

        private V_HIS_FUND_ALLOCATION currentFundAllocation = null;
        private bool isFundAllocationLineChanged = false;
        //Allocated insurer amount not yet paid on the treatment's bills
        private decimal fundAllocationRemain = 0;
        //Fund line of the allocation in the fund grid (amount = receipt part + invoice part)
        private VHisBillFundADO fundAllocationRow = null;

        private void LoadFundAllocation()
        {
            try
            {
                this.currentFundAllocation = null;
                this.isFundAllocationLineChanged = false;
                if (this.treatment == null || !this.treatment.FUND_ID.HasValue)
                {
                    return;
                }
                CommonParam param = new CommonParam();
                HisFundAllocationViewFilter filter = new HisFundAllocationViewFilter();
                filter.TREATMENT_ID = this.treatment.ID;
                filter.IS_CURRENT = true;
                List<V_HIS_FUND_ALLOCATION> allocations = new BackendAdapter(param).Get<List<V_HIS_FUND_ALLOCATION>>("api/HisFundAllocation/GetView", ApiConsumers.MosConsumer, filter, param);
                this.currentFundAllocation = allocations != null ? allocations.OrderByDescending(o => o.VERSION_NUMBER).FirstOrDefault() : null;
                if (this.IsFundAllocationFinished() && this.currentFundAllocation.IS_COST_CHANGED == FUND_ALLOCATION_IS_COST_CHANGED)
                {
                    this.isFundAllocationLineChanged = this.HasChangedFundLine();
                }
            }
            catch (Exception ex)
            {
                this.currentFundAllocation = null;
                this.isFundAllocationLineChanged = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// A line allocated to the insurer is no longer a cost line, or its amount/price changed after the allocation was saved
        /// (same rule as MOS HisFundAllocationBillCheck). New lines added after finalizing belong to the patient and do not block billing.
        /// </summary>
        private bool HasChangedFundLine()
        {
            CommonParam param = new CommonParam();
            HisFundAllocationDetailSDO detail = new BackendAdapter(param).Get<HisFundAllocationDetailSDO>("api/HisFundAllocation/GetDetail", ApiConsumers.MosConsumer, this.treatment.ID, param);
            if (detail == null || detail.Lines == null)
            {
                return false;
            }
            return detail.Lines.Any(o => (o.ChangeType == FUND_ALLOCATION_LINE_CHANGED && (o.OldFundTotalPrice ?? 0) != 0)
                || (o.ChangeType == FUND_ALLOCATION_LINE_REMOVED && o.FUND_TOTAL_PRICE != 0));
        }

        private bool IsFundAllocationFinished()
        {
            return this.currentFundAllocation != null && this.currentFundAllocation.IS_FINISH == FUND_ALLOCATION_IS_FINISH;
        }

        /// <summary>
        /// Billing is blocked by the fund allocation (not finalized, or insurer lines changed after finalizing)
        /// </summary>
        private bool IsFundAllocationLocked()
        {
            return this.currentFundAllocation != null && (!this.IsFundAllocationFinished() || this.isFundAllocationLineChanged);
        }

        private string GetFundAllocationLockMessage()
        {
            return this.IsFundAllocationFinished() ? Base.ResourceMessageLang.ChiPhiBaoHiemThayDoiSauChot : Base.ResourceMessageLang.DangPhanBoBaoLanhChuaChot;
        }

        /// <summary>
        /// Shows the lock message when the allocation blocks billing. Every save path (buttons, shortcuts) calls it before saving
        /// </summary>
        private bool CheckFundAllocationLocked()
        {
            if (!this.IsFundAllocationLocked())
            {
                return false;
            }
            WaitingManager.Hide();
            DevExpress.XtraEditors.XtraMessageBox.Show(this, this.GetFundAllocationLockMessage(), Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return true;
        }

        /// <summary>
        /// Fund amount already used by the allocation on the treatment's live bills (same rule as MOS HisFundAllocationBillCheck):
        /// per bill, the larger of its HIS_BILL_FUND rows of the allocated fund and TDL_BILL_FUND_AMOUNT when the bill belongs to
        /// the current version (HIS_BILL_FUND rows are not recreated when a bill is uncancelled)
        /// </summary>
        private decimal GetFundAllocationPaidAmount()
        {
            CommonParam param = new CommonParam();
            HisTransactionFilter filter = new HisTransactionFilter();
            filter.TREATMENT_ID = this.treatment.ID;
            filter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;
            filter.IS_CANCEL = false;
            filter.HAS_SALE_TYPE_ID = false;
            List<HIS_TRANSACTION> liveBills = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filter, param);
            if (liveBills == null || liveBills.Count == 0)
            {
                return 0;
            }
            HisBillFundViewFilter fundFilter = new HisBillFundViewFilter();
            fundFilter.TREATMENT_ID = this.treatment.ID;
            fundFilter.FUND_ID = this.currentFundAllocation.FUND_ID;
            List<V_HIS_BILL_FUND> billFunds = new BackendAdapter(param).Get<List<V_HIS_BILL_FUND>>("api/HisBillFund/GetView", ApiConsumers.MosConsumer, fundFilter, param);
            Dictionary<long, decimal> fundByBill = (billFunds ?? new List<V_HIS_BILL_FUND>())
                .GroupBy(o => o.BILL_ID)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.AMOUNT));
            decimal paid = 0;
            foreach (HIS_TRANSACTION bill in liveBills)
            {
                decimal byRows = fundByBill.ContainsKey(bill.ID) ? fundByBill[bill.ID] : 0;
                decimal byTdl = bill.FUND_ALLOCATION_ID == this.currentFundAllocation.ID ? (bill.TDL_BILL_FUND_AMOUNT ?? 0) : 0;
                paid += Math.Max(byRows, byTdl);
            }
            return paid;
        }

        /// <summary>
        /// Loads the current allocation and applies it to the fund grid and the save buttons.
        /// Returns true when the treatment has a finalized allocation: the fund grid then holds the allocation fund line
        /// (allocated insurer amount minus the fund already paid) instead of the default fund lines
        /// </summary>
        private bool ProcessFundAllocation()
        {
            bool result = false;
            try
            {
                this.LoadFundAllocation();
                this.fundAllocationRow = null;
                this.fundAllocationRemain = 0;
                if (this.IsFundAllocationFinished())
                {
                    this.fundAllocationRemain = this.currentFundAllocation.TOTAL_FUND_PRICE - this.GetFundAllocationPaidAmount();
                    if (this.fundAllocationRemain > 0)
                    {
                        HIS_FUND fund = BackendDataWorker.Get<HIS_FUND>().FirstOrDefault(o => o.ID == this.currentFundAllocation.FUND_ID);
                        this.fundAllocationRow = new VHisBillFundADO();
                        this.fundAllocationRow.IsNotEdit = true;
                        this.fundAllocationRow.FUND_ID = this.currentFundAllocation.FUND_ID;
                        this.fundAllocationRow.FUND_CODE = fund != null ? fund.FUND_CODE : this.currentFundAllocation.FUND_CODE;
                        this.fundAllocationRow.FUND_NAME = fund != null ? fund.FUND_NAME : this.currentFundAllocation.FUND_NAME;
                        this.fundAllocationRow.FUND_BUDGET = this.treatment.FUND_BUDGET;
                        this.listBillFundReciept.Add(this.fundAllocationRow);
                        this.RefreshFundAllocationRow();
                    }
                    this.bindingSource1.DataSource = this.listBillFundReciept;
                    this.gridControlFund.DataSource = this.bindingSource1;
                    result = true;
                }
                this.ApplyFundAllocationState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Allocation fund of each book for the selected services: the receipt book takes the fund first (up to its amount),
        /// the rest goes to the invoice book (up to its amount)
        /// </summary>
        private void GetFundAllocationBookAmounts(out decimal recieptFund, out decimal invoiceFund)
        {
            recieptFund = 0;
            invoiceFund = 0;
            if (this.fundAllocationRow == null)
            {
                return;
            }
            decimal recieptPrice = (!checkNotReciept.Checked && listRecieptData != null) ? listRecieptData.Sum(o => o.RecieptPrice ?? 0) : 0;
            decimal invoicePrice = (!checkNotInvoice.Checked && listInvoiceData != null) ? listInvoiceData.Sum(o => o.InvoicePrice ?? 0) : 0;
            recieptFund = Math.Max(0, Math.Min(this.fundAllocationRemain, recieptPrice));
            invoiceFund = Math.Max(0, Math.Min(this.fundAllocationRemain - recieptFund, invoicePrice));
        }

        /// <summary>
        /// Fund line amount follows the selected services (called before computing the amount to collect)
        /// </summary>
        private void RefreshFundAllocationRow()
        {
            try
            {
                if (this.fundAllocationRow == null)
                {
                    return;
                }
                decimal recieptFund, invoiceFund;
                this.GetFundAllocationBookAmounts(out recieptFund, out invoiceFund);
                if (this.fundAllocationRow.AMOUNT != recieptFund + invoiceFund)
                {
                    this.fundAllocationRow.AMOUNT = recieptFund + invoiceFund;
                    this.gridControlFund.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Adds the allocation fund part of the book to the transaction, returns the added amount
        /// </summary>
        private decimal AddFundAllocationBillFund(HIS_TRANSACTION transaction, bool isReciept)
        {
            if (this.fundAllocationRow == null || transaction == null)
            {
                return 0;
            }
            decimal recieptFund, invoiceFund;
            this.GetFundAllocationBookAmounts(out recieptFund, out invoiceFund);
            decimal amount = isReciept ? recieptFund : invoiceFund;
            if (amount <= 0)
            {
                return 0;
            }
            HIS_BILL_FUND billFund = new HIS_BILL_FUND();
            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_BILL_FUND>(billFund, this.fundAllocationRow);
            billFund.AMOUNT = amount;
            if (transaction.HIS_BILL_FUND == null)
            {
                transaction.HIS_BILL_FUND = new List<HIS_BILL_FUND>();
            }
            transaction.HIS_BILL_FUND.Add(billFund);
            return amount;
        }

        private void ApplyFundAllocationState()
        {
            try
            {
                if (this.currentFundAllocation == null)
                {
                    return;
                }
                if (this.IsFundAllocationLocked())
                {
                    //SetEnableButtonSave keeps the save buttons disabled while the allocation blocks billing
                    this.SetEnableButtonSave(false);
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, this.GetFundAllocationLockMessage(), Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.checkIsKC.CheckState = CheckState.Unchecked;
                this.lciIsKC.Enabled = false;
                if (this.checkIsAutoRepay.Enabled && !this.checkIsAutoRepay.Properties.ReadOnly)
                {
                    this.checkIsAutoRepay.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
