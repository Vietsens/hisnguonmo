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
using System.IO;
using System.Linq;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000524.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000524
{
    /// <summary>
    /// Giấy khám sức khỏe của người lái xe (Mẫu 5 — TT 36/2024/TT-BYT).
    /// Cột của HIS_KSK_PRO_DRIVER / HIS_KSK_GENERAL đọc THEO TÊN (Mps000524Util) để MOS.EFMODEL khác phiên bản
    /// không làm hỏng cả phiếu. Mọi cột gốc của bản ghi lái xe cũng đổ ra key tiền tố PRO_ để viện tự thêm vào mẫu.
    /// </summary>
    public partial class Mps000524Processor : AbstractProcessor
    {
        Mps000524PDO rdo;

        /// <summary>8 chuyên khoa của Mẫu 5: tiền tố key trên mẫu -> tiền tố cột HIS_KSK_PRO_DRIVER.</summary>
        private static readonly string[][] SECTIONS = {
            new[] { "MENTAL", "EXAM_MENTAL" },          // 1. Tâm thần
            new[] { "NEURO", "EXAM_NEUROLOGICAL" },     // 2. Thần kinh
            new[] { "EYE", "EXAM_EYE" },                // 3. Mắt
            new[] { "ENT", "EXAM_ENT" },                // 4. Tai - Mũi - Họng
            new[] { "CARDIO", "EXAM_CIRCULATION" },     // 5. Tim mạch
            new[] { "RESP", "EXAM_RESPIRATORY" },       // 6. Hô hấp
            new[] { "MUSCLE", "EXAM_MUSCLE_BONE" },     // 7. Cơ xương khớp
            new[] { "OEND", "EXAM_OEND" } };            // 8. Nội tiết

        /// <summary>Key cận lâm sàng -> cột HIS_KSK_PRO_DRIVER.</summary>
        private static readonly string[][] CLS_FIELDS = {
            new[] { "DRUG_AMPHETAMIN", "TEST_AMPHETAMIN" }, new[] { "DRUG_MARIJUANA", "TEST_MARIJUANA" },
            new[] { "DRUG_MORPHIN", "TEST_MORPHIN" }, new[] { "DRUG_CODEIN", "TEST_CODEIN" }, new[] { "DRUG_HEROIN", "TEST_HEROIN" },
            new[] { "ALCOHOL", "TEST_ALCOHOL" },
            new[] { "CLS_RBC", "TEST_BLOOD_HC" }, new[] { "CLS_HGB", "TEST_BLOOD_HGB" }, new[] { "CLS_HCT", "TEST_BLOOD_HCT" },
            new[] { "CLS_MCV", "TEST_BLOOD_MCV" }, new[] { "CLS_MCH", "TEST_BLOOD_MCH" }, new[] { "CLS_MCHC", "TEST_BLOOD_MCHC" },
            new[] { "CLS_RDW", "TEST_BLOOD_RDW" }, new[] { "CLS_WBC", "TEST_BLOOD_BC" }, new[] { "CLS_NEUT", "TEST_BLOOD_NEUT" },
            new[] { "CLS_LYMPH", "TEST_BLOOD_LYMPH" }, new[] { "CLS_MONO", "TEST_BLOOD_MONO" }, new[] { "CLS_EOS", "TEST_BLOOD_EOS" },
            new[] { "CLS_BASO", "TEST_BLOOD_BASO" }, new[] { "CLS_PLT", "TEST_BLOOD_TC" },
            new[] { "CLS_GLUCOSE", "TEST_BLOOD_GLUCO" }, new[] { "CLS_CREATININE", "TEST_BLOOD_CREATININ" },
            new[] { "CLS_AST", "TEST_BLOOD_ASAT" }, new[] { "CLS_ALT", "TEST_BLOOD_ALAT" },
            new[] { "CLS_URINE_SG", "TEST_URINE_SG" }, new[] { "CLS_URINE_PH", "TEST_URINE_PH" }, new[] { "CLS_URINE_LEU", "TEST_URINE_LEU" },
            new[] { "CLS_URINE_ERY", "TEST_URINE_ERY" }, new[] { "CLS_URINE_NIT", "TEST_URINE_NITRIT" }, new[] { "CLS_URINE_PRO", "TEST_URINE_PROTEIN" },
            new[] { "CLS_URINE_GLU", "TEST_URINE_GLUCO" }, new[] { "CLS_URINE_KET", "TEST_URINE_KETONE" }, new[] { "CLS_URINE_BIL", "TEST_URINE_BILIRUBIN" },
            new[] { "CLS_URINE_UBG", "TEST_URINE_UBG" }, new[] { "CLS_URINE_OTHER", "TEST_URINE_OTHER" },
            new[] { "CLS_DIIM_RESULT", "RESULT_DIIM" }, new[] { "CLS_OTHER_RESULT", "RESULT_OTHER_TEST" } };

        public Mps000524Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000524PDO)rdoBase;
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

                objectTag.AddObjectData(store, "KskGeneral", new List<HIS_KSK_GENERAL>() { rdo.HisKskGeneral ?? new HIS_KSK_GENERAL() });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst ?? new HIS_DHST() });
                objectTag.AddObjectData(store, "Treatment", new List<V_HIS_TREATMENT_4>() { rdo.Treatment ?? new V_HIS_TREATMENT_4() });

                SetSingleKey();
                SetSignatureKeyImageByCFG();
                SetAvatarKey();

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
                // Key dẫn xuất ghi TRƯỚC: SetSingleKey giữ giá trị ghi đầu tiên, key cột gốc không được che mất.
                SetKey(Mps000524ExtendSingleKey.KSK_NUMBER, Mps000524Util.Str(rdo.HisKskProDriver, "KSK_PRO_DRIVER_CODE"));
                SetAdministrativeKeys();
                SetHistoryKeys();
                SetPhysicalKeys();
                SetSpecialtyKeys();
                SetEyeEntKeys();
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
                if (rdo.HisKskGeneral != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_KSK_GENERAL>(rdo.HisKskGeneral, "GENERAL_", false);
                if (rdo.Treatment != null)
                    AddObjectKeyIntoListkeyWithPrefix<V_HIS_TREATMENT_4>(rdo.Treatment, "TREATMENT_", false);
                AddRawKeys(rdo.HisKskProDriver, "PRO_");
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

                string name = s != null ? s.TDL_PATIENT_NAME : (t != null ? t.TDL_PATIENT_NAME : null);
                SetKey(Mps000524ExtendSingleKey.PATIENT_NAME_UPPER, (name ?? "").ToUpper());

                long? genderId = GenderId();
                SetX(Mps000524ExtendSingleKey.GENDER_MALE_X, genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE);
                SetX(Mps000524ExtendSingleKey.GENDER_FEMALE_X, genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE);

                long dob = s != null && s.TDL_PATIENT_DOB > 0 ? s.TDL_PATIENT_DOB : (t != null ? t.TDL_PATIENT_DOB : 0);
                bool onlyYear = (s != null ? s.TDL_PATIENT_IS_HAS_NOT_DAY_DOB : (t != null ? t.TDL_PATIENT_IS_HAS_NOT_DAY_DOB : null)) == 1;
                SetKey(Mps000524ExtendSingleKey.DOB_STR, dob <= 0 ? "" : (onlyYear ? dob.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(dob)));

                SetKey(Mps000524ExtendSingleKey.ETHNIC_NAME, Mps000524Util.First(t != null ? t.TDL_PATIENT_ETHNIC_NAME : null, p != null ? p.ETHNIC_NAME : null));
                string abo = Mps000524Util.First(t != null ? t.TDL_PATIENT_BLOOD_ABO_CODE : null, p != null ? p.BLOOD_ABO_CODE : null);
                string rh = Mps000524Util.First(t != null ? t.TDL_PATIENT_BLOOD_RH_CODE : null, p != null ? p.BLOOD_RH_CODE : null);
                SetKey(Mps000524ExtendSingleKey.BLOOD_TYPE, Mps000524Util.Join(" ", abo, rh));

                // Giấy tờ tùy thân: CCCD -> hộ chiếu -> CMND, kèm ngày cấp / nơi cấp của đúng loại đó.
                string idNo = null, idPlace = null;
                long? idDate = null;
                if (p != null && !string.IsNullOrWhiteSpace(p.CCCD_NUMBER)) { idNo = p.CCCD_NUMBER; idDate = p.CCCD_DATE; idPlace = p.CCCD_PLACE; }
                else if (p != null && !string.IsNullOrWhiteSpace(p.PASSPORT_NUMBER)) { idNo = p.PASSPORT_NUMBER; idDate = p.PASSPORT_DATE; idPlace = p.PASSPORT_PLACE; }
                else if (p != null && !string.IsNullOrWhiteSpace(p.CMND_NUMBER)) { idNo = p.CMND_NUMBER; idDate = p.CMND_DATE; idPlace = p.CMND_PLACE; }
                else
                {
                    idNo = Mps000524Util.First(t != null ? t.TDL_PATIENT_CCCD_NUMBER : null, s != null ? s.TDL_PATIENT_CCCD_NUMBER : null,
                        t != null ? t.TDL_PATIENT_PASSPORT_NUMBER : null, s != null ? s.TDL_PATIENT_PASSPORT_NUMBER : null,
                        t != null ? t.TDL_PATIENT_CMND_NUMBER : null, s != null ? s.TDL_PATIENT_CMND_NUMBER : null);
                }
                SetKey(Mps000524ExtendSingleKey.ID_NUMBER, idNo);
                SetKey(Mps000524ExtendSingleKey.ID_DATE, Mps000524Util.Date(idDate));
                SetKey(Mps000524ExtendSingleKey.ID_PLACE, idPlace);

                SetKey(Mps000524ExtendSingleKey.HEIN_CARD_NUMBER, Mps000524Util.First(t != null ? t.TDL_HEIN_CARD_NUMBER : null,
                    s != null ? s.TDL_HEIN_CARD_NUMBER : null, p != null ? p.TDL_HEIN_CARD_NUMBER : null));

                // Nơi ở hiện tại (HT_*) của bệnh nhân; không có thì lấy địa chỉ thường trú.
                bool hasCurrent = p != null && (!string.IsNullOrWhiteSpace(p.HT_ADDRESS) || !string.IsNullOrWhiteSpace(p.HT_COMMUNE_NAME));
                SetKey(Mps000524ExtendSingleKey.ADDRESS_DETAIL, hasCurrent ? p.HT_ADDRESS
                    : Mps000524Util.First(p != null ? p.ADDRESS : null, t != null ? t.TDL_PATIENT_ADDRESS : null, s != null ? s.TDL_PATIENT_ADDRESS : null));
                SetKey(Mps000524ExtendSingleKey.ADDRESS_COMMUNE, hasCurrent
                    ? Mps000524Util.Join(", ", p.HT_COMMUNE_NAME, p.HT_PROVINCE_NAME)
                    : Mps000524Util.Join(", ",
                        Mps000524Util.First(p != null ? p.COMMUNE_NAME : null, t != null ? t.TDL_PATIENT_COMMUNE_NAME : null),
                        Mps000524Util.First(p != null ? p.PROVINCE_NAME : null, t != null ? t.TDL_PATIENT_PROVINCE_NAME : null)));

                SetKey(Mps000524ExtendSingleKey.PHONE, Mps000524Util.First(
                    t != null ? t.TDL_PATIENT_MOBILE : null, t != null ? t.TDL_PATIENT_PHONE : null,
                    s != null ? s.TDL_PATIENT_MOBILE : null, s != null ? s.TDL_PATIENT_PHONE : null,
                    p != null ? p.MOBILE : null, p != null ? p.PHONE : null));

                // Hạng GPLX đề nghị + hình thức cấp (LICENSE_ISSUE_TYPE: 1 cấp mới, 2 cấp đổi, 3 cấp lại — đọc theo tên, chưa có cột thì trống).
                SetKey(Mps000524ExtendSingleKey.LICENSE_CLASS_NAME, rdo.LicenseClassName);
                long? issue = Mps000524Util.Long(rdo.HisKskProDriver, "LICENSE_ISSUE_TYPE");
                SetX(Mps000524ExtendSingleKey.LICENSE_NEW_X, issue == 1);
                SetX(Mps000524ExtendSingleKey.LICENSE_CHANGE_X, issue == 2);
                SetX(Mps000524ExtendSingleKey.LICENSE_RENEW_X, issue == 3);
                string issueText = issue == 1 ? "cấp mới" : issue == 2 ? "cấp đổi" : issue == 3 ? "cấp lại" : null;
                SetKey(Mps000524ExtendSingleKey.LICENSE_REQUEST_TEXT, Mps000524Util.Join(" - ", rdo.LicenseClassName, issueText));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ TIỀN SỬ BỆNH

        private void SetHistoryKeys()
        {
            try
            {
                var d = rdo.HisKskProDriver;
                var g = rdo.HisKskGeneral;
                bool hasRecord = d != null;

                // 1. Tiền sử gia đình: theo cờ IS_FAMILY_HISTORY; chưa chọn thì suy theo nội dung đã ghi.
                string famText = Mps000524Util.Join("; ", Mps000524Util.Str(d, "FAMILY_HISTORY"),
                    Mps000524Util.Icd(Mps000524Util.Str(g, "FAMILY_HISTORY_ICD_CODE"), Mps000524Util.Str(g, "FAMILY_HISTORY_ICD_NAME")));
                SetYesNo("FH", hasRecord, Mps000524Util.Long(d, "IS_FAMILY_HISTORY"), famText);
                SetKey(Mps000524ExtendSingleKey.FH_TEXT, famText);

                // 2. 22 dòng tiền sử bản thân, theo thứ tự mẫu.
                List<Mps000524HistoryADO> rows = rdo.PersonalHistories ?? new List<Mps000524HistoryADO>();
                for (int i = 1; i <= 22; i++)
                {
                    Mps000524HistoryADO row = i <= rows.Count ? rows[i - 1] : null;
                    string k = "PH_" + i.ToString("00");
                    string yn = row != null ? (row.IS_YES_NO ?? "").Trim() : "";
                    SetX(k + "_CO_X", yn == "1");
                    SetX(k + "_KHONG_X", yn == "0");
                    SetKey(k + "_NAME", row != null ? row.DISEASE_TYPE_NAME : "");
                }
                // Chữ "Bệnh khác (ghi rõ)" của dòng 22 chỉ in khi dòng 22 = Có (cùng quy tắc Mps000452/Mps000523).
                Mps000524HistoryADO other22 = rows.FirstOrDefault(o => o != null && (o.DISEASE_TYPE_CODE ?? "").Trim() == "22")
                    ?? (rows.Count >= 22 ? rows[21] : null);
                bool other22Yes = other22 != null && (other22.IS_YES_NO ?? "").Trim() == "1";
                SetKey(Mps000524ExtendSingleKey.PH_22_TEXT, other22Yes ? Mps000524Util.Str(d, "OTHER_DISEASE") : "");
                // "Nếu có, đề nghị ghi cụ thể tên bệnh": các dòng Có + ICD tiền sử bản thân.
                string yesNames = string.Join("; ", rows.Where(o => o != null && (o.IS_YES_NO ?? "").Trim() == "1"
                    && (o.DISEASE_TYPE_CODE ?? "").Trim() != "22").Select(o => o.DISEASE_TYPE_NAME).ToArray());
                SetKey(Mps000524ExtendSingleKey.PH_DETAIL_TEXT, Mps000524Util.Join("; ", yesNames,
                    other22Yes ? Mps000524Util.Str(d, "OTHER_DISEASE") : null,
                    Mps000524Util.Icd(Mps000524Util.Str(g, "PERSONAL_HISTORY_ICD_CODE"), Mps000524Util.Str(g, "PERSONAL_HISTORY_ICD_NAME"))));

                // 3a. Đang điều trị bệnh: thuốc, liều lượng.
                string treating = Mps000524Util.Join("; ", Mps000524Util.Str(d, "TREATING_MEDICINE"),
                    Mps000524Util.Icd(Mps000524Util.Str(g, "TREATING_DISEASE_ICD_CODE"), Mps000524Util.Str(g, "TREATING_DISEASE_ICD_NAME")));
                SetYesNo("TREATING", hasRecord, Mps000524Util.Long(d, "IS_TREATING"), treating);
                SetKey(Mps000524ExtendSingleKey.TREATING_TEXT, treating);

                // 3b. Tiền sử thai sản: chỉ nữ.
                bool female = IsFemale();
                string maternity = female ? (Mps000524Util.Str(d, "MATERNITY_HISTORY") ?? "") : "";
                SetYesNo("MATERNITY", hasRecord && female, Mps000524Util.Long(d, "IS_MATERNITY_HISTORY"), maternity);
                SetKey(Mps000524ExtendSingleKey.MATERNITY_TEXT, maternity);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// {prefix}_YES_X / _NO_X: theo cờ đã lưu (1 Có, 0 Không); cờ trống thì suy theo nội dung ghi kèm.
        /// Không có bản ghi (hoặc không áp dụng) thì để trống cả hai.
        /// </summary>
        private void SetYesNo(string prefix, bool applicable, long? flag, string text)
        {
            bool yes = applicable && (flag == 1 || (!flag.HasValue && !string.IsNullOrWhiteSpace(text)));
            bool no = applicable && (flag == 0 || (!flag.HasValue && string.IsNullOrWhiteSpace(text)));
            SetX(prefix + "_YES_X", yes);
            SetX(prefix + "_NO_X", no);
        }

        // ================================================================ KHÁM THỂ LỰC

        private void SetPhysicalKeys()
        {
            try
            {
                var h = rdo.HisDhst;
                SetKey(Mps000524ExtendSingleKey.HEIGHT_STR, Mps000524Util.Num(h != null ? h.HEIGHT : null));
                SetKey(Mps000524ExtendSingleKey.WEIGHT_STR, Mps000524Util.Num(h != null ? h.WEIGHT : null));
                decimal? bmi = h != null ? h.VIR_BMI : null;
                if (!bmi.HasValue && h != null && h.HEIGHT.HasValue && h.HEIGHT.Value > 0 && h.WEIGHT.HasValue)
                    bmi = Math.Round(h.WEIGHT.Value / ((h.HEIGHT.Value / 100) * (h.HEIGHT.Value / 100)), 2);
                SetKey(Mps000524ExtendSingleKey.BMI_STR, bmi.HasValue ? Mps000524Util.Num(Math.Round(bmi.Value, 2)) : "");
                SetKey(Mps000524ExtendSingleKey.PULSE_STR, h != null && h.PULSE.HasValue ? h.PULSE.Value.ToString() : "");
                SetKey(Mps000524ExtendSingleKey.BP_MAX_STR, h != null && h.BLOOD_PRESSURE_MAX.HasValue ? h.BLOOD_PRESSURE_MAX.Value.ToString() : "");
                SetKey(Mps000524ExtendSingleKey.BP_MIN_STR, h != null && h.BLOOD_PRESSURE_MIN.HasValue ? h.BLOOD_PRESSURE_MIN.Value.ToString() : "");
                SetKey(Mps000524ExtendSingleKey.BREATH_RATE_STR, Mps000524Util.Num(h != null ? h.BREATH_RATE : null));
                SetRank("PHYSIQUE", Mps000524Util.Long(rdo.HisKskProDriver, "DHST_RANK"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ KHÁM LÂM SÀNG (8 CHUYÊN KHOA)

        private void SetSpecialtyKeys()
        {
            try
            {
                var d = rdo.HisKskProDriver;
                foreach (string[] s in SECTIONS)
                {
                    string p = s[0], c = s[1];
                    string pre = Mps000524Util.Icd(Mps000524Util.Str(d, c + "_PRE_ICD_CODE"), Mps000524Util.Str(d, c + "_PRE_ICD_NAME"));
                    string fin = Mps000524Util.Icd(Mps000524Util.Str(d, c + "_ICD_CODE"), Mps000524Util.Str(d, c + "_ICD_NAME"));
                    SetX(p + "_NORMAL_X", Mps000524Util.Long(d, c + "_IS_NORMAL") == 1);
                    SetX(p + "_PRE_X", pre.Length > 0);
                    SetKey(p + "_PRE_ICD", pre);
                    SetX(p + "_FINAL_X", fin.Length > 0);
                    SetKey(p + "_FINAL_ICD", fin);
                    SetKey(p + "_CONCLUDE", Mps000524Util.Str(d, c + "_CONCLUDE"));
                    SetKey(p + "_DOCTOR", EmployeeName(Mps000524Util.Str(d, c + "_LOGINNAME")));
                    SetRank(p + "_RANK", Mps000524Util.Long(d, c + "_RANK"));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ MẮT, TAI - MŨI - HỌNG

        private void SetEyeEntKeys()
        {
            try
            {
                var d = rdo.HisKskProDriver;
                SetKey(Mps000524ExtendSingleKey.EYE_NOGLASS_R, Mps000524Util.Str(d, "EXAM_EYESIGHT_RIGHT"));
                SetKey(Mps000524ExtendSingleKey.EYE_NOGLASS_L, Mps000524Util.Str(d, "EXAM_EYESIGHT_LEFT"));
                SetKey(Mps000524ExtendSingleKey.EYE_PINHOLE_R, Mps000524Util.Str(d, "EXAM_EYESIGHT_PINHOLE_RIGHT"));
                SetKey(Mps000524ExtendSingleKey.EYE_PINHOLE_L, Mps000524Util.Str(d, "EXAM_EYESIGHT_PINHOLE_LEFT"));
                SetKey(Mps000524ExtendSingleKey.EYE_GLASS_R, Mps000524Util.Str(d, "EXAM_EYESIGHT_GLASS_RIGHT"));
                SetKey(Mps000524ExtendSingleKey.EYE_GLASS_L, Mps000524Util.Str(d, "EXAM_EYESIGHT_GLASS_LEFT"));
                SetKey(Mps000524ExtendSingleKey.EYE_TWO_NOGLASS, Mps000524Util.Str(d, "EXAM_TWO_EYESIGHT"));
                SetKey(Mps000524ExtendSingleKey.EYE_TWO_GLASS, Mps000524Util.Str(d, "EXAM_TWO_EYESIGHT_GLASS"));

                // Thị trường: ô có giá trị (cờ 1 hoặc chữ) thì tích.
                SetX(Mps000524ExtendSingleKey.EYEFIELD_HORI_NORMAL_X, Marked(d, "EXAM_EYEFIELD_HORI_NORMAL"));
                SetX(Mps000524ExtendSingleKey.EYEFIELD_HORI_LIMIT_X, Marked(d, "EXAM_EYEFIELD_HORI_LIMIT"));
                SetX(Mps000524ExtendSingleKey.EYEFIELD_VERT_NORMAL_X, Marked(d, "EXAM_EYEFIELD_VERT_NORMAL"));
                SetX(Mps000524ExtendSingleKey.EYEFIELD_VERT_LIMIT_X, Marked(d, "EXAM_EYEFIELD_VERT_LIMIT"));

                // Sắc giác.
                SetX(Mps000524ExtendSingleKey.EYECOLOR_NORMAL_X, Mps000524Util.Long(d, "EXAM_EYECOLOR_IS_NORMAL") == 1);
                SetX(Mps000524ExtendSingleKey.EYECOLOR_BLIND_X, Mps000524Util.Long(d, "EXAM_EYECOLOR_IS_BLIND") == 1);
                SetX(Mps000524ExtendSingleKey.EYECOLOR_RED_X, Mps000524Util.Long(d, "EXAM_EYECOLOR_IS_BLIND_RED") == 1);
                SetX(Mps000524ExtendSingleKey.EYECOLOR_GREEN_X, Mps000524Util.Long(d, "EXAM_EYECOLOR_IS_BLIND_GREEN") == 1);
                SetX(Mps000524ExtendSingleKey.EYECOLOR_YELLOW_X, Mps000524Util.Long(d, "EXAM_EYECOLOR_IS_BLIND_YELOW") == 1);

                SetKey(Mps000524ExtendSingleKey.ENT_L_NORMAL, Mps000524Util.Str(d, "EXAM_ENT_LEFT_NORMAL"));
                SetKey(Mps000524ExtendSingleKey.ENT_L_WHISPER, Mps000524Util.Str(d, "EXAM_ENT_LEFT_WHISPER"));
                SetKey(Mps000524ExtendSingleKey.ENT_R_NORMAL, Mps000524Util.Str(d, "EXAM_ENT_RIGHT_NORMAL"));
                SetKey(Mps000524ExtendSingleKey.ENT_R_WHISPER, Mps000524Util.Str(d, "EXAM_ENT_RIGHT_WHISPER"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Ô tích lưu dạng số (1) hoặc chữ: có giá trị khác 0 thì coi là đã tích.</summary>
        private static bool Marked(object data, string column)
        {
            string v = Mps000524Util.Str(data, column);
            return v != null && v != "0";
        }

        // ================================================================ CẬN LÂM SÀNG

        private void SetSubclinicalKeys()
        {
            try
            {
                var d = rdo.HisKskProDriver;
                foreach (string[] f in CLS_FIELDS)
                    SetKey(f[0], Mps000524Util.Str(d, f[1]));

                SetKey(Mps000524ExtendSingleKey.CLS_DRUG_DOCTOR, EmployeeName(Mps000524Util.Str(d, "TEST_DRUG_LOGINNAME")));
                SetKey(Mps000524ExtendSingleKey.CLS_BLOOD_DOCTOR, EmployeeName(Mps000524Util.Str(d, "TEST_BLOOD_LOGINNAME")));
                SetKey(Mps000524ExtendSingleKey.CLS_URINE_DOCTOR, EmployeeName(Mps000524Util.Str(d, "TEST_URINE_LOGINNAME")));
                SetKey(Mps000524ExtendSingleKey.CLS_DIIM_DOCTOR, EmployeeName(Mps000524Util.Str(d, "DIIM_LOGINNAME")));
                SetKey(Mps000524ExtendSingleKey.CLS_OTHER_DOCTOR, EmployeeName(Mps000524Util.Str(d, "OTHER_TEST_LOGINNAME")));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ IV. BỆNH TẬT, ĐỀ NGHỊ — V. KẾT LUẬN

        private void SetConclusionKeys()
        {
            try
            {
                var d = rdo.HisKskProDriver;
                var g = rdo.HisKskGeneral;

                // IV.1 Các bệnh, tật: theo bản ghi lái xe; bản ghi chưa có thì theo "Kết luận theo bệnh (ICD-10)" chung.
                string pre = Mps000524Util.Icd(Mps000524Util.Str(d, "CONCLUSION_PRE_ICD_CODE"), Mps000524Util.Str(d, "CONCLUSION_PRE_ICD_NAME"));
                string fin = Mps000524Util.Icd(Mps000524Util.Str(d, "CONCLUSION_ICD_CODE"), Mps000524Util.Str(d, "CONCLUSION_ICD_NAME"));
                bool normal = Mps000524Util.Long(d, "CONCLUSION_IS_NORMAL") == 1;
                if (!normal && pre.Length == 0 && fin.Length == 0)
                {
                    // CONCLUSION_ICD_TYPE: 1 chưa phát hiện bất thường, 2 chẩn đoán sơ bộ, 3 chẩn đoán xác định.
                    long? type = Mps000524Util.Long(g, "CONCLUSION_ICD_TYPE");
                    string icd = Mps000524Util.Icd(Mps000524Util.Str(g, "CONCLUSION_ICD_CODE"), Mps000524Util.Str(g, "CONCLUSION_ICD_NAME"));
                    normal = type == 1;
                    if (type == 2) pre = icd;
                    if (type == 3) fin = icd;
                }
                SetX(Mps000524ExtendSingleKey.CONCL_NORMAL_X, normal);
                SetX(Mps000524ExtendSingleKey.CONCL_PRE_X, pre.Length > 0);
                SetKey(Mps000524ExtendSingleKey.CONCL_PRE_ICD, pre);
                SetX(Mps000524ExtendSingleKey.CONCL_FINAL_X, fin.Length > 0);
                SetKey(Mps000524ExtendSingleKey.CONCL_FINAL_ICD, fin);

                // IV.2 Đề nghị: 1 bình thường, 2 có yếu tố nguy cơ, 3 bệnh mạn tính, 4 chuyển cơ sở KBCB, 5 khác.
                long? suggest = Mps000524Util.Long(d, "SUGGEST_TYPE");
                string suggestNote = Mps000524Util.Str(d, "SUGGEST_NOTE");
                for (int i = 1; i <= 5; i++) SetX("SUGGEST_" + i + "_X", suggest == i);
                SetKey(Mps000524ExtendSingleKey.SUGGEST_TRANSFER_TEXT, suggest == 4 ? suggestNote : "");
                SetKey(Mps000524ExtendSingleKey.SUGGEST_OTHER_TEXT, suggest == 5 ? suggestNote : "");

                // V. Kết luận: 1 đủ điều kiện, 2 không đủ, 3 đạt nhưng khám lại, 4 hạng A1 người khuyết tật, 5 hạng B người khuyết tật.
                long? conclusion = Mps000524Util.Long(d, "DRIVER_CONCLUSION_TYPE");
                for (int i = 1; i <= 5; i++) SetX("DRIVER_CONCL_" + i + "_X", conclusion == i);
                for (int i = 1; i <= 3; i++) SetKey("DRIVER_CONCL_" + i + "_CLASS", conclusion == i ? rdo.LicenseClassName : "");
                SetKey(Mps000524ExtendSingleKey.REEXAM_NOTE, conclusion == 3 ? Mps000524Util.Str(d, "REEXAM_NOTE") : "");
                SetRank("HEALTH_RANK", Mps000524Util.Long(d, "HEALTH_EXAM_RANK_ID"));

                // Ngày kết luận: của bản ghi lái xe nếu có cột, không thì của HIS_KSK_GENERAL.
                long? ct = Mps000524Util.Long(d, "CONCLUSION_TIME") ?? Mps000524Util.Long(g, "CONCLUSION_TIME");
                string ctStr = ct.HasValue && ct.Value > 0 ? ct.Value.ToString() : "";
                SetKey(Mps000524ExtendSingleKey.CONCLUSION_DAY, ctStr.Length >= 8 ? ctStr.Substring(6, 2) : "......");
                SetKey(Mps000524ExtendSingleKey.CONCLUSION_MONTH, ctStr.Length >= 8 ? ctStr.Substring(4, 2) : "......");
                SetKey(Mps000524ExtendSingleKey.CONCLUSION_YEAR, ctStr.Length >= 8 ? ctStr.Substring(0, 4) : "20......");
                SetKey(Mps000524ExtendSingleKey.CONCLUDER_USERNAME, Mps000524Util.First(Mps000524Util.Str(g, "CONCLUDER_USERNAME"),
                    EmployeeName(Mps000524Util.Str(g, "CONCLUDER_LOGINNAME"))));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ ẢNH

        private void SetAvatarKey()
        {
            try
            {
                string url = Mps000524Util.First(rdo.HisServiceReq != null ? rdo.HisServiceReq.TDL_PATIENT_AVATAR_URL : null,
                    rdo.Treatment != null ? rdo.Treatment.TDL_PATIENT_AVATAR_URL : null);
                if (string.IsNullOrEmpty(url))
                {
                    SetKey(Mps000524ExtendSingleKey.IMG_AVATAR, "");
                    return;
                }
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(url);
                SetSingleKey(new KeyValue(Mps000524ExtendSingleKey.IMG_AVATAR, stream != null ? (object)stream.ToArray() : ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        /// <summary>Tích {prefix}_1.._5_X theo id HIS_HEALTH_EXAM_RANK.</summary>
        private void SetRank(string prefix, long? rankId)
        {
            int level = Mps000524Util.RankLevel(rankId, rdo.ExamRanks);
            for (int i = 1; i <= 5; i++) SetX(prefix + "_" + i + "_X", level == i);
        }

        /// <summary>Đổ mọi thuộc tính của đối tượng (kiểu bất kỳ) thành key {prefix}{TÊN_CỘT}.</summary>
        private void AddRawKeys(object data, string prefix)
        {
            if (data == null) return;
            foreach (var pi in data.GetType().GetProperties())
            {
                try
                {
                    if (pi.GetIndexParameters().Length > 0) continue;
                    Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                    if (!t.IsPrimitive && t != typeof(string) && t != typeof(decimal)) continue;
                    SetKey(prefix + pi.Name, pi.GetValue(data, null));
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
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
    }
}
