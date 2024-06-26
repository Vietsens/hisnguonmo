using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DispenseDetail.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormDispenseDetail { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormDispenseDetail = new ResourceManager("HIS.Desktop.Plugins.DispenseDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.DispenseDetail.frmDispenseDetail).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static ResourceManager LanguageResource { get; set; }
        public static ResourceManager LanguageResource__frmDetail { get; set; }

        internal static string TaiKhoanKhongCoQuyenThucHienChucNang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanKhongCoQuyenThucHienChucNang", LanguageFormDispenseDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
