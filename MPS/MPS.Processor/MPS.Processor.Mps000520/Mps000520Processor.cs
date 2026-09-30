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
using MPS.Processor.Mps000520.PDO;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000520
{
    /// <summary>
    /// Giấy khám sức khỏe tâm thần — Mẫu số 04, Phụ lục XXIV Thông tư 25/2026/TT-BYT (HIS_KSK_MENTAL).
    /// Khuôn theo Mps000516. Ngoài key thô còn sinh key phái sinh (Mps000520ExtendSingleKey): ô đánh dấu
    /// Có/Không, giấy tờ tùy thân (CCCD → Hộ chiếu → CMND), thể lực đã định dạng, mã + tên ICD-10, ngày kết luận.
    /// </summary>
    public class Mps000520Processor : AbstractProcessor
    {
        Mps000520PDO rdo;

        public Mps000520Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000520PDO)rdoBase;
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

                objectTag.AddObjectData(store, "KskMental", new List<HIS_KSK_MENTAL>() { rdo.HisKskMental ?? new HIS_KSK_MENTAL() });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst ?? new HIS_DHST() });
                objectTag.AddObjectData(store, "KskGeneral", new List<HIS_KSK_GENERAL>() { rdo.HisKskGeneral ?? new HIS_KSK_GENERAL() });
                objectTag.AddObjectData(store, "Treatment", new List<V_HIS_TREATMENT_4>() { rdo.treatment ?? new V_HIS_TREATMENT_4() });

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
                if (rdo.HisKskMental != null)
                    AddObjectKeyIntoListkey<HIS_KSK_MENTAL>(rdo.HisKskMental, false);
                if (rdo.HisServiceReq != null)
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);

                SetKey(Mps000520ExtendSingleKey.KSK_NUMBER, rdo.HisKskMental != null ? rdo.HisKskMental.KSK_MENTAL_CODE : null);
                SetAdministrativeKeys();
                SetHistoryKeys();
                SetPhysicalKeys();
                SetConclusionKeys();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Mục I — ưu tiên hồ sơ điều trị (V_HIS_TREATMENT_4), thiếu thì lấy từ y lệnh.</summary>
        private void SetAdministrativeKeys()
        {
            var t = rdo.treatment;
            var s = rdo.HisServiceReq;

            long dob = t != null && t.TDL_PATIENT_DOB > 0 ? t.TDL_PATIENT_DOB : (s != null ? s.TDL_PATIENT_DOB : 0);
            bool onlyYear = (t != null ? t.TDL_PATIENT_IS_HAS_NOT_DAY_DOB : (s != null ? s.TDL_PATIENT_IS_HAS_NOT_DAY_DOB : null)) == 1;
            string dobStr = "";
            if (dob > 0)
                dobStr = onlyYear ? dob.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(dob);
            SetKey(Mps000520ExtendSingleKey.DOB_STR, dobStr);
            long atTime = s != null && s.INTRUCTION_TIME > 0 ? s.INTRUCTION_TIME : long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            SetKey(Mps000520ExtendSingleKey.AGE, CalcAge(dob, atTime));

            string gender = t != null && !string.IsNullOrEmpty(t.TDL_PATIENT_GENDER_NAME) ? t.TDL_PATIENT_GENDER_NAME : (s != null ? s.TDL_PATIENT_GENDER_NAME : null);
            gender = (gender ?? "").Trim();
            SetKey(Mps000520ExtendSingleKey.GENDER_MALE_X, gender.Equals("Nam", StringComparison.OrdinalIgnoreCase) ? "X" : "");
            SetKey(Mps000520ExtendSingleKey.GENDER_FEMALE_X, gender.Equals("Nữ", StringComparison.OrdinalIgnoreCase) ? "X" : "");

            // Giấy tờ: CCCD -> Hộ chiếu -> CMND; ngày/nơi cấp theo đúng loại giấy tờ đó
            string idNo = null, idPlace = null;
            long? idDate = null;
            if (t != null)
            {
                if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_CCCD_NUMBER)) { idNo = t.TDL_PATIENT_CCCD_NUMBER; idDate = t.TDL_PATIENT_CCCD_DATE; idPlace = t.TDL_PATIENT_CCCD_PLACE; }
                else if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_PASSPORT_NUMBER)) { idNo = t.TDL_PATIENT_PASSPORT_NUMBER; idDate = t.TDL_PATIENT_PASSPORT_DATE; idPlace = t.TDL_PATIENT_PASSPORT_PLACE; }
                else if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_CMND_NUMBER)) { idNo = t.TDL_PATIENT_CMND_NUMBER; idDate = t.TDL_PATIENT_CMND_DATE; idPlace = t.TDL_PATIENT_CMND_PLACE; }
            }
            if (idNo == null && s != null)
            {
                if (!string.IsNullOrWhiteSpace(s.TDL_PATIENT_CCCD_NUMBER)) { idNo = s.TDL_PATIENT_CCCD_NUMBER; idDate = s.TDL_PATIENT_CCCD_DATE; idPlace = s.TDL_PATIENT_CCCD_PLACE; }
                else if (!string.IsNullOrWhiteSpace(s.TDL_PATIENT_PASSPORT_NUMBER)) { idNo = s.TDL_PATIENT_PASSPORT_NUMBER; idDate = s.TDL_PATIENT_PASSPORT_DATE; idPlace = s.TDL_PATIENT_PASSPORT_PLACE; }
                else if (!string.IsNullOrWhiteSpace(s.TDL_PATIENT_CMND_NUMBER)) { idNo = s.TDL_PATIENT_CMND_NUMBER; idDate = s.TDL_PATIENT_CMND_DATE; idPlace = s.TDL_PATIENT_CMND_PLACE; }
            }
            SetKey(Mps000520ExtendSingleKey.ID_NUMBER, idNo);
            SetKey(Mps000520ExtendSingleKey.ID_DATE_STR, (idDate.HasValue && idDate.Value > 0) ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(idDate.Value) : "");
            SetKey(Mps000520ExtendSingleKey.ID_PLACE, idPlace);

            SetKey(Mps000520ExtendSingleKey.ETHNIC_NAME, t != null ? t.TDL_PATIENT_ETHNIC_NAME : null);
            SetKey(Mps000520ExtendSingleKey.KSK_PATIENT_TYPES_NAME, rdo.KskPatientTypesName);
            SetKey(Mps000520ExtendSingleKey.KSK_PAY_SOURCE_NAME, rdo.KskPaySourceName);

            string abo = t != null ? t.TDL_PATIENT_BLOOD_ABO_CODE : null;
            string rh = t != null ? t.TDL_PATIENT_BLOOD_RH_CODE : null;
            SetKey(Mps000520ExtendSingleKey.BLOOD_TYPE, string.Join(" ", new[] { abo, rh }.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray()));

            SetKey(Mps000520ExtendSingleKey.PROVINCE_NAME, t != null && !string.IsNullOrEmpty(t.TDL_PATIENT_PROVINCE_NAME) ? t.TDL_PATIENT_PROVINCE_NAME : (s != null ? s.TDL_PATIENT_PROVINCE_NAME : null));
            SetKey(Mps000520ExtendSingleKey.COMMUNE_NAME, t != null && !string.IsNullOrEmpty(t.TDL_PATIENT_COMMUNE_NAME) ? t.TDL_PATIENT_COMMUNE_NAME : (s != null ? s.TDL_PATIENT_COMMUNE_NAME : null));
            SetKey(Mps000520ExtendSingleKey.ADDRESS_DETAIL, rdo.KskPatient != null && !string.IsNullOrEmpty(rdo.KskPatient.ADDRESS)
                ? rdo.KskPatient.ADDRESS : (t != null ? t.TDL_PATIENT_ADDRESS : (s != null ? s.TDL_PATIENT_ADDRESS : null)));
            string mobile = t != null && !string.IsNullOrEmpty(t.TDL_PATIENT_MOBILE) ? t.TDL_PATIENT_MOBILE : (t != null ? t.TDL_PATIENT_PHONE : null);
            if (string.IsNullOrEmpty(mobile) && s != null) mobile = !string.IsNullOrEmpty(s.TDL_PATIENT_MOBILE) ? s.TDL_PATIENT_MOBILE : s.TDL_PATIENT_PHONE;
            SetKey(Mps000520ExtendSingleKey.PHONE, mobile);
            SetKey(Mps000520ExtendSingleKey.EXAM_REASON, s != null ? s.HOSPITALIZATION_REASON : null);
        }

        /// <summary>Mục II — Không / Có.</summary>
        private void SetHistoryKeys()
        {
            var k = rdo.HisKskMental;
            long? fam = k != null ? N(k.IS_FAMILY_MENTAL_HISTORY) : null;
            long? per = k != null ? N(k.IS_PERSONAL_MENTAL_HISTORY) : null;
            SetKey(Mps000520ExtendSingleKey.FAMILY_HISTORY_NO_X, fam == 0 ? "X" : "");
            SetKey(Mps000520ExtendSingleKey.FAMILY_HISTORY_YES_X, fam == 1 ? "X" : "");
            SetKey(Mps000520ExtendSingleKey.PERSONAL_HISTORY_NO_X, per == 0 ? "X" : "");
            SetKey(Mps000520ExtendSingleKey.PERSONAL_HISTORY_YES_X, per == 1 ? "X" : "");
        }

        /// <summary>Mục III.1 — thể lực từ HIS_DHST, bỏ phần thập phân thừa (170.00 -> 170).</summary>
        private void SetPhysicalKeys()
        {
            var d = rdo.HisDhst;
            SetKey(Mps000520ExtendSingleKey.PULSE_STR, d != null && d.PULSE.HasValue ? d.PULSE.Value.ToString() : "");
            SetKey(Mps000520ExtendSingleKey.BLOOD_PRESSURE_MAX_STR, d != null && d.BLOOD_PRESSURE_MAX.HasValue ? d.BLOOD_PRESSURE_MAX.Value.ToString() : "");
            SetKey(Mps000520ExtendSingleKey.BLOOD_PRESSURE_MIN_STR, d != null && d.BLOOD_PRESSURE_MIN.HasValue ? d.BLOOD_PRESSURE_MIN.Value.ToString() : "");
            SetKey(Mps000520ExtendSingleKey.HEIGHT_STR, d != null && d.HEIGHT.HasValue ? d.HEIGHT.Value.ToString("0.##", CultureInfo.InvariantCulture) : "");
            SetKey(Mps000520ExtendSingleKey.WEIGHT_STR, d != null && d.WEIGHT.HasValue ? d.WEIGHT.Value.ToString("0.##", CultureInfo.InvariantCulture) : "");
        }

        /// <summary>Mục V — V.1 Có/Không + ICD-10 (HIS_KSK_MENTAL); bác sĩ khám + ngày kết luận (HIS_KSK_GENERAL).</summary>
        private void SetConclusionKeys()
        {
            var k = rdo.HisKskMental;
            long? dis = k != null ? N(k.IS_MENTAL_DISORDER) : null;
            SetKey(Mps000520ExtendSingleKey.MENTAL_DISORDER_NO_X, dis == 0 ? "X" : "");
            SetKey(Mps000520ExtendSingleKey.MENTAL_DISORDER_YES_X, dis == 1 ? "X" : "");
            SetKey(Mps000520ExtendSingleKey.MENTAL_DISORDER_ICD_TEXT, dis == 1 && k != null
                ? JoinIcd(k.MENTAL_DISORDER_ICD_CODE, k.MENTAL_DISORDER_ICD_NAME) : "");

            var g = rdo.HisKskGeneral;
            SetKey(Mps000520ExtendSingleKey.CONCLUDER_USERNAME, g != null ? g.CONCLUDER_USERNAME : null);
            SetKey(Mps000520ExtendSingleKey.CONCLUDER_LOGINNAME, g != null ? g.CONCLUDER_LOGINNAME : null);
            long? ct = g != null ? N(g.CONCLUSION_TIME) : null;
            string ctStr = (ct.HasValue && ct.Value > 0) ? ct.Value.ToString() : "";
            SetKey(Mps000520ExtendSingleKey.CONCLUSION_TIME_STR, ctStr.Length >= 8 ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(ct.Value) : "");
            SetKey(Mps000520ExtendSingleKey.CONCLUSION_DAY, ctStr.Length >= 8 ? ctStr.Substring(6, 2) : "......");
            SetKey(Mps000520ExtendSingleKey.CONCLUSION_MONTH, ctStr.Length >= 8 ? ctStr.Substring(4, 2) : "......");
            SetKey(Mps000520ExtendSingleKey.CONCLUSION_YEAR, ctStr.Length >= 8 ? ctStr.Substring(0, 4) : "20......");
        }

        // ===== Helpers =====

        private void SetKey(string key, object value)
        {
            SetSingleKey(new KeyValue(key, value ?? ""));
        }

        /// <summary>Đọc giá trị số bất kể EF sinh ra short?/long?/decimal? -> long?.</summary>
        private static long? N(object v)
        {
            if (v == null) return null;
            long r;
            return long.TryParse(v.ToString(), out r) ? (long?)r : (long?)null;
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

        /// <summary>Ghép mã + tên ICD (cùng thứ tự, ngăn bởi dấu ;) thành "F20.0 - Tên; F32 - Tên".</summary>
        private static string JoinIcd(string codes, string names)
        {
            try
            {
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
