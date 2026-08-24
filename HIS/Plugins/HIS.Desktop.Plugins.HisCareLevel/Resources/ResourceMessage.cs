using System;

namespace HIS.Desktop.Plugins.HisCareLevel.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisCareLevel.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Trang thai hien thi tren luoi khi IS_ACTIVE = 1</summary>
        internal static string TrangThaiHoatDong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisCareLevel__TrangThaiHoatDong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "Hoạt động";
            }
        }

        /// <summary>Trang thai hien thi tren luoi khi IS_ACTIVE = 0</summary>
        internal static string TrangThaiTamKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisCareLevel__TrangThaiTamKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "Tạm khóa";
            }
        }

        /// <summary>Canh bao khi ban ghi dang bi khoa nhung nguoi dung bam Sua</summary>
        internal static string BanGhiDangBiKhoaKhongTheSua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisCareLevel__BanGhiDangBiKhoaKhongTheSua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "Bản ghi đang bị khóa, không thể sửa";
            }
        }
    }
}
