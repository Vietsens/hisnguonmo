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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000471
{
    class SereServADO : V_HIS_SERE_SERV
    {
        //SereServExt

        public string BACTERIAL_CULTIVATION_RESULT { get; set; }
        public string BED_CODE_LIST { get; set; }
        public long? BED_LOG_ID { get; set; }
        public long? BEGIN_TIME { get; set; }
        public string CONCLUDE { get; set; }
        public string DESCRIPTION { get; set; }
        public string DESCRIPTION_SAR_PRINT_ID { get; set; }
        public string DOC_PROTECTED_LOCATION { get; set; }
        public long? END_TIME { get; set; }
        public long? FILM_SIZE_ID { get; set; }
        public string IMPLANTION_RESULT { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public short? IS_FEE { get; set; }
        public short? IS_GATHER_DATA { get; set; }
        public string JSON_FORM_ID { get; set; }
        public string JSON_PRINT_ID { get; set; }
        public string MACHINE_CODE { get; set; }
        public long? MACHINE_ID { get; set; }
        public string MICROCOPY_RESULT { get; set; }
        public string NOTE { get; set; }
        public long? NUMBER_OF_FAIL_FILM { get; set; }
        public long? NUMBER_OF_FILM { get; set; }
        public long? SAMPLE_TIME { get; set; }
        public long SERE_SERV_ID { get; set; }
        public string SUBCLINICAL_NURSE_LOGINNAME { get; set; }
        public string SUBCLINICAL_NURSE_USERNAME { get; set; }
        public long? SUBCLINICAL_PRES_ID { get; set; }
        public string SUBCLINICAL_PRES_LOGINNAME { get; set; }
        public string SUBCLINICAL_PRES_USERNAME { get; set; }
        public string SUBCLINICAL_RESULT_LOGINNAME { get; set; }
        public string SUBCLINICAL_RESULT_USERNAME { get; set; }
        public long? TDL_SERVICE_REQ_ID { get; set; }
        public string XML_DESCRIPTION_LOCATION { get; set; }

        //ServiceReq
        public string TDL_PATIENT_ADDRESS { get; set; }
        public string TDL_PATIENT_AVATAR_URL { get; set; }
        public string TDL_PATIENT_CAREER_NAME { get; set; }
        public long? TDL_PATIENT_CCCD_DATE { get; set; }
        public string TDL_PATIENT_CCCD_NUMBER { get; set; }
        public string TDL_PATIENT_CCCD_PLACE { get; set; }
        public long? TDL_PATIENT_CLASSIFY_ID { get; set; }
        public long? TDL_PATIENT_CMND_DATE { get; set; }
        public string TDL_PATIENT_CMND_NUMBER { get; set; }
        public string TDL_PATIENT_CMND_PLACE { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public string TDL_PATIENT_COMMUNE_CODE { get; set; }
        public string TDL_PATIENT_COMMUNE_NAME { get; set; }
        public string TDL_PATIENT_DISTRICT_CODE { get; set; }
        public string TDL_PATIENT_DISTRICT_NAME { get; set; }
        public long TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_FIRST_NAME { get; set; }
        public long? TDL_PATIENT_GENDER_ID { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public short? TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public string TDL_PATIENT_LAST_NAME { get; set; }
        public string TDL_PATIENT_MILITARY_RANK_NAME { get; set; }
        public string TDL_PATIENT_MOBILE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_NATIONAL_CODE { get; set; }
        public string TDL_PATIENT_NATIONAL_NAME { get; set; }
        public long? TDL_PATIENT_PASSPORT_DATE { get; set; }
        public string TDL_PATIENT_PASSPORT_NUMBER { get; set; }
        public string TDL_PATIENT_PASSPORT_PLACE { get; set; }
        public string TDL_PATIENT_PHONE { get; set; }
        public long? TDL_PATIENT_POSITION_ID { get; set; }
        public string TDL_PATIENT_PROVINCE_CODE { get; set; }
        public string TDL_PATIENT_PROVINCE_NAME { get; set; }
        public long? TDL_PATIENT_TYPE_ID { get; set; }
        public string TDL_PATIENT_UNSIGNED_NAME { get; set; }
        public string TDL_PATIENT_WORK_PLACE { get; set; }
        public string TDL_PATIENT_WORK_PLACE_NAME { get; set; }

        public SereServADO()
        {
        }


        public SereServADO(V_HIS_SERE_SERV sereServADO, V_HIS_SERVICE_REQ serviceReqADO)
        {
            if (sereServADO != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, sereServADO);
            }
            if (serviceReqADO != null)
            {
                this.TDL_PATIENT_ADDRESS = serviceReqADO.TDL_PATIENT_ADDRESS;
                this.TDL_PATIENT_AVATAR_URL = serviceReqADO.TDL_PATIENT_AVATAR_URL;
                this.TDL_PATIENT_CAREER_NAME = serviceReqADO.TDL_PATIENT_CAREER_NAME;
                this.TDL_PATIENT_CCCD_DATE = serviceReqADO.TDL_PATIENT_CCCD_DATE;
                this.TDL_PATIENT_CCCD_NUMBER = serviceReqADO.TDL_PATIENT_CCCD_NUMBER;
                this.TDL_PATIENT_CCCD_PLACE = serviceReqADO.TDL_PATIENT_CCCD_PLACE;
                this.TDL_PATIENT_CLASSIFY_ID = serviceReqADO.TDL_PATIENT_CLASSIFY_ID;
                this.TDL_PATIENT_CMND_DATE = serviceReqADO.TDL_PATIENT_CMND_DATE;
                this.TDL_PATIENT_CMND_NUMBER = serviceReqADO.TDL_PATIENT_CMND_NUMBER;
                this.TDL_PATIENT_CMND_PLACE = serviceReqADO.TDL_PATIENT_CMND_PLACE;
                this.TDL_PATIENT_CODE = serviceReqADO.TDL_PATIENT_CODE;
                this.TDL_PATIENT_COMMUNE_CODE = serviceReqADO.TDL_PATIENT_COMMUNE_CODE;
                this.TDL_PATIENT_COMMUNE_NAME = serviceReqADO.TDL_PATIENT_COMMUNE_NAME;
                this.TDL_PATIENT_DISTRICT_CODE = serviceReqADO.TDL_PATIENT_DISTRICT_CODE;
                this.TDL_PATIENT_DISTRICT_NAME = serviceReqADO.TDL_PATIENT_DISTRICT_NAME;
                this.TDL_PATIENT_DOB = serviceReqADO.TDL_PATIENT_DOB;
                this.TDL_PATIENT_FIRST_NAME = serviceReqADO.TDL_PATIENT_FIRST_NAME;
                this.TDL_PATIENT_GENDER_ID = serviceReqADO.TDL_PATIENT_GENDER_ID;
                this.TDL_PATIENT_GENDER_NAME = serviceReqADO.TDL_PATIENT_GENDER_NAME;
                this.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = serviceReqADO.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                this.TDL_PATIENT_LAST_NAME = serviceReqADO.TDL_PATIENT_LAST_NAME;
                this.TDL_PATIENT_MILITARY_RANK_NAME = serviceReqADO.TDL_PATIENT_MILITARY_RANK_NAME;
                this.TDL_PATIENT_MOBILE = serviceReqADO.TDL_PATIENT_MOBILE;
                this.TDL_PATIENT_NAME = serviceReqADO.TDL_PATIENT_NAME;
                this.TDL_PATIENT_NATIONAL_CODE = serviceReqADO.TDL_PATIENT_NATIONAL_CODE;
                this.TDL_PATIENT_NATIONAL_NAME = serviceReqADO.TDL_PATIENT_NATIONAL_NAME;
                this.TDL_PATIENT_PASSPORT_DATE = serviceReqADO.TDL_PATIENT_PASSPORT_DATE;
                this.TDL_PATIENT_PASSPORT_NUMBER = serviceReqADO.TDL_PATIENT_PASSPORT_NUMBER;
                this.TDL_PATIENT_PASSPORT_PLACE = serviceReqADO.TDL_PATIENT_PASSPORT_PLACE;
                this.TDL_PATIENT_PHONE = serviceReqADO.TDL_PATIENT_PHONE;
                this.TDL_PATIENT_POSITION_ID = serviceReqADO.TDL_PATIENT_POSITION_ID;
                this.TDL_PATIENT_PROVINCE_CODE = serviceReqADO.TDL_PATIENT_PROVINCE_CODE;
                this.TDL_PATIENT_PROVINCE_NAME = serviceReqADO.TDL_PATIENT_PROVINCE_NAME;
                this.TDL_PATIENT_TYPE_ID = serviceReqADO.TDL_PATIENT_TYPE_ID;
                this.TDL_PATIENT_UNSIGNED_NAME = serviceReqADO.TDL_PATIENT_UNSIGNED_NAME;
                this.TDL_PATIENT_WORK_PLACE = serviceReqADO.TDL_PATIENT_WORK_PLACE;
                this.TDL_PATIENT_WORK_PLACE_NAME = serviceReqADO.TDL_PATIENT_WORK_PLACE_NAME;
            }
        }

        public SereServADO(V_HIS_SERE_SERV sereServADO, HIS_SERE_SERV_EXT sereServExtADO, V_HIS_SERVICE_REQ serviceReqADO)
        {
            if (sereServADO != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, sereServADO);
            }
            if (serviceReqADO != null)
            {
                this.TDL_PATIENT_ADDRESS = serviceReqADO.TDL_PATIENT_ADDRESS;
                this.TDL_PATIENT_AVATAR_URL = serviceReqADO.TDL_PATIENT_AVATAR_URL;
                this.TDL_PATIENT_CAREER_NAME = serviceReqADO.TDL_PATIENT_CAREER_NAME;
                this.TDL_PATIENT_CCCD_DATE = serviceReqADO.TDL_PATIENT_CCCD_DATE;
                this.TDL_PATIENT_CCCD_NUMBER = serviceReqADO.TDL_PATIENT_CCCD_NUMBER;
                this.TDL_PATIENT_CCCD_PLACE = serviceReqADO.TDL_PATIENT_CCCD_PLACE;
                this.TDL_PATIENT_CLASSIFY_ID = serviceReqADO.TDL_PATIENT_CLASSIFY_ID;
                this.TDL_PATIENT_CMND_DATE = serviceReqADO.TDL_PATIENT_CMND_DATE;
                this.TDL_PATIENT_CMND_NUMBER = serviceReqADO.TDL_PATIENT_CMND_NUMBER;
                this.TDL_PATIENT_CMND_PLACE = serviceReqADO.TDL_PATIENT_CMND_PLACE;
                this.TDL_PATIENT_CODE = serviceReqADO.TDL_PATIENT_CODE;
                this.TDL_PATIENT_COMMUNE_CODE = serviceReqADO.TDL_PATIENT_COMMUNE_CODE;
                this.TDL_PATIENT_COMMUNE_NAME = serviceReqADO.TDL_PATIENT_COMMUNE_NAME;
                this.TDL_PATIENT_DISTRICT_CODE = serviceReqADO.TDL_PATIENT_DISTRICT_CODE;
                this.TDL_PATIENT_DISTRICT_NAME = serviceReqADO.TDL_PATIENT_DISTRICT_NAME;
                this.TDL_PATIENT_DOB = serviceReqADO.TDL_PATIENT_DOB;
                this.TDL_PATIENT_FIRST_NAME = serviceReqADO.TDL_PATIENT_FIRST_NAME;
                this.TDL_PATIENT_GENDER_ID = serviceReqADO.TDL_PATIENT_GENDER_ID;
                this.TDL_PATIENT_GENDER_NAME = serviceReqADO.TDL_PATIENT_GENDER_NAME;
                this.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = serviceReqADO.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                this.TDL_PATIENT_LAST_NAME = serviceReqADO.TDL_PATIENT_LAST_NAME;
                this.TDL_PATIENT_MILITARY_RANK_NAME = serviceReqADO.TDL_PATIENT_MILITARY_RANK_NAME;
                this.TDL_PATIENT_MOBILE = serviceReqADO.TDL_PATIENT_MOBILE;
                this.TDL_PATIENT_NAME = serviceReqADO.TDL_PATIENT_NAME;
                this.TDL_PATIENT_NATIONAL_CODE = serviceReqADO.TDL_PATIENT_NATIONAL_CODE;
                this.TDL_PATIENT_NATIONAL_NAME = serviceReqADO.TDL_PATIENT_NATIONAL_NAME;
                this.TDL_PATIENT_PASSPORT_DATE = serviceReqADO.TDL_PATIENT_PASSPORT_DATE;
                this.TDL_PATIENT_PASSPORT_NUMBER = serviceReqADO.TDL_PATIENT_PASSPORT_NUMBER;
                this.TDL_PATIENT_PASSPORT_PLACE = serviceReqADO.TDL_PATIENT_PASSPORT_PLACE;
                this.TDL_PATIENT_PHONE = serviceReqADO.TDL_PATIENT_PHONE;
                this.TDL_PATIENT_POSITION_ID = serviceReqADO.TDL_PATIENT_POSITION_ID;
                this.TDL_PATIENT_PROVINCE_CODE = serviceReqADO.TDL_PATIENT_PROVINCE_CODE;
                this.TDL_PATIENT_PROVINCE_NAME = serviceReqADO.TDL_PATIENT_PROVINCE_NAME;
                this.TDL_PATIENT_TYPE_ID = serviceReqADO.TDL_PATIENT_TYPE_ID;
                this.TDL_PATIENT_UNSIGNED_NAME = serviceReqADO.TDL_PATIENT_UNSIGNED_NAME;
                this.TDL_PATIENT_WORK_PLACE = serviceReqADO.TDL_PATIENT_WORK_PLACE;
                this.TDL_PATIENT_WORK_PLACE_NAME = serviceReqADO.TDL_PATIENT_WORK_PLACE_NAME;
            }
            if (sereServExtADO != null)
            {
                this.BACTERIAL_CULTIVATION_RESULT = sereServExtADO.BACTERIAL_CULTIVATION_RESULT;
                this.BED_CODE_LIST = sereServExtADO.BED_CODE_LIST;
                this.BED_LOG_ID = sereServExtADO.BED_LOG_ID;
                this.BEGIN_TIME = sereServExtADO.BEGIN_TIME;
                this.CONCLUDE = sereServExtADO.CONCLUDE;
                this.DESCRIPTION = sereServExtADO.DESCRIPTION;
                this.DESCRIPTION_SAR_PRINT_ID = sereServExtADO.DESCRIPTION_SAR_PRINT_ID;
                this.DOC_PROTECTED_LOCATION = sereServExtADO.DOC_PROTECTED_LOCATION;
                this.END_TIME = sereServExtADO.END_TIME;
                this.FILM_SIZE_ID = sereServExtADO.FILM_SIZE_ID;
                this.GROUP_CODE = sereServExtADO.GROUP_CODE;
                this.IMPLANTION_RESULT = sereServExtADO.IMPLANTION_RESULT;
                this.INSTRUCTION_NOTE = sereServExtADO.INSTRUCTION_NOTE;
                this.IS_FEE = sereServExtADO.IS_FEE;
                this.IS_GATHER_DATA = sereServExtADO.IS_GATHER_DATA;
                this.JSON_FORM_ID = sereServExtADO.JSON_FORM_ID;
                this.JSON_PRINT_ID = sereServExtADO.JSON_PRINT_ID;
                this.MACHINE_CODE = sereServExtADO.MACHINE_CODE;
                this.MACHINE_ID = sereServExtADO.MACHINE_ID;
                this.MICROCOPY_RESULT = sereServExtADO.MICROCOPY_RESULT;
                this.NOTE = sereServExtADO.NOTE;
                this.NUMBER_OF_FAIL_FILM = sereServExtADO.NUMBER_OF_FAIL_FILM;
                this.NUMBER_OF_FILM = sereServExtADO.NUMBER_OF_FILM;
                this.SAMPLE_TIME = sereServExtADO.SAMPLE_TIME;
                this.SERE_SERV_ID = sereServExtADO.SERE_SERV_ID;
                this.SUBCLINICAL_NURSE_LOGINNAME = sereServExtADO.SUBCLINICAL_NURSE_LOGINNAME;
                this.SUBCLINICAL_NURSE_USERNAME = sereServExtADO.SUBCLINICAL_NURSE_USERNAME;
                this.SUBCLINICAL_PRES_ID = sereServExtADO.SUBCLINICAL_PRES_ID;
                this.SUBCLINICAL_PRES_LOGINNAME = sereServExtADO.SUBCLINICAL_PRES_LOGINNAME;
                this.SUBCLINICAL_PRES_USERNAME = sereServExtADO.SUBCLINICAL_PRES_USERNAME;
                this.SUBCLINICAL_RESULT_LOGINNAME = sereServExtADO.SUBCLINICAL_RESULT_LOGINNAME;
                this.SUBCLINICAL_RESULT_USERNAME = sereServExtADO.SUBCLINICAL_RESULT_USERNAME;
                this.TDL_SERVICE_REQ_ID = sereServExtADO.TDL_SERVICE_REQ_ID;
                this.TDL_TREATMENT_ID = sereServExtADO.TDL_TREATMENT_ID;
                this.XML_DESCRIPTION_LOCATION = sereServExtADO.XML_DESCRIPTION_LOCATION;
            }
        }
    }


}
