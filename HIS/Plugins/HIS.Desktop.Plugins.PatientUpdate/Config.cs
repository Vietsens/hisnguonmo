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

namespace HIS.Desktop.Plugins.PatientUpdate
{
	class Config
	{
        private const string AllowEditBloodGroupRhOption = "HIS.Desktop.Plugins.PatientUpdate.AllowEditBloodGroupRh";
        public static bool AllowEditBloodGroupRh
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AllowEditBloodGroupRhOption) == "1";
            }
        }
        private const string IsPatientClassifyOption = "HIS.Desktop.Plugins.PatientClassifyOption";
        public static bool IsPatientClassify
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(IsPatientClassifyOption) == "1";
            }
        }

        private const string IsCheckMimsPregnancyLactationOption = "HIS.Desktop.Mims.IsCheckPregnancyLactation";
        /// <summary>
        /// "1" = hiện checklist PN mang thai / cho con bú (MIMS Drug Pregnancy/Lactation).
        /// </summary>
        public static bool IsCheckMimsPregnancyLactation
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(IsCheckMimsPregnancyLactationOption) == "1";
            }
        }

        private const string ChronicChangeTreatmentTypeOption = "MOS.HIS_TREATMENT.FINISH.CHRONIC_CHANGE_TREATMENT_TYPE_OPTION";
        /// <summary>
        /// "1" = cờ mãn tính theo đợt điều trị (việc 54036/58919): ô "BN mãn tính" hiển thị và ghi theo hồ sơ điều trị đích,
        /// không ghi cờ bệnh nhân.
        /// </summary>
        public static bool IsChronicChangeTreatmentType
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ChronicChangeTreatmentTypeOption) == "1";
            }
        }
    }
}
