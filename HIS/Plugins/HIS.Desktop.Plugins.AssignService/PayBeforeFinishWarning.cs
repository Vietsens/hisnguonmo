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
using Inventec.Core;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService
{
    /// <summary>
    /// Canh bao ke them cho ho so da co hoa don thanh toan (QT16).
    ///
    /// Backend GIU LAI thao tac va tra ve ma canh bao HisTreatment_DaCoHoaDon___XacNhanKeThem.
    /// Man hinh hoi lai nguoi dung, dong y thi gui lai dung thao tac do kem co xac nhan.
    /// Khong lien quan den vien chua bat khoa MOS.HIS_TREATMENT.PAY_BEFORE_FINISH_OPTION:
    /// Backend khong bao gio tra ma canh bao nay nen nhanh duoi day khong chay.
    ///
    /// Xem PTTK_TBD_Thanh_Toan_Vien_Phi_Truoc_Ket_Thuc_Dieu_Tri.
    /// </summary>
    internal static class PayBeforeFinishWarning
    {
        private const string MESSAGE_CODE__ADD_AFTER_BILL = "HisTreatment_DaCoHoaDon___XacNhanKeThem";
        private const string TITLE__THONG_BAO = "Thông báo";

        /// <summary>
        /// Thao tac vua roi co bi giu lai vi ho so da co hoa don hay khong.
        /// </summary>
        internal static bool IsNeedConfirm(CommonParam param)
        {
            try
            {
                return param != null && param.MessageCodes != null && param.MessageCodes.Contains(MESSAGE_CODE__ADD_AFTER_BILL);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Hoi nguoi dung co ke them hay khong. Tra ve true la dong y gui lai.
        /// Du tra loi the nao cung xoa cau canh bao khoi param de man hinh khong bao lai lan nua.
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
