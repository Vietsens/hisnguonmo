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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.FundAllocation.ADO;
using HIS.Desktop.Plugins.FundAllocation.Base;
using HIS.Desktop.Plugins.FundAllocation.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.FundAllocation
{
    public partial class frmFundAllocation
    {
        private void btnMoveToFund_Click(object sender, EventArgs e)
        {
            this.MoveSelected(this.gridViewPatient, AllocationSide.Fund);
        }

        private void btnMoveToPatient_Click(object sender, EventArgs e)
        {
            this.MoveSelected(this.gridViewFund, AllocationSide.Patient);
        }

        /// <summary>
        /// Move the selected lines of a grid as a whole to the other side
        /// </summary>
        private void MoveSelected(GridView fromView, AllocationSide toSide)
        {
            try
            {
                if (this.IsFinished())
                {
                    return;
                }
                fromView.CloseEditor();
                int[] handles = fromView.GetSelectedRows();
                if (handles == null || handles.Length == 0)
                {
                    return;
                }
                foreach (int handle in handles)
                {
                    FundAllocationLineADO line = fromView.GetRow(handle) as FundAllocationLineADO;
                    if (line != null && !line.IsRemoved)
                    {
                        AllocationCalculator.MoveAll(line, toSide);
                    }
                }
                this.isDirty = true;
                this.BindGrids();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinFundBudget_EditValueChanged(object sender, EventArgs e)
        {
            if (!this.isBinding)
            {
                this.isDirty = true;
                this.RefreshTotals();
            }
        }

        private void spinSurchargeRatio_EditValueChanged(object sender, EventArgs e)
        {
            if (!this.isBinding)
            {
                this.isDirty = true;
                this.RefreshTotals();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (!this.btnSave.Enabled)
                {
                    return;
                }
                this.gridViewFund.CloseEditor();
                this.gridViewPatient.CloseEditor();
                this.btnSave.Focus();

                HisFundAllocationSaveSDO sdo = new HisFundAllocationSaveSDO();
                sdo.TreatmentId = this.treatmentId;
                sdo.FundAllocationId = this.allocation != null ? (long?)this.allocation.ID : null;
                sdo.ModifyTime = this.allocation != null ? this.allocation.MODIFY_TIME : null;
                sdo.FundBudget = this.GetBudget();
                sdo.SurchargeRatio = this.GetRatio();
                sdo.Lines = this.lines.Where(o => !o.IsRemoved).Select(o => new HisFundAllocationSaveLineSDO
                {
                    SereServId = o.SERE_SERV_ID,
                    Amount = o.CurrentAmount,
                    TotalPrice = o.CurrentTotalPrice,
                    FundAmount = o.FUND_AMOUNT,
                    FundTotalPrice = o.FUND_TOTAL_PRICE
                }).ToList();

                WaitingManager.Show();
                V_HIS_FUND_ALLOCATION result = new BackendAdapter(param).Post<V_HIS_FUND_ALLOCATION>(RequestUriStore.HIS_FUND_ALLOCATION__SAVE, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                success = result != null;
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.AfterChanged();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.allocation == null)
                {
                    return;
                }
                if (this.isDirty)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("frmFundAllocation.Message.LuuTruocKhiChot"), ResourceLanguageManager.Get("frmFundAllocation.Message.ThongBao"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("frmFundAllocation.Message.XacNhanChot"), ResourceLanguageManager.Get("frmFundAllocation.Message.ThongBao"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.CallAction(RequestUriStore.HIS_FUND_ALLOCATION__FINISH, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReopen_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.allocation == null)
                {
                    return;
                }
                this.CallAction(RequestUriStore.HIS_FUND_ALLOCATION__REOPEN, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelAllocation_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.allocation == null)
                {
                    return;
                }
                using (frmCancelReason frm = new frmCancelReason())
                {
                    if (frm.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
                    this.CallAction(RequestUriStore.HIS_FUND_ALLOCATION__CANCEL, frm.Reason);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallAction(string uri, string cancelReason)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HisFundAllocationActionSDO sdo = new HisFundAllocationActionSDO();
                sdo.FundAllocationId = this.allocation.ID;
                sdo.ModifyTime = this.allocation.MODIFY_TIME;
                sdo.CancelReason = cancelReason;

                WaitingManager.Show();
                V_HIS_FUND_ALLOCATION result = new BackendAdapter(param).Post<V_HIS_FUND_ALLOCATION>(uri, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                success = result != null;
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.AfterChanged();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AfterChanged()
        {
            this.LoadData();
            if (this.refreshReference != null)
            {
                this.refreshReference();
            }
        }
    }
}
