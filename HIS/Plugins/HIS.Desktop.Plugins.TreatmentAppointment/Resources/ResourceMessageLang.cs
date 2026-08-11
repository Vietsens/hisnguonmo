using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentAppointment.Resources
{
    public class ResourceMessageLang
    {
        public static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentAppointment.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ChiBacSiTaoLichHenDuocSua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiBacSiTaoLichHenDuocSua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string DaTaiKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaTaiKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaTaiKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaTaiKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string DenNgayHenKhamTrong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DenNgayHenKhamTrong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DaQuaThoiGianHenKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaQuaThoiGianHenKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThoiGianHenKhamTrongKhoang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianHenKhamTrongKhoang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Vui lòng chọn ít nhất một bệnh nhân để gửi tin nhắn nhắc tái khám.</summary>
        internal static string VuiLongChonItNhatMotBenhNhan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VuiLongChonItNhatMotBenhNhan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Tính năng gửi Zalo nhắc tái khám chưa được bật. Vui lòng liên hệ quản trị viên.</summary>
        internal static string ChucNangGuiZaloChuaDuocBat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChucNangGuiZaloChuaDuocBat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Đã chọn {0} bệnh nhân để gửi tin nhắn Zalo nhắc tái khám.</summary>
        internal static string DaChonNBenhNhanDeGuiZalo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaChonNBenhNhanDeGuiZalo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Vui lòng chọn một template để gửi tin nhắn.</summary>
        internal static string VuiLongChonTemplate
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VuiLongChonTemplate", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Format tổng kết: "Tổng số: {0} | Thành công: {1} | Thất bại: {2}".</summary>
        internal static string TongKetGuiZaloFormat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TongKetGuiZaloFormat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Tiêu đề chi tiết các trường hợp gửi thất bại.</summary>
        internal static string ChiTietGuiThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiTietGuiThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Format: "{0} bệnh nhân"</summary>
        internal static string NBenhNhanFormat
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("NBenhNhanFormat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Tên gateway OneSMS hiển thị trên popup</summary>
        internal static string GatewayOneSms
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("GatewayOneSms", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Tên gateway FNS ZNS (FPT) hiển thị trên popup</summary>
        internal static string GatewayFnsZns
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("GatewayFnsZns", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Format: "Nội dung xem trước (với bệnh nhân: {0} · {1})"</summary>
        internal static string NoiDungXemTruocVoiBenhNhanFormat
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("NoiDungXemTruocVoiBenhNhanFormat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>"Nội dung xem trước:"</summary>
        internal static string NoiDungXemTruoc
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("NoiDungXemTruoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Tooltip badge chất lượng HIGH</summary>
        internal static string QualityHighTooltip
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("QualityHighTooltip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Tooltip badge chất lượng MEDIUM</summary>
        internal static string QualityMediumTooltip
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("QualityMediumTooltip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Tooltip badge chất lượng LOW — cảnh báo</summary>
        internal static string QualityLowTooltip
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("QualityLowTooltip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Format heading: "Đã gửi thành công {0}/{1} bệnh nhân"</summary>
        internal static string DaGuiThanhCongFormat
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("DaGuiThanhCongFormat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Mô tả nghiệp vụ gửi thành công toàn bộ</summary>
        internal static string DescriptionGuiThanhCong
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("DescriptionGuiThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Format heading: "Gửi thất bại {0}/{1} bệnh nhân"</summary>
        internal static string GuiThatBaiFormat
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("GuiThatBaiFormat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Mô tả nghiệp vụ gửi thất bại toàn bộ</summary>
        internal static string DescriptionGuiThatBai
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("DescriptionGuiThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Format heading: "Gửi thành công {0}/{1} · Thất bại {2}"</summary>
        internal static string GuiMotPhanThanhCongFormat
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("GuiMotPhanThanhCongFormat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }

        /// <summary>Mô tả nghiệp vụ gửi một phần thành công</summary>
        internal static string DescriptionGuiMotPhanThanhCong
        {
            get
            {
                try { return Inventec.Common.Resource.Get.Value("DescriptionGuiMotPhanThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return "";
            }
        }
    }
}
