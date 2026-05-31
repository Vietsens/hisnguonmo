/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using System;
using System.Resources;

namespace HIS.Desktop.Plugins.HisServiceConsult.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.HisServiceConsult.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Vui long chon goi dich vu</summary>
        internal static string VuiLongChonGoiDichVu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "VuiLongChonGoiDichVu",
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

        /// <summary>Truong du lieu bat buoc</summary>
        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "TruongDuLieuBatBuoc",
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

        /// <summary>Ly do khong duoc vuot qua 2000 ky tu</summary>
        internal static string LyDoKhongDuocVuotQua2000KyTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "LyDoKhongDuocVuotQua2000KyTu",
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

        /// <summary>Mo ta khong duoc vuot qua 2000 ky tu</summary>
        internal static string MoTaKhongDuocVuotQua2000KyTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "MoTaKhongDuocVuotQua2000KyTu",
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

        /// <summary>Xu ly thanh cong</summary>
        internal static string XuLyThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "XuLyThanhCong",
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

        /// <summary>Xu ly that bai</summary>
        internal static string XuLyThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "XuLyThatBai",
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
