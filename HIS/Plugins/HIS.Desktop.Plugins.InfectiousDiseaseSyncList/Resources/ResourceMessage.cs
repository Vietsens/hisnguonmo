/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Accessor thông báo riêng plugin (Message.Lang.vi/en.resx).
 */
using System;
using System.Reflection;
using System.Resources;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Resources
{
    class ResourceMessage
    {
        static ResourceManager languageMessage = new ResourceManager(
            "HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Resources.Message.Lang",
            Assembly.GetExecutingAssembly());

        private static string Get(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key, languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Bệnh của điều trị không thuộc danh mục bệnh truyền nhiễm.</summary>
        internal static string BenhKhongThuocDanhMucTruyenNhiem
        {
            get { return Get("BenhKhongThuocDanhMucTruyenNhiem"); }
        }

        /// <summary>Chưa cấu hình kết nối cổng ECDS. Vui lòng kiểm tra cấu hình.</summary>
        internal static string ChuaCauHinhKetNoiEcds
        {
            get { return Get("ChuaCauHinhKetNoiEcds"); }
        }

        /// <summary>Không tìm thấy mã địa bàn/danh mục trên cổng ECDS.</summary>
        internal static string KhongTimThayMaTrenCong
        {
            get { return Get("KhongTimThayMaTronCong"); }
        }

        /// <summary>Bạn có chắc muốn đẩy ca bệnh này lên cổng ECDS?</summary>
        internal static string XacNhanDayCaBenh
        {
            get { return Get("XacNhanDayCaBenh"); }
        }
    }
}
