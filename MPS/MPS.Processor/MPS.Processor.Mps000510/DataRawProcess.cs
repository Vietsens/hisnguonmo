/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000510.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000510
{
    public class DataRawProcess
    {
        public static PatientADO PatientRawToADO(V_HIS_TREATMENT treatment)
        {
            PatientADO patientADO = new PatientADO();
            try
            {
                if (treatment != null)
                {
                    patientADO.VIR_PATIENT_NAME = treatment.TDL_PATIENT_NAME;
                    patientADO.VIR_ADDRESS = treatment.TDL_PATIENT_ADDRESS;
                    patientADO.DOB = treatment.TDL_PATIENT_DOB;
                    patientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB);
                    patientADO.AGE = AgeUtil.CalculateFullAge(patientADO.DOB);
                    patientADO.GENDER_NAME = treatment.TDL_PATIENT_GENDER_NAME;
                    if (treatment.TDL_PATIENT_DOB > 0)
                    {
                        patientADO.DOB_YEAR = treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                patientADO = null;
            }
            return patientADO;
        }

        public static PatyAlterBhytADO PatyAlterBHYTRawToADO(V_HIS_PATIENT_TYPE_ALTER patyAlter, List<HIS_PATIENT_TYPE_ALTER> lstPatientTypeAlter, HIS_BRANCH branch, List<HIS_TREATMENT_TYPE> treatmentTypes)
        {
            PatyAlterBhytADO patyAlterBhytADO = new PatyAlterBhytADO();
            try
            {
                if (patyAlter == null)
                {
                    return patyAlterBhytADO;
                }
                Inventec.Common.Mapper.DataObjectMapper.Map<PatyAlterBhytADO>(patyAlterBhytADO, patyAlter);

                patyAlterBhytADO.HEIN_CARD_NUMBER_SEPARATE = SetHeinCardNumberDisplayByNumber(patyAlter.HEIN_CARD_NUMBER);
                patyAlterBhytADO.IS_HEIN = "X";
                patyAlterBhytADO.IS_VIENPHI = "";
                if (!String.IsNullOrEmpty(patyAlter.HEIN_CARD_NUMBER) && patyAlter.HEIN_CARD_NUMBER.Length == 15)
                {
                    patyAlterBhytADO.HEIN_CARD_NUMBER_1 = patyAlter.HEIN_CARD_NUMBER.Substring(0, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_2 = patyAlter.HEIN_CARD_NUMBER.Substring(2, 1);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_3 = patyAlter.HEIN_CARD_NUMBER.Substring(3, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_4 = patyAlter.HEIN_CARD_NUMBER.Substring(5, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_5 = patyAlter.HEIN_CARD_NUMBER.Substring(7, 3);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_6 = patyAlter.HEIN_CARD_NUMBER.Substring(10, 5);
                }
                if (lstPatientTypeAlter != null)
                {
                    patyAlterBhytADO.CO_PAID_ACCUMULATE_AMOUNT = lstPatientTypeAlter.FirstOrDefault(o => o.ID == patyAlter.ID).CO_PAID_ACCUMULATE_AMOUNT;
                }

                if (patyAlter.HEIN_CARD_FROM_TIME.HasValue)
                {
                    patyAlterBhytADO.STR_HEIN_CARD_FROM_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patyAlter.HEIN_CARD_FROM_TIME.Value));
                }
                if (patyAlter.HEIN_CARD_TO_TIME.HasValue)
                {
                    patyAlterBhytADO.STR_HEIN_CARD_TO_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patyAlter.HEIN_CARD_TO_TIME.Value));
                }
                patyAlterBhytADO.RATIO_STR = GetDefaultHeinRatioForView(patyAlter.HEIN_CARD_NUMBER, patyAlter.HEIN_TREATMENT_TYPE_CODE, patyAlter.LEVEL_CODE, patyAlter.RIGHT_ROUTE_CODE, patyAlter.FACILITY_CLASS, patyAlter.FORMER_LEVEL_CODE, (long)(patyAlter.CLASSIFY_POINT ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                patyAlterBhytADO = null;
            }
            return patyAlterBhytADO;
        }

        public static string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode, string facilityClassCode, string formerLevelCode = null, long point = 0)
        {
            string result = "";
            try
            {
                result = ((int)((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode, facilityClassCode, formerLevelCode, point) ?? 0) * 100)) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder()
                        .Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol)
                        .Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol)
                        .Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol)
                        .Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol)
                        .Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol)
                        .Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }
    }
}
