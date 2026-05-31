/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using System;

namespace HIS.Desktop.Plugins.HisPatientPackage.Resources
{
    /// <summary>
    /// Thông báo riêng của module Gói dịch vụ bệnh nhân.
    /// Mỗi câu 1 property, có try-catch, trả "" khi lỗi.
    /// </summary>
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.HisPatientPackage.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        private static string Get(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Bạn có chắc chắn muốn xóa gói dịch vụ này không? Toàn bộ chi tiết của gói cũng sẽ bị xóa.</summary>
        internal static string BanCoMuonXoaGoiKhong { get { return Get("BanCoMuonXoaGoiKhong"); } }

        /// <summary>Bạn có chắc chắn muốn mở khóa gói dịch vụ này không?</summary>
        internal static string BanCoMuonMoKhoaGoiKhong { get { return Get("BanCoMuonMoKhoaGoiKhong"); } }

        /// <summary>Vui lòng nhập lý do khóa gói.</summary>
        internal static string VuiLongNhapLyDoKhoa { get { return Get("VuiLongNhapLyDoKhoa"); } }

        /// <summary>Khóa gói</summary>
        internal static string TieuDeKhoaGoi { get { return Get("TieuDeKhoaGoi"); } }

        /// <summary>Lý do khóa</summary>
        internal static string LyDoKhoa { get { return Get("LyDoKhoa"); } }

        /// <summary>Vui lòng chọn một gói trong danh sách.</summary>
        internal static string VuiLongChonGoi { get { return Get("VuiLongChonGoi"); } }

        /// <summary>Không lấy được dữ liệu gói để in. Vui lòng thử lại.</summary>
        internal static string KhongLayDuocDuLieuGoiDeIn { get { return Get("KhongLayDuocDuLieuGoiDeIn"); } }

        /// <summary>Hành động không khả dụng với trạng thái hiện tại của gói.</summary>
        internal static string HanhDongKhongKhaDungVoiTrangThai { get { return Get("HanhDongKhongKhaDungVoiTrangThai"); } }

        /// <summary>Đồng ý (label nút OK).</summary>
        internal static string DongY { get { return Get("DongY"); } }

        /// <summary>Hủy (label nút Cancel).</summary>
        internal static string Huy { get { return Get("Huy"); } }
    }
}
