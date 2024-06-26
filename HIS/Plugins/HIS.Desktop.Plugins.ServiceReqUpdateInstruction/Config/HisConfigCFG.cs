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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Config
{
    public class HisConfigCFG
    {
        internal const string CONFIG_KEY__StartTimeMustBeGreaterThanInstructionTime = "HIS.Desktop.Plugins.StartTimeMustBeGreaterThanInstructionTime";
        internal const string CONFIG_KEY__MOS_PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string KEY__InstructionTimeServiceMustBeGreaterThanStartTimeExam = "HIS.Desktop.Plugins.InstructionTimeServiceMustBeGreaterThanStartTimeExam";

        internal static string InstructionTimeServiceMustBeGreaterThanStartTimeExam
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__InstructionTimeServiceMustBeGreaterThanStartTimeExam);
            }
        }

        internal static string StartTimeMustBeGreaterThanInstructionTime
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__StartTimeMustBeGreaterThanInstructionTime);
            }
        }
        internal static string MOS_PATIENT_TYPE_CODE__BHYT
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__MOS_PATIENT_TYPE_CODE__BHYT);
            }
        }
    }
}
