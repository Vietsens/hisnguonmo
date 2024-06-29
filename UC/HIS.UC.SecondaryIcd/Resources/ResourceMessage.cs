using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.SecondaryIcd.Resources
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.SecondaryIcd.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static System.Resources.ResourceManager LanguagefrmSecondaryIcd = new System.Resources.ResourceManager("HIS.UC.SecondaryIcd.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        public static ResourceManager LanguageResourceUCSecondaryIcd { get; set; }

        internal static string MabenhPhuDaDuocSuDungChoMaBenhChinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MabenhPhuDaDuocSuDungChoMaBenhChinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("KhongTimThayIcdTuongUngVoiCacMaSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
              
        internal static string NhapQuaKyTuBenhPhu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhapQuaKyTuBenhPhu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
