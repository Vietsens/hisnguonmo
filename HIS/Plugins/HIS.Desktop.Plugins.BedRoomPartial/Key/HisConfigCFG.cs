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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedRoomPartial.Key
{
    class HisConfigCFG
    {
        private const string IS_ShowResultWhenReqComplete = "HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete";
        internal static string IsShowResultWhenReqComplete
        {
            get
            {
                var ptBHYT = HisConfigs.Get<string>(IS_ShowResultWhenReqComplete);
                return ptBHYT;
            }
        }
        internal static long PatientTypeId__BHYT
        {
            get
            {
                var ptBHYT = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE == HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).FirstOrDefault();
                return ptBHYT != null ? ptBHYT.ID : 0;
            }
        }

        internal static string PatientTypeCode__BHYT
        {
            get
            {
                var ptBHYT = HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                return ptBHYT;
            }
        }

        internal static string PatientTypeCode__VP
        {
            get
            {
                var ptVP = HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                return ptVP;
            }
        }
    }
}
