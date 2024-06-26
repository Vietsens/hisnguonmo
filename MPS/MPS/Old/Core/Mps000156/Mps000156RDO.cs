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
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000156
{
    public class Mps000156RDO : RDOBase
    {
        internal List<ExeMediReactADO> expMestMediReact = new List<ExeMediReactADO>();
        V_HIS_TREATMENT_2 currentTreatment = new V_HIS_TREATMENT_2();
        internal V_HIS_ROOM thisRoom = new V_HIS_ROOM();

        public Mps000156RDO(List<ExeMediReactADO> expMestMediReact, V_HIS_TREATMENT_2 currentTreatment, V_HIS_ROOM thisRoom)
        {
            this.expMestMediReact = expMestMediReact;
            this.currentTreatment = currentTreatment;
            this.thisRoom = thisRoom;
        }

        internal override void SetSingleKey()
        {
            try
            {



                if (this.currentTreatment != null)
                {
                    keyValues.Add(new KeyValue(Mps000156ExtendSingleKey.VIR_PATIENT_NAME, this.currentTreatment.VIR_PATIENT_NAME));
                    keyValues.Add(new KeyValue(Mps000156ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(this.currentTreatment.DOB)));
                    keyValues.Add(new KeyValue(Mps000156ExtendSingleKey.GENDER_NAME, this.currentTreatment.GENDER_NAME));
                    keyValues.Add(new KeyValue(Mps000156ExtendSingleKey.VIR_ADDRESS, this.currentTreatment.VIR_ADDRESS));
                    keyValues.Add(new KeyValue(Mps000156ExtendSingleKey.EXECUTE_DEPARTMENT_NAME, this.thisRoom.DEPARTMENT_NAME));
                    keyValues.Add(new KeyValue(Mps000156ExtendSingleKey.EXECUTE_ROOM_NAME, this.thisRoom.ROOM_NAME));
                    keyValues.Add(new KeyValue(Mps000156ExtendSingleKey.ICD_MAIN_TEXT, this.currentTreatment.ICD_MAIN_TEXT));

                }



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
