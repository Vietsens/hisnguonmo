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

namespace MPS.Processor.Mps000260.PDO
{
    public partial class Mps000260PDO : RDOBase
    {
        public List<HIS_PATIENT_TYPE_ALTER> PatyAlters { get; set; }
        public List<V_HIS_DEPARTMENT_TRAN> DepartmentTrans { get; set; }
        public List<V_HIS_TREATMENT_FEE> TreatmentFees { get; set; }
        public List<HIS_SERE_SERV> SereServs { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
    }

    public class PatientADO
    {
        public string VIR_ADDRESS { get; set; }
        public string VIR_PATIENT_NAME { get; set; }
        public long DOB { get; set; }
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }
        public string GENDER_NAME { get; set; }
    }

    public class PatyAlterBhytADO : HIS_PATIENT_TYPE_ALTER
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
        public string RATIO_STR { get; set; }
    }

    public class SereServKey : HIS_SERE_SERV
    {
        public long SERVICE_TYPE_ID { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public long? HEIN_SERVICE_TYPE_ID { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public string HEIN_SERVICE_TYPE_CODE { get; set; }
        public long? HEIN_SERVICE_TYPE_NUM_ORDER { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public long? NUMBER_OF_FILM { get; set; }

        public int ROW_POS { get; set; }
        public decimal PRICE_BHYT { get; set; }
        public decimal TOTAL_PRICE_BHYT { get; set; }
        public decimal? TOTAL_PRICE_OTHER { get; set; }
        public decimal TOTAL_PRICE_PATIENT_SELF { get; set; }
        public decimal RADIO_SERIVCE { get; set; }
        public decimal? TOTAL_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_HEIN_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_PRICE_ROOM { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_ROOM { get; set; }
        public decimal? TOTAL_HEIN_PRICE_ROOM { get; set; }
        public decimal? TOTAL_HEIN_PRICE_ONE_AMOUNT { get; set; }
        public decimal? PRICE_CO_PAYMENT { get; set; }

        public long? MEDICINE_LINE_ID { get; set; }
        public string MEDICINE_LINE_CODE { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
    }

    public class HeinServiceTypeADO
    {
        public long? ID { get; set; }
        public int ROW_POS { get; set; }
        public string HEIN_SERVICE_TYPE_CODE { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public decimal? TOTAL_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_BHYT_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PRICE_OTHER { get; set; }
    }

    public class MedicineLineADO
    {
        public long? ID { get; set; }
        public int ROW_POS { get; set; }
        public string MEDICINE_LINE_CODE { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
        public decimal? TOTAL_PRICE_MEDICINE_LINE { get; set; }
        public decimal? TOTAL_HEIN_PRICE_MEDICINE_LINE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_MEDICINE_LINE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_SELF_MEDICINE_LINE { get; set; }
        public long? HEIN_SERVICE_TYPE_ID { get; set; }
        public long? REMEDY_COUNT { get; set; }
    }
}
