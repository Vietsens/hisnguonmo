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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PrescriptionAbsentList.Config
{
    class ConfigKeys
    {
        internal const string WAITING_SCREEN__TIMER_FOR_AUTO_LOAD_PATIENTS_KEY = "EXE.WAITING_SCREEN.TIMER_FOR_AUTO_LOAD_PATIENTS";

        internal static long timerForAutoPatients;

        internal static void GetConfig()
        {
            try
            {
                timerForAutoPatients = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(WAITING_SCREEN__TIMER_FOR_AUTO_LOAD_PATIENTS_KEY);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
