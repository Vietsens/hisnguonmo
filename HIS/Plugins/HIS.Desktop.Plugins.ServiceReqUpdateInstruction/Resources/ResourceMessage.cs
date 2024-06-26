using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string KhongTimThayIcdTuongUng
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__KhongTimThayIcdTuongUng", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongChoNhapThoiGianNhoHonThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__KhongChoNhapThoiGianNhoHonThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaTriBatBuocNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_SERVICE_REQ_UPDATE_INSTRUCTION__INSTRUCTION_TIME_NOT_NULL", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianYLenhBeHonThoiGianBatDau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_SERVICE_REQ_UPDATE_INSTRUCTION__THOI_GIAN_Y_LENH_BE_HON_THOI_GIAN_BAT_DAU", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauBeHonThoiGianKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_SERVICE_REQ_UPDATE_INSTRUCTION__THOI_GIAN_KET_THUC_BE_HON_THOI_GIAN_BAT_DAU", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiChiDinhKhongCoThongTinHoTen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiChiDinhKhongCoThongTinHoTen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
