using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ManuImpMestUpdate.Resources
{
    class ResourceMessageManager
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ManuImpMestUpdate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

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

    }
}
