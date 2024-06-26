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

namespace HIS.Desktop.Plugins.ImpMestCreate.Base
{
    class ResourceMessageManager
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ImpMestCreate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ThuocVatTuKhongNamTrongGoiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__ThuocVatTuKhongNamTrongGoiThau_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoChungTuDaDuocSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__SoChungTuDaDuocSuDung_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaNhapKhacVoiGiaNhapTrongGoiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__GiaNhapKhacVoiGiaNhapTrongGoiThau_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }


        internal static string ChuaNhapHanSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__ChuaNhapHanSuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VatNhapKhacVoiVatNhapTrongGoiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__VatNhapKhacVoiVatNhapTrongGoiThau_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuDaCoTrongDanhSachNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__ThuocVatTuDaCoTrongDanhSachNhap_BanCoMuonThayThe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaVatNhapKhacVoiGiaVatNhapTrongGoiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__GiaVatNhapKhacVoiGiaVatNhapTrongGoiThau_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongPhaiLoaiNhapTuNhaCungCap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__KhongPhaiLoaiNhapTuNhaCungCap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TonTaiThuocVatTuKhongNamTrongGoiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__TonTaiThuocVatTuKhongNamTrongGoiThau_BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongNhapLonHonSoLuongKhaNhapTrongGoiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__SoLuongNhapLonHonSoLuongKhaNhapTrongGoiThau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayVatTuTheoMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__KhongTimThayVatTuTheoMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayThuocTheoMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__KhongTimThayThuocTheoMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongXacDinhDuocThuocHayVatTuMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__KhongXacDinhDuocThuocHayVatTuMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaBanPhaiLonHonKhongMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__GiaBanPhaiLonHonKhongMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayDoiTuongTheoMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__KhongTimThayDoiTuongTheoMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongKhaNhapCuThuocVatTuTrongGoiThauNhoHon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ImpMestCreate__SoLuongKhaNhapCuThuocVatTuTrongGoiThauNhoHon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string XuLyThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__XuLyThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TieuDeCuaSoThongBaoLaThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TieuDeCuaSoThongBaoLaCanhBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__TieuDeCuaSoThongBaoLaCanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoLuongPhaiLonHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__SoLuongPhaiLonHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiDungNhapNgayKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__NguoiDungNhapNgayKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DuLieuDocTuFileExcelRong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__DuLieuDocTuFileExcelRong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiDungChuaChonLoaiNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__NguoiDungChuaChonLoaiNhap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiDungChuaChonKhoNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__NguoiDungChuaChonKhoNhap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string LoaiNhapLaNhapTuNhaCungCapNguoiDungPhaiChonNhaCungCap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__LoaiNhapLaNhapTuNhaCungCapNguoiDungPhaiChonNhaCungCap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string LoaiNhapLaNhapTuNhaCungCaoNguoiDungPhaiChonGoiThauHoacTichVaoNgoaiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__LoaiNhapLaNhapTuNhaCungCaoNguoiDungPhaiChonGoiThauHoacTichVaoNgoaiThau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TieuDeThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__TieuDeThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TieuDeVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__TieuDeVatTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TieuDeDaHetHanSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__TieuDeDaHetHanSuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TieuDeBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__TieuDeBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Plugins_ImpMestCreate__HanSuDungNhoHonCauHinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__HanSuDungNhoHonCauHinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaSauVatLonHonGiaHopDong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__GiaSauVatLonHonGiaHopDong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VatNhapLonHonVatTrongHopDong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__VatNhapLonHonVatTrongHopDong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongNhapLonHonSoLuongTrongHopDong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__SoLuongNhapLonHonSoLuongTrongHopDong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianNhapVuotHieuLucHopDong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__ThoiGianNhapVuotHieuLucHopDong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianNhapVuotHanNhapThuocVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ImpMestCreate__ThoiGianNhapVuotHanNhapThuocVatTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
