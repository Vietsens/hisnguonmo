using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExecuteRoom.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExecuteRoom.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

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

        internal static string BanCoMuonHuyBatDauKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__BanCoMuonHuyBatDauKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string XacNhanHuyBatDau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__XacNhanHuyBatDau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string DaKhamDuSoLuongBNBHYTChoPhepTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__DaKhamDuSoLuongBNBHYTChoPhepTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoManHinhCho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__KhongCoManHinhCho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuDaDuocKeVoiYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__DichVuDaDuocKeVoiYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTonTaiPhieuDaKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTonTaiPhieuDaKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongBhytTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__NumberBHYT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__Number", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
