using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CashCollect.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.CashCollect.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanCoMuonXoaDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CashCollect__BanCoMuonXoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Không có giao dịch chưa nộp quỹ nào theo điều kiện lọc.</summary>
        internal static string KhongCoGiaoDichChuaNopQuyTheoDieuKienLoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CashCollect__KhongCoGiaoDichChuaNopQuyTheoDieuKienLoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Chưa chọn giao dịch nộp quỹ.</summary>
        internal static string ChuaChonGiaoDichNopQuy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CashCollect__ChuaChonGiaoDichNopQuy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
