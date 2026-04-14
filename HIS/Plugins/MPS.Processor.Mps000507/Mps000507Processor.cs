using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000507.ADO;
using MPS.Processor.Mps000507.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000507
{
    public class Mps000507Processor : AbstractProcessor
    {
        Mps000507PDO rdo;

        // ADO lists cho từng PARENT_TYPE - dùng vlookup trong template
        List<DiseaseDetailADO> lstHabits = new List<DiseaseDetailADO>();           // PARENT_TYPE=1
        List<DiseaseDetailADO> lstDiseaseHistory = new List<DiseaseDetailADO>();    // PARENT_TYPE=2
        List<DiseaseDetailADO> lstFamilyHistory = new List<DiseaseDetailADO>();     // PARENT_TYPE=3
        List<DiseaseDetailADO> lstFunctionalSymptoms = new List<DiseaseDetailADO>();// PARENT_TYPE=4
        List<DiseaseDetailADO> lstPainSymptoms = new List<DiseaseDetailADO>();      // PARENT_TYPE=5
        List<DiseaseDetailADO> lstAllDetails = new List<DiseaseDetailADO>();        // Tất cả

        public Mps000507Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000507PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                SetSingleKey();
                SetSignatureKeyImageByCFG();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                // Object data cho template
                if (rdo.HisKskGeneral != null)
                    objectTag.AddObjectData(store, "KskGeneral", new List<HIS_KSK_GENERAL>() { rdo.HisKskGeneral });
                if (rdo.Treatment != null)
                    objectTag.AddObjectData(store, "Treatment", new List<V_HIS_TREATMENT_4>() { rdo.Treatment });
                if (rdo.HisDhst != null)
                    objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst });
                if (rdo.ExamRanks != null)
                    objectTag.AddObjectData(store, "ExamRank", rdo.ExamRanks);

                // Build disease detail ADO data
                BuildDiseaseDetailData();

                objectTag.AddObjectData(store, "Habits", lstHabits);
                objectTag.AddObjectData(store, "DiseaseHistory", lstDiseaseHistory);
                objectTag.AddObjectData(store, "FamilyHistory", lstFamilyHistory);
                objectTag.AddObjectData(store, "FunctionalSymptoms", lstFunctionalSymptoms);
                objectTag.AddObjectData(store, "PainSymptoms", lstPainSymptoms);
                objectTag.AddObjectData(store, "AllDetails", lstAllDetails);

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Build ADO data từ V_HIS_DISEASE_DETAIL + HIS_DISEASE_DETAIL_RESULT
        /// Mỗi disease detail sẽ có IS_CHECK_X = "X" nếu đã check, OTHER_VALUE = giá trị nhập khác
        /// Template Excel dùng vlookup theo DISEASE_DETAIL_ID hoặc NAME để hiển thị
        /// </summary>
        private void BuildDiseaseDetailData()
        {
            try
            {
                if (rdo.DiseaseDetails == null) return;

                var results = rdo.DiseaseDetailResults ?? new List<HIS_DISEASE_DETAIL_RESULT>();

                foreach (var detail in rdo.DiseaseDetails.OrderBy(o => o.PARENT_TYPE).ThenBy(o => o.NUM_ORDER_TYPE).ThenBy(o => o.NUM_ORDER_DETAIL))
                {
                    var matchResult = results.FirstOrDefault(r => r.DISEASE_DETAIL_ID == detail.ID);

                    var ado = new DiseaseDetailADO();
                    ado.DISEASE_TYPE_ID = detail.DISEASE_TYPE_ID;
                    ado.DISEASE_TYPE_NAME = detail.DISEASE_TYPE_NAME;
                    ado.PARENT_TYPE = detail.PARENT_TYPE;
                    ado.NUM_ORDER_TYPE = detail.NUM_ORDER_TYPE;
                    ado.DISEASE_DETAIL_ID = detail.ID;
                    ado.NAME = detail.NAME;
                    ado.NUM_ORDER_DETAIL = detail.NUM_ORDER_DETAIL;
                    ado.IS_CHECKBOX = detail.IS_CHECKBOX;
                    ado.IS_OTHER = detail.IS_OTHER;
                    ado.IS_CHECK_X = (matchResult != null && (matchResult.IS_CHECK ?? 0) == 1) ? "X" : "";
                    ado.OTHER_VALUE = matchResult != null ? (matchResult.OTHER ?? "") : "";

                    lstAllDetails.Add(ado);

                    switch (detail.PARENT_TYPE)
                    {
                        case 1: lstHabits.Add(ado); break;
                        case 2: lstDiseaseHistory.Add(ado); break;
                        case 3: lstFamilyHistory.Add(ado); break;
                        case 4: lstFunctionalSymptoms.Add(ado); break;
                        case 5: lstPainSymptoms.Add(ado); break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.HisKskGeneral != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_GENERAL>(rdo.HisKskGeneral, false);
                }
                if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                }
                if (rdo.HisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDhst, false);
                }
                if (rdo.Treatment != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_4>(rdo.Treatment, false);
                }

                // BMI
                if (rdo.HisDhst != null && rdo.HisDhst.VIR_BMI != null)
                {
                    SetSingleKey(new KeyValue(Mps000507ExtendSingleKey.BMI_VALUE, Math.Round(rdo.HisDhst.VIR_BMI.Value, 2).ToString()));
                }

                // Huyết áp display: MAX/MIN mmHg
                if (rdo.HisDhst != null && rdo.HisDhst.BLOOD_PRESSURE_MAX != null && rdo.HisDhst.BLOOD_PRESSURE_MIN != null)
                {
                    SetSingleKey(new KeyValue(Mps000507ExtendSingleKey.BLOOD_PRESSURE_DISPLAY,
                        rdo.HisDhst.BLOOD_PRESSURE_MAX + "/" + rdo.HisDhst.BLOOD_PRESSURE_MIN + " mmHg"));
                }

                // DHST loginname
                if (rdo.HisDhst != null && !string.IsNullOrEmpty(rdo.HisDhst.EXECUTE_LOGINNAME))
                {
                    SetSingleKey(new KeyValue(Mps000507ExtendSingleKey.DHST_LOGINNAME, rdo.HisDhst.EXECUTE_LOGINNAME));
                }

                // Health Exam Rank Name
                if (rdo.HisKskGeneral != null && rdo.HisKskGeneral.HEALTH_EXAM_RANK_ID != null && rdo.ExamRanks != null)
                {
                    var rank = rdo.ExamRanks.FirstOrDefault(o => o.ID == rdo.HisKskGeneral.HEALTH_EXAM_RANK_ID);
                    if (rank != null)
                    {
                        SetSingleKey(new KeyValue(Mps000507ExtendSingleKey.HEALTH_EXAM_RANK_NAME, rank.HEALTH_EXAM_RANK_NAME));
                    }
                }

                // Conclusion date display
                if (rdo.HisKskGeneral != null && rdo.HisKskGeneral.CONCLUSION_TIME != null)
                {
                    var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.HisKskGeneral.CONCLUSION_TIME.Value);
                    if (dt.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000507ExtendSingleKey.CONCLUSION_DATE_DISPLAY,
                            "Ngày " + dt.Value.Day + " Tháng " + dt.Value.Month + " Năm " + dt.Value.Year));
                    }
                }

                // Concluder username
                if (rdo.HisKskGeneral != null && !string.IsNullOrEmpty(rdo.HisKskGeneral.CONCLUDER_USERNAME))
                {
                    SetSingleKey(new KeyValue(Mps000507ExtendSingleKey.CONCLUDER_USERNAME, rdo.HisKskGeneral.CONCLUDER_USERNAME));
                }

                // Work place name (cơ quan công tác) từ Treatment
                if (rdo.Treatment != null && !string.IsNullOrEmpty(rdo.Treatment.TDL_PATIENT_WORK_PLACE_NAME))
                {
                    SetSingleKey(new KeyValue(Mps000507ExtendSingleKey.WORK_PLACE_NAME, rdo.Treatment.TDL_PATIENT_WORK_PLACE_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
