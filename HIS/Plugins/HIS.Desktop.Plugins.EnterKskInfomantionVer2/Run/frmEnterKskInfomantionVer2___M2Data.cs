/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong61141 - load / fill / clear / save of HIS_KSK_SYT_HCM_M2 (one record per under-18 record).
 *
 * Order (same as M3): save the under-18 record (existing API) -> the response gives its ID ->
 * create or update the M2 record (R2). Update starts from the stored record so columns of
 * controls that could not be read (catalog not loaded yet) keep their stored values (R3).
 * If the M2 save fails the KSK record is already saved -> tell the user to save again (R4).
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private const string API__SYT_HCM_M2_GET = "api/HisKskSytHcmM2/Get";
        private const string API__SYT_HCM_M2_CREATE = "api/HisKskSytHcmM2/Create";
        private const string API__SYT_HCM_M2_UPDATE = "api/HisKskSytHcmM2/Update";

        /// <summary>Index of tab "Ksk duoi 18 tuoi" in xtraTabControl1.</summary>
        private const int TAB_INDEX__UNDER_EIGHTEEN = 2;

        /// <summary>M2 record of the open under-18 record. Null = none stored yet.</summary>
        private HIS_KSK_SYT_HCM_M2 currentKskSytHcmM2;

        private const string M2_SUFFIX__IS_NORMAL = "_IS_NORMAL";
        private const string M2_SUFFIX__PRE_CODE = "_PRE_ICD_CODE";
        private const string M2_SUFFIX__PRE_NAME = "_PRE_ICD_NAME";
        private const string M2_SUFFIX__CODE = "_ICD_CODE";
        private const string M2_SUFFIX__NAME = "_ICD_NAME";

        #region ===== Load =====

        /// <summary>Loads the M2 record of one under-18 record. Portal not declared -> no API call.</summary>
        private void LoadKskSytHcmM2(long kskUnderEighteenId)
        {
            try
            {
                currentKskSytHcmM2 = null;
                if (!IsSytHcmDeclared() || kskUnderEighteenId <= 0) return;
                CommonParam param = new CommonParam();
                HisKskSytHcmM2Filter filter = new HisKskSytHcmM2Filter();
                filter.KSK_UNDER_EIGHTEEN_ID = kskUnderEighteenId;
                List<HIS_KSK_SYT_HCM_M2> rs = new BackendAdapter(param).Get<List<HIS_KSK_SYT_HCM_M2>>(
                    API__SYT_HCM_M2_GET, ApiConsumers.MosConsumer, filter, param);
                // Re-check the key on each row: never trust a list filter alone.
                currentKskSytHcmM2 = (rs != null)
                    ? rs.FirstOrDefault(o => o != null && o.KSK_UNDER_EIGHTEEN_ID == kskUnderEighteenId)
                    : null;
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        #endregion

        #region ===== Fill / clear =====

        /// <summary>Fills the M2 tabs from currentKskSytHcmM2 (clears them when null).</summary>
        private void FillM2Controls()
        {
            try
            {
                if (!isM2Inited) return;
                // The under-18 page resets its captions when it is (re)filled -> apply ours again.
                ApplyM2GuardianCaption();
                HIS_KSK_SYT_HCM_M2 d = currentKskSytHcmM2;
                m2Filling = true;
                try
                {
                    ClearM2ControlsCore();

                    // Values that live in the under-18 record (family list, medicine).
                    if (currentKskUnderEight != null)
                    {
                        if (IsM2IdList(currentKskUnderEight.PATHOLOGICAL_HISTORY_FAMILY))
                            SetM2CheckedIds(cboM2FamilyList, currentKskUnderEight.PATHOLOGICAL_HISTORY_FAMILY);
                    }
                    if (this.txtMedicineUsing3 != null) txtM2TreatingMedicine.Text = this.txtMedicineUsing3.Text;

                    if (d == null) return;

                    SetM2CheckedIds(cboM2PatientTypes, d.SYT_PATIENT_TYPES);
                    SetM2LookupId(cboM2ExamPlace, d.SYT_EXAM_PLACE_ID);
                    SetM2LookupId(cboM2PaySource, d.SYT_PAYSOURCE_ID);
                    SetM2LookupId(cboM2PaySourceDetail, d.SYT_PAY_SOURCE_DETAIL_ID);
                    txtM2PayOther.Text = d.PAY_SOURCE_OTHER;

                    SetM2Flag(rdoM2GoToSchool, d.IS_GO_TO_SCHOOL);
                    SetM2LookupId(cboM2School, d.SCHOOL_ID);
                    txtM2Class.Text = d.CLASS_NAME;
                    txtM2SchoolAddress.Text = d.SCHOOL_ADDRESS;
                    SetM2LookupId(cboM2SchoolWard, d.SCHOOL_WARD_ID);
                    txtM2GuardianCccd.Text = d.GUARDIAN_CCCD;
                    txtM2GuardianPhone.Text = d.GUARDIAN_PHONE;

                    SetM2Flag(rdoM2FamilyHas, d.FAMILY_HISTORY_HAS);
                    SetM2Flag(rdoM2Obstetric, d.OBSTETRIC_IS_NORMAL);
                    SetM2CheckedIds(cboM2ObstetricAbnormal, d.OBSTETRIC_ABNORMAL_IDS);
                    txtM2MotherDisease.Text = d.MOTHER_DISEASE;
                    SetM2Flag(rdoM2PersonalHas, d.PERSONAL_HISTORY_HAS);
                    SetM2CheckedIds(cboM2PersonalList, d.PERSONAL_HISTORY_IDS);
                    SetSytIcdValue(ucM2PersonalIcd, d.PERSONAL_HISTORY_ICD, null);
                    SetM2Flag(rdoM2Treating, d.IS_TREATING);

                    FillM2Vaccines(d.VACCINATION_JSON);

                    M2LongToDate(dteM2AdhdDate, d.ADHD_DATE);
                    FillM2Answers(m2AdhdRows, d.ADHD_JSON);
                    txtM2AdhdResult.Text = d.ADHD_RESULT;
                    txtM2AdhdNote.Text = d.ADHD_NOTE;
                    M2LongToDate(dteM2AsdDate, d.ASD_DATE);
                    FillM2Answers(m2AsdRows, d.ASD_JSON);
                    txtM2AsdResult.Text = d.ASD_RESULT;
                    txtM2AsdNote.Text = d.ASD_NOTE;
                    Dictionary<string, int> asd = ReadM2Answers(m2AsdRows);
                    txtM2AsdScore.Text = asd.Count > 0 ? KskM2Definition.ScoreAsd(asd).ToString() : "";
                    // Stored results count as "typed by the user" for R13 unless equal to the suggestion.
                    m2AdhdAutoText = KskM2Definition.SuggestAdhdResult(ReadM2Answers(m2AdhdRows));
                    m2AsdAutoText = KskM2Definition.SuggestAsdResult(asd);

                    foreach (M2SectionRow row in m2SectionRows)
                    {
                        string p = row.Def.ColumnPrefix;
                        SetSytIcdValue(row.UcPre, GetM2Str(d, p + M2_SUFFIX__PRE_CODE), GetM2Str(d, p + M2_SUFFIX__PRE_NAME));
                        SetSytIcdValue(row.UcFinal, GetM2Str(d, p + M2_SUFFIX__CODE), GetM2Str(d, p + M2_SUFFIX__NAME));
                        // Checkbox LAST: checking it clears the two ICD boxes.
                        row.ChkNormal.Checked = GetM2Short(d, p + M2_SUFFIX__IS_NORMAL) == 1;
                    }

                    SetM2RadioId(rdoM2Spine, d.SPINE_ID);
                    SetM2RadioId(rdoM2Kyphosis, d.SPINE_KYPHOSIS_ID);
                    SetM2RadioId(rdoM2Scoliosis, d.SPINE_SCOLIOSIS_ID);
                    SetM2RadioId(rdoM2ScoliosisLevel, d.SPINE_SCOLIOSIS_LEVEL_ID);

                    foreach (KeyValuePair<string, LookUpEdit> t in m2ToothEdits)
                    {
                        long? v = GetM2Long(d, "TOOTH_" + t.Key);
                        t.Value.EditValue = v.HasValue ? (object)(int)v.Value : null;
                    }

                    M2LongToDate(dteM2TestDate, d.TEST_DATE);
                    txtM2TestUnit.Text = d.TEST_UNIT_NAME;
                    txtM2TestResult.Text = d.TEST_RESULT;
                    txtM2ImagingResult.Text = d.IMAGING_RESULT;
                    SetM2RadioId(rdoM2Suggest, d.SYT_SUGGEST_ID);
                    txtM2SuggestOther.Text = d.SUGGEST_OTHER;
                }
                finally { m2Filling = false; }
                ApplyM2EnableRules();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// ICD boxes cannot show stored portal ids before the ICD catalog is loaded (they stay empty).
        /// When the catalog arrives, put the stored values back so the next save does not clear them.
        /// </summary>
        private void RefillM2IcdFromRecord()
        {
            try
            {
                if (!isM2Inited || currentKskSytHcmM2 == null) return;
                HIS_KSK_SYT_HCM_M2 d = currentKskSytHcmM2;
                m2Filling = true;
                try
                {
                    SetSytIcdValue(ucM2PersonalIcd, d.PERSONAL_HISTORY_ICD, null);
                    foreach (M2SectionRow row in m2SectionRows)
                    {
                        if (row.ChkNormal.Checked) continue;
                        string p = row.Def.ColumnPrefix;
                        SetSytIcdValue(row.UcPre, GetM2Str(d, p + M2_SUFFIX__PRE_CODE), GetM2Str(d, p + M2_SUFFIX__PRE_NAME));
                        SetSytIcdValue(row.UcFinal, GetM2Str(d, p + M2_SUFFIX__CODE), GetM2Str(d, p + M2_SUFFIX__NAME));
                    }
                }
                finally { m2Filling = false; }
                ApplyM2EnableRules();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Clears the M2 tabs ("Xoa trang" / other record opened).</summary>
        private void ClearM2Controls()
        {
            try
            {
                if (!isM2Inited) return;
                m2Filling = true;
                try { ClearM2ControlsCore(); }
                finally { m2Filling = false; }
                ApplyM2EnableRules();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void ClearM2ControlsCore()
        {
            foreach (CheckedComboBoxEdit c in new[] { cboM2PatientTypes, cboM2FamilyList, cboM2ObstetricAbnormal, cboM2PersonalList })
                SetM2CheckedIds(c, null);
            foreach (GridLookUpEdit c in new[] { cboM2ExamPlace, cboM2PaySource, cboM2PaySourceDetail, cboM2School, cboM2SchoolWard })
                c.EditValue = null;
            foreach (BaseEdit c in new BaseEdit[] { txtM2PayOther, txtM2Class, txtM2SchoolAddress, txtM2GuardianCccd,
                txtM2GuardianPhone, txtM2MotherDisease, txtM2AdhdResult, txtM2AdhdNote, txtM2AsdResult, txtM2AsdNote,
                txtM2AsdScore, txtM2TestUnit, txtM2TestResult, txtM2ImagingResult, txtM2SuggestOther,
                dteM2AdhdDate, dteM2AsdDate, dteM2TestDate })
                c.EditValue = null;
            foreach (RadioGroup r in new[] { rdoM2GoToSchool, rdoM2FamilyHas, rdoM2Obstetric, rdoM2PersonalHas, rdoM2Treating })
                SetM2Flag(r, null);
            foreach (RadioGroup r in new[] { rdoM2Spine, rdoM2Kyphosis, rdoM2Scoliosis, rdoM2ScoliosisLevel, rdoM2Suggest })
                SetM2RadioId(r, null);
            SetSytIcdValue(ucM2PersonalIcd, null, null);
            foreach (M2SectionRow row in m2SectionRows)
            {
                row.ChkNormal.Checked = false;
                SetSytIcdValue(row.UcPre, null, null);
                SetSytIcdValue(row.UcFinal, null, null);
            }
            foreach (LookUpEdit t in m2ToothEdits.Values) t.EditValue = null;
            foreach (KskM2VaccineRowADO r in m2VaccineRows)
            {
                r.Status = 0;
                r.InjectDate = null;
                if (r.IsOther) { r.Disease = "Các loại khác"; r.Schedule = null; r.Dose = null; }
            }
            foreach (KskM2QuestionRowADO r in m2AdhdRows) r.Level = 0;
            foreach (KskM2QuestionRowADO r in m2AsdRows) r.Level = 0;
            grvM2Vaccine.RefreshData();
            grvM2Adhd.RefreshData();
            grvM2Asd.RefreshData();
            m2AdhdAutoText = null;
            m2AsdAutoText = null;
        }

        private void FillM2Vaccines(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return;
                JObject o = JObject.Parse(json);
                foreach (KskM2VaccineRowADO r in m2VaccineRows)
                {
                    JObject e = o[r.Field] as JObject;
                    if (e == null) continue;
                    r.Status = e.Value<int?>("s") ?? 0;
                    r.InjectDate = ParseM2Ymd(e.Value<string>("d"));
                    if (r.IsOther)
                    {
                        r.Disease = e.Value<string>("ten") ?? r.Disease;
                        r.Schedule = e.Value<string>("lich");
                        r.Dose = e.Value<string>("mui");
                    }
                }
                grvM2Vaccine.RefreshData();
            }
            catch (Exception ex) { LogSystem.Warn("SytHcm/M2: VACCINATION_JSON khong doc duoc", ex); }
        }

        private static void FillM2Answers(List<KskM2QuestionRowADO> rows, string json)
        {
            try
            {
                if (rows == null || string.IsNullOrWhiteSpace(json)) return;
                JObject o = JObject.Parse(json);
                foreach (KskM2QuestionRowADO r in rows) r.Level = o.Value<int?>(r.Field) ?? 0;
            }
            catch (Exception ex) { LogSystem.Warn("SytHcm/M2: JSON cau tra loi khong doc duoc", ex); }
        }

        private static DateTime? ParseM2Ymd(string s)
        {
            DateTime d;
            if (!string.IsNullOrWhiteSpace(s)
                && DateTime.TryParseExact(s, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out d)) return d;
            return null;
        }

        private static bool IsM2IdList(string s)
        {
            return !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(s.Trim(), @"^\d+(\s*[,;]\s*\d+)*$");
        }

        #endregion

        #region ===== Validate (R17) =====

        /// <summary>Format checks shown at the control. Returns false = do not save.</summary>
        private bool ValidateM2BeforeSave()
        {
            try
            {
                if (!isM2Inited || xtraTabControl1.SelectedTabPageIndex != TAB_INDEX__UNDER_EIGHTEEN) return true;
                m2ErrorProvider.ClearErrors();
                List<string> messages = new List<string>();
                CheckM2Digits(txtM2GuardianCccd, 12, "CCCD người giám hộ phải đủ 12 chữ số.", messages);
                CheckM2Digits(txtM2GuardianPhone, 10, "Điện thoại người giám hộ phải đủ 10 chữ số.", messages);
                CheckM2Length(txtM2Class, 10, "Lớp tối đa 10 ký tự.", messages);
                CheckM2Length(txtM2AdhdResult, 200, "Kết quả B1 tối đa 200 ký tự.", messages);
                CheckM2Length(txtM2AdhdNote, 200, "Ghi chú B1 tối đa 200 ký tự.", messages);
                CheckM2Length(txtM2AsdResult, 200, "Kết quả B2 tối đa 200 ký tự.", messages);
                CheckM2Length(txtM2AsdNote, 200, "Ghi chú B2 tối đa 200 ký tự.", messages);
                CheckM2Length(txtM2SuggestOther, 500, "Đề nghị ghi rõ tối đa 500 ký tự.", messages);
                if (messages.Count == 0) return true;

                XtraMessageBox.Show("Mẫu 02 — vui lòng kiểm tra lại:\r\n\r\n- " + string.Join("\r\n- ", messages.ToArray()),
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return true;
            }
        }

        private void CheckM2Digits(BaseEdit e, int digits, string msg, List<string> messages)
        {
            string t = M2Text(e);
            if (t == null) return;
            if (t.Length == digits && t.All(char.IsDigit)) return;
            m2ErrorProvider.SetError(e, msg, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
            messages.Add(msg);
        }

        private void CheckM2Length(BaseEdit e, int max, string msg, List<string> messages)
        {
            string t = M2Text(e);
            if (t == null || t.Length <= max) return;
            m2ErrorProvider.SetError(e, msg, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
            messages.Add(msg);
        }

        #endregion

        #region ===== Save =====

        /// <summary>
        /// Writes Mau 02 values that are stored in the under-18 record itself (family history ids, D9).
        /// Called right after GetValueUnderEighteen when saving tab "Ksk duoi 18 tuoi".
        /// </summary>
        private void ApplyM2ToUnderEighteen(HIS_KSK_UNDER_EIGHTEEN obj)
        {
            try
            {
                if (!isM2Inited || obj == null) return;
                if (cboM2FamilyList.Properties.Items.Count == 0) return;   // catalog missing -> keep stored value
                short? has = GetM2Flag(rdoM2FamilyHas);
                string ids = GetM2CheckedIds(cboM2FamilyList);
                if (has == 0) obj.PATHOLOGICAL_HISTORY_FAMILY = null;
                else if (!string.IsNullOrEmpty(ids)) obj.PATHOLOGICAL_HISTORY_FAMILY = ids;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Saves the M2 record right after the under-18 record was saved.</summary>
        private void SaveKskSytHcmM2(HIS_KSK_UNDER_EIGHTEEN savedKsk)
        {
            try
            {
                if (!isM2Inited || !IsSytHcmDeclared()) return;
                if (xtraTabControl1.SelectedTabPageIndex != TAB_INDEX__UNDER_EIGHTEEN) return;
                if (savedKsk == null || savedKsk.ID <= 0) return;

                HIS_KSK_SYT_HCM_M2 data = BuildKskSytHcmM2FromControls(savedKsk.ID);
                if (data == null) return;

                bool isUpdate = currentKskSytHcmM2 != null && currentKskSytHcmM2.KSK_UNDER_EIGHTEEN_ID == savedKsk.ID;
                CommonParam param = new CommonParam();
                LogSystem.Debug("SytHcm/M2: " + (isUpdate ? "Update" : "Create") + " ban ghi M2 cho ho so KSK duoi 18 ID="
                    + savedKsk.ID);
                HIS_KSK_SYT_HCM_M2 rs = new BackendAdapter(param).Post<HIS_KSK_SYT_HCM_M2>(
                    isUpdate ? API__SYT_HCM_M2_UPDATE : API__SYT_HCM_M2_CREATE, ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    currentKskSytHcmM2 = rs;
                    return;
                }

                LogSystem.Error("SytHcm/M2: luu ban ghi M2 THAT BAI. "
                    + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                XtraMessageBox.Show("Hồ sơ khám sức khỏe đã lưu, nhưng CHƯA lưu được phần dữ liệu Mẫu 02 (Sở Y tế TP.HCM)."
                    + "\r\nVui lòng bấm Lưu lại.\r\n\r\n" + string.Join("\r\n", (param.Messages ?? new List<string>()).ToArray()),
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Collects the screen into a record. Starts from the stored record (R3).</summary>
        private HIS_KSK_SYT_HCM_M2 BuildKskSytHcmM2FromControls(long kskUnderEighteenId)
        {
            try
            {
                HIS_KSK_SYT_HCM_M2 d = new HIS_KSK_SYT_HCM_M2();
                if (currentKskSytHcmM2 != null && currentKskSytHcmM2.KSK_UNDER_EIGHTEEN_ID == kskUnderEighteenId)
                    CopyM2Values(currentKskSytHcmM2, d);
                d.KSK_UNDER_EIGHTEEN_ID = kskUnderEighteenId;

                d.SYT_PATIENT_TYPES = GetM2CheckedIds(cboM2PatientTypes);
                d.SYT_EXAM_PLACE_ID = GetM2LookupId(cboM2ExamPlace);
                d.SYT_PAYSOURCE_ID = GetM2LookupId(cboM2PaySource);
                d.SYT_PAY_SOURCE_DETAIL_ID = GetM2LookupId(cboM2PaySourceDetail);
                d.PAY_SOURCE_OTHER = M2Text(txtM2PayOther);

                d.IS_GO_TO_SCHOOL = GetM2Flag(rdoM2GoToSchool);
                d.SCHOOL_ID = GetM2LookupId(cboM2School);
                // Names stored with the ids so the form prints without the catalog (Q3).
                string schoolName = GetM2LookupName(cboM2School);
                d.SCHOOL_NAME = d.SCHOOL_ID.HasValue ? (schoolName ?? d.SCHOOL_NAME) : null;
                d.SCHOOL_ADDRESS = M2Text(txtM2SchoolAddress);
                d.SCHOOL_WARD_ID = GetM2LookupId(cboM2SchoolWard);
                string wardName = GetM2LookupName(cboM2SchoolWard);
                d.SCHOOL_WARD_NAME = d.SCHOOL_WARD_ID.HasValue ? (wardName ?? d.SCHOOL_WARD_NAME) : null;
                d.CLASS_NAME = M2Text(txtM2Class);
                d.GUARDIAN_CCCD = M2Text(txtM2GuardianCccd);
                d.GUARDIAN_PHONE = M2Text(txtM2GuardianPhone);

                d.FAMILY_HISTORY_HAS = GetM2Flag(rdoM2FamilyHas);
                d.OBSTETRIC_IS_NORMAL = GetM2Flag(rdoM2Obstetric);
                d.OBSTETRIC_ABNORMAL_IDS = GetM2CheckedIds(cboM2ObstetricAbnormal);
                d.MOTHER_DISEASE = M2Text(txtM2MotherDisease);
                d.PERSONAL_HISTORY_HAS = GetM2Flag(rdoM2PersonalHas);
                d.PERSONAL_HISTORY_IDS = GetM2CheckedIds(cboM2PersonalList);
                string personalIcd, personalIcdName;
                GetSytIcdValue(ucM2PersonalIcd, out personalIcd, out personalIcdName);
                if (sytIcdItems != null && sytIcdItems.Count > 0) d.PERSONAL_HISTORY_ICD = personalIcd;
                d.IS_TREATING = GetM2Flag(rdoM2Treating);

                d.VACCINATION_JSON = BuildM2VaccineJson();
                d.ADHD_DATE = M2DateToLong(dteM2AdhdDate);
                d.ADHD_JSON = BuildM2AnswerJson(m2AdhdRows);
                d.ADHD_RESULT = M2Text(txtM2AdhdResult);
                d.ADHD_NOTE = M2Text(txtM2AdhdNote);
                d.ASD_DATE = M2DateToLong(dteM2AsdDate);
                d.ASD_JSON = BuildM2AnswerJson(m2AsdRows);
                d.ASD_RESULT = M2Text(txtM2AsdResult);
                d.ASD_NOTE = M2Text(txtM2AsdNote);

                bool icdReady = sytIcdItems != null && sytIcdItems.Count > 0;
                foreach (M2SectionRow row in m2SectionRows)
                {
                    string p = row.Def.ColumnPrefix;
                    SetM2Value(d, p + M2_SUFFIX__IS_NORMAL, row.ChkNormal.Checked ? (short?)1 : null);
                    if (!icdReady) continue;   // ICD boxes cannot show stored ids yet -> keep stored values
                    string code, name;
                    GetSytIcdValue(row.UcPre, out code, out name);
                    SetM2Value(d, p + M2_SUFFIX__PRE_CODE, code);
                    SetM2Value(d, p + M2_SUFFIX__PRE_NAME, name);
                    GetSytIcdValue(row.UcFinal, out code, out name);
                    SetM2Value(d, p + M2_SUFFIX__CODE, code);
                    SetM2Value(d, p + M2_SUFFIX__NAME, name);
                }

                d.SPINE_ID = GetM2RadioId(rdoM2Spine);
                d.SPINE_KYPHOSIS_ID = GetM2RadioId(rdoM2Kyphosis);
                d.SPINE_SCOLIOSIS_ID = GetM2RadioId(rdoM2Scoliosis);
                d.SPINE_SCOLIOSIS_LEVEL_ID = GetM2RadioId(rdoM2ScoliosisLevel);

                foreach (KeyValuePair<string, LookUpEdit> t in m2ToothEdits)
                    SetM2Value(d, "TOOTH_" + t.Key, ToM2Long(t.Value.EditValue));

                d.TEST_DATE = M2DateToLong(dteM2TestDate);
                d.TEST_UNIT_NAME = M2Text(txtM2TestUnit);
                d.TEST_RESULT = M2Text(txtM2TestResult);
                d.IMAGING_RESULT = M2Text(txtM2ImagingResult);
                d.SYT_SUGGEST_ID = GetM2RadioId(rdoM2Suggest);
                d.SUGGEST_OTHER = M2Text(txtM2SuggestOther);
                return d;
            }
            catch (Exception ex) { LogSystem.Error(ex); return null; }
        }

        private string BuildM2VaccineJson()
        {
            grvM2Vaccine.PostEditor();
            JObject o = new JObject();
            foreach (KskM2VaccineRowADO r in m2VaccineRows)
            {
                bool hasOtherText = r.IsOther && (!string.IsNullOrWhiteSpace(r.Schedule) || !string.IsNullOrWhiteSpace(r.Dose)
                    || (r.Disease ?? "") != "Các loại khác");
                if (r.Status == 0 && !r.InjectDate.HasValue && !hasOtherText) continue;
                JObject e = new JObject();
                e["s"] = r.Status;
                if (r.InjectDate.HasValue) e["d"] = r.InjectDate.Value.ToString("yyyyMMdd");
                if (r.IsOther)
                {
                    e["ten"] = r.Disease;
                    e["lich"] = r.Schedule;
                    e["mui"] = r.Dose;
                }
                o[r.Field] = e;
            }
            return o.Count > 0 ? o.ToString(Newtonsoft.Json.Formatting.None) : null;
        }

        private static string BuildM2AnswerJson(List<KskM2QuestionRowADO> rows)
        {
            JObject o = new JObject();
            foreach (KskM2QuestionRowADO r in rows)
            {
                if (r.Level > 0) o[r.Field] = r.Level;
            }
            return o.Count > 0 ? o.ToString(Newtonsoft.Json.Formatting.None) : null;
        }

        #endregion

        #region ===== Reflection helpers (column prefix + suffix) =====

        private static void CopyM2Values(HIS_KSK_SYT_HCM_M2 from, HIS_KSK_SYT_HCM_M2 to)
        {
            foreach (PropertyInfo pi in typeof(HIS_KSK_SYT_HCM_M2).GetProperties())
            {
                if (!pi.CanRead || !pi.CanWrite) continue;
                Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                if (!t.IsPrimitive && t != typeof(string) && t != typeof(decimal)) continue;
                pi.SetValue(to, pi.GetValue(from, null), null);
            }
        }

        private static void SetM2Value(HIS_KSK_SYT_HCM_M2 d, string column, object value)
        {
            PropertyInfo pi = typeof(HIS_KSK_SYT_HCM_M2).GetProperty(column);
            if (pi == null)
            {
                LogSystem.Warn("SytHcm/M2: KHONG co cot " + column + " trong HIS_KSK_SYT_HCM_M2");
                return;
            }
            Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
            object v = (value == null) ? null : Convert.ChangeType(value, t);
            pi.SetValue(d, v, null);
        }

        private static object GetM2Value(HIS_KSK_SYT_HCM_M2 d, string column)
        {
            if (d == null) return null;
            PropertyInfo pi = typeof(HIS_KSK_SYT_HCM_M2).GetProperty(column);
            return pi != null ? pi.GetValue(d, null) : null;
        }

        private static string GetM2Str(HIS_KSK_SYT_HCM_M2 d, string column)
        {
            object v = GetM2Value(d, column);
            return v != null ? v.ToString() : null;
        }

        private static short? GetM2Short(HIS_KSK_SYT_HCM_M2 d, string column)
        {
            object v = GetM2Value(d, column);
            return v != null ? (short?)Convert.ToInt16(v) : null;
        }

        private static long? GetM2Long(HIS_KSK_SYT_HCM_M2 d, string column)
        {
            object v = GetM2Value(d, column);
            return v != null ? (long?)Convert.ToInt64(v) : null;
        }

        #endregion
    }
}
