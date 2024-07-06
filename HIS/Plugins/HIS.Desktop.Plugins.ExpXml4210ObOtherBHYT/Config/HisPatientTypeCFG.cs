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
using System;
using System.Linq;
using System.Collections.Generic;
using Inventec.Common.LocalStorage.SdaConfig;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ExpXml4210ObOtherBHYT.Config
{
    public class HisPatientTypeCFG
    {
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT

        private static long patientTypeIdIsHein;
        public static long PATIENT_TYPE_ID__BHYT
        {
            get
            {
                if (patientTypeIdIsHein == 0)
                {
                    patientTypeIdIsHein = GetId(HisConfigCFG.GetValue(SDA_CONFIG__PATIENT_TYPE_CODE__BHYT));
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