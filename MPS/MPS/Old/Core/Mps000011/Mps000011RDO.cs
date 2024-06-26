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

namespace MPS.Core.Mps000011
{
    /// <summary>
    /// In Giay Hẹn khám.
    /// </summary>
    public class Mps000011RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentTreatment { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati { get; set; }
        internal List<TranPatiReasonADO> tranpatiReasonADOs { get; set; }
        internal List<HIS_TRAN_PATI_FORM> tranPatiForms { get; set; }
        public Mps000011RDO(
            PatientADO Patient,
            PatyAlterBhytADO PatyAlterBhyt,
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentTreatment,
            MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran,
            MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati,
            List<TranPatiReasonADO> tranpatiReasonADOs,
            List<HIS_TRAN_PATI_FORM> tranPatiForms
            )
        {
            try
            {
                this.Patient = Patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentTreatment = currentTreatment;
                this.departmentTran = departmentTran;
                this.hisTranPati = hisTranPati;
                this.tranpatiReasonADOs = tranpatiReasonADOs;
                this.tranPatiForms = tranPatiForms;

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
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.HEIN_CARD_NUMBER, PatyAlterBhyt.HEIN_CARD_NUMBER));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatyAlterBhyt.HEIN_CARD_FROM_TIME)));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatyAlterBhyt.HEIN_CARD_TO_TIME)));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.HEIN_MEDI_ORG_CODE, PatyAlterBhyt.HEIN_MEDI_ORG_CODE));
                }

                if (currentTreatment != null)
                {
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.IN_TIME)));
                    if (currentTreatment.OUT_TIME.HasValue)
                        keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.OUT_TIME.Value)));

                }

                if (hisTranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.MEDI_ORG_TO_NAME, hisTranPati.MEDI_ORG_NAME));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.DAU_HIEU_LAM_SANG, hisTranPati.CLINICAL_NOTE));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.XET_NGHIEM, hisTranPati.SUBCLINICAL_RESULT));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.THUOC_DA_DUNG, hisTranPati.TREATMENT_METHOD));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.HUONG_DIEU_TRI, hisTranPati.TREATMENT_DIRECTION));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.TINH_TRANG, hisTranPati.PATIENT_CONDITION));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.PHUONG_TIEN_CHUYEN, hisTranPati.TRANSPORT_VEHICLE));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.NGUOI_HO_TONG, hisTranPati.TRANSPORTER));
                    keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.OUT_ORDER, hisTranPati.OUT_ORDER));

                    if (hisTranPati.TRAN_PATI_TYPE_ID == 2)
                    {
                        keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.OUT_CODE, hisTranPati.OUT_CODE));
                    }
                    else
                    {
                        keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.OUT_CODE, ""));
                    }

                    if (tranPatiForms != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM tranPatiForm = tranPatiForms.FirstOrDefault();
                        keyValues.Add(new KeyValue(Mps000011ExtendSingleKey.HINH_THUC_CHUYEN, tranPatiForm != null ? tranPatiForm.TRAN_PATI_FORM_NAME : ""));
                    }

                }


                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
                //GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(hisTranPati, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentTreatment, keyValues, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
