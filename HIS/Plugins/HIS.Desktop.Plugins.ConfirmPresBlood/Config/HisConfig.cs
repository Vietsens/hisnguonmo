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

namespace HIS.Desktop.Plugins.ConfirmPresBlood.Config
{
    public class HisConfig
    {
        private static string BCS_APPROVE_OTHER_TYPE_IS_ALLOW = "MOS.HIS_EXP_MEST.BCS.APPROVE_OTHER_TYPE.IS_ALLOW";
        private static string BCS_APPROVE_IS_AUTO_REPLACE = "HIS.HIS_EXP_MEST.BCS.APPROVE.IS_AUTO_REPLACE";
        private static string DONT_PRES_EXPIRED_ITEM = "MOS.HIS_MEDI_STOCK.DONT_PRES_EXPIRED_ITEM";

        internal static string IS_ALLOW_REPLACE;
        internal static string IS_AUTO_REPLACE;
        internal static bool IS_DONT_PRES_EXPIRED_ITEM { get; set; }

        internal static void LoadConfig()
        {
            try
            {
                IS_ALLOW_REPLACE = HisConfigs.Get<string>(BCS_APPROVE_OTHER_TYPE_IS_ALLOW);
                IS_AUTO_REPLACE = HisConfigs.Get<string>(BCS_APPROVE_IS_AUTO_REPLACE);
                IS_DONT_PRES_EXPIRED_ITEM = HisConfigs.Get<string>(DONT_PRES_EXPIRED_ITEM) == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
