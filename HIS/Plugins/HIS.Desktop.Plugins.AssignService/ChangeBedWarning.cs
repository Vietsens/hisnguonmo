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
using MOS.SDO;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService
{
    /// <summary>
    /// Viec 63227: hoi xac nhan doi giuong khi bat khoa MOS.HIS_SERVICE_REQ.DEFAULT_BED_BY_LAST_ASSIGNED (1 hoac 2).
    ///
    /// Chon giuong khac giuong duoc chi dinh gan nhat thi Backend giu lai y lenh va tra ma
    /// HisServiceReq_XacNhanChuyenGiuong ("Nguoi benh dang nam giuong {0}, xac nhan chuyen sang giuong {1}?").
    /// Man hinh hoi lai nguoi dung, dong y thi gui lai dung y lenh do kem IsConfirmedChangeBed = 1 cho cac dong
    /// dich vu giuong, Backend ghi nhat ky nguoi xac nhan chuyen giuong.
    /// Lam theo cach xu ly canh bao ho so da co hoa don (QT16, PayBeforeFinishWarning).
    /// </summary>
    internal static class ChangeBedWarning
    {
        private const string MESSAGE_CODE__CHANGE_BED = "HisServiceReq_XacNhanChuyenGiuong";
        private const string TITLE__THONG_BAO = "Thông báo";
        private const string MESSAGE__NOT_CONFIRMED = "Chưa lưu do không đồng ý chuyển giường, vui lòng chọn lại giường.";

        /// <summary>
        /// Y lenh vua roi co bi giu lai vi doi giuong chua xac nhan hay khong.
        /// </summary>
        internal static bool IsNeedConfirm(CommonParam param)
        {
            try
            {
                return param != null && param.MessageCodes != null && param.MessageCodes.Contains(MESSAGE_CODE__CHANGE_BED);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Hoi nguoi dung co chuyen giuong hay khong. Tra ve true la dong y gui lai.
        /// Du tra loi the nao cung xoa cau canh bao khoi param de man hinh khong bao lai lan nua.
        /// Khong dong y thi de lai cau bao chua luu.
        /// </summary>
        /// <param name="param">Ket qua goi api</param>
        /// <param name="prefix">Ten nguoi benh, dung khi chi dinh cho nhieu nguoi benh cung luc (co the rong)</param>
        internal static bool Confirm(CommonParam param, string prefix)
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
                    (prefix ?? "") + message,
                    TITLE__THONG_BAO,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes;

                ClearWarning(param);

                if (!agreed && param.Messages != null)
                {
                    param.Messages.Add(MESSAGE__NOT_CONFIRMED);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return agreed;
        }

        /// <summary>
        /// Danh dau da xac nhan doi giuong cho cac dong dich vu giuong truoc khi gui lai.
        /// </summary>
        internal static void SetConfirmed(AssignServiceSDO sdo)
        {
            try
            {
                if (sdo == null || sdo.ServiceReqDetails == null)
                {
                    return;
                }

                foreach (var detail in sdo.ServiceReqDetails)
                {
                    if (detail != null && detail.BedId.HasValue)
                    {
                        detail.IsConfirmedChangeBed = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
