using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicalStoreV2.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.MedicalStoreV2.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string DaKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string VuiLongChonHoSoCanLuuTru
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VuiLongChonHoSoCanLuuTru", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TatCa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TatCa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string XuLyThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XuLyThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoluuTruCuHoSoLa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoluuTruCuHoSoLa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonXoaDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonXoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCothucSuMuonHuyTuChoiDuyetBenhAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCothucSuMuonHuyTuChoiDuyetBenhAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayFile
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BieuMauDangMoHoacKhongTonTaiFile
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BieuMauDangMoHoacKhongTonTaiFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string XuatFileThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XuatFileThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonMoFile
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonMoFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("XuLyThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Đã kiểm tra</summary>
        internal static string DaKiemTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaKiemTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Chưa kiểm tra</summary>
        internal static string ChuaKiemTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaKiemTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Bạn có chắc chắn muốn đánh dấu hồ sơ đã kiểm tra? Thao tác này không hoàn tác được.</summary>
        internal static string XacNhanDanhDauDaKiemTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XacNhanDanhDauDaKiemTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Bạn không có quyền đánh dấu hồ sơ đã kiểm tra.</summary>
        internal static string KhongDuQuyenDanhDauDaKiemTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuQuyenDanhDauDaKiemTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
