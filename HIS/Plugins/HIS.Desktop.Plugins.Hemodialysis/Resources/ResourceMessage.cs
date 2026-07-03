using System;
using System.Resources;

namespace HIS.Desktop.Plugins.Hemodialysis.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.Hemodialysis.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        private static string GetValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Yêu cầu chưa được gắn với gói vật tư</summary>
        internal static string YeuCauChuaDuocGanVoiGoiVatTu { get { return GetValue("YeuCauChuaDuocGanVoiGoiVatTu"); } }

        /// <summary>Chưa xử lý</summary>
        internal static string TrangThaiChuaXuLy { get { return GetValue("TrangThaiChuaXuLy"); } }

        /// <summary>Đang xử lý</summary>
        internal static string TrangThaiDangXuLy { get { return GetValue("TrangThaiDangXuLy"); } }

        /// <summary>Kết thúc</summary>
        internal static string TrangThaiKetThuc { get { return GetValue("TrangThaiKetThuc"); } }

        /// <summary>Đã tạo gói vật tư chạy thận</summary>
        internal static string DaTaoGoiVatTuChayThan { get { return GetValue("DaTaoGoiVatTuChayThan"); } }

        /// <summary>Chưa tạo gói vật tư chạy thận</summary>
        internal static string ChuaTaoGoiVatTuChayThan { get { return GetValue("ChuaTaoGoiVatTuChayThan"); } }
    }
}
