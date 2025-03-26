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

namespace Inventec.Common.WitAI
{
    public class RegconizeWorker
    {
        private static RegistryKey appFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);

        public static void ChangeRegconizeVoice(bool reg)
        {
            try
            {
                appFolder.SetValue(RegistryConstant.NAME_KEY, reg);
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static bool GetRegconizeVoice()
        {
            bool reg = false;
            try
            {
                var f = appFolder.GetValue(RegistryConstant.NAME_KEY, false);
                reg = (bool)(f);
            }
            catch (Exception ex)
            {
                reg = false;
                //Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return reg;
        }
    }
}
