using System;

namespace HIS.Desktop.Plugins.HisGoodsType.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager(
            "HIS.Desktop.Plugins.HisGoodsType.Resources.Message.Lang",
            System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Loại dịch vụ đang được sử dụng. Không thể xóa.</summary>
        internal static string LoaiDichVuDangSuDungKhongTheXoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "frmHisGoodsType.LoaiDichVuDangSuDungKhongTheXoa",
                        languageMessage,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
