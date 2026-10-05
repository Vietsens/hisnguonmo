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

namespace HIS.Desktop.Utilities.RemoteSupport
{
    /// <summary>
    /// vCong57682 - Duong dan dich vu cua chuc nang Yeu cau ho tro noi bo.
    /// Gom ve mot cho de khi phia may chu dat ten khac thi chi sua duy nhat file nay.
    /// Dung chung cho man Tao yeu cau (Ctrl+F2) va plugin Quan ly yeu cau ho tro.
    /// </summary>
    public static class ApiUrl
    {
        public const string HIS_SUPPORT_REQUEST__CREATE = "api/HisSupportRequest/Create";
        public const string HIS_SUPPORT_REQUEST__UPDATE = "api/HisSupportRequest/Update";
        public const string HIS_SUPPORT_REQUEST__DELETE = "api/HisSupportRequest/Delete";
        public const string HIS_SUPPORT_REQUEST__GET_VIEW = "api/HisSupportRequest/GetView";
        public const string HIS_SUPPORT_REQUEST__RECEIVE = "api/HisSupportRequest/Receive";
        public const string HIS_SUPPORT_REQUEST__FINISH = "api/HisSupportRequest/Finish";
        public const string HIS_SUPPORT_REQUEST__FORWARD = "api/HisSupportRequest/Forward";
        public const string HIS_SUPPORT_REQUEST__TRANSFER = "api/HisSupportRequest/Transfer";

        public const string HIS_SUPPORT_REQUEST_CMT__CREATE = "api/HisSupportRequestCmt/Create";
        public const string HIS_SUPPORT_REQUEST_CMT__UPDATE = "api/HisSupportRequestCmt/Update";
        public const string HIS_SUPPORT_REQUEST_CMT__DELETE = "api/HisSupportRequestCmt/Delete";
        public const string HIS_SUPPORT_REQUEST_CMT__GET_VIEW = "api/HisSupportRequestCmt/GetView";

        public const string HIS_SUPPORT_REQUEST_FILE__GET = "api/HisSupportRequestFile/Get";
    }

    /// <summary>
    /// vCong57682 - Gia tri cot SUPPORT_REQUEST_STT. Dong bo voi PTTK_57682 muc B.2.2.
    /// </summary>
    public static class SupportRequestStt
    {
        public const short MOI_TAO = 1;
        public const short DANG_XU_LY = 2;
        public const short HOAN_THANH = 3;
        public const short TU_CHOI = 4;
        public const short DA_CHUYEN_CTY = 5;

        public static string GetName(short? stt)
        {
            if (stt == MOI_TAO) return "Mới tạo";
            if (stt == DANG_XU_LY) return "Đang xử lý";
            if (stt == HOAN_THANH) return "Hoàn thành";
            if (stt == TU_CHOI) return "Từ chối";
            if (stt == DA_CHUYEN_CTY) return "Đã chuyển công ty";
            return "";
        }

        /// <summary>Trang thai ket thuc thi khong con thao tac xu ly nao.</summary>
        public static bool IsClosed(short? stt)
        {
            return stt == HOAN_THANH || stt == TU_CHOI || stt == DA_CHUYEN_CTY;
        }
    }

    /// <summary>
    /// vCong57682 - Phan loai tep dinh kem cua yeu cau ho tro.
    /// </summary>
    public static class SupportRequestFileKind
    {
        public const short USER_ATTACH = 1;
        public const short SCREEN_CAPTURE = 2;
        public const short LOG_SYSTEM = 3;
    }
}
