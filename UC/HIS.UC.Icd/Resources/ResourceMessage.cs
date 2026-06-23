using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.Icd.Resources
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.Icd.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

       
        internal static string IcdKhongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IcdKhongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Bệnh {0} không khuyến khích dùng làm bệnh chính. Bạn có chắc chắn sử dụng không?</summary>
        internal static string BenhKhongKhuyenKhichDungLamBenhChinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhKhongKhuyenKhichDungLamBenhChinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Bệnh {0} chỉ sử dụng đối với bệnh nhân tử vong. Bạn có chắc chắn sử dụng không?</summary>
        internal static string BenhChiSuDungChoBenhNhanTuVong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhChiSuDungChoBenhNhanTuVong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
