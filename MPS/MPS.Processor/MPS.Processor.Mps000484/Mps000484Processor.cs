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
using MPS.Processor.Mps000484.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000484
{
    class Mps000484Processor : AbstractProcessor
    {
        Mps000484PDO rdo;
        public Mps000484Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000484PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                //SetBarcodeKey();
                ProcessSingleKey();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                List<EventsCauseDeathADO> lstPrint = new List<EventsCauseDeathADO>();
                if (rdo.lstEventsCausesDeath != null && rdo.lstEventsCausesDeath.Count > 0)
                {
                    foreach (var item in rdo.lstEventsCausesDeath)
                    {
                        EventsCauseDeathADO ado = new EventsCauseDeathADO(item);
                        ado.ICD_NAME = rdo.Icds.FirstOrDefault(o => o.ICD_CODE == item.ICD_CODE) != null ? rdo.Icds.FirstOrDefault(o => o.ICD_CODE == item.ICD_CODE).ICD_NAME : null;
                        lstPrint.Add(ado);
                    }
                }
                objectTag.AddObjectData(store, "EventsCauseDeathChain", lstPrint.Where(o => o.IS_OTHER_CAUSE == 1).ToList() != null ? lstPrint.Where(o => o.IS_OTHER_CAUSE == 1).ToList() : new List<EventsCauseDeathADO>());
                objectTag.AddObjectData(store, "EventsCauseDeathImportance", lstPrint != null && lstPrint.Count > 0 && lstPrint.Where(o => o.IS_OTHER_CAUSE != 1 && string.IsNullOrEmpty(o.EXTERNAL_CAUSE)).ToList() != null ? lstPrint.Where(o => o.IS_OTHER_CAUSE != 1 && string.IsNullOrEmpty(o.EXTERNAL_CAUSE)).ToList() : new List<EventsCauseDeathADO>());
                objectTag.AddObjectData(store, "EventsCauseDeathOther", lstPrint != null && lstPrint.Count > 0 && lstPrint.Where(o => o.IS_OTHER_CAUSE != 1 && !string.IsNullOrEmpty(o.EXTERNAL_CAUSE)).ToList() != null ? lstPrint.Where(o => o.IS_OTHER_CAUSE != 1 && !string.IsNullOrEmpty(o.EXTERNAL_CAUSE)).ToList() : new List<EventsCauseDeathADO>());

                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.severeIllnessInfo != null && rdo.treatment != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.treatment.TREATMENT_CODE;
                    string severeIllnessInfoId = "SEVERE_ILLNESS_INFO_ID:" + rdo.severeIllnessInfo.ID;
                    result = String.Format("Mps000484 {0} {1}", treatmentCode, severeIllnessInfoId);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        void ProcessSingleKey()
        {
            try
            {
                if (rdo.severeIllnessInfo != null)
                {
                    SevereIllInfoADO ado = new SevereIllInfoADO(rdo.severeIllnessInfo);
                    ado.ICD_CODE_MOTHER_SEVERE_ILLNESS_INFO = rdo.severeIllnessInfo.FETAL_INFANT_AFFECTED_ICD;
                    if (rdo.Icds.FirstOrDefault(o => o.ICD_CODE == ado.ICD_CODE_MOTHER_SEVERE_ILLNESS_INFO) != null)
                    {
                        ado.ICD_NAME_MOTHER_SEVERE_ILLNESS_INFO = rdo.Icds.FirstOrDefault(o => o.ICD_CODE == ado.ICD_CODE_MOTHER_SEVERE_ILLNESS_INFO).ICD_NAME;
                    }
                    ado.ICD_NAME_CONCLU_SEVERE_ILLNESS_INFO = rdo.severeIllnessInfo.DEATH_MAIN_CAUSE;
                    if (rdo.Icds.FirstOrDefault(o => o.ICD_NAME == ado.ICD_NAME_CONCLU_SEVERE_ILLNESS_INFO) != null)
                    {
                        ado.ICD_CODE_CONCLU_SEVERE_ILLNESS_INFO = rdo.Icds.FirstOrDefault(o => o.ICD_NAME == ado.ICD_NAME_CONCLU_SEVERE_ILLNESS_INFO).ICD_CODE;
                    }
                    AddObjectKeyIntoListkey<SevereIllInfoADO>(ado, false);
                }
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.treatment, false);
                AddObjectKeyIntoListkey<HIS_PATIENT_TYPE_ALTER>(rdo.patientTypeAlter, false);
                AddObjectKeyIntoListkey<HIS_PATIENT>(rdo.patient, true);
                AddObjectKeyIntoListkey<HIS_DEPARTMENT_TRAN>(rdo.departmentTran, false);
                AddObjectKeyIntoListkey<HIS_DEPARTMENT>(rdo.department, false);
                AddObjectKeyIntoListkeyWithPrefix(rdo.branch, "BRANCH_", false);


                if (rdo.treatment.TREATMENT_END_TYPE_ID != null)
                    SetSingleKey(new KeyValue(Mps000484ExtendSingleKey.TREATMENT_END_TYPE_NAME, rdo.treatmentEndType.FirstOrDefault(o => o.ID == rdo.treatment.TREATMENT_END_TYPE_ID).TREATMENT_END_TYPE_NAME));
                if (rdo.treatment.TREATMENT_RESULT_ID != null)
                    SetSingleKey(new KeyValue(Mps000484ExtendSingleKey.TREATMENT_RESULT_NAME, rdo.treatmentResult.FirstOrDefault(o => o.ID == rdo.treatment.TREATMENT_RESULT_ID).TREATMENT_RESULT_NAME));
                if (rdo.patientTypeAlter != null)
                {
                    SetSingleKey(new KeyValue(Mps000484ExtendSingleKey.HEIN_CARD_TO_TIME, rdo.patientTypeAlter.HEIN_CARD_TO_TIME));
                    SetSingleKey(new KeyValue(Mps000484ExtendSingleKey.HEIN_CARD_FROM_TIME, rdo.patientTypeAlter.HEIN_CARD_FROM_TIME));
                    SetSingleKey(new KeyValue(Mps000484ExtendSingleKey.JOIN_5_YEAR_TIME, rdo.patientTypeAlter.JOIN_5_YEAR_TIME));
                    SetSingleKey(new KeyValue(Mps000484ExtendSingleKey.FREE_CO_PAID_TIME, rdo.patientTypeAlter.FREE_CO_PAID_TIME));
                    SetSingleKey(new KeyValue(Mps000484ExtendSingleKey.ALTER_HEIN_MEDI_ORG_CODE, rdo.patientTypeAlter.HEIN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000484ExtendSingleKey.ALTER_HEIN_MEDI_ORG_NAME, rdo.patientTypeAlter.HEIN_MEDI_ORG_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
