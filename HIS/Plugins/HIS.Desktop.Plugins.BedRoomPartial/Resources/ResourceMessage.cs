using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedRoomPartial.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.BedRoomPartial.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

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

        internal static string ToolTipLeaves
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_BedRoomPartial__ToolTipLeaves", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ToolTipCome
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_BedRoomPartial__ToolTipCome", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string DieuTriThuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriThuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DieuTriKetHop
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DieuTriKetHop", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DangDieuTriTrongBuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DangDieuTriTrongBuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DangDieuTriNgoaiBuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DangDieuTriNgoaiBuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    return Inventec.Common.Resource.Get.Value("DaKetThucDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TrongKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrongKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BNDangOTrongBuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BNDangOTrongBuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BNVaoTrongKhoang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BNVaoTrongKhoang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BNChuaKeDonTrongKhoang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BNChuaKeDonTrongKhoang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CoBenhNhanhBHYTTrongTongSoBenhNhan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CoBenhNhanhBHYTTrongTongSoBenhNhan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BHYT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BHYT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string MucAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MucAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DTTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DTTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ToolTipDTTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ToolTipDTTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string MucAnDTTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MucAnDTTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ToolTipMucAnDTTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ToolTipMucAnDTTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HoSoDieuTriDangTamKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HoSoDieuTriDangTamKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DonKhongLayKhongChoPhepSua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DonKhongLayKhongChoPhepSua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string YLenhDaTonTaiVanBanKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YLenhDaTonTaiVanBanKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanCapCuu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanCapCuu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanDaDuDieuKienRaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanDaDuDieuKienRaVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThoiGianDenPhaiLonHonThoiGianTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianDenPhaiLonHonThoiGianTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string ThuocVtBNDaDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVtBNDaDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TraSoatHoSoBenhAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TraSoatHoSoBenhAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HoSoGiayToDinhKem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HoSoGiayToDinhKem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhanLoaiBN
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhanLoaiBN", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
