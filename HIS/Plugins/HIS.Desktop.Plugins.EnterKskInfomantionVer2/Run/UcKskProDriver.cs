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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// Một dòng bảng tiền sử bản thân 22 bệnh, tật của tab Mẫu 5.
    /// </summary>
    internal class ProDriverDityRow
    {
        public long ID { get; set; }
        public long DITY_ID { get; set; }
        public string DISEASE_TYPE_CODE { get; set; }
        public string DISEASE_TYPE_NAME { get; set; }
        public bool IS_YES { get; set; }
        public bool IS_NO { get; set; }
    }

    /// <summary>
    /// Giao diện tab "Ksk lái xe (Mẫu 5)" — Giấy khám sức khỏe của người lái xe, Mẫu 5 TT 36/2024/TT-BYT.
    /// Lưu vào HIS_KSK_PRO_DRIVER (+ HIS_DHST cho thể lực). Bố cục 3 cột, theo thứ tự mẫu:
    ///   trái : thông tin đề nghị khám + hành chính + tiền sử bệnh + khám thể lực
    ///   giữa : II. khám lâm sàng 8 chuyên khoa (mỗi khoa: chưa phát hiện bất thường / ICD sơ bộ / ICD xác định / kết luận / bác sĩ)
    ///   phải : III. cận lâm sàng + IV. bệnh tật, đề nghị + V. kết luận
    /// Bố cục dựng bằng code (BuildLayout) vì 8 chuyên khoa cùng một khuôn; dữ liệu gán theo TÊN CỘT
    /// (Set/Get) để 8 khoa dùng chung một đoạn, khớp quy luật đặt tên của bảng.
    /// </summary>
    public partial class UcKskProDriver : XtraUserControl
    {
        /// <summary>Một chuyên khoa: tiền tố cột HIS_KSK_PRO_DRIVER + các ô của khoa.</summary>
        private class Section
        {
            public string Column;
            public CheckEdit ChkNormal;
            public UcKskHistoryIcd PreIcd;
            public UcKskHistoryIcd FinalIcd;
            public TextEdit TxtConclude;
            public GridLookUpEdit CboDoctor;
        }

        private readonly List<Section> sections = new List<Section>();
        private readonly List<UcKskHistoryIcd> icdEditors = new List<UcKskHistoryIcd>();
        private readonly List<GridLookUpEdit> doctorCombos = new List<GridLookUpEdit>();
        private bool isInited = false;

        // ----- Thông tin đề nghị + hành chính -----
        private GridLookUpEdit cboLicenseClass, cboObject, cboPaymentSource;
        private RadioGroup rdoLicenseIssue;
        private TextEdit txtCccdDate, txtCccdPlace, txtEthnic, txtResidence, txtPhone;
        // ----- Tiền sử -----
        private RadioGroup rdoFamily, rdoTreating, rdoMaternity;
        private TextEdit txtFamily, txtOtherDisease;
        private MemoEdit memTreating, memMaternity;
        private GridControl gridDity;
        private GridView gridViewDity;
        // ----- Thể lực -----
        private SpinEdit spnHeight, spnWeight, spnBmi, spnPulse, spnBloodMax, spnBloodMin, spnBreath;
        // ----- Mắt / Tai -----
        private TextEdit txtEyeR, txtEyeL, txtPinholeR, txtPinholeL, txtGlassR, txtGlassL, txtTwoEye, txtTwoEyeGlass;
        private CheckEdit chkFieldHoriNormal, chkFieldHoriLimit, chkFieldVertNormal, chkFieldVertLimit;
        private CheckEdit chkColorNormal, chkColorBlind, chkColorRed, chkColorGreen, chkColorYellow;
        private TextEdit txtEntLeftNormal, txtEntLeftWhisper, txtEntRightNormal, txtEntRightWhisper;
        // ----- Cận lâm sàng -----
        private readonly Dictionary<string, TextEdit> clsEditors = new Dictionary<string, TextEdit>();
        private MemoEdit memDiim, memOtherTest;
        private GridLookUpEdit cboDrugDoctor, cboBloodDoctor, cboUrineDoctor, cboDiimDoctor, cboOtherDoctor;
        // ----- IV, V -----
        private CheckEdit chkConclusionNormal;
        private UcKskHistoryIcd icdConclusionPre, icdConclusionFinal;
        private RadioGroup rdoSuggest, rdoDriverConclusion;
        private TextEdit txtSuggestNote, txtReexamNote;
        private GridLookUpEdit cboConcluder;
        private DateEdit dteConclusionTime;

        public UcKskProDriver()
        {
            InitializeComponent();
            try
            {
                BuildLayout();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        #region Thuộc tính cho form

        public GridLookUpEdit CboLicenseClass { get { return this.cboLicenseClass; } }
        public GridLookUpEdit CboObject { get { return this.cboObject; } }
        public GridLookUpEdit CboPaymentSource { get { return this.cboPaymentSource; } }
        public GridLookUpEdit CboConcluder { get { return this.cboConcluder; } }
        public DateEdit DteConclusionTime { get { return this.dteConclusionTime; } }
        public SpinEdit SpnBloodPressureMax { get { return this.spnBloodMax; } }
        public SpinEdit SpnBloodPressureMin { get { return this.spnBloodMin; } }
        /// <summary>Các combo bác sĩ khám từng khoa / bác sĩ cận lâm sàng — form nạp danh sách nhân viên.</summary>
        public List<GridLookUpEdit> DoctorCombos { get { return this.doctorCombos; } }

        #endregion

        #region Dựng bố cục

        private void BuildLayout()
        {
            this.layoutControl1.BeginUpdate();
            try
            {
                LayoutControlGroup root = this.layoutControlGroupRoot;
                LayoutControlGroup col1 = NewColumn(root, null);
                LayoutControlGroup col2 = NewColumn(root, col1);
                LayoutControlGroup col3 = NewColumn(root, col2);
                BuildColumnLeft(col1);
                BuildColumnMiddle(col2);
                BuildColumnRight(col3);
            }
            finally
            {
                this.layoutControl1.EndUpdate();
            }
        }

        /// <summary>Cột của bố cục: cột đầu thêm vào root, cột sau chèn NGAY BÊN PHẢI cột trước (cùng hàng).</summary>
        private LayoutControlGroup NewColumn(LayoutControlGroup root, LayoutControlGroup leftOf)
        {
            LayoutControlGroup g = new LayoutControlGroup();
            g.GroupBordersVisible = false;
            g.TextVisible = false;
            if (leftOf == null) root.AddGroup(g);
            else root.AddGroup(g, leftOf, InsertType.Right);
            return g;
        }

        private static LayoutControlGroup Section_(LayoutControlGroup col, string title)
        {
            LayoutControlGroup g = col.AddGroup(title);
            g.ExpandButtonVisible = false;
            return g;
        }

        /// <summary>Thêm 1 ô vào nhóm; <paramref name="rightOf"/> != null thì đặt cùng hàng bên phải ô đó.</summary>
        private static LayoutControlItem Add(LayoutControlGroup g, string text, Control c, int textWidth = 120,
            LayoutControlItem rightOf = null, int height = 0)
        {
            LayoutControlItem it = rightOf == null ? g.AddItem(text ?? "", c) : g.AddItem(text ?? "", c, rightOf, InsertType.Right);
            if (string.IsNullOrEmpty(text))
            {
                it.TextVisible = false;
            }
            else
            {
                it.TextAlignMode = TextAlignModeItem.CustomSize;
                it.TextSize = new Size(textWidth, 20);
                it.TextToControlDistance = 4;
            }
            if (height > 0)
            {
                it.SizeConstraintsType = SizeConstraintsType.Custom;
                it.MinSize = new Size(60, height);
                it.MaxSize = new Size(0, height);
            }
            return it;
        }

        private static TextEdit Txt(string name)
        {
            return new TextEdit { Name = name };
        }

        private static TextEdit TxtReadOnly(string name)
        {
            TextEdit t = new TextEdit { Name = name };
            t.Properties.ReadOnly = true;
            return t;
        }

        private static MemoEdit Mem(string name)
        {
            return new MemoEdit { Name = name };
        }

        private static CheckEdit Chk(string name, string text)
        {
            CheckEdit c = new CheckEdit { Name = name };
            c.Properties.Caption = text;
            return c;
        }

        private static SpinEdit Spn(string name, bool isFloat)
        {
            SpinEdit s = new SpinEdit { Name = name };
            s.Properties.IsFloatValue = isFloat;
            s.Properties.Increment = isFloat ? 0.1m : 1m;
            s.Properties.MinValue = 0;
            s.Properties.MaxValue = 999;
            s.Properties.EditMask = isFloat ? "n1" : "n0";
            s.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            s.EditValue = null;
            return s;
        }

        private static RadioGroup Rdo(string name, params string[] items)
        {
            RadioGroup r = new RadioGroup { Name = name };
            for (int i = 0; i < items.Length; i++)
            {
                // Mục đầu "Không" = 0 khi có 2 mục Không/Có; danh sách đánh số 1..n khi > 2 mục.
                int value = items.Length == 2 ? i : i + 1;
                r.Properties.Items.Add(new RadioGroupItem(value, items[i]));
            }
            r.Properties.Columns = items.Length <= 3 ? items.Length : 1;
            return r;
        }

        private GridLookUpEdit Cbo(string name, bool isDoctor)
        {
            GridLookUpEdit c = new GridLookUpEdit { Name = name };
            c.Properties.NullText = "";
            if (isDoctor) this.doctorCombos.Add(c);
            return c;
        }

        private UcKskHistoryIcd Icd(string name)
        {
            UcKskHistoryIcd u = new UcKskHistoryIcd { Name = name };
            this.icdEditors.Add(u);
            return u;
        }

        private void BuildColumnLeft(LayoutControlGroup col)
        {
            // Thông tin đề nghị khám
            LayoutControlGroup g = Section_(col, "THÔNG TIN ĐỀ NGHỊ KHÁM");
            this.cboLicenseClass = Cbo("cboLicenseClass", false);
            Add(g, "Hạng GPLX đề nghị:", this.cboLicenseClass);
            this.rdoLicenseIssue = Rdo("rdoLicenseIssue", "Cấp mới", "Cấp đổi", "Cấp lại");
            Add(g, "Hình thức cấp:", this.rdoLicenseIssue, height: 26);
            this.cboObject = Cbo("cboObject", false);
            Add(g, "Đối tượng:", this.cboObject);
            this.cboPaymentSource = Cbo("cboPaymentSource", false);
            Add(g, "Nguồn chi trả:", this.cboPaymentSource);

            // Hành chính (chỉ đọc — lấy từ hồ sơ bệnh nhân)
            g = Section_(col, "THÔNG TIN HÀNH CHÍNH");
            this.txtCccdDate = TxtReadOnly("txtCccdDate");
            LayoutControlItem it = Add(g, "Ngày cấp CCCD:", this.txtCccdDate);
            this.txtEthnic = TxtReadOnly("txtEthnic");
            Add(g, "Dân tộc:", this.txtEthnic, 60, it);
            this.txtCccdPlace = TxtReadOnly("txtCccdPlace");
            Add(g, "Nơi cấp:", this.txtCccdPlace);
            this.txtResidence = TxtReadOnly("txtResidence");
            Add(g, "Nơi ở hiện tại:", this.txtResidence);
            this.txtPhone = TxtReadOnly("txtPhone");
            Add(g, "Điện thoại:", this.txtPhone);

            // Tiền sử bệnh
            g = Section_(col, "TIỀN SỬ BỆNH");
            this.rdoFamily = Rdo("rdoFamily", "Không", "Có");
            Add(g, "1. Tiền sử gia đình:", this.rdoFamily, height: 26);
            this.txtFamily = Txt("txtFamily");
            Add(g, "Tên bệnh:", this.txtFamily);
            BuildDityGrid();
            Add(g, null, this.gridDity, height: 530);
            this.txtOtherDisease = Txt("txtOtherDisease");
            Add(g, "Bệnh khác (ghi rõ):", this.txtOtherDisease);
            this.rdoTreating = Rdo("rdoTreating", "Không", "Có");
            Add(g, "3a. Đang điều trị bệnh:", this.rdoTreating, height: 26);
            this.memTreating = Mem("memTreating");
            Add(g, "Thuốc, liều lượng:", this.memTreating, height: 44);
            this.rdoMaternity = Rdo("rdoMaternity", "Không", "Có");
            Add(g, "3b. Tiền sử thai sản:", this.rdoMaternity, height: 26);
            this.memMaternity = Mem("memMaternity");
            Add(g, "Thuốc, liều lượng:", this.memMaternity, height: 44);

            // I. Khám thể lực
            g = Section_(col, "I. KHÁM THỂ LỰC");
            this.spnHeight = Spn("spnHeight", true);
            it = Add(g, "Chiều cao (cm):", this.spnHeight);
            this.spnWeight = Spn("spnWeight", true);
            Add(g, "Cân nặng (kg):", this.spnWeight, 90, it);
            this.spnBmi = Spn("spnBmi", true);
            this.spnBmi.Properties.ReadOnly = true;
            this.spnBmi.Properties.EditMask = "n2";
            it = Add(g, "Chỉ số BMI:", this.spnBmi);
            this.spnBreath = Spn("spnBreath", false);
            Add(g, "Nhịp thở (l/p):", this.spnBreath, 90, it);
            this.spnPulse = Spn("spnPulse", false);
            it = Add(g, "Mạch (l/p):", this.spnPulse);
            this.spnBloodMax = Spn("spnBloodMax", false);
            it = Add(g, "Huyết áp:", this.spnBloodMax, 90, it);
            this.spnBloodMin = Spn("spnBloodMin", false);
            Add(g, "/", this.spnBloodMin, 8, it);
            this.spnHeight.EditValueChanged += (s, e) => RefreshBmi();
            this.spnWeight.EditValueChanged += (s, e) => RefreshBmi();

            col.AddItem(new EmptySpaceItem());
        }

        private void BuildDityGrid()
        {
            this.gridDity = new GridControl { Name = "gridDity" };
            this.gridViewDity = new GridView(this.gridDity);
            this.gridDity.MainView = this.gridViewDity;
            this.gridDity.ViewCollection.Add(this.gridViewDity);
            this.gridViewDity.OptionsView.ShowGroupPanel = false;
            this.gridViewDity.OptionsView.ShowIndicator = false;
            this.gridViewDity.OptionsCustomization.AllowSort = false;
            this.gridViewDity.OptionsCustomization.AllowFilter = false;
            this.gridViewDity.OptionsView.RowAutoHeight = true;

            RepositoryItemCheckEdit chk = new RepositoryItemCheckEdit();
            // Đẩy giá trị vào dòng ngay khi bấm, để CellValueChanged bỏ tích cột còn lại.
            chk.EditValueChanged += (s, e) => this.gridViewDity.PostEditor();
            RepositoryItemMemoEdit wrap = new RepositoryItemMemoEdit();
            this.gridDity.RepositoryItems.Add(chk);
            this.gridDity.RepositoryItems.Add(wrap);

            GridColumn colStt = new GridColumn { Caption = "TT", FieldName = "STT", UnboundType = DevExpress.Data.UnboundColumnType.Integer, Visible = true, VisibleIndex = 0, Width = 30 };
            colStt.OptionsColumn.AllowEdit = false;
            GridColumn colName = new GridColumn { Caption = "2. Tiền sử bệnh, tật của bản thân", FieldName = "DISEASE_TYPE_NAME", Visible = true, VisibleIndex = 1, Width = 260, ColumnEdit = wrap };
            colName.OptionsColumn.AllowEdit = false;
            GridColumn colYes = new GridColumn { Caption = "Có", FieldName = "IS_YES", Visible = true, VisibleIndex = 2, Width = 40, ColumnEdit = chk };
            GridColumn colNo = new GridColumn { Caption = "Không", FieldName = "IS_NO", Visible = true, VisibleIndex = 3, Width = 50, ColumnEdit = chk };
            this.gridViewDity.Columns.AddRange(new[] { colStt, colName, colYes, colNo });
            this.gridViewDity.CustomUnboundColumnData += (s, e) =>
            {
                if (e.IsGetData && e.Column.FieldName == "STT") e.Value = e.ListSourceRowIndex + 1;
            };
            this.gridViewDity.CellValueChanged += (s, e) =>
            {
                try
                {
                    ProDriverDityRow row = this.gridViewDity.GetRow(e.RowHandle) as ProDriverDityRow;
                    if (row == null || !(e.Value is bool) || !(bool)e.Value) return;
                    if (e.Column.FieldName == "IS_YES") row.IS_NO = false;
                    else if (e.Column.FieldName == "IS_NO") row.IS_YES = false;
                    this.gridViewDity.RefreshRow(e.RowHandle);
                }
                catch (Exception ex) { LogSystem.Warn(ex); }
            };
        }

        private void BuildColumnMiddle(LayoutControlGroup col)
        {
            BuildSection(col, "1. Tâm thần", "EXAM_MENTAL", null);
            BuildSection(col, "2. Thần kinh", "EXAM_NEUROLOGICAL", null);
            BuildSection(col, "3. Mắt", "EXAM_EYE", BuildEyeExtra);
            BuildSection(col, "4. Tai - Mũi - Họng", "EXAM_ENT", BuildEntExtra);
            BuildSection(col, "5. Tim mạch", "EXAM_CIRCULATION", null);
            BuildSection(col, "6. Hô hấp", "EXAM_RESPIRATORY", null);
            BuildSection(col, "7. Cơ xương khớp", "EXAM_MUSCLE_BONE", null);
            BuildSection(col, "8. Nội tiết", "EXAM_OEND", null);
            col.AddItem(new EmptySpaceItem());
        }

        /// <summary>Một chuyên khoa theo Mẫu 5: [ô riêng của khoa] + 3 lựa chọn kết luận ICD + kết luận + bác sĩ.</summary>
        private void BuildSection(LayoutControlGroup col, string title, string column, Action<LayoutControlGroup> extra)
        {
            LayoutControlGroup g = Section_(col, "II. " + title);
            if (extra != null) extra(g);
            Section s = new Section { Column = column };
            string key = column.Replace("EXAM_", "").ToLower();
            s.ChkNormal = Chk("chkNormal_" + key, "Chưa phát hiện bất thường");
            Add(g, null, s.ChkNormal, height: 22);
            s.PreIcd = Icd("icdPre_" + key);
            Add(g, "Chẩn đoán sơ bộ (ICD):", s.PreIcd, 140, height: 24);
            s.FinalIcd = Icd("icdFinal_" + key);
            Add(g, "Chẩn đoán xác định (ICD):", s.FinalIcd, 140, height: 24);
            s.TxtConclude = Txt("txtConclude_" + key);
            Add(g, "Kết luận:", s.TxtConclude, 140);
            s.CboDoctor = Cbo("cboDoctor_" + key, true);
            Add(g, "Bác sĩ:", s.CboDoctor, 140);
            // "Chưa phát hiện bất thường" loại trừ chẩn đoán: tích thì bỏ ICD đã chọn.
            s.ChkNormal.CheckedChanged += (o, e) => ApplyNormalState(s.ChkNormal, s.PreIcd, s.FinalIcd);
            this.sections.Add(s);
        }

        private void BuildEyeExtra(LayoutControlGroup g)
        {
            this.txtEyeR = Txt("txtEyeR"); this.txtEyeL = Txt("txtEyeL");
            LayoutControlItem it = Add(g, "Không kính MP:", this.txtEyeR, 100);
            Add(g, "MT:", this.txtEyeL, 30, it);
            this.txtPinholeR = Txt("txtPinholeR"); this.txtPinholeL = Txt("txtPinholeL");
            it = Add(g, "Kính lỗ MP:", this.txtPinholeR, 100);
            Add(g, "MT:", this.txtPinholeL, 30, it);
            this.txtGlassR = Txt("txtGlassR"); this.txtGlassL = Txt("txtGlassL");
            it = Add(g, "Có kính MP:", this.txtGlassR, 100);
            Add(g, "MT:", this.txtGlassL, 30, it);
            this.txtTwoEye = Txt("txtTwoEye"); this.txtTwoEyeGlass = Txt("txtTwoEyeGlass");
            it = Add(g, "Hai mắt không kính:", this.txtTwoEye, 120);
            Add(g, "Có kính:", this.txtTwoEyeGlass, 50, it);
            this.chkFieldHoriNormal = Chk("chkFieldHoriNormal", "Bình thường");
            this.chkFieldHoriLimit = Chk("chkFieldHoriLimit", "Hạn chế");
            it = Add(g, "Thị trường ngang:", this.chkFieldHoriNormal, 120, height: 22);
            Add(g, null, this.chkFieldHoriLimit, rightOf: it, height: 22);
            this.chkFieldVertNormal = Chk("chkFieldVertNormal", "Bình thường");
            this.chkFieldVertLimit = Chk("chkFieldVertLimit", "Hạn chế");
            it = Add(g, "Thị trường đứng:", this.chkFieldVertNormal, 120, height: 22);
            Add(g, null, this.chkFieldVertLimit, rightOf: it, height: 22);
            this.chkColorNormal = Chk("chkColorNormal", "Bình thường");
            this.chkColorBlind = Chk("chkColorBlind", "Mù màu toàn bộ");
            it = Add(g, "Sắc giác:", this.chkColorNormal, 120, height: 22);
            Add(g, null, this.chkColorBlind, rightOf: it, height: 22);
            this.chkColorRed = Chk("chkColorRed", "Đỏ");
            this.chkColorGreen = Chk("chkColorGreen", "Xanh lá cây");
            this.chkColorYellow = Chk("chkColorYellow", "Vàng");
            it = Add(g, "Mù màu:", this.chkColorRed, 120, height: 22);
            it = Add(g, null, this.chkColorGreen, rightOf: it, height: 22);
            Add(g, null, this.chkColorYellow, rightOf: it, height: 22);
        }

        private void BuildEntExtra(LayoutControlGroup g)
        {
            this.txtEntLeftNormal = Txt("txtEntLeftNormal"); this.txtEntLeftWhisper = Txt("txtEntLeftWhisper");
            LayoutControlItem it = Add(g, "Tai trái nói thường (m):", this.txtEntLeftNormal, 140);
            Add(g, "Nói thầm (m):", this.txtEntLeftWhisper, 80, it);
            this.txtEntRightNormal = Txt("txtEntRightNormal"); this.txtEntRightWhisper = Txt("txtEntRightWhisper");
            it = Add(g, "Tai phải nói thường (m):", this.txtEntRightNormal, 140);
            Add(g, "Nói thầm (m):", this.txtEntRightWhisper, 80, it);
        }

        /// <summary>Ô cận lâm sàng -> cột HIS_KSK_PRO_DRIVER (thứ tự theo Mẫu 5).</summary>
        private static readonly string[][] DRUG_FIELDS = {
            new[] { "TEST_AMPHETAMIN", "Amphetamin:" }, new[] { "TEST_MARIJUANA", "Marijuana:" }, new[] { "TEST_MORPHIN", "Morphin:" },
            new[] { "TEST_CODEIN", "Codein:" }, new[] { "TEST_HEROIN", "Heroin:" } };
        private static readonly string[][] BLOOD_FIELDS = {
            new[] { "TEST_BLOOD_HC", "Số lượng HC (T/L):" }, new[] { "TEST_BLOOD_HGB", "Huyết sắc tố (g/L):" },
            new[] { "TEST_BLOOD_HCT", "Hematocrit (L/L):" }, new[] { "TEST_BLOOD_MCV", "MCV (fL):" }, new[] { "TEST_BLOOD_MCH", "MCH (pg):" },
            new[] { "TEST_BLOOD_MCHC", "MCHC (g/L):" }, new[] { "TEST_BLOOD_RDW", "RDW (%):" }, new[] { "TEST_BLOOD_BC", "Bạch cầu (G/L):" },
            new[] { "TEST_BLOOD_NEUT", "BC trung tính (G/L):" }, new[] { "TEST_BLOOD_LYMPH", "BC lympho (G/L):" },
            new[] { "TEST_BLOOD_MONO", "BC đơn nhân (G/L):" }, new[] { "TEST_BLOOD_EOS", "BC ái toan (G/L):" },
            new[] { "TEST_BLOOD_BASO", "BC ái kiềm (G/L):" }, new[] { "TEST_BLOOD_TC", "Tiểu cầu (G/L):" },
            new[] { "TEST_BLOOD_GLUCO", "Đường máu (mmol/L):" }, new[] { "TEST_BLOOD_CREATININ", "Creatinin (µmol/L):" },
            new[] { "TEST_BLOOD_ASAT", "ASAT/GOT (U/L):" }, new[] { "TEST_BLOOD_ALAT", "ALAT/GPT (U/L):" } };
        private static readonly string[][] URINE_FIELDS = {
            new[] { "TEST_URINE_SG", "Tỉ trọng:" }, new[] { "TEST_URINE_PH", "pH:" }, new[] { "TEST_URINE_LEU", "Bạch cầu (Leu/µL):" },
            new[] { "TEST_URINE_ERY", "Hồng cầu (Ery/µL):" }, new[] { "TEST_URINE_NITRIT", "Nitrit:" }, new[] { "TEST_URINE_PROTEIN", "Protein (g/L):" },
            new[] { "TEST_URINE_GLUCO", "Glucose (mmol/L):" }, new[] { "TEST_URINE_KETONE", "Thể cetonic (mmol/L):" },
            new[] { "TEST_URINE_BILIRUBIN", "Bilirubin (µmol/L):" }, new[] { "TEST_URINE_UBG", "Urobilinogen (µmol/L):" },
            new[] { "TEST_URINE_OTHER", "Khác (nếu có):" } };

        /// <summary>Hai ô mỗi hàng để cột cận lâm sàng không quá dài.</summary>
        private void AddClsPairs(LayoutControlGroup g, string[][] fields)
        {
            LayoutControlItem left = null;
            for (int i = 0; i < fields.Length; i++)
            {
                TextEdit t = Txt("txt" + fields[i][0]);
                this.clsEditors[fields[i][0]] = t;
                if (i % 2 == 0) left = Add(g, fields[i][1], t, 130);
                else Add(g, fields[i][1], t, 130, left);
            }
        }

        private void BuildColumnRight(LayoutControlGroup col)
        {
            LayoutControlGroup g = Section_(col, "III. CẬN LÂM SÀNG — 1. Xét nghiệm ma túy, nồng độ cồn");
            AddClsPairs(g, DRUG_FIELDS);
            TextEdit alcohol = Txt("txtTEST_ALCOHOL");
            this.clsEditors["TEST_ALCOHOL"] = alcohol;
            Add(g, "Nồng độ cồn:", alcohol, 130);
            this.cboDrugDoctor = Cbo("cboDrugDoctor", true);
            Add(g, "Bác sĩ/CN XN:", this.cboDrugDoctor, 130);

            g = Section_(col, "2b. Xét nghiệm máu");
            AddClsPairs(g, BLOOD_FIELDS);
            this.cboBloodDoctor = Cbo("cboBloodDoctor", true);
            Add(g, "Bác sĩ/CN XN:", this.cboBloodDoctor, 130);

            g = Section_(col, "2c. Xét nghiệm nước tiểu");
            AddClsPairs(g, URINE_FIELDS);
            this.cboUrineDoctor = Cbo("cboUrineDoctor", true);
            Add(g, "Bác sĩ/CN XN:", this.cboUrineDoctor, 130);

            g = Section_(col, "2d, 2e. Chẩn đoán hình ảnh, xét nghiệm khác");
            this.memDiim = Mem("memDiim");
            Add(g, "Chẩn đoán hình ảnh:", this.memDiim, 130, height: 44);
            this.cboDiimDoctor = Cbo("cboDiimDoctor", true);
            Add(g, "Bác sĩ:", this.cboDiimDoctor, 130);
            this.memOtherTest = Mem("memOtherTest");
            Add(g, "Xét nghiệm khác:", this.memOtherTest, 130, height: 44);
            this.cboOtherDoctor = Cbo("cboOtherDoctor", true);
            Add(g, "Bác sĩ:", this.cboOtherDoctor, 130);

            g = Section_(col, "IV. CÁC BỆNH TẬT VÀ ĐỀ NGHỊ");
            this.chkConclusionNormal = Chk("chkConclusionNormal", "Chưa phát hiện bất thường");
            Add(g, null, this.chkConclusionNormal, height: 22);
            this.icdConclusionPre = Icd("icdConclusionPre");
            Add(g, "Chẩn đoán sơ bộ (ICD):", this.icdConclusionPre, 140, height: 24);
            this.icdConclusionFinal = Icd("icdConclusionFinal");
            Add(g, "Chẩn đoán xác định (ICD):", this.icdConclusionFinal, 140, height: 24);
            this.chkConclusionNormal.CheckedChanged += (o, e) => ApplyNormalState(this.chkConclusionNormal, this.icdConclusionPre, this.icdConclusionFinal);
            this.rdoSuggest = Rdo("rdoSuggest",
                "Bình thường, hẹn khám định kỳ lần sau",
                "Có yếu tố nguy cơ, cần theo dõi thêm",
                "Đã có bệnh mạn tính, tiếp tục điều trị theo phác đồ/toa cũ",
                "Chuyển cơ sở KBCB, khám chuyên khoa",
                "Khác (tư vấn chế độ ăn uống, tập luyện...)");
            Add(g, "2. Đề nghị:", this.rdoSuggest, 140, height: 110);
            this.txtSuggestNote = Txt("txtSuggestNote");
            Add(g, "Ghi rõ (chuyển/khác):", this.txtSuggestNote, 140);

            g = Section_(col, "V. KẾT LUẬN");
            this.rdoDriverConclusion = Rdo("rdoDriverConclusion",
                "Đủ điều kiện sức khỏe lái xe/điều khiển xe máy chuyên dùng (theo hạng đề nghị)",
                "Không đủ điều kiện sức khỏe lái xe/điều khiển xe máy chuyên dùng",
                "Đạt tiêu chuẩn sức khỏe nhưng yêu cầu khám lại",
                "Đủ điều kiện hạng A1 - người khuyết tật điều khiển xe mô tô ba bánh",
                "Đủ điều kiện hạng B - người khuyết tật điều khiển xe ô tô số tự động");
            Add(g, null, this.rdoDriverConclusion, height: 110);
            this.txtReexamNote = Txt("txtReexamNote");
            Add(g, "Thời gian khám lại:", this.txtReexamNote, 140);
            this.cboConcluder = Cbo("cboConcluder", false);
            Add(g, "Người kết luận:", this.cboConcluder, 140);
            this.dteConclusionTime = new DateEdit { Name = "dteConclusionTime" };
            this.dteConclusionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dteConclusionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteConclusionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dteConclusionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteConclusionTime.Properties.EditMask = "dd/MM/yyyy HH:mm";
            Add(g, "Ngày kết luận:", this.dteConclusionTime, 140);
            this.rdoDriverConclusion.EditValueChanged += (o, e) => ApplyReexamState();

            col.AddItem(new EmptySpaceItem());
        }

        #endregion

        #region Hành vi

        /// <summary>Gắn danh mục ICD cho các cụm chọn bệnh. Gọi 1 lần sau khi tab được dựng.</summary>
        public void InitUc(List<HIS_ICD> icdData, int icdPageSize)
        {
            if (isInited) return;
            isInited = true;
            try
            {
                foreach (UcKskHistoryIcd u in this.icdEditors) u.InitUc(icdData, icdPageSize);
                this.rdoFamily.EditValueChanged += (s, e) => ApplyYesNoText(this.rdoFamily, this.txtFamily);
                this.rdoTreating.EditValueChanged += (s, e) => ApplyYesNoText(this.rdoTreating, this.memTreating);
                this.rdoMaternity.EditValueChanged += (s, e) => ApplyYesNoText(this.rdoMaternity, this.memMaternity);
                RefreshState();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Sau khi form xóa trắng các ô: đưa các ô phụ thuộc về đúng trạng thái khóa/mở.</summary>
        public void RefreshState()
        {
            ApplyYesNoText(this.rdoFamily, this.txtFamily);
            ApplyYesNoText(this.rdoTreating, this.memTreating);
            ApplyYesNoText(this.rdoMaternity, this.memMaternity);
            foreach (Section s in this.sections) ApplyNormalState(s.ChkNormal, s.PreIcd, s.FinalIcd);
            ApplyNormalState(this.chkConclusionNormal, this.icdConclusionPre, this.icdConclusionFinal);
            ApplyReexamState();
            RefreshBmi();
        }

        /// <summary>"Có" mới được ghi nội dung; "Không"/chưa chọn thì khóa và bỏ nội dung.</summary>
        private static void ApplyYesNoText(RadioGroup rdo, BaseEdit txt)
        {
            try
            {
                bool isYes = rdo.EditValue != null && rdo.EditValue != DBNull.Value && Convert.ToInt32(rdo.EditValue) == 1;
                if (!isYes) txt.EditValue = null;
                txt.Enabled = isYes;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>"Chưa phát hiện bất thường" thì không có chẩn đoán: bỏ và khóa 2 cụm ICD.</summary>
        private static void ApplyNormalState(CheckEdit chk, UcKskHistoryIcd pre, UcKskHistoryIcd fin)
        {
            try
            {
                if (chk.Checked)
                {
                    pre.SetData(null, null);
                    fin.SetData(null, null);
                }
                pre.SetReadOnly(chk.Checked);
                fin.SetReadOnly(chk.Checked);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Thời gian khám lại chỉ ghi khi kết luận "Đạt tiêu chuẩn nhưng yêu cầu khám lại" (3).</summary>
        private void ApplyReexamState()
        {
            try
            {
                bool reexam = this.rdoDriverConclusion.EditValue != null && this.rdoDriverConclusion.EditValue != DBNull.Value
                    && Convert.ToInt32(this.rdoDriverConclusion.EditValue) == 3;
                if (!reexam) this.txtReexamNote.EditValue = null;
                this.txtReexamNote.Enabled = reexam;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void RefreshBmi()
        {
            try
            {
                if (HasSpin(this.spnHeight) && HasSpin(this.spnWeight) && this.spnHeight.Value > 0)
                {
                    decimal h = this.spnHeight.Value / 100;
                    this.spnBmi.EditValue = Math.Round(this.spnWeight.Value / (h * h), 2);
                }
                else
                {
                    this.spnBmi.EditValue = null;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region Nạp / lấy dữ liệu

        /// <summary>Hành chính chỉ đọc (lấy từ hồ sơ bệnh nhân/hồ sơ điều trị).</summary>
        public void SetAdministrativeInfo(string cccdDate, string cccdPlace, string ethnic, string residence, string phone)
        {
            this.txtCccdDate.Text = cccdDate;
            this.txtCccdPlace.Text = cccdPlace;
            this.txtEthnic.Text = ethnic;
            this.txtResidence.Text = residence;
            this.txtPhone.Text = phone;
        }

        internal void SetDityRows(List<ProDriverDityRow> rows)
        {
            this.gridDity.DataSource = null;
            this.gridDity.DataSource = rows ?? new List<ProDriverDityRow>();
        }

        internal List<ProDriverDityRow> GetDityRows()
        {
            try { this.gridViewDity.PostEditor(); this.gridViewDity.UpdateCurrentRow(); }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return this.gridDity.DataSource as List<ProDriverDityRow> ?? new List<ProDriverDityRow>();
        }

        /// <summary>
        /// Đổ bản ghi đã lưu lên các ô. <paramref name="data"/> null = hồ sơ mới (để trống);
        /// <paramref name="dhst"/> null = chưa có thể lực. Đối tượng/Nguồn chi trả/Hạng GPLX/Người kết luận do form đổ.
        /// </summary>
        internal void FillData(KskProDriverADO data, HIS_DHST dhst)
        {
            try
            {
                // Radio/checkbox trước, ô phụ thuộc sau (đổi radio sẽ khóa/xóa ô đi kèm).
                SetRadio(this.rdoLicenseIssue, data != null ? data.LICENSE_ISSUE_TYPE : null);
                SetRadio(this.rdoFamily, data != null ? data.IS_FAMILY_HISTORY : null);
                SetRadio(this.rdoTreating, data != null ? data.IS_TREATING : null);
                SetRadio(this.rdoMaternity, data != null ? data.IS_MATERNITY_HISTORY : null);
                SetRadio(this.rdoSuggest, data != null ? data.SUGGEST_TYPE : null);
                SetRadio(this.rdoDriverConclusion, data != null ? data.DRIVER_CONCLUSION_TYPE : null);
                foreach (Section s in this.sections)
                    s.ChkNormal.Checked = data != null && ToShort(GetProp(data, s.Column + "_IS_NORMAL")) == 1;
                this.chkConclusionNormal.Checked = data != null && data.CONCLUSION_IS_NORMAL == 1;
                RefreshState();

                if (data != null)
                {
                    this.txtFamily.Text = data.FAMILY_HISTORY;
                    this.txtOtherDisease.Text = data.OTHER_DISEASE;
                    this.memTreating.Text = data.TREATING_MEDICINE;
                    this.memMaternity.Text = data.MATERNITY_HISTORY;

                    foreach (Section s in this.sections)
                    {
                        s.PreIcd.SetData(GetStr(data, s.Column + "_PRE_ICD_CODE"), GetStr(data, s.Column + "_PRE_ICD_NAME"));
                        s.FinalIcd.SetData(GetStr(data, s.Column + "_ICD_CODE"), GetStr(data, s.Column + "_ICD_NAME"));
                        s.TxtConclude.Text = GetStr(data, s.Column + "_CONCLUDE");
                        s.CboDoctor.EditValue = GetStr(data, s.Column + "_LOGINNAME");
                    }

                    this.txtEyeR.Text = data.EXAM_EYESIGHT_RIGHT;
                    this.txtEyeL.Text = data.EXAM_EYESIGHT_LEFT;
                    this.txtPinholeR.Text = data.EXAM_EYESIGHT_PINHOLE_RIGHT;
                    this.txtPinholeL.Text = data.EXAM_EYESIGHT_PINHOLE_LEFT;
                    this.txtGlassR.Text = data.EXAM_EYESIGHT_GLASS_RIGHT;
                    this.txtGlassL.Text = data.EXAM_EYESIGHT_GLASS_LEFT;
                    this.txtTwoEye.Text = data.EXAM_TWO_EYESIGHT;
                    this.txtTwoEyeGlass.Text = data.EXAM_TWO_EYESIGHT_GLASS;
                    this.chkFieldHoriNormal.Checked = IsMarked(data.EXAM_EYEFIELD_HORI_NORMAL);
                    this.chkFieldHoriLimit.Checked = IsMarked(data.EXAM_EYEFIELD_HORI_LIMIT);
                    this.chkFieldVertNormal.Checked = IsMarked(data.EXAM_EYEFIELD_VERT_NORMAL);
                    this.chkFieldVertLimit.Checked = IsMarked(data.EXAM_EYEFIELD_VERT_LIMIT);
                    this.chkColorNormal.Checked = data.EXAM_EYECOLOR_IS_NORMAL == 1;
                    this.chkColorBlind.Checked = data.EXAM_EYECOLOR_IS_BLIND == 1;
                    this.chkColorRed.Checked = data.EXAM_EYECOLOR_IS_BLIND_RED == 1;
                    this.chkColorGreen.Checked = data.EXAM_EYECOLOR_IS_BLIND_GREEN == 1;
                    this.chkColorYellow.Checked = data.EXAM_EYECOLOR_IS_BLIND_YELOW == 1;
                    this.txtEntLeftNormal.Text = data.EXAM_ENT_LEFT_NORMAL;
                    this.txtEntLeftWhisper.Text = data.EXAM_ENT_LEFT_WHISPER;
                    this.txtEntRightNormal.Text = data.EXAM_ENT_RIGHT_NORMAL;
                    this.txtEntRightWhisper.Text = data.EXAM_ENT_RIGHT_WHISPER;

                    foreach (var kv in this.clsEditors) kv.Value.Text = GetStr(data, kv.Key);
                    this.memDiim.Text = data.RESULT_DIIM;
                    this.memOtherTest.Text = data.RESULT_OTHER_TEST;
                    this.cboDrugDoctor.EditValue = data.TEST_DRUG_LOGINNAME;
                    this.cboBloodDoctor.EditValue = data.TEST_BLOOD_LOGINNAME;
                    this.cboUrineDoctor.EditValue = data.TEST_URINE_LOGINNAME;
                    this.cboDiimDoctor.EditValue = data.DIIM_LOGINNAME;
                    this.cboOtherDoctor.EditValue = data.OTHER_TEST_LOGINNAME;

                    this.icdConclusionPre.SetData(data.CONCLUSION_PRE_ICD_CODE, data.CONCLUSION_PRE_ICD_NAME);
                    this.icdConclusionFinal.SetData(data.CONCLUSION_ICD_CODE, data.CONCLUSION_ICD_NAME);
                    this.txtSuggestNote.Text = data.SUGGEST_NOTE;
                    this.txtReexamNote.Text = data.REEXAM_NOTE;
                }

                this.spnPulse.EditValue = (dhst != null && dhst.PULSE.HasValue) ? (object)(decimal)dhst.PULSE.Value : null;
                this.spnBloodMax.EditValue = (dhst != null && dhst.BLOOD_PRESSURE_MAX.HasValue) ? (object)(decimal)dhst.BLOOD_PRESSURE_MAX.Value : null;
                this.spnBloodMin.EditValue = (dhst != null && dhst.BLOOD_PRESSURE_MIN.HasValue) ? (object)(decimal)dhst.BLOOD_PRESSURE_MIN.Value : null;
                this.spnHeight.EditValue = (dhst != null && dhst.HEIGHT.HasValue) ? (object)dhst.HEIGHT.Value : null;
                this.spnWeight.EditValue = (dhst != null && dhst.WEIGHT.HasValue) ? (object)dhst.WEIGHT.Value : null;
                this.spnBreath.EditValue = (dhst != null && dhst.BREATH_RATE.HasValue) ? (object)dhst.BREATH_RATE.Value : null;
                RefreshBmi();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ghi giá trị các ô vào bản ghi (không đụng ID / liên kết / mã / DHST_ID / các combo form quản lý).</summary>
        internal void GetData(KskProDriverADO data)
        {
            if (data == null) return;
            try
            {
                data.LICENSE_ISSUE_TYPE = GetRadio(this.rdoLicenseIssue);
                data.IS_FAMILY_HISTORY = GetRadio(this.rdoFamily);
                data.FAMILY_HISTORY = data.IS_FAMILY_HISTORY == 1 ? NullIfEmpty(this.txtFamily.Text) : null;
                data.OTHER_DISEASE = NullIfEmpty(this.txtOtherDisease.Text);
                data.IS_TREATING = GetRadio(this.rdoTreating);
                data.TREATING_MEDICINE = data.IS_TREATING == 1 ? NullIfEmpty(this.memTreating.Text) : null;
                data.IS_MATERNITY_HISTORY = GetRadio(this.rdoMaternity);
                data.MATERNITY_HISTORY = data.IS_MATERNITY_HISTORY == 1 ? NullIfEmpty(this.memMaternity.Text) : null;

                foreach (Section s in this.sections)
                {
                    SetProp(data, s.Column + "_IS_NORMAL", s.ChkNormal.Checked ? (short?)1 : null);
                    SetProp(data, s.Column + "_PRE_ICD_CODE", NullIfEmpty(s.PreIcd.GetCodes()));
                    SetProp(data, s.Column + "_PRE_ICD_NAME", NullIfEmpty(s.PreIcd.GetNames()));
                    SetProp(data, s.Column + "_ICD_CODE", NullIfEmpty(s.FinalIcd.GetCodes()));
                    SetProp(data, s.Column + "_ICD_NAME", NullIfEmpty(s.FinalIcd.GetNames()));
                    SetProp(data, s.Column + "_CONCLUDE", NullIfEmpty(s.TxtConclude.Text));
                    SetProp(data, s.Column + "_LOGINNAME", ComboValue(s.CboDoctor));
                }

                data.EXAM_EYESIGHT_RIGHT = NullIfEmpty(this.txtEyeR.Text);
                data.EXAM_EYESIGHT_LEFT = NullIfEmpty(this.txtEyeL.Text);
                data.EXAM_EYESIGHT_PINHOLE_RIGHT = NullIfEmpty(this.txtPinholeR.Text);
                data.EXAM_EYESIGHT_PINHOLE_LEFT = NullIfEmpty(this.txtPinholeL.Text);
                data.EXAM_EYESIGHT_GLASS_RIGHT = NullIfEmpty(this.txtGlassR.Text);
                data.EXAM_EYESIGHT_GLASS_LEFT = NullIfEmpty(this.txtGlassL.Text);
                data.EXAM_TWO_EYESIGHT = NullIfEmpty(this.txtTwoEye.Text);
                data.EXAM_TWO_EYESIGHT_GLASS = NullIfEmpty(this.txtTwoEyeGlass.Text);
                data.EXAM_EYEFIELD_HORI_NORMAL = this.chkFieldHoriNormal.Checked ? "1" : null;
                data.EXAM_EYEFIELD_HORI_LIMIT = this.chkFieldHoriLimit.Checked ? "1" : null;
                data.EXAM_EYEFIELD_VERT_NORMAL = this.chkFieldVertNormal.Checked ? "1" : null;
                data.EXAM_EYEFIELD_VERT_LIMIT = this.chkFieldVertLimit.Checked ? "1" : null;
                data.EXAM_EYECOLOR_IS_NORMAL = this.chkColorNormal.Checked ? (short?)1 : null;
                data.EXAM_EYECOLOR_IS_BLIND = this.chkColorBlind.Checked ? (short?)1 : null;
                data.EXAM_EYECOLOR_IS_BLIND_RED = this.chkColorRed.Checked ? (short?)1 : null;
                data.EXAM_EYECOLOR_IS_BLIND_GREEN = this.chkColorGreen.Checked ? (short?)1 : null;
                data.EXAM_EYECOLOR_IS_BLIND_YELOW = this.chkColorYellow.Checked ? (short?)1 : null;
                data.EXAM_ENT_LEFT_NORMAL = NullIfEmpty(this.txtEntLeftNormal.Text);
                data.EXAM_ENT_LEFT_WHISPER = NullIfEmpty(this.txtEntLeftWhisper.Text);
                data.EXAM_ENT_RIGHT_NORMAL = NullIfEmpty(this.txtEntRightNormal.Text);
                data.EXAM_ENT_RIGHT_WHISPER = NullIfEmpty(this.txtEntRightWhisper.Text);

                foreach (var kv in this.clsEditors) SetProp(data, kv.Key, NullIfEmpty(kv.Value.Text));
                data.RESULT_DIIM = NullIfEmpty(this.memDiim.Text);
                data.RESULT_OTHER_TEST = NullIfEmpty(this.memOtherTest.Text);
                data.TEST_DRUG_LOGINNAME = ComboValue(this.cboDrugDoctor);
                data.TEST_BLOOD_LOGINNAME = ComboValue(this.cboBloodDoctor);
                data.TEST_URINE_LOGINNAME = ComboValue(this.cboUrineDoctor);
                data.DIIM_LOGINNAME = ComboValue(this.cboDiimDoctor);
                data.OTHER_TEST_LOGINNAME = ComboValue(this.cboOtherDoctor);

                data.CONCLUSION_IS_NORMAL = this.chkConclusionNormal.Checked ? (short?)1 : null;
                data.CONCLUSION_PRE_ICD_CODE = NullIfEmpty(this.icdConclusionPre.GetCodes());
                data.CONCLUSION_PRE_ICD_NAME = NullIfEmpty(this.icdConclusionPre.GetNames());
                data.CONCLUSION_ICD_CODE = NullIfEmpty(this.icdConclusionFinal.GetCodes());
                data.CONCLUSION_ICD_NAME = NullIfEmpty(this.icdConclusionFinal.GetNames());
                data.SUGGEST_TYPE = GetRadio(this.rdoSuggest);
                data.SUGGEST_NOTE = NullIfEmpty(this.txtSuggestNote.Text);
                data.DRIVER_CONCLUSION_TYPE = GetRadio(this.rdoDriverConclusion);
                data.REEXAM_NOTE = data.DRIVER_CONCLUSION_TYPE == 3 ? NullIfEmpty(this.txtReexamNote.Text) : null;
                if (this.dteConclusionTime.EditValue != null)
                    data.CONCLUSION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dteConclusionTime.DateTime);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ghi chỉ số thể lực vào HIS_DHST — không đụng ID / người đo.</summary>
        public void GetDhst(HIS_DHST dhst)
        {
            if (dhst == null) return;
            try
            {
                dhst.PULSE = HasSpin(this.spnPulse) ? (long?)Convert.ToInt64(this.spnPulse.Value) : null;
                dhst.BLOOD_PRESSURE_MAX = HasSpin(this.spnBloodMax) ? (long?)Convert.ToInt64(this.spnBloodMax.Value) : null;
                dhst.BLOOD_PRESSURE_MIN = HasSpin(this.spnBloodMin) ? (long?)Convert.ToInt64(this.spnBloodMin.Value) : null;
                dhst.HEIGHT = HasSpin(this.spnHeight) ? (decimal?)Math.Round(this.spnHeight.Value, 2) : null;
                dhst.WEIGHT = HasSpin(this.spnWeight) ? (decimal?)Math.Round(this.spnWeight.Value, 2) : null;
                dhst.BREATH_RATE = HasSpin(this.spnBreath) ? (decimal?)Math.Round(this.spnBreath.Value, 2) : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region Tiện ích

        private static object GetProp(object data, string name)
        {
            var pi = data.GetType().GetProperty(name);
            return pi != null ? pi.GetValue(data, null) : null;
        }

        private static string GetStr(object data, string name)
        {
            object v = GetProp(data, name);
            return v != null ? v.ToString() : null;
        }

        /// <summary>Gán theo tên cột; cột không có (sai tên) thì ghi nhật ký để phát hiện, không im lặng bỏ qua.</summary>
        private static void SetProp(object data, string name, object value)
        {
            var pi = data.GetType().GetProperty(name);
            if (pi == null || !pi.CanWrite)
            {
                LogSystem.Warn("KskProDriver: khong co cot " + name);
                return;
            }
            pi.SetValue(data, value, null);
        }

        private static short? ToShort(object v)
        {
            if (v == null) return null;
            short r;
            return short.TryParse(v.ToString(), out r) ? (short?)r : null;
        }

        private static bool IsMarked(string v)
        {
            return !string.IsNullOrWhiteSpace(v) && v.Trim() != "0";
        }

        private static string ComboValue(GridLookUpEdit cbo)
        {
            return cbo.EditValue != null && !string.IsNullOrWhiteSpace(cbo.EditValue.ToString()) ? cbo.EditValue.ToString() : null;
        }

        private static bool HasSpin(SpinEdit spn)
        {
            return spn.EditValue != null && spn.EditValue != DBNull.Value;
        }

        /// <summary>Radio: giá trị kiểu int — gán đúng kiểu int, không gán short/long.</summary>
        private static void SetRadio(RadioGroup rdo, short? value)
        {
            rdo.EditValue = value.HasValue ? (object)(int)value.Value : null;
        }

        private static short? GetRadio(RadioGroup rdo)
        {
            if (rdo.EditValue == null || rdo.EditValue == DBNull.Value) return null;
            return Convert.ToInt16(rdo.EditValue);
        }

        private static string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        #endregion
    }
}
