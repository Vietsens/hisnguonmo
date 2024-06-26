using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PatientUpdate.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource { get; set; }
        internal static string NgaySinhKhongDuocNhoHon7
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Update_Patient__NgaySinhKhongDuocNhoHon7", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NhapGioSinhKhongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Update_Patient__NhapGioSinhKhongDung", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NhapNgaySinhKhongDungDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Update_Patient__NhapNgaySinhKhongDungDinhDang", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThongTinNgaySinhPhaiNhoHonNgayHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Update_Patient__ThongTinNgaySinhPhaiNhoHonNgayHienTai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
