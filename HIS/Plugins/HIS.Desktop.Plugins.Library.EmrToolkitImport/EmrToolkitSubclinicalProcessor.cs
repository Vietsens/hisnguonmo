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
using System.Windows.Forms;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Config;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models.LabResult;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Popup;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Service;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport
{
    /// <summary>
    /// Public entry point for reading subclinical results another facility shared on the
    /// EMRToolkit gateway. A calling plugin references this dll and uses the methods below.
    ///
    /// Pushing results of this hospital is NOT here — a background job in MOS owns it.
    ///
    /// <code>
    /// if (!EmrToolkitSubclinicalProcessor.IsViewEnable(EmrToolkitSubclinicalType.TEST)) return;
    ///
    /// var records = new EmrToolkitSubclinicalProcessor().CheckValidity(cccd, out message);
    /// var pdf = new EmrToolkitSubclinicalProcessor().RequestViewAndDownloadPdf(cccd, maPhieuXN, null, this);
    /// </code>
    /// </summary>
    public class EmrToolkitSubclinicalProcessor
    {
        /// <summary>
        /// Whether looking up this subclinical type is usable: the gateway connection is
        /// configured and the type is listed in the configuration. Use it to show or hide
        /// the button and the tab on the calling plugin.
        /// </summary>
        public static bool IsViewEnable(EmrToolkitSubclinicalType type)
        {
            try
            {
                return EmrToolkitSubclinicalConfigCFG.IsViewEnable(type);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Lists the records of a patient still valid on the gateway. No UI.
        /// An empty list with an empty message means there is simply nothing shared.
        /// </summary>
        /// <param name="soDinhDanhBenhNhan">Patient identification number</param>
        /// <param name="message">Failure reason, empty when the call succeeded</param>
        /// <returns>Records on the gateway, never null</returns>
        public List<LabResultValidityADO> CheckValidity(string soDinhDanhBenhNhan, out string message)
        {
            try
            {
                return new EmrToolkitSubclinicalApiService().CheckValidity(soDinhDanhBenhNhan, out message);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                message = ex.Message;
                return new List<LabResultValidityADO>();
            }
        }

        /// <summary>
        /// Full read flow for one record: asks for an OTP, collects it from the user,
        /// then downloads the result form(s) as pdf.
        ///
        /// The gateway consumes an OTP session on the first successful use, so one OTP
        /// yields one download; viewing again requires a new OTP.
        /// </summary>
        /// <param name="soDinhDanhBenhNhan">Patient identification number</param>
        /// <param name="maPhieuXN">Lab request code to view, null for every valid record</param>
        /// <param name="maDungChung">Single test index code to filter by, may be null</param>
        /// <param name="owner">Parent window for the OTP dialog, may be null</param>
        /// <returns>Local pdf paths, never null. Cancelled by user gives Success = false with no message.</returns>
        public LabResultPdfResultADO RequestViewAndDownloadPdf(string soDinhDanhBenhNhan,
            string maPhieuXN, string maDungChung, IWin32Window owner = null)
        {
            LabResultPdfResultADO result = new LabResultPdfResultADO();
            try
            {
                EmrToolkitSubclinicalApiService service = new EmrToolkitSubclinicalApiService();

                RequestViewRequestADO request = new RequestViewRequestADO();
                request.SoDinhDanhBenhNhan = soDinhDanhBenhNhan;
                request.MaPhieuXN = maPhieuXN;
                request.MaDungChung = maDungChung;

                string message;
                RequestViewResultADO transaction = service.RequestView(request, out message);
                if (transaction == null)
                {
                    result.Success = false;
                    result.Message = message;
                    return result;
                }

                string otp = AskOtp(transaction, owner);
                if (string.IsNullOrWhiteSpace(otp))
                {
                    // User cancelled — no message, the caller shows nothing
                    result.Success = false;
                    return result;
                }

                DownloadPdfRequestADO downloadRequest = new DownloadPdfRequestADO();
                downloadRequest.TransactionId = transaction.TransactionId;
                downloadRequest.OTP = otp;

                return service.DownloadPdf(downloadRequest);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
        }

        /// <summary>Opens the OTP dialog and returns what the user typed, empty when cancelled.</summary>
        private string AskOtp(RequestViewResultADO transaction, IWin32Window owner)
        {
            try
            {
                using (frmEmrToolkitOtp frm = new frmEmrToolkitOtp(transaction))
                {
                    DialogResult dialogResult = owner != null ? frm.ShowDialog(owner) : frm.ShowDialog();
                    return dialogResult == DialogResult.OK ? frm.OtpValue : "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return "";
            }
        }
    }
}
