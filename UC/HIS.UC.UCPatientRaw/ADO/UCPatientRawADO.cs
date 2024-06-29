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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.DelegateRegister;

namespace HIS.UC.UCPatientRaw.ADO
{
    public class UCPatientRawADO
    {
        public string PATIENT_NAME { get; set; }
        public string PATIENT_LAST_NAME { get; set; }
        public string PATIENT_FIRST_NAME { get; set; }
        public long GENDER_ID { get; set; }
        public long DOB { get; set; }
        public long PATIENTTYPE_ID { get; set; }
        public long? CARRER_ID { get; set; }
        public string CARRER_NAME { get; set; }
        public string CARRER_CODE { get; set; }
        //public long? ETHNIC_ID { get; set; }
        public string ETHNIC_NAME { get; set; }
        public string ETHNIC_CODE { get; set; }
        public string PATIENT_CODE { get; set; }
        public short? IS_HAS_NOT_DAY_DOB { get; set; }
        public string HEIN_CARD_NUMBER { get; set; }
        public string DOB_STR { get; set; }
        public string PERSON_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public long? PRIMARY_PATIENT_TYPE_ID { get; set; }
        public long? TREATMENT_TYPE_ID { get; set; }
        public short? IS_FIND_BY_PATIENT_CODE { get; set; }

        public long? PATIENT_CLASSIFY_ID { get; set; }
        public long? WORK_PLACE_ID { get; set; }
        public long? MILITARY_RANK_ID { get; set; }
        public long? POSITION_ID { get; set; }
        public long PATIENT_ID { get; set; }


        public bool IsReadQrCccd { get; set; }

        public List<string> lstPreviousDebtTreatments { get; set; }
        public long? ReceptionForm { get; set; }
        public string CardCode { get; set; }
        public string CardServiceCode { get; set; }
        public string BankCardCode { get; set; }
        public string SocialInsuranceNumberPatient { get; set; }
    }
}
