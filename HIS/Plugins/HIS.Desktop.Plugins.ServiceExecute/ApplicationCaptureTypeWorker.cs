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

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public class ApplicationCaptureTypeWorker
    {
        private static RegistryKey appFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);

        public static void ChangeCaptureConnectType(int CaptureConnectType)
        {
            try
            {
                appFolder.SetValue(RegistryConstant.CONNECTION_TYPE_KEY, CaptureConnectType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static int GetCaptureConnectType()
        {
            int captureConnectType = 1;
            try
            {
                var f = appFolder.GetValue(RegistryConstant.CONNECTION_TYPE_KEY, 1);
                captureConnectType = Inventec.Common.TypeConvert.Parse.ToInt32(f.ToString());
            }
            catch (Exception ex)
            {
                captureConnectType = 1;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return captureConnectType;
        }               
    }
}
