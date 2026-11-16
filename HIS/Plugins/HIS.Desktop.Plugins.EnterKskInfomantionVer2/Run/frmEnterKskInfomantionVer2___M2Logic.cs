/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong61141 - Mau 02: catalog binding, value helpers, enable rules (R5, R7, R8, R12, R14, R15),
 * B1/B2 automatic results (R9-R11, R13) and grid handlers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Last text written automatically into the B1 / B2 result box (R13).</summary>
        private string m2AdhdAutoText;
        private string m2AsdAutoText;

        /// <summary>Text of the selected pay-source items considered as "other source" (opens the note box).</summary>
        private static readonly string[] M2_OTHER_KEYWORDS = new string[] { "khac" };

        #region ===== Catalog binding =====

        /// <summary>
        /// (Re)binds every catalog-bound control. Current values are read first and written back after
        /// binding, so calling it again when the catalogs arrive late never loses what is on screen.
        /// </summary>
        private void BindM2Catalogs()
        {
            try
            {
                if (cboM2PatientTypes == null) return;
                bool keepFilling = m2Filling;
                m2Filling = true;
                try
                {
                    BindM2CheckedCombo(cboM2PatientTypes, M2_CAT__DOI_TUONG);
                    BindM2CheckedCombo(cboM2FamilyList, M2_CAT__TS_GIA_DINH);
                    BindM2CheckedCombo(cboM2ObstetricAbnormal, M2_CAT__SAN_KHOA);
                    BindM2CheckedCombo(cboM2PersonalList, M2_CAT__TS_BAN_THAN);

                    BindM2Lookup(cboM2ExamPlace, M2_CAT__DIA_DIEM);
                    BindM2Lookup(cboM2PaySource, M2_CAT__HINH_THUC);
                    BindM2Lookup(cboM2PaySourceDetail, M2_CAT__HINH_THUC_CT);
                    BindM2Lookup(cboM2School, M2_CAT__TRUONG);
                    BindM2Lookup(cboM2SchoolWard, M2_CAT__XA_PHUONG);

                    BindM2Radio(rdoM2Spine, M2_CAT__COT_SONG);
                    BindM2Radio(rdoM2Kyphosis, M2_CAT__CONG_COT_SONG);
                    BindM2Radio(rdoM2Scoliosis, M2_CAT__VEO_COT_SONG);
                    BindM2Radio(rdoM2ScoliosisLevel, M2_CAT__MUC_DO_VEO);
                    BindM2Radio(rdoM2Suggest, M2_CAT__DE_NGHI);

                    List<KskCodeNameADO> teeth = ToCodeNameList(M2_CAT__TINH_TRANG_RANG);
                    if (teeth.Count > 0)
                    {
                        // Same symbol rule as the print (digits for permanent teeth, letters for primary teeth);
                        // a status name that matches no legend entry shows its full name instead of a blank cell.
                        List<KskM2ToothItemADO> permanent = BuildM2ToothItems(teeth, false);
                        List<KskM2ToothItemADO> primary = BuildM2ToothItems(teeth, true);
                        HashSet<string> primaryTeeth = new HashSet<string>(
                            KskM2Definition.TEETH_PRIMARY_UPPER.Concat(KskM2Definition.TEETH_PRIMARY_LOWER));
                        foreach (KeyValuePair<string, LookUpEdit> t in m2ToothEdits)
                        {
                            object keep = t.Value.EditValue;
                            t.Value.Properties.DataSource = primaryTeeth.Contains(t.Key) ? primary : permanent;
                            t.Value.EditValue = keep;
                        }
                    }

                    if (sytIcdItems != null && sytIcdItems.Count > 0)
                    {
                        RebindSytIcdSource(ucM2PersonalIcd);
                        foreach (M2SectionRow row in m2SectionRows)
                        {
                            RebindSytIcdSource(row.UcPre);
                            RebindSytIcdSource(row.UcFinal);
                        }
                    }
                }
                finally { m2Filling = keepFilling; }
                ApplyM2EnableRules();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static void BindM2Lookup(GridLookUpEdit cbo, string catalogCode)
        {
            if (cbo == null) return;
            List<KskCodeNameADO> data = ToCodeNameList(catalogCode);
            if (data.Count == 0) return;
            object keep = cbo.EditValue;
            cbo.Properties.DataSource = data;
            cbo.EditValue = keep;
        }

        private static void BindM2CheckedCombo(CheckedComboBoxEdit cbo, string catalogCode)
        {
            if (cbo == null) return;
            List<KskCodeNameADO> data = ToCodeNameList(catalogCode);
            if (data.Count == 0) return;
            string keep = GetM2CheckedIds(cbo);
            cbo.Properties.Items.Clear();
            foreach (KskCodeNameADO it in data)
                cbo.Properties.Items.Add(it.ID, it.NAME, CheckState.Unchecked, true);
            SetM2CheckedIds(cbo, keep);
        }

        private static void BindM2Radio(RadioGroup rdo, string catalogCode)
        {
            if (rdo == null) return;
            List<KskCodeNameADO> data = ToCodeNameList(catalogCode);
            if (data.Count == 0) return;
            long? keep = GetM2RadioId(rdo);
            rdo.Properties.Items.Clear();
            foreach (KskCodeNameADO it in data)
                rdo.Properties.Items.Add(new RadioGroupItem((long)it.ID, it.NAME));
            SetM2RadioId(rdo, keep);
        }

        #endregion

        #region ===== Value helpers =====

        /// <summary>
        /// Checked ids "1,2,3". When the catalog is not loaded yet the value waits in Tag, so a save
        /// in that state keeps the stored value instead of clearing it (R20).
        /// </summary>
        private static string GetM2CheckedIds(CheckedComboBoxEdit cbo)
        {
            if (cbo == null) return null;
            if (cbo.Properties.Items.Count == 0) return cbo.Tag as string;
            List<string> ids = new List<string>();
            foreach (CheckedListBoxItem it in cbo.Properties.Items)
            {
                if (it.CheckState == CheckState.Checked && it.Value != null) ids.Add(it.Value.ToString());
            }
            return ids.Count > 0 ? string.Join(",", ids.ToArray()) : null;
        }

        private static void SetM2CheckedIds(CheckedComboBoxEdit cbo, string ids)
        {
            if (cbo == null) return;
            cbo.Tag = ids;
            HashSet<string> set = new HashSet<string>(
                (ids ?? "").Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()));
            foreach (CheckedListBoxItem it in cbo.Properties.Items)
            {
                it.CheckState = (it.Value != null && set.Contains(it.Value.ToString()))
                    ? CheckState.Checked : CheckState.Unchecked;
            }
            cbo.RefreshEditValue();
        }

        private static long? GetM2RadioId(RadioGroup rdo)
        {
            if (rdo == null) return null;
            if (rdo.Properties.Items.Count == 0) return rdo.Tag as long?;
            return ToM2Long(rdo.EditValue);
        }

        private static void SetM2RadioId(RadioGroup rdo, long? id)
        {
            if (rdo == null) return;
            rdo.Tag = id;
            object match = null;
            if (id.HasValue)
            {
                foreach (RadioGroupItem it in rdo.Properties.Items)
                {
                    if (ToM2Long(it.Value) == id.Value) { match = it.Value; break; }
                }
            }
            rdo.EditValue = match;
        }

        private static short? GetM2Flag(RadioGroup rdo)
        {
            if (rdo == null || rdo.EditValue == null) return null;
            return Convert.ToInt16(rdo.EditValue);
        }

        private static void SetM2Flag(RadioGroup rdo, short? value)
        {
            if (rdo == null) return;
            rdo.EditValue = value.HasValue ? (object)value.Value : null;
        }

        private static long? GetM2LookupId(GridLookUpEdit cbo)
        {
            if (cbo == null) return null;
            return ToM2Long(cbo.EditValue);
        }

        private static void SetM2LookupId(GridLookUpEdit cbo, long? id)
        {
            if (cbo == null) return;
            cbo.EditValue = id.HasValue ? (object)(int)id.Value : null;
        }

        private static string GetM2LookupName(GridLookUpEdit cbo)
        {
            try
            {
                if (cbo == null || cbo.EditValue == null) return null;
                string t = cbo.Text;
                return string.IsNullOrWhiteSpace(t) ? null : t;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static long? ToM2Long(object v)
        {
            if (v == null) return null;
            long l;
            return long.TryParse(v.ToString(), out l) ? (long?)l : null;
        }

        private static string M2Text(BaseEdit e)
        {
            if (e == null || e.EditValue == null) return null;
            string t = e.Text;
            return string.IsNullOrWhiteSpace(t) ? null : t.Trim();
        }

        private static long? M2DateToLong(DateEdit d)
        {
            if (d == null || d.EditValue == null) return null;
            return long.Parse(d.DateTime.ToString("yyyyMMdd") + "000000");
        }

        private static void M2LongToDate(DateEdit d, long? v)
        {
            if (d == null) return;
            DateTime? dt = (v.HasValue && v.Value > 0)
                ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(v.Value) : null;
            d.EditValue = dt.HasValue ? (object)dt.Value : null;
        }

        #endregion

        #region ===== Enable rules =====

        private void M2_EnableRule_Changed(object sender, EventArgs e)
        {
            try { ApplyM2EnableRules(); }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>R5, R7, R8, R12, R14, R15 + "nguon khac".</summary>
        private void ApplyM2EnableRules()
        {
            try
            {
                if (cboM2PatientTypes == null) return;

                // Pay source "other" -> note box.
                bool payLoaded = cboM2PaySource.Properties.DataSource is System.Collections.ICollection
                    && ((System.Collections.ICollection)cboM2PaySource.Properties.DataSource).Count > 0;
                string payName = Norm2(GetM2LookupName(cboM2PaySource));
                bool payOther = M2_OTHER_KEYWORDS.Any(k => payName.Contains(k));
                if (payLoaded) SetM2Enabled(txtM2PayOther, payOther, !m2Filling);

                // R5: not going to school -> school fields locked and cleared.
                bool school = GetM2Flag(rdoM2GoToSchool) == 1;
                SetM2Enabled(cboM2School, school, !m2Filling);
                SetM2Enabled(txtM2Class, school, !m2Filling);
                SetM2Enabled(txtM2SchoolAddress, school, !m2Filling);
                SetM2Enabled(cboM2SchoolWard, school, !m2Filling);

                // R8 / R7.
                SetM2Enabled(cboM2FamilyList, GetM2Flag(rdoM2FamilyHas) == 1, !m2Filling);
                SetM2Enabled(cboM2ObstetricAbnormal, GetM2Flag(rdoM2Obstetric) == 0, !m2Filling);
                bool personal = GetM2Flag(rdoM2PersonalHas) == 1;
                SetM2Enabled(cboM2PersonalList, personal, !m2Filling);
                SetSytIcdEnabled(ucM2PersonalIcd, personal);
                if (!personal && !m2Filling) SetSytIcdValue(ucM2PersonalIcd, null, null);

                // R15: spine normal -> kyphosis / scoliosis locked; level only with a scoliosis type.
                bool spineLoaded = rdoM2Spine != null && rdoM2Spine.Properties.Items.Count > 0;
                string spineName = Norm2(RadioText(rdoM2Spine));
                bool spineAbnormal = spineName.Contains("bat thuong");
                if (spineLoaded)
                {
                    SetM2Enabled(rdoM2Kyphosis, spineAbnormal, !m2Filling);
                    SetM2Enabled(rdoM2Scoliosis, spineAbnormal, !m2Filling);
                    bool hasScoliosis = spineAbnormal && GetM2RadioId(rdoM2Scoliosis).HasValue;
                    SetM2Enabled(rdoM2ScoliosisLevel, hasScoliosis, !m2Filling);
                }

                // R14.
                foreach (M2SectionRow row in m2SectionRows)
                {
                    bool normal = row.ChkNormal != null && row.ChkNormal.Checked;
                    SetSytIcdEnabled(row.UcPre, !normal);
                    SetSytIcdEnabled(row.UcFinal, !normal);
                }

                ApplyM2AsdAgeRule();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Lock (and optionally clear) one control.</summary>
        private static void SetM2Enabled(Control c, bool enabled, bool clearWhenLocked)
        {
            if (c == null) return;
            c.Enabled = enabled;
            if (enabled || !clearWhenLocked) return;
            if (c is CheckedComboBoxEdit) SetM2CheckedIds((CheckedComboBoxEdit)c, null);
            else if (c is RadioGroup) SetM2RadioId((RadioGroup)c, null);
            else if (c is BaseEdit) ((BaseEdit)c).EditValue = null;
        }

        private static string RadioText(RadioGroup rdo)
        {
            if (rdo == null || rdo.EditValue == null) return null;
            foreach (RadioGroupItem it in rdo.Properties.Items)
            {
                if (object.Equals(it.Value, rdo.EditValue)) return it.Description;
            }
            return null;
        }

        /// <summary>Lower-case, no accents — used to recognise catalog items by name.</summary>
        private static string Norm2(string s)
        {
            return NormToothName(s);
        }

        /// <summary>R12: B2 only for children 6..11 years old at the exam date.</summary>
        private void ApplyM2AsdAgeRule()
        {
            try
            {
                if (grdM2Asd == null) return;
                int? age = GetM2ChildAge();
                bool allowed = !age.HasValue
                    || (age.Value >= KskM2Definition.ASD_MIN_AGE && age.Value <= KskM2Definition.ASD_MAX_AGE);
                grdM2Asd.Enabled = allowed;
                dteM2AsdDate.Enabled = allowed;
                txtM2AsdResult.Enabled = allowed;
                txtM2AsdNote.Enabled = allowed;
                if (lblM2AsdAgeNote != null)
                {
                    lblM2AsdAgeNote.Text = allowed ? "" : "Trẻ " + age.Value
                        + " tuổi tại ngày khám — không đánh giá B2 (chỉ áp dụng trẻ từ 6 đến 11 tuổi), không gửi khối B2 lên cổng.";
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Age in full years at the exam (instruction) date; null if unknown.</summary>
        private int? GetM2ChildAge()
        {
            try
            {
                if (currentServiceReq == null || currentServiceReq.TDL_PATIENT_DOB <= 0) return null;
                DateTime? dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentServiceReq.TDL_PATIENT_DOB);
                DateTime? at = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentServiceReq.INTRUCTION_TIME);
                if (!dob.HasValue) return null;
                return KskM2Definition.AgeAt(dob.Value, at ?? DateTime.Now);
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        #endregion

        #region ===== Handlers =====

        private void ChkM2Normal_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit chk = sender as CheckEdit;
                M2SectionRow row = chk != null ? chk.Tag as M2SectionRow : null;
                if (row == null) return;
                if (chk.Checked && !m2Filling)
                {
                    SetSytIcdValue(row.UcPre, null, null);
                    SetSytIcdValue(row.UcFinal, null, null);
                }
                SetSytIcdEnabled(row.UcPre, !chk.Checked);
                SetSytIcdEnabled(row.UcFinal, !chk.Checked);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private bool m2MedicineSyncing = false;

        /// <summary>The medicine box of tab M2 and of the old tab are the same column -> kept in sync.</summary>
        private void TxtM2TreatingMedicine_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (m2MedicineSyncing || this.txtMedicineUsing3 == null) return;
                m2MedicineSyncing = true;
                try { this.txtMedicineUsing3.Text = txtM2TreatingMedicine.Text; }
                finally { m2MedicineSyncing = false; }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void TxtMedicineUsing3_M2Mirror(object sender, EventArgs e)
        {
            try
            {
                if (m2MedicineSyncing || txtM2TreatingMedicine == null) return;
                m2MedicineSyncing = true;
                try { txtM2TreatingMedicine.Text = this.txtMedicineUsing3.Text; }
                finally { m2MedicineSyncing = false; }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void GrvM2Vaccine_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                GridView v = sender as GridView;
                KskM2VaccineRowADO row = v != null ? v.GetFocusedRow() as KskM2VaccineRowADO : null;
                if (row == null || v.FocusedColumn == null) return;
                string f = v.FocusedColumn.FieldName;
                // Only the "other" row has editable name / schedule / dose.
                if ((f == "Disease" || f == "Schedule" || f == "Dose") && !row.IsOther) e.Cancel = true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>R16: one status per dose; the date may not be after the exam date.</summary>
        private void GrvM2Vaccine_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                GridView v = sender as GridView;
                KskM2VaccineRowADO row = v != null ? v.GetRow(e.RowHandle) as KskM2VaccineRowADO : null;
                if (row == null) return;
                bool on = e.Value is bool && (bool)e.Value;
                if (on && e.Column.FieldName == "Done") { row.NotDone = false; row.Unknown = false; }
                else if (on && e.Column.FieldName == "NotDone") { row.Done = false; row.Unknown = false; }
                else if (on && e.Column.FieldName == "Unknown") { row.Done = false; row.NotDone = false; }
                else if (e.Column.FieldName == "InjectDate" && row.InjectDate.HasValue)
                {
                    DateTime? exam = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        currentServiceReq != null ? currentServiceReq.INTRUCTION_TIME : 0);
                    if (exam.HasValue && row.InjectDate.Value.Date > exam.Value.Date)
                    {
                        XtraMessageBox.Show("Ngày tiêm/uống không được lớn hơn ngày khám.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        row.InjectDate = null;
                    }
                }
                v.RefreshRow(e.RowHandle);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void GrvM2Question_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                GridView v = sender as GridView;
                KskM2QuestionRowADO row = v != null ? v.GetRow(e.RowHandle) as KskM2QuestionRowADO : null;
                if (row == null) return;
                if (e.Value is bool && (bool)e.Value)
                {
                    string f = e.Column.FieldName;
                    row.C1 = f == "C1"; row.C2 = f == "C2"; row.C3 = f == "C3"; row.C4 = f == "C4";
                }
                v.RefreshRow(e.RowHandle);
                if (!m2Filling) RecalcM2MentalResults();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>R9-R11 + R13: suggest results; never overwrite text typed by the user.</summary>
        private void RecalcM2MentalResults()
        {
            try
            {
                Dictionary<string, int> adhd = ReadM2Answers(m2AdhdRows);
                string sAdhd = KskM2Definition.SuggestAdhdResult(adhd);
                m2AdhdAutoText = ApplyM2AutoText(txtM2AdhdResult, sAdhd, m2AdhdAutoText);

                Dictionary<string, int> asd = ReadM2Answers(m2AsdRows);
                txtM2AsdScore.Text = asd.Count > 0 ? KskM2Definition.ScoreAsd(asd).ToString() : "";
                string sAsd = KskM2Definition.SuggestAsdResult(asd);
                m2AsdAutoText = ApplyM2AutoText(txtM2AsdResult, sAsd, m2AsdAutoText);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static string ApplyM2AutoText(MemoEdit box, string suggestion, string lastAuto)
        {
            if (box == null) return lastAuto;
            string cur = box.Text ?? "";
            bool untouched = cur.Trim().Length == 0 || (lastAuto != null && cur == lastAuto);
            if (!untouched) return lastAuto;
            string val = suggestion ?? "";
            if (val.Length > 200) val = val.Substring(0, 200);
            box.Text = val;
            return val;
        }

        private static Dictionary<string, int> ReadM2Answers(List<KskM2QuestionRowADO> rows)
        {
            Dictionary<string, int> rs = new Dictionary<string, int>();
            if (rows == null) return rs;
            foreach (KskM2QuestionRowADO r in rows)
            {
                if (r.Level > 0) rs[r.Field] = r.Level;
            }
            return rs;
        }

        private void BtnM2VaccineAllDone_Click(object sender, EventArgs e)
        {
            try
            {
                if (m2VaccineRows == null) return;
                foreach (KskM2VaccineRowADO r in m2VaccineRows)
                {
                    if (r.IsOther) continue;
                    r.Status = (int)KskM2VaccineStatus.Done;
                }
                grvM2Vaccine.RefreshData();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void BtnM2TeethAllNormal_Click(object sender, EventArgs e)
        {
            try
            {
                List<KskCodeNameADO> data = ToCodeNameList(M2_CAT__TINH_TRANG_RANG);
                KskCodeNameADO normal = data.FirstOrDefault(o => Norm2(o.NAME).Contains("binh thuong"));
                if (normal == null)
                {
                    XtraMessageBox.Show("Chưa tải được danh mục tình trạng răng của Sở Y tế TP.HCM.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                foreach (LookUpEdit cbo in m2ToothEdits.Values) cbo.EditValue = normal.ID;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void BtnM2TeethClear_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (LookUpEdit cbo in m2ToothEdits.Values) cbo.EditValue = null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Tooth statuses of the portal catalog with their legend symbol (same rule as the print).</summary>
        private static List<KskM2ToothItemADO> BuildM2ToothItems(List<KskCodeNameADO> catalog, bool primaryTooth)
        {
            List<KskM2ToothItemADO> result = new List<KskM2ToothItemADO>();
            foreach (KskCodeNameADO o in catalog)
            {
                string sym = M2ToothSymbol(o.NAME, primaryTooth);
                result.Add(new KskM2ToothItemADO
                {
                    ID = o.ID,
                    NAME = o.NAME,
                    SYMBOL = string.IsNullOrEmpty(sym) ? o.NAME : sym
                });
            }
            return result;
        }

        #endregion
    }
}
