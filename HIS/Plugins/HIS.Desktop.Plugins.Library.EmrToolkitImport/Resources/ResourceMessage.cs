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

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Resources
{
    public class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.Library.EmrToolkitImport.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        private static string GetValue(string key)
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

        /// <summary>Gửi dữ liệu qua EMRTOOLKIT thành công.</summary>
        internal static string GuiDuLieuThanhCong { get { return GetValue("GuiDuLieuThanhCong"); } }

        /// <summary>Gửi dữ liệu qua EMRTOOLKIT thất bại.</summary>
        internal static string GuiDuLieuThatBai { get { return GetValue("GuiDuLieuThatBai"); } }

        /// <summary>Đã sao chép JSON vào clipboard.</summary>
        internal static string DaSaoChepJson { get { return GetValue("DaSaoChepJson"); } }

        #region Liên thông kết quả xét nghiệm (LabResult)

        /// <summary>Chưa cấu hình thông tin kết nối cổng EMRToolkit.</summary>
        internal static string ChuaCauHinhKetNoiEmrToolkit { get { return GetValue("ChuaCauHinhKetNoiEmrToolkit"); } }

        /// <summary>Chức năng liên thông KQXN chưa được bật.</summary>
        internal static string ChuaBatChucNangLienThongKqxn { get { return GetValue("ChuaBatChucNangLienThongKqxn"); } }

        /// <summary>Không đăng nhập được cổng EMRToolkit.</summary>
        internal static string KhongLayDuocTokenEmrToolkit { get { return GetValue("KhongLayDuocTokenEmrToolkit"); } }

        /// <summary>Mã hóa dữ liệu trên cổng thất bại.</summary>
        internal static string MaHoaDuLieuThatBai { get { return GetValue("MaHoaDuLieuThatBai"); } }

        /// <summary>Gửi kết quả xét nghiệm lên cổng thất bại.</summary>
        internal static string GuiKetQuaLenCongThatBai { get { return GetValue("GuiKetQuaLenCongThatBai"); } }

        /// <summary>Không có dữ liệu để gửi lên cổng.</summary>
        internal static string DuLieuGuiDiRong { get { return GetValue("DuLieuGuiDiRong"); } }

        /// <summary>Bệnh nhân chưa có số định danh.</summary>
        internal static string ThieuSoDinhDanhBenhNhan { get { return GetValue("ThieuSoDinhDanhBenhNhan"); } }

        /// <summary>Kiểm tra kết quả còn hiệu lực thất bại.</summary>
        internal static string KiemTraHieuLucThatBai { get { return GetValue("KiemTraHieuLucThatBai"); } }

        /// <summary>Xin mã OTP thất bại.</summary>
        internal static string XinMaOtpThatBai { get { return GetValue("XinMaOtpThatBai"); } }

        /// <summary>Tải phiếu kết quả từ cổng thất bại.</summary>
        internal static string TaiPhieuKetQuaThatBai { get { return GetValue("TaiPhieuKetQuaThatBai"); } }

        /// <summary>Thiếu mã phiên xác thực hoặc mã OTP.</summary>
        internal static string ThieuTransactionIdHoacOtp { get { return GetValue("ThieuTransactionIdHoacOtp"); } }

        /// <summary>Vui lòng nhập mã OTP.</summary>
        internal static string ThieuMaOtp { get { return GetValue("ThieuMaOtp"); } }

        /// <summary>Mã OTP đã hết hạn.</summary>
        internal static string MaOtpDaHetHan { get { return GetValue("MaOtpDaHetHan"); } }

        /// <summary>Thành công (trạng thái dòng đồng bộ).</summary>
        internal static string DongBoThanhCong { get { return GetValue("DongBoThanhCong"); } }

        /// <summary>Thất bại (trạng thái dòng đồng bộ).</summary>
        internal static string DongBoThatBai { get { return GetValue("DongBoThatBai"); } }

        /// <summary>Đồng bộ thành công {0}/{1} dịch vụ xét nghiệm.</summary>
        internal static string KetQuaDongBoTongHop { get { return GetValue("KetQuaDongBoTongHop"); } }

        /// <summary>Bệnh nhân không còn kết quả CLS hiệu lực trên cổng.</summary>
        public static string KhongCoKetQuaConHieuLuc { get { return GetValue("KhongCoKetQuaConHieuLuc"); } }

        /// <summary>Vui lòng chọn bản ghi kết quả cần xem.</summary>
        public static string ChuaChonBanGhiDeXem { get { return GetValue("ChuaChonBanGhiDeXem"); } }

        /// <summary>Không có dịch vụ xét nghiệm nào đủ điều kiện đồng bộ.</summary>
        public static string ChuaCoKetQuaDeDongBo { get { return GetValue("ChuaCoKetQuaDeDongBo"); } }

        /// <summary>Đồng bộ {0} dịch vụ xét nghiệm lên cổng EMRToolkit?</summary>
        public static string XacNhanDongBoKqxn { get { return GetValue("XacNhanDongBoKqxn"); } }

        /// <summary>Phiếu này đã có kết quả trên cổng, đồng bộ lại sẽ tạo bản ghi mới.</summary>
        public static string PhieuDaDongBoTruocDo { get { return GetValue("PhieuDaDongBoTruocDo"); } }

        /// <summary>Lần khám này chưa có chỉ định xét nghiệm.</summary>
        public static string KhongCoChiDinhXetNghiem { get { return GetValue("KhongCoChiDinhXetNghiem"); } }

        /// <summary>Chưa cấu hình kết nối — dùng cho plugin gọi (public).</summary>
        public static string ChuaCauHinhKetNoi { get { return GetValue("ChuaCauHinhKetNoiEmrToolkit"); } }

        #endregion
    }
}
