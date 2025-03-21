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
using MOS.SDO;

namespace HIS.UC.PlusInfo.ADO
{
    public class UCPlusInfoADO
    {
        public UCPlusInfoADO() { }

        public string ETHNIC_NAME { get; set; }
        public string ETHNIC_CODE { get; set; }
        public long? MILITARYRANK_ID { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string NATIONAL_CODE { get; set; }
        public string PROGRAM_CODE { get; set; }
        public long PROGRAM_ID { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string EMAIL { get; set; }
        public object workPlace { get; set; }
        public long PATIENT_ID { get; set; }
        public string PROVINCE_OfBIRTH_CODE { get; set; }
        public string PROVINCE_OfBIRTH_NAME { get; set; }
        public string ADDRESS_OfBIRTH { get; set; }
        public string COMMUNE_OfBIRTH_NAME { get; set; }
        public string COMMUNE_OfBIRTH_CODE { get; set; }
        public string DISTRICT_OfBIRTH_NAME { get; set; }
        public string DISTRICT_OfBIRTH_CODE { get; set; }

        public long? BLOOD_ABO_ID { get; set; }
        public string BLOOD_ABO_CODE { get; set; }
        public long? BLOOD_RH_ID { get; set; }
        public string BLOOD_RH_CODE { get; set; }

        public long? CMND_DATE { get; set; }
        public string CMND_NUMBER { get; set; }
        public string PASSPORT_NUMBER { get; set; }
        public string CCCD_NUMBER { get; set; }
        public string CMND_PLACE { get; set; }
        public string FATHER_NAME { get; set; }
        public string MOTHER_NAME { get; set; }

        public string HOUSEHOLD_CODE { get; set; }
        public string HOH_NAME { get; set; }
        public string HOUSEHOLD_RELATION_NAME { get; set; }
        public long? HOUSEHOLD_RELATION_ID { get; set; }
        public string HONGHEO_CODE {get;set;}
        public string HT_ADDRESS { get; set; }
        public string HT_COMMUNE_NAME { get; set; }
        public string HT_COMMUNE_CODE { get; set; }
        public string HT_DISTRICT_NAME { get; set; }
        public string HT_DISTRICT_CODE { get; set; }
        public string HT_PROVINCE_NAME { get; set; }
        public string HT_PROVINCE_CODE { get; set; }

        public string PATIENT_STORE_CODE { get; set; }

        public string HRM_KSK_CODE { get; set; }

        public string TAX_CODE { get; set; }
        public string MPS_NATIONAL_CODE { get; set; }
        public bool IsCheckBoxCCCD { get; set; }
        public WorkPlaceADO workPlaceADO { get; set; }
    }
}
