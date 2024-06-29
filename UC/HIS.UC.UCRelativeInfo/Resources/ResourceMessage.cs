using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.UCRelativeInfo.Resources
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.UCRelativeInfo.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        
        internal static string ThongTinCMNDKhongDungDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCRelativeThongTinCMNDKhongChinhXac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongTinBatBuocMotTrongHai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCRelativeThongTinBatBuocMotTrongHai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Thongbao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCRelativeThongbao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
