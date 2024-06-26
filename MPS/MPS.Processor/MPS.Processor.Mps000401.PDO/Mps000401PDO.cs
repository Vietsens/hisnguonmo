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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000401.PDO
{
    public class Mps000401PDO : RDOBase
    {
        public HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public Mps000401SingleKey _WorkPlaceSDO { get; set; }

        public Mps000401PDO(
           HIS_TREATMENT _treatment,
            V_HIS_PATIENT_TYPE_ALTER _PatientTypeAlter,
            V_HIS_PATIENT _Patient,
           Mps000401SingleKey _workPlaceSDO
           )
        {
            try
            {
                this._Treatment = _treatment;
                this.PatientTypeAlter = _PatientTypeAlter;
                this.Patient = _Patient;
                this._WorkPlaceSDO = _workPlaceSDO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class Mps000401SingleKey : WorkPlaceSDO
    {
        public string LOGIN_NAME { get; set; }
        public string USER_NAME { get; set; }

        public Mps000401SingleKey(WorkPlaceSDO data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Mps000401SingleKey>(this, data);
            }
        }
    }
}
