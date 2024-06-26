using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisServiceChangeReqList.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServiceChangeReqList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServiceChangeReqList.HisServiceChangeReqList.frmHisServiceChangeReqList).Assembly);

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.HisServiceChangeReqList.TruongDuLieuBatBuoc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTienChuyenKhoanLonHonSoTienTamUng
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.HisServiceChangeReqList.SoTienChuyenKhoanLonHonSoTienTamUng", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTienQuetTheLonHonSoTienTamUng
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS.Desktop.Plugins.HisServiceChangeReqList.SoTienQuetTheLonHonSoTienTamUng", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
