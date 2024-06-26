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
using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.EmrGenerate
{
    class HisConfigCFG
    {
        public const string CONFIG_KEY__HIS_DESKTOP_PLUGINS_TRACKING_IS_PRINT_MERGE = "HIS.Desktop.Plugins.Tracking.IsPrintMerge";
        private const string CONFIG_KEY__IsNotShowingSignInformation = "EMR.Desktop.DocumentViewer.IsNotShowingSignInformation";
        private const string CONFIG_KEY__EMR_SIGN_TYPE = "HIS.Desktop.Plugins.Library.EmrGenerate.SignType";
        public static int emrSignType;
        internal static int EmrSignType
        {
            get
            {
                if (emrSignType == 0)
                {
                    emrSignType = Inventec.Common.TypeConvert.Parse.ToInt32(GetValue(CONFIG_KEY__EMR_SIGN_TYPE));
                }
                return emrSignType;
            }
        }

        internal static bool IsNotShowingSignInformation
        {
            get
            {
                return GetValue(CONFIG_KEY__IsNotShowingSignInformation) == "1";
            }
        }

        private static string GetValue(string key)
        {
            try
            {
                return HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }
    }
}
