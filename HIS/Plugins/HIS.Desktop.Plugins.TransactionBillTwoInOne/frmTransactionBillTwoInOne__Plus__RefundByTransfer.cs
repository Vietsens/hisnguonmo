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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Base;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// After Save success: if user ticked "Hoàn tiền ngân hàng" and a HU (hoàn ứng)
        /// transaction was generated, open the RefundByTransfer plugin form.
        ///
        /// Skip silently when no HU transaction exists or user did not tick.
        /// Show warning + skip when refund config is missing or patient has no beneficiary info.
        /// </summary>
        private void OpenRefundByTransferIfNeeded()
        {
            try
            {
                if (chkRefundByTransfer == null || !chkRefundByTransfer.Checked)
                    return;

                if (this.lastRepayTransactionForRefund == null)
                    return;

                if (HisConfig.RefundConfig == null || HisConfig.RefundConfig.Count == 0)
                {
                    XtraMessageBox.Show(
                        ResourceMessageLang.ChuaCauHinhHoanTienNganHang,
                        ResourceMessageLang.TieuDeCuaSoThongBaoLaCanhBao,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                long? patientId = this.treatment != null ? this.treatment.PATIENT_ID : (long?)null;
                if (!patientId.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Warn("OpenRefundByTransferIfNeeded: PATIENT_ID is null, skip.");
                    return;
                }

                if (!HasPatientBankAccount(patientId.Value))
                {
                    XtraMessageBox.Show(
                        ResourceMessageLang.BNChuaCoThongTinThuHuong,
                        ResourceMessageLang.TieuDeCuaSoThongBaoLaCanhBao,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                long? treatmentId = this.lastRepayTransactionForRefund.TREATMENT_ID
                    ?? (this.treatment != null ? this.treatment.ID : (long?)null);
                if (!treatmentId.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Warn("OpenRefundByTransferIfNeeded: TREATMENT_ID is null, skip.");
                    return;
                }

                HIS_TREATMENT treatmentForRefund = GetTreatment(treatmentId);
                if (treatmentForRefund == null || treatmentForRefund.ID == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("OpenRefundByTransferIfNeeded: GetTreatment returned null/empty, skip.");
                    return;
                }

                string bankCode = HisConfig.RefundConfig.First().KEY
                    .Replace("HIS.Desktop.Plugins.RefundByTransfer.", "")
                    .Replace("Info", "");

                HIS_TRANSACTION transactionForRefund = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(transactionForRefund, this.lastRepayTransactionForRefund);

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentForRefund);
                listArgs.Add(transactionForRefund);
                listArgs.Add(bankCode);
                listArgs.Add((HIS.Desktop.Common.RefeshReference)RefreshAfterRefund);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                    "HIS.Desktop.Plugins.RefundByTransfer",
                    this.currentModule != null ? this.currentModule.RoomId : 0,
                    this.currentModule != null ? this.currentModule.RoomTypeId : 0,
                    listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Callback for RefundByTransfer to refresh session info after refund completes.</summary>
        private void RefreshAfterRefund()
        {
            try
            {
                this.RefreshSessionInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool HasPatientBankAccount(long patientId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientBankAccountFilter filter = new HisPatientBankAccountFilter();
                filter.PATIENT_ID = patientId;
                List<HIS_PATIENT_BANK_ACCOUNT> accs = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT_BANK_ACCOUNT>>(
                        "api/HisPatientBankAccount/Get",
                        ApiConsumers.MosConsumer,
                        filter,
                        param);
                return accs != null && accs.Count > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void chkRefundByTransfer_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                    return;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate =
                    (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                        ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == chkRefundByTransfer.Name && o.MODULE_LINK == currentModule.ModuleLink)
                        : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkRefundByTransfer.Checked ? "1" : "";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkRefundByTransfer.Name;
                    csAddOrUpdate.VALUE = chkRefundByTransfer.Checked ? "1" : "";
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
