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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.ADO;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.Base;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.Config;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.Popup;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile
{
    /// <summary>
    /// v57853 - Public entry of the "attach prescription to sale export ticket" feature.
    /// Used by ExpMestSaleCreate (attach before/after saving) and HisExportMestMedicine (mark + view on the list).
    /// Documents are stored on EMR (EMR_DOCUMENT type EXPSA), one merged PDF per attach action.
    /// </summary>
    public class ExpMestAttachFileProcessor
    {
        /// <summary>
        /// Feature ON: hospital connected to EMR (MOS.HAS_CONNECTION_EMR = 1)
        /// and HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable = 1
        /// </summary>
        public static bool IsEnable()
        {
            return AttachFileConfig.IsEnable;
        }

        /// <summary>
        /// Requirement #6: finished (DONE) / paid (BILL_ID) / debt-confirmed (DEBT_ID) tickets keep the evidence -> no delete.
        /// A ticket in a closed stock period is always DONE, so it is covered by the DONE condition.
        /// </summary>
        public static bool IsAllowDelete(ExpMestAttachInfoADO expMest)
        {
            if (expMest == null) return false;
            return expMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                && !expMest.BILL_ID.HasValue
                && !expMest.DEBT_ID.HasValue;
        }

        #region Texts for caller screens

        public static string GetButtonCaption(int pendingFileCount)
        {
            if (pendingFileCount > 0)
                return String.Format(Resources.ResourceLanguageManager.GetValue("Processor.ButtonCaptionWithCount", "Đính kèm đơn ({0})"), pendingFileCount);
            return Resources.ResourceLanguageManager.GetValue("Processor.ButtonCaption", "Đính kèm đơn");
        }

        public static string GetButtonToolTip()
        {
            return Resources.ResourceLanguageManager.GetValue("Processor.ButtonToolTip", "Đính kèm tệp hoặc chụp ảnh đơn thuốc người bệnh mang đến");
        }

        public static string GetGridColumnCaption()
        {
            return Resources.ResourceLanguageManager.GetValue("Processor.GridColumnCaption", "Đơn đính kèm");
        }

        public static string GetGridToolTip(bool hasAttach)
        {
            return hasAttach
                ? Resources.ResourceLanguageManager.GetValue("Processor.GridToolTipHasAttach", "Đã có đơn thuốc đính kèm — bấm để xem")
                : Resources.ResourceLanguageManager.GetValue("Processor.GridToolTipNoAttach", "Chưa có đơn thuốc đính kèm — bấm để đính kèm");
        }

        #endregion

        #region Pending files (ticket not saved yet)

        /// <summary>
        /// Open the attach form in "pending" mode: files stay in memory, nothing is sent to EMR.
        /// Returns the edited pending data, or null when the user closes without confirming.
        /// </summary>
        public static PendingAttachADO ChooseFiles(PendingAttachADO current)
        {
            PendingAttachADO result = null;
            try
            {
                using (frmExpMestAttachFile frm = new frmExpMestAttachFile(current))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                        result = frm.PendingResult;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Upload the pending files to every saved ticket (one merged PDF per ticket).
        /// Shows a warning listing the tickets that failed. Returns true when all tickets succeeded.
        /// </summary>
        public static bool AttachPendingFiles(List<ExpMestAttachInfoADO> expMests, PendingAttachADO pending)
        {
            bool result = false;
            try
            {
                if (pending == null || pending.Files == null || pending.Files.Count == 0 || expMests == null || expMests.Count == 0)
                    return false;

                List<string> failCodes = new List<string>();
                WaitingManager.Show();
                byte[] pdf = PdfMergeUtil.MergeToPdf(pending.Files);
                foreach (var expMest in expMests)
                {
                    CommonParam param = new CommonParam();
                    var created = pdf != null ? AttachDocumentWorker.CreateDocument(expMest, pdf, pending.DocumentName, param) : null;
                    if (created == null)
                        failCodes.Add(expMest.EXP_MEST_CODE);
                    else
                        WriteAuditLog("AttachPrescription", expMest.EXP_MEST_CODE, pending.Files.Count);
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                }
                WaitingManager.Hide();

                result = failCodes.Count == 0;
                if (!result)
                {
                    XtraMessageBox.Show(
                        String.Format(Resources.ResourceMessage.DinhKemDonThatBai, String.Join(", ", failCodes)),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>True when there is nothing pending or the user agrees to discard the pending files</summary>
        public static bool ConfirmDiscard(PendingAttachADO pending)
        {
            try
            {
                if (pending == null || pending.Files == null || pending.Files.Count == 0)
                    return true;
                return XtraMessageBox.Show(
                    String.Format(Resources.ResourceMessage.CoDonChuaLuuBanCoMuonBo, pending.Files.Count),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }

        /// <summary>Dispose the in-memory images of the pending files</summary>
        public static void Release(PendingAttachADO pending)
        {
            try
            {
                if (pending == null || pending.Files == null) return;
                foreach (var file in pending.Files)
                {
                    if (file.Image != null)
                    {
                        file.Image.Dispose();
                        file.Image = null;
                    }
                }
                pending.Files.Clear();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Sale for several patients in one save: pending files are not attached automatically</summary>
        public static void ShowMultiPatientWarning()
        {
            try
            {
                XtraMessageBox.Show(
                    Resources.ResourceMessage.BanNhieuBenhNhanKhongDinhKem,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Saved ticket

        /// <summary>
        /// Open the list of attached prescriptions of a saved ticket (view/print, add, delete by status).
        /// actChanged is called after closing when something was added/deleted (to refresh the caller).
        /// </summary>
        public static void ShowAttachList(ExpMestAttachInfoADO expMest, long roomId, Action actChanged)
        {
            try
            {
                if (expMest == null || String.IsNullOrEmpty(expMest.EXP_MEST_CODE))
                    return;
                using (frmExpMestAttachList frm = new frmExpMestAttachList(expMest, roomId))
                {
                    frm.ShowDialog();
                    if (frm.HasChange && actChanged != null)
                        actChanged();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>EXP_MEST_CODEs having at least one attached prescription — one API call for the whole list</summary>
        public static HashSet<string> GetExpMestCodesHasAttach(List<string> expMestCodes)
        {
            return AttachDocumentWorker.GetExpMestCodesHasAttach(expMestCodes);
        }

        #endregion

        /// <summary>Audit trail: AppCode____Version____Seconds____Module____Action____User____IP____Customer</summary>
        internal static void WriteAuditLog(string action, string expMestCode, object detail)
        {
            try
            {
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}",
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_CODE,
                    HIS.Desktop.Utility.GlobalString.VersionApp,
                    0,
                    "HIS.Desktop.Plugins.Library.ExpMestAttachFile",
                    action + ":EXP_MEST_CODE=" + expMestCode + ":" + detail,
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                    HIS.Desktop.Utility.StringUtil.GetIpLocal(),
                    HIS.Desktop.Utility.StringUtil.CustomerCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
