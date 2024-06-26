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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000116
{
    /// <summary>
    /// 
    /// </summary>
    public class Mps000116RDO : RDOBase
    {
        internal PatientADO currentPatientADO { get; set; }
        internal TreatmentADO currentTreatmentADO { get; set; }
        internal List<MedicinesPublicByDateADO> medicinePublicByDates { get; set; }
        string roomName = "";
        string departmentName = "";
        public Mps000116RDO(
            PatientADO currentPatientADO,
            TreatmentADO currentTreatmentADO,
            List<MedicinesPublicByDateADO> medicinePublicByDates,
            string roomName,
            string departmentName
            )
        {
            try
            {
                this.currentPatientADO = currentPatientADO;
                this.currentTreatmentADO = currentTreatmentADO;
                this.medicinePublicByDates = medicinePublicByDates;
                this.roomName = roomName;
                this.departmentName = departmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentTreatmentADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(currentPatientADO, keyValues);
                if (currentTreatmentADO != null)
                {
                    keyValues.Add((new KeyValue(Mps000116ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatmentADO.IN_TIME))));
                    if (currentTreatmentADO.OUT_TIME != null)
                        keyValues.Add((new KeyValue(Mps000116ExtendSingleKey.OUT_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatmentADO.OUT_TIME ?? 0))));
                    if (currentTreatmentADO.CLINICAL_IN_TIME != null)
                        keyValues.Add((new KeyValue(Mps000116ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatmentADO.CLINICAL_IN_TIME ?? 0))));
                }

                keyValues.Add(new KeyValue(Mps000116ExtendSingleKey.REQUEST_BED_ROOM_NAME, roomName));
                keyValues.Add(new KeyValue(Mps000116ExtendSingleKey.REQUEST_DEPARTMENT_NAME, departmentName));
                keyValues.Add(new KeyValue(Mps000116ExtendSingleKey.INTRUCTION_TIME_DAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString((medicinePublicByDates[0].INTRUCTION_TIME))));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
