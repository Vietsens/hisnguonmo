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
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisPrintTestCFG
    {

        const string isSplit = "1";
        private static bool? assignPrintTEST;
        public static bool AssignPrintTEST
        {
            get
            {
                try
                {
                    if (!assignPrintTEST.HasValue)
                    {
                        assignPrintTEST = GetData(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_Desktop_Plugins_AssignServicePrintTEST));
                    }
                }
                catch (Exception ex)
                {
                    assignPrintTEST = false;
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                
                return assignPrintTEST.Value;
            }
            set
            {
                assignPrintTEST = value;
            }
        }

        private static bool GetData(string code)
        {
            bool result = false;
            try
            {
                result = (isSplit == code);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
