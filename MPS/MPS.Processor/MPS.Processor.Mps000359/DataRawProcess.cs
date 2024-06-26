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
using MPS.Processor.Mps000359.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000359
{
    class DataRawProcess
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

        public static PatyAlterBhytADO PatyAlterBHYTRawToADO(HIS_PATIENT_TYPE_ALTER patyAlter, List<HIS_PATIENT_TYPE_ALTER> patyAlterAlls, V_HIS_TREATMENT treatment, HIS_BRANCH branch, List<HIS_TREATMENT_TYPE> treatmentTypes, V_HIS_PATIENT_TYPE_ALTER currentPatyAlter)
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
                patyAlterBhytADO.HEIN_MEDI_ORG_CODE = patyAlter.HEIN_MEDI_ORG_CODE;
                patyAlterBhytADO.HEIN_MEDI_ORG_NAME = patyAlter.HEIN_MEDI_ORG_NAME;
                patyAlterBhytADO.IS_HEIN = "X";
                patyAlterBhytADO.IS_VIENPHI = "";
                if (!String.IsNullOrEmpty(patyAlter.HEIN_CARD_NUMBER))
                {
                    patyAlterBhytADO.HEIN_CARD_NUMBER_1 = patyAlter.HEIN_CARD_NUMBER.Substring(0, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_2 = patyAlter.HEIN_CARD_NUMBER.Substring(2, 1);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_3 = patyAlter.HEIN_CARD_NUMBER.Substring(3, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_4 = patyAlter.HEIN_CARD_NUMBER.Substring(5, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_5 = patyAlter.HEIN_CARD_NUMBER.Substring(7, 3);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_6 = patyAlter.HEIN_CARD_NUMBER.Substring(10, 5);
                }

                if (patyAlter.HEIN_CARD_FROM_TIME.HasValue)
                {
                    patyAlterBhytADO.STR_HEIN_CARD_FROM_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patyAlter.HEIN_CARD_FROM_TIME.Value));
                }

                if (patyAlter.HEIN_CARD_TO_TIME.HasValue)
                {
                    patyAlterBhytADO.STR_HEIN_CARD_TO_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patyAlter.HEIN_CARD_TO_TIME.Value));
                }

                //Mức hưởng
                HIS_TREATMENT_TYPE treatmentType = currentPatyAlter != null ? treatmentTypes.FirstOrDefault(o => o.ID == currentPatyAlter.TREATMENT_TYPE_ID) : null;
                if (treatmentType == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Không tìm thấy treatmentType của thẻ " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patyAlterBhytADO), patyAlterBhytADO));
                    return null;
                }

                patyAlterBhytADO.RATIO_STR = GetDefaultHeinRatioForView(patyAlterBhytADO.HEIN_CARD_NUMBER, treatmentType.HEIN_TREATMENT_TYPE_CODE, branch.HEIN_LEVEL_CODE, patyAlterBhytADO.RIGHT_ROUTE_CODE);

                if (patyAlterAlls != null && treatment != null)
                {
                    //kbcb time
                    patyAlterBhytADO.KBCB_TIME_FROM_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(patyAlterBhytADO.LOG_TIME);
                    HIS_PATIENT_TYPE_ALTER patientTypeAlterNext = patyAlterAlls.FirstOrDefault(o =>
                        o.LOG_TIME > patyAlterBhytADO.LOG_TIME
                        &&
                        (o.HEIN_CARD_NUMBER != patyAlterBhytADO.HEIN_CARD_NUMBER
                        || o.HEIN_MEDI_ORG_CODE != patyAlterBhytADO.HEIN_MEDI_ORG_CODE
                        || o.LEVEL_CODE != patyAlterBhytADO.LEVEL_CODE
                        || o.RIGHT_ROUTE_CODE != patyAlterBhytADO.RIGHT_ROUTE_CODE
                        || o.RIGHT_ROUTE_TYPE_CODE != patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE
                        || o.JOIN_5_YEAR != patyAlterBhytADO.JOIN_5_YEAR
                        || o.PAID_6_MONTH != patyAlterBhytADO.PAID_6_MONTH
                        || o.LIVE_AREA_CODE != patyAlterBhytADO.LIVE_AREA_CODE
                        || o.HNCODE != patyAlterBhytADO.HNCODE
                        || o.HEIN_CARD_FROM_TIME != patyAlterBhytADO.HEIN_CARD_FROM_TIME
                        || o.HEIN_CARD_TO_TIME != patyAlterBhytADO.HEIN_CARD_TO_TIME));

                    if (patientTypeAlterNext != null)
                    {
                        patyAlterBhytADO.KBCB_TIME_TO_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(patientTypeAlterNext.LOG_TIME);
                    }
                    else
                    {
                        if (treatment.OUT_TIME.HasValue)
                        {
                            patyAlterBhytADO.KBCB_TIME_TO_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.OUT_TIME.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                patyAlterBhytADO = null;
            }
            return patyAlterBhytADO;
        }

        public static PatyAlterBhytADO PatyAlterBHYTRawToADO(V_HIS_PATIENT_TYPE_ALTER patyAlter, HIS_BRANCH branch, List<HIS_TREATMENT_TYPE> treatmentTypes)
        {
            PatyAlterBhytADO patyAlterBhytADO = new PatyAlterBhytADO();
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<PatyAlterBhytADO>(patyAlterBhytADO, patyAlter);
                patyAlterBhytADO.HEIN_CARD_NUMBER_SEPARATE = SetHeinCardNumberDisplayByNumber(patyAlter.HEIN_CARD_NUMBER);
                patyAlterBhytADO.IS_HEIN = "X";
                patyAlterBhytADO.IS_VIENPHI = "";
                if (!String.IsNullOrEmpty(patyAlter.HEIN_CARD_NUMBER))
                {
                    patyAlterBhytADO.HEIN_CARD_NUMBER_1 = patyAlter.HEIN_CARD_NUMBER.Substring(0, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_2 = patyAlter.HEIN_CARD_NUMBER.Substring(2, 1);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_3 = patyAlter.HEIN_CARD_NUMBER.Substring(3, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_4 = patyAlter.HEIN_CARD_NUMBER.Substring(5, 2);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_5 = patyAlter.HEIN_CARD_NUMBER.Substring(7, 3);
                    patyAlterBhytADO.HEIN_CARD_NUMBER_6 = patyAlter.HEIN_CARD_NUMBER.Substring(10, 5);
                }

                if (patyAlter.HEIN_CARD_FROM_TIME.HasValue)
                {
                    patyAlterBhytADO.STR_HEIN_CARD_FROM_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patyAlter.HEIN_CARD_FROM_TIME.Value));
                }

                if (patyAlter.HEIN_CARD_TO_TIME.HasValue)
                {
                    patyAlterBhytADO.STR_HEIN_CARD_TO_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patyAlter.HEIN_CARD_TO_TIME.Value));
                }

                patyAlterBhytADO.RATIO_STR = GetDefaultHeinRatioForView(patyAlterBhytADO.HEIN_CARD_NUMBER, patyAlter.HEIN_TREATMENT_TYPE_CODE, branch.HEIN_LEVEL_CODE, patyAlterBhytADO.RIGHT_ROUTE_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                patyAlterBhytADO = null;
            }
            return patyAlterBhytADO;
        }

        public static string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((int)((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100)) + "%";
                Inventec.Common.Logging.LogSystem.Error(String.Format("treatmentTypeCode {0} , heinCardNumber {1}, levelCode {2}, rightRouteCode {3} ", treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode));
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
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }
    }
}
