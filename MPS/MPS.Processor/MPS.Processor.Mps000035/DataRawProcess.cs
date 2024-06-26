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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000035.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000035
{
    public class DataRawProcess
    {
        public static PatientADO PatientRawToADO(V_HIS_PATIENT patient)
        {
            
            PatientADO patientADO = new PatientADO();
            try
            {
                if (patient != null)
                {
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_PATIENT, PatientADO>();
                    patientADO = AutoMapper.Mapper.Map<V_HIS_PATIENT, PatientADO>(patient);
                    patientADO.VIR_PATIENT_NAME = patient.VIR_PATIENT_NAME;
                    patientADO.VIR_ADDRESS = patient.VIR_ADDRESS;
                    patientADO.DOB = patient.DOB;
                    patientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                    patientADO.AGE = AgeUtil.CalculateFullAge(patientADO.DOB);
                    if (patient.DOB > 0)
                    {
                        patientADO.DOB_YEAR = patient.DOB.ToString().Substring(0, 4);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                patientADO = null;
            }
            return patientADO;
        }
    }
}
