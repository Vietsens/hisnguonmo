using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TrackingCreate.Resources
{
   public class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TrackingCreate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string TextEdit__KhongDuocNhapSoKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TrackingCreate__TextEdit__KhongDuocNhapSoKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SpinEdit__Dhst__KhongDuocNhapSoAm
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TrackingCreate__SpinEdit__Dhst__KhongDuocNhapSoAm", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NhapQuaMaxlength
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TrackingCreate__NhapQuaMaxlength", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Validate_Tracking_Time
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TrackingCreate__Validate_Tracking_Time", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_TrackingCreate__Validate_Thong_Bao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayIcdTuongUngVoiCacMaSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TrackingCreate__KhongTimThayIcdTuongUngVoiCacMaSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongBaoVuotQuaDoDaiChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TrackingCreate__Validate_Thong_Bao_VuotQuaDoDaiChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string KhongTimThayHoSoDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayHoSoDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        /// <summary>Viec 59656: - Y lenh {0} do {1} chi dinh.</summary>
        internal static string TrackingOwner__YLenhNguoiKhacChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrackingOwner__YLenhNguoiKhacChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 59656: chan luu to dieu tri co y lenh nguoi khac chi dinh.</summary>
        internal static string TrackingOwner__ChanLuuToDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrackingOwner__ChanLuuToDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 59656: canh bao luu to dieu tri co y lenh nguoi khac chi dinh.</summary>
        internal static string TrackingOwner__CanhBaoLuuToDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrackingOwner__CanhBaoLuuToDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
