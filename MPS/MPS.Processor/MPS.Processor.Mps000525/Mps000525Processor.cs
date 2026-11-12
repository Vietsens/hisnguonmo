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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000525.PDO;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000525
{
    /// <summary>
    /// Giấy khám sức khỏe định kỳ đối với người hành nghề lái xe ô tô — Mẫu 06 Thông tư 25/2026/TT-BYT
    /// (HIS_KSK_PRO_DRIVER). Khuôn theo Mps000520. Ngoài key thô còn sinh key phái sinh (xem
    /// Mps000525ExtendSingleKey): ô đánh dấu, ICD mã + tên, mức phân loại I–V, tên bác sĩ từng mục,
    /// 21 câu tiền sử, ngày kết luận.
    /// </summary>
    public class Mps000525Processor : AbstractProcessor
    {
        Mps000525PDO rdo;

        /// <summary>14 mục khám lâm sàng — tiền tố cột của HIS_KSK_PRO_DRIVER.</summary>
        private static readonly string[] SECTIONS = new string[]
        {
            "EXAM_CIRCULATION", "EXAM_RESPIRATORY", "EXAM_DIGESTION", "EXAM_KIDNEY_URO", "EXAM_OEND",
            "EXAM_MUSCLE_BONE", "EXAM_NEUROLOGICAL", "EXAM_MENTAL", "EXAM_SURGERY", "EXAM_DERMATOLOGY",
            "EXAM_OBSTETRIC", "EXAM_EYE", "EXAM_ENT", "EXAM_STOMATOLOGY"
        };

        /// <summary>Cột người thực hiện cận lâm sàng -> key họ tên in ra.</summary>
        private static readonly string[][] CLS_PERFORMERS = new string[][]
        {
            new[] { "TEST_DRUG_LOGINNAME", "TEST_DRUG_DOCTOR" },
            new[] { "TEST_FEMALE_LOGINNAME", "TEST_FEMALE_DOCTOR" },
            new[] { "TEST_BLOOD_LOGINNAME", "TEST_BLOOD_DOCTOR" },
            new[] { "TEST_URINE_LOGINNAME", "TEST_URINE_DOCTOR" },
            new[] { "DIIM_LOGINNAME", "DIIM_DOCTOR" },
            new[] { "OTHER_TEST_LOGINNAME", "OTHER_TEST_DOCTOR" }
        };

        public Mps000525Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000525PDO)rdoBase;
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

                objectTag.AddObjectData(store, "KskProDriver", new List<HIS_KSK_PRO_DRIVER>() { rdo.HisKskProDriver ?? new HIS_KSK_PRO_DRIVER() });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst ?? new HIS_DHST() });
                objectTag.AddObjectData(store, "KskGeneral", new List<HIS_KSK_GENERAL>() { rdo.HisKskGeneral ?? new HIS_KSK_GENERAL() });
                objectTag.AddObjectData(store, "Treatment", new List<V_HIS_TREATMENT_4>() { rdo.treatment ?? new V_HIS_TREATMENT_4() });
                objectTag.AddObjectData(store, "Diseases", rdo.Diseases ?? new List<Mps000525DiseaseADO>());

                SetSingleKey();
                SetSignatureKeyImageByCFG();

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

        private void SetSingleKey()
        {
            try
            {
                if (rdo.KskServiceReq != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_SERVICE_REQ>(rdo.KskServiceReq, "SREQ_", false);
                if (rdo.KskPatient != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_PATIENT>(rdo.KskPatient, "PATIENT_", false);
                if (rdo.HisDhst != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_DHST>(rdo.HisDhst, "DHST_", false);
                if (rdo.HisKskProDriver != null)
                    AddObjectKeyIntoListkey<HIS_KSK_PRO_DRIVER>(rdo.HisKskProDriver, false);
                if (rdo.HisServiceReq != null)
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);

                SetKey(Mps000525ExtendSingleKey.KSK_NUMBER, rdo.HisKskProDriver != null ? rdo.HisKskProDriver.KSK_PRO_DRIVER_CODE : null);
                SetAdministrativeKeys();
                SetHistoryKeys();
                SetPhysicalKeys();
                SetClinicalKeys();
                SetSubclinicalKeys();
                SetConclusionKeys();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Hành chính + đối tượng - chi trả — ưu tiên hồ sơ điều trị, thiếu thì lấy từ y lệnh.</summary>
        private void SetAdministrativeKeys()
        {
            var t = rdo.treatment;
            var s = rdo.HisServiceReq;
            var k = rdo.HisKskProDriver;

            long dob = t != null && t.TDL_PATIENT_DOB > 0 ? t.TDL_PATIENT_DOB : (s != null ? s.TDL_PATIENT_DOB : 0);
            bool onlyYear = (t != null ? t.TDL_PATIENT_IS_HAS_NOT_DAY_DOB : (s != null ? s.TDL_PATIENT_IS_HAS_NOT_DAY_DOB : null)) == 1;
            string dobStr = "";
            if (dob > 0)
                dobStr = onlyYear ? dob.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(dob);
            SetKey(Mps000525ExtendSingleKey.DOB_STR, dobStr);
            long atTime = s != null && s.INTRUCTION_TIME > 0 ? s.INTRUCTION_TIME : long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            SetKey(Mps000525ExtendSingleKey.AGE, CalcAge(dob, atTime));

            string gender = t != null && !string.IsNullOrEmpty(t.TDL_PATIENT_GENDER_NAME) ? t.TDL_PATIENT_GENDER_NAME : (s != null ? s.TDL_PATIENT_GENDER_NAME : null);
            gender = (gender ?? "").Trim();
            SetKey(Mps000525ExtendSingleKey.GENDER_MALE_X, gender.Equals("Nam", StringComparison.OrdinalIgnoreCase) ? "X" : "");
            SetKey(Mps000525ExtendSingleKey.GENDER_FEMALE_X, gender.Equals("Nữ", StringComparison.OrdinalIgnoreCase) ? "X" : "");

            // Số CCCD / mã số định danh / hộ chiếu
            string idNo = null;
            if (t != null)
                idNo = FirstNotEmpty(t.TDL_PATIENT_CCCD_NUMBER, t.TDL_PATIENT_PASSPORT_NUMBER, t.TDL_PATIENT_CMND_NUMBER);
            if (string.IsNullOrEmpty(idNo) && s != null)
                idNo = FirstNotEmpty(s.TDL_PATIENT_CCCD_NUMBER, s.TDL_PATIENT_PASSPORT_NUMBER, s.TDL_PATIENT_CMND_NUMBER);
            SetKey(Mps000525ExtendSingleKey.ID_NUMBER, idNo);
            SetKey(Mps000525ExtendSingleKey.HEIN_CARD_NUMBER, t != null ? t.TDL_HEIN_CARD_NUMBER : null);

            SetKey(Mps000525ExtendSingleKey.ETHNIC_NAME, t != null ? t.TDL_PATIENT_ETHNIC_NAME : null);
            string abo = t != null ? t.TDL_PATIENT_BLOOD_ABO_CODE : null;
            string rh = t != null ? t.TDL_PATIENT_BLOOD_RH_CODE : null;
            SetKey(Mps000525ExtendSingleKey.BLOOD_TYPE, string.Join(" ", new[] { abo, rh }.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray()));

            SetKey(Mps000525ExtendSingleKey.PROVINCE_NAME, t != null && !string.IsNullOrEmpty(t.TDL_PATIENT_PROVINCE_NAME) ? t.TDL_PATIENT_PROVINCE_NAME : (s != null ? s.TDL_PATIENT_PROVINCE_NAME : null));
            SetKey(Mps000525ExtendSingleKey.COMMUNE_NAME, t != null && !string.IsNullOrEmpty(t.TDL_PATIENT_COMMUNE_NAME) ? t.TDL_PATIENT_COMMUNE_NAME : (s != null ? s.TDL_PATIENT_COMMUNE_NAME : null));
            SetKey(Mps000525ExtendSingleKey.ADDRESS_DETAIL, rdo.KskPatient != null && !string.IsNullOrEmpty(rdo.KskPatient.ADDRESS)
                ? rdo.KskPatient.ADDRESS : (t != null ? t.TDL_PATIENT_ADDRESS : (s != null ? s.TDL_PATIENT_ADDRESS : null)));
            SetKey(Mps000525ExtendSingleKey.CAREER_NAME, t != null ? t.TDL_PATIENT_CAREER_NAME : null);
            SetKey(Mps000525ExtendSingleKey.WORK_PLACE, t != null ? FirstNotEmpty(t.TDL_PATIENT_WORK_PLACE_NAME, t.TDL_PATIENT_WORK_PLACE) : null);
            string mobile = t != null ? FirstNotEmpty(t.TDL_PATIENT_MOBILE, t.TDL_PATIENT_PHONE) : null;
            if (string.IsNullOrEmpty(mobile) && s != null) mobile = FirstNotEmpty(s.TDL_PATIENT_MOBILE, s.TDL_PATIENT_PHONE);
            SetKey(Mps000525ExtendSingleKey.PHONE, mobile);
            SetKey(Mps000525ExtendSingleKey.EXAM_REASON, s != null ? s.HOSPITALIZATION_REASON : null);
            SetKey(Mps000525ExtendSingleKey.LICENSE_CLASS_NAME, rdo.LicenseClassName);

            // Đối tượng - chi trả. Mẫu 06 in 3 ô: Người sử dụng lao động / Người dân tự chi trả / Nguồn khác;
            // danh sách nguồn chi trả dùng chung (QĐ 2062) -> 4 = Người sử dụng lao động, còn lại in ở "Nguồn khác".
            SetKey(Mps000525ExtendSingleKey.KSK_PATIENT_TYPES_NAME, rdo.KskPatientTypesName);
            SetKey(Mps000525ExtendSingleKey.KSK_PAY_SOURCE_NAME, rdo.KskPaySourceName);
            long? pay = k != null ? N(k.KSK_PAY_SOURCE) : null;
            SetKey(Mps000525ExtendSingleKey.PAY_SOURCE_EMPLOYER_X, pay == 4 ? "X" : "");
            SetKey(Mps000525ExtendSingleKey.PAY_SOURCE_OTHER_X, pay.HasValue && pay != 4 ? "X" : "");
            SetKey(Mps000525ExtendSingleKey.PAY_SOURCE_OTHER_NAME, pay.HasValue && pay != 4 ? rdo.KskPaySourceName : "");
            long? place = k != null ? N(k.EXAM_PLACE_TYPE) : null;
            SetKey(Mps000525ExtendSingleKey.EXAM_PLACE_FACILITY_X, place == 1 ? "X" : "");
            SetKey(Mps000525ExtendSingleKey.EXAM_PLACE_MOBILE_X, place == 2 ? "X" : "");
        }

        /// <summary>Tiền sử: gia đình, 22 câu bản thân, đang điều trị, thai sản.</summary>
        private void SetHistoryKeys()
        {
            var k = rdo.HisKskProDriver;
            SetYesNo(Mps000525ExtendSingleKey.FAMILY_HISTORY_YES_X, Mps000525ExtendSingleKey.FAMILY_HISTORY_NO_X, k != null ? N(k.IS_FAMILY_HISTORY) : null);
            SetYesNo(Mps000525ExtendSingleKey.TREATING_YES_X, Mps000525ExtendSingleKey.TREATING_NO_X, k != null ? N(k.IS_TREATING) : null);
            SetYesNo(Mps000525ExtendSingleKey.MATERNITY_YES_X, Mps000525ExtendSingleKey.MATERNITY_NO_X, k != null ? N(k.IS_MATERNITY_HISTORY) : null);

            var list = rdo.Diseases ?? new List<Mps000525DiseaseADO>();
            for (int i = 1; i <= 21; i++)
            {
                var d = list.FirstOrDefault(o => o != null && o.STT == i);
                SetKey("DISEASE_" + i + "_NAME", d != null ? d.DISEASE_TYPE_NAME : "");
                SetKey("DISEASE_" + i + "_YES_X", d != null && d.IS_YES_NO == "1" ? "X" : "");
                SetKey("DISEASE_" + i + "_NO_X", d != null && d.IS_YES_NO == "0" ? "X" : "");
            }
            SetKey(Mps000525ExtendSingleKey.DISEASE_22_YES_X, k != null && !string.IsNullOrWhiteSpace(k.OTHER_DISEASE) ? "X" : "");
        }

        /// <summary>Khám thể lực — số đo từ HIS_DHST, bỏ phần thập phân thừa; phân loại thể lực I–V.</summary>
        private void SetPhysicalKeys()
        {
            var d = rdo.HisDhst;
            SetKey(Mps000525ExtendSingleKey.HEIGHT_STR, d != null && d.HEIGHT.HasValue ? d.HEIGHT.Value.ToString("0.##", CultureInfo.InvariantCulture) : "");
            SetKey(Mps000525ExtendSingleKey.WEIGHT_STR, d != null && d.WEIGHT.HasValue ? d.WEIGHT.Value.ToString("0.##", CultureInfo.InvariantCulture) : "");
            decimal? bmi = d != null ? d.VIR_BMI : null;
            if (!bmi.HasValue && d != null && d.HEIGHT.HasValue && d.WEIGHT.HasValue && d.HEIGHT.Value > 0)
            {
                decimal h = d.HEIGHT.Value / 100m;
                bmi = d.WEIGHT.Value / (h * h);
            }
            SetKey(Mps000525ExtendSingleKey.BMI_STR, bmi.HasValue ? Math.Round(bmi.Value, 1).ToString("0.#", CultureInfo.InvariantCulture) : "");
            SetKey(Mps000525ExtendSingleKey.PULSE_STR, d != null && d.PULSE.HasValue ? d.PULSE.Value.ToString() : "");
            SetKey(Mps000525ExtendSingleKey.BLOOD_PRESSURE_STR, d != null && d.BLOOD_PRESSURE_MAX.HasValue && d.BLOOD_PRESSURE_MIN.HasValue
                ? d.BLOOD_PRESSURE_MAX.Value + "/" + d.BLOOD_PRESSURE_MIN.Value : "");
            SetKey(Mps000525ExtendSingleKey.BREATH_RATE_STR, d != null && d.BREATH_RATE.HasValue ? d.BREATH_RATE.Value.ToString("0.##", CultureInfo.InvariantCulture) : "");
            SetRankKeys("DHST_RANK", rdo.HisKskProDriver != null ? rdo.HisKskProDriver.DHST_RANK : null);
        }

        /// <summary>14 mục khám lâm sàng — đọc cột theo tiền tố bằng phản chiếu.</summary>
        private void SetClinicalKeys()
        {
            var k = rdo.HisKskProDriver;
            foreach (string p in SECTIONS)
            {
                long? isNormal = N(Get(k, p + "_IS_NORMAL"));
                string preCode = Str(Get(k, p + "_PRE_ICD_CODE"));
                string preName = Str(Get(k, p + "_PRE_ICD_NAME"));
                string code = Str(Get(k, p + "_ICD_CODE"));
                string name = Str(Get(k, p + "_ICD_NAME"));
                SetKey(p + "_NORMAL_X", isNormal == 1 ? "X" : "");
                SetKey(p + "_PRE_X", !string.IsNullOrWhiteSpace(preCode) || !string.IsNullOrWhiteSpace(preName) ? "X" : "");
                SetKey(p + "_PRE_ICD_TEXT", JoinIcd(preCode, preName));
                SetKey(p + "_FINAL_X", !string.IsNullOrWhiteSpace(code) || !string.IsNullOrWhiteSpace(name) ? "X" : "");
                SetKey(p + "_FINAL_ICD_TEXT", JoinIcd(code, name));
                SetRankKeys(p + "_RANK", N(Get(k, p + "_RANK")));
                SetKey(p + "_DOCTOR", EmployeeName(Str(Get(k, p + "_LOGINNAME"))));
            }
            SetKey(Mps000525ExtendSingleKey.EXAM_OBSTETRIC_REFUSE_X, k != null && N(k.IS_REFUSE_OBSTETRIC) == 1 ? "X" : "");
        }

        /// <summary>Họ tên người thực hiện từng nhóm cận lâm sàng.</summary>
        private void SetSubclinicalKeys()
        {
            var k = rdo.HisKskProDriver;
            foreach (var pr in CLS_PERFORMERS)
                SetKey(pr[1], EmployeeName(Str(Get(k, pr[0]))));
        }

        /// <summary>Kết luận lái xe (5 trường hợp), bệnh tật ICD, phân loại sức khỏe, đề nghị, người kết luận.</summary>
        private void SetConclusionKeys()
        {
            var k = rdo.HisKskProDriver;
            long? type = k != null ? N(k.DRIVER_CONCLUSION_TYPE) : null;
            for (int i = 1; i <= 5; i++)
                SetKey("DRIVER_CONCLUSION_" + i + "_X", type == i ? "X" : "");
            // Hạng GPLX chỉ ghi ở dòng kết luận đã chọn (trường hợp 1–3 có chỗ "ghi rõ hạng").
            for (int i = 1; i <= 3; i++)
                SetKey("CONCLUSION_LICENSE_" + i, type == i ? (rdo.LicenseClassName ?? "") : "");

            SetKey(Mps000525ExtendSingleKey.CONCLUSION_NORMAL_X, k != null && N(k.CONCLUSION_IS_NORMAL) == 1 ? "X" : "");
            bool hasPre = k != null && (!string.IsNullOrWhiteSpace(k.CONCLUSION_PRE_ICD_CODE) || !string.IsNullOrWhiteSpace(k.CONCLUSION_PRE_ICD_NAME));
            bool hasFinal = k != null && (!string.IsNullOrWhiteSpace(k.CONCLUSION_ICD_CODE) || !string.IsNullOrWhiteSpace(k.CONCLUSION_ICD_NAME));
            SetKey(Mps000525ExtendSingleKey.CONCLUSION_PRE_X, hasPre ? "X" : "");
            SetKey(Mps000525ExtendSingleKey.CONCLUSION_PRE_ICD_TEXT, hasPre ? JoinIcd(k.CONCLUSION_PRE_ICD_CODE, k.CONCLUSION_PRE_ICD_NAME) : "");
            SetKey(Mps000525ExtendSingleKey.CONCLUSION_FINAL_X, hasFinal ? "X" : "");
            SetKey(Mps000525ExtendSingleKey.CONCLUSION_FINAL_ICD_TEXT, hasFinal ? JoinIcd(k.CONCLUSION_ICD_CODE, k.CONCLUSION_ICD_NAME) : "");

            SetRankKeys("HEALTH_RANK", k != null ? k.HEALTH_EXAM_RANK_ID : null);
            long? suggest = k != null ? N(k.SUGGEST_TYPE) : null;
            for (int i = 1; i <= 5; i++)
                SetKey("SUGGEST_" + i + "_X", suggest == i ? "X" : "");

            var g = rdo.HisKskGeneral;
            SetKey(Mps000525ExtendSingleKey.CONCLUDER_USERNAME, g != null ? g.CONCLUDER_USERNAME : null);
            SetKey(Mps000525ExtendSingleKey.CONCLUDER_LOGINNAME, g != null ? g.CONCLUDER_LOGINNAME : null);
            long? ct = g != null ? N(g.CONCLUSION_TIME) : null;
            string ctStr = (ct.HasValue && ct.Value > 0) ? ct.Value.ToString() : "";
            SetKey(Mps000525ExtendSingleKey.CONCLUSION_TIME_STR, ctStr.Length >= 8 ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(ct.Value) : "");
            SetKey(Mps000525ExtendSingleKey.CONCLUSION_DAY, ctStr.Length >= 8 ? ctStr.Substring(6, 2) : "......");
            SetKey(Mps000525ExtendSingleKey.CONCLUSION_MONTH, ctStr.Length >= 8 ? ctStr.Substring(4, 2) : "......");
            SetKey(Mps000525ExtendSingleKey.CONCLUSION_YEAR, ctStr.Length >= 8 ? ctStr.Substring(0, 4) : "20......");
        }

        // ===== Helpers =====

        private void SetKey(string key, object value)
        {
            SetSingleKey(new KeyValue(key, value ?? ""));
        }

        private void SetYesNo(string yesKey, string noKey, long? value)
        {
            SetKey(yesKey, value == 1 ? "X" : "");
            SetKey(noKey, value == 0 ? "X" : "");
        }

        /// <summary>{prefix}_1_X .. {prefix}_5_X theo mức phân loại của ID trong danh mục.</summary>
        private void SetRankKeys(string prefix, long? rankId)
        {
            int level = RankLevel(rankId);
            for (int i = 1; i <= 5; i++)
                SetKey(prefix + "_" + i + "_X", level == i ? "X" : "");
        }

        /// <summary>
        /// Mức I–V của phân loại sức khỏe — cùng quy ước "PL:Lx" của màn nhập: mã "1".."5" trước,
        /// không có thì theo thứ tự mã trong danh mục.
        /// </summary>
        private int RankLevel(long? rankId)
        {
            try
            {
                if (!rankId.HasValue || rdo.HealthExamRanks == null || rdo.HealthExamRanks.Count == 0) return 0;
                var rank = rdo.HealthExamRanks.FirstOrDefault(o => o.ID == rankId.Value);
                if (rank == null) return 0;
                int code;
                if (int.TryParse((rank.HEALTH_EXAM_RANK_CODE ?? "").Trim(), out code) && code >= 1 && code <= 5) return code;
                var ordered = rdo.HealthExamRanks
                    .OrderBy(o => { long n; return long.TryParse((o.HEALTH_EXAM_RANK_CODE ?? "").Trim(), out n) ? n : long.MaxValue; })
                    .ThenBy(o => o.HEALTH_EXAM_RANK_CODE)
                    .ToList();
                int idx = ordered.FindIndex(o => o.ID == rank.ID);
                return (idx >= 0 && idx < 5) ? idx + 1 : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        private string EmployeeName(string loginname)
        {
            if (string.IsNullOrWhiteSpace(loginname)) return "";
            string name;
            if (rdo.EmployeeNames != null && rdo.EmployeeNames.TryGetValue(loginname, out name) && !string.IsNullOrWhiteSpace(name))
                return name;
            return loginname;
        }

        private static object Get(object obj, string property)
        {
            if (obj == null) return null;
            var pi = obj.GetType().GetProperty(property);
            return pi != null ? pi.GetValue(obj, null) : null;
        }

        private static string Str(object v)
        {
            return v != null ? v.ToString() : null;
        }

        /// <summary>Đọc giá trị số bất kể EF sinh ra short?/long?/decimal? -> long?.</summary>
        private static long? N(object v)
        {
            if (v == null) return null;
            long r;
            return long.TryParse(v.ToString(), out r) ? (long?)r : (long?)null;
        }

        private static string FirstNotEmpty(params string[] values)
        {
            foreach (string v in values)
                if (!string.IsNullOrWhiteSpace(v)) return v;
            return null;
        }

        /// <summary>Tuổi tròn năm tại thời điểm khám (yyyyMMddHHmmss).</summary>
        private static string CalcAge(long dob, long atTime)
        {
            try
            {
                if (dob <= 0 || atTime <= 0) return "";
                string d = dob.ToString(), a = atTime.ToString();
                int age = int.Parse(a.Substring(0, 4)) - int.Parse(d.Substring(0, 4));
                if (string.CompareOrdinal(a.Substring(4, 4), d.Substring(4, 4)) < 0) age--;
                return age >= 0 ? age.ToString() : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>Ghép mã + tên ICD (cùng thứ tự, ngăn bởi dấu ;) thành "I10 - Tên; E11 - Tên".</summary>
        private static string JoinIcd(string codes, string names)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codes) && string.IsNullOrWhiteSpace(names)) return "";
                var c = (codes ?? "").Split(';').Select(o => o.Trim()).ToList();
                var n = (names ?? "").Split(';').Select(o => o.Trim()).ToList();
                var parts = new List<string>();
                for (int i = 0; i < Math.Max(c.Count, n.Count); i++)
                {
                    string ci = i < c.Count ? c[i] : "", ni = i < n.Count ? n[i] : "";
                    if (ci.Length == 0 && ni.Length == 0) continue;
                    parts.Add(ci.Length > 0 && ni.Length > 0 ? ci + " - " + ni : ci + ni);
                }
                return string.Join("; ", parts.ToArray());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return (codes ?? "") + " " + (names ?? "");
            }
        }
    }
}
