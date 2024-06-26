using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisAssignBlood.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisAssignBlood.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ChuaChonNgayChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__ChuaChonNgayChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MauDaKeBanCoMuonThem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__MauDaKeBanCoMuonThem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CanhBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__CanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__KhongTimThayIcdTuongUngVoiCacMaSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DuLieuKhongHopHeVuiLongKiemTraLai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__DuLieuKhongHopHeVuiLongKiemTraLai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string MessageMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__Mau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTimThayHoSoBenhNhanCoId
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__KhongTimThayHoSoBenhNhanCoId", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTimThayHoSoDieuTriCoId
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__KhongTimThayHoSoDieuTriCoId", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongNhapSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__KhongNhapSoLuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongCoDoiTuongThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__KhongCoDoiTuongThanhToan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanDangThieuVienPhi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignBlood__BenhNhanDangThieuVienPhi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
