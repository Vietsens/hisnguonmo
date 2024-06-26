using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EInvoiceCreate.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EInvoiceCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.EInvoiceCreate.FormEInvoiceCreate).Assembly);

        internal static string ThongBaoNhapNgay
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("ThongBaoNhapNgay", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }

        internal static string BanChuaChonSo
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("BanChuaChonSo", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }

        internal static string BanChuaChonHinhThuc
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("BanChuaChonHinhThuc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }

        internal static string BanChuaChonHoSo
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("BanChuaChonHoSo", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }
    }
}
