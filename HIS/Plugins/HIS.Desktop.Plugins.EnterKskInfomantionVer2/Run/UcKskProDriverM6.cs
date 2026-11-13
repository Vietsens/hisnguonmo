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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// Giao diện tab "Ksk định kỳ lái xe ô tô" — Giấy khám sức khỏe định kỳ đối với người hành nghề lái xe ô tô,
    /// Mẫu 06 (Thông tư 25/2026/TT-BYT). Xem _Software-Specs PTTK_XXXXX_BV_DK_Xuyen_A_KSK_Dinh_Ky_Lai_Xe_O_To_Mau_06_TT25.
    ///
    /// Dựng hoàn toàn bằng code (không Designer) theo 5 tab con đúng thứ tự trên giấy:
    ///   Hành chính - Tiền sử · Khám thể lực · Khám lâm sàng (14 mục) · Cận lâm sàng · Kết luận.
    /// Mỗi tab con là 1 LayoutControl chia 2 cột, mỗi mục 1 khung có tiêu đề — cùng kiểu tab Ksk lái xe (Mẫu 5);
    /// khám lâm sàng: mục 1–4 cột trái, mục 5 Mắt · 6 Tai - Mũi - Họng · 7 Răng - Hàm - Mặt cột phải.
    ///
    /// Nạp / lấy dữ liệu đi qua BẢNG NỐI tên cột HIS_KSK_PRO_DRIVER -> ô nhập (dicField / dicIcd), đọc ghi
    /// bằng phản chiếu: hơn 200 cột, viết tay từng dòng gán cho cả 2 chiều rất dễ sót.
    /// Các ô dùng danh mục chung (Đối tượng, Nguồn chi trả, Phân loại, Người khám, Hạng GPLX, Người kết luận)
    /// do form khởi tạo nguồn — cùng cơ chế với các tab khác.
    /// </summary>
    public class UcKskProDriverM6 : XtraUserControl
    {
        #region ===== Khai báo =====

        /// <summary>Độ rộng nhãn mặc định trong 1 khung.</summary>
        private const int TXT_W = 150;
        /// <summary>Chiều cao 1 dòng ô nhập.</summary>
        private const int ROW_H = 24;

        /// <summary>14 mục khám lâm sàng: tiền tố cột · nhãn mục · tiêu đề nhóm đặt trước (rỗng = không mở nhóm).</summary>
        internal static readonly string[][] SECTIONS = new string[][]
        {
            new[] { "EXAM_CIRCULATION",  "a) Tuần hoàn",            "1. Nội khoa" },
            new[] { "EXAM_RESPIRATORY",  "b) Hô hấp",               "" },
            new[] { "EXAM_DIGESTION",    "c) Tiêu hóa",             "" },
            new[] { "EXAM_KIDNEY_URO",   "d) Thận - Tiết niệu",     "" },
            new[] { "EXAM_OEND",         "đ) Nội tiết",             "" },
            new[] { "EXAM_MUSCLE_BONE",  "e) Cơ - xương - khớp",    "" },
            new[] { "EXAM_NEUROLOGICAL", "g) Thần kinh",            "" },
            new[] { "EXAM_MENTAL",       "h) Tâm thần",             "" },
            new[] { "EXAM_SURGERY",      "2. Ngoại khoa",           "" },
            new[] { "EXAM_DERMATOLOGY",  "3. Da liễu",              "" },
            new[] { "EXAM_OBSTETRIC",    "4. Sản phụ khoa",         "" },
            new[] { "EXAM_EYE",          "5. Mắt",                  "" },
            new[] { "EXAM_ENT",          "6. Tai - Mũi - Họng",     "" },
            new[] { "EXAM_STOMATOLOGY",  "7. Răng - Hàm - Mặt",     "" }
        };

        /// <summary>Mục khám lâm sàng đặt ở cột phải: 5 Mắt, 6 Tai - Mũi - Họng, 7 Răng - Hàm - Mặt.</summary>
        private static readonly HashSet<string> RIGHT_SECTIONS = new HashSet<string> { "EXAM_EYE", "EXAM_ENT", "EXAM_STOMATOLOGY" };

        private static readonly int[] TEETH_UPPER = { 18, 17, 16, 15, 14, 13, 12, 11, 21, 22, 23, 24, 25, 26, 27, 28 };
        private static readonly int[] TEETH_LOWER = { 48, 47, 46, 45, 44, 43, 42, 41, 31, 32, 33, 34, 35, 36, 37, 38 };

        /// <summary>Tên cột HIS_KSK_PRO_DRIVER -> ô nhập (TextEdit / MemoEdit / CheckEdit / RadioGroup / GridLookUpEdit / ComboBoxEdit).</summary>
        private readonly Dictionary<string, BaseEdit> dicField = new Dictionary<string, BaseEdit>();
        /// <summary>Tiền tố cột ICD (VD EXAM_EYE_PRE_ICD -> cột ..._CODE / ..._NAME) -> ô chọn ICD.</summary>
        private readonly Dictionary<string, UcKskHistoryIcd> dicIcd = new Dictionary<string, UcKskHistoryIcd>();
        /// <summary>Câu tiền sử bản thân: mã bệnh -> ô Có/Không.</summary>
        private readonly Dictionary<long, RadioGroup> dicDisease = new Dictionary<long, RadioGroup>();
        private readonly List<GridLookUpEdit> rankCombos = new List<GridLookUpEdit>();
        private readonly List<GridLookUpEdit> loginCombos = new List<GridLookUpEdit>();
        /// <summary>Các ô chỉ dành cho bệnh nhân nữ (sản phụ khoa, XN nữ, tiền sử thai sản).</summary>
        private readonly List<Control> femaleOnly = new List<Control>();

        private XtraTabControl tabs;
        private LayoutControl lcAdmin, lcPhysical, lcClinical, lcCls, lcConclusion;
        /// <summary>Nhóm chứa các câu tiền sử bản thân — dựng lại theo danh mục bệnh.</summary>
        private LayoutControlGroup grpDiseases;
        private readonly List<BaseLayoutItem> diseaseItems = new List<BaseLayoutItem>();
        private readonly List<SimpleLabelItem> diseaseLabels = new List<SimpleLabelItem>();

        private TextEdit txtEthnic, txtBlood, txtHeinCard, txtResidence, txtCareer, txtWorkPlace, txtPhone;
        private GridLookUpEdit cboObject, cboPaymentSource, cboLicenseClass, cboConcluder, cboDhstExecutor;
        private DateEdit dteConclusionTime;
        private SpinEdit spnHeight, spnWeight, spnPulse, spnBpMax, spnBpMin, spnBreathRate;
        private TextEdit txtBmi, txtReexam;
        private LabelControl lblConclusionLicense;
        private RadioGroup rdoConclusion, rdoSuggest, rdoFamily, rdoTreating, rdoMaternity;
        private MemoEdit memSuggestNote;
        private bool isFemale = true;
        private bool isInited = false;
        private bool applyingState = false;

        /// <summary>Bấm "..." cạnh ô Kết luận của 1 mục khám: (ô kết luận, ô phân loại của mục) — form mở Thư viện văn bản.</summary>
        public event Action<TextEdit, GridLookUpEdit> ConclusionTextLibRequested;
        /// <summary>Bấm "Nối chỉ số CLS…" — form mở hộp thoại nối chỉ số xét nghiệm với ô của Mẫu 06.</summary>
        public event EventHandler ClsMapRequested;

        #endregion

        #region ===== Thuộc tính cho form =====

        public GridLookUpEdit CboObject { get { return this.cboObject; } }
        public GridLookUpEdit CboPaymentSource { get { return this.cboPaymentSource; } }
        public GridLookUpEdit CboLicenseClass { get { return this.cboLicenseClass; } }
        public GridLookUpEdit CboConcluder { get { return this.cboConcluder; } }
        /// <summary>Bác sĩ khám thể lực — lưu vào người đo (EXECUTE_LOGINNAME) của HIS_DHST.</summary>
        public GridLookUpEdit CboDhstExecutor { get { return this.cboDhstExecutor; } }
        public DateEdit DteConclusionTime { get { return this.dteConclusionTime; } }
        public SpinEdit SpnBloodPressureMax { get { return this.spnBpMax; } }
        public SpinEdit SpnBloodPressureMin { get { return this.spnBpMin; } }
        public RadioGroup RdoConclusion { get { return this.rdoConclusion; } }
        public TextEdit TxtReexam { get { return this.txtReexam; } }
        /// <summary>Mọi ô Phân loại (thể lực, 14 mục, phân loại sức khỏe) — form đổ danh mục phân loại.</summary>
        public IList<GridLookUpEdit> RankCombos { get { return this.rankCombos; } }
        /// <summary>Mọi ô Bác sĩ khám / Người thực hiện (trừ Người kết luận) — form đổ danh sách nhân viên.</summary>
        public IList<GridLookUpEdit> LoginCombos { get { return this.loginCombos; } }

        /// <summary>Ô nhập theo tên cột (null nếu không có).</summary>
        public BaseEdit GetFieldEditor(string column)
        {
            BaseEdit be;
            return dicField.TryGetValue(column, out be) ? be : null;
        }

        #endregion

        public UcKskProDriverM6()
        {
            try
            {
                this.SuspendLayout();
                this.Dock = DockStyle.Fill;
                tabs = new XtraTabControl();
                tabs.Dock = DockStyle.Fill;
                this.Controls.Add(tabs);
                lcAdmin = AddPage("Hành chính - Tiền sử");
                lcPhysical = AddPage("Khám thể lực");
                lcClinical = AddPage("Khám lâm sàng");
                lcCls = AddPage("Cận lâm sàng");
                lcConclusion = AddPage("Kết luận");
                Build(lcAdmin, BuildAdmin);
                Build(lcPhysical, BuildPhysical);
                Build(lcClinical, BuildClinical);
                Build(lcCls, BuildCls);
                Build(lcConclusion, BuildConclusion);
                BlockWheelChange(this);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { this.ResumeLayout(false); }
        }

        /// <summary>Gắn danh mục ICD cho các ô chọn ICD + hành vi khóa/mở. Gọi 1 lần sau khi tab được dựng.</summary>
        public void InitUc(List<HIS_ICD> icdData, int icdPageSize)
        {
            if (isInited) return;
            isInited = true;
            try
            {
                foreach (var uc in dicIcd.Values) uc.InitUc(icdData, icdPageSize);
                RefreshState();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #region ===== Bố cục (LayoutControl) =====

        private LayoutControl AddPage(string text)
        {
            XtraTabPage page = new XtraTabPage();
            page.Text = text;
            LayoutControl lc = new LayoutControl();
            lc.Dock = DockStyle.Fill;
            lc.AllowCustomizationMenu = false;
            lc.Root.GroupBordersVisible = false;
            lc.Root.TextVisible = false;
            page.Controls.Add(lc);
            tabs.TabPages.Add(page);
            return lc;
        }

        private static void Build(LayoutControl lc, Action<LayoutControlGroup> build)
        {
            lc.BeginUpdate();
            try { build(lc.Root); }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { lc.EndUpdate(); }
        }

        /// <summary>Cột bố cục (không viền): cột đầu thêm cuối root, cột sau chèn bên phải cột trước — chia đôi bề ngang.</summary>
        private static LayoutControlGroup Column(LayoutControlGroup root, LayoutControlGroup leftOf)
        {
            LayoutControlGroup g = new LayoutControlGroup();
            g.GroupBordersVisible = false;
            g.TextVisible = false;
            if (leftOf == null) root.AddGroup(g);
            else root.AddGroup(g, leftOf, InsertType.Right);
            return g;
        }

        /// <summary>Ô trống cuối cột — dồn các khung lên trên.</summary>
        private static void EndColumn(LayoutControlGroup col)
        {
            col.AddItem(new EmptySpaceItem());
        }

        /// <summary>Khung có tiêu đề (1 mục trên giấy).</summary>
        private static LayoutControlGroup Box(LayoutControlGroup col, string title)
        {
            LayoutControlGroup g = col.AddGroup(title);
            g.ExpandButtonVisible = false;
            return g;
        }

        /// <summary>
        /// Thêm 1 ô vào khung; <paramref name="rightOf"/> != null thì đặt cùng hàng, bên phải ô đó (chia đôi chỗ ô đó).
        /// <paramref name="width"/> / <paramref name="height"/> &gt; 0 thì cố định kích thước ô (tính cả nhãn).
        /// </summary>
        private static LayoutControlItem Add(LayoutControlGroup g, string text, Control c, int textWidth = TXT_W,
            BaseLayoutItem rightOf = null, int height = 0, int width = 0)
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
            if (height > 0 || width > 0)
            {
                it.SizeConstraintsType = SizeConstraintsType.Custom;
                it.MinSize = new Size(width > 0 ? width : 40, height > 0 ? height : ROW_H);
                it.MaxSize = new Size(width > 0 ? width : 0, height > 0 ? height : 0);
            }
            return it;
        }

        /// <summary>Dòng chú thích chữ xám.</summary>
        private static SimpleLabelItem Hint(LayoutControlGroup g, string text, BaseLayoutItem rightOf = null)
        {
            SimpleLabelItem s = new SimpleLabelItem();
            s.Text = text;
            s.AppearanceItemCaption.ForeColor = Color.DimGray;
            s.AppearanceItemCaption.Options.UseForeColor = true;
            if (rightOf == null) g.AddItem(s);
            else g.AddItem(s, rightOf, InsertType.Right);
            return s;
        }

        /// <summary>Ô trống bên phải 1 ô cố định độ rộng — để ô đó không bị kéo giãn.</summary>
        private static void Filler(LayoutControlGroup g, BaseLayoutItem rightOf)
        {
            g.AddItem(new EmptySpaceItem(), rightOf, InsertType.Right);
        }

        /// <summary>
        /// Nhóm con dạng bảng (không viền) cho các lưới ô thẳng cột (thị lực, thính lực, răng, chỉ số xét nghiệm).
        /// <paramref name="cols"/>: &gt; 0 = độ rộng cố định (px), &lt; 0 = phần trăm phần còn lại.
        /// </summary>
        private static LayoutControlGroup Table(LayoutControlGroup parent, int[] cols, int[] rowHeights)
        {
            LayoutControlGroup t = new LayoutControlGroup();
            t.GroupBordersVisible = false;
            t.TextVisible = false;
            parent.AddGroup(t);
            t.LayoutMode = LayoutMode.Table;
            OptionsTableLayoutGroup o = t.OptionsTableLayoutGroup;
            int c0 = o.ColumnDefinitions.Count, r0 = o.RowDefinitions.Count;
            foreach (int w in cols)
                o.ColumnDefinitions.Add(w > 0
                    ? new ColumnDefinition(t, w, System.Windows.Forms.SizeType.Absolute)
                    : new ColumnDefinition(t, -w, System.Windows.Forms.SizeType.Percent));
            foreach (int h in rowHeights)
                o.RowDefinitions.Add(new RowDefinition(t, h, System.Windows.Forms.SizeType.Absolute));
            for (int i = 0; i < c0; i++) o.ColumnDefinitions.RemoveAt(0);
            for (int i = 0; i < r0; i++) o.RowDefinitions.RemoveAt(0);
            return t;
        }

        private static int[] Rows(int count, int height)
        {
            return Enumerable.Repeat(height, count).ToArray();
        }

        private static void Place(BaseLayoutItem it, int row, int col)
        {
            it.OptionsTableLayoutItem.RowIndex = row;
            it.OptionsTableLayoutItem.ColumnIndex = col;
        }

        /// <summary>Ô nhập trong bảng (không nhãn).</summary>
        private static LayoutControlItem Cell(LayoutControlGroup t, Control c, int row, int col)
        {
            LayoutControlItem it = t.AddItem("", c);
            it.TextVisible = false;
            Place(it, row, col);
            return it;
        }

        /// <summary>Chữ trong bảng; <paramref name="header"/> = tiêu đề cột (chữ xám, canh giữa).</summary>
        private static SimpleLabelItem CellText(LayoutControlGroup t, string text, int row, int col, bool header = false)
        {
            SimpleLabelItem s = new SimpleLabelItem();
            s.Text = text;
            if (header)
            {
                s.AppearanceItemCaption.ForeColor = Color.DimGray;
                s.AppearanceItemCaption.Options.UseForeColor = true;
                s.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                s.AppearanceItemCaption.Options.UseTextOptions = true;
            }
            t.AddItem(s);
            Place(s, row, col);
            return s;
        }

        #endregion

        #region ===== Tạo ô nhập =====

        private void Reg(string column, BaseEdit be)
        {
            if (!string.IsNullOrEmpty(column)) dicField[column] = be;
        }

        private TextEdit Txt(string column, bool readOnly = false)
        {
            TextEdit t = new TextEdit();
            t.Properties.ReadOnly = readOnly;
            Reg(column, t);
            return t;
        }

        private MemoEdit Memo(string column)
        {
            MemoEdit m = new MemoEdit();
            Reg(column, m);
            return m;
        }

        private CheckEdit Chk(string column, string text)
        {
            CheckEdit c = new CheckEdit();
            c.Properties.Caption = text;
            Reg(column, c);
            return c;
        }

        /// <summary>Ô chọn 1 trong nhiều: <paramref name="valueText"/> = giá trị, nhãn, giá trị, nhãn...</summary>
        private RadioGroup Rdo(string column, int columns, params object[] valueText)
        {
            RadioGroup r = new RadioGroup();
            r.Properties.AllowFocused = false;
            r.Properties.Columns = columns;
            for (int i = 0; i + 1 < valueText.Length; i += 2)
                r.Properties.Items.Add(new RadioGroupItem(valueText[i], (string)valueText[i + 1]));
            r.EditValue = null;
            Reg(column, r);
            return r;
        }

        private GridLookUpEdit Gle(string column)
        {
            GridLookUpEdit g = new GridLookUpEdit();
            g.Properties.NullText = "";
            Reg(column, g);
            return g;
        }

        private GridLookUpEdit Rank(string column)
        {
            GridLookUpEdit g = Gle(column);
            rankCombos.Add(g);
            return g;
        }

        private GridLookUpEdit Login(string column)
        {
            GridLookUpEdit g = Gle(column);
            loginCombos.Add(g);
            return g;
        }

        private static SpinEdit Spin(bool isFloat, decimal max)
        {
            SpinEdit s = new SpinEdit();
            s.Properties.IsFloatValue = isFloat;
            s.Properties.MinValue = 0;
            s.Properties.MaxValue = max;
            s.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            s.EditValue = null;
            return s;
        }

        private UcKskHistoryIcd Icd(string prefix)
        {
            UcKskHistoryIcd uc = new UcKskHistoryIcd();
            dicIcd[prefix] = uc;
            return uc;
        }

        private ComboBoxEdit ToothBox(int tooth)
        {
            ComboBoxEdit c = new ComboBoxEdit();
            c.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            c.Properties.Items.AddRange(new object[] { "", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            dicField["TOOTH_" + tooth] = c;
            return c;
        }

        #endregion

        #region ===== Lăn chuột không đổi giá trị =====

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Lăn chuột trên ô Có/Không, ô chọn, ô số, ô ngày, ô tích KHÔNG đổi giá trị (chỉ đổi khi bấm chuột / gõ phím) —
        /// lượt lăn chuyển lên LayoutControl chứa ô để màn hình cuộn như bình thường. Cùng cách tab Ksk lái xe (Mẫu 5).
        /// </summary>
        private static void BlockWheelChange(Control root)
        {
            foreach (Control c in root.Controls)
            {
                if (c is RadioGroup || c is CheckEdit || c is SpinEdit || c is ComboBoxEdit || c is DateEdit || c is GridLookUpEdit)
                    NoWheel((BaseEdit)c);
                else if (!(c is UcKskHistoryIcd))
                    BlockWheelChange(c);
            }
        }

        private static void NoWheel(BaseEdit be)
        {
            be.MouseWheel += (s, e) =>
            {
                // Đang mở danh sách thả xuống thì để danh sách tự cuộn.
                PopupBaseEdit pe = be as PopupBaseEdit;
                if (pe != null && pe.IsPopupOpen) return;
                DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                ScrollParentByWheel(be, e.Delta);
            };
        }

        private static void ScrollParentByWheel(Control c, int delta)
        {
            try
            {
                Control p = c.Parent;
                while (p != null && !(p is LayoutControl)) p = p.Parent;
                if (p == null || !p.IsHandleCreated) return;
                Point pt = Control.MousePosition;
                IntPtr wParam = new IntPtr(delta << 16);
                IntPtr lParam = new IntPtr((pt.Y << 16) | (pt.X & 0xFFFF));
                SendMessage(p.Handle, 0x020A /* WM_MOUSEWHEEL */, wParam, lParam);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Dựng các tab con =====

        /// <summary>Tab con 1 — trái: hành chính, đề nghị khám, tiền sử gia đình / điều trị / thai sản; phải: 21 câu tiền sử bản thân.</summary>
        private void BuildAdmin(LayoutControlGroup root)
        {
            LayoutControlGroup c1 = Column(root, null);
            LayoutControlGroup c2 = Column(root, c1);

            LayoutControlGroup g = Box(c1, "THÔNG TIN HÀNH CHÍNH");
            Hint(g, "Họ tên, ngày sinh, giới tính, CCCD: xem ở khối thông tin bệnh nhân phía trên.");
            txtEthnic = Txt(null, true);
            LayoutControlItem it = Add(g, "Dân tộc:", txtEthnic, 140);
            txtBlood = Txt(null, true);
            Add(g, "Nhóm máu:", txtBlood, 80, it);
            txtHeinCard = Txt(null, true);
            it = Add(g, "Số thẻ BHYT:", txtHeinCard, 140);
            txtPhone = Txt(null, true);
            Add(g, "Điện thoại:", txtPhone, 80, it);
            txtResidence = Txt(null, true);
            Add(g, "Nơi ở hiện tại:", txtResidence, 140);
            txtCareer = Txt(null, true);
            it = Add(g, "Nghề nghiệp:", txtCareer, 140);
            txtWorkPlace = Txt(null, true);
            Add(g, "Nơi làm việc:", txtWorkPlace, 80, it);
            Add(g, "Xã, phường nơi làm việc:", Txt("WORK_COMMUNE_NAME"), 140);

            g = Box(c1, "THÔNG TIN ĐỀ NGHỊ KHÁM");
            cboLicenseClass = Gle("LICENSE_CLASS_ID");
            Add(g, "Hạng giấy phép lái xe:", cboLicenseClass, 140);
            cboObject = Gle(null);
            Add(g, "Đối tượng:", cboObject, 140);
            cboPaymentSource = Gle(null);
            Add(g, "Nguồn chi trả:", cboPaymentSource, 140);
            it = Add(g, "Địa điểm khám:", Rdo("EXAM_PLACE_TYPE", 2, 1, "Cơ sở KBCB", 2, "Khám lưu động"), 140, height: 26, width: 144 + 260);
            Filler(g, it);

            g = Box(c1, "TIỀN SỬ BỆNH — 1. TIỀN SỬ GIA ĐÌNH");
            rdoFamily = Rdo("IS_FAMILY_HISTORY", 2, 0, "Không", 1, "Có");
            it = Add(g, "Tiền sử gia đình:", rdoFamily, 140, height: 26, width: 144 + 130);
            TextEdit txtFamily = Txt("FAMILY_HISTORY");
            Add(g, "Tên bệnh:", txtFamily, 60, it);
            Hint(g, "Gia đình có ai mắc: truyền nhiễm, tim mạch, đái tháo đường, lao, hen phế quản, ung thư, động kinh, rối loạn tâm thần, bệnh khác.");
            rdoFamily.EditValueChanged += (s, e) => ApplyYesNoState(rdoFamily, txtFamily);

            g = Box(c2, "TIỀN SỬ BỆNH — 2. BỆNH, TẬT CỦA BẢN THÂN: ông (bà) đã/đang mắc bệnh, tình trạng nào sau đây");
            grpDiseases = new LayoutControlGroup();
            grpDiseases.GroupBordersVisible = false;
            grpDiseases.TextVisible = false;
            g.AddGroup(grpDiseases);
            Add(g, "22. Bệnh khác (ghi rõ):", Txt("OTHER_DISEASE"), 140);

            // 3a, 3b đặt cột phải dưới 21 câu (đúng thứ tự trên giấy, lấp khoảng trống bên phải).
            g = Box(c2, "TIỀN SỬ BỆNH — 3. ĐIỀU TRỊ, THAI SẢN");
            rdoTreating = Rdo("IS_TREATING", 2, 0, "Không", 1, "Có");
            it = Add(g, "3a. Đang điều trị bệnh:", rdoTreating, 140, height: 26, width: 144 + 130);
            Filler(g, it);
            MemoEdit memTreating = Memo("TREATING_MEDICINE");
            Add(g, "Thuốc, liều lượng:", memTreating, 140, height: 44);
            rdoTreating.EditValueChanged += (s, e) => ApplyYesNoState(rdoTreating, memTreating);

            rdoMaternity = Rdo("IS_MATERNITY_HISTORY", 2, 0, "Không", 1, "Có");
            it = Add(g, "3b. Tiền sử thai sản (nữ):", rdoMaternity, 140, height: 26, width: 144 + 130);
            Filler(g, it);
            MemoEdit memMaternity = Memo("MATERNITY_HISTORY");
            Add(g, "Ghi rõ:", memMaternity, 140, height: 44);
            rdoMaternity.EditValueChanged += (s, e) => ApplyYesNoState(rdoMaternity, memMaternity);
            femaleOnly.Add(rdoMaternity);
            femaleOnly.Add(memMaternity);

            EndColumn(c1);
            EndColumn(c2);
        }

        /// <summary>Tab con 2 — khám thể lực.</summary>
        private void BuildPhysical(LayoutControlGroup root)
        {
            LayoutControlGroup c1 = Column(root, null);
            LayoutControlGroup c2 = Column(root, c1);

            LayoutControlGroup g = Box(c1, "I. KHÁM THỂ LỰC");
            spnHeight = Spin(true, 300);
            LayoutControlItem it = Add(g, "Chiều cao (cm):", spnHeight, 120);
            spnWeight = Spin(true, 500);
            Add(g, "Cân nặng (kg):", spnWeight, 120, it);
            txtBmi = Txt(null, true);
            it = Add(g, "Chỉ số BMI:", txtBmi, 120);
            spnBreathRate = Spin(false, 200);
            Add(g, "Nhịp thở (lần/phút):", spnBreathRate, 120, it);
            spnPulse = Spin(false, 500);
            it = Add(g, "Mạch (lần/phút):", spnPulse, 120);
            // Huyết áp: 2 ô tâm thu / tâm trương cùng độ rộng.
            spnBpMax = Spin(false, 400);
            LayoutControlItem itBp = Add(g, "Huyết áp (mmHg):", spnBpMax, 120, it, width: 124 + 90);
            spnBpMin = Spin(false, 400);
            LayoutControlItem itBpMin = Add(g, "/", spnBpMin, 8, itBp, width: 12 + 90);
            Filler(g, itBpMin);
            it = Add(g, "Phân loại thể lực:", Rank("DHST_RANK"), 120);
            // Bác sĩ khám thể lực = người đo của HIS_DHST (như các tab khác) — không phải cột của HIS_KSK_PRO_DRIVER.
            cboDhstExecutor = Login(null);
            Add(g, "Bác sĩ:", cboDhstExecutor, 120, it);
            Add(g, "Kết luận:", Txt("DHST_CONCLUDE"), 120);
            spnHeight.EditValueChanged += (s, e) => UpdateBmi();
            spnWeight.EditValueChanged += (s, e) => UpdateBmi();

            EndColumn(c1);
            EndColumn(c2);
        }

        /// <summary>Tab con 3 — 14 mục khám lâm sàng: mục 1–4 cột trái, mục 5–7 cột phải.</summary>
        private void BuildClinical(LayoutControlGroup root)
        {
            Hint(root, "Kết luận từng chuyên khoa: đủ / không đủ tiêu chuẩn theo Phụ lục 1 Thông tư 36/2024/TT-BYT.");
            LayoutControlGroup c1 = Column(root, null);
            LayoutControlGroup c2 = Column(root, c1);
            foreach (string[] s in SECTIONS)
            {
                bool sub = s[1].Length > 1 && s[1][1] == ')';
                BuildSection(RIGHT_SECTIONS.Contains(s[0]) ? c2 : c1, s[0], sub ? "1. Nội khoa — " + s[1] : s[1]);
            }
            EndColumn(c1);
            EndColumn(c2);
        }

        /// <summary>
        /// 1 mục khám: [ô riêng của mục] · Chưa phát hiện bất thường | Phân loại | Bác sĩ · 2 ô ICD · Kết luận [...].
        /// </summary>
        private void BuildSection(LayoutControlGroup col, string pre, string title)
        {
            LayoutControlGroup g = Box(col, title);
            CheckEdit chkRefuse = null;
            if (pre == "EXAM_OBSTETRIC")
            {
                chkRefuse = Chk("IS_REFUSE_OBSTETRIC", "Từ chối khám sản phụ khoa");
                chkRefuse.CheckedChanged += (sd, e) => RefreshState();
                Add(g, null, chkRefuse, height: ROW_H);
                femaleOnly.Add(chkRefuse);
            }
            else if (pre == "EXAM_EYE") BuildEyeTable(g);
            else if (pre == "EXAM_ENT") BuildEntTable(g);
            else if (pre == "EXAM_STOMATOLOGY") BuildTeeth(g);

            CheckEdit chkNormal = Chk(pre + "_IS_NORMAL", "Chưa phát hiện bất thường");
            chkNormal.CheckedChanged += (sd, e) => RefreshState();
            LayoutControlItem itChk = Add(g, null, chkNormal, height: ROW_H);
            GridLookUpEdit login = Login(pre + "_LOGINNAME");
            Add(g, "Bác sĩ:", login, 45, itChk);
            GridLookUpEdit rank = Rank(pre + "_RANK");
            Add(g, "Phân loại:", rank, 60, itChk);

            Add(g, "Chẩn đoán sơ bộ (ICD):", Icd(pre + "_PRE_ICD"), TXT_W, height: ROW_H);
            Add(g, "Chẩn đoán xác định (ICD):", Icd(pre + "_ICD"), TXT_W, height: ROW_H);
            TextEdit conclude = Txt(pre + "_CONCLUDE");
            LayoutControlItem itConclude = Add(g, "Kết luận:", conclude, TXT_W);
            SimpleButton btnLib = new SimpleButton();
            btnLib.Text = "...";
            btnLib.ToolTip = "Thư viện văn bản — cú pháp mẫu: nội dung;PL:Lx  (x = 1..5 tự chọn Phân loại)";
            btnLib.Click += (sd, e) => { if (ConclusionTextLibRequested != null) ConclusionTextLibRequested(conclude, rank); };
            Add(g, null, btnLib, rightOf: itConclude, height: ROW_H, width: 30);

            if (pre == "EXAM_OBSTETRIC")
            {
                femaleOnly.Add(chkNormal); femaleOnly.Add(rank); femaleOnly.Add(conclude); femaleOnly.Add(btnLib);
                femaleOnly.Add(dicIcd[pre + "_PRE_ICD"]); femaleOnly.Add(dicIcd[pre + "_ICD"]); femaleOnly.Add(login);
            }
        }

        /// <summary>Mắt: bảng thị lực (không kính / kính lỗ / có kính) và khúc xạ (độ cầu / độ trụ / trục), mắt phải · mắt trái.</summary>
        private void BuildEyeTable(LayoutControlGroup g)
        {
            LayoutControlGroup t = Table(g, new[] { 90, 90, 90, 30, 110, 90, 90, -100 }, new[] { 20, ROW_H, ROW_H, ROW_H });
            CellText(t, "Thị lực", 0, 0, true);
            CellText(t, "Mắt phải", 0, 1, true);
            CellText(t, "Mắt trái", 0, 2, true);
            CellText(t, "Khúc xạ (nếu có)", 0, 4, true);
            CellText(t, "Mắt phải", 0, 5, true);
            CellText(t, "Mắt trái", 0, 6, true);
            string[][] eye = { new[] { "Không kính", "EXAM_EYESIGHT_RIGHT", "EXAM_EYESIGHT_LEFT", "Độ cầu", "EXAM_EYE_SPHERE_RIGHT", "EXAM_EYE_SPHERE_LEFT" },
                               new[] { "Kính lỗ", "EXAM_EYESIGHT_PINHOLE_RIGHT", "EXAM_EYESIGHT_PINHOLE_LEFT", "Độ trụ", "EXAM_EYE_CYLINDER_RIGHT", "EXAM_EYE_CYLINDER_LEFT" },
                               new[] { "Có kính", "EXAM_EYESIGHT_GLASS_RIGHT", "EXAM_EYESIGHT_GLASS_LEFT", "Trục", "EXAM_EYE_AXIS_RIGHT", "EXAM_EYE_AXIS_LEFT" } };
            for (int i = 0; i < eye.Length; i++)
            {
                int r = i + 1;
                CellText(t, eye[i][0] + ":", r, 0);
                Cell(t, Txt(eye[i][1]), r, 1);
                Cell(t, Txt(eye[i][2]), r, 2);
                CellText(t, eye[i][3] + ":", r, 4);
                Cell(t, Txt(eye[i][4]), r, 5);
                Cell(t, Txt(eye[i][5]), r, 6);
            }
        }

        /// <summary>Tai - Mũi - Họng: thính lực (m) nói thường · nói thầm, tai trái · tai phải.</summary>
        private void BuildEntTable(LayoutControlGroup g)
        {
            LayoutControlGroup t = Table(g, new[] { 90, 110, 110, -100 }, new[] { 20, ROW_H, ROW_H });
            CellText(t, "Thính lực (m)", 0, 0, true);
            CellText(t, "Nói thường", 0, 1, true);
            CellText(t, "Nói thầm", 0, 2, true);
            CellText(t, "Tai trái:", 1, 0);
            Cell(t, Txt("EXAM_ENT_LEFT_NORMAL"), 1, 1);
            Cell(t, Txt("EXAM_ENT_LEFT_WHISPER"), 1, 2);
            CellText(t, "Tai phải:", 2, 0);
            Cell(t, Txt("EXAM_ENT_RIGHT_NORMAL"), 2, 1);
            Cell(t, Txt("EXAM_ENT_RIGHT_WHISPER"), 2, 2);
        }

        /// <summary>Răng - Hàm - Mặt: sơ đồ 32 răng — mỗi răng 1 ô chọn mã 0–9 theo chú thích Mẫu 06.</summary>
        private void BuildTeeth(LayoutControlGroup g)
        {
            List<int> cols = new List<int> { 70 };
            for (int i = 0; i < TEETH_UPPER.Length; i++)
            {
                if (i == 8) cols.Add(12);
                cols.Add(40);
            }
            cols.Add(-100);
            LayoutControlGroup t = Table(g, cols.ToArray(), new[] { 18, ROW_H, ROW_H, 18 });
            CellText(t, "Hàm trên", 1, 0);
            CellText(t, "Hàm dưới", 2, 0);
            for (int i = 0; i < TEETH_UPPER.Length; i++)
            {
                int col = 1 + i + (i >= 8 ? 1 : 0);
                CellText(t, TEETH_UPPER[i].ToString(), 0, col, true);
                Cell(t, ToothBox(TEETH_UPPER[i]), 1, col);
                Cell(t, ToothBox(TEETH_LOWER[i]), 2, col);
                CellText(t, TEETH_LOWER[i].ToString(), 3, col, true);
            }
            Hint(g, "Tình trạng răng: 0 Bình thường · 1 Sâu · 2 Trám sâu lại · 3 Trám tốt · 4 Mất do sâu");
            Hint(g, "5 Mất lý do khác · 6 Bít hố rãnh · 7 Trụ, cầu, implant · 8 Chưa mọc · 9 Loại trừ");
        }

        /// <summary>Bảng chỉ số 2 cặp mỗi dòng: nhãn · ô · đơn vị (nếu <paramref name="withUnit"/>).</summary>
        private void ValueTable(LayoutControlGroup g, string[][] rows, bool withUnit, int labelW)
        {
            int[] cols = withUnit
                ? new[] { labelW, -50, 56, 16, labelW, -50, 56 }
                : new[] { labelW, -50, 16, labelW, -50 };
            int per = withUnit ? 3 : 2;
            LayoutControlGroup t = Table(g, cols, Rows((rows.Length + 1) / 2, ROW_H + 2));
            for (int i = 0; i < rows.Length; i++)
            {
                int r = i / 2, c = (i % 2) * (per + 1);
                CellText(t, rows[i][0] + ":", r, c);
                Cell(t, Txt(rows[i][1]), r, c + 1);
                if (withUnit && !string.IsNullOrEmpty(rows[i][2]))
                {
                    SimpleLabelItem u = CellText(t, rows[i][2], r, c + 2);
                    u.AppearanceItemCaption.ForeColor = Color.DimGray;
                    u.AppearanceItemCaption.Options.UseForeColor = true;
                }
            }
        }

        /// <summary>Tab con 4 — cận lâm sàng: trái XN bắt buộc, XN nữ, CĐHA, XN khác; phải XN máu, nước tiểu.</summary>
        private void BuildCls(LayoutControlGroup root)
        {
            LayoutControlGroup c1 = Column(root, null);
            LayoutControlGroup c2 = Column(root, c1);

            // Dòng nút nối chỉ số đặt đầu cột trái (đặt ở root sẽ làm 2 cột lệch theo độ rộng chú thích / nút).
            SimpleButton btnMap = new SimpleButton();
            btnMap.Text = "Nối chỉ số CLS…";
            btnMap.ToolTip = "Nối chỉ số xét nghiệm / dịch vụ của viện với từng ô dưới đây. Sau đó tích \"Tự động lấy kết quả xét nghiệm\" để điền tự động.";
            btnMap.Click += (s, e) => { if (ClsMapRequested != null) ClsMapRequested(this, EventArgs.Empty); };
            LayoutControlItem itMap = Add(c1, null, btnMap, height: 26, width: 140);
            Hint(c1, "Nối chỉ số xét nghiệm / dịch vụ của viện với từng ô, sau đó tích \"Tự động lấy kết quả CLS\" để điền tự động.", itMap);

            LayoutControlGroup g = Box(c1, "1. CÁC XÉT NGHIỆM BẮT BUỘC");
            Hint(g, "a) Ma túy (sàng lọc và định tính):");
            ValueTable(g, new[] { new[] { "Amphetamin", "TEST_AMPHETAMIN" }, new[] { "Marijuana", "TEST_MARIJUANA" }, new[] { "Morphin", "TEST_MORPHIN" },
                                  new[] { "Codein", "TEST_CODEIN" }, new[] { "Heroin", "TEST_HEROIN" } }, false, 100);
            Add(g, "b) Nồng độ cồn (máu / hơi thở):", Txt("TEST_ALCOHOL"), 180);
            Add(g, "Người thực hiện (ma túy, cồn):", Login("TEST_DRUG_LOGINNAME"), 180);

            g = Box(c1, "RIÊNG NỮ (Phụ lục XXV Thông tư 32/2023/TT-BYT)");
            MemoEdit memCervical = Memo("TEST_CERVICAL");
            Add(g, "Tế bào cổ tử cung / HPV:", memCervical, 180, height: 40);
            MemoEdit memBreast = Memo("TEST_BREAST");
            Add(g, "X-quang nhũ / Siêu âm vú:", memBreast, 180, height: 40);
            GridLookUpEdit cboFemale = Login("TEST_FEMALE_LOGINNAME");
            Add(g, "Người thực hiện (XN nữ):", cboFemale, 180);
            femaleOnly.Add(memCervical);
            femaleOnly.Add(memBreast);
            femaleOnly.Add(cboFemale);

            g = Box(c1, "2c. KẾT QUẢ CHẨN ĐOÁN HÌNH ẢNH (khi có chỉ định)");
            Add(g, null, Memo("RESULT_DIIM"), height: 52);
            Add(g, "Người thực hiện (CĐHA):", Login("DIIM_LOGINNAME"), 180);

            g = Box(c1, "2d. CÁC XÉT NGHIỆM KHÁC (khi có chỉ định)");
            Add(g, null, Memo("RESULT_OTHER_TEST"), height: 52);
            Add(g, "Người thực hiện (XN khác):", Login("OTHER_TEST_LOGINNAME"), 180);

            g = Box(c2, "2a. XÉT NGHIỆM MÁU (khi có chỉ định của bác sĩ khám sức khỏe)");
            Hint(g, "Huyết học — tổng phân tích tế bào máu ngoại vi:");
            ValueTable(g, new[] {
                new[] { "Số lượng HC", "TEST_BLOOD_HC", "T/L" }, new[] { "Huyết sắc tố", "TEST_BLOOD_HGB", "g/L" },
                new[] { "Hematocrit", "TEST_BLOOD_HCT", "L/L" }, new[] { "MCV", "TEST_BLOOD_MCV", "fL" },
                new[] { "MCH", "TEST_BLOOD_MCH", "pg" }, new[] { "MCHC", "TEST_BLOOD_MCHC", "g/L" },
                new[] { "RDW", "TEST_BLOOD_RDW", "%" }, new[] { "Số lượng bạch cầu", "TEST_BLOOD_BC", "G/L" },
                new[] { "BC trung tính", "TEST_BLOOD_NEUT", "G/L" }, new[] { "BC lympho", "TEST_BLOOD_LYMPH", "G/L" },
                new[] { "BC đơn nhân", "TEST_BLOOD_MONO", "G/L" }, new[] { "BC ái toan", "TEST_BLOOD_EOS", "G/L" },
                new[] { "BC ái kiềm", "TEST_BLOOD_BASO", "G/L" }, new[] { "Số lượng tiểu cầu", "TEST_BLOOD_TC", "G/L" } }, true, 115);
            Hint(g, "Sinh hóa máu:");
            ValueTable(g, new[] { new[] { "Đường máu", "TEST_BLOOD_GLUCO", "mmol/L" }, new[] { "Creatinin", "TEST_BLOOD_CREATININ", "µmol/L" },
                                  new[] { "ASAT (GOT)", "TEST_BLOOD_ASAT", "U/L" }, new[] { "ALAT (GPT)", "TEST_BLOOD_ALAT", "U/L" } }, true, 115);
            Add(g, "Người thực hiện (XN máu):", Login("TEST_BLOOD_LOGINNAME"), 180);

            g = Box(c2, "2b. XÉT NGHIỆM NƯỚC TIỂU — tổng phân tích nước tiểu (máy tự động)");
            ValueTable(g, new[] {
                new[] { "Tỉ trọng", "TEST_URINE_SG", "" }, new[] { "pH", "TEST_URINE_PH", "" },
                new[] { "Bạch cầu", "TEST_URINE_LEU", "Leu/µL" }, new[] { "Hồng cầu", "TEST_URINE_ERY", "Ery/µL" },
                new[] { "Nitrit", "TEST_URINE_NITRIT", "" }, new[] { "Protein", "TEST_URINE_PROTEIN", "g/L" },
                new[] { "Glucose", "TEST_URINE_GLUCO", "mmol/L" }, new[] { "Thể cetonic", "TEST_URINE_KETONE", "mmol/L" },
                new[] { "Bilirubin", "TEST_URINE_BILIRUBIN", "µmol/L" }, new[] { "Urobilinogen", "TEST_URINE_UBG", "µmol/L" } }, true, 115);
            Add(g, "Khác (nếu có):", Txt("TEST_URINE_OTHER"), 180);
            Add(g, "Người thực hiện (XN nước tiểu):", Login("TEST_URINE_LOGINNAME"), 180);

            EndColumn(c1);
            EndColumn(c2);
        }

        /// <summary>Tab con 5 — trái: kết luận lái xe, bệnh tật; phải: phân loại sức khỏe, đề nghị, người kết luận.</summary>
        private void BuildConclusion(LayoutControlGroup root)
        {
            LayoutControlGroup c1 = Column(root, null);
            LayoutControlGroup c2 = Column(root, c1);

            LayoutControlGroup g = Box(c1, "1. KẾT LUẬN — TÌNH TRẠNG SỨC KHỎE ĐỂ LÁI XE");
            rdoConclusion = Rdo("DRIVER_CONCLUSION_TYPE", 1,
                1, "Đủ điều kiện sức khỏe lái xe/điều khiển xe máy chuyên dùng hạng (ghi rõ hạng giấy phép lái xe)",
                2, "Không đủ điều kiện sức khỏe lái xe/điều khiển xe máy chuyên dùng hạng (ghi rõ hạng giấy phép lái xe)",
                3, "Đạt tiêu chuẩn sức khỏe lái xe/điều khiển xe máy chuyên dùng hạng (ghi rõ hạng) nhưng yêu cầu khám lại",
                4, "Đủ điều kiện sức khỏe lái xe hạng A1 đối với người khuyết tật điều khiển xe mô tô ba bánh dùng cho người khuyết tật",
                5, "Đủ điều kiện sức khỏe lái xe hạng B đối với người khuyết tật điều khiển xe ô tô số tự động có kết cấu phù hợp");
            rdoConclusion.EditValueChanged += (s, e) => RefreshState();
            Add(g, null, rdoConclusion, height: 5 * 20 + 10);
            cboLicenseClass.EditValueChanged += (s, e) => UpdateConclusionLicenseLabel();
            lblConclusionLicense = new LabelControl();
            lblConclusionLicense.AutoSizeMode = LabelAutoSizeMode.None;
            lblConclusionLicense.Appearance.ForeColor = Color.FromArgb(0, 51, 153);
            Add(g, null, lblConclusionLicense, height: 22);
            txtReexam = Txt("REEXAM_NOTE");
            Add(g, "Thời gian khám lại:", txtReexam, 120);

            g = Box(c1, "2. CÁC BỆNH, TẬT (nếu có)");
            CheckEdit chkNormal = Chk("CONCLUSION_IS_NORMAL", "Chưa phát hiện bất thường");
            chkNormal.CheckedChanged += (s, e) => RefreshState();
            Add(g, null, chkNormal, height: ROW_H);
            Add(g, "Chẩn đoán sơ bộ (ICD):", Icd("CONCLUSION_PRE_ICD"), TXT_W, height: ROW_H);
            Add(g, "Chẩn đoán xác định (ICD):", Icd("CONCLUSION_ICD"), TXT_W, height: ROW_H);

            g = Box(c2, "3. PHÂN LOẠI SỨC KHỎE");
            LayoutControlItem it = Add(g, "Phân loại:", Rank("HEALTH_EXAM_RANK_ID"), 120);
            Filler(g, it);

            g = Box(c2, "4. ĐỀ NGHỊ");
            rdoSuggest = Rdo("SUGGEST_TYPE", 1,
                1, "Bình thường, hẹn khám định kỳ lần sau",
                2, "Có yếu tố nguy cơ, cần theo dõi thêm",
                3, "Đã có bệnh mạn tính, tiếp tục điều trị theo phác đồ/toa cũ",
                4, "Chuyển cơ sở KBCB, khám chuyên khoa",
                5, "Khác, ghi rõ (nếu có tư vấn chế độ ăn uống, tập luyện…)");
            rdoSuggest.EditValueChanged += (s, e) => RefreshState();
            Add(g, null, rdoSuggest, height: 5 * 20 + 10);
            memSuggestNote = Memo("SUGGEST_NOTE");
            Add(g, "Ghi rõ:", memSuggestNote, 120, height: 44);

            g = Box(c2, "NGƯỜI KẾT LUẬN");
            cboConcluder = Gle(null);
            it = Add(g, "Người kết luận:", cboConcluder, 120);
            dteConclusionTime = new DateEdit();
            dteConclusionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dteConclusionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            dteConclusionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dteConclusionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            dteConclusionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            dteConclusionTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            Add(g, "Ngày kết luận:", dteConclusionTime, 90, it);

            EndColumn(c1);
            EndColumn(c2);
        }

        #endregion

        #region ===== Tiền sử bản thân (21 câu theo danh mục) =====

        /// <summary>
        /// Số câu trên giấy Mẫu 06 của 1 mục danh mục bệnh: mã "01".."21" = câu 1..21 (giấy Mẫu 06 dùng đúng
        /// thứ tự mã danh mục). Mã khác trả 0 (không thuộc 21 câu).
        /// </summary>
        public static int QuestionNo(HIS_DISEASE_TYPE d)
        {
            int n;
            return (d != null && int.TryParse((d.DISEASE_TYPE_CODE ?? "").Trim(), out n) && n >= 1 && n <= 21) ? n : 0;
        }

        /// <summary>
        /// Dựng các câu theo danh mục bệnh mã 01–21 (đã sắp theo số câu), mỗi câu 1 dòng: nội dung · Có / Không;
        /// số câu theo mã. Gọi mỗi lần nạp tab — chỉ dựng lại nếu danh mục khác lần trước.
        /// </summary>
        public void SetDiseaseCatalog(List<HIS_DISEASE_TYPE> diseases)
        {
            try
            {
                List<HIS_DISEASE_TYPE> list = diseases ?? new List<HIS_DISEASE_TYPE>();
                if (dicDisease.Count == list.Count && list.All(o => dicDisease.ContainsKey(o.ID))) return;
                lcAdmin.BeginUpdate();
                try
                {
                    foreach (BaseLayoutItem old in diseaseItems)
                    {
                        grpDiseases.Remove(old);
                        old.Dispose();
                    }
                    foreach (RadioGroup old in dicDisease.Values) old.Dispose();
                    diseaseItems.Clear();
                    diseaseLabels.Clear();
                    dicDisease.Clear();
                    for (int i = 0; i < list.Count; i++)
                    {
                        int no = QuestionNo(list[i]);
                        SimpleLabelItem lbl = new SimpleLabelItem();
                        lbl.Text = (no > 0 ? no : i + 1) + ". " + list[i].DISEASE_TYPE_NAME;
                        lbl.OptionsToolTip.ToolTip = lbl.Text;
                        lbl.TextAlignMode = TextAlignModeItem.CustomSize;
                        lbl.TextSize = new Size(120, 20);
                        grpDiseases.AddItem(lbl);
                        RadioGroup r = new RadioGroup();
                        r.Properties.AllowFocused = false;
                        r.Properties.Columns = 2;
                        r.Properties.Items.Add(new RadioGroupItem(1, "Có"));
                        r.Properties.Items.Add(new RadioGroupItem(0, "Không"));
                        r.EditValue = null;
                        NoWheel(r);
                        LayoutControlItem ri = Add(grpDiseases, null, r, rightOf: lbl, height: ROW_H, width: 140);
                        diseaseItems.Add(lbl);
                        diseaseItems.Add(ri);
                        diseaseLabels.Add(lbl);
                        dicDisease[list[i].ID] = r;
                    }
                }
                finally { lcAdmin.EndUpdate(); }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đổ câu trả lời. <paramref name="defaultNo"/> = true (hồ sơ mới): câu chưa có câu trả lời mặc định
        /// "Không" — cùng hành vi lưới tiền sử của tab Ksk lái xe.
        /// </summary>
        public void SetDiseaseAnswers(List<HIS_PERIOD_DRIVER_DITY> ditys, bool defaultNo)
        {
            try
            {
                foreach (var kv in dicDisease)
                {
                    var d = ditys != null ? ditys.FirstOrDefault(o => o != null && o.DISEASE_TYPE_ID == kv.Key) : null;
                    if (d != null && d.IS_YES_NO == "1") kv.Value.EditValue = 1;
                    else if (d != null && d.IS_YES_NO == "0") kv.Value.EditValue = 0;
                    else kv.Value.EditValue = defaultNo ? (object)0 : null;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Câu trả lời đã chọn (bỏ câu chưa chọn) — form gửi kèm khi Lưu.</summary>
        public List<HIS_PERIOD_DRIVER_DITY> GetDiseaseAnswers()
        {
            List<HIS_PERIOD_DRIVER_DITY> rs = new List<HIS_PERIOD_DRIVER_DITY>();
            try
            {
                foreach (var kv in dicDisease)
                {
                    if (kv.Value.EditValue == null || kv.Value.EditValue == DBNull.Value) continue;
                    HIS_PERIOD_DRIVER_DITY d = new HIS_PERIOD_DRIVER_DITY();
                    d.DISEASE_TYPE_ID = kv.Key;
                    d.IS_YES_NO = Convert.ToInt32(kv.Value.EditValue) == 1 ? "1" : "0";
                    rs.Add(d);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return rs;
        }

        #endregion

        #region ===== Hành vi khóa / mở =====

        /// <summary>Bệnh nhân nữ mới nhập được sản phụ khoa, XN riêng cho nữ, tiền sử thai sản.</summary>
        public void SetFemale(bool female)
        {
            isFemale = female;
            RefreshState();
        }

        /// <summary>Đặt lại toàn bộ trạng thái khóa/mở theo giá trị đang có trên các ô.</summary>
        public void RefreshState()
        {
            if (applyingState) return;
            applyingState = true;
            try
            {
                // Mục dành cho nữ
                foreach (Control c in femaleOnly)
                {
                    c.Enabled = isFemale;
                    if (!isFemale) ClearControl(c);
                }
                bool refuse = isFemale && IsChecked("IS_REFUSE_OBSTETRIC");
                foreach (string[] s in SECTIONS)
                {
                    string pre = s[0];
                    bool sectionOn = pre != "EXAM_OBSTETRIC" || (isFemale && !refuse);
                    bool normal = IsChecked(pre + "_IS_NORMAL");
                    UcKskHistoryIcd ucPre = dicIcd[pre + "_PRE_ICD"];
                    UcKskHistoryIcd ucFin = dicIcd[pre + "_ICD"];
                    if (normal || !sectionOn) { ucPre.SetData(null, null); ucFin.SetData(null, null); }
                    ucPre.SetReadOnly(normal || !sectionOn);
                    ucFin.SetReadOnly(normal || !sectionOn);
                    if (pre == "EXAM_OBSTETRIC")
                    {
                        foreach (string col in new[] { pre + "_IS_NORMAL", pre + "_RANK", pre + "_CONCLUDE" })
                        {
                            BaseEdit be = dicField[col];
                            be.Enabled = sectionOn;
                            if (!sectionOn) ClearControl(be);
                        }
                    }
                }
                // Bệnh tật trong kết luận
                bool cNormal = IsChecked("CONCLUSION_IS_NORMAL");
                if (cNormal) { dicIcd["CONCLUSION_PRE_ICD"].SetData(null, null); dicIcd["CONCLUSION_ICD"].SetData(null, null); }
                dicIcd["CONCLUSION_PRE_ICD"].SetReadOnly(cNormal);
                dicIcd["CONCLUSION_ICD"].SetReadOnly(cNormal);
                // Kết luận lái xe: trường hợp 3 mới ghi thời gian khám lại
                int type = RadioInt(rdoConclusion);
                bool reexam = type == 3;
                if (!reexam && txtReexam.Enabled) txtReexam.Text = null;
                txtReexam.Enabled = reexam;
                UpdateConclusionLicenseLabel();
                // Đề nghị "Khác" mới ghi rõ
                bool other = RadioInt(rdoSuggest) == 5;
                if (!other && memSuggestNote.Enabled) memSuggestNote.Text = null;
                memSuggestNote.Enabled = other;
                ApplyYesNoState(rdoFamily, dicField["FAMILY_HISTORY"]);
                ApplyYesNoState(rdoTreating, dicField["TREATING_MEDICINE"]);
                if (isFemale) ApplyYesNoState(rdoMaternity, dicField["MATERNITY_HISTORY"]);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { applyingState = false; }
        }

        /// <summary>Ô tên bệnh / thuốc chỉ nhập được khi chọn "Có"; chọn "Không" thì xóa và khóa.</summary>
        private void ApplyYesNoState(RadioGroup rdo, BaseEdit detail)
        {
            try
            {
                bool yes = RadioInt(rdo) == 1;
                if (!yes) detail.EditValue = null;
                detail.Enabled = yes && rdo.Enabled;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Hạng GPLX ghi trong kết luận = hạng đề nghị khám (1 ô Hạng duy nhất, như tab Ksk lái xe).</summary>
        public void UpdateConclusionLicenseLabel()
        {
            try
            {
                int type = RadioInt(rdoConclusion);
                string cls = cboLicenseClass != null ? cboLicenseClass.Text : "";
                lblConclusionLicense.Text = (type >= 1 && type <= 3)
                    ? "Hạng giấy phép lái xe ghi trong kết luận: " + (string.IsNullOrWhiteSpace(cls) ? "(chưa chọn — chọn ở tab Hành chính - Tiền sử)" : cls)
                    : "";
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void UpdateBmi()
        {
            try
            {
                if (HasSpin(spnHeight) && HasSpin(spnWeight) && spnHeight.Value > 0)
                {
                    decimal h = spnHeight.Value / 100m;
                    txtBmi.Text = Math.Round(spnWeight.Value / (h * h), 1).ToString("0.#", CultureInfo.InvariantCulture);
                }
                else txtBmi.Text = null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private bool IsChecked(string column)
        {
            BaseEdit be;
            return dicField.TryGetValue(column, out be) && be is CheckEdit && ((CheckEdit)be).Checked;
        }

        private static int RadioInt(RadioGroup rdo)
        {
            if (rdo == null || rdo.EditValue == null || rdo.EditValue == DBNull.Value) return 0;
            return Convert.ToInt32(rdo.EditValue);
        }

        private static void ClearControl(Control c)
        {
            if (c is CheckEdit) ((CheckEdit)c).Checked = false;
            else if (c is UcKskHistoryIcd) ((UcKskHistoryIcd)c).SetData(null, null);
            else if (c is BaseEdit) ((BaseEdit)c).EditValue = null;
        }

        private static bool HasSpin(SpinEdit spn)
        {
            return spn.EditValue != null && spn.EditValue != DBNull.Value;
        }

        #endregion

        #region ===== Nạp / lấy dữ liệu =====

        /// <summary>Phần hành chính chỉ đọc (lấy từ hồ sơ bệnh nhân / điều trị).</summary>
        public void SetAdministrativeInfo(string ethnic, string blood, string heinCard, string residence, string career, string workPlace, string phone)
        {
            txtEthnic.Text = ethnic;
            txtBlood.Text = blood;
            txtHeinCard.Text = heinCard;
            txtResidence.Text = residence;
            txtCareer.Text = career;
            txtWorkPlace.Text = workPlace;
            txtPhone.Text = phone;
        }

        /// <summary>
        /// Đổ dữ liệu đã lưu lên các ô. <paramref name="data"/> null = hồ sơ mới (để trống);
        /// <paramref name="dhst"/> null = không có số đo thể lực. Đối tượng / Nguồn chi trả / Người kết luận do form đổ.
        /// </summary>
        public void FillData(HIS_KSK_PRO_DRIVER data, HIS_DHST dhst)
        {
            try
            {
                applyingState = true;
                foreach (var kv in dicField)
                    SetEditorValue(kv.Value, data != null ? GetProp(data, kv.Key) : null);
                foreach (var kv in dicIcd)
                    kv.Value.SetData(data != null ? GetProp(data, kv.Key + "_CODE") as string : null,
                                     data != null ? GetProp(data, kv.Key + "_NAME") as string : null);
                spnHeight.EditValue = (dhst != null && dhst.HEIGHT.HasValue) ? (object)dhst.HEIGHT.Value : null;
                spnWeight.EditValue = (dhst != null && dhst.WEIGHT.HasValue) ? (object)dhst.WEIGHT.Value : null;
                spnPulse.EditValue = (dhst != null && dhst.PULSE.HasValue) ? (object)(decimal)dhst.PULSE.Value : null;
                spnBpMax.EditValue = (dhst != null && dhst.BLOOD_PRESSURE_MAX.HasValue) ? (object)(decimal)dhst.BLOOD_PRESSURE_MAX.Value : null;
                spnBpMin.EditValue = (dhst != null && dhst.BLOOD_PRESSURE_MIN.HasValue) ? (object)(decimal)dhst.BLOOD_PRESSURE_MIN.Value : null;
                spnBreathRate.EditValue = (dhst != null && dhst.BREATH_RATE.HasValue) ? (object)dhst.BREATH_RATE.Value : null;
                cboDhstExecutor.EditValue = (dhst != null && !string.IsNullOrEmpty(dhst.EXECUTE_LOGINNAME)) ? dhst.EXECUTE_LOGINNAME : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { applyingState = false; }
            UpdateBmi();
            RefreshState();
        }

        /// <summary>Ghi giá trị các ô vào entity trước khi lưu (không đụng ID / liên kết / số giấy / đối tượng / nguồn chi trả).</summary>
        public void GetData(HIS_KSK_PRO_DRIVER data)
        {
            if (data == null) return;
            try
            {
                foreach (var kv in dicField)
                    SetProp(data, kv.Key, GetEditorValue(kv.Value));
                foreach (var kv in dicIcd)
                {
                    SetProp(data, kv.Key + "_CODE", kv.Value.GetCodes());
                    SetProp(data, kv.Key + "_NAME", kv.Value.GetNames());
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ghi số đo thể lực vào HIS_DHST — không đụng ID / người đo.</summary>
        public void GetDhst(HIS_DHST dhst)
        {
            if (dhst == null) return;
            try
            {
                dhst.HEIGHT = HasSpin(spnHeight) ? (decimal?)Math.Round(spnHeight.Value, 2) : null;
                dhst.WEIGHT = HasSpin(spnWeight) ? (decimal?)Math.Round(spnWeight.Value, 2) : null;
                dhst.PULSE = HasSpin(spnPulse) ? (long?)Convert.ToInt64(spnPulse.Value) : null;
                dhst.BLOOD_PRESSURE_MAX = HasSpin(spnBpMax) ? (long?)Convert.ToInt64(spnBpMax.Value) : null;
                dhst.BLOOD_PRESSURE_MIN = HasSpin(spnBpMin) ? (long?)Convert.ToInt64(spnBpMin.Value) : null;
                dhst.BREATH_RATE = HasSpin(spnBreathRate) ? (decimal?)Math.Round(spnBreathRate.Value, 2) : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Điền ô theo tên cột khi ô đang mở và còn trống (lấy kết quả tự động không đè số liệu đã nhập).</summary>
        public bool SetFieldTextIfEmpty(string column, string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value)) return false;
                BaseEdit be = GetFieldEditor(column);
                if (be == null || !be.Enabled || !(be is TextEdit) || be is GridLookUpEdit || be is ComboBoxEdit) return false;
                if (!string.IsNullOrWhiteSpace(be.Text)) return false;
                be.Text = value;
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        private static object GetEditorValue(BaseEdit be)
        {
            if (be is CheckEdit) return ((CheckEdit)be).Checked ? (object)(short)1 : null;
            if (be is RadioGroup) return (be.EditValue == null || be.EditValue == DBNull.Value) ? null : (object)Convert.ToInt16(be.EditValue);
            if (be is GridLookUpEdit) return (be.EditValue == null || be.EditValue == DBNull.Value) ? null : be.EditValue;
            if (be is ComboBoxEdit)
            {
                string t = be.Text;
                short n;
                return (!string.IsNullOrWhiteSpace(t) && short.TryParse(t.Trim(), out n)) ? (object)n : null;
            }
            string s = be.Text;
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        private static void SetEditorValue(BaseEdit be, object value)
        {
            if (be is CheckEdit) { ((CheckEdit)be).Checked = value != null && Convert.ToInt64(value) == 1; return; }
            if (be is RadioGroup) { be.EditValue = value == null ? null : (object)Convert.ToInt32(value); return; }
            if (be is GridLookUpEdit) { be.EditValue = value; return; }
            if (be is ComboBoxEdit) { be.EditValue = value != null ? value.ToString() : ""; return; }
            be.EditValue = value != null ? value.ToString() : null;
        }

        private static object GetProp(object obj, string name)
        {
            var pi = obj.GetType().GetProperty(name);
            return (pi != null && pi.CanRead) ? pi.GetValue(obj, null) : null;
        }

        /// <summary>Gán giá trị vào thuộc tính theo tên, đổi kiểu về đúng kiểu cột (string / short? / long?).</summary>
        private static void SetProp(object obj, string name, object value)
        {
            var pi = obj.GetType().GetProperty(name);
            if (pi == null || !pi.CanWrite) return;
            Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
            if (value == null || value == DBNull.Value) { pi.SetValue(obj, null, null); return; }
            if (t == typeof(string))
            {
                string s = value.ToString().Trim();
                pi.SetValue(obj, s.Length == 0 ? null : s, null);
                return;
            }
            if (value is string && string.IsNullOrWhiteSpace((string)value)) { pi.SetValue(obj, null, null); return; }
            pi.SetValue(obj, Convert.ChangeType(value, t, CultureInfo.InvariantCulture), null);
        }

        #endregion
    }
}
