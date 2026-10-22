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
using HIS.Desktop.Plugins.FundAllocation.ADO;
using HIS.Desktop.Plugins.FundAllocation.Base;
using HIS.Desktop.Plugins.FundAllocation.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.FundAllocation
{
    public partial class frmFundAllocation
    {
        private void LoadData()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = this.treatmentId;
                List<HIS_TREATMENT> treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT__GET, ApiConsumers.MosConsumer, treatmentFilter, param);
                this.treatment = treatments != null ? treatments.FirstOrDefault() : null;
                this.fund = this.treatment != null && this.treatment.FUND_ID.HasValue ? BackendDataWorker.Get<HIS_FUND>().FirstOrDefault(o => o.ID == this.treatment.FUND_ID.Value) : null;

                HisFundAllocationDetailSDO detail = new BackendAdapter(param).Get<HisFundAllocationDetailSDO>(RequestUriStore.HIS_FUND_ALLOCATION__GET_DETAIL, ApiConsumers.MosConsumer, this.treatmentId, param);
                this.allocation = detail != null ? detail.Allocation : null;
                this.lines = detail != null && detail.Lines != null ? detail.Lines.Select(o => new FundAllocationLineADO(o)).ToList() : new List<FundAllocationLineADO>();
                this.SetLineNotes();

                this.BindHeader();
                this.BindGrids();
                this.LoadHistory();
                this.isDirty = false;
                this.ApplyState();
                WaitingManager.Hide();

                if (this.treatment != null && !this.treatment.FUND_ID.HasValue)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("frmFundAllocation.Message.HoSoChuaCoDonViCungChiTra"), ResourceLanguageManager.Get("frmFundAllocation.Message.ThongBao"));
                }
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetLineNotes()
        {
            foreach (FundAllocationLineADO line in this.lines)
            {
                if (line.ChangeType == FundAllocationLineADO.CHANGE_TYPE__NEW)
                {
                    line.NOTE = ResourceLanguageManager.Get("frmFundAllocation.Note.DongMoi");
                }
                else if (line.ChangeType == FundAllocationLineADO.CHANGE_TYPE__CHANGED)
                {
                    line.NOTE = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Note.DongThayDoi"),
                        AllocationCalculator.FormatNumber(line.OldFundAmount ?? 0), AllocationCalculator.FormatNumber(line.OldFundTotalPrice ?? 0));
                }
                else if (line.ChangeType == FundAllocationLineADO.CHANGE_TYPE__REMOVED)
                {
                    line.NOTE = ResourceLanguageManager.Get("frmFundAllocation.Note.DongKhongCon");
                }
            }
        }

        private void BindHeader()
        {
            try
            {
                this.isBinding = true;
                this.txtTreatmentCode.Text = this.treatment != null ? this.treatment.TREATMENT_CODE : "";
                this.txtPatient.Text = this.treatment != null ? (this.treatment.TDL_PATIENT_CODE + " - " + this.treatment.TDL_PATIENT_NAME) : "";
                this.txtFund.Text = this.fund != null ? (this.fund.FUND_CODE + " - " + this.fund.FUND_NAME) : "";
                this.txtFundNumber.Text = this.treatment != null ? this.treatment.FUND_NUMBER : "";
                string fromTime = this.treatment != null && this.treatment.FUND_FROM_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.treatment.FUND_FROM_TIME.Value) : "";
                string toTime = this.treatment != null && this.treatment.FUND_TO_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.treatment.FUND_TO_TIME.Value) : "";
                this.txtFundTime.Text = (fromTime != "" || toTime != "") ? (fromTime + " - " + toTime) : "";
                this.txtStatus.Text = this.GetStatusText(this.allocation);
                this.txtStatus.ForeColor = this.allocation != null && this.allocation.IS_COST_CHANGED == IS_TRUE ? Color.Red : Color.Black;
                this.txtVersion.Text = this.allocation != null ? this.allocation.VERSION_NUMBER.ToString() : "";
                if (this.treatment != null && this.treatment.FUND_BUDGET.HasValue)
                {
                    this.spinFundBudget.EditValue = this.treatment.FUND_BUDGET.Value;
                }
                else
                {
                    this.spinFundBudget.EditValue = null;
                }
                decimal? ratio = this.allocation != null ? this.allocation.SURCHARGE_RATIO : (this.fund != null ? this.fund.SURCHARGE_RATIO : null);
                if (ratio.HasValue)
                {
                    this.spinSurchargeRatio.EditValue = ratio.Value;
                }
                else
                {
                    this.spinSurchargeRatio.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.isBinding = false;
            }
        }

        /// <summary>
        /// Insurer grid shows lines with an insurer amount, patient grid shows lines with a patient amount;
        /// lines no longer in the treatment's costs are shown at the end of the patient grid
        /// </summary>
        private void BindGrids()
        {
            try
            {
                this.gridControlFund.BeginUpdate();
                this.gridControlPatient.BeginUpdate();
                this.gridControlFund.DataSource = this.lines.Where(o => !o.IsRemoved && o.FUND_TOTAL_PRICE != 0).ToList();
                this.gridControlPatient.DataSource = this.lines.Where(o => o.IsRemoved || o.PATIENT_TOTAL_PRICE != 0).OrderBy(o => o.IsRemoved ? 1 : 0).ToList();
                this.gridControlFund.EndUpdate();
                this.gridControlPatient.EndUpdate();
                this.RefreshTotals();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal? GetBudget()
        {
            return this.spinFundBudget.EditValue != null ? (decimal?)this.spinFundBudget.Value : null;
        }

        private decimal? GetRatio()
        {
            return this.spinSurchargeRatio.EditValue != null ? (decimal?)this.spinSurchargeRatio.Value : null;
        }

        private void RefreshTotals()
        {
            try
            {
                List<FundAllocationLineADO> active = this.lines.Where(o => !o.IsRemoved).ToList();
                decimal totalPrice = active.Sum(o => o.CurrentTotalPrice);
                decimal totalFund = active.Sum(o => o.FUND_TOTAL_PRICE);
                decimal totalPatient = active.Sum(o => o.PATIENT_TOTAL_PRICE);
                decimal? budget = this.GetBudget();
                decimal ratio = this.GetRatio() ?? 0;
                decimal surcharge = Math.Round(totalFund * ratio / 100, 4);
                bool overBudget = budget.HasValue && totalFund > budget.Value;

                this.lblTotalPrice.Text = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Total.TongChiPhi"), AllocationCalculator.FormatNumber(totalPrice));
                this.lblTotalFund.Text = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Total.TongBaoHiemTra"), AllocationCalculator.FormatNumber(totalFund));
                this.lblTotalPatient.Text = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Total.TongBenhNhanTra"), AllocationCalculator.FormatNumber(totalPatient));
                this.lblBudget.Text = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Total.HanMuc"), budget.HasValue ? AllocationCalculator.FormatNumber(budget.Value) : "");
                this.lblBudgetRemain.Text = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Total.HanMucConLai"), budget.HasValue ? AllocationCalculator.FormatNumber(budget.Value - totalFund) : "");
                this.lblSurchargeRatio.Text = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Total.TyLePhuThu"), AllocationCalculator.FormatNumber(ratio));
                this.lblSurchargeAmount.Text = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Total.PhuThu"), AllocationCalculator.FormatNumber(surcharge));
                this.lblTotalFundPay.Text = string.Format(ResourceLanguageManager.Get("frmFundAllocation.Total.TongBaoHiemThanhToan"), AllocationCalculator.FormatNumber(totalFund + surcharge));

                Color warn = overBudget ? Color.Red : Color.Black;
                this.lblTotalFund.Appearance.ForeColor = warn;
                this.lblBudgetRemain.Appearance.ForeColor = warn;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadHistory()
        {
            CommonParam param = new CommonParam();
            try
            {
                HisFundAllocationViewFilter filter = new HisFundAllocationViewFilter();
                filter.TREATMENT_ID = this.treatmentId;
                List<V_HIS_FUND_ALLOCATION> versions = new BackendAdapter(param).Get<List<V_HIS_FUND_ALLOCATION>>(RequestUriStore.HIS_FUND_ALLOCATION__GET_VIEW, ApiConsumers.MosConsumer, filter, param) ?? new List<V_HIS_FUND_ALLOCATION>();

                HisTransactionFilter tranFilter = new HisTransactionFilter();
                tranFilter.TREATMENT_ID = this.treatmentId;
                List<HIS_TRANSACTION> transactions = versions.Count > 0
                    ? (new BackendAdapter(param).Get<List<HIS_TRANSACTION>>(RequestUriStore.HIS_TRANSACTION__GET, ApiConsumers.MosConsumer, tranFilter, param) ?? new List<HIS_TRANSACTION>())
                    : new List<HIS_TRANSACTION>();
                Dictionary<long, List<HIS_TRANSACTION>> dicTran = transactions.Where(o => o.FUND_ALLOCATION_ID.HasValue)
                    .GroupBy(o => o.FUND_ALLOCATION_ID.Value).ToDictionary(g => g.Key, g => g.ToList());
                string cancelled = ResourceLanguageManager.Get("frmFundAllocation.History.GiaoDichDaHuy");

                List<FundAllocationHistoryADO> data = new List<FundAllocationHistoryADO>();
                foreach (V_HIS_FUND_ALLOCATION v in versions.OrderByDescending(o => o.VERSION_NUMBER))
                {
                    FundAllocationHistoryADO ado = new FundAllocationHistoryADO(v);
                    ado.STATUS_DISPLAY = this.GetStatusText(v);
                    List<HIS_TRANSACTION> trans = null;
                    if (dicTran.TryGetValue(v.ID, out trans))
                    {
                        ado.TRANSACTION_CODES = string.Join(", ", trans.OrderBy(o => o.TRANSACTION_CODE)
                            .Select(o => o.TRANSACTION_CODE + (o.IS_CANCEL == IS_TRUE ? " (" + cancelled + ")" : "")));
                    }
                    data.Add(ado);
                }
                this.gridControlHistory.DataSource = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
