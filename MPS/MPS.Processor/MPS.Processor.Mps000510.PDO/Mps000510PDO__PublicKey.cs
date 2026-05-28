/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000510.PDO
{
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
        public string KEY { get; set; }
        public string KBCB_TIME_FROM_STR { get; set; }
        public string KBCB_TIME_TO_STR { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal? TOTAL_PRICE_BHYT { get; set; }
        public decimal? TOTAL_PRICE_HEIN { get; set; }
        public decimal? TOTAL_PRICE_OTHER { get; set; }
        public decimal? TOTAL_PRICE_PATIENT { get; set; }
        public decimal? TOTAL_PRICE_PATIENT_SELF { get; set; }

        public decimal TOTAL_PATIENT_PRICE_LEFT { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
    }

    public class HeinServiceTypeADO
    {
        public long? ID { get; set; }
        public string HEIN_SERVICE_TYPE_CODE { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public decimal? NUM_ORDER { get; set; }
        public long? PARENT_ID { get; set; }
        public long? MEDICINE_LINE_ID { get; set; }

        public decimal? TOTAL_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE { get; set; }
        public decimal? OTHER_SOURCE_PRICE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_LEFT { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
    }

    public class MedicineLineADO
    {
        public long? ID { get; set; }
        public string MEDICINE_LINE_CODE { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
        public long? HEIN_SERVICE_TYPE_ID { get; set; }
        public long? REMEDY_COUNT { get; set; }
    }
}
