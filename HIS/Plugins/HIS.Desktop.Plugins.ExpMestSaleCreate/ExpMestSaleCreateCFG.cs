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
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    class ExpMestSaleCreateCFG
    {

        public const string PatientTypeCodeDefault = "HIS.Desktop.Plugins.ExpMestSaleCreate.PatientTypeCodeDefault";

        private static long patientID;
        public static long patientTypeIdByCode
        {
            get
            {
                if (patientID <= 0)
                {
                    patientID = GetId(SdaConfigs.Get<string>(PatientTypeCodeDefault));
                }
                return patientID;
            }
        }


        private static long GetId(string patientTypeCode)
        {
            long result = 0;
            try
            {
                var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == patientTypeCode);
                if (patientType != null)
                {
                    result = patientType.ID;
                }
                if (result == 0) throw new NullReferenceException(patientTypeCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
