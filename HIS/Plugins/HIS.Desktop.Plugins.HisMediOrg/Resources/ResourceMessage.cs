using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMediOrg.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisMediOrg.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string DuLieuPhongDangChonThuocNhieuChiNhanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ChooseRoom__DuLieuPhongDangChonThuocNhieuChiNhanh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string InvoiceBook_GiaTriLonHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("InvoiceBook_GiaTriLonHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_HisMediOrg__NhapQuaMaxlength", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "Kh\u00f4ng đ\u01b0\u1ee3c nh\u1eadp qu\u00e1 {0} k\u00fd t\u1ef1";
            }
        }
    }
}
