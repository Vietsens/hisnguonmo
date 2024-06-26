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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.UploadFileAUP
{
    internal class PathStoreWorker
    {
        private static RegistryKey appFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);

        internal static void ChangeWorkingPath(string path)
        {
            try
            {
                appFolder.SetValue(RegistryConstant.DATA_KEY, path);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static string GetWorkingPath()
        {
            string path = "";
            try
            {
                var f = appFolder.GetValue(RegistryConstant.DATA_KEY, "");
                path = f != null ? f.ToString() : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return path;
        }
    }
}
