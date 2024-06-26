using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BrowseExportTicket.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.BrowseExportTicket.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string SpinEdit__SoLuongDuyetPhaiLonHon0
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__SoLuongDuyetPhaiLonHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TuiMauKhongThuocLoaiMauDangChon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__TuiMauKhongThuocLoaiMauDangChon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongTuiMauCuaLoaiLonHonSoLuongYeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__SoLuongTuiMauCuaLoaiLonHonSoLuongYeuCau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocDaCoTrongDanhSachDuyet_BanCoMuonThayThe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__ThuocDaCoTrongDanhSachDuyet_BanCoMuonThayThe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string VatTuDaCoTrongDanhSachDuyet_BanCoMuonThayThe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__VatTuDaCoTrongDanhSachDuyet_BanCoMuonThayThe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string DaCoTrongDanhSachDuyet_BanCoMuonThayThe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__DaCoTrongDanhSachDuyet_BanCoMuonThayThe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__TieuDeCuaSoThongBaoLaCanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TuiMauKhongCoChinhSachGiaChoDoiTuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__TuiMauKhongCoChinhSachGiaChoDoiTuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__NguoiDungNhapNgayKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string LoaiMauKhongCoTuiMauNaoTrongDanhSachXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__LoaiMauKhongCoTuiMauNaoTrongDanhSachXuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCanChonYeuCauXuatTruocKhiChonTuiMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__ButtonAdd__BanCanChonYeuCauXuatTruocKhiChonTuiMau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongTuiMauXuatNhoHonSoLuongYeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__SoLuongTuiMauXuatNhoHonSoLuongYeuCau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__ChuaNhapTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HanDungKhongDuocBeHonHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__HanDungKhongDuocBeHonHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaVachKhongChinhXac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SpinEdit__Dhst__MaVachKhongChinhXac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaDuSoLuongMauYeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__DaDuSoLuongMauYeuCau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongChoPhepDuyetLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__KhongCHoPhepDuyetLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongDuyetLonHonSoLuongYeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_BrowseExportTicket__SoLuongDuyetLonHonSoLuongYeuCau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
