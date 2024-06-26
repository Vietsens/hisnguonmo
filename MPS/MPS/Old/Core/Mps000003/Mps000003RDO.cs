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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000003
{
    /// <summary>
    /// In bang ke thanh toan ra ngoai tru BHYT Template 8.
    /// </summary>
    public class Mps000003RDO : RDOBase
    {
        internal V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        internal V_HIS_PATIENT Patient { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        internal List<MPS.ADO.SereSrevPrintADO> HisSereServ_Bordereau_Medicines = null;//list Medicine
        internal List<MPS.ADO.SereSrevPrintADO> HisSereServ_Bordereau_Services = null;//list Service
        internal List<MPS.ADO.SereSrevPrintADO> HisSereServ_Bordereau_Materials = null;//list Material
        internal List<MPS.ADO.SereSrevPrintADO> HisSereServ_Bordereau_ExamServices = null;//list exam Service
        internal V_HIS_TREATMENT treatment { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans;
        internal List<V_HIS_SERE_SERV> SereServ2s { get; set; }
        string currentDateSeparateFullTime = "";



        public Mps000003RDO(
            V_HIS_PATIENT patient,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter,
            List<MPS.ADO.SereSrevPrintADO> HisSereServ_Bordereau_Medicines,
            List<MPS.ADO.SereSrevPrintADO> HisSereServ_Bordereau_Services,
            List<MPS.ADO.SereSrevPrintADO> HisSereServ_Bordereau_Materials,
            List<MPS.ADO.SereSrevPrintADO> HisSereServ_Bordereau_ExamServices,
            V_HIS_TREATMENT treatment,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
            List<V_HIS_SERE_SERV> SereServ2s,
            string currentDateSeparateFullTime)
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.PatientTypeAlter = PatientTypeAlter;
                this.HisSereServ_Bordereau_Medicines = HisSereServ_Bordereau_Medicines;
                this.HisSereServ_Bordereau_Services = HisSereServ_Bordereau_Services;
                this.HisSereServ_Bordereau_Materials = HisSereServ_Bordereau_Materials;
                this.HisSereServ_Bordereau_ExamServices = HisSereServ_Bordereau_ExamServices;
                this.treatment = treatment;
                this.departmentTrans = departmentTrans;
                this.SereServ2s = SereServ2s;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                decimal TOTAL_AMOUNT_MEDICINE = 0;
                decimal TOTAL_VIR_PRICE_MEDICINE = 0;
                decimal TOTAL_VIR_TOTAL_PRICE_MEDICINE = 0;
                decimal TOTAL_AMOUNT_SERVICE = 0;
                decimal TOTAL_VIR_PRICE_SERVICE = 0;
                decimal TOTAL_VIR_TOTAL_PRICE_SERVICE = 0;
                decimal TOTAL_AMOUNT_MATERIAL = 0;
                decimal TOTAL_VIR_PRICE_MATERIAL = 0;
                decimal TOTAL_VIR_TOTAL_PRICE_MATERIAL = 0;
                decimal TOTAL_PRICE_EXAM = 0;
                decimal TOTAL_PERCENT = 0;
                decimal TOTAL_PRICE = 0;
                decimal TOTAL_PRICE_PATIENT = 0;
                decimal TOTAL_PRICE_HEIN = 0;



                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT>(Patient, keyValues);
                //Tuoi
                IFormatProvider culture = new CultureInfo("vi-VN", true);

                string age = AgeUtil.CalculateFullAge(Patient.DOB);
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.AGE, age));
                //
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TREATMENT_CODE, treatment.TREATMENT_CODE));

                if (PatyAlterBhyt != null)
                {
                    keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.HEIN_CARD_NUMBER, PatyAlterBhyt.HEIN_CARD_NUMBER));
                    keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                }

                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null)
                    {

                        keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[departmentTrans.Count - 1].LOG_TIME)));
                    }
                }
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.HEIN_MEDI_ORG_NAME, PatyAlterBhyt.HEIN_MEDI_ORG_NAME));
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.HEIN_MEDI_ORG_CODE, PatyAlterBhyt.HEIN_MEDI_ORG_CODE));

                TOTAL_AMOUNT_MEDICINE = HisSereServ_Bordereau_Medicines.Sum(o => o.AMOUNT);
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_AMOUNT_MEDICINE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_AMOUNT_MEDICINE, 0)));

                TOTAL_VIR_PRICE_MEDICINE = HisSereServ_Bordereau_Medicines.Sum(o => o.VIR_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_PRICE_MEDICINE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_PRICE_MEDICINE, 0)));

                TOTAL_VIR_TOTAL_PRICE_MEDICINE = HisSereServ_Bordereau_Medicines.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_TOTAL_PRICE_MEDICINE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_TOTAL_PRICE_MEDICINE, 0)));

                TOTAL_AMOUNT_SERVICE = HisSereServ_Bordereau_Services.Sum(o => o.AMOUNT);
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_AMOUNT_SERVICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_AMOUNT_SERVICE, 0)));

                TOTAL_VIR_PRICE_SERVICE = HisSereServ_Bordereau_Services.Sum(o => o.VIR_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_PRICE_SERVICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_PRICE_SERVICE, 0)));

                TOTAL_VIR_TOTAL_PRICE_SERVICE = HisSereServ_Bordereau_Services.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_TOTAL_PRICE_SERVICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_TOTAL_PRICE_SERVICE, 0)));

                TOTAL_AMOUNT_MATERIAL = HisSereServ_Bordereau_Materials.Sum(o => o.AMOUNT);
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_AMOUNT_MATERIAL, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_AMOUNT_MATERIAL, 0)));
                TOTAL_VIR_PRICE_MATERIAL = HisSereServ_Bordereau_Materials.Sum(o => o.VIR_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_PRICE_MATERIAL, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_PRICE_MATERIAL, 0)));
                TOTAL_VIR_TOTAL_PRICE_MATERIAL = HisSereServ_Bordereau_Materials.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_TOTAL_PRICE_MATERIAL, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_TOTAL_PRICE_MATERIAL, 0)));

                TOTAL_PRICE_EXAM = HisSereServ_Bordereau_ExamServices.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PRICE_EXAM, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PRICE_EXAM, 0)));
                TOTAL_PRICE = SereServ2s.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PRICE, 0)));
                TOTAL_PRICE_HEIN = SereServ2s.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PRICE_HEIN, 0)));
                TOTAL_PRICE_PATIENT = SereServ2s.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PRICE_PATIENT, 0)));
                TOTAL_PERCENT = (TOTAL_PRICE != 0 ? ((TOTAL_PRICE_PATIENT / TOTAL_PRICE) * 100) : 0);
                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PERCENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PERCENT, 0)));

                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, currentDateSeparateFullTime));

                keyValues.Add(new KeyValue(Mps000003ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(treatment, keyValues, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
