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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000010
{
    /// <summary>
    /// In Giay Hẹn khám.
    /// </summary>
    public class Mps000010RDO : RDOBase
    {
        internal MOS.EFMODEL.DataModels.V_HIS_TREATMENT_OUT treatmentOut { get; set; }
internal MOS.EFMODEL.DataModels.HIS_APPOINTMENT appointment { get; set; }
        internal PatientADO Patient { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentTreatment { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }

        public Mps000010RDO(
            PatientADO Patient,
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT_OUT treatmentOut,
            MOS.EFMODEL.DataModels.HIS_APPOINTMENT appointment,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentTreatment,
            MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran)
        {
            try
            {
                this.Patient = Patient;
                this.treatmentOut = treatmentOut;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.departmentTran = departmentTran;
                this.appointment = appointment;

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
                if (PatyAlterBhyt != null)
                {
                    keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.HEIN_CARD_NUMBER, PatyAlterBhyt.HEIN_CARD_NUMBER));
                    keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatyAlterBhyt.HEIN_CARD_FROM_TIME)));
                    keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatyAlterBhyt.HEIN_CARD_TO_TIME)));
                }
                if (treatmentOut != null)
                {
                    keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.METHOD, treatmentOut.TREATMENT_METHOD));
                    keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.ADVISE, treatmentOut.ADVISE));
                    
                }
                if (appointment != null)
                {
                    keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.DATE_TIME_APPOINT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(appointment.APPOINTMENT_TIME ?? 0)));
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_APPOINTMENT>(appointment, keyValues,false);
                }

                keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.END_ORDER, currentTreatment.END_ORDER));

                if (currentTreatment != null)
                {
                    keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.IN_TIME)));
                    if (currentTreatment.OUT_TIME.HasValue)
                        keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.OUT_TIME.Value)));

                }

                keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                ////Tính tuổi
                //string age = AgeUtil.CalculateFullAge(Patient.DOB);
                //keyValues.Add(new KeyValue(Mps000010ExtendSingleKey.AGE, age));


                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT_OUT>(treatmentOut, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentTreatment, keyValues,false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
