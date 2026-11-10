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

namespace MPS.Processor.Mps000523
{
    /// <summary>B, D1-D8 (interview JSON), clinical sections 1-7, lab, conclusion.</summary>
    public partial class Mps000523Processor
    {
        /// <summary>JSON keys that are not interview answers (version, work place, last-year weight).</summary>
        private static readonly HashSet<string> NON_ANSWER_KEYS = new HashSet<string> { "_v", "noi_cong_tac", "noi_cong_tac_xa_phuong", "cannang_namtruoc" };

        private static readonly string[] B1_KEYS = { "tsbgd_timmach", "tsbgd_tanghuyetap", "tsbgd_daithaoduong", "tsbgd_phoi", "tsbgd_henphequan", "tsbgd_tramcam", "tsbgd_ungthu" };
        private static readonly string[] D1_KEYS = { "benh_tanghuyetap", "benh_daithaoduong", "benh_phoi", "benh_hen", "benh_ungthu", "benh_suytim", "benh_khop",
            "benh_thanmam", "benh_timthieumau", "benh_nhoimau", "benh_dotqui", "benh_roiloan_tramcam", "benh_roiloan_loau", "benh_sasut_tritue" };
        private static readonly string[] D2_KEYS = { "dtd_maudoi", "dtd_khatnuoc", "dtd_ditieunhieu", "dtd_sutcan", "dtd_vetthuong" };
        private static readonly string[] D3_KEYS = { "bpq_ho_hangngay", "bpq_khacdam", "bpq_khotho" };
        private static readonly string[] D4_KEYS = { "hpq_khokhe", "hpq_ho_demkhuya", "hpq_ho_thucgiac", "hpq_ho_vandong", "hpq_hohap_theomua", "hpq_ho_chatkichthich",
            "hpq_dotcamlanh", "hpq_trieuchung_caithien" };
        private static readonly string[] D5_KEYS = { "ut_vetloet", "ut_hodai", "ut_ankhongtieu", "ut_thaydoi_thoiquen_ruotbongdai", "ut_cucu", "ut_notruoi", "ut_hachto",
            "ut_utai_nghetmui", "ut_sutcan", "ut_chaymau_dauvu", "ut_chaymau_amdao" };
        private static readonly string[] D6_KEYS = { "rltc_ithungthu", "rltc_channan", "rltc_khongu", "rltc_metmoi", "rltc_khongngonmieng", "rltc_camthayte",
            "rltc_khotaptrung", "rltc_chamchap", "rltc_ynghi" };
        private static readonly string[] D7_KEYS = { "rlla_canthang", "rlla_kiemsoat_lolang", "rlla_lolang_nhieuthu", "rlla_khothugian", "rlla_bucrut", "rlla_bucboi", "rlla_lolang" };
        private static readonly string[] D81_KEYS = { "hds_tutam", "hds_tumacquanao", "hds_tudivesinh", "hds_tudichuyen_khoigiuong", "hds_kiemsoat_tieutieu", "hds_tuanuong" };
        private static readonly string[] D82_KEYS = { "hdhn_nghedt", "hdhn_tumua_vatdung", "hdhn_tunauan", "hdhn_tulamviecnha", "hdhn_tugiatquanao", "hdhn_tulaixe_batxe",
            "hdhn_tuchia_uongthuoc", "hdhn_tugiutien" };

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

        // 1. Nội khoa of the form = sections a) .. h) of the screen.
        private static readonly Section[] INTERNAL_SECTIONS = {
            new Section("EXAM_CIRCULATION", "EXAM_CIRCULATION"), new Section("EXAM_RESPIRATORY", "EXAM_RESPIRATORY"),
            new Section("EXAM_DIGESTION", "EXAM_DIGESTION"), new Section("EXAM_KIDNEY_URO", "EXAM_KIDNEY_UROLOGY"),
            new Section("EXAM_OEND", "EXAM_OEND"), new Section("EXAM_MUSCLE_BONE", "EXAM_MUSCLE_BONE"),
            new Section("EXAM_NEUROLOGICAL", "EXAM_NEUROLOGICAL"), new Section("EXAM_MENTAL", "EXAM_MENTAL") };
        private static readonly Section OBSTETRIC = new Section("EXAM_OBSTETRIC", "EXAM_OBSTETRIC");
        private static readonly Section GYNECOLOGY = new Section("EXAM_GYNECOLOGY", "EXAM_GYNECOLOGY", true);

        private bool interviewAnswered;

        // ================================================================ B + D (INTERVIEW_JSON)

        private void SetInterviewKeys()
        {
            try
            {
                // "No" is stored as a missing key: a record whose interview tab was never answered must print blank, not "No".
                interviewAnswered = rdo.InterviewAnswers != null && rdo.InterviewAnswers.Keys.Any(k => !NON_ANSWER_KEYS.Contains(k));

                YesNoHead("B1", "tsb_giadinh");
                for (int i = 0; i < B1_KEYS.Length; i++) SetX("B1_" + (i + 1) + "_X", interviewAnswered && On(B1_KEYS[i]));

                YesNoHead("D1", "macbenh");
                for (int i = 0; i < D1_KEYS.Length; i++)
                {
                    SetX("D1_" + (i + 1) + "_X", interviewAnswered && On(D1_KEYS[i]));
                    SetKey("D1_" + (i + 1) + "_PLACE", Name(D1_KEYS[i] + "_cskcb"));
                }
                string otherDisease = Mps000523Util.Str(rdo.HisKskSytHcm, "INTERVIEW_OTHER_DISEASE");
                SetX(Mps000523ExtendSingleKey.D1_OTHER_X, !string.IsNullOrWhiteSpace(otherDisease));
                SetKey(Mps000523ExtendSingleKey.D1_OTHER_TEXT, otherDisease);

                // Skip rules of the form: a section is not asked when the matching D1 disease is "Có".
                YesNoGroup("D2", D2_KEYS, On("benh_daithaoduong"));
                YesNoGroup("D3", D3_KEYS, On("benh_phoi"));
                YesNoGroup("D4", D4_KEYS, On("benh_hen"));
                // Last D5 item (vaginal bleeding) applies to women only: skipped here, SetSingleKey keeps the first value.
                YesNoGroup("D5", D5_KEYS.Take(D5_KEYS.Length - 1).ToArray(), false);
                YesNo("D5_" + D5_KEYS.Length, D5_KEYS[D5_KEYS.Length - 1], IsMale());
                FrequencyGroup("D6", D6_KEYS, On("benh_roiloan_tramcam"));
                FrequencyGroup("D7", D7_KEYS, On("benh_roiloan_loau"));

                YesNoGroup("D8_1", D81_KEYS, false);
                YesNoGroup("D8_2", D82_KEYS, false);
                string tired = Mps000523Util.Norm(Name("ttsy_metmoi"));
                SetX(Mps000523ExtendSingleKey.D8_3_1_A_X, tired.Contains("tất cả") || tired.Contains("hầu hết"));
                SetX(Mps000523ExtendSingleKey.D8_3_1_B_X, tired.Contains("không") || tired.Contains("một số"));
                YesNo("D8_3_2", "ttsy_khokhan_leothang", false);
                YesNo("D8_3_4", "ttsy_khokhan_dibo", false);
                YesNo("D8_4_1", "dgtn_bite", false);
                YesNo("D8_4_2", "dgtn_loso_binga", false);
                YesNo("D8_4_4", "dgtn_didung", false);
                bool skipMemory = On("benh_sasut_tritue");
                YesNo("D8_5_1", "gnt_trinho_bigiam", skipMemory);
                YesNo("D8_5_2", "gnt_ghinho", skipMemory || !On("gnt_trinho_bigiam"));
                YesNo("D8_5_3", "gnt_nho_noilai", skipMemory || !On("gnt_trinho_bigiam"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool On(string key)
        {
            string v = Answer(key);
            return v != null && v.Trim() == "1";
        }

        /// <summary>Group head "Không / Có" ({prefix}_NO_X / _YES_X).</summary>
        private void YesNoHead(string prefix, string key)
        {
            SetX(prefix + "_YES_X", interviewAnswered && On(key));
            SetX(prefix + "_NO_X", interviewAnswered && !On(key));
        }

        private void YesNo(string prefix, string key, bool skipped)
        {
            bool asked = interviewAnswered && !skipped;
            SetX(prefix + "_CO_X", asked && On(key));
            SetX(prefix + "_KHONG_X", asked && !On(key));
        }

        private void YesNoGroup(string prefix, string[] keys, bool skipped)
        {
            for (int i = 0; i < keys.Length; i++) YesNo(prefix + "_" + (i + 1), keys[i], skipped);
        }

        /// <summary>4-level frequency stored as a portal catalog id; the level is read from its resolved NAME.</summary>
        private void FrequencyGroup(string prefix, string[] keys, bool skipped)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                int level = skipped ? 0 : Mps000523Util.FrequencyLevel(Name(keys[i]));
                for (int l = 1; l <= 4; l++) SetX(prefix + "_" + (i + 1) + "_L" + l + "_X", level == l);
            }
        }

        // ================================================================ 1-7. KHÁM LÂM SÀNG

        private void SetSpecialtyKeys()
        {
            try
            {
                SetSectionKeys("NOI", INTERNAL_SECTIONS);
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
        /// Several screen sections merged into one block of the form: "normal" only when every examined section is normal
        /// and none has an ICD; ICD names are united; the worst rank wins; doctors are listed once each.
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
                bool isNormal = Mps000523Util.Long(hcm, s.HcmPrefix + "_IS_NORMAL") == 1;
                string preName = Mps000523Util.Str(hcm, s.HcmPrefix + "_PRE_ICD_NAME");
                string finName = Mps000523Util.Str(hcm, s.HcmPrefix + "_ICD_NAME");
                bool hasPre = !string.IsNullOrWhiteSpace(Mps000523Util.Str(hcm, s.HcmPrefix + "_PRE_ICD_CODE")) || !string.IsNullOrWhiteSpace(preName);
                bool hasFin = !string.IsNullOrWhiteSpace(Mps000523Util.Str(hcm, s.HcmPrefix + "_ICD_CODE")) || !string.IsNullOrWhiteSpace(finName);
                if (hasPre) Mps000523Util.AddIcdNames(pre, preName);
                if (hasFin) Mps000523Util.AddIcdNames(fin, finName);
                if (isNormal || hasPre || hasFin) { examined++; if (isNormal && !hasPre && !hasFin) normal++; }

                object owner = s.RankInHcm ? (object)hcm : o18;
                worst = Math.Max(worst, Mps000523Util.RankLevel(Mps000523Util.Long(owner, s.RankColumn), rdo.ExamRanks));
                string doctor = EmployeeName(Mps000523Util.Str(owner, s.LoginColumn));
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
                SetKey(Mps000523ExtendSingleKey.SAN_REFUSE_X, "");
                SetSectionKeys("SAN", new Section[0]);
                return;
            }
            bool refuseObstetric = Mps000523Util.Long(hcm, "IS_REFUSE_OBSTETRIC") == 1;
            bool refuseGynecology = Mps000523Util.Long(hcm, "IS_REFUSE_GYNECOLOGY") == 1;
            var parts = new List<Section>();
            if (!refuseObstetric) parts.Add(OBSTETRIC);
            if (!refuseGynecology) parts.Add(GYNECOLOGY);
            bool anyResult = parts.Any(s => Mps000523Util.Long(hcm, s.HcmPrefix + "_IS_NORMAL") == 1
                || !string.IsNullOrWhiteSpace(Mps000523Util.Str(hcm, s.HcmPrefix + "_PRE_ICD_CODE"))
                || !string.IsNullOrWhiteSpace(Mps000523Util.Str(hcm, s.HcmPrefix + "_ICD_CODE")));
            SetX(Mps000523ExtendSingleKey.SAN_REFUSE_X, (refuseObstetric || refuseGynecology) && !anyResult);
            SetSectionKeys("SAN", parts.ToArray());
        }

        // ================================================================ MẮT, TAI - MŨI - HỌNG, RĂNG

        private void SetEyeEntTeethKeys()
        {
            try
            {
                var o18 = rdo.HisKskOverEighteen;
                var hcm = rdo.HisKskSytHcm;
                SetKey(Mps000523ExtendSingleKey.EYE_NOGLASS_R, Mps000523Util.Str(o18, "EXAM_EYESIGHT_RIGHT"));
                SetKey(Mps000523ExtendSingleKey.EYE_NOGLASS_L, Mps000523Util.Str(o18, "EXAM_EYESIGHT_LEFT"));
                SetKey(Mps000523ExtendSingleKey.EYE_GLASS_R, Mps000523Util.Str(o18, "EXAM_EYESIGHT_GLASS_RIGHT"));
                SetKey(Mps000523ExtendSingleKey.EYE_GLASS_L, Mps000523Util.Str(o18, "EXAM_EYESIGHT_GLASS_LEFT"));
                SetKey(Mps000523ExtendSingleKey.EYE_PINHOLE_R, Mps000523Util.Str(hcm, "EXAM_EYESIGHT_PINHOLE_RIGHT"));
                SetKey(Mps000523ExtendSingleKey.EYE_PINHOLE_L, Mps000523Util.Str(hcm, "EXAM_EYESIGHT_PINHOLE_LEFT"));
                SetKey(Mps000523ExtendSingleKey.EYE_REFRACTION_R, Mps000523Util.Join(" – ", Mps000523Util.Str(hcm, "EXAM_EYE_SPHERE_RIGHT"),
                    Mps000523Util.Str(hcm, "EXAM_EYE_CYLINDER_RIGHT"), Mps000523Util.Str(hcm, "EXAM_EYE_AXIS_RIGHT")));
                SetKey(Mps000523ExtendSingleKey.EYE_REFRACTION_L, Mps000523Util.Join(" – ", Mps000523Util.Str(hcm, "EXAM_EYE_SPHERE_LEFT"),
                    Mps000523Util.Str(hcm, "EXAM_EYE_CYLINDER_LEFT"), Mps000523Util.Str(hcm, "EXAM_EYE_AXIS_LEFT")));

                SetKey(Mps000523ExtendSingleKey.ENT_L_NORMAL, Mps000523Util.Str(o18, "EXAM_ENT_LEFT_NORMAL"));
                SetKey(Mps000523ExtendSingleKey.ENT_L_WHISPER, Mps000523Util.Str(o18, "EXAM_ENT_LEFT_WHISPER"));
                SetKey(Mps000523ExtendSingleKey.ENT_R_NORMAL, Mps000523Util.Str(o18, "EXAM_ENT_RIGHT_NORMAL"));
                SetKey(Mps000523ExtendSingleKey.ENT_R_WHISPER, Mps000523Util.Str(o18, "EXAM_ENT_RIGHT_WHISPER"));

                foreach (int tooth in Mps000523Util.TEETH)
                    SetKey("TOOTH_" + tooth + "_STR", Mps000523Util.ToothCode(Name("TOOTH_" + tooth)));
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
                SetKey("CLS_GLU_FASTING", Mps000523Util.First(Mps000523Util.Str(o18, "TEST_BLOOD_GLUCO"), Cls("shm_duongmau")));
                SetKey(Mps000523ExtendSingleKey.CLS_GLU_RANDOM, "");
                SetKey("CLS_CREATININE", Mps000523Util.First(Mps000523Util.Str(o18, "TEST_BLOOD_CREATININ"), Cls("shm_creatinin")));
                SetKey("CLS_AST", Mps000523Util.First(Mps000523Util.Str(o18, "TEST_BLOOD_ASAT"), Cls("shm_asat_got")));
                SetKey("CLS_ALT", Mps000523Util.First(Mps000523Util.Str(o18, "TEST_BLOOD_ALAT"), Cls("shm_alat_gpt")));
                SetKey("CLS_URINE_OTHER", Mps000523Util.First(Mps000523Util.Str(o18, "TEST_URINE_OTHER"), Cls("xnnt_khac")));

                // No mapped value on this workstation: keep the free-text results of the KSK screen.
                SetKey(Mps000523ExtendSingleKey.CLS_BLOOD_GENERAL, anyBlood ? "" : Mps000523Util.Join("; ",
                    Mps000523Util.Str(o18, "TEST_BLOOD_HC"), Mps000523Util.Str(o18, "TEST_BLOOD_BC"), Mps000523Util.Str(o18, "TEST_BLOOD_TC")));
                SetKey(Mps000523ExtendSingleKey.CLS_URINE_GENERAL, anyUrine ? "" : Mps000523Util.Join("; ",
                    Mps000523Util.Str(o18, "TEST_URINE_GLUCO"), Mps000523Util.Str(o18, "TEST_URINE_PROTEIN")));

                SetKey(Mps000523ExtendSingleKey.CLS_DIIM_RESULT, Mps000523Util.Str(o18, "RESULT_DIIM"));
                SetKey(Mps000523ExtendSingleKey.CLS_OTHER_RESULT, Mps000523Util.Join("; ",
                    Mps000523Util.Str(o18, "OTHER_CLS_RESULT"), Mps000523Util.Str(o18, "PERIODIC_CLS_RESULT")));
                // TEST_BLOOD holds the LOGINNAME of the blood-test doctor (legacy column name).
                SetKey(Mps000523ExtendSingleKey.CLS_BLOOD_DOCTOR, EmployeeName(Mps000523Util.Str(o18, "TEST_BLOOD")));
                SetKey(Mps000523ExtendSingleKey.CLS_URINE_DOCTOR, EmployeeName(Mps000523Util.Str(o18, "TEST_URINE_LOGINNAME")));
                SetKey(Mps000523ExtendSingleKey.CLS_DIIM_DOCTOR, EmployeeName(Mps000523Util.Str(o18, "DIIM_LOGINNAME")));
                SetKey(Mps000523ExtendSingleKey.CLS_OTHER_DOCTOR, Mps000523Util.Join(", ",
                    EmployeeName(Mps000523Util.Str(o18, "OTHER_CLS_LOGINNAME")), EmployeeName(Mps000523Util.Str(o18, "PERIODIC_CLS_LOGINNAME"))));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                SetRank("HEALTH_RANK", Mps000523Util.Long(rdo.HisKskOverEighteen, "HEALTH_EXAM_RANK_ID"));

                // CONCLUSION_ICD_TYPE: 1 = chưa phát hiện bất thường, 2 = chẩn đoán sơ bộ, 3 = chẩn đoán xác định.
                long? type = Mps000523Util.Long(g, "CONCLUSION_ICD_TYPE");
                string icd = Mps000523Util.JoinIcd(Mps000523Util.Str(g, "CONCLUSION_ICD_CODE"), Mps000523Util.Str(g, "CONCLUSION_ICD_NAME"));
                SetX(Mps000523ExtendSingleKey.CONCL_NORMAL_X, type == 1);
                SetX(Mps000523ExtendSingleKey.CONCL_PRE_X, type == 2);
                SetKey(Mps000523ExtendSingleKey.CONCL_PRE_ICD, type == 2 ? icd : "");
                SetX(Mps000523ExtendSingleKey.CONCL_FINAL_X, type == 3);
                SetKey(Mps000523ExtendSingleKey.CONCL_FINAL_ICD, type == 3 ? icd : "");

                int suggest = Mps000523Util.SuggestOption(Name("SYT_SUGGEST_ID"));
                for (int i = 1; i <= 5; i++) SetX("SUGGEST_" + i + "_X", suggest == i);
                SetKey(Mps000523ExtendSingleKey.SUGGEST_OTHER_TEXT, Mps000523Util.Str(rdo.HisKskSytHcm, "SUGGEST_OTHER"));

                long? ct = Mps000523Util.Long(g, "CONCLUSION_TIME");
                string ctStr = ct.HasValue && ct.Value > 0 ? ct.Value.ToString() : "";
                SetKey(Mps000523ExtendSingleKey.CONCLUSION_DAY, ctStr.Length >= 8 ? ctStr.Substring(6, 2) : "......");
                SetKey(Mps000523ExtendSingleKey.CONCLUSION_MONTH, ctStr.Length >= 8 ? ctStr.Substring(4, 2) : "......");
                SetKey(Mps000523ExtendSingleKey.CONCLUSION_YEAR, ctStr.Length >= 8 ? ctStr.Substring(0, 4) : "20......");
                SetKey(Mps000523ExtendSingleKey.CONCLUDER_USERNAME, Mps000523Util.First(Mps000523Util.Str(g, "CONCLUDER_USERNAME"),
                    EmployeeName(Mps000523Util.Str(g, "CONCLUDER_LOGINNAME"))));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
