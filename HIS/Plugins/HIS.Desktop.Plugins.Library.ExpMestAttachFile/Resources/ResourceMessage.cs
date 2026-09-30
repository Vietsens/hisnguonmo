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

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager(
            "HIS.Desktop.Plugins.Library.ExpMestAttachFile.Resources.Message.Lang",
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

        /// <summary>Tệp "{0}" không đúng định dạng cho phép (jpg, jpeg, png, bmp, gif, pdf).</summary>
        internal static string FileKhongDungDinhDang { get { return Get("FileKhongDungDinhDang"); } }

        /// <summary>Tệp "{0}" có dung lượng {1} MB, vượt quá dung lượng cho phép {2} MB...</summary>
        internal static string FileVuotDungLuong { get { return Get("FileVuotDungLuong"); } }

        /// <summary>Vui lòng chọn tệp hoặc chụp ảnh đơn thuốc cần đính kèm.</summary>
        internal static string ChuaChonTepDinhKem { get { return Get("ChuaChonTepDinhKem"); } }

        /// <summary>Chưa khai báo loại văn bản "{0}" trên hệ thống EMR...</summary>
        internal static string KhongTimThayLoaiVanBan { get { return Get("KhongTimThayLoaiVanBan"); } }

        /// <summary>Tài khoản chưa được phân quyền chức năng Chụp ảnh...</summary>
        internal static string KhongTimThayModuleCamera { get { return Get("KhongTimThayModuleCamera"); } }

        /// <summary>Phiếu xuất đã hoàn thành/đã thanh toán nên không được xóa đơn đã đính kèm...</summary>
        internal static string PhieuDaHoanTatKhongDuocXoa { get { return Get("PhieuDaHoanTatKhongDuocXoa"); } }

        /// <summary>Không tải được nội dung tài liệu.</summary>
        internal static string KhongTaiDuocNoiDungTaiLieu { get { return Get("KhongTaiDuocNoiDungTaiLieu"); } }

        /// <summary>Phiếu xuất {0} đã được lưu nhưng đính kèm đơn thuốc thất bại...</summary>
        internal static string DinhKemDonThatBai { get { return Get("DinhKemDonThatBai"); } }

        /// <summary>Phiếu bán cho nhiều bệnh nhân: đơn thuốc chưa được đính kèm tự động...</summary>
        internal static string BanNhieuBenhNhanKhongDinhKem { get { return Get("BanNhieuBenhNhanKhongDinhKem"); } }

        /// <summary>Có {0} tệp đơn thuốc đính kèm chưa được lưu. Bạn có muốn bỏ các tệp này không?</summary>
        internal static string CoDonChuaLuuBanCoMuonBo { get { return Get("CoDonChuaLuuBanCoMuonBo"); } }

        /// <summary>Chức năng chỉ áp dụng cho phiếu xuất bán.</summary>
        internal static string ChuaChonPhieuXuatBan { get { return Get("ChuaChonPhieuXuatBan"); } }
    }
}
