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

namespace MPS.Core.Mps000008
{
    /// <summary>
    /// In Giay ra Vien.
    /// </summary>
    public class Mps000008RDO : RDOBase
    {
        internal MOS.EFMODEL.DataModels.V_HIS_TREATMENT_OUT treatmentOut { get; set; }
        internal PatientADO Patient { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentTreatment { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }

        public Mps000008RDO(
            PatientADO Patient,
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT_OUT treatmentOut,
            PatyAlterBhytADO PatyAlterBhyt,
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
                if (treatmentOut != null)
                {
                    keyValues.Add(new KeyValue(Mps000008ExtendSingleKey.METHOD, treatmentOut.TREATMENT_METHOD));
                    keyValues.Add(new KeyValue(Mps000008ExtendSingleKey.ADVISE, treatmentOut.ADVISE));

                }

                if (currentTreatment != null)
                {
                    keyValues.Add(new KeyValue(Mps000008ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.IN_TIME)));
                    if (currentTreatment.OUT_TIME.HasValue)
                        keyValues.Add(new KeyValue(Mps000008ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.OUT_TIME.Value)));

                }

                if (this.PatyAlterBhyt != null && this.departmentTran != null)
                {
                    if (String.IsNullOrEmpty(this.PatyAlterBhyt.TREATMENT_TYPE_NAME))
                    {
                        this.PatyAlterBhyt.TREATMENT_TYPE_NAME = this.departmentTran.TREATMENT_TYPE_NAME;
                    }
                }

                if (this.PatyAlterBhyt != null)
                {
                    keyValues.Add(new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_ADDRESS, this.PatyAlterBhyt.ADDRESS));
                }

                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT_OUT>(treatmentOut, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
