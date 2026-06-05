/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using System;
using System.Resources;

namespace HIS.UC.TransactionPayformGrid.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.UC.TransactionPayformGrid.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Vui long chon hinh thuc thanh toan</summary>
        internal static string VuiLongChonHinhThucThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "VuiLongChonHinhThucThanhToan",
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

        /// <summary>Vui long chon ngan hang</summary>
        internal static string VuiLongChonNganHang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "VuiLongChonNganHang",
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

        /// <summary>So tien phai lon hon 0</summary>
        internal static string SoTienPhaiLonHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "SoTienPhaiLonHonKhong",
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
