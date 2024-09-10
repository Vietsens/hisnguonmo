using Microsoft.Win32;
using System;

namespace Inventec.Common.RegistryUtil
{
    public class RegistryProcessor
    {
        private const string SOFTWARE_FOLDER = "SOFTWARE";
        private const string INVENTEC_FOLDER = "INVENTEC";

        /// <summary>
        /// Ghi vao registry
        /// </summary>
        /// <param name="key">Ten tham so registry</param>
        /// <param name="value">Gia tri</param>
        /// <param name="subFolders">Mang quy dinh thu muc con can luu registry</param>
        public static void Write(string key, object value, params string[] subFolders)
        {
            RegistryKey register = GetRegister(subFolders);
            register.SetValue(key, value);
        }

        /// <summary>
        /// Doc tu registry
        /// </summary>
        /// <param name="key">Ten tham so registry</param>
        /// <param name="subFolders">Mang quy dinh thu muc con can luu registry</param>
        public static object Read(string key, params string[] subFolders)
        {
            RegistryKey register = GetRegister(subFolders);
            return register.GetValue(key);
        }

        /// <summary>
        /// Doc tu registry
        /// </summary>
        /// <param name="key">Ten tham so registry</param>
        /// <param name="subFolders">Mang quy dinh thu muc con can luu registry</param>
        public static void DeleteValue(string key, params string[] subFolders)
        {
            RegistryKey register = GetRegister(subFolders);
            register.DeleteValue(key);
        }

        private static RegistryKey GetRegister(params string[] subFolders)
        {
            RegistryKey register = Registry.CurrentUser.CreateSubKey(SOFTWARE_FOLDER).CreateSubKey(INVENTEC_FOLDER);
            if (subFolders != null && subFolders.Length > 0)
            {
                foreach (string subFolder in subFolders)
                {
                    register = register.CreateSubKey(subFolder);
                }
            }
            return register;
        }
    }
}
