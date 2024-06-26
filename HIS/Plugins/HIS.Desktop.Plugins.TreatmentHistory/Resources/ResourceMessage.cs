using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentHistory.Resources
{
    public class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentHistory.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string DaDayThongTinLenHeThongYBaDienTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__DaDayThongTinLenHeThongYBaDienTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string DaDuyetKhoaBaoHiem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__DaDuyetKhoaBaoHiem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string DaDuyetKhoaTaiChinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__DaDuyetKhoaTaiChinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string DaKetThucDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__DaKetThucDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string DangDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__DangDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string TrangThai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__TrangThai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__TatCa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string ChuaDay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__ChuaDay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string DaDay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__DaDay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string KhongCoHoSoNaoDaKetThucMaChuaDayYBaDienTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__KhongCoHoSoNaoDaKetThucMaChuaDayYBaDienTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string DuLieuRong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__DuLieuRong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string KhongDayDuocLenHID
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__KhongDayDuocLenHID", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string ChuyenSangKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TreatmentHistory__ChuyenSangKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
    }
}
