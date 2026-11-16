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

namespace MPS.Processor.Mps000526
{
    /// <summary>Clinical sections 1-7, lab, conclusion.</summary>
    public partial class Mps000526Processor
    {
        /// <summary>
        /// Clinical sections of the screen: column prefix in HIS_KSK_SYT_HCM, rank/doctor column and the table holding them
        /// (gynecology keeps rank/doctor in HIS_KSK_SYT_HCM, the others in HIS_KSK_OVER_EIGHTEEN).
        /// </summary>
        private class Section
        {
            public string HcmPrefix, RankColumn, LoginColumn;
            public bool RankInHcm;
            public Section(string hcmPrefix, string o18Prefix, bool rankInHcm = false)
            {
                HcmPrefix = hcmPrefix; RankColumn = o18Prefix + "_RANK"; LoginColumn = o18Prefix + "_LOGINNAME"; RankInHcm = rankInHcm;
            }
        }

        // 1. Nội khoa: this form prints the eight parts a) .. h) of the screen one by one (key prefix -> section).
        private static readonly KeyValuePair<string, Section>[] INTERNAL_SECTIONS = {
            new KeyValuePair<string, Section>("TUANHOAN", new Section("EXAM_CIRCULATION", "EXAM_CIRCULATION")),
            new KeyValuePair<string, Section>("HOHAP", new Section("EXAM_RESPIRATORY", "EXAM_RESPIRATORY")),
            new KeyValuePair<string, Section>("TIEUHOA", new Section("EXAM_DIGESTION", "EXAM_DIGESTION")),
            new KeyValuePair<string, Section>("THANTN", new Section("EXAM_KIDNEY_URO", "EXAM_KIDNEY_UROLOGY")),
            new KeyValuePair<string, Section>("NOITIET", new Section("EXAM_OEND", "EXAM_OEND")),
            new KeyValuePair<string, Section>("COXK", new Section("EXAM_MUSCLE_BONE", "EXAM_MUSCLE_BONE")),
            new KeyValuePair<string, Section>("THANKINH", new Section("EXAM_NEUROLOGICAL", "EXAM_NEUROLOGICAL")),
            new KeyValuePair<string, Section>("TAMTHAN", new Section("EXAM_MENTAL", "EXAM_MENTAL")) };
        private static readonly Section OBSTETRIC = new Section("EXAM_OBSTETRIC", "EXAM_OBSTETRIC");
        private static readonly Section GYNECOLOGY = new Section("EXAM_GYNECOLOGY", "EXAM_GYNECOLOGY", true);

        // ================================================================ II. KHÁM LÂM SÀNG

        private void SetSpecialtyKeys()
        {
            try
            {
                foreach (KeyValuePair<string, Section> part in INTERNAL_SECTIONS)
                    SetSectionKeys(part.Key, new[] { part.Value });
                SetSectionKeys("NGOAI", new[] { new Section("EXAM_SURGERY", "EXAM_SURGERY") });
                SetSectionKeys("DALIEU", new[] { new Section("EXAM_DERMATOLOGY", "EXAM_DERMATOLOGY") });
                SetSectionKeys("MAT", new[] { new Section("EXAM_EYE", "EXAM_EYE") });
                SetSectionKeys("TMH", new[] { new Section("EXAM_ENT", "EXAM_ENT") });
                SetSectionKeys("RHM", new[] { new Section("EXAM_STOMATOLOGY", "EXAM_STOMATOLOGY") });
                SetObstetricKeys();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// One block of the form from one screen section, or from several merged ones (Sản phụ khoa): "normal" only when
        /// every examined section is normal and none has an ICD; ICD names are united; the worst rank wins; doctors are
        /// listed once each.
        /// </summary>
        private void SetSectionKeys(string p, Section[] sections)
        {
            var hcm = rdo.HisKskSytHcm;
            var o18 = rdo.HisKskOverEighteen;
            var pre = new List<string>();
            var fin = new List<string>();
            var doctors = new List<string>();
            int examined = 0, normal = 0, worst = 0;
            foreach (Section s in sections)
            {
                bool isNormal = Mps000526Util.Long(hcm, s.HcmPrefix + "_IS_NORMAL") == 1;
                string preName = Mps000526Util.Str(hcm, s.HcmPrefix + "_PRE_ICD_NAME");
                string finName = Mps000526Util.Str(hcm, s.HcmPrefix + "_ICD_NAME");
                bool hasPre = !string.IsNullOrWhiteSpace(Mps000526Util.Str(hcm, s.HcmPrefix + "_PRE_ICD_CODE")) || !string.IsNullOrWhiteSpace(preName);
                bool hasFin = !string.IsNullOrWhiteSpace(Mps000526Util.Str(hcm, s.HcmPrefix + "_ICD_CODE")) || !string.IsNullOrWhiteSpace(finName);
                if (hasPre) Mps000526Util.AddIcdNames(pre, preName);
                if (hasFin) Mps000526Util.AddIcdNames(fin, finName);
                if (isNormal || hasPre || hasFin) { examined++; if (isNormal && !hasPre && !hasFin) normal++; }

                object owner = s.RankInHcm ? (object)hcm : o18;
                worst = Math.Max(worst, Mps000526Util.RankLevel(Mps000526Util.Long(owner, s.RankColumn), rdo.ExamRanks));
                string doctor = EmployeeName(Mps000526Util.Str(owner, s.LoginColumn));
                if (!string.IsNullOrWhiteSpace(doctor) && !doctors.Contains(doctor)) doctors.Add(doctor);
            }
            SetX(p + "_NORMAL_X", examined > 0 && normal == examined);
            SetX(p + "_PRE_X", pre.Count > 0);
            SetKey(p + "_PRE_ICD", string.Join("; ", pre));
            SetX(p + "_FINAL_X", fin.Count > 0);
            SetKey(p + "_FINAL_ICD", string.Join("; ", fin));
            for (int i = 1; i <= 5; i++) SetX(p + "_RANK_" + i + "_X", worst == i);
            SetKey(p + "_DOCTOR", string.Join(", ", doctors));
        }

        /// <summary>4. Sản phụ khoa = obstetric + gynecology parts; blank for men; "refused" when no part was examined.</summary>
        private void SetObstetricKeys()
        {
            var hcm = rdo.HisKskSytHcm;
            if (IsMale())
            {
                SetKey(Mps000526ExtendSingleKey.SAN_REFUSE_X, "");
                SetSectionKeys("SAN", new Section[0]);
                return;
            }
            bool refuseObstetric = Mps000526Util.Long(hcm, "IS_REFUSE_OBSTETRIC") == 1;
            bool refuseGynecology = Mps000526Util.Long(hcm, "IS_REFUSE_GYNECOLOGY") == 1;
            var parts = new List<Section>();
            if (!refuseObstetric) parts.Add(OBSTETRIC);
            if (!refuseGynecology) parts.Add(GYNECOLOGY);
            bool anyResult = parts.Any(s => Mps000526Util.Long(hcm, s.HcmPrefix + "_IS_NORMAL") == 1
                || !string.IsNullOrWhiteSpace(Mps000526Util.Str(hcm, s.HcmPrefix + "_PRE_ICD_CODE"))
                || !string.IsNullOrWhiteSpace(Mps000526Util.Str(hcm, s.HcmPrefix + "_ICD_CODE")));
            SetX(Mps000526ExtendSingleKey.SAN_REFUSE_X, (refuseObstetric || refuseGynecology) && !anyResult);
            SetSectionKeys("SAN", parts.ToArray());
        }

        // ================================================================ MẮT, TAI - MŨI - HỌNG, RĂNG

        private void SetEyeEntTeethKeys()
        {
            try
            {
                var o18 = rdo.HisKskOverEighteen;
                var hcm = rdo.HisKskSytHcm;
                SetKey(Mps000526ExtendSingleKey.EYE_NOGLASS_R, Mps000526Util.Str(o18, "EXAM_EYESIGHT_RIGHT"));
                SetKey(Mps000526ExtendSingleKey.EYE_NOGLASS_L, Mps000526Util.Str(o18, "EXAM_EYESIGHT_LEFT"));
                SetKey(Mps000526ExtendSingleKey.EYE_GLASS_R, Mps000526Util.Str(o18, "EXAM_EYESIGHT_GLASS_RIGHT"));
                SetKey(Mps000526ExtendSingleKey.EYE_GLASS_L, Mps000526Util.Str(o18, "EXAM_EYESIGHT_GLASS_LEFT"));
                SetKey(Mps000526ExtendSingleKey.EYE_PINHOLE_R, Mps000526Util.Str(hcm, "EXAM_EYESIGHT_PINHOLE_RIGHT"));
                SetKey(Mps000526ExtendSingleKey.EYE_PINHOLE_L, Mps000526Util.Str(hcm, "EXAM_EYESIGHT_PINHOLE_LEFT"));
                SetKey(Mps000526ExtendSingleKey.EYE_REFRACTION_R, Mps000526Util.Refraction(Mps000526Util.Str(hcm, "EXAM_EYE_SPHERE_RIGHT"),
                    Mps000526Util.Str(hcm, "EXAM_EYE_CYLINDER_RIGHT"), Mps000526Util.Str(hcm, "EXAM_EYE_AXIS_RIGHT")));
                SetKey(Mps000526ExtendSingleKey.EYE_REFRACTION_L, Mps000526Util.Refraction(Mps000526Util.Str(hcm, "EXAM_EYE_SPHERE_LEFT"),
                    Mps000526Util.Str(hcm, "EXAM_EYE_CYLINDER_LEFT"), Mps000526Util.Str(hcm, "EXAM_EYE_AXIS_LEFT")));

                SetKey(Mps000526ExtendSingleKey.ENT_L_NORMAL, Mps000526Util.Str(o18, "EXAM_ENT_LEFT_NORMAL"));
                SetKey(Mps000526ExtendSingleKey.ENT_L_WHISPER, Mps000526Util.Str(o18, "EXAM_ENT_LEFT_WHISPER"));
                SetKey(Mps000526ExtendSingleKey.ENT_R_NORMAL, Mps000526Util.Str(o18, "EXAM_ENT_RIGHT_NORMAL"));
                SetKey(Mps000526ExtendSingleKey.ENT_R_WHISPER, Mps000526Util.Str(o18, "EXAM_ENT_RIGHT_WHISPER"));

                foreach (int tooth in Mps000526Util.TEETH)
                    SetKey("TOOTH_" + tooth + "_STR", Mps000526Util.ToothCode(Name("TOOTH_" + tooth)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // ================================================================ CẬN LÂM SÀNG

        /// <summary>Template key -> portal field code of the CLS map (same codes as the HCMC portal push).</summary>
        private static readonly string[][] CLS_FIELDS = {
            new[] { "CLS_RBC", "xnm_slhc" }, new[] { "CLS_HGB", "xnm_huyetsacto" }, new[] { "CLS_HCT", "xnm_hematocrit" },
            new[] { "CLS_MCV", "xnm_mcv" }, new[] { "CLS_MCH", "xnm_mch" }, new[] { "CLS_MCHC", "xnm_mchc" }, new[] { "CLS_RDW", "xnm_rdw" },
            new[] { "CLS_WBC", "xnm_slbc" }, new[] { "CLS_NEUT", "xnm_slbc_trungtinh" }, new[] { "CLS_LYMPH", "xnm_slbc_lympho" },
            new[] { "CLS_MONO", "xnm_slbc_donnhan" }, new[] { "CLS_EOS", "xnm_slbc_aitoan" }, new[] { "CLS_BASO", "xnm_slbc_aikiem" },
            new[] { "CLS_PLT", "xnm_sltc" },
            new[] { "CLS_URINE_SG", "xnnt_titrong" }, new[] { "CLS_URINE_PH", "xnnt_ph" }, new[] { "CLS_URINE_LEU", "xnnt_bachcau" },
            new[] { "CLS_URINE_ERY", "xnnt_hongcau" }, new[] { "CLS_URINE_NIT", "xnnt_nitrit" }, new[] { "CLS_URINE_PRO", "xnnt_protein" },
            new[] { "CLS_URINE_GLU", "xnnt_glucose" }, new[] { "CLS_URINE_KET", "xnnt_cetonic" }, new[] { "CLS_URINE_BIL", "xnnt_bilirubin" },
            new[] { "CLS_URINE_UBG", "xnnt_urobilinogen" } };

        /// <summary>
        /// The paper has two lab blocks ("để đi học, đi làm" and "khám sức khỏe định kỳ") with the same lists. HIS has no flag
        /// telling which one a record belongs to and the portal push fills both sets alike, so there is ONE set of keys
        /// here and the template repeats the tags in both blocks.
        /// </summary>
        private void SetSubclinicalKeys()
        {
            try
            {
                var o18 = rdo.HisKskOverEighteen;
                bool anyBlood = false, anyUrine = false;
                foreach (string[] f in CLS_FIELDS)
                {
                    string v = Cls(f[1]);
                    SetKey(f[0], v);
                    if (!string.IsNullOrWhiteSpace(v)) { if (f[1].StartsWith("xnm_")) anyBlood = true; else anyUrine = true; }
                }
                // Biochemistry: the value confirmed on the KSK screen first, the CLS map second.
                SetKey("CLS_GLU_FASTING", Mps000526Util.First(Mps000526Util.Str(o18, "TEST_BLOOD_GLUCO"), Cls("shm_duongmau")));
                SetKey(Mps000526ExtendSingleKey.CLS_GLU_RANDOM, "");
                SetKey(Mps000526ExtendSingleKey.CLS_UREA, Mps000526Util.First(Mps000526Util.Str(o18, "TEST_BLOOD_URE"), Cls("shm_ure")));
                SetKey("CLS_CREATININE", Mps000526Util.First(Mps000526Util.Str(o18, "TEST_BLOOD_CREATININ"), Cls("shm_creatinin")));
                SetKey("CLS_AST", Mps000526Util.First(Mps000526Util.Str(o18, "TEST_BLOOD_ASAT"), Cls("shm_asat_got")));
                SetKey("CLS_ALT", Mps000526Util.First(Mps000526Util.Str(o18, "TEST_BLOOD_ALAT"), Cls("shm_alat_gpt")));
                SetKey("CLS_URINE_OTHER", Mps000526Util.First(Mps000526Util.Str(o18, "TEST_URINE_OTHER"), Cls("xnnt_khac")));

                // No mapped value on this workstation: keep the free-text results of the KSK screen.
                SetKey(Mps000526ExtendSingleKey.CLS_BLOOD_GENERAL, anyBlood ? "" : Mps000526Util.Join("; ",
                    Mps000526Util.Str(o18, "TEST_BLOOD_HC"), Mps000526Util.Str(o18, "TEST_BLOOD_BC"), Mps000526Util.Str(o18, "TEST_BLOOD_TC")));
                SetKey(Mps000526ExtendSingleKey.CLS_URINE_GENERAL, anyUrine ? "" : Mps000526Util.Join("; ",
                    Mps000526Util.Str(o18, "TEST_URINE_GLUCO"), Mps000526Util.Str(o18, "TEST_URINE_PROTEIN")));

                SetKey(Mps000526ExtendSingleKey.CLS_DIIM_RESULT, Mps000526Util.Str(o18, "RESULT_DIIM"));
                // Block 1 d) "Cận lâm sàng khác: Không / Có" = the flag the portal push sends (can_lam_sang_khac).
                string other = Mps000526Util.Str(o18, "OTHER_CLS_RESULT");
                SetX(Mps000526ExtendSingleKey.CLS_OTHER_YES_X, o18 != null && other != null);
                SetX(Mps000526ExtendSingleKey.CLS_OTHER_NO_X, o18 != null && other == null);
                SetKey(Mps000526ExtendSingleKey.CLS_OTHER_RESULT, other);
                // Block 2 d) = the "KHÁM SỨC KHỎE ĐỊNH KỲ" result and examiner of the screen.
                SetKey(Mps000526ExtendSingleKey.CLS_PERIODIC_RESULT, Mps000526Util.Str(o18, "PERIODIC_CLS_RESULT"));
                // TEST_BLOOD holds the LOGINNAME of the blood-test doctor (legacy column name).
                SetKey(Mps000526ExtendSingleKey.CLS_BLOOD_DOCTOR, EmployeeName(Mps000526Util.Str(o18, "TEST_BLOOD")));
                SetKey(Mps000526ExtendSingleKey.CLS_URINE_DOCTOR, EmployeeName(Mps000526Util.Str(o18, "TEST_URINE_LOGINNAME")));
                SetKey(Mps000526ExtendSingleKey.CLS_DIIM_DOCTOR, EmployeeName(Mps000526Util.Str(o18, "DIIM_LOGINNAME")));
                SetKey(Mps000526ExtendSingleKey.CLS_OTHER_DOCTOR, EmployeeName(Mps000526Util.Str(o18, "OTHER_CLS_LOGINNAME")));
                SetKey(Mps000526ExtendSingleKey.CLS_PERIODIC_DOCTOR, EmployeeName(Mps000526Util.Str(o18, "PERIODIC_CLS_LOGINNAME")));

                // Screening of female workers (block 2): results come through the CLS map like the portal push; women only.
                // The paper gives two tests one unlabeled blank each pair, so a result carries the name of its test.
                bool female = IsFemale();
                SetKey(Mps000526ExtendSingleKey.CLS_PAP, Labeled("Xét nghiệm tế bào cổ tử cung", female ? Cls("xet_nghiem_te_bao_co_tu_cung") : null));
                SetKey(Mps000526ExtendSingleKey.CLS_HPV, Labeled("Xét nghiệm HPV", female ? Cls("xet_nghiem_hpv") : null));
                SetKey(Mps000526ExtendSingleKey.CLS_MAMMO, Labeled("X-quang nhũ", female ? Cls("xquang_nhu") : null));
                SetKey(Mps000526ExtendSingleKey.CLS_BREAST_US, Labeled("Siêu âm 02 tuyến vú", female ? Cls("sieu_am_2_tuyen_vu") : null));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>"label: value", or "" when there is no value (the blank form stays as on paper).</summary>
        private static string Labeled(string label, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : label + ": " + value.Trim();
        }

        private string Cls(string fieldCode)
        {
            string v;
            return rdo.ClsValues != null && rdo.ClsValues.TryGetValue(fieldCode, out v) && !string.IsNullOrWhiteSpace(v) ? v.Trim() : null;
        }

        // ================================================================ KẾT LUẬN

        private void SetConclusionKeys()
        {
            try
            {
                var g = rdo.HisKskGeneral;
                SetRank("HEALTH_RANK", Mps000526Util.Long(rdo.HisKskOverEighteen, "HEALTH_EXAM_RANK_ID"));

                // CONCLUSION_ICD_TYPE: 1 = chưa phát hiện bất thường, 2 = chẩn đoán sơ bộ, 3 = chẩn đoán xác định.
                long? type = Mps000526Util.Long(g, "CONCLUSION_ICD_TYPE");
                string icd = Mps000526Util.JoinIcd(Mps000526Util.Str(g, "CONCLUSION_ICD_CODE"), Mps000526Util.Str(g, "CONCLUSION_ICD_NAME"));
                SetX(Mps000526ExtendSingleKey.CONCL_NORMAL_X, type == 1);
                SetX(Mps000526ExtendSingleKey.CONCL_PRE_X, type == 2);
                SetKey(Mps000526ExtendSingleKey.CONCL_PRE_ICD, type == 2 ? icd : "");
                SetX(Mps000526ExtendSingleKey.CONCL_FINAL_X, type == 3);
                SetKey(Mps000526ExtendSingleKey.CONCL_FINAL_ICD, type == 3 ? icd : "");

                int suggest = Mps000526Util.SuggestOption(Name("SYT_SUGGEST_ID"));
                for (int i = 1; i <= 5; i++) SetX("SUGGEST_" + i + "_X", suggest == i);
                SetKey(Mps000526ExtendSingleKey.SUGGEST_OTHER_TEXT, Mps000526Util.Str(rdo.HisKskSytHcm, "SUGGEST_OTHER"));

                long? ct = Mps000526Util.Long(g, "CONCLUSION_TIME");
                string ctStr = ct.HasValue && ct.Value > 0 ? ct.Value.ToString() : "";
                SetKey(Mps000526ExtendSingleKey.CONCLUSION_DAY, ctStr.Length >= 8 ? ctStr.Substring(6, 2) : "......");
                SetKey(Mps000526ExtendSingleKey.CONCLUSION_MONTH, ctStr.Length >= 8 ? ctStr.Substring(4, 2) : "......");
                SetKey(Mps000526ExtendSingleKey.CONCLUSION_YEAR, ctStr.Length >= 8 ? ctStr.Substring(0, 4) : "20......");
                SetKey(Mps000526ExtendSingleKey.CONCLUDER_USERNAME, Mps000526Util.First(Mps000526Util.Str(g, "CONCLUDER_USERNAME"),
                    EmployeeName(Mps000526Util.Str(g, "CONCLUDER_LOGINNAME"))));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
