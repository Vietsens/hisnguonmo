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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000003.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000003
{
    class Mps000003Processor : AbstractProcessor
    {
        Mps000003PDO rdo;
        public Mps000003Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000003PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                //objectTag.AddObjectData(store, "ServiceGroups", HeinServiceTypes);
                objectTag.AddObjectData(store, "ServiceMaterials", rdo.HisSereServ_Bordereau_Materials);

                objectTag.AddObjectData(store, "ServiceMedicines", rdo.HisSereServ_Bordereau_Medicines);
                objectTag.AddObjectData(store, "ServiceServices", rdo.HisSereServ_Bordereau_Services);
                objectTag.AddObjectData(store, "ServiceExamServices", rdo.HisSereServ_Bordereau_ExamServices);


                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
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



                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                //Tuoi
                IFormatProvider culture = new CultureInfo("vi-VN", true);

                string age = AgeUtil.CalculateFullAge(rdo.Patient.DOB);
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.AGE, age));
                //
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TREATMENT_CODE, rdo.treatment.TREATMENT_CODE));

                if (rdo.PatyAlterBhyt != null)
                {
                    SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.HEIN_CARD_NUMBER, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME)));
                    SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME)));
                }

                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].LOG_TIME)));
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null)
                    {

                        SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].LOG_TIME)));
                    }
                }
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.HEIN_MEDI_ORG_NAME, rdo.PatyAlterBhyt.HEIN_MEDI_ORG_NAME));
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.PatyAlterBhyt.HEIN_MEDI_ORG_CODE));

                TOTAL_AMOUNT_MEDICINE = rdo.HisSereServ_Bordereau_Medicines.Sum(o => o.AMOUNT);
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_AMOUNT_MEDICINE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_AMOUNT_MEDICINE, 0)));

                TOTAL_VIR_PRICE_MEDICINE = rdo.HisSereServ_Bordereau_Medicines.Sum(o => o.VIR_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_PRICE_MEDICINE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_PRICE_MEDICINE, 0)));

                TOTAL_VIR_TOTAL_PRICE_MEDICINE = rdo.HisSereServ_Bordereau_Medicines.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_TOTAL_PRICE_MEDICINE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_TOTAL_PRICE_MEDICINE, 0)));

                TOTAL_AMOUNT_SERVICE = rdo.HisSereServ_Bordereau_Services.Sum(o => o.AMOUNT);
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_AMOUNT_SERVICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_AMOUNT_SERVICE, 0)));

                TOTAL_VIR_PRICE_SERVICE = rdo.HisSereServ_Bordereau_Services.Sum(o => o.VIR_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_PRICE_SERVICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_PRICE_SERVICE, 0)));

                TOTAL_VIR_TOTAL_PRICE_SERVICE = rdo.HisSereServ_Bordereau_Services.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_TOTAL_PRICE_SERVICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_TOTAL_PRICE_SERVICE, 0)));

                TOTAL_AMOUNT_MATERIAL = rdo.HisSereServ_Bordereau_Materials.Sum(o => o.AMOUNT);
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_AMOUNT_MATERIAL, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_AMOUNT_MATERIAL, 0)));
                TOTAL_VIR_PRICE_MATERIAL = rdo.HisSereServ_Bordereau_Materials.Sum(o => o.VIR_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_PRICE_MATERIAL, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_PRICE_MATERIAL, 0)));
                TOTAL_VIR_TOTAL_PRICE_MATERIAL = rdo.HisSereServ_Bordereau_Materials.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_VIR_TOTAL_PRICE_MATERIAL, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_VIR_TOTAL_PRICE_MATERIAL, 0)));

                TOTAL_PRICE_EXAM = rdo.HisSereServ_Bordereau_ExamServices.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PRICE_EXAM, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PRICE_EXAM, 0)));
                TOTAL_PRICE = rdo.SereServ2s.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PRICE, 0)));
                TOTAL_PRICE_HEIN = rdo.SereServ2s.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PRICE_HEIN, 0)));
                TOTAL_PRICE_PATIENT = rdo.SereServ2s.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PRICE_PATIENT, 0)));
                TOTAL_PERCENT = (TOTAL_PRICE != 0 ? ((TOTAL_PRICE_PATIENT / TOTAL_PRICE) * 100) : 0);
                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.TOTAL_PERCENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(TOTAL_PERCENT, 0)));

                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.currentDateSeparateFullTime));

                SetSingleKey(new KeyValue(Mps000003ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
