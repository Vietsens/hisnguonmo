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
using HIS.Desktop.Plugins.TransactionBill.ADO;
using HIS.Desktop.Plugins.TransactionBill.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBill
{
    /// <summary>
    /// Fund allocation (commercial guarantee insurance) of the treatment:
    /// - allocation saved but not finalized: billing is not allowed
    /// - allocation finalized but a line allocated to the insurer changed its cost afterwards: billing is not allowed
    /// - allocation finalized: the fund line takes the allocated insurer amount, deposits are not offset
    ///   and the whole remaining deposit is refunded in the same bill (auto repay)
    /// </summary>
    public partial class frmTransactionBill
    {
        private const short FUND_ALLOCATION_IS_FINISH = 1;
        private const short FUND_ALLOCATION_IS_COST_CHANGED = 1;
        private const short FUND_ALLOCATION_LINE_CHANGED = 2;
        private const short FUND_ALLOCATION_LINE_REMOVED = 3;

        private V_HIS_FUND_ALLOCATION currentFundAllocation = null;
        private bool isFundAllocationLineChanged = false;

        private void LoadFundAllocation()
        {
            try
            {
                this.currentFundAllocation = null;
                this.isFundAllocationLineChanged = false;
                if (this.currentTreatment == null || !this.currentTreatment.FUND_ID.HasValue)
                {
                    return;
                }
                CommonParam param = new CommonParam();
                HisFundAllocationViewFilter filter = new HisFundAllocationViewFilter();
                filter.TREATMENT_ID = this.currentTreatment.ID;
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
            HisFundAllocationDetailSDO detail = new BackendAdapter(param).Get<HisFundAllocationDetailSDO>("api/HisFundAllocation/GetDetail", ApiConsumers.MosConsumer, this.currentTreatment.ID, param);
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
            return this.IsFundAllocationFinished() ? ResourceMessageLang.ChiPhiBaoHiemThayDoiSauChot : ResourceMessageLang.DangPhanBoBaoLanhChuaChot;
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
            filter.TREATMENT_ID = this.currentTreatment.ID;
            filter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;
            filter.IS_CANCEL = false;
            filter.HAS_SALE_TYPE_ID = false;
            List<HIS_TRANSACTION> liveBills = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filter, param);
            if (liveBills == null || liveBills.Count == 0)
            {
                return 0;
            }
            Dictionary<long, decimal> fundByBill = (this.ListBillFundPay ?? new List<V_HIS_BILL_FUND>())
                .Where(o => o.FUND_ID == this.currentFundAllocation.FUND_ID)
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
        /// Fund line = allocated insurer amount minus the fund amount already paid on the treatment's bills,
        /// not greater than the patient price of the selected services
        /// </summary>
        private void ProcessFundAllocationFund()
        {
            try
            {
                decimal paid = this.GetFundAllocationPaidAmount();
                decimal amount = Math.Min(this.currentFundAllocation.TOTAL_FUND_PRICE - paid, this.totalPatientPrice);
                if (amount > 0)
                {
                    HIS_FUND fund = BackendDataWorker.Get<HIS_FUND>().FirstOrDefault(o => o.ID == this.currentFundAllocation.FUND_ID);
                    VHisBillFundADO ado = new VHisBillFundADO();
                    ado.IsNotEdit = true;
                    ado.FUND_ID = this.currentFundAllocation.FUND_ID;
                    ado.FUND_CODE = fund != null ? fund.FUND_CODE : this.currentFundAllocation.FUND_CODE;
                    ado.FUND_NAME = fund != null ? fund.FUND_NAME : this.currentFundAllocation.FUND_NAME;
                    ado.FUND_BUDGET = this.currentTreatment.FUND_BUDGET;
                    ado.AMOUNT = amount;
                    this.ListBillFund.Add(ado);
                }
                this.bindingSource1.DataSource = this.ListBillFund;
                this.gridControlBillFund.DataSource = this.bindingSource1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, this.GetFundAllocationLockMessage(), ResourceMessageLang.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.chkCoKetChuyen.CheckState = CheckState.Unchecked;
                this.lciCoKetChuyen.Enabled = false;
                if (this.chkAutoRepay.Enabled && !this.chkAutoRepay.ReadOnly)
                {
                    this.chkAutoRepay.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
