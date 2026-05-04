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

        // SereServ ADO master lists (build từ V_HIS_SERE_SERV + HIS_SERVICE + HIS_SERE_SERV_EXT)
        List<SereServADO> SereServADOs;
        List<SereServTeinADO> SereServTeinADOs;

        // 18 list phân loại dịch vụ kỹ thuật (theo SERVICE_TYPE_ID + FUEX/DIIM/TEST)
        List<SereServADO> SereServADO_GiaiPhauBenhLys = new List<SereServADO>();
        List<SereServADO> SereServADO_SieuAms = new List<SereServADO>();
        List<SereServADO> SereServADO_DienTims = new List<SereServADO>();
        List<SereServADO> SereServADO_XQuangs = new List<SereServADO>();
        List<SereServADO> SereServADO_CTs = new List<SereServADO>();
        List<SereServADO> SereServADO_MRIs = new List<SereServADO>();
        List<SereServADO> SereServADO_PETs = new List<SereServADO>();
        List<SereServADO> SereServADO_NoiSois = new List<SereServADO>();
        List<SereServADO> SereServADO_MatDoXuongs = new List<SereServADO>();
        List<SereServADO> SereServADO_ThamDoChucNangKhacs = new List<SereServADO>();
        List<SereServADO> SereServADO_HuyetHocs = new List<SereServADO>();
        List<SereServADO> SereServADO_SinhHoas = new List<SereServADO>();
        List<SereServADO> SereServADO_UngThus = new List<SereServADO>();
        List<SereServADO> SereServADO_ViSinhs = new List<SereServADO>();
        List<SereServADO> SereServADO_NuocTieus = new List<SereServADO>();
        List<SereServADO> SereServADO_Phans = new List<SereServADO>();
        List<SereServADO> SereServADO_CoTuCungs = new List<SereServADO>();
        List<SereServADO> SereServADO_GiaiPhauBenhs = new List<SereServADO>();

        // Test component lists
        List<SereServTeinADO> SereServTeinADO_HuyetHocs = new List<SereServTeinADO>();
        List<SereServTeinADO> SereServTeinADO_SinhHoas = new List<SereServTeinADO>();
        List<SereServTeinADO> SereServTeinADO_UngThus = new List<SereServTeinADO>();
        List<SereServTeinADO> SereServTeinADO_ViSinhs = new List<SereServTeinADO>();
        List<SereServTeinADO> SereServTeinADO_NuocTieus = new List<SereServTeinADO>();
        List<SereServTeinADO> SereServTeinADO_Phans = new List<SereServTeinADO>();
        List<SereServTeinADO> SereServTeinADO_CoTuCungs = new List<SereServTeinADO>();
        List<SereServTeinADO> SereServTeinADO_GiaiPhauBenhs = new List<SereServTeinADO>();
        List<SereServTeinADO> SereServTeinADO_GiaiPhauBenhLys = new List<SereServTeinADO>();

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
                if (rdo.HisServiceReq != null)
                    objectTag.AddObjectData(store, "ServiceReq", new List<V_HIS_SERVICE_REQ>() { rdo.HisServiceReq });
                if (rdo.Treatment != null)
                    objectTag.AddObjectData(store, "Treatment", new List<V_HIS_TREATMENT_4>() { rdo.Treatment });
                if (rdo.HisDhst != null)
                    objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst });
                if (rdo.ExamRanks != null)
                    objectTag.AddObjectData(store, "ExamRank", rdo.ExamRanks);
                if (rdo.DiseaseDetailResults != null)
                    objectTag.AddObjectData(store, "DiseaseDetailResult", rdo.DiseaseDetailResults);

                // Build disease detail ADO data
                BuildDiseaseDetailData();

                objectTag.AddObjectData(store, "Habits", lstHabits);
                objectTag.AddObjectData(store, "DiseaseHistory", lstDiseaseHistory);
                objectTag.AddObjectData(store, "FamilyHistory", lstFamilyHistory);
                objectTag.AddObjectData(store, "FunctionalSymptoms", lstFunctionalSymptoms);
                objectTag.AddObjectData(store, "PainSymptoms", lstPainSymptoms);
                objectTag.AddObjectData(store, "AllDetails", lstAllDetails);

                // Build danh sách dịch vụ kỹ thuật phân theo nhóm (giống Mps000481)
                BuildSereServData();

                objectTag.AddObjectData(store, "listSieuAm", SereServADO_SieuAms);
                objectTag.AddObjectData(store, "listDienTim", SereServADO_DienTims);
                objectTag.AddObjectData(store, "listGiaiPhauBenhly", SereServADO_GiaiPhauBenhLys);
                objectTag.AddObjectData(store, "listXquang", SereServADO_XQuangs);
                objectTag.AddObjectData(store, "listCT", SereServADO_CTs);
                objectTag.AddObjectData(store, "listMRI", SereServADO_MRIs);
                objectTag.AddObjectData(store, "listPET", SereServADO_PETs);
                objectTag.AddObjectData(store, "listNoiSoi", SereServADO_NoiSois);
                objectTag.AddObjectData(store, "listMatDoXuong", SereServADO_MatDoXuongs);
                objectTag.AddObjectData(store, "listKhac", SereServADO_ThamDoChucNangKhacs);
                objectTag.AddObjectData(store, "listHuyetHoc", SereServADO_HuyetHocs);
                objectTag.AddObjectData(store, "listSinhHoa", SereServADO_SinhHoas);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SereServADO_SinhHoas), SereServADO_SinhHoas));
                objectTag.AddObjectData(store, "listUngThu", SereServADO_UngThus);
                objectTag.AddObjectData(store, "listViSinh", SereServADO_ViSinhs);
                objectTag.AddObjectData(store, "listNuocTieu", SereServADO_NuocTieus);
                objectTag.AddObjectData(store, "listPhan", SereServADO_Phans);
                objectTag.AddObjectData(store, "listCoTuCung", SereServADO_CoTuCungs);
                objectTag.AddObjectData(store, "listGiaiPhauBenh", SereServADO_GiaiPhauBenhs);

                objectTag.AddObjectData(store, "SereServ_HH", SereServTeinADO_HuyetHocs);
                objectTag.AddObjectData(store, "SereServ_SH", SereServTeinADO_SinhHoas);
                objectTag.AddObjectData(store, "SereServ_UT", SereServTeinADO_UngThus);
                objectTag.AddObjectData(store, "SereServ_VS", SereServTeinADO_ViSinhs);
                objectTag.AddObjectData(store, "SereServ_NT", SereServTeinADO_NuocTieus);
                objectTag.AddObjectData(store, "SereServ_Phan", SereServTeinADO_Phans);
                objectTag.AddObjectData(store, "SereServ_CTC", SereServTeinADO_CoTuCungs);
                objectTag.AddObjectData(store, "SereServ_GPB", SereServTeinADO_GiaiPhauBenhs);
                objectTag.AddObjectData(store, "SereServ_GPBL", SereServTeinADO_GiaiPhauBenhLys);

                objectTag.AddRelationship(store, "listHuyetHoc", "SereServ_HH", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listSinhHoa", "SereServ_SH", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listUngThu", "SereServ_UT", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listViSinh", "SereServ_VS", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listNuocTieu", "SereServ_NT", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listPhan", "SereServ_Phan", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listCoTuCung", "SereServ_CTC", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listGiaiPhauBenh", "SereServ_GPB", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "listGiaiPhauBenhly", "SereServ_GPBL", "ID", "SERE_SERV_ID");

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
                // Key đơn cho TỪNG disease detail: {DD_<ID>_CHECK} = "X" hoặc "", {DD_<ID>_OTHER} = giá trị
                SetDiseaseDetailSingleKeys();

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

                // Health Exam Rank
                if (rdo.HisKskGeneral != null && rdo.HisKskGeneral.HEALTH_EXAM_RANK_ID != null && rdo.ExamRanks != null)
                {
                    var rank = rdo.ExamRanks.FirstOrDefault(o => o.ID == rdo.HisKskGeneral.HEALTH_EXAM_RANK_ID);
                    if (rank != null)
                    {
                        AddObjectKeyIntoListkey<HIS_HEALTH_EXAM_RANK>(rank, false);
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

                // Username các bác sĩ khám chuyên khoa (resolve LOGINNAME → TDL_USERNAME qua Employees)
                SetExamUsernameKeys();

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

        /// <summary>
        /// Tạo key đơn cho TỪNG disease detail theo ID:
        ///   {DD_<ID>_CHECK} = "X" nếu đã check, "" nếu không
        ///   {DD_<ID>_OTHER} = giá trị nhập khác
        ///   {DD_<ID>_NAME}  = tên disease detail
        /// Ví dụ: {DD_1_CHECK} = "X", {DD_1_NAME} = "<5", {DD_5_CHECK} = "X", {DD_5_NAME} = "Mất ngủ"
        /// </summary>
        private void SetDiseaseDetailSingleKeys()
        {
            try
            {
                if (rdo.DiseaseDetails == null) return;

                var results = rdo.DiseaseDetailResults ?? new List<HIS_DISEASE_DETAIL_RESULT>();
                var resultDict = results
                    .Where(r => r.DISEASE_DETAIL_ID != null)
                    .GroupBy(r => r.DISEASE_DETAIL_ID.Value)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.ID).First());

                foreach (var detail in rdo.DiseaseDetails)
                {
                    string prefix = "DD_" + detail.ID + "_";
                    HIS_DISEASE_DETAIL_RESULT matchResult;
                    resultDict.TryGetValue(detail.ID, out matchResult);

                    bool isChecked = matchResult != null && (matchResult.IS_CHECK ?? 0) == 1;
                    string otherValue = matchResult != null ? (matchResult.OTHER ?? "") : "";

                    SetSingleKey(new KeyValue(prefix + "CHECK", isChecked ? "X" : ""));
                    SetSingleKey(new KeyValue(prefix + "OTHER", otherValue));
                    SetSingleKey(new KeyValue(prefix + "NAME", detail.NAME ?? ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Resolve LOGINNAME (15 cột chuyên khoa khám trong HIS_KSK_GENERAL) → TDL_USERNAME
        /// dựa trên rdo.Employees (V_HIS_EMPLOYEE), set 15 single key {EXAM_*_USERNAME}.
        /// CONCLUDER_USERNAME đã được set riêng từ HIS_KSK_GENERAL.CONCLUDER_USERNAME.
        /// </summary>
        private void SetExamUsernameKeys()
        {
            try
            {
                if (rdo.HisKskGeneral == null) return;

                var ksk = rdo.HisKskGeneral;
                var mappings = new[]
                {
                    new { Key = Mps000507ExtendSingleKey.EXAM_CIRCULATION_USERNAME,    Loginname = ksk.EXAM_CIRCULATION_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_RESPIRATORY_USERNAME,    Loginname = ksk.EXAM_RESPIRATORY_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_DIGESTION_USERNAME,      Loginname = ksk.EXAM_DIGESTION_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_KIDNEY_UROLOGY_USERNAME, Loginname = ksk.EXAM_KIDNEY_UROLOGY_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_NEUROLOGICAL_USERNAME,   Loginname = ksk.EXAM_NEUROLOGICAL_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_MUSCLE_BONE_USERNAME,    Loginname = ksk.EXAM_MUSCLE_BONE_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_ENT_USERNAME,            Loginname = ksk.EXAM_ENT_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_STOMATOLOGY_USERNAME,    Loginname = ksk.EXAM_STOMATOLOGY_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_EYE_USERNAME,            Loginname = ksk.EXAM_EYE_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_OEND_USERNAME,           Loginname = ksk.EXAM_OEND_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_MENTAL_USERNAME,         Loginname = ksk.EXAM_MENTAL_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_DERMATOLOGY_USERNAME,    Loginname = ksk.EXAM_DERMATOLOGY_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_SURGERY_USERNAME,        Loginname = ksk.EXAM_SURGERY_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_OBSTETRIC_USERNAME,      Loginname = ksk.EXAM_OBSTETRIC_LOGINNAME },
                    new { Key = Mps000507ExtendSingleKey.EXAM_SUBCLINICAL_USERNAME,    Loginname = ksk.EXAM_SUBCLINICAL_LOGINNAME },
                };

                foreach (var m in mappings)
                {
                    string username = "";
                    if (!string.IsNullOrEmpty(m.Loginname) && rdo.Employees != null && rdo.Employees.Count > 0)
                    {
                        var emp = rdo.Employees.FirstOrDefault(e => e.LOGINNAME == m.Loginname);
                        if (emp != null) username = emp.TDL_USERNAME ?? "";
                    }
                    SetSingleKey(new KeyValue(m.Key, username));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Build SereServADOs + SereServTeinADOs từ V_HIS_SERE_SERV/HIS_SERVICE/HIS_SERE_SERV_EXT
        /// rồi phân loại thành 18 list theo SERVICE_TYPE_ID + FUEX_TYPE_ID/DIIM_TYPE_ID/TEST_TYPE_ID.
        /// Pattern port từ Mps000481.
        /// </summary>
        private void BuildSereServData()
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                SereServTeinADOs = new List<SereServTeinADO>();

                if (rdo.SereServs == null || rdo.SereServs.Count <= 0) return;

                foreach (var itemSS in rdo.SereServs)
                {
                    HIS_SERVICE service = null;
                    HIS_SERE_SERV_EXT sereServExt = null;

                    if (rdo.HisServices != null && rdo.HisServices.Count > 0)
                    {
                        service = rdo.HisServices.FirstOrDefault(o => o.ID == itemSS.SERVICE_ID);
                    }

                    if (rdo.SereSErvExts != null && rdo.SereSErvExts.Count > 0)
                    {
                        sereServExt = rdo.SereSErvExts.FirstOrDefault(o => o.SERE_SERV_ID == itemSS.ID);
                        if (sereServExt != null)
                        {
                            SereServADOs.Add(new SereServADO(itemSS, service, sereServExt));
                        }
                    }

                    if (!SereServADOs.Select(o => o.ID).Contains(itemSS.ID))
                    {
                        SereServADOs.Add(new SereServADO(itemSS, service));
                    }
                }

                if (rdo.SereServTeins != null && rdo.SereServTeins.Count > 0)
                {
                    foreach (var itemTein in rdo.SereServTeins)
                    {
                        V_HIS_TEST_INDEX testIndex = null;
                        if (rdo.TestIndexs != null && rdo.TestIndexs.Count > 0)
                        {
                            testIndex = rdo.TestIndexs.FirstOrDefault(o => o.ID == itemTein.TEST_INDEX_ID);
                        }
                        SereServTeinADOs.Add(new SereServTeinADO(itemTein, testIndex));
                    }
                }

                this.SereServADO_GiaiPhauBenhLys = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL, null, null, null);
                this.SereServTeinADO_GiaiPhauBenhLys = getSereServTeinADOADO(null);
                foreach (var ado in this.SereServADO_GiaiPhauBenhLys)
                {
                    var tein = this.SereServTeinADO_GiaiPhauBenhLys.FirstOrDefault(t => t.SERE_SERV_ID == ado.ID);
                    ado.DISPLAY_VALUE = !string.IsNullOrEmpty(tein?.VALUE) ? tein.VALUE : ado.CONCLUDE;
                }

                this.SereServADO_SieuAms = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA, null, null, null);
                this.SereServADO_DienTims = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN, 1, null, null);
                this.SereServADO_XQuangs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, null, 1, null);
                this.SereServADO_CTs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, null, 2, null);
                this.SereServADO_MRIs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, null, 3, null);
                this.SereServADO_PETs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA, null, 4, null);
                this.SereServADO_NoiSois = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS, null, null, null);
                this.SereServADO_MatDoXuongs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN, 4, null, null);
                this.SereServADO_ThamDoChucNangKhacs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN, 0, null, null);

                this.SereServADO_HuyetHocs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 1);
                this.SereServTeinADO_HuyetHocs = getSereServTeinADOADO(null);
                this.SereServADO_SinhHoas = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 3);
                this.SereServTeinADO_SinhHoas = getSereServTeinADOADO(null);
                this.SereServADO_UngThus = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 8);
                this.SereServTeinADO_UngThus = getSereServTeinADOADO(null);

                this.SereServADO_ViSinhs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 2);
                this.SereServADO_ViSinhs.AddRange(getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 4));
                this.SereServTeinADO_ViSinhs = getSereServTeinADOADO(null);

                this.SereServADO_NuocTieus = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 7);
                this.SereServTeinADO_NuocTieus = getSereServTeinADOADO(1);
                this.SereServADO_Phans = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 9);
                this.SereServTeinADO_Phans = getSereServTeinADOADO(null);
                this.SereServADO_CoTuCungs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 10);
                this.SereServTeinADO_CoTuCungs = getSereServTeinADOADO(null);
                this.SereServADO_GiaiPhauBenhs = getSereServADO(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, null, null, 6);
                this.SereServTeinADO_GiaiPhauBenhs = getSereServTeinADOADO(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<SereServADO> getSereServADO(long ServiceTypeId, long? FuexTypeId, long? DiimTypeId, long? TestTypeId)
        {
            List<SereServADO> lstSereServADO = new List<SereServADO>();
            try
            {
                if (this.SereServADOs != null && this.SereServADOs.Count > 0)
                {
                    lstSereServADO = this.SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == ServiceTypeId).ToList();
                }

                if (lstSereServADO != null && lstSereServADO.Count > 0)
                {
                    if (FuexTypeId != null)
                    {
                        if (FuexTypeId > 0)
                        {
                            lstSereServADO = lstSereServADO.Where(o => o.FUEX_TYPE_ID == FuexTypeId).ToList();
                        }
                        else
                        {
                            lstSereServADO = lstSereServADO.Where(o => o.FUEX_TYPE_ID == null).ToList();
                        }
                    }

                    if (DiimTypeId != null)
                    {
                        lstSereServADO = lstSereServADO.Where(o => o.DIIM_TYPE_ID == DiimTypeId).ToList();
                    }

                    if (TestTypeId != null)
                    {
                        lstSereServADO = lstSereServADO.Where(o => o.TEST_TYPE_ID == TestTypeId).ToList();
                    }

                    lstSereServADO = lstSereServADO.OrderBy(o => o.NUM_ORDER ?? 999999999999).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<SereServADO>();
            }
            return lstSereServADO;
        }

        private List<SereServTeinADO> getSereServTeinADOADO(long? IsImportant)
        {
            List<SereServTeinADO> lstSereServTeinADO = new List<SereServTeinADO>();
            try
            {
                if (SereServTeinADOs != null && SereServTeinADOs.Count > 0)
                {
                    if (IsImportant != null)
                    {
                        lstSereServTeinADO = SereServTeinADOs.Where(o => o.IS_IMPORTANT == IsImportant).ToList();
                    }
                    else
                    {
                        lstSereServTeinADO = SereServTeinADOs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<SereServTeinADO>();
            }
            return lstSereServTeinADO;
        }
    }
}
