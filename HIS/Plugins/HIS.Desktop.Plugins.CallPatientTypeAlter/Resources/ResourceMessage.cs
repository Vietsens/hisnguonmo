using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter.Resources
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.CallPatientTypeAlter.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string RaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("RaVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuyenVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuyenVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TrongVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrongVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string XinRaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XinRaVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Khoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Khoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Do
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Do", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongThayDoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongThayDoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NangHon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NangHon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TuVong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TuVong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Ma
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Ma", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Ten
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Ten", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TLTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TLTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DichVuBatBuocChonDieuKien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuBatBuocChonDieuKien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TheNayDaDuocSD
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TheNayDaDuocSD", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TongSoTienCuaCacDichVu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TongSoTienCuaCacDichVu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SuaTTBN
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SuaTTBN", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DVKhongTheChuyenDoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DVKhongTheChuyenDoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CapNhatTTDTTB
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CapNhatTTDTTB", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string DaCoThongTinDoiTuongBHYT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaCoThongTinDoiTuongBHYT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BNCoTTDienDoiTuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BNCoTTDienDoiTuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BNTreEmCanNhapDuTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BNTreEmCanNhapDuTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DoiTuongKhamSucKhoe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DoiTuongKhamSucKhoe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TheSapHetHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TheSapHetHan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string TheBhytKhongHopLeBanCoMuonSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TheBhytKhongHopLeBanCoMuonSuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TheBhytKhongHopLeKhongChoPhepDangKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TheBhytKhongHopLeKhongChoPhepDangKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string GoiSangCongBHXHTraVeMaLoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GoiSangCongBHXHTraVeMaLoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TheSaiNgaySinhGov070
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Gov070", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TheSaiHTenGov060
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Gov060", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
