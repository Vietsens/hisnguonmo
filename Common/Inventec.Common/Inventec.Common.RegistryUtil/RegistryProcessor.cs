/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
