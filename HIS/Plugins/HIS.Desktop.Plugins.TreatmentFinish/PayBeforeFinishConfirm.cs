using Inventec.Core;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    /// <summary>
    /// Canh bao chua hoan tat vien phi khi ket thuc dieu tri.
    ///
    /// Chi chay khi khoa MOS.HIS_TREATMENT.PAY_BEFORE_FINISH_OPTION dat muc canh bao
    /// (gia tri 2). O muc chan han (gia tri 1) Backend tra ma thong bao khac nen
    /// nhanh duoi day khong chay, thao tac bi chan nhu cu.
    ///
    /// Xem PTTK_TBD_Thanh_Toan_Vien_Phi_Truoc_Ket_Thuc_Dieu_Tri.
    /// </summary>
    internal static class PayBeforeFinishConfirm
    {
        private const string MESSAGE_CODE__UNPAID_FEE = "HisTreatment_ChuaHoanTatVienPhi___XacNhanKetThuc";
        private const string TITLE__THONG_BAO = "Thông báo";

        /// <summary>
        /// Lan luu vua roi co bi giu lai vi ho so chua hoan tat vien phi hay khong.
        /// </summary>
        internal static bool IsNeedConfirm(CommonParam param)
        {
            try
            {
                return param != null && param.MessageCodes != null
                    && param.MessageCodes.Contains(MESSAGE_CODE__UNPAID_FEE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Hoi nguoi dung co ket thuc tiep hay khong. Tra ve true la dong y gui lai.
        /// Du tra loi the nao cung xoa cau canh bao khoi param de khong bao lai lan nua.
        /// </summary>
        internal static bool Confirm(CommonParam param)
        {
            bool agreed = false;
            try
            {
                string message = param != null ? param.GetMessage() : null;
                if (string.IsNullOrWhiteSpace(message))
                {
                    return false;
                }

                agreed = DevExpress.XtraEditors.XtraMessageBox.Show(
                    message,
                    TITLE__THONG_BAO,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes;

                ClearWarning(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return agreed;
        }

        private static void ClearWarning(CommonParam param)
        {
            try
            {
                if (param == null)
                {
                    return;
                }

                if (param.Messages != null)
                {
                    param.Messages.Clear();
                }

                if (param.MessageCodes != null)
                {
                    param.MessageCodes.Clear();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
