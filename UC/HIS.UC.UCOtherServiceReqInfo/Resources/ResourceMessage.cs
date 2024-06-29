using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.UCOtherServiceReqInfo.Resources
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.UCOtherServiceReqInfo.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        
        internal static string SoThuTuUuTienPhaiNamTrongDanhSachCauHinhCacSoUuTien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoThuTuUuTienPhaiNamTrongDanhSachCauHinhCacSoUuTien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NhapNgayThangKhongDungDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_NhapNgayThangKhongDungDinhDang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapThongTinDoiTuongCungChiTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_ChuaNhapThongTinDoiTuongCungChiTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
