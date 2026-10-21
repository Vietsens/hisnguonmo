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
using System.Resources;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Resources
{
    /// <summary>
    /// Plugin messages (Resources/Message.Lang.vi|en.resx). Format placeholders are documented in the resx values.
    /// </summary>
    class ResourceMessage
    {
        static ResourceManager languageMessage = new ResourceManager(
            "HIS.Desktop.Plugins.TestConnectDeviceSample.Resources.Message.Lang",
            System.Reflection.Assembly.GetExecutingAssembly());

        private static string Get(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Cổng {0} đang được chương trình khác sử dụng.</summary>
        internal static string CongDangDuocSuDung { get { return Get("CongDangDuocSuDung"); } }
        /// <summary>Không mở được cổng {0}: máy quét chưa cắm hoặc cổng không tồn tại.</summary>
        internal static string KhongMoDuocCong { get { return Get("KhongMoDuocCong"); } }
        /// <summary>Không mở được cổng {0}.</summary>
        internal static string KhongMoDuocCongLoiKhac { get { return Get("KhongMoDuocCongLoiKhac"); } }
        /// <summary>Cổng {0} đã kết nối.</summary>
        internal static string CongDaKetNoi { get { return Get("CongDaKetNoi"); } }
        /// <summary>Đã kết nối {0}. Quét thẻ nhân viên để đăng nhập.</summary>
        internal static string DaKetNoiCong { get { return Get("DaKetNoiCong"); } }
        /// <summary>Đã ngắt kết nối {0}.</summary>
        internal static string DaNgatKetNoiCong { get { return Get("DaNgatKetNoiCong"); } }
        /// <summary>Cổng {0} chưa kết nối.</summary>
        internal static string CongChuaKetNoi { get { return Get("CongChuaKetNoi"); } }
        /// <summary>Thẻ {0}: không tìm thấy tài khoản đang hoạt động.</summary>
        internal static string TaiKhoanKhongTonTai { get { return Get("TaiKhoanKhongTonTai"); } }
        /// <summary>Thẻ {0}: tài khoản đang bị khóa.</summary>
        internal static string TaiKhoanDaKhoa { get { return Get("TaiKhoanDaKhoa"); } }
        /// <summary>Không tra cứu được tài khoản {0}.</summary>
        internal static string KhongTraCuuDuocTaiKhoan { get { return Get("KhongTraCuuDuocTaiKhoan"); } }
        /// <summary>{0}: đã đăng nhập {1}.</summary>
        internal static string DaDangNhap { get { return Get("DaDangNhap"); } }
        /// <summary>{0} đang đăng nhập {1}.</summary>
        internal static string CongDangDangNhapTaiKhoan { get { return Get("CongDangDangNhapTaiKhoan"); } }
        /// <summary>{0} đang đăng nhập {1} từ {2}. Bấm "DS đăng nhập" → Đăng xuất {1} (hoặc Ngắt kết nối rồi Kết nối lại {0}) rồi quét lại thẻ.</summary>
        internal static string CongDangDangNhapNguoiKhac { get { return Get("CongDangDangNhapNguoiKhac"); } }
        /// <summary>{0}: đăng xuất {1} vì tài khoản vừa đăng nhập ở {2}.</summary>
        internal static string ChuyenTaiKhoanSangCongKhac { get { return Get("ChuyenTaiKhoanSangCongKhac"); } }
        /// <summary>{0} chưa có người lấy mẫu. Vui lòng quét thẻ tài khoản trước.</summary>
        internal static string CongChuaCoNguoiLayMau { get { return Get("CongChuaCoNguoiLayMau"); } }
        /// <summary>Không xác định được phòng đang làm việc...</summary>
        internal static string KhongXacDinhPhongLamViec { get { return Get("KhongXacDinhPhongLamViec"); } }
        /// <summary>Không tra cứu được mẫu theo mã {0}.</summary>
        internal static string LoiTraCuuMau { get { return Get("LoiTraCuuMau"); } }
        /// <summary>Không có mẫu chờ lấy theo mã {0} từ ngày {1} tại phòng...</summary>
        internal static string KhongCoMauChoLay { get { return Get("KhongCoMauChoLay"); } }
        /// <summary>Không có mẫu chờ lấy theo mã {0} tại phòng (đã tìm theo barcode từ ngày {1} và theo mã y lệnh)...</summary>
        internal static string KhongCoMauChoLayTheoMaYLenh { get { return Get("KhongCoMauChoLayTheoMaYLenh"); } }
        /// <summary>{0}: bỏ lượt quét {1} vì cổng đã ngắt hoặc mất kết nối. Kết nối lại, quét thẻ rồi quét lại.</summary>
        internal static string BoLuotQuetDoCongNgat { get { return Get("BoLuotQuetDoCongNgat"); } }
        /// <summary>Mẫu {0} đã lấy lúc {1} bởi {2}.</summary>
        internal static string MauDaLay { get { return Get("MauDaLay"); } }
        /// <summary>Mã {0} trùng mẫu của {1} hồ sơ ({2}). Chọn trên màn Lấy mẫu bệnh phẩm.</summary>
        internal static string TrungMauNhieuHoSo { get { return Get("TrungMauNhieuHoSo"); } }
        /// <summary>Hồ sơ đã kết thúc điều trị. Không cho phép thực hiện lấy mẫu.</summary>
        internal static string HoSoDaKetThucDieuTri { get { return Get("HoSoDaKetThucDieuTri"); } }
        /// <summary>Y lệnh {0} hẹn lúc {1}, chưa lấy mẫu được.</summary>
        internal static string YLenhHenGio { get { return Get("YLenhHenGio"); } }
        /// <summary>Tài khoản đăng nhập chưa được phân quyền chức năng Thông tin lấy mẫu.</summary>
        internal static string ChuaCoQuyenThongTinLayMau { get { return Get("ChuaCoQuyenThongTinLayMau"); } }
        /// <summary>Tài khoản đăng nhập chưa được phân quyền chức năng Thiết bị lấy thông tin mẫu bệnh phẩm.</summary>
        internal static string ChuaCoQuyenThietBiLayMau { get { return Get("ChuaCoQuyenThietBiLayMau"); } }
        /// <summary>{0} – {1} – Người lấy mẫu: {2}</summary>
        internal static string TieuDePopupLayMau { get { return Get("TieuDePopupLayMau"); } }
        /// <summary>{0}: đã lấy mẫu {1} – {2} – người lấy mẫu {3}.</summary>
        internal static string DaLayMau { get { return Get("DaLayMau"); } }
        /// <summary>{0}: đóng Thông tin lấy mẫu {1}, chưa lưu.</summary>
        internal static string DongPopupChuaLuu { get { return Get("DongPopupChuaLuu"); } }
        /// <summary>{0}: đã nhận {1}, sẽ xử lý khi cửa sổ đang mở được đóng ({2} lượt chờ).</summary>
        internal static string DaNhanChoXuLy { get { return Get("DaNhanChoXuLy"); } }
        /// <summary>{0}: mất kết nối máy quét. Kết nối lại và quét thẻ.</summary>
        internal static string MatKetNoiCong { get { return Get("MatKetNoiCong"); } }
        /// <summary>Kết nối thiết bị lấy mẫu: {0} đăng nhập tài khoản {1} lúc {2}.</summary>
        internal static string NhatKyDangNhap { get { return Get("NhatKyDangNhap"); } }
        /// <summary>Kết nối thiết bị lấy mẫu: {0} đăng xuất tài khoản {1}. Lý do: {2}.</summary>
        internal static string NhatKyDangXuat { get { return Get("NhatKyDangXuat"); } }
        /// <summary>Kết nối thiết bị lấy mẫu: {0} đang đăng nhập {1}, chặn thẻ {2}.</summary>
        internal static string NhatKyChanThe { get { return Get("NhatKyChanThe"); } }
        /// <summary>Kết nối thiết bị lấy mẫu: mẫu {0} (y lệnh {1}) quét trên {2} đang đăng nhập {3}, người lấy mẫu đã lưu là {4}.</summary>
        internal static string NhatKyDoiNguoiLayMau { get { return Get("NhatKyDoiNguoiLayMau"); } }
        /// <summary>ngắt kết nối cổng</summary>
        internal static string LyDoNgatKetNoi { get { return Get("LyDoNgatKetNoi"); } }
        /// <summary>đóng màn Kết nối thiết bị lấy mẫu</summary>
        internal static string LyDoDongMan { get { return Get("LyDoDongMan"); } }
        /// <summary>mất kết nối máy quét</summary>
        internal static string LyDoMatKetNoi { get { return Get("LyDoMatKetNoi"); } }
        /// <summary>tài khoản đăng nhập ở {0}</summary>
        internal static string LyDoChuyenCong { get { return Get("LyDoChuyenCong"); } }
    }
}
