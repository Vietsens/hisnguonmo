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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportPatient.ADO
{
    public class ImportPatientADO 
    {

        public bool IsFalseAfterSave { get; set; }
        public bool IsDobFalse { get; set; }

        public bool IsDateCmndFalse { get; set; }
        public bool IsDateInstructionFalse { get; set; }
        public long RowId { get; set; }    
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string VIR_PATIENT_NAME { get; set; }

        public long GENDER_ID { get; set; }
        public string GENDER_CODE { get; set; }     
        public string GENDER_NAME { get; set; }

        public bool IS_HAS_NOT_DAY_DOB { get; set; }

        public long? DOB { get; set; }
        public string DOB_DATE { get; set; }
        public string DOB_YEAR { get; set; }

        public string CAREER_CODE { get; set; }
        public string CAREER_NAME { get; set; }


        public long PATIENT_TYPE_ID { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public string PHONE { get; set; }

        public string ERROR { get; set; }

        public string PROVINCE_CODE { get; set; }
        public string PROVINCE_NAME { get; set; }
        public string DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string COMMUNE_CODE { get; set; }
        public string COMMUNE_NAME { get; set; }
        public string ADDRESS { get; set; }

        public string FATHER_NAME { get; set; }
        public string MOTHER_NAME { get; set; }
        public string RELATIVE_NAME { get; set; }
        public string RELATIVE_TYPE { get; set; }
        public string RELATIVE_CMND_NUMBER { get; set; }
        public string RELATIVE_PHONE { get; set; }
        public string RELATIVE_ADDRESS { get; set; }
        public string WORK_PLACE { get; set; }

        public string ETHNIC_CODE { get; set; }
        public string ETHNIC_NAME { get; set; }
        public string NATIONAL_CODE { get; set; }
        public string NATIONAL_NAME { get; set; }

        public bool IsCccd { get; set; }
        public bool IsCmnd { get; set; }
        public bool IsHC { get; set; }
        public string CMND { get; set; }

        public long RELEASE_CMCCHC_DATE_VALUE { get; set; }
        public string RELEASE_CMCCHC_DATE { get; set; }
        public string CMCCHC_PLACE { get; set; }


        private  List<ServiceReqDetailSDO> lstServiceReqDetailSDO { get; set; }

        public void SetListServiceDetailSDO(List<ServiceReqDetailSDO> lstServiceReqDetailSDO)
        {
            this.lstServiceReqDetailSDO = lstServiceReqDetailSDO;
        }

        public List<ServiceReqDetailSDO> GetListServiceDetailSDO()
        {
            return this.lstServiceReqDetailSDO;
        }

        public bool IsExamnation { get; set; }
        private  List<long> LIST_SERVICE_ID { get; set; }

        public void SetListService(List<long> LIST_SERVICE_ID)
		{
            this.LIST_SERVICE_CLS_ID = LIST_SERVICE_ID;
		}

        public List<long> GetListService()
        {
             return this.LIST_SERVICE_CLS_ID;
        }

        private  List<long> LIST_SERVICE_CLS_ID { get; set; }

        public void SetListServiceCLS(List<long> LIST_SERVICE_CLS_ID)
        {
            this.LIST_SERVICE_CLS_ID = LIST_SERVICE_CLS_ID;
        }

        public List<long> GetListServiceCLS()
        {
            return this.LIST_SERVICE_CLS_ID;
        }
        public long SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }

        public string AMOUNT { get; set; }
        public long? ROOM_ID { get; set; }
        public string ROOM_CODE { get; set; }
        public string ROOM_NAME { get; set; }
        public string SERVICE_CLS_CODE { get; set; }
        public string SERVICE_CLS_NAME { get; set; }
        public long PATIENT_TYPE_DTTT_ID { get; set; }
        public string PATIENT_TYPE_CODE_DTTT { get; set; }
        public string PATIENT_TYPE_NAME_DTTT { get; set; }
        public long? DATE_INSTRUCTION { get; set; }

        public string DATE_INSTRUCTION_STR { get; set; }

        public string PRIORITY_TYPE_CODE { get; set; }
        public long? PRIORITY_TYPE_ID { get; set; }
        public string PRIORITY_TYPE_NAME { get; set; }
        public string IS_PRIORITY { get; set; }
        public string IS_EMERGENCY { get; set; }
        public string IS_NOT_REQUIRE_FEE { get; set; }
        public string IS_CHRONIC { get; set; }
        public string IS_TUBERCULOSIS { get; set; }

    }
}
