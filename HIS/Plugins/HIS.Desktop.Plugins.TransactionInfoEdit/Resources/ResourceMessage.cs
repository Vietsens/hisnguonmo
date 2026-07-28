using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionInfoEdit.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TransactionInfoEdit.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>
        /// Giao dich thanh toan QR - vui long dung chuc nang Doi hinh thuc thanh toan trong Danh sach giao dich.
        /// </summary>
        internal static string GiaoDichQrVuiLongDungChucNangDoiHinhThucThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaoDichQrVuiLongDungChucNangDoiHinhThucThanhToan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
