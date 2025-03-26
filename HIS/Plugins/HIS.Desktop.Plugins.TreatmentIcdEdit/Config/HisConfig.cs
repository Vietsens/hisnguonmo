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

namespace HIS.Desktop.Plugins.TreatmentIcdEdit
{
    class HisConfig
    {
        const string IS__TRUE = "1";

        private const string DefaultCheckedCheckboxIssueOutPatientStoreCodeSTR = "HIS.Desktop.Plugins.TreatmentFinish.DefaultCheckedCheckboxCreateOutPatientMediRecord";
        private const string EnableCheckboxIssueOutPatientStoreCodeSTR = "HIS.Desktop.Plugins.TreatmentFinish.EnableCheckboxCreateOutPatientMediRecord";
        private const string KEY_IsCheckSubIcdExceedLimit = "HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit";

        private const string checkSovaovien = "MOS.HIS_TREATMENT.IS_MANUAL_IN_CODE";

        private const string SuaThongTinHoSoDieuTri = "MOS.HIS_TREATMENT.UPDATE_INFO_OPTION";


        internal static bool checkSovaovien_;
        internal static bool IsCheckedCheckboxIssueOutPatientStoreCode;
        internal static bool IsEnableCheckboxIssueOutPatientStoreCode;
        internal static bool SuaThongTinHoSoDieuTri_;

        internal static string IsCheckSubIcdExceedLimit;

        internal static void GetConfig()
        {
            try
            {
                IsCheckedCheckboxIssueOutPatientStoreCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(DefaultCheckedCheckboxIssueOutPatientStoreCodeSTR) == IS__TRUE;
                IsEnableCheckboxIssueOutPatientStoreCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(EnableCheckboxIssueOutPatientStoreCodeSTR) == IS__TRUE;

                IsCheckSubIcdExceedLimit = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_IsCheckSubIcdExceedLimit);

                checkSovaovien_ = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(checkSovaovien) == IS__TRUE;
                SuaThongTinHoSoDieuTri_ = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SuaThongTinHoSoDieuTri) == IS__TRUE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
