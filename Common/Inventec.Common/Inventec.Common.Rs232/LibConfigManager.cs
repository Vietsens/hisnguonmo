using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Inventec.Common.Rs232
{
    class LibConfigManager
    {
        private static readonly LibConfigManager instance = new LibConfigManager();
        private Configuration config;

        //Khai báo hàm khởi tạo static để cho trình biên dịch C# không đánh dấu kiểu "beforefieldinit"
        static LibConfigManager()
        {
        }

        /// <summary>
        /// Hàm khởi tạo "Private" để ngăn truy cập từ bên ngoài
        /// </summary>
        private LibConfigManager()
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = @"Inventec.Common.Rs232.dll.config";
            config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }

        /// <summary>
        /// Get instance
        /// </summary>
        private static LibConfigManager Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Ghi giá trị cấu hình kiểu chuỗi ra file
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Write(string key, string value)
        {
            if (Instance.config != null && key != null && value != null)
            {
                Instance.config.AppSettings.Settings.Remove(key);
                Instance.config.AppSettings.Settings.Add(key, value);
                Instance.config.Save();
            }
        }

        /// <summary>
        /// Ghi giá trị cấu hình kiểu số nguyên ra file
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Write(string key, int? value)
        {

            if (Instance.config != null && key != null && value != null)
            {
                Instance.config.AppSettings.Settings.Remove(key);
                Instance.config.AppSettings.Settings.Add(key, value.ToString());
                Instance.config.Save();
            }
        }

        /// <summary>
        /// Lấy giá trị cấu hình kiểu số nguyên
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Integer value</returns>
        public static int GetIntConfig(string key)
        {
            if (Instance.config != null)
            {
                return int.Parse((string)Instance.config.AppSettings.Settings[key].Value);
            }
            return 0;
        }

        /// <summary>
        /// Lấy giá trị cấu hình kiểu chuỗi
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>String value</returns>
        public static string GetStringConfig(string key)
        {

            if (Instance.config != null)
            {
                return (string)Instance.config.AppSettings.Settings[key].Value;
            }

            return null;
        }

        /// <summary>
        /// Lấy danh sách key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>String value</returns>
        public static string[] GetKeyCollection()
        {
            if (Instance.config != null)
            {
                return Instance.config.AppSettings.Settings.AllKeys;
            }
            return null;
        }
    }
}
