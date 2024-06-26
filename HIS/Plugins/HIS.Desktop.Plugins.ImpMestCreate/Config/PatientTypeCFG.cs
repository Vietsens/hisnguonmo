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
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.Config
{
    class PatientTypeCFG
    {
        private static long patientTypeIdIsHein;
        public static long PATIENT_TYPE_ID__BHYT
        {
            get
            {
                if (patientTypeIdIsHein == 0)
                {
                    patientTypeIdIsHein = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
                }
                return patientTypeIdIsHein;
            }
            set
            {
                patientTypeIdIsHein = value;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }
    }
}
