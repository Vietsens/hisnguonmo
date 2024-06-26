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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000082.PDO
{
    public partial class Mps000082PDO : RDOBase
    {
        public PatyAlterBhytADO patyAlterBhytADO { get; set; }
        public V_HIS_TREATMENT currentHisTreatment;
        public PatientADO patientADO { get; set; }
    }

    public class PatientADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT
    {
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }

        public PatientADO()
        {

        }

        public PatientADO(V_HIS_PATIENT data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_PATIENT>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(data));
                    }

                    this.AGE = AgeUtil.CalculateFullAge(this.DOB);
                    this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.DOB);
                    string temp = this.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        this.DOB_YEAR = temp.Substring(0, 4);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class TreatmentADO : MOS.EFMODEL.DataModels.V_HIS_TREATMENT
    {
        public string LOCK_TIME_STR { get; set; }
    }

    public class PatyAlterBhytADO : V_HIS_PATY_ALTER_BHYT
    {
        public string PATIENT_TYPE_NAME { get; set; }
        public string HEIN_CARD_NUMBER_SEPARATE { get; set; }
        public string IS_HEIN { get; set; }
        public string IS_VIENPHI { get; set; }
        public string STR_HEIN_CARD_FROM_TIME { get; set; }
        public string STR_HEIN_CARD_TO_TIME { get; set; }
        public string RATIO { get; set; }
        public string HEIN_CARD_NUMBER_1 { get; set; }
        public string HEIN_CARD_NUMBER_2 { get; set; }
        public string HEIN_CARD_NUMBER_3 { get; set; }
        public string HEIN_CARD_NUMBER_4 { get; set; }
        public string HEIN_CARD_NUMBER_5 { get; set; }
        public string HEIN_CARD_NUMBER_6 { get; set; }
        public long TIME_IN_TREATMENT { get; set; }
    }

    public class SereServGroupPlusADO : V_HIS_SERE_SERV
    {
        public string KTC_FEE_GROUP_NAME { get; set; }
        public decimal TOTAL_PRICE_GROUP { get; set; }
        public string EXECUTE_GROUP_NAME { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public decimal TOTAL_PRICE_SERVICE_GROUP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_SERVICE_GROUP { get; set; }
        public decimal TOTAL_HEIN_PRICE_SERVICE_GROUP { get; set; }

        public decimal TOTAL_PRICE_DEPARTMENT_GROUP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP { get; set; }
        public decimal TOTAL_HEIN_PRICE_DEPARTMENT_GROUP { get; set; }

        public decimal VIR_TOTAL_PRICE_KTC { get; set; }
        public decimal VIR_TOTAL_HEIN_PRICE_SUM { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_SUM { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_SUM_END { get; set; }

        public decimal VIR_TOTAL_PRICE_KTC_HIGHTTECH_GROUP { get; set; }
        public decimal VIR_TOTAL_HEIN_PRICE_HIGHTTECH_GROUP { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_HIGHTTECH_GROUP { get; set; }

        public decimal ROW_POS { get; set; }
        public decimal PRICE_BHYT { get; set; }
        public decimal PRICE_POLICY { get; set; }

        public long DEPARTMENT__GROUP_SERE_SERV { get; set; }
        public long DEPARTMENT__GROUP_SERVICE_REPORT { get; set; }
        public long SERE_SERV__GROUP_SERVICE_REPORT { get; set; }

    }
}
