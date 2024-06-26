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
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000224.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000224
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
                    patientADO.GENDER_NAME = treatment.TDL_PATIENT_GENDER_NAME;
                    patientADO.VIR_ADDRESS = treatment.TDL_PATIENT_ADDRESS;
                    patientADO.DOB = treatment.TDL_PATIENT_DOB;
                    patientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB);
                    patientADO.AGE = AgeUtil.CalculateFullAge(patientADO.DOB);
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

        public static List<PatyAlterBhytADO> PatyAlterBHYTRawToADOs(List<HIS_PATIENT_TYPE_ALTER> patyAlters, HIS_BRANCH branch, List<HIS_TREATMENT_TYPE> treatmentTypes)
        {
            List<PatyAlterBhytADO> patyAlterBhytADOs = new List<PatyAlterBhytADO>();
            try
            {
                if (patyAlters != null && patyAlters.Count > 0)
                {
                    foreach (var item in patyAlters)
                    {
                        PatyAlterBhytADO patyAlterBhytADO = new PatyAlterBhytADO();
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, PatyAlterBhytADO>();
                        patyAlterBhytADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, PatyAlterBhytADO>(item);

                        patyAlterBhytADO.HEIN_CARD_NUMBER_SEPARATE = SetHeinCardNumberDisplayByNumber(item.HEIN_CARD_NUMBER);
                        patyAlterBhytADO.HEIN_MEDI_ORG_CODE = item.HEIN_MEDI_ORG_CODE;
                        patyAlterBhytADO.HEIN_MEDI_ORG_NAME = item.HEIN_MEDI_ORG_NAME;
                        if (!String.IsNullOrEmpty(item.HEIN_CARD_NUMBER))
                        {
                            patyAlterBhytADO.HEIN_CARD_NUMBER_1 = item.HEIN_CARD_NUMBER.Substring(0, 2);
                            patyAlterBhytADO.HEIN_CARD_NUMBER_2 = item.HEIN_CARD_NUMBER.Substring(2, 1);
                            patyAlterBhytADO.HEIN_CARD_NUMBER_3 = item.HEIN_CARD_NUMBER.Substring(3, 2);
                            patyAlterBhytADO.HEIN_CARD_NUMBER_4 = item.HEIN_CARD_NUMBER.Substring(5, 2);
                            patyAlterBhytADO.HEIN_CARD_NUMBER_5 = item.HEIN_CARD_NUMBER.Substring(7, 3);
                            patyAlterBhytADO.HEIN_CARD_NUMBER_6 = item.HEIN_CARD_NUMBER.Substring(10, 5);
                        }
                        patyAlterBhytADO.STR_HEIN_CARD_FROM_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((item.HEIN_CARD_FROM_TIME ?? 0));
                        patyAlterBhytADO.STR_HEIN_CARD_TO_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString((item.HEIN_CARD_TO_TIME ?? 0));

                        //Mức hưởng
                        HIS_TREATMENT_TYPE treatmentType = treatmentTypes.FirstOrDefault(o=>o.ID == item.TREATMENT_TYPE_ID);
                        if (treatmentType==null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Không tìm thấy treatmentType của thẻ " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patyAlterBhytADO), patyAlterBhytADO));
                            continue;
                        }

                        patyAlterBhytADO.RATIO_STR = GetDefaultHeinRatioForView(patyAlterBhytADO.HEIN_CARD_NUMBER, treatmentType.HEIN_TREATMENT_TYPE_CODE, branch.HEIN_LEVEL_CODE, patyAlterBhytADO.RIGHT_ROUTE_CODE);
                        patyAlterBhytADOs.Add(patyAlterBhytADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                patyAlterBhytADOs = null;
            }
            return patyAlterBhytADOs;
        }

        public static string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((int)((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100)) + "%";
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
                LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }
    }
}
