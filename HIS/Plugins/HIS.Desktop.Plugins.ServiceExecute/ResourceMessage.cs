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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string XacNhanKhongThucHien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XacNhanKhongThucHien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HuyXacNhanKhongThucHien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HuyXacNhanKhongThucHien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HuyXuLy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HuyXuLy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string MaYLenhKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaYLenhKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCapNhatDuocTrangThaiXuLyCuaYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCapNhatDuocTrangThaiXuLyCuaYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayDULieuTheoMaYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayDULieuTheoMaYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonInKetQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonInKetQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiKhoanKhongCoQuyenThucHienChucNang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanKhongCoQuyenThucHienChucNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayIn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayIn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaCauHinhDiaChiLayKetQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaCauHinhDiaChiLayKetQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThieuThongTinKip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuThongTinKip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuoiKyTuQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuoiKyTuQuaDai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongXacDinhDuocMay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongXacDinhDuocMay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaCauHinhDiaChiPACS
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaCauHinhDiaChiPACS", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaXuLyHetDichVu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaXuLyHetDichVu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonAnh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonAnh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChiDuocChonMotAnh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChiDuocChonMotAnh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongXacDinhDuocVungChuaAnh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongXacDinhDuocVungChuaAnh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuChuaKeThuocVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuChuaKeThuocVatTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucThoiGianRaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucThoiGianRaVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauThoiGianRaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauThoiGianRaVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauThoiGianKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauThoiGianKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayICDTuongUng
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayICDTuongUng", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauKhongDuocLonHonThoiGianKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauKhongDuocLonHonThoiGianKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauPhaiLonHonThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauPhaiLonHonThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhongDuocNhoHonThoiGianBatDau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucKhongDuocNhoHonThoiGianBatDau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhongDuocLonHonThoiGianHeThong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucKhongDuocLonHonThoiGianHeThong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongDuLieuVuotQuaKyTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuVuotQuaKyTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhongDuocNhoHonThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucKhongDuocNhoHonThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonSuaThoiGianYLenhBangThoiGianBatDauPTTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonSuaThoiGianYLenhBangThoiGianBatDauPTTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoMayVuotQuaSoluongXuLy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoMayVuotQuaSoluongXuLy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuChuaCoMay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuChuaCoMay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BatBuocChonKip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocChonKip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauThoiGianKetThucKhongDuocNhoHonThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauThoiGianKetThucKhongDuocNhoHonThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TieuDeChonAnh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TieuDeChonAnh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonDuongDanLuuAnh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonDuongDanLuuAnh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CopyImage
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CopyImage", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChangeStt
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChangeStt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThucHienCls
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThucHienCls", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KeKhaiThuocHaoPhi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KeKhaiThuocHaoPhi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PhieuPhauThuatThuThuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhieuPhauThuatThuThuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KeThuocVatTuTieuHao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KeThuocVatTuTieuHao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhaiBaoThuocVTTH
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhaiBaoThuocVTTH", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HuyPhieuThuocVatTuTieuHaoDaKe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HuyPhieuThuocVatTuTieuHaoDaKe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuChuaCoNhomPttt
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuChuaCoNhomPttt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonTiepTucKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonTiepTucKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonKipThucHien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonKipThucHien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiKhoanDuocGanNhieuVaiTro
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanDuocGanNhieuVaiTro", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
    }
}
