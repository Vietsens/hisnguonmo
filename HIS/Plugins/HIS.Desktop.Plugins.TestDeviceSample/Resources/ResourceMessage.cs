using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestDeviceSample.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TestDeviceSample.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ThoiGianKhongDuocNhoHonThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TestDeviceSample__ThoiGianKhongDuocNhoHonThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuDaThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TestDeviceSample__DichVuDaThanhToan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        private static string Get(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Đăng xuất tài khoản {0} khỏi {1}? ...</summary>
        internal static string XacNhanDangXuat { get { return Get("TestDeviceSample__XacNhanDangXuat"); } }
        /// <summary>Phiên đăng nhập trên cổng này đã thay đổi. Danh sách đã được tải lại.</summary>
        internal static string PhienDaThayDoi { get { return Get("TestDeviceSample__PhienDaThayDoi"); } }
        /// <summary>Kết nối thiết bị lấy mẫu: {0} đăng xuất tài khoản {1}. Lý do: {2}.</summary>
        internal static string NhatKyDangXuat { get { return Get("TestDeviceSample__NhatKyDangXuat"); } }
        /// <summary>đăng xuất ở màn Thiết bị lấy thông tin mẫu bệnh phẩm</summary>
        internal static string LyDoDangXuatTaiMan { get { return Get("TestDeviceSample__LyDoDangXuatTaiMan"); } }
    }
}
