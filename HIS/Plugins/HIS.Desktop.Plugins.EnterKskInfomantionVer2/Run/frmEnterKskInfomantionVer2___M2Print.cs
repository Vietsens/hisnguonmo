/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong61141 - print keys of the Mau 02 template (Mps000453, prefix "M2_").
 *
 * Every tick box of the paper form has its own key whose value is "X" or "". Names of portal
 * catalog items come from the catalogs downloaded on this workstation; school / ward names are
 * stored with the record so they print without the catalog (Q3). The full key list is in
 * docs/HIS.Desktop.Plugins.EnterKskInfomantionVer2.md.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private const string M2_X = "X";

        /// <summary>Keys for the Mau 02 template. Null when the portal is not declared (old template unchanged).</summary>
        private Dictionary<string, object> BuildM2PrintKeys()
        {
            try
            {
                if (!IsSytHcmDeclared()) return null;
                Dictionary<string, object> k = new Dictionary<string, object>();
                HIS_KSK_SYT_HCM_M2 m = currentKskSytHcmM2 ?? new HIS_KSK_SYT_HCM_M2();
                HIS_KSK_UNDER_EIGHTEEN u = currentKskUnderEight ?? new HIS_KSK_UNDER_EIGHTEEN();

                AddM2AdminKeys(k, m, u);
                AddM2HistoryKeys(k, m, u);
                AddM2VaccineKeys(k, m);
                AddM2MentalKeys(k, m);
                AddM2ClinicalKeys(k, m, u);
                AddM2ConclusionKeys(k, m);
                AddM2HeaderKeys(k);
                AddM2DoctorKeys(k, u);
                AddM2GeneralConclusionKeys(k, u);
                AddM2BoxKeys(k);
                return k;
            }
            catch (Exception ex) { LogSystem.Error(ex); return null; }
        }

        private void AddM2AdminKeys(Dictionary<string, object> k, HIS_KSK_SYT_HCM_M2 m, HIS_KSK_UNDER_EIGHTEEN u)
        {
            long gender = currentServiceReq != null ? (currentServiceReq.TDL_PATIENT_GENDER_ID ?? 0) : 0;
            k["M2_GIOI_TINH_NAM_X"] = Mx(gender == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE);
            k["M2_GIOI_TINH_NU_X"] = Mx(gender == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE);

            // Administration, already formatted for the paper form.
            HIS_PATIENT pt = printKskPatient;
            string name = (pt != null ? pt.VIR_PATIENT_NAME : null)
                ?? (currentServiceReq != null ? currentServiceReq.TDL_PATIENT_NAME : null) ?? "";
            k["M2_HO_TEN"] = name.ToUpper();
            long dob = (pt != null) ? pt.DOB : (currentServiceReq != null ? currentServiceReq.TDL_PATIENT_DOB : 0);
            k["M2_NGAY_SINH"] = M2DateStr(dob);
            k["M2_DAN_TOC"] = (pt != null ? pt.ETHNIC_NAME : null) ?? "";
            string abo = (pt != null ? pt.BLOOD_ABO_CODE : null) ?? "";
            string rh = (pt != null ? pt.BLOOD_RH_CODE : null) ?? "";
            k["M2_NHOM_MAU"] = (abo + " " + rh).Trim();
            k["M2_CCCD"] = (pt != null ? (pt.CCCD_NUMBER ?? pt.CMND_NUMBER ?? pt.PASSPORT_NUMBER) : null) ?? "";
            k["M2_BHYT"] = (pt != null ? pt.TDL_HEIN_CARD_NUMBER : null) ?? "";
            k["M2_NOI_O"] = (pt != null ? (pt.HT_ADDRESS ?? pt.ADDRESS) : null) ?? "";
            k["M2_XA_PHUONG"] = (pt != null ? (pt.HT_COMMUNE_NAME ?? pt.COMMUNE_NAME) : null) ?? "";
            k["M2_LY_DO_KHAM"] = (txtLyDoKham != null ? txtLyDoKham.Text : null) ?? "";

            k["M2_DOI_TUONG"] = M2Names(M2_CAT__DOI_TUONG, m.SYT_PATIENT_TYPES);
            k["M2_DOI_TUONG_ID"] = m.SYT_PATIENT_TYPES ?? "";

            string place = M2Name(M2_CAT__DIA_DIEM, m.SYT_EXAM_PLACE_ID);
            k["M2_DIA_DIEM_KHAM"] = place;
            string np = Norm2(place);
            k["M2_DDK_CSKBCB_X"] = Mx(np.Contains("co so") || np.Contains("cs kbcb") || np.Contains("kham benh"));
            k["M2_DDK_LUU_DONG_X"] = Mx(np.Contains("luu dong"));
            k["M2_DDK_TRAM_YT_X"] = Mx(np.Contains("tram"));
            k["M2_DDK_TRUONG_HOC_X"] = Mx(np.Contains("truong"));
            k["M2_DDK_TAI_NHA_X"] = Mx(np.Contains("nha"));

            string pay = M2Name(M2_CAT__HINH_THUC, m.SYT_PAYSOURCE_ID);
            string payDetail = M2Name(M2_CAT__HINH_THUC_CT, m.SYT_PAY_SOURCE_DETAIL_ID);
            k["M2_HT_CHI_TRA"] = pay;
            k["M2_HT_CHI_TRA_CT"] = payDetail;
            string npay = Norm2(pay);
            string ndet = Norm2(payDetail);
            k["M2_CT_NGAN_SACH_X"] = Mx(npay.Contains("ngan sach"));
            k["M2_CT_TU_CHI_TRA_X"] = Mx(npay.Contains("tu chi tra"));
            k["M2_CT_NGUON_KHAC_X"] = Mx(npay.Contains("khac"));
            k["M2_CT_HOP_DONG_X"] = Mx(ndet.Contains("hop dong"));
            k["M2_CT_PHI_DIA_GIOI_X"] = Mx(ndet.Contains("phi dia gioi"));
            k["M2_NGUON_KHAC"] = m.PAY_SOURCE_OTHER ?? "";

            k["M2_DI_HOC_CO_X"] = Mx(m.IS_GO_TO_SCHOOL == 1);
            k["M2_DI_HOC_KHONG_X"] = Mx(m.IS_GO_TO_SCHOOL == 0);
            k["M2_TRUONG"] = m.SCHOOL_NAME ?? "";
            k["M2_LOP"] = m.CLASS_NAME ?? "";
            k["M2_DIA_CHI_TRUONG"] = m.SCHOOL_ADDRESS ?? "";
            k["M2_XA_PHUONG_TRUONG"] = m.SCHOOL_WARD_NAME ?? "";

            k["M2_GIAM_HO_TEN"] = u.ACCOMPANY_PERSON_NAME ?? "";
            k["M2_GIAM_HO_CCCD"] = m.GUARDIAN_CCCD ?? "";
            k["M2_GIAM_HO_SDT"] = m.GUARDIAN_PHONE ?? "";
            for (int i = 1; i <= 6; i++)
                k["M2_QH_" + i + "_X"] = Mx(u.ACCOMPANY_RELATIONSHIP == i);

            // One character per box of the paper form (CCCD 12 boxes, BHYT 2 + 13 boxes).
            AddM2CharKeys(k, "M2_CCCD_", (string)k["M2_CCCD"], 12);
            AddM2CharKeys(k, "M2_GIAM_HO_CCCD_", (string)k["M2_GIAM_HO_CCCD"], 12);
            AddM2CharKeys(k, "M2_BHYT_", (string)k["M2_BHYT"], 15);
        }

        /// <summary>Keys {prefix}1..{count}: the letters/digits of a value, one per key (blank when shorter).</summary>
        private static void AddM2CharKeys(Dictionary<string, object> k, string prefix, string value, int count)
        {
            string s = new string((value ?? "").Where(char.IsLetterOrDigit).ToArray()).ToUpper();
            for (int i = 1; i <= count; i++)
                k[prefix + i] = i <= s.Length ? s[i - 1].ToString() : "";
        }

        private void AddM2HistoryKeys(Dictionary<string, object> k, HIS_KSK_SYT_HCM_M2 m, HIS_KSK_UNDER_EIGHTEEN u)
        {
            k["M2_TSGD_KHONG_X"] = Mx(m.FAMILY_HISTORY_HAS == 0);
            k["M2_TSGD_CO_X"] = Mx(m.FAMILY_HISTORY_HAS == 1);
            k["M2_TSGD_DS"] = (m.FAMILY_HISTORY_HAS == 1 && IsM2IdList(u.PATHOLOGICAL_HISTORY_FAMILY))
                ? M2Names(M2_CAT__TS_GIA_DINH, u.PATHOLOGICAL_HISTORY_FAMILY) : "";

            k["M2_SK_BT_X"] = Mx(m.OBSTETRIC_IS_NORMAL == 1);
            k["M2_SK_KBT_X"] = Mx(m.OBSTETRIC_IS_NORMAL == 0);
            string obst = (m.OBSTETRIC_IS_NORMAL == 0) ? M2Names(M2_CAT__SAN_KHOA, m.OBSTETRIC_ABNORMAL_IDS) : "";
            k["M2_SK_DS"] = obst;
            List<string> obstNames = SplitM2Names(obst);
            k["M2_SK_THIEU_THANG_X"] = Mx(obstNames.Any(n => n.Contains("thieu thang")));
            k["M2_SK_THUA_CAN_X"] = Mx(obstNames.Any(n => n.Contains("thua can")));
            k["M2_SK_CAN_THIEP_X"] = Mx(obstNames.Any(n => n.Contains("can thiep")));
            k["M2_SK_NGAT_X"] = Mx(obstNames.Any(n => n.Contains("ngat")));
            k["M2_ME_BENH"] = m.MOTHER_DISEASE ?? "";
            k["M2_SK_ME_BENH_X"] = Mx(!string.IsNullOrWhiteSpace(m.MOTHER_DISEASE));

            k["M2_TSBT_KHONG_X"] = Mx(m.PERSONAL_HISTORY_HAS == 0);
            k["M2_TSBT_CO_X"] = Mx(m.PERSONAL_HISTORY_HAS == 1);
            List<string> personal = new List<string>();
            if (m.PERSONAL_HISTORY_HAS == 1)
            {
                string a = M2Names(M2_CAT__TS_BAN_THAN, m.PERSONAL_HISTORY_IDS);
                if (!string.IsNullOrEmpty(a)) personal.Add(a);
                string b = M2IcdNames(m.PERSONAL_HISTORY_ICD, null);
                if (!string.IsNullOrEmpty(b)) personal.Add(b);
            }
            k["M2_TSBT_DS"] = string.Join("; ", personal.ToArray());

            k["M2_DIEU_TRI_KHONG_X"] = Mx(m.IS_TREATING == 0);
            k["M2_DIEU_TRI_CO_X"] = Mx(m.IS_TREATING == 1);
            k["M2_DIEU_TRI_THUOC"] = (m.IS_TREATING == 1) ? (u.MEDICINE_USING ?? "") : "";
        }

        private void AddM2VaccineKeys(Dictionary<string, object> k, HIS_KSK_SYT_HCM_M2 m)
        {
            JObject o = null;
            try { if (!string.IsNullOrWhiteSpace(m.VACCINATION_JSON)) o = JObject.Parse(m.VACCINATION_JSON); }
            catch (Exception ex) { LogSystem.Warn(ex); }

            List<string> fields = KskM2Definition.VACCINES.Select(v => v.Field).ToList();
            fields.Add(KskM2Definition.VACCINE_OTHER_KEY);
            foreach (string f in fields)
            {
                string p = "M2_" + (f == KskM2Definition.VACCINE_OTHER_KEY ? "TIEM_KHAC" : f.ToUpperInvariant());
                JObject e = (o != null) ? o[f] as JObject : null;
                int s = (e != null) ? (e.Value<int?>("s") ?? 0) : 0;
                k[p + "_DA_X"] = Mx(s == (int)KskM2VaccineStatus.Done);
                k[p + "_CHUA_X"] = Mx(s == (int)KskM2VaccineStatus.NotDone);
                k[p + "_KNR_X"] = Mx(s == (int)KskM2VaccineStatus.Unknown);
                DateTime? d = (e != null) ? ParseM2Ymd(e.Value<string>("d")) : null;
                k[p + "_NGAY"] = d.HasValue ? d.Value.ToString("dd/MM/yyyy") : "";
                if (f == KskM2Definition.VACCINE_OTHER_KEY)
                {
                    k[p + "_TEN"] = (e != null) ? (e.Value<string>("ten") ?? "") : "";
                    k[p + "_LICH"] = (e != null) ? (e.Value<string>("lich") ?? "") : "";
                    k[p + "_MUI"] = (e != null) ? (e.Value<string>("mui") ?? "") : "";
                }
            }
        }

        private void AddM2MentalKeys(Dictionary<string, object> k, HIS_KSK_SYT_HCM_M2 m)
        {
            k["M2_ADHD_NGAY"] = M2DateStr(m.ADHD_DATE);
            AddM2AnswerKeys(k, "M2_ADHD_", KskM2Definition.ADHD_QUESTIONS, m.ADHD_JSON, 3);
            k["M2_ADHD_KET_QUA"] = m.ADHD_RESULT ?? "";
            k["M2_ADHD_GHI_CHU"] = m.ADHD_NOTE ?? "";

            k["M2_ASD_NGAY"] = M2DateStr(m.ASD_DATE);
            Dictionary<string, int> asd = AddM2AnswerKeys(k, "M2_ASD_", KskM2Definition.ASD_QUESTIONS, m.ASD_JSON, 4);
            k["M2_ASD_DIEM"] = asd.Count > 0 ? KskM2Definition.ScoreAsd(asd).ToString() : "";
            k["M2_ASD_KET_QUA"] = m.ASD_RESULT ?? "";
            k["M2_ASD_GHI_CHU"] = m.ASD_NOTE ?? "";
        }

        /// <summary>Keys {prefix}{no}_{level}_X for every question and answer column.</summary>
        private static Dictionary<string, int> AddM2AnswerKeys(Dictionary<string, object> k, string prefix,
            List<KskM2QuestionDef> questions, string json, int levels)
        {
            Dictionary<string, int> answers = new Dictionary<string, int>();
            JObject o = null;
            try { if (!string.IsNullOrWhiteSpace(json)) o = JObject.Parse(json); }
            catch (Exception ex) { LogSystem.Warn(ex); }
            foreach (KskM2QuestionDef q in questions)
            {
                int v = (o != null) ? (o.Value<int?>(q.Field) ?? 0) : 0;
                if (v > 0) answers[q.Field] = v;
                for (int l = 1; l <= levels; l++) k[prefix + q.No + "_" + l + "_X"] = Mx(v == l);
            }
            return answers;
        }

        private void AddM2ClinicalKeys(Dictionary<string, object> k, HIS_KSK_SYT_HCM_M2 m, HIS_KSK_UNDER_EIGHTEEN u)
        {
            foreach (KskM2SectionDef s in KskM2Definition.SECTIONS)
            {
                string p = "M2_" + s.Key.ToUpperInvariant();
                bool normal = GetM2Short(m, s.ColumnPrefix + M2_SUFFIX__IS_NORMAL) == 1;
                string pre = M2IcdNames(GetM2Str(m, s.ColumnPrefix + M2_SUFFIX__PRE_CODE), GetM2Str(m, s.ColumnPrefix + M2_SUFFIX__PRE_NAME));
                string fin = M2IcdNames(GetM2Str(m, s.ColumnPrefix + M2_SUFFIX__CODE), GetM2Str(m, s.ColumnPrefix + M2_SUFFIX__NAME));
                k[p + "_BT_X"] = Mx(normal);
                k[p + "_SB_X"] = Mx(!string.IsNullOrEmpty(pre));
                k[p + "_SB_ICD"] = pre;
                k[p + "_XD_X"] = Mx(!string.IsNullOrEmpty(fin));
                k[p + "_XD_ICD"] = fin;
            }

            // Rank I..V of each specialty + physical rank (columns of the under-18 record).
            AddM2RankKeys(k, "M2_THE_LUC", u.DHST_RANK);
            AddM2RankKeys(k, "M2_NOIKHOA", u.EXAM_CIRCULATION_RANK);
            AddM2RankKeys(k, "M2_HOHAP", u.EXAM_RESPIRATORY_RANK);
            AddM2RankKeys(k, "M2_TIEUHOA", u.EXAM_DIGESTION_RANK);
            AddM2RankKeys(k, "M2_THANTIETNIEU", u.EXAM_KIDNEY_UROLOGY_RANK);
            AddM2RankKeys(k, "M2_THANKINH", u.EXAM_NEURO_MENTAL_RANK ?? u.EXAM_NEUROLOGICAL_RANK);
            AddM2RankKeys(k, "M2_TAMTHAN", u.EXAM_MENTAL_RANK);
            AddM2RankKeys(k, "M2_MAT", u.EXAM_EYE_RANK);
            AddM2RankKeys(k, "M2_TMH", u.EXAM_ENT_RANK);
            AddM2RankKeys(k, "M2_RHM", u.EXAM_STOMATOLOGY_RANK);

            // Spine (names of the portal catalogs).
            string spine = Norm2(M2Name(M2_CAT__COT_SONG, m.SPINE_ID));
            k["M2_COT_SONG_BT_X"] = Mx(spine.Contains("binh thuong"));
            k["M2_COT_SONG_BAT_THUONG_X"] = Mx(spine.Contains("bat thuong"));
            string ky = Norm2(M2Name(M2_CAT__CONG_COT_SONG, m.SPINE_KYPHOSIS_ID));
            k["M2_GU_X"] = Mx(ky.Contains("gu"));
            k["M2_UON_X"] = Mx(ky.Contains("uon"));
            k["M2_BET_X"] = Mx(ky.Contains("bet"));
            string sc = Norm2(M2Name(M2_CAT__VEO_COT_SONG, m.SPINE_SCOLIOSIS_ID));
            k["M2_VEO_S_X"] = Mx(sc.EndsWith(" s") || sc.Contains("chu s") || sc == "s");
            k["M2_VEO_C_X"] = Mx(sc.EndsWith(" c") || sc.Contains("chu c") || sc == "c");
            string lv = Norm2(M2Name(M2_CAT__MUC_DO_VEO, m.SPINE_SCOLIOSIS_LEVEL_ID));
            k["M2_VEO_NHE_X"] = Mx(lv.Contains("nhe"));
            k["M2_VEO_VUA_X"] = Mx(lv.Contains("vua"));
            k["M2_VEO_NANG_X"] = Mx(lv.Contains("nang"));

            // Teeth: legend symbol of the form (digits for permanent teeth, letters for primary teeth).
            List<KskCodeNameADO> toothCat = ToCodeNameList(M2_CAT__TINH_TRANG_RANG);
            Dictionary<int, string> toothNames = toothCat.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First().NAME);
            HashSet<string> primary = new HashSet<string>(
                KskM2Definition.TEETH_PRIMARY_UPPER.Concat(KskM2Definition.TEETH_PRIMARY_LOWER));
            foreach (string t in KskM2Definition.AllTeeth())
            {
                long? v = GetM2Long(m, "TOOTH_" + t);
                string name;
                string sym = (v.HasValue && toothNames.TryGetValue((int)v.Value, out name))
                    ? M2ToothSymbol(name, primary.Contains(t)) : "";
                k["M2_RANG_" + t] = sym;
            }

            k["M2_XN_NGAY"] = M2DateStr(m.TEST_DATE);
            k["M2_XN_DON_VI"] = m.TEST_UNIT_NAME ?? "";
            k["M2_XN_KET_QUA"] = m.TEST_RESULT ?? "";
            k["M2_CDHA_KET_QUA"] = m.IMAGING_RESULT ?? "";
        }

        private void AddM2ConclusionKeys(Dictionary<string, object> k, HIS_KSK_SYT_HCM_M2 m)
        {
            string name = M2Name(M2_CAT__DE_NGHI, m.SYT_SUGGEST_ID);
            string n = Norm2(name);
            k["M2_DN_TEN"] = name;
            k["M2_DN_1_X"] = Mx(n.Contains("binh thuong"));
            k["M2_DN_2_X"] = Mx(n.Contains("nguy co"));
            k["M2_DN_3_X"] = Mx(n.Contains("man tinh"));
            k["M2_DN_4_X"] = Mx(n.Contains("chuyen"));
            k["M2_DN_5_X"] = Mx(n.Contains("khac"));
            k["M2_DN_GHI_RO"] = m.SUGGEST_OTHER ?? "";
        }

        /// <summary>Header of the form: managing body + name of the facility (current branch).</summary>
        private void AddM2HeaderKeys(Dictionary<string, object> k)
        {
            string parent = "";
            string branchName = "";
            try
            {
                HIS_BRANCH b = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch;
                if (b != null)
                {
                    parent = b.PARENT_ORGANIZATION_NAME ?? "";
                    branchName = b.BRANCH_NAME ?? "";
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            k["M2_SO_Y_TE"] = (string.IsNullOrWhiteSpace(parent) ? "SỞ Y TẾ THÀNH PHỐ HỒ CHÍ MINH" : parent).ToUpper();
            k["M2_TEN_CO_SO"] = branchName.ToUpper();
        }

        /// <summary>Full name of the doctor of each specialty (column "Họ tên, chữ ký" of the form).
        /// The under-18 record only stores login names.</summary>
        private void AddM2DoctorKeys(Dictionary<string, object> k, HIS_KSK_UNDER_EIGHTEEN u)
        {
            k["M2_NOIKHOA_BS"] = M2UserName(u.EXAM_CIRCULATION_LOGINNAME);
            k["M2_HOHAP_BS"] = M2UserName(u.EXAM_RESPIRATORY_LOGINNAME);
            k["M2_TIEUHOA_BS"] = M2UserName(u.EXAM_DIGESTION_LOGINNAME);
            k["M2_THANTIETNIEU_BS"] = M2UserName(u.EXAM_KIDNEY_UROLOGY_LOGINNAME);
            k["M2_THANKINH_BS"] = M2UserName(u.EXAM_NEURO_MENTAL_LOGINNAME ?? u.EXAM_NEUROLOGICAL_LOGINNAME);
            k["M2_TAMTHAN_BS"] = M2UserName(u.EXAM_MENTAL_LOGINNAME);
            k["M2_COTSONG_CLSKHAC_BS"] = M2UserName(u.EXAM_CLINICAL_OTHER_LOGINNAME);
            k["M2_MAT_BS"] = M2UserName(u.EXAM_EYE_LOGINNAME);
            k["M2_TMH_BS"] = M2UserName(u.EXAM_ENT_LOGINNAME);
            k["M2_RHM_BS"] = M2UserName(u.EXAM_STOMATOLOGY_LOGINNAME);
            k["M2_CLS_BS"] = M2UserName(u.EXAM_SUBCLINICAL_LOGINNAME);
        }

        /// <summary>KET LUAN of the form: health status (ICD conclusion saved in HIS_KSK_GENERAL),
        /// rank I..V of the under-18 record, concluder name.</summary>
        private void AddM2GeneralConclusionKeys(Dictionary<string, object> k, HIS_KSK_UNDER_EIGHTEEN u)
        {
            HIS_KSK_GENERAL g = currentKskGeneral;
            long type = (g != null && g.CONCLUSION_ICD_TYPE.HasValue) ? (long)g.CONCLUSION_ICD_TYPE.Value : 0;
            string icd = (g != null) ? string.Join(" - ", new[] { g.CONCLUSION_ICD_CODE, g.CONCLUSION_ICD_NAME }
                .Where(o => !string.IsNullOrWhiteSpace(o)).ToArray()) : "";
            k["M2_KL_BT_X"] = Mx(type == 1);
            k["M2_KL_SB_X"] = Mx(type == 2);
            k["M2_KL_XD_X"] = Mx(type == 3);
            k["M2_KL_SB_ICD"] = type == 2 ? icd : "";
            k["M2_KL_XD_ICD"] = type == 3 ? icd : "";
            AddM2RankKeys(k, "M2_KL", u.HEALTH_EXAM_RANK_ID);
            string concluder = (g != null) ? g.CONCLUDER_USERNAME : null;
            k["M2_KL_BS"] = !string.IsNullOrWhiteSpace(concluder) ? concluder
                : M2UserName(g != null ? g.CONCLUDER_LOGINNAME : null);
        }

        /// <summary>For every tick key "..._X" add "..._O" = a box glyph (checked / empty) used for the
        /// inline tick boxes of the template; "..._X" ("X" / "") stays for the table cells.</summary>
        private static void AddM2BoxKeys(Dictionary<string, object> k)
        {
            foreach (string key in k.Keys.Where(o => o.EndsWith("_X")).ToList())
            {
                string boxKey = key.Substring(0, key.Length - 2) + "_O";
                k[boxKey] = (k[key] as string) == M2_X ? "☒" : "☐";
            }
        }

        #region ===== Helpers =====

        private static string Mx(bool b) { return b ? M2_X : ""; }

        /// <summary>Full name of an employee from the login name (login name itself when not found).</summary>
        private static string M2UserName(string loginName)
        {
            if (string.IsNullOrWhiteSpace(loginName)) return "";
            try
            {
                V_HIS_EMPLOYEE e = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                    .FirstOrDefault(o => o.LOGINNAME == loginName);
                if (e != null && !string.IsNullOrWhiteSpace(e.TDL_USERNAME)) return e.TDL_USERNAME;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return loginName;
        }

        private static string M2DateStr(long? v)
        {
            if (!v.HasValue || v.Value <= 0) return "";
            DateTime? d = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(v.Value);
            return d.HasValue ? d.Value.ToString("dd/MM/yyyy") : "";
        }

        private static string M2Name(string catalogCode, long? id)
        {
            if (!id.HasValue) return "";
            KskCodeNameADO it = ToCodeNameList(catalogCode).FirstOrDefault(o => o.ID == id.Value);
            return it != null ? (it.NAME ?? "") : "";
        }

        /// <summary>Names of a list of ids "1,2,3" (separator ", ").</summary>
        private static string M2Names(string catalogCode, string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return "";
            Dictionary<int, string> map = ToCodeNameList(catalogCode).GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First().NAME);
            List<string> names = new List<string>();
            foreach (string part in ids.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int id;
                string name;
                if (int.TryParse(part.Trim(), out id) && map.TryGetValue(id, out name)) names.Add(name);
            }
            return string.Join(", ", names.ToArray());
        }

        private static List<string> SplitM2Names(string names)
        {
            return (names ?? "").Split(',').Select(o => Norm2(o)).Where(o => o.Length > 0).ToList();
        }

        /// <summary>ICD names: stored names first, else looked up in the portal ICD catalog.</summary>
        private static string M2IcdNames(string ids, string storedNames)
        {
            if (!string.IsNullOrWhiteSpace(storedNames)) return storedNames;
            if (string.IsNullOrWhiteSpace(ids) || sytIcdItems == null) return "";
            List<string> names = new List<string>();
            foreach (string part in ids.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                long id;
                if (!long.TryParse(part.Trim(), out id)) continue;
                SytIcdItem it = sytIcdItems.FirstOrDefault(o => o.ID == id);
                if (it != null) names.Add(it.TEN_BENH);
            }
            return string.Join("; ", names.ToArray());
        }

        /// <summary>Keys {prefix}_LOAI_1_X .. _LOAI_5_X from a HIS rank id.</summary>
        private static void AddM2RankKeys(Dictionary<string, object> k, string prefix, long? rankId)
        {
            int level = M2RankLevel(rankId);
            for (int i = 1; i <= 5; i++) k[prefix + "_LOAI_" + i + "_X"] = Mx(level == i);
        }

        private static int M2RankLevel(long? rankId)
        {
            try
            {
                if (!rankId.HasValue) return 0;
                HIS_HEALTH_EXAM_RANK r = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>()
                    .FirstOrDefault(o => o.ID == rankId.Value);
                if (r == null) return 0;
                string n = " " + Norm2(r.HEALTH_EXAM_RANK_NAME ?? r.HEALTH_EXAM_RANK_CODE) + " ";
                string[] romans = { " v ", " iv ", " iii ", " ii ", " i " };
                int[] levels = { 5, 4, 3, 2, 1 };
                for (int i = 0; i < romans.Length; i++)
                    if (n.Contains(romans[i])) return levels[i];
                for (int i = 1; i <= 5; i++)
                    if (n.Contains(" " + i + " ")) return i;
                return 0;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return 0; }
        }

        /// <summary>Legend of Mau 02: 0/A Binh thuong, 1/B Sau, 2/C Tram sau lai, 3/D Tram tot, 4/E Mat do sau,
        /// 5 Mat ly do khac, 6/F Bit ho ranh, 7/G Tru cau, 8 Chua moc, 9 Loai tru.</summary>
        private static string M2ToothSymbol(string name, bool primaryTooth)
        {
            string n = Norm2(name);
            string[][] map =
            {
                new[] { "tram sau lai", "2", "C" }, new[] { "tram tot", "3", "D" }, new[] { "mat do sau", "4", "E" },
                new[] { "mat", "5", "5" }, new[] { "bit ho", "6", "F" }, new[] { "loai tru", "9", "9" },
                new[] { "tru", "7", "G" }, new[] { "chua moc", "8", "8" }, new[] { "sau", "1", "B" },
                new[] { "binh thuong", "0", "A" }
            };
            foreach (string[] e in map)
                if (n.Contains(e[0])) return primaryTooth ? e[2] : e[1];
            return "";
        }

        #endregion
    }
}
