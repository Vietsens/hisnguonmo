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
using System.Linq;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000526.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000526
{
    /// <summary>
    /// Giấy khám sức khỏe và khám sức khỏe định kỳ dùng cho người từ đủ 18 tuổi trở lên (hospital file "Mẫu 3" — SYT TP.HCM,
    /// body Mẫu 03 TT 25/2026/TT-BYT). Sibling of Mps000523 (elderly form): same saved data, no interview section,
    /// internal medicine printed part by part, two lab blocks sharing one set of keys.
    /// Columns of HIS_KSK_SYT_HCM / HIS_KSK_GENERAL are read by name (Mps000526Util) so an older MOS.EFMODEL
    /// at the hospital cannot break the whole print with MissingMethodException.
    /// </summary>
    public partial class Mps000526Processor : AbstractProcessor
    {
        Mps000526PDO rdo;

        public Mps000526Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000526PDO)rdoBase;
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

                objectTag.AddObjectData(store, "KskOverEighteen", new List<HIS_KSK_OVER_EIGHTEEN>() { rdo.HisKskOverEighteen ?? new HIS_KSK_OVER_EIGHTEEN() });
                objectTag.AddObjectData(store, "KskSytHcm", new List<HIS_KSK_SYT_HCM>() { rdo.HisKskSytHcm ?? new HIS_KSK_SYT_HCM() });
                objectTag.AddObjectData(store, "KskGeneral", new List<HIS_KSK_GENERAL>() { rdo.HisKskGeneral ?? new HIS_KSK_GENERAL() });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst ?? new HIS_DHST() });
                objectTag.AddObjectData(store, "Treatment", new List<V_HIS_TREATMENT_4>() { rdo.Treatment ?? new V_HIS_TREATMENT_4() });

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
                // Derived keys first: SetSingleKey keeps the first value, raw keys must not shadow them.
                SetKey(Mps000526ExtendSingleKey.KSK_NUMBER, Mps000526Util.Str(rdo.HisKskOverEighteen, "KSK_OVER_EIGHTEEN_CODE"));
                SetAdministrativeKeys();
                SetHistoryKeys();
                SetObjectPaymentKeys();
                SetPhysicalKeys();
                SetSpecialtyKeys();
                SetEyeEntTeethKeys();
                SetSubclinicalKeys();
                SetConclusionKeys();

                if (rdo.HisServiceReq != null)
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                if (rdo.KskServiceReq != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_SERVICE_REQ>(rdo.KskServiceReq, "SREQ_", false);
                if (rdo.KskPatient != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_PATIENT>(rdo.KskPatient, "PATIENT_", false);
                if (rdo.HisDhst != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_DHST>(rdo.HisDhst, "DHST_", false);
                if (rdo.HisKskOverEighteen != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_KSK_OVER_EIGHTEEN>(rdo.HisKskOverEighteen, "KSK_", false);
                if (rdo.HisKskSytHcm != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_KSK_SYT_HCM>(rdo.HisKskSytHcm, "HCM_", false);
                if (rdo.HisKskGeneral != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_KSK_GENERAL>(rdo.HisKskGeneral, "GENERAL_", false);
                if (rdo.Treatment != null)
                    AddObjectKeyIntoListkeyWithPrefix<V_HIS_TREATMENT_4>(rdo.Treatment, "TREATMENT_", false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ THÔNG TIN HÀNH CHÍNH

        private void SetAdministrativeKeys()
        {
            try
            {
                var t = rdo.Treatment;
                var s = rdo.HisServiceReq;
                var p = rdo.KskPatient;

                string name = s != null ? s.TDL_PATIENT_NAME : null;
                SetKey(Mps000526ExtendSingleKey.PATIENT_NAME_UPPER, (name ?? "").ToUpper());

                long? genderId = s != null && s.TDL_PATIENT_GENDER_ID.HasValue ? s.TDL_PATIENT_GENDER_ID
                    : (t != null ? (long?)t.TDL_PATIENT_GENDER_ID : null);
                SetX(Mps000526ExtendSingleKey.GENDER_MALE_X, genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE);
                SetX(Mps000526ExtendSingleKey.GENDER_FEMALE_X, genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE);

                long dob = s != null && s.TDL_PATIENT_DOB > 0 ? s.TDL_PATIENT_DOB : (t != null ? t.TDL_PATIENT_DOB : 0);
                bool onlyYear = (s != null ? s.TDL_PATIENT_IS_HAS_NOT_DAY_DOB : (t != null ? t.TDL_PATIENT_IS_HAS_NOT_DAY_DOB : null)) == 1;
                SetKey(Mps000526ExtendSingleKey.DOB_STR, dob <= 0 ? "" : (onlyYear ? dob.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(dob)));
                long atTime = s != null && s.INTRUCTION_TIME > 0 ? s.INTRUCTION_TIME : long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                SetKey(Mps000526ExtendSingleKey.AGE, Mps000526Util.Age(dob, atTime));

                SetKey(Mps000526ExtendSingleKey.ETHNIC_NAME, Mps000526Util.First(t != null ? t.TDL_PATIENT_ETHNIC_NAME : null, p != null ? p.ETHNIC_NAME : null));
                string abo = Mps000526Util.First(t != null ? t.TDL_PATIENT_BLOOD_ABO_CODE : null, p != null ? p.BLOOD_ABO_CODE : null);
                string rh = Mps000526Util.First(t != null ? t.TDL_PATIENT_BLOOD_RH_CODE : null, p != null ? p.BLOOD_RH_CODE : null);
                SetKey(Mps000526ExtendSingleKey.BLOOD_TYPE, Mps000526Util.Join(" ", abo, rh));

                string idNo = Mps000526Util.First(
                    t != null ? t.TDL_PATIENT_CCCD_NUMBER : null, s != null ? s.TDL_PATIENT_CCCD_NUMBER : null, p != null ? p.CCCD_NUMBER : null,
                    t != null ? t.TDL_PATIENT_PASSPORT_NUMBER : null, s != null ? s.TDL_PATIENT_PASSPORT_NUMBER : null, p != null ? p.PASSPORT_NUMBER : null,
                    t != null ? t.TDL_PATIENT_CMND_NUMBER : null, s != null ? s.TDL_PATIENT_CMND_NUMBER : null, p != null ? p.CMND_NUMBER : null);
                SetBoxes("ID_", idNo, 12, Mps000526ExtendSingleKey.ID_NUMBER, Mps000526ExtendSingleKey.ID_NUMBER_LONG);

                string hein = Mps000526Util.First(t != null ? t.TDL_HEIN_CARD_NUMBER : null, s != null ? s.TDL_HEIN_CARD_NUMBER : null, p != null ? p.TDL_HEIN_CARD_NUMBER : null);
                SetBoxes("HEIN_", hein, 15, Mps000526ExtendSingleKey.HEIN_CARD_NUMBER, Mps000526ExtendSingleKey.HEIN_CARD_NUMBER_LONG);

                // Current address (HT_*) of the patient; falls back to the permanent address.
                bool hasCurrent = p != null && (!string.IsNullOrWhiteSpace(p.HT_ADDRESS) || !string.IsNullOrWhiteSpace(p.HT_COMMUNE_NAME));
                SetKey(Mps000526ExtendSingleKey.ADDRESS_DETAIL, hasCurrent ? p.HT_ADDRESS
                    : Mps000526Util.First(p != null ? p.ADDRESS : null, t != null ? t.TDL_PATIENT_ADDRESS : null, s != null ? s.TDL_PATIENT_ADDRESS : null));
                SetKey(Mps000526ExtendSingleKey.ADDRESS_COMMUNE, hasCurrent
                    ? Mps000526Util.Join(", ", p.HT_COMMUNE_NAME, p.HT_PROVINCE_NAME)
                    : Mps000526Util.Join(", ",
                        Mps000526Util.First(p != null ? p.COMMUNE_NAME : null, t != null ? t.TDL_PATIENT_COMMUNE_NAME : null),
                        Mps000526Util.First(p != null ? p.PROVINCE_NAME : null, t != null ? t.TDL_PATIENT_PROVINCE_NAME : null)));

                SetKey(Mps000526ExtendSingleKey.CAREER_NAME, Mps000526Util.First(t != null ? t.TDL_PATIENT_CAREER_NAME : null, p != null ? p.CAREER_NAME : null));
                SetKey(Mps000526ExtendSingleKey.WORK_PLACE, Mps000526Util.First(Name("noi_cong_tac"),
                    t != null ? t.TDL_PATIENT_WORK_PLACE_NAME : null, t != null ? t.TDL_PATIENT_WORK_PLACE : null, p != null ? p.WORK_PLACE : null));
                SetKey(Mps000526ExtendSingleKey.WORK_PLACE_COMMUNE, Name("noi_cong_tac_xa_phuong"));
                SetKey(Mps000526ExtendSingleKey.PHONE, Mps000526Util.First(
                    t != null ? t.TDL_PATIENT_MOBILE : null, t != null ? t.TDL_PATIENT_PHONE : null,
                    s != null ? s.TDL_PATIENT_MOBILE : null, s != null ? s.TDL_PATIENT_PHONE : null,
                    p != null ? p.MOBILE : null, p != null ? p.PHONE : null));
                SetKey(Mps000526ExtendSingleKey.EXAM_REASON, Mps000526Util.First(
                    rdo.KskServiceReq != null ? rdo.KskServiceReq.HOSPITALIZATION_REASON : null, s != null ? s.HOSPITALIZATION_REASON : null));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>One character per box when the value fits exactly/under the box count, else the whole value in *_LONG.</summary>
        private void SetBoxes(string prefix, string value, int count, string fullKey, string longKey)
        {
            string v = (value ?? "").Trim();
            SetKey(fullKey, v);
            bool fits = v.Length > 0 && v.Length <= count;
            for (int i = 1; i <= count; i++)
                SetKey(prefix + i, fits && i <= v.Length ? v.Substring(i - 1, 1) : "");
            SetKey(longKey, fits ? "" : v);
        }

        // ================================================================ TIỀN SỬ BỆNH

        private void SetHistoryKeys()
        {
            try
            {
                var o18 = rdo.HisKskOverEighteen;
                var g = rdo.HisKskGeneral;
                bool hasRecord = o18 != null;

                // 1. Family history: yes when the text (or its portal ids, resolved by the plugin) or the ICD is filled.
                string famText = Mps000526Util.First(Name("PATHOLOGICAL_HISTORY_FAMILY"), Mps000526Util.Str(o18, "PATHOLOGICAL_HISTORY_FAMILY"));
                string famIcd = Mps000526Util.IcdText(Mps000526Util.Str(g, "FAMILY_HISTORY_ICD_NAME"));
                bool famYes = !string.IsNullOrWhiteSpace(famText) || !string.IsNullOrWhiteSpace(famIcd);
                SetX(Mps000526ExtendSingleKey.FH_YES_X, hasRecord && famYes);
                SetX(Mps000526ExtendSingleKey.FH_NO_X, hasRecord && !famYes);
                SetKey(Mps000526ExtendSingleKey.FH_TEXT, Mps000526Util.Join("; ", famText, famIcd));

                // 2. 22 personal history rows, in HIS_DISEASE_TYPE code order (the form order).
                List<Mps000526HistoryADO> rows = rdo.PersonalHistories ?? new List<Mps000526HistoryADO>();
                for (int i = 1; i <= 22; i++)
                {
                    Mps000526HistoryADO row = i <= rows.Count ? rows[i - 1] : null;
                    string k = "PH_" + i.ToString("00");
                    string yn = row != null ? (row.IS_YES_NO ?? "").Trim() : "";
                    SetX(k + "_CO_X", yn == "1");
                    SetX(k + "_KHONG_X", yn == "0");
                    SetKey(k + "_NAME", row != null ? row.DISEASE_TYPE_NAME : "");
                }
                // Text of row 22 "Bệnh khác (ghi rõ)" only when that row is "Có" (same rule as Mps000452: disease type code "22").
                Mps000526HistoryADO other22 = rows.FirstOrDefault(o => o != null && (o.DISEASE_TYPE_CODE ?? "").Trim() == "22")
                    ?? (rows.Count >= 22 ? rows[21] : null);
                bool other22Yes = other22 != null && (other22.IS_YES_NO ?? "").Trim() == "1";
                SetKey(Mps000526ExtendSingleKey.PH_22_TEXT, other22Yes ? Mps000526Util.Join("; ",
                    Mps000526Util.Str(o18, "PATHOLOGICAL_HISTORY"), Mps000526Util.IcdText(Mps000526Util.Str(g, "PERSONAL_HISTORY_ICD_NAME"))) : "");

                // 3a. Treatment in progress: medicines + ICD of the disease being treated.
                string treating = Mps000526Util.Join("; ", Mps000526Util.Str(o18, "MEDICINE_USING"), Mps000526Util.IcdText(Mps000526Util.Str(g, "TREATING_DISEASE_ICD_NAME")));
                SetX(Mps000526ExtendSingleKey.TREATING_YES_X, hasRecord && treating.Length > 0);
                SetX(Mps000526ExtendSingleKey.TREATING_NO_X, hasRecord && treating.Length == 0);
                SetKey(Mps000526ExtendSingleKey.TREATING_TEXT, treating);

                // 3b. Maternity: women only.
                string maternity = Mps000526Util.Str(o18, "MATERNITY_HISTORY") ?? "";
                bool female = IsFemale();
                SetX(Mps000526ExtendSingleKey.MATERNITY_YES_X, hasRecord && female && maternity.Trim().Length > 0);
                SetX(Mps000526ExtendSingleKey.MATERNITY_NO_X, hasRecord && female && maternity.Trim().Length == 0);
                SetKey(Mps000526ExtendSingleKey.MATERNITY_TEXT, female ? maternity.Trim() : "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ ĐỐI TƯỢNG - CHI TRẢ

        private void SetObjectPaymentKeys()
        {
            try
            {
                SetKey(Mps000526ExtendSingleKey.SYT_PATIENT_TYPE_TEXT, rdo.PatientTypesText);

                // Names come from the portal catalogs (or HIS lists when the portal catalog was not loaded).
                string pay = Mps000526Util.Norm(Mps000526Util.First(Name("SYT_PAYSOURCE_ID"), Name("KSK_PAY_SOURCE")));
                string detail = Mps000526Util.Norm(Name("SYT_PAY_SOURCE_DETAIL_ID"));
                string other = Mps000526Util.Str(rdo.HisKskSytHcm, "PAY_SOURCE_OTHER");

                bool nsnn = pay.Contains("ngân sách");
                bool contract = detail.Contains("hợp đồng") || pay.Contains("hợp đồng");
                bool nonBorder = detail.Contains("phi địa giới") || pay.Contains("phi địa giới");
                // This form has its own box for the employer (the elderly form prints it under "Nguồn khác").
                bool employer = pay.Contains("sử dụng lao động");
                bool self = pay.Contains("tự chi trả") || pay.Contains("tự trả") || pay.Contains("người dân");
                // Unknown pay source -> "Nguồn khác": judged on the pay source only (the detail ticks its own boxes).
                bool payKnown = nsnn || pay.Contains("hợp đồng") || pay.Contains("phi địa giới") || employer || self;
                bool otherPay = pay.Contains("khác") || !string.IsNullOrWhiteSpace(other) || (pay.Length > 0 && !payKnown);
                SetX(Mps000526ExtendSingleKey.PAY_NSNN_X, nsnn);
                SetX(Mps000526ExtendSingleKey.PAY_CONTRACT_X, contract);
                SetX(Mps000526ExtendSingleKey.PAY_NON_BORDER_X, nonBorder);
                SetX(Mps000526ExtendSingleKey.PAY_EMPLOYER_X, employer);
                SetX(Mps000526ExtendSingleKey.PAY_SELF_X, self);
                SetX(Mps000526ExtendSingleKey.PAY_OTHER_X, otherPay);
                // Unknown source name is printed after "Nguồn khác" so it is not lost.
                SetKey(Mps000526ExtendSingleKey.PAY_OTHER_TEXT, Mps000526Util.First(other,
                    otherPay && !pay.Contains("khác") ? Mps000526Util.First(Name("SYT_PAYSOURCE_ID"), Name("KSK_PAY_SOURCE")) : null));

                string place = Mps000526Util.Norm(Name("SYT_EXAM_PLACE_ID"));
                bool mobile = place.Contains("lưu động");
                bool station = !mobile && place.Contains("trạm");
                bool school = !mobile && !station && place.Contains("trường");
                bool home = !mobile && !station && !school && place.Contains("nhà");
                bool facility = !mobile && !station && !school && !home && place.Length > 0;
                SetX(Mps000526ExtendSingleKey.PLACE_MOBILE_X, mobile);
                SetX(Mps000526ExtendSingleKey.PLACE_STATION_X, station);
                SetX(Mps000526ExtendSingleKey.PLACE_SCHOOL_X, school);
                SetX(Mps000526ExtendSingleKey.PLACE_HOME_X, home);
                SetX(Mps000526ExtendSingleKey.PLACE_FACILITY_X, facility);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ I. KHÁM THỂ LỰC

        private void SetPhysicalKeys()
        {
            try
            {
                var d = rdo.HisDhst;
                SetKey(Mps000526ExtendSingleKey.HEIGHT_STR, Mps000526Util.Num(d != null ? d.HEIGHT : null));
                SetKey(Mps000526ExtendSingleKey.WEIGHT_STR, Mps000526Util.Num(d != null ? d.WEIGHT : null));
                SetKey(Mps000526ExtendSingleKey.BMI_STR, Mps000526Util.Bmi(Mps000526Util.Dec(d, "VIR_BMI"), d != null ? d.HEIGHT : null, d != null ? d.WEIGHT : null));
                SetKey(Mps000526ExtendSingleKey.PULSE_STR, d != null && d.PULSE.HasValue ? d.PULSE.Value.ToString() : "");
                string bpMax = d != null && d.BLOOD_PRESSURE_MAX.HasValue ? d.BLOOD_PRESSURE_MAX.Value.ToString() : "";
                string bpMin = d != null && d.BLOOD_PRESSURE_MIN.HasValue ? d.BLOOD_PRESSURE_MIN.Value.ToString() : "";
                SetKey(Mps000526ExtendSingleKey.BP_MAX_STR, bpMax);
                SetKey(Mps000526ExtendSingleKey.BP_MIN_STR, bpMin);
                // One blank on the form: "max/min"; a lone value is printed alone, never a bare "/".
                SetKey(Mps000526ExtendSingleKey.BP_STR, bpMax.Length > 0 && bpMin.Length > 0 ? bpMax + "/" + bpMin : bpMax + bpMin);
                SetKey(Mps000526ExtendSingleKey.BREATH_RATE_STR, Mps000526Util.Num(d != null ? d.BREATH_RATE : null));

                SetRank("PHYSIQUE", Mps000526Util.Long(rdo.HisKskOverEighteen, "DHST_RANK"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ HELPERS

        private void SetKey(string key, object value)
        {
            SetSingleKey(new KeyValue(key, value ?? ""));
        }

        private void SetX(string key, bool on)
        {
            SetKey(key, on ? "X" : "");
        }

        /// <summary>Ticks {prefix}_1.._5_X from a HIS_HEALTH_EXAM_RANK id.</summary>
        private void SetRank(string prefix, long? rankId)
        {
            int level = Mps000526Util.RankLevel(rankId, rdo.ExamRanks);
            for (int i = 1; i <= 5; i++) SetX(prefix + "_" + i + "_X", level == i);
        }

        /// <summary>Display name resolved by the plugin for a field, or null.</summary>
        private string Name(string field)
        {
            string v;
            return rdo.ResolvedNames != null && rdo.ResolvedNames.TryGetValue(field, out v) && !string.IsNullOrWhiteSpace(v) ? v.Trim() : null;
        }

        private string EmployeeName(string loginname)
        {
            if (string.IsNullOrWhiteSpace(loginname)) return null;
            string v;
            return rdo.EmployeeNames != null && rdo.EmployeeNames.TryGetValue(loginname.Trim(), out v) && !string.IsNullOrWhiteSpace(v)
                ? v.Trim() : loginname.Trim();
        }

        private long? GenderId()
        {
            var s = rdo.HisServiceReq;
            if (s != null && s.TDL_PATIENT_GENDER_ID.HasValue) return s.TDL_PATIENT_GENDER_ID;
            return rdo.Treatment != null ? (long?)rdo.Treatment.TDL_PATIENT_GENDER_ID : null;
        }

        private bool IsFemale() { return GenderId() == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE; }

        private bool IsMale() { return GenderId() == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE; }
    }
}
