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
using MPS.Processor.Mps000481.PDO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MPS.Processor.Mps000481
{
    /// <summary>
    /// Dữ liệu cho mẫu "Hồ sơ sức khỏe" (Phiếu kết quả CLS khám sức khỏe dạng báo cáo):
    ///  - listTreatment: bổ sung tuổi, ngày khám, thể lực (DHST), tiền sử, bác sĩ/ngày kết luận.
    ///  - listXetNghiemNhom → listXetNghiem: bảng XN theo loại XN, có ĐƠN VỊ + BÌNH THƯỜNG + đánh dấu bất thường.
    ///  - listCdhaTdcn: CĐHA &amp; TDCN (siêu âm, X-quang/CT/MRI, nội soi, thăm dò chức năng) + kết luận.
    ///  - listPttt: phẫu thuật / thủ thuật.
    /// Mẫu cũ không dùng các band này nên không bị ảnh hưởng.
    /// </summary>
    public partial class Mps000481Processor
    {
        private const string TEST_GROUP_PREFIX = "Xét nghiệm ";
        private const string TEST_GROUP_OTHER = "Xét nghiệm khác";
        private const string CHILD_INDENT = "    ";
        private const string FLAG_HIGH = "H";
        private const string FLAG_LOW = "L";

        List<TestGroupADO> TestGroupADOs = new List<TestGroupADO>();
        List<TestResultRowADO> TestResultRowADOs = new List<TestResultRowADO>();
        List<SubclinicalResultADO> CdhaTdcnADOs = new List<SubclinicalResultADO>();
        List<SubclinicalResultADO> PtttADOs = new List<SubclinicalResultADO>();

        /// <summary>Đẩy các band của mẫu "Hồ sơ sức khỏe" vào store (gọi trong ProcessData).</summary>
        private void AddHoSoSucKhoeObjectData(Inventec.Common.FlexCellExport.ProcessObjectTag objectTag)
        {
            try
            {
                objectTag.AddObjectData(store, "listXetNghiemNhom", TestGroupADOs);
                objectTag.AddObjectData(store, "listXetNghiem", TestResultRowADOs);
                objectTag.AddObjectData(store, "listCdhaTdcn", CdhaTdcnADOs);
                objectTag.AddObjectData(store, "listPttt", PtttADOs);

                objectTag.AddRelationship(store, "listTreatment", "listXetNghiemNhom", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listXetNghiemNhom", "listXetNghiem", "GROUP_KEY", "GROUP_KEY");
                objectTag.AddRelationship(store, "listTreatment", "listCdhaTdcn", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "listTreatment", "listPttt", "ID", "TDL_TREATMENT_ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Dựng dữ liệu mẫu "Hồ sơ sức khỏe" (gọi cuối SetSingleKey, sau khi đã có TreatmentlADOs + SereServADOs).</summary>
        private void BuildHoSoSucKhoeData()
        {
            try
            {
                TestGroupADOs = new List<TestGroupADO>();
                TestResultRowADOs = new List<TestResultRowADO>();
                CdhaTdcnADOs = new List<SubclinicalResultADO>();
                PtttADOs = new List<SubclinicalResultADO>();

                if (TreatmentlADOs == null || TreatmentlADOs.Count == 0) return;

                Dictionary<long, HIS_SERE_SERV_EXT> extBySereServ = BuildExtDictionary();
                foreach (var treatment in TreatmentlADOs)
                {
                    FillTreatmentInfo(treatment);
                }

                if (SereServADOs == null || SereServADOs.Count == 0) return;
                var validSereServs = SereServADOs.Where(o => o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE).ToList();
                var sereServByTreatment = validSereServs.Where(o => o.TDL_TREATMENT_ID.HasValue).ToLookup(o => o.TDL_TREATMENT_ID.Value);
                var treatmentById = TreatmentlADOs.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());

                foreach (var treatmentId in treatmentById.Keys)
                {
                    var sereServs = sereServByTreatment[treatmentId].ToList();
                    if (sereServs.Count == 0) continue;
                    BuildTestRows(treatmentById[treatmentId], sereServs);
                    BuildSubclinicalRows(treatmentId, sereServs, extBySereServ);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Thông tin hồ sơ (hành chính, thể lực, tiền sử, kết luận)

        private void FillTreatmentInfo(TreatmentlADO treatment)
        {
            try
            {
                List<V_HIS_SERVICE_REQ> examReqs = (rdo.ServiceReqs != null)
                    ? rdo.ServiceReqs.Where(o => o.TREATMENT_ID == treatment.ID).OrderByDescending(o => o.INTRUCTION_TIME).ToList()
                    : new List<V_HIS_SERVICE_REQ>();
                V_HIS_SERVICE_REQ examReq = examReqs.FirstOrDefault();
                HashSet<long> examReqIds = new HashSet<long>(examReqs.Select(o => o.ID));
                HIS_KSK_GENERAL general = (rdo.ksk_Generals != null && examReqIds.Count > 0)
                    ? rdo.ksk_Generals.FirstOrDefault(o => examReqIds.Contains(o.SERVICE_REQ_ID))
                    : null;
                // Phần khám mẫu chuyên biệt theo thứ tự ưu tiên trên-18 → dưới-18 → dưới-6.
                object kskForm = FindKskForm(rdo.ksk_OverEighteens, treatment.ID)
                    ?? FindKskForm(rdo.ksk_UnderEighteens, treatment.ID)
                    ?? FindKskForm(rdo.ksk_UnderSixs, treatment.ID);

                treatment.AGE_STR = CalculateAgeStr(treatment.TDL_PATIENT_DOB, treatment.IN_TIME);
                treatment.PATIENT_PHONE_STR = FirstNotEmpty(treatment.TDL_PATIENT_MOBILE, treatment.TDL_PATIENT_PHONE);
                treatment.WORK_PLACE_DISPLAY = FirstNotEmpty(treatment.TDL_PATIENT_WORK_PLACE_NAME, treatment.TDL_PATIENT_WORK_PLACE, treatment.WORK_PLACE_NAME);
                treatment.EXAM_DATE_STR = TimeToDateStr(treatment.IN_TIME);

                treatment.PATHOLOGICAL_HISTORY = FirstNotEmpty(GetKskStr(kskForm, "PATHOLOGICAL_HISTORY"),
                    examReq != null ? examReq.PATHOLOGICAL_HISTORY : null,
                    general != null ? general.PATHOLOGICAL_HISTORY : null);
                treatment.PATHOLOGICAL_HISTORY_FAMILY = FirstNotEmpty(GetKskStr(kskForm, "PATHOLOGICAL_HISTORY_FAMILY"),
                    examReq != null ? examReq.PATHOLOGICAL_HISTORY_FAMILY : null);

                FillDhst(treatment, kskForm, general, examReq);
                FillConcluder(treatment, kskForm, general, examReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDhst(TreatmentlADO treatment, object kskForm, HIS_KSK_GENERAL general, V_HIS_SERVICE_REQ examReq)
        {
            try
            {
                if (rdo.Dhsts == null || rdo.Dhsts.Count == 0) return;
                long? dhstId = GetKskLong(kskForm, "DHST_ID")
                    ?? (general != null ? general.DHST_ID : null)
                    ?? (examReq != null ? examReq.DHST_ID : null);

                V_HIS_DHST dhst = dhstId.HasValue ? rdo.Dhsts.FirstOrDefault(o => o.ID == dhstId.Value) : null;
                if (dhst == null)
                {
                    dhst = rdo.Dhsts.Where(o => o.TREATMENT_ID == treatment.ID)
                        .OrderByDescending(o => o.EXECUTE_TIME ?? 0).ThenByDescending(o => o.MODIFY_TIME ?? 0).FirstOrDefault();
                }
                if (dhst == null) return;

                treatment.HEIGHT_STR = FormatNumber(dhst.HEIGHT);
                treatment.WEIGHT_STR = FormatNumber(dhst.WEIGHT);
                treatment.BMI_STR = dhst.VIR_BMI.HasValue ? Math.Round(dhst.VIR_BMI.Value, 1).ToString("0.0", CultureInfo.InvariantCulture) : null;
                treatment.PULSE_STR = FormatNumber(dhst.PULSE);
                treatment.TEMPERATURE_STR = FormatNumber(dhst.TEMPERATURE);
                treatment.BREATH_RATE_STR = FormatNumber(dhst.BREATH_RATE);
                if (dhst.BLOOD_PRESSURE_MAX.HasValue || dhst.BLOOD_PRESSURE_MIN.HasValue)
                {
                    treatment.BLOOD_PRESSURE_STR = FormatNumber(dhst.BLOOD_PRESSURE_MAX) + "/" + FormatNumber(dhst.BLOOD_PRESSURE_MIN);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillConcluder(TreatmentlADO treatment, object kskForm, HIS_KSK_GENERAL general, V_HIS_SERVICE_REQ examReq)
        {
            try
            {
                treatment.CONCLUDER_USERNAME = FirstNotEmpty(GetKskStr(kskForm, "CONCLUDER_USERNAME"),
                    general != null ? general.CONCLUDER_USERNAME : null,
                    examReq != null ? examReq.EXECUTE_USERNAME : null);
                treatment.CONCLUDER_LOGINNAME = FirstNotEmpty(GetKskStr(kskForm, "CONCLUDER_LOGINNAME"),
                    general != null ? general.CONCLUDER_LOGINNAME : null,
                    examReq != null ? examReq.EXECUTE_LOGINNAME : null);
                treatment.CONCLUSION_TIME = GetKskLong(kskForm, "CONCLUSION_TIME")
                    ?? (general != null ? general.CONCLUSION_TIME : null)
                    ?? (examReq != null ? examReq.FINISH_TIME : null)
                    ?? treatment.OUT_TIME;

                DateTime? conclusionDate = treatment.CONCLUSION_TIME.HasValue
                    ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.CONCLUSION_TIME.Value)
                    : null;
                if (conclusionDate.HasValue)
                {
                    treatment.CONCLUSION_DAY = conclusionDate.Value.ToString("dd");
                    treatment.CONCLUSION_MONTH = conclusionDate.Value.ToString("MM");
                    treatment.CONCLUSION_YEAR = conclusionDate.Value.ToString("yyyy");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Tìm bản ghi mẫu KSK (bảng nào cũng có TDL_TREATMENT_ID) của đợt điều trị.</summary>
        private static object FindKskForm(System.Collections.IEnumerable records, long treatmentId)
        {
            if (records == null) return null;
            foreach (var record in records)
            {
                if (record != null && GetKskLong(record, "TDL_TREATMENT_ID") == treatmentId) return record;
            }
            return null;
        }

        #endregion

        #region Bảng xét nghiệm

        private void BuildTestRows(TreatmentlADO treatment, List<SereServADO> sereServs)
        {
            try
            {
                var tests = sereServs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                if (tests.Count == 0) return;

                var teinBySereServ = (SereServTeinADOs ?? new List<SereServTeinADO>())
                    .Where(o => o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                    .ToLookup(o => o.SERE_SERV_ID);
                var testTypeNames = (rdo.TestTypes ?? new List<HIS_TEST_TYPE>())
                    .GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First().TEST_TYPE_NAME);
                var rangeByCode = (rdo.TestIndexRanges ?? new List<V_HIS_TEST_INDEX_RANGE>())
                    .Where(o => !String.IsNullOrEmpty(o.TEST_INDEX_CODE)).ToLookup(o => o.TEST_INDEX_CODE);

                // Nhóm theo loại XN; thứ tự nhóm theo dịch vụ có NUM_ORDER nhỏ nhất trong nhóm.
                var groups = tests.GroupBy(o => o.TEST_TYPE_ID)
                    .OrderBy(g => g.Min(o => o.NUM_ORDER ?? long.MaxValue)).ThenBy(g => g.Key ?? long.MaxValue);
                foreach (var group in groups)
                {
                    TestGroupADO groupAdo = new TestGroupADO();
                    groupAdo.TDL_TREATMENT_ID = treatment.ID;
                    groupAdo.TEST_TYPE_ID = group.Key;
                    groupAdo.GROUP_KEY = treatment.ID + "_" + (group.Key.HasValue ? group.Key.Value.ToString() : "0");
                    string typeName = null;
                    if (group.Key.HasValue) testTypeNames.TryGetValue(group.Key.Value, out typeName);
                    groupAdo.TEST_TYPE_NAME = typeName;
                    groupAdo.GROUP_NAME = BuildTestGroupName(typeName);
                    TestGroupADOs.Add(groupAdo);

                    long stt = 0;
                    foreach (var sereServ in group.OrderBy(o => o.NUM_ORDER ?? long.MaxValue).ThenBy(o => o.TDL_SERVICE_NAME))
                    {
                        var teins = teinBySereServ[sereServ.ID].OrderBy(o => o.NUM_ORDER ?? long.MaxValue).ThenBy(o => o.ID).ToList();
                        AddTestRows(treatment, groupAdo.GROUP_KEY, sereServ, teins, rangeByCode, ref stt);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dịch vụ có 0–1 chỉ số → 1 dòng (tên dịch vụ + kết quả chỉ số).
        /// Dịch vụ nhiều chỉ số → 1 dòng dịch vụ cha + các dòng chỉ số con thụt lề.
        /// </summary>
        private void AddTestRows(TreatmentlADO treatment, string groupKey, SereServADO sereServ, List<SereServTeinADO> teins,
            ILookup<string, V_HIS_TEST_INDEX_RANGE> rangeByCode, ref long stt)
        {
            if (teins.Count <= 1)
            {
                TestResultRowADO row = CreateRow(treatment.ID, groupKey, sereServ.ID, ++stt, sereServ.TDL_SERVICE_NAME);
                if (teins.Count == 1)
                    FillIndexResult(row, teins[0], treatment, rangeByCode);
                else
                    row.UNIT = sereServ.SERVICE_UNIT_NAME;
                TestResultRowADOs.Add(row);
                return;
            }

            TestResultRowADO parent = CreateRow(treatment.ID, groupKey, sereServ.ID, ++stt, sereServ.TDL_SERVICE_NAME);
            parent.IS_PARENT = 1;
            parent.UNIT = sereServ.SERVICE_UNIT_NAME;
            TestResultRowADOs.Add(parent);
            foreach (var tein in teins)
            {
                TestResultRowADO child = CreateRow(treatment.ID, groupKey, sereServ.ID, ++stt, CHILD_INDENT + tein.TEST_INDEX_NAME);
                child.IS_CHILD = 1;
                FillIndexResult(child, tein, treatment, rangeByCode);
                TestResultRowADOs.Add(child);
            }
        }

        private static TestResultRowADO CreateRow(long treatmentId, string groupKey, long sereServId, long stt, string name)
        {
            TestResultRowADO row = new TestResultRowADO();
            row.TDL_TREATMENT_ID = treatmentId;
            row.GROUP_KEY = groupKey;
            row.SERE_SERV_ID = sereServId;
            row.STT = stt;
            row.NAME = name;
            return row;
        }

        private void FillIndexResult(TestResultRowADO row, SereServTeinADO tein, TreatmentlADO treatment, ILookup<string, V_HIS_TEST_INDEX_RANGE> rangeByCode)
        {
            try
            {
                row.VALUE = tein.VALUE;
                row.UNIT = tein.TEST_INDEX_UNIT_NAME;
                row.NOTE = tein.NOTE;

                V_HIS_TEST_INDEX_RANGE range = FindRange(rangeByCode, tein.TEST_INDEX_CODE, treatment);
                if (range == null) return;
                row.NORMAL_RANGE = BuildNormalRangeText(range);
                row.ABNORMAL_FLAG = GetAbnormalFlag(tein.VALUE, range);
                row.IS_ABNORMAL = String.IsNullOrEmpty(row.ABNORMAL_FLAG) ? (short?)null : (short)1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Chọn khoảng tham chiếu theo mã chỉ số + tuổi (theo AGE_TYPE) + giới tính — như Mps000096.</summary>
        private static V_HIS_TEST_INDEX_RANGE FindRange(ILookup<string, V_HIS_TEST_INDEX_RANGE> rangeByCode, string testIndexCode, TreatmentlADO treatment)
        {
            if (String.IsNullOrEmpty(testIndexCode) || !rangeByCode.Contains(testIndexCode)) return null;
            long dob = treatment.TDL_PATIENT_DOB;
            long refTime = treatment.IN_TIME > 0 ? treatment.IN_TIME : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0);

            var candidates = rangeByCode[testIndexCode].Where(o => o.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE).Where(o =>
            {
                long age = CalculateAgeByType(dob, refTime, o.AGE_TYPE_ID);
                return (!o.AGE_FROM.HasValue || o.AGE_FROM.Value <= age) && (!o.AGE_TO.HasValue || o.AGE_TO.Value >= age);
            });

            if (treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                candidates = candidates.Where(o => o.IS_MALE == 1);
            else if (treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                candidates = candidates.Where(o => o.IS_FEMALE == 1);

            return candidates.FirstOrDefault();
        }

        private static long CalculateAgeByType(long dob, long refTime, long? ageTypeId)
        {
            if (ageTypeId == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH)
                return Inventec.Common.DateTime.Calculation.DifferenceMonth(dob, refTime);
            if (ageTypeId == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY)
                return Inventec.Common.DateTime.Calculation.DifferenceDate(dob, refTime);
            if (ageTypeId == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR)
                return Inventec.Common.DateTime.Calculation.DifferenceTime(dob, refTime, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.HOUR);
            return Inventec.Common.DateTime.Calculation.DifferenceTime(dob, refTime, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) / 365;
        }

        /// <summary>"Bình thường": NORMAL_VALUE nếu có; ngược lại "MIN - MAX" / "&lt; MAX" / "≥ MIN" theo cờ chấp nhận bằng.</summary>
        private static string BuildNormalRangeText(V_HIS_TEST_INDEX_RANGE range)
        {
            if (!String.IsNullOrWhiteSpace(range.NORMAL_VALUE)) return range.NORMAL_VALUE.Trim();
            bool hasMin = !String.IsNullOrWhiteSpace(range.MIN_VALUE);
            bool hasMax = !String.IsNullOrWhiteSpace(range.MAX_VALUE);
            if (hasMin && hasMax) return range.MIN_VALUE.Trim() + " - " + range.MAX_VALUE.Trim();
            if (hasMax) return (range.IS_ACCEPT_EQUAL_MAX == 1 ? "≤ " : "< ") + range.MAX_VALUE.Trim();
            if (hasMin) return (range.IS_ACCEPT_EQUAL_MIN == 1 ? "≥ " : "> ") + range.MIN_VALUE.Trim();
            return null;
        }

        /// <summary>So kết quả số với MIN/MAX (tôn trọng cờ chấp nhận bằng). Kết quả chữ → không đánh dấu.</summary>
        private static string GetAbnormalFlag(string value, V_HIS_TEST_INDEX_RANGE range)
        {
            decimal? v = ParseDecimal(value);
            if (!v.HasValue) return null;
            decimal? min = ParseDecimal(range.MIN_VALUE);
            decimal? max = ParseDecimal(range.MAX_VALUE);
            if (min.HasValue && (range.IS_ACCEPT_EQUAL_MIN == 1 ? v.Value < min.Value : v.Value <= min.Value)) return FLAG_LOW;
            if (max.HasValue && (range.IS_ACCEPT_EQUAL_MAX == 1 ? v.Value > max.Value : v.Value >= max.Value)) return FLAG_HIGH;
            return null;
        }

        private static string BuildTestGroupName(string testTypeName)
        {
            if (String.IsNullOrWhiteSpace(testTypeName)) return TEST_GROUP_OTHER;
            string name = testTypeName.Trim();
            return name.StartsWith(TEST_GROUP_PREFIX.Trim(), StringComparison.OrdinalIgnoreCase) ? name : TEST_GROUP_PREFIX + name;
        }

        #endregion

        #region CĐHA & TDCN, PTTT

        private void BuildSubclinicalRows(long treatmentId, List<SereServADO> sereServs, Dictionary<long, HIS_SERE_SERV_EXT> extBySereServ)
        {
            try
            {
                // Thứ tự nhóm: Siêu âm → CĐHA → Nội soi → TDCN (giống mẫu: siêu âm, X-quang, điện tim).
                long[] cdhaTypes = new long[]
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                };
                long[] ptttTypes = new long[]
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                };

                AddSubclinicalRows(CdhaTdcnADOs, treatmentId, sereServs, cdhaTypes, extBySereServ);
                AddSubclinicalRows(PtttADOs, treatmentId, sereServs, ptttTypes, extBySereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void AddSubclinicalRows(List<SubclinicalResultADO> target, long treatmentId, List<SereServADO> sereServs,
            long[] serviceTypeIds, Dictionary<long, HIS_SERE_SERV_EXT> extBySereServ)
        {
            long stt = 0;
            var ordered = sereServs.Where(o => serviceTypeIds.Contains(o.TDL_SERVICE_TYPE_ID))
                .OrderBy(o => Array.IndexOf(serviceTypeIds, o.TDL_SERVICE_TYPE_ID))
                .ThenBy(o => o.NUM_ORDER ?? long.MaxValue).ThenBy(o => o.TDL_SERVICE_NAME);
            foreach (var sereServ in ordered)
            {
                SubclinicalResultADO ado = new SubclinicalResultADO();
                ado.TDL_TREATMENT_ID = treatmentId;
                ado.SERE_SERV_ID = sereServ.ID;
                ado.STT = ++stt;
                ado.SERVICE_CODE = sereServ.TDL_SERVICE_CODE;
                ado.SERVICE_NAME = sereServ.TDL_SERVICE_NAME;
                HIS_SERE_SERV_EXT ext;
                if (extBySereServ.TryGetValue(sereServ.ID, out ext) && ext != null)
                {
                    ado.RESULT = ext.CONCLUDE;
                    ado.NOTE = ext.NOTE;
                }
                target.Add(ado);
            }
        }

        private Dictionary<long, HIS_SERE_SERV_EXT> BuildExtDictionary()
        {
            Dictionary<long, HIS_SERE_SERV_EXT> result = new Dictionary<long, HIS_SERE_SERV_EXT>();
            try
            {
                if (rdo.SereSErvExts == null) return result;
                foreach (var ext in rdo.SereSErvExts)
                {
                    if (ext != null && !result.ContainsKey(ext.SERE_SERV_ID)) result[ext.SERE_SERV_ID] = ext;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>Tuổi tại ngày khám: "48 tuổi"; dưới 1 tuổi → "x tháng tuổi".</summary>
        private static string CalculateAgeStr(long dob, long refTime)
        {
            try
            {
                DateTime? dobDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob);
                DateTime? refDate = refTime > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(refTime) : DateTime.Now;
                if (!dobDate.HasValue || !refDate.HasValue) return null;
                int years = refDate.Value.Year - dobDate.Value.Year;
                if (refDate.Value < dobDate.Value.AddYears(years)) years--;
                if (years >= 1) return years + " tuổi";
                int months = (refDate.Value.Year - dobDate.Value.Year) * 12 + refDate.Value.Month - dobDate.Value.Month;
                if (refDate.Value.Day < dobDate.Value.Day) months--;
                return Math.Max(months, 0) + " tháng tuổi";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private static string TimeToDateStr(long time)
        {
            if (time <= 0) return null;
            DateTime? date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(time);
            return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : null;
        }

        private static string FormatNumber(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("0.##", CultureInfo.InvariantCulture) : null;
        }

        private static decimal? ParseDecimal(string text)
        {
            if (String.IsNullOrWhiteSpace(text)) return null;
            decimal d;
            string normalized = text.Trim().Replace(',', '.');
            return Decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out d) ? (decimal?)d : null;
        }

        private static string FirstNotEmpty(params string[] values)
        {
            if (values == null) return null;
            foreach (var v in values)
            {
                if (!String.IsNullOrWhiteSpace(v)) return v;
            }
            return null;
        }

        #endregion
    }
}
