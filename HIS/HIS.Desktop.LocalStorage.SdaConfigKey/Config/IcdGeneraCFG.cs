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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;
using SDA.Filter;
using Inventec.Common.LocalStorage.SdaConfig;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class IcdGeneraCFG
    {
        private const string ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";

        private static string autoCheckIcd;
        public static string AutoCheckIcd
        {
            get
            {
                if (string.IsNullOrEmpty(autoCheckIcd))
                {
                    autoCheckIcd = GetCode(ICD_GENERA_KEY);
                }
                return autoCheckIcd;
            }
            set
            {
                autoCheckIcd = value;
            }
        }

        private static string GetCode(string code)
        {
            string result = "";
            try
            {
                result = SdaConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }
}
