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
    ///
    /// Nạp / lấy dữ liệu đi qua BẢNG NỐI tên cột HIS_KSK_PRO_DRIVER -> ô nhập (dicField / dicIcd), đọc ghi
    /// bằng phản chiếu: hơn 200 cột, viết tay từng dòng gán cho cả 2 chiều rất dễ sót.
    /// Các ô dùng danh mục chung (Đối tượng, Nguồn chi trả, Phân loại, Người khám, Hạng GPLX, Người kết luận)
    /// do form khởi tạo nguồn — cùng cơ chế với các tab khác.
    /// </summary>
    public class UcKskProDriverM6 : XtraUserControl
    {
        #region ===== Khai báo =====

        private const int LBL_W = 150;
        private const int ROW_H = 26;
        private const int LEFT = 10;

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
        private XtraScrollableControl pnlAdmin, pnlPhysical, pnlClinical, pnlCls, pnlConclusion;
        private PanelControl pnlDiseases;
        private int diseaseTop;

        private TextEdit txtEthnic, txtBlood, txtHeinCard, txtResidence, txtCareer, txtWorkPlace, txtPhone;
        private GridLookUpEdit cboObject, cboPaymentSource, cboLicenseClass, cboConcluder;
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
                pnlAdmin = AddPage("Hành chính - Tiền sử");
                pnlPhysical = AddPage("Khám thể lực");
                pnlClinical = AddPage("Khám lâm sàng");
                pnlCls = AddPage("Cận lâm sàng");
                pnlConclusion = AddPage("Kết luận");
                BuildAdmin();
                BuildPhysical();
                BuildClinical();
                BuildCls();
                BuildConclusion();
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

        #region ===== Dựng giao diện =====

        private XtraScrollableControl AddPage(string text)
        {
            XtraTabPage page = new XtraTabPage();
            page.Text = text;
            XtraScrollableControl sc = new XtraScrollableControl();
            sc.Dock = DockStyle.Fill;
            sc.AutoScroll = true;
            page.Controls.Add(sc);
            tabs.TabPages.Add(page);
            return sc;
        }

        private static LabelControl Lbl(Control parent, string text, int x, int y, int w, bool bold = false)
        {
            LabelControl l = new LabelControl();
            l.AutoSizeMode = LabelAutoSizeMode.None;
            l.Text = text;
            l.Location = new Point(x, y + 3);
            l.Size = new Size(w, 18);
            if (bold) l.Appearance.Font = new Font(l.Appearance.Font ?? SystemFonts.DefaultFont, FontStyle.Bold);
            parent.Controls.Add(l);
            return l;
        }

        private static LabelControl Title(Control parent, string text, ref int y)
        {
            y += 6;
            LabelControl l = Lbl(parent, text, LEFT, y, 900, true);
            l.Appearance.ForeColor = Color.FromArgb(0, 51, 153);
            y += ROW_H;
            return l;
        }

        private TextEdit Txt(Control parent, string column, int x, int y, int w, bool readOnly = false)
        {
            TextEdit t = new TextEdit();
            t.Location = new Point(x, y);
            t.Size = new Size(w, 20);
            t.Properties.ReadOnly = readOnly;
            parent.Controls.Add(t);
            if (!string.IsNullOrEmpty(column)) dicField[column] = t;
            return t;
        }

        private MemoEdit Memo(Control parent, string column, int x, int y, int w, int h)
        {
            MemoEdit m = new MemoEdit();
            m.Location = new Point(x, y);
            m.Size = new Size(w, h);
            parent.Controls.Add(m);
            if (!string.IsNullOrEmpty(column)) dicField[column] = m;
            return m;
        }

        private CheckEdit Chk(Control parent, string column, string text, int x, int y, int w)
        {
            CheckEdit c = new CheckEdit();
            c.Text = text;
            c.Location = new Point(x, y);
            c.Size = new Size(w, 20);
            parent.Controls.Add(c);
            if (!string.IsNullOrEmpty(column)) dicField[column] = c;
            return c;
        }

        private RadioGroup Rdo(Control parent, string column, int x, int y, int w, int h, params object[] valueText)
        {
            RadioGroup r = new RadioGroup();
            r.Location = new Point(x, y);
            r.Size = new Size(w, h);
            r.Properties.AllowFocused = false;
            for (int i = 0; i + 1 < valueText.Length; i += 2)
                r.Properties.Items.Add(new RadioGroupItem(valueText[i], (string)valueText[i + 1]));
            r.EditValue = null;
            parent.Controls.Add(r);
            if (!string.IsNullOrEmpty(column)) dicField[column] = r;
            return r;
        }

        private GridLookUpEdit Gle(Control parent, string column, int x, int y, int w)
        {
            GridLookUpEdit g = new GridLookUpEdit();
            g.Location = new Point(x, y);
            g.Size = new Size(w, 20);
            g.Properties.NullText = "";
            parent.Controls.Add(g);
            if (!string.IsNullOrEmpty(column)) dicField[column] = g;
            return g;
        }

        private GridLookUpEdit Rank(Control parent, string column, int x, int y, int w)
        {
            GridLookUpEdit g = Gle(parent, column, x, y, w);
            rankCombos.Add(g);
            return g;
        }

        private GridLookUpEdit Login(Control parent, string column, int x, int y, int w)
        {
            GridLookUpEdit g = Gle(parent, column, x, y, w);
            loginCombos.Add(g);
            return g;
        }

        private SpinEdit Spin(Control parent, int x, int y, int w, bool isFloat, decimal max)
        {
            SpinEdit s = new SpinEdit();
            s.Location = new Point(x, y);
            s.Size = new Size(w, 20);
            s.Properties.IsFloatValue = isFloat;
            s.Properties.MinValue = 0;
            s.Properties.MaxValue = max;
            s.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            s.EditValue = null;
            parent.Controls.Add(s);
            return s;
        }

        private UcKskHistoryIcd Icd(Control parent, string prefix, int x, int y, int w)
        {
            UcKskHistoryIcd uc = new UcKskHistoryIcd();
            uc.Location = new Point(x, y);
            uc.Size = new Size(w, 20);
            parent.Controls.Add(uc);
            dicIcd[prefix] = uc;
            return uc;
        }

        /// <summary>Tab con 1 — hành chính bổ sung, đối tượng - chi trả, tiền sử.</summary>
        private void BuildAdmin()
        {
            Control p = pnlAdmin;
            int y = 4;
            Title(p, "THÔNG TIN HÀNH CHÍNH (bổ sung — họ tên, ngày sinh, giới tính, CCCD ở khối thông tin bệnh nhân phía trên)", ref y);
            Lbl(p, "Dân tộc:", LEFT, y, 100); txtEthnic = Txt(p, null, LEFT + 100, y, 160, true);
            Lbl(p, "Nhóm máu:", 290, y, 80); txtBlood = Txt(p, null, 370, y, 100, true);
            Lbl(p, "Số thẻ BHYT:", 500, y, 90); txtHeinCard = Txt(p, null, 590, y, 220, true);
            y += ROW_H;
            Lbl(p, "Nơi ở hiện tại:", LEFT, y, 100); txtResidence = Txt(p, null, LEFT + 100, y, 700, true);
            y += ROW_H;
            Lbl(p, "Nghề nghiệp:", LEFT, y, 100); txtCareer = Txt(p, null, LEFT + 100, y, 260, true);
            Lbl(p, "Nơi làm việc:", 390, y, 90); txtWorkPlace = Txt(p, null, 480, y, 330, true);
            y += ROW_H;
            Lbl(p, "Xã, phường (nơi làm việc):", LEFT, y, 170); Txt(p, "WORK_COMMUNE_NAME", LEFT + 170, y, 300);
            Lbl(p, "Điện thoại:", 500, y, 90); txtPhone = Txt(p, null, 590, y, 220, true);
            y += ROW_H;
            Lbl(p, "Hạng giấy phép lái xe:", LEFT, y, 170); cboLicenseClass = Gle(p, "LICENSE_CLASS_ID", LEFT + 170, y, 300);
            y += ROW_H;

            Title(p, "THÔNG TIN ĐỐI TƯỢNG - CHI TRẢ", ref y);
            Lbl(p, "Đối tượng:", LEFT, y, 100); cboObject = Gle(p, null, LEFT + 100, y, 710);
            y += ROW_H;
            Lbl(p, "Nguồn chi trả:", LEFT, y, 100); cboPaymentSource = Gle(p, null, LEFT + 100, y, 300);
            Lbl(p, "Địa điểm khám:", 440, y, 100);
            Rdo(p, "EXAM_PLACE_TYPE", 540, y - 2, 270, 24, 1, "Cơ sở KBCB", 2, "Khám lưu động");
            y += ROW_H + 2;

            Title(p, "TIỀN SỬ BỆNH CỦA ĐỐI TƯỢNG KHÁM SỨC KHỎE", ref y);
            Lbl(p, "1. Tiền sử gia đình (truyền nhiễm, tim mạch, đái tháo đường, lao, hen phế quản, ung thư, động kinh, rối loạn tâm thần, bệnh khác):", LEFT, y, 820);
            y += ROW_H - 4;
            rdoFamily = Rdo(p, "IS_FAMILY_HISTORY", LEFT + 20, y - 2, 140, 24, 0, "Không", 1, "Có");
            Lbl(p, "Ghi cụ thể tên bệnh:", 180, y, 130); TextEdit txtFamily = Txt(p, "FAMILY_HISTORY", 310, y, 500);
            rdoFamily.EditValueChanged += (s, e) => ApplyYesNoState(rdoFamily, txtFamily);
            y += ROW_H + 2;

            Lbl(p, "2. Tiền sử bệnh, tật của bản thân — ông (bà) đã/đang mắc bệnh, tình trạng nào sau đây:", LEFT, y, 820);
            y += ROW_H - 4;
            pnlDiseases = new PanelControl();
            pnlDiseases.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlDiseases.Location = new Point(LEFT, y);
            pnlDiseases.Size = new Size(830, 40);
            p.Controls.Add(pnlDiseases);
            diseaseTop = y;
            y += 44;
            Lbl(p, "22. Bệnh khác (ghi rõ):", LEFT + 20, y, 160, false).Name = "lblOtherDisease";
            Txt(p, "OTHER_DISEASE", LEFT + 180, y, 630).Name = "txtOtherDisease";
            y += ROW_H + 2;

            Lbl(p, "3a. Ông (bà) có đang điều trị bệnh gì không? Nếu có, liệt kê thuốc đang dùng và liều lượng:", LEFT, y, 820);
            y += ROW_H - 4;
            rdoTreating = Rdo(p, "IS_TREATING", LEFT + 20, y - 2, 140, 24, 0, "Không", 1, "Có");
            MemoEdit memTreating = Memo(p, "TREATING_MEDICINE", 180, y, 630, 40);
            rdoTreating.EditValueChanged += (s, e) => ApplyYesNoState(rdoTreating, memTreating);
            y += 46;

            Lbl(p, "3b. Tiền sử thai sản (đối với phụ nữ):", LEFT, y, 820);
            y += ROW_H - 4;
            rdoMaternity = Rdo(p, "IS_MATERNITY_HISTORY", LEFT + 20, y - 2, 140, 24, 0, "Không", 1, "Có");
            MemoEdit memMaternity = Memo(p, "MATERNITY_HISTORY", 180, y, 630, 40);
            rdoMaternity.EditValueChanged += (s, e) => ApplyYesNoState(rdoMaternity, memMaternity);
            femaleOnly.Add(rdoMaternity);
            femaleOnly.Add(memMaternity);
            y += 50;
            RelocateBelowDiseases();
        }

        /// <summary>Tab con 2 — khám thể lực.</summary>
        private void BuildPhysical()
        {
            Control p = pnlPhysical;
            int y = 4;
            Title(p, "KHÁM THỂ LỰC", ref y);
            Lbl(p, "Chiều cao (cm):", LEFT, y, 120); spnHeight = Spin(p, LEFT + 120, y, 100, true, 300);
            Lbl(p, "Mạch (lần/phút):", 300, y, 130); spnPulse = Spin(p, 430, y, 100, false, 500);
            y += ROW_H;
            Lbl(p, "Cân nặng (kg):", LEFT, y, 120); spnWeight = Spin(p, LEFT + 120, y, 100, true, 500);
            Lbl(p, "Huyết áp (mmHg):", 300, y, 130); spnBpMax = Spin(p, 430, y, 70, false, 400);
            Lbl(p, "/", 505, y, 10); spnBpMin = Spin(p, 515, y, 70, false, 400);
            y += ROW_H;
            Lbl(p, "Chỉ số BMI:", LEFT, y, 120); txtBmi = Txt(p, null, LEFT + 120, y, 100, true);
            Lbl(p, "Nhịp thở (lần/phút):", 300, y, 130); spnBreathRate = Spin(p, 430, y, 100, false, 200);
            y += ROW_H + 6;
            Lbl(p, "Phân loại thể lực:", LEFT, y, 120); Rank(p, "DHST_RANK", LEFT + 120, y, 200);
            y += ROW_H;
            Lbl(p, "Kết luận:", LEFT, y, 120); Txt(p, "DHST_CONCLUDE", LEFT + 120, y, 690);
            spnHeight.EditValueChanged += (s, e) => UpdateBmi();
            spnWeight.EditValueChanged += (s, e) => UpdateBmi();
        }

        /// <summary>Tab con 3 — 14 mục khám lâm sàng.</summary>
        private void BuildClinical()
        {
            Control p = pnlClinical;
            int y = 4;
            Title(p, "KHÁM LÂM SÀNG — kết luận từng chuyên khoa: đủ / không đủ tiêu chuẩn theo Phụ lục 1 Thông tư 36/2024/TT-BYT", ref y);
            foreach (string[] s in SECTIONS)
            {
                string pre = s[0];
                if (!string.IsNullOrEmpty(s[2])) Title(p, s[2], ref y);
                int indent = s[1].StartsWith("a)") || s[1].StartsWith("b)") || s[1].StartsWith("c)") || s[1].StartsWith("d)")
                    || s[1].StartsWith("đ)") || s[1].StartsWith("e)") || s[1].StartsWith("g)") || s[1].StartsWith("h)") ? 20 : 0;
                LabelControl head = Lbl(p, s[1], LEFT + indent, y, 300, true);
                List<Control> sectionCtrls = new List<Control>();

                if (pre == "EXAM_OBSTETRIC")
                {
                    CheckEdit chkRefuse = Chk(p, "IS_REFUSE_OBSTETRIC", "Từ chối khám sản phụ khoa", 330, y, 220);
                    chkRefuse.CheckedChanged += (sd, e) => RefreshState();
                    femaleOnly.Add(chkRefuse);
                }
                y += ROW_H - 2;

                if (pre == "EXAM_EYE")
                {
                    Lbl(p, "Thị lực", LEFT + 30, y, 100); Lbl(p, "Mắt phải", 170, y, 80); Lbl(p, "Mắt trái", 290, y, 80);
                    Lbl(p, "Khúc xạ (nếu có)", 430, y, 120); Lbl(p, "Mắt phải", 560, y, 80); Lbl(p, "Mắt trái", 680, y, 80);
                    y += 22;
                    string[][] eye = { new[] { "Không kính", "EXAM_EYESIGHT_RIGHT", "EXAM_EYESIGHT_LEFT", "Độ cầu", "EXAM_EYE_SPHERE_RIGHT", "EXAM_EYE_SPHERE_LEFT" },
                                       new[] { "Kính lỗ", "EXAM_EYESIGHT_PINHOLE_RIGHT", "EXAM_EYESIGHT_PINHOLE_LEFT", "Độ trụ", "EXAM_EYE_CYLINDER_RIGHT", "EXAM_EYE_CYLINDER_LEFT" },
                                       new[] { "Có kính", "EXAM_EYESIGHT_GLASS_RIGHT", "EXAM_EYESIGHT_GLASS_LEFT", "Trục", "EXAM_EYE_AXIS_RIGHT", "EXAM_EYE_AXIS_LEFT" } };
                    foreach (string[] r in eye)
                    {
                        Lbl(p, r[0], LEFT + 30, y, 120); Txt(p, r[1], 170, y, 110); Txt(p, r[2], 290, y, 110);
                        Lbl(p, r[3], 430, y, 120); Txt(p, r[4], 560, y, 110); Txt(p, r[5], 680, y, 110);
                        y += ROW_H;
                    }
                }
                else if (pre == "EXAM_ENT")
                {
                    Lbl(p, "Thính lực (m)", LEFT + 30, y, 120); Lbl(p, "Nói thường", 170, y, 100); Lbl(p, "Nói thầm", 290, y, 100);
                    y += 22;
                    Lbl(p, "Tai trái", LEFT + 30, y, 120); Txt(p, "EXAM_ENT_LEFT_NORMAL", 170, y, 110); Txt(p, "EXAM_ENT_LEFT_WHISPER", 290, y, 110);
                    y += ROW_H;
                    Lbl(p, "Tai phải", LEFT + 30, y, 120); Txt(p, "EXAM_ENT_RIGHT_NORMAL", 170, y, 110); Txt(p, "EXAM_ENT_RIGHT_WHISPER", 290, y, 110);
                    y += ROW_H;
                }
                else if (pre == "EXAM_STOMATOLOGY")
                {
                    y = BuildTeeth(p, y);
                }

                CheckEdit chkNormal = Chk(p, pre + "_IS_NORMAL", "Chưa phát hiện bất thường", LEFT + 30 + indent, y, 220);
                chkNormal.CheckedChanged += (sd, e) => RefreshState();
                y += ROW_H;
                Lbl(p, "Chẩn đoán sơ bộ (ICD):", LEFT + 30 + indent, y, 160); Icd(p, pre + "_PRE_ICD", 200 + indent, y, 610 - indent);
                y += ROW_H;
                Lbl(p, "Chẩn đoán xác định (ICD):", LEFT + 30 + indent, y, 160); Icd(p, pre + "_ICD", 200 + indent, y, 610 - indent);
                y += ROW_H;
                Lbl(p, "Phân loại:", LEFT + 30 + indent, y, 70); GridLookUpEdit rank = Rank(p, pre + "_RANK", 110 + indent, y, 130);
                Lbl(p, "Kết luận:", 260, y, 60); TextEdit conclude = Txt(p, pre + "_CONCLUDE", 320, y, 270);
                SimpleButton btnLib = new SimpleButton();
                btnLib.Text = "...";
                btnLib.Location = new Point(592, y);
                btnLib.Size = new Size(26, 20);
                btnLib.ToolTip = "Thư viện văn bản — cú pháp mẫu: nội dung;PL:Lx  (x = 1..5 tự chọn Phân loại)";
                btnLib.Click += (sd, e) => { if (ConclusionTextLibRequested != null) ConclusionTextLibRequested(conclude, rank); };
                p.Controls.Add(btnLib);
                Lbl(p, "Bác sĩ:", 628, y, 45); Login(p, pre + "_LOGINNAME", 673, y, 137);
                y += ROW_H + 8;

                if (pre == "EXAM_OBSTETRIC")
                {
                    femaleOnly.Add(chkNormal); femaleOnly.Add(rank); femaleOnly.Add(conclude); femaleOnly.Add(btnLib);
                    femaleOnly.Add(dicIcd[pre + "_PRE_ICD"]); femaleOnly.Add(dicIcd[pre + "_ICD"]); femaleOnly.Add(dicField[pre + "_LOGINNAME"]);
                }
            }
        }

        /// <summary>Sơ đồ 32 răng — mỗi răng 1 ô chọn mã 0–9 theo chú thích Mẫu 06.</summary>
        private int BuildTeeth(Control p, int y)
        {
            int x0 = LEFT + 100;
            int w = 42;
            Lbl(p, "Hàm trên", LEFT + 30, y + 22, 70);
            for (int i = 0; i < TEETH_UPPER.Length; i++)
            {
                int x = x0 + i * w + (i >= 8 ? 10 : 0);
                Lbl(p, TEETH_UPPER[i].ToString(), x + 12, y, 30);
                ToothBox(p, TEETH_UPPER[i], x, y + 20, w - 4);
            }
            y += 46;
            Lbl(p, "Hàm dưới", LEFT + 30, y, 70);
            for (int i = 0; i < TEETH_LOWER.Length; i++)
            {
                int x = x0 + i * w + (i >= 8 ? 10 : 0);
                ToothBox(p, TEETH_LOWER[i], x, y, w - 4);
                Lbl(p, TEETH_LOWER[i].ToString(), x + 12, y + 20, 30);
            }
            y += 44;
            Lbl(p, "Tình trạng răng: 0 Bình thường · 1 Sâu · 2 Trám sâu lại · 3 Trám tốt · 4 Mất do sâu · 5 Mất lý do khác · 6 Bít hố rãnh · 7 Trụ, cầu, implant · 8 Chưa mọc · 9 Loại trừ",
                LEFT + 30, y, 800).Appearance.ForeColor = Color.DimGray;
            return y + ROW_H;
        }

        private void ToothBox(Control p, int tooth, int x, int y, int w)
        {
            ComboBoxEdit c = new ComboBoxEdit();
            c.Location = new Point(x, y);
            c.Size = new Size(w, 20);
            c.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            c.Properties.Items.AddRange(new object[] { "", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            p.Controls.Add(c);
            dicField["TOOTH_" + tooth] = c;
        }

        /// <summary>Tab con 4 — cận lâm sàng.</summary>
        private void BuildCls()
        {
            Control p = pnlCls;
            int y = 4;
            SimpleButton btnMap = new SimpleButton();
            btnMap.Text = "Nối chỉ số CLS…";
            btnMap.ToolTip = "Nối chỉ số xét nghiệm / dịch vụ của viện với từng ô dưới đây. Sau đó tích \"Tự động lấy kết quả xét nghiệm\" để điền tự động.";
            btnMap.Location = new Point(680, y);
            btnMap.Size = new Size(130, 22);
            btnMap.Click += (s, e) => { if (ClsMapRequested != null) ClsMapRequested(this, EventArgs.Empty); };
            p.Controls.Add(btnMap);

            Title(p, "1. CÁC XÉT NGHIỆM BẮT BUỘC", ref y);
            Lbl(p, "a) Ma túy (sàng lọc và định tính):", LEFT + 20, y, 300);
            y += ROW_H - 2;
            string[][] drugs = { new[] { "Amphetamin", "TEST_AMPHETAMIN" }, new[] { "Marijuana", "TEST_MARIJUANA" }, new[] { "Morphin", "TEST_MORPHIN" },
                                 new[] { "Codein", "TEST_CODEIN" }, new[] { "Heroin", "TEST_HEROIN" } };
            int col = 0;
            foreach (string[] d in drugs)
            {
                int x = LEFT + 40 + col * 260;
                Lbl(p, d[0] + ":", x, y, 90); Txt(p, d[1], x + 90, y, 150);
                col++;
                if (col == 3) { col = 0; y += ROW_H; }
            }
            y += ROW_H;
            Lbl(p, "b) Nồng độ cồn trong máu hoặc hơi thở:", LEFT + 20, y, 260); Txt(p, "TEST_ALCOHOL", 290, y, 300);
            y += ROW_H;
            Lbl(p, "Người thực hiện (ma túy, cồn):", LEFT + 40, y, 200); Login(p, "TEST_DRUG_LOGINNAME", 250, y, 250);
            y += ROW_H + 4;
            Lbl(p, "Riêng nữ (Phụ lục XXV TT 32/2023): Tế bào cổ tử cung / HPV (nếu có):", LEFT + 20, y, 450);
            y += ROW_H - 4;
            femaleOnly.Add(Memo(p, "TEST_CERVICAL", LEFT + 40, y, 770, 36));
            y += 40;
            Lbl(p, "X-quang nhũ / Siêu âm 02 tuyến vú (nếu có):", LEFT + 20, y, 450);
            y += ROW_H - 4;
            femaleOnly.Add(Memo(p, "TEST_BREAST", LEFT + 40, y, 770, 36));
            y += 40;
            Lbl(p, "Người thực hiện (XN nữ):", LEFT + 40, y, 200); femaleOnly.Add(Login(p, "TEST_FEMALE_LOGINNAME", 250, y, 250));
            y += ROW_H + 4;

            Title(p, "2. CÁC XÉT NGHIỆM CHỈ THỰC HIỆN KHI CÓ CHỈ ĐỊNH CỦA BÁC SĨ KHÁM SỨC KHỎE", ref y);
            Lbl(p, "a) Xét nghiệm máu — Huyết học: tổng phân tích tế bào máu ngoại vi", LEFT + 20, y, 600);
            y += ROW_H - 2;
            string[][] blood = {
                new[] { "Số lượng HC", "TEST_BLOOD_HC", "T/L" }, new[] { "Huyết sắc tố", "TEST_BLOOD_HGB", "g/L" },
                new[] { "Hematocrit", "TEST_BLOOD_HCT", "L/L" }, new[] { "MCV", "TEST_BLOOD_MCV", "fL" },
                new[] { "MCH", "TEST_BLOOD_MCH", "pg" }, new[] { "MCHC", "TEST_BLOOD_MCHC", "g/L" },
                new[] { "RDW", "TEST_BLOOD_RDW", "%" }, new[] { "Số lượng bạch cầu", "TEST_BLOOD_BC", "G/L" },
                new[] { "BC trung tính", "TEST_BLOOD_NEUT", "G/L" }, new[] { "BC lympho", "TEST_BLOOD_LYMPH", "G/L" },
                new[] { "BC đơn nhân", "TEST_BLOOD_MONO", "G/L" }, new[] { "BC ái toan", "TEST_BLOOD_EOS", "G/L" },
                new[] { "BC ái kiềm", "TEST_BLOOD_BASO", "G/L" }, new[] { "Số lượng tiểu cầu", "TEST_BLOOD_TC", "G/L" } };
            y = Grid2(p, blood, y);
            Lbl(p, "Sinh hóa máu:", LEFT + 20, y, 300);
            y += ROW_H - 2;
            string[][] bio = { new[] { "Đường máu", "TEST_BLOOD_GLUCO", "mmol/L" }, new[] { "Creatinin", "TEST_BLOOD_CREATININ", "µmol/L" },
                               new[] { "ASAT (GOT)", "TEST_BLOOD_ASAT", "U/L" }, new[] { "ALAT (GPT)", "TEST_BLOOD_ALAT", "U/L" } };
            y = Grid2(p, bio, y);
            Lbl(p, "Người thực hiện (XN máu):", LEFT + 40, y, 200); Login(p, "TEST_BLOOD_LOGINNAME", 250, y, 250);
            y += ROW_H + 4;
            Lbl(p, "b) Xét nghiệm nước tiểu — tổng phân tích nước tiểu (máy tự động):", LEFT + 20, y, 600);
            y += ROW_H - 2;
            string[][] urine = {
                new[] { "Tỉ trọng", "TEST_URINE_SG", "" }, new[] { "pH", "TEST_URINE_PH", "" },
                new[] { "Bạch cầu", "TEST_URINE_LEU", "Leu/µL" }, new[] { "Hồng cầu", "TEST_URINE_ERY", "Ery/µL" },
                new[] { "Nitrit", "TEST_URINE_NITRIT", "" }, new[] { "Protein", "TEST_URINE_PROTEIN", "g/L" },
                new[] { "Glucose", "TEST_URINE_GLUCO", "mmol/L" }, new[] { "Thể cetonic", "TEST_URINE_KETONE", "mmol/L" },
                new[] { "Bilirubin", "TEST_URINE_BILIRUBIN", "µmol/L" }, new[] { "Urobilinogen", "TEST_URINE_UBG", "µmol/L" } };
            y = Grid2(p, urine, y);
            Lbl(p, "Khác (nếu có):", LEFT + 40, y, 120); Txt(p, "TEST_URINE_OTHER", 170, y, 640);
            y += ROW_H;
            Lbl(p, "Người thực hiện (XN nước tiểu):", LEFT + 40, y, 200); Login(p, "TEST_URINE_LOGINNAME", 250, y, 250);
            y += ROW_H + 4;
            Lbl(p, "c) Kết quả chẩn đoán hình ảnh:", LEFT + 20, y, 300);
            y += ROW_H - 4;
            Memo(p, "RESULT_DIIM", LEFT + 40, y, 770, 48);
            y += 52;
            Lbl(p, "Người thực hiện (CĐHA):", LEFT + 40, y, 200); Login(p, "DIIM_LOGINNAME", 250, y, 250);
            y += ROW_H + 4;
            Lbl(p, "d) Các xét nghiệm khác:", LEFT + 20, y, 300);
            y += ROW_H - 4;
            Memo(p, "RESULT_OTHER_TEST", LEFT + 40, y, 770, 48);
            y += 52;
            Lbl(p, "Người thực hiện (XN khác):", LEFT + 40, y, 200); Login(p, "OTHER_TEST_LOGINNAME", 250, y, 250);
        }

        /// <summary>Lưới 2 cột "nhãn · ô · đơn vị" cho các chỉ số xét nghiệm.</summary>
        private int Grid2(Control p, string[][] rows, int y)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                int x = LEFT + 40 + (i % 2) * 400;
                Lbl(p, rows[i][0] + ":", x, y, 140);
                Txt(p, rows[i][1], x + 140, y, 140);
                if (!string.IsNullOrEmpty(rows[i][2])) Lbl(p, rows[i][2], x + 285, y, 70).Appearance.ForeColor = Color.DimGray;
                if (i % 2 == 1 || i == rows.Length - 1) y += ROW_H;
            }
            return y + 2;
        }

        /// <summary>Tab con 5 — kết luận.</summary>
        private void BuildConclusion()
        {
            Control p = pnlConclusion;
            int y = 4;
            Title(p, "1. KẾT LUẬN — tình trạng sức khỏe để lái xe", ref y);
            rdoConclusion = Rdo(p, "DRIVER_CONCLUSION_TYPE", LEFT + 20, y, 800, 130,
                1, "Đủ điều kiện sức khỏe lái xe/điều khiển xe máy chuyên dùng hạng (ghi rõ hạng giấy phép lái xe)",
                2, "Không đủ điều kiện sức khỏe lái xe/điều khiển xe máy chuyên dùng hạng (ghi rõ hạng giấy phép lái xe)",
                3, "Đạt tiêu chuẩn sức khỏe lái xe/điều khiển xe máy chuyên dùng hạng (ghi rõ hạng) nhưng yêu cầu khám lại",
                4, "Đủ điều kiện sức khỏe lái xe hạng A1 đối với người khuyết tật điều khiển xe mô tô ba bánh dùng cho người khuyết tật",
                5, "Đủ điều kiện sức khỏe lái xe hạng B đối với người khuyết tật điều khiển xe ô tô số tự động có kết cấu phù hợp");
            rdoConclusion.EditValueChanged += (s, e) => RefreshState();
            cboLicenseClass.EditValueChanged += (s, e) => UpdateConclusionLicenseLabel();
            y += 134;
            lblConclusionLicense = Lbl(p, "", LEFT + 40, y, 700);
            y += ROW_H - 4;
            Lbl(p, "Thời gian khám lại:", LEFT + 40, y, 130); txtReexam = Txt(p, "REEXAM_NOTE", 170, y, 640);
            y += ROW_H + 4;

            Title(p, "2. CÁC BỆNH, TẬT (nếu có)", ref y);
            CheckEdit chkNormal = Chk(p, "CONCLUSION_IS_NORMAL", "Chưa phát hiện bất thường", LEFT + 20, y, 220);
            chkNormal.CheckedChanged += (s, e) => RefreshState();
            y += ROW_H;
            Lbl(p, "Chẩn đoán sơ bộ (ICD):", LEFT + 20, y, 170); Icd(p, "CONCLUSION_PRE_ICD", 200, y, 610);
            y += ROW_H;
            Lbl(p, "Chẩn đoán xác định (ICD):", LEFT + 20, y, 170); Icd(p, "CONCLUSION_ICD", 200, y, 610);
            y += ROW_H + 4;

            Title(p, "3. PHÂN LOẠI SỨC KHỎE", ref y);
            Lbl(p, "Phân loại:", LEFT + 20, y, 80); Rank(p, "HEALTH_EXAM_RANK_ID", LEFT + 100, y, 200);
            y += ROW_H + 4;

            Title(p, "4. ĐỀ NGHỊ", ref y);
            rdoSuggest = Rdo(p, "SUGGEST_TYPE", LEFT + 20, y, 800, 110,
                1, "Bình thường, hẹn khám định kỳ lần sau",
                2, "Có yếu tố nguy cơ, cần theo dõi thêm",
                3, "Đã có bệnh mạn tính, tiếp tục điều trị theo phác đồ/toa cũ",
                4, "Chuyển cơ sở KBCB, khám chuyên khoa",
                5, "Khác, ghi rõ (nếu có tư vấn chế độ ăn uống, tập luyện…)");
            rdoSuggest.EditValueChanged += (s, e) => RefreshState();
            y += 114;
            memSuggestNote = Memo(p, "SUGGEST_NOTE", LEFT + 40, y, 770, 40);
            y += 46;

            Title(p, "NGƯỜI KẾT LUẬN", ref y);
            Lbl(p, "Người kết luận:", LEFT + 20, y, 110); cboConcluder = Gle(p, null, LEFT + 130, y, 260);
            Lbl(p, "Ngày kết luận:", 430, y, 100);
            dteConclusionTime = new DateEdit();
            dteConclusionTime.Location = new Point(530, y);
            dteConclusionTime.Size = new Size(170, 20);
            dteConclusionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dteConclusionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            dteConclusionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dteConclusionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            dteConclusionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            dteConclusionTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            p.Controls.Add(dteConclusionTime);
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
        /// Dựng 21 câu theo danh mục bệnh mã 01–21 (đã sắp theo số câu), 2 cột như trên giấy; số câu theo mã.
        /// Gọi mỗi lần nạp tab — dựng lại nếu danh mục khác lần trước.
        /// </summary>
        public void SetDiseaseCatalog(List<HIS_DISEASE_TYPE> diseases)
        {
            try
            {
                List<HIS_DISEASE_TYPE> list = diseases ?? new List<HIS_DISEASE_TYPE>();
                if (dicDisease.Count == list.Count && list.All(o => dicDisease.ContainsKey(o.ID))) return;
                pnlDiseases.Controls.Clear();
                dicDisease.Clear();
                int half = (list.Count + 1) / 2;
                for (int i = 0; i < list.Count; i++)
                {
                    int colX = (i < half) ? 0 : 415;
                    int rowY = ((i < half) ? i : i - half) * ROW_H;
                    LabelControl l = new LabelControl();
                    l.AutoSizeMode = LabelAutoSizeMode.None;
                    int no = QuestionNo(list[i]);
                    l.Text = (no > 0 ? no : i + 1) + ". " + list[i].DISEASE_TYPE_NAME;
                    l.ToolTip = l.Text;
                    l.Location = new Point(colX + 20, rowY + 3);
                    l.Size = new Size(275, 18);
                    pnlDiseases.Controls.Add(l);
                    RadioGroup r = new RadioGroup();
                    r.Location = new Point(colX + 298, rowY);
                    r.Size = new Size(110, 22);
                    r.Properties.AllowFocused = false;
                    r.Properties.Items.Add(new RadioGroupItem(1, "Có"));
                    r.Properties.Items.Add(new RadioGroupItem(0, "Không"));
                    r.EditValue = null;
                    pnlDiseases.Controls.Add(r);
                    dicDisease[list[i].ID] = r;
                }
                pnlDiseases.Height = Math.Max(1, half) * ROW_H + 4;
                RelocateBelowDiseases();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Dời các ô nằm dưới lưới 21 câu theo chiều cao thực của lưới.</summary>
        private void RelocateBelowDiseases()
        {
            try
            {
                int bottom = pnlDiseases.Top + pnlDiseases.Height + 4;
                List<Control> below = pnlAdmin.Controls.Cast<Control>()
                    .Where(c => c != pnlDiseases && c.Top >= diseaseTop && !(c is ScrollBar))
                    .OrderBy(c => c.Top).ToList();
                if (below.Count == 0) return;
                int shift = bottom - below.Min(c => c.Top);
                if (shift == 0) return;
                foreach (Control c in below) c.Top += shift;
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
