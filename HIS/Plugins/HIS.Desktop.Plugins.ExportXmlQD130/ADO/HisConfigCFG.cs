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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
{
    class HisConfigCFG
    {
        internal const string HIS_QD_130_BYT__CONNECTION_INFO = "HIS.QD_130_BYT.CONNECTION_INFO";
        internal const string MOS_BHXH__XML_SIGN_OPTION = "MOS.BHXH.XML_SIGN.OPTION";

        internal static string QD_130_BYT__CONNECTION_INFO;
        internal static string BHXH__XML_SIGN_OPTION;

        internal static void LoadConfig()
        {
            try
            {
                QD_130_BYT__CONNECTION_INFO = GetValue(HIS_QD_130_BYT__CONNECTION_INFO);
                BHXH__XML_SIGN_OPTION = GetValue(MOS_BHXH__XML_SIGN_OPTION);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
