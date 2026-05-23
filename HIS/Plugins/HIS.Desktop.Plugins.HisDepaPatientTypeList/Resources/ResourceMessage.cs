using System;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.HisDepaPatientTypeList.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Vui lòng chọn một khoa!</summary>
        internal static string VuiLongChonMotKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "HisDepaPatientTypeList.VuiLongChonMotKhoa",
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

        /// <summary>Vui lòng chọn một đối tượng thanh toán!</summary>
        internal static string VuiLongChonMotDoiTuongThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "HisDepaPatientTypeList.VuiLongChonMotDoiTuongThanhToan",
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
