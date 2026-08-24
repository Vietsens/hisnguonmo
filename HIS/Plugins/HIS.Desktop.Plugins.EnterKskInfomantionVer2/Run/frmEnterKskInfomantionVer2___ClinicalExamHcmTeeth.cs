/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Sơ đồ răng cho mục "Răng - Hàm - Mặt" của tab Khám lâm sàng HCM.
 *
 * Thay 2 ô nhập chữ (Hàm trên / Hàm dưới) bằng 32 chiếc răng theo ký hiệu FDI:
 *      Hàm trên  Q1: 18 17 16 15 14 13 12 11 | Q2: 21 22 23 24 25 26 27 28
 *      Hàm dưới  Q4: 48 47 46 45 44 43 42 41 | Q3: 31 32 33 34 35 36 37 38
 *
 * Cách nhập: TÍCH CHỌN nhiều răng -> chọn trạng thái ở ô chọn -> bấm "Áp dụng".
 * Trạng thái áp cho TẤT CẢ răng đang chọn, nên đánh dấu cả hàm chỉ mất vài thao tác.
 *
 * Khác bản mẫu trên web: bản mẫu xếp cả hàm trên trên MỘT hàng 16 răng, mỗi răng 1 ô chọn.
 * Bề ngang tab không đủ cho 16 ô/hàng có chữ (mỗi ô chỉ còn ~46px), nên ở đây mỗi hàm tách
 * thành 2 HÀNG × 8 RĂNG theo phần hàm. Nhờ vậy mỗi ô rộng gấp đôi, đủ chỗ hiện SỐ RĂNG ở
 * dòng trên và TÊN TRẠNG THÁI ở dòng dưới — không phải rê chuột mới biết.
 *
 * GIAI ĐOẠN NÀY: CHỈ DỰNG GIAO DIỆN, chưa nạp/lưu dữ liệu (xem TODO(BE) ở cuối file).
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Danh mục trạng thái răng =====

        /// <summary>Một trạng thái răng: mã nội bộ, tên đầy đủ, màu nền thể hiện trên sơ đồ.</summary>
        private class HcmToothStatus
        {
            public int Code { get; set; }
            /// <summary>Tên đầy đủ — dùng ở ô chọn, chú giải và chú thích khi rê chuột.</summary>
            public string Name { get; set; }
            /// <summary>Tên rút gọn hiện TRONG nút răng (chỗ hẹp).</summary>
            public string ShortName { get; set; }
            public Color BackColor { get; set; }
            public Color ForeColor { get; set; }
        }

        /// <summary>
        /// Bảng mã trạng thái răng CỦA HIS — dùng khi chưa bật cổng SYT TP.HCM.
        /// Khi đẩy lên cổng phải quy đổi sang mã định danh danh mục tình trạng răng của cổng.
        /// </summary>
        private static readonly HcmToothStatus[] HCM_TOOTH_STATUSES_HIS = new HcmToothStatus[]
        {
            NewToothStatus(10, "Không ghi nhận",    "Chưa ghi",       0xFA, 0xFA, 0xFA, 0x75, 0x75, 0x75),
            NewToothStatus(1,  "Bình thường",       "Bình thường",    0xE3, 0xF6, 0xE5, 0x2E, 0x7D, 0x32),
            NewToothStatus(2,  "Sâu",               "Sâu",            0xFF, 0xEB, 0xEE, 0xC6, 0x28, 0x28),
            NewToothStatus(3,  "Trám sâu lại",      "Trám sâu lại",   0xFF, 0xF3, 0xE0, 0xE6, 0x51, 0x00),
            NewToothStatus(4,  "Trám tốt",          "Trám tốt",       0xE3, 0xF2, 0xFD, 0x15, 0x65, 0xC0),
            NewToothStatus(5,  "Mất do sâu",        "Mất do sâu",     0xFF, 0xCD, 0xD2, 0xB7, 0x1C, 0x1C),
            NewToothStatus(6,  "Mất lý do khác",    "Mất, lý do khác",0xEC, 0xEF, 0xF1, 0x45, 0x5A, 0x64),
            NewToothStatus(7,  "Bít hố rãnh",       "Bít hố rãnh",    0xF3, 0xE5, 0xF5, 0x6A, 0x1B, 0x9A),
            NewToothStatus(8,  "Trụ, cầu, implant", "Trụ/cầu/implant",0xFF, 0xF9, 0xC4, 0x9E, 0x7D, 0x0A),
            NewToothStatus(9,  "Chưa mọc",          "Chưa mọc",       0xF5, 0xF5, 0xF5, 0x9E, 0x9E, 0x9E)
        };

        /// <summary>
        /// Danh sách trạng thái răng ĐANG DÙNG. Mặc định là bảng mã của HIS; khi viện bật khóa
        /// cấu hình cổng SYT TP.HCM thì được thay bằng danh mục tình trạng răng tải từ cổng
        /// (xem ApplySytCatalogToControls).
        ///
        /// Khởi tạo TRẺ (lúc dùng lần đầu) chứ không gán ngay khi khai báo: biến tĩnh chạy theo
        /// thứ tự khai báo trong tệp, gán ngay mà đặt nhầm chỗ thì mảng còn rỗng và chương trình
        /// VĂNG ngay khi mở chức năng — lỗi này không try/catch nào trong hàm bắt được.
        /// </summary>
        private static List<HcmToothStatus> hcmToothStatusList;

        private static List<HcmToothStatus> HCM_TOOTH_STATUSES
        {
            get
            {
                if (hcmToothStatusList == null || hcmToothStatusList.Count == 0)
                {
                    hcmToothStatusList = (HCM_TOOTH_STATUSES_HIS != null)
                        ? new List<HcmToothStatus>(HCM_TOOTH_STATUSES_HIS)
                        : new List<HcmToothStatus>();
                }
                return hcmToothStatusList;
            }
            set { hcmToothStatusList = value; }
        }

        private static HcmToothStatus NewToothStatus(int code, string name, string shortName,
            int br, int bg, int bb, int fr, int fg, int fb)
        {
            return new HcmToothStatus
            {
                Code = code,
                Name = name,
                ShortName = shortName,
                BackColor = Color.FromArgb(br, bg, bb),
                ForeColor = Color.FromArgb(fr, fg, fb)
            };
        }

        /// <summary>
        /// Trạng thái mặc định khi mở hồ sơ mới = "Bình thường" (theo quyết định của người yêu cầu,
        /// giống bản mẫu của cổng). Bác sĩ chỉ cần sửa những chiếc răng có vấn đề.
        /// Răng thực sự chưa khám thì chọn thủ công trạng thái "Không ghi nhận".
        /// </summary>
        private const int HCM_TOOTH_STATUS_DEFAULT = 1;

        /// <summary>4 phần hàm, mỗi phần 8 răng — mỗi phần chiếm 1 hàng để nút đủ rộng hiện chữ.</summary>
        private static readonly string[][] HCM_TOOTH_QUADRANTS = new string[][]
        {
            new[] { "18","17","16","15","14","13","12","11" },
            new[] { "21","22","23","24","25","26","27","28" },
            new[] { "48","47","46","45","44","43","42","41" },
            new[] { "31","32","33","34","35","36","37","38" }
        };

        private static readonly string[] HCM_TOOTH_QUADRANT_LABELS = new string[]
        {
            "Hàm trên  ·  Q1 bên phải:  18 → 11",
            "Hàm trên  ·  Q2 bên trái:  21 → 28",
            "Hàm dưới  ·  Q4 bên phải:  48 → 41",
            "Hàm dưới  ·  Q3 bên trái:  31 → 38"
        };

        #endregion

        #region ===== Khai báo =====

        private PanelControl panelHcmTeeth;
        private ComboBoxEdit cboHcmToothStatus;
        private SimpleButton btnHcmToothApply;
        private SimpleButton btnHcmToothSelectAll;
        private SimpleButton btnHcmToothClearSel;
        private LabelControl lblHcmToothSelCount;
        private readonly LabelControl[] lblHcmToothQuadrants = new LabelControl[4];
        private LabelControl lblHcmToothLegend;

        /// <summary>Nút của từng chiếc răng, khóa = số răng FDI.</summary>
        private readonly Dictionary<string, CheckButton> hcmToothButtons = new Dictionary<string, CheckButton>();
        /// <summary>Trạng thái hiện tại của từng chiếc răng, khóa = số răng FDI.</summary>
        private readonly Dictionary<string, int> hcmToothStatus = new Dictionary<string, int>();

        private const int HCM_TEETH_PANEL_H = 296;
        private const int HCM_TOOTH_H = 38;          // 2 dòng chữ: số răng + tên trạng thái
        private const int HCM_TOOTH_GAP = 4;
        private const int HCM_TOOTH_PAD = 6;
        private const int HCM_TOOTH_LBL_H = 16;
        private const int HCM_TOOTH_ROW_GAP = 6;

        #endregion

        #region ===== Dựng sơ đồ răng =====

        /// <summary>Dựng khối sơ đồ răng, trả về panel để đặt vào 1 dòng chiếm hết bề ngang.</summary>
        private PanelControl BuildHcmToothChart()
        {
            panelHcmTeeth = new PanelControl();
            panelHcmTeeth.Name = "panelClinicalHcmTeeth";
            panelHcmTeeth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // --- Hàng thao tác: chọn trạng thái -> áp cho các răng đang tích ---
            LabelControl lblStatus = new LabelControl();
            lblStatus.Name = "lblClinicalHcmToothStatus";
            lblStatus.Text = "Trạng thái:";
            lblStatus.AutoSizeMode = LabelAutoSizeMode.None;
            panelHcmTeeth.Controls.Add(lblStatus);

            cboHcmToothStatus = new ComboBoxEdit();
            cboHcmToothStatus.Name = "cboClinicalHcmToothStatus";
            cboHcmToothStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboHcmToothStatus.Properties.NullText = "";
            foreach (HcmToothStatus st in HCM_TOOTH_STATUSES)
                cboHcmToothStatus.Properties.Items.Add(st.Name);
            cboHcmToothStatus.SelectedIndex = 1;   // mặc định để sẵn "Bình thường" cho thao tác nhanh
            panelHcmTeeth.Controls.Add(cboHcmToothStatus);

            btnHcmToothApply = NewHcmToothButton("btnClinicalHcmToothApply", "Áp dụng",
                btnHcmToothApply_Click);
            btnHcmToothSelectAll = NewHcmToothButton("btnClinicalHcmToothSelectAll", "Chọn tất cả",
                btnHcmToothSelectAll_Click);
            btnHcmToothClearSel = NewHcmToothButton("btnClinicalHcmToothClearSel", "Bỏ chọn",
                btnHcmToothClearSel_Click);

            lblHcmToothSelCount = new LabelControl();
            lblHcmToothSelCount.Name = "lblClinicalHcmToothSelCount";
            lblHcmToothSelCount.AutoSizeMode = LabelAutoSizeMode.None;
            panelHcmTeeth.Controls.Add(lblHcmToothSelCount);

            // --- 4 phần hàm: nhãn + 8 chiếc răng mỗi phần ---
            for (int q = 0; q < HCM_TOOTH_QUADRANTS.Length; q++)
            {
                lblHcmToothQuadrants[q] = NewHcmToothLabel(
                    "lblClinicalHcmToothQuadrant" + q, HCM_TOOTH_QUADRANT_LABELS[q]);
                foreach (string t in HCM_TOOTH_QUADRANTS[q]) CreateHcmTooth(t);
            }

            // --- Chú giải kèm số lượng từng trạng thái ---
            lblHcmToothLegend = new LabelControl();
            lblHcmToothLegend.Name = "lblClinicalHcmToothLegend";
            lblHcmToothLegend.AutoSizeMode = LabelAutoSizeMode.None;
            lblHcmToothLegend.Appearance.ForeColor = Color.FromArgb(90, 90, 90);
            panelHcmTeeth.Controls.Add(lblHcmToothLegend);

            panelHcmTeeth.Resize += delegate { LayoutHcmToothChart(); };

            RefreshHcmToothSelCount();
            RefreshHcmToothLegend();
            return panelHcmTeeth;
        }

        private SimpleButton NewHcmToothButton(string name, string text, EventHandler onClick)
        {
            SimpleButton btn = new SimpleButton();
            btn.Name = name;
            btn.Text = text;
            btn.Click += onClick;
            panelHcmTeeth.Controls.Add(btn);
            return btn;
        }

        private LabelControl NewHcmToothLabel(string name, string text)
        {
            LabelControl lbl = new LabelControl();
            lbl.Name = name;
            lbl.Text = text;
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            lbl.Appearance.ForeColor = Color.FromArgb(70, 90, 120);
            panelHcmTeeth.Controls.Add(lbl);
            return lbl;
        }

        /// <summary>Tạo 1 chiếc răng: nút bấm hai trạng thái, chữ là số răng, màu nền là trạng thái.</summary>
        private void CreateHcmTooth(string toothNo)
        {
            CheckButton btn = new CheckButton();
            btn.Name = "btnClinicalHcmTooth_" + toothNo;
            btn.Text = toothNo;
            btn.AllowFocus = false;
            btn.Appearance.Options.UseBackColor = true;
            btn.Appearance.Options.UseForeColor = true;
            btn.Appearance.Options.UseFont = true;
            btn.CheckedChanged += btnHcmTooth_CheckedChanged;

            panelHcmTeeth.Controls.Add(btn);
            hcmToothButtons[toothNo] = btn;
            hcmToothStatus[toothNo] = HCM_TOOTH_STATUS_DEFAULT;
            PaintHcmTooth(toothNo);
        }

        #endregion

        #region ===== Bố cục sơ đồ răng =====

        /// <summary>Xếp lại toàn bộ sơ đồ theo bề ngang thật của panel. Gọi mỗi lần panel đổi kích thước.</summary>
        private void LayoutHcmToothChart()
        {
            try
            {
                if (panelHcmTeeth == null || hcmToothButtons.Count == 0) return;

                int w = panelHcmTeeth.ClientSize.Width;
                if (w <= 0) return;

                panelHcmTeeth.SuspendLayout();
                try
                {
                    int y = 2;

                    // Hàng thao tác.
                    int x = HCM_TOOTH_PAD;
                    lblHcmToothStatusLayout(ref x, y);
                    y += 26;

                    // 4 phần hàm, mỗi phần 1 nhãn + 1 hàng 8 răng.
                    for (int q = 0; q < HCM_TOOTH_QUADRANTS.Length; q++)
                    {
                        lblHcmToothQuadrants[q].Location = new Point(HCM_TOOTH_PAD, y);
                        lblHcmToothQuadrants[q].Size = new Size(w - HCM_TOOTH_PAD * 2, HCM_TOOTH_LBL_H);
                        y += HCM_TOOTH_LBL_H + 2;
                        LayoutHcmToothRow(HCM_TOOTH_QUADRANTS[q], y, w);
                        y += HCM_TOOTH_H + HCM_TOOTH_ROW_GAP;
                    }

                    // Chú giải.
                    lblHcmToothLegend.Location = new Point(HCM_TOOTH_PAD, y);
                    lblHcmToothLegend.Size = new Size(w - HCM_TOOTH_PAD * 2, 16);
                }
                finally { panelHcmTeeth.ResumeLayout(true); }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Xếp hàng thao tác (nhãn, ô chọn trạng thái, 3 nút, số răng đang chọn).</summary>
        private void lblHcmToothStatusLayout(ref int x, int y)
        {
            Control lblStatus = panelHcmTeeth.Controls["lblClinicalHcmToothStatus"];
            if (lblStatus != null)
            {
                lblStatus.Location = new Point(x, y + 4);
                lblStatus.Size = new Size(64, 16);
                x += 64 + 4;
            }

            cboHcmToothStatus.Location = new Point(x, y);
            cboHcmToothStatus.Size = new Size(170, 22);
            x += 170 + 6;

            btnHcmToothApply.Location = new Point(x, y);
            btnHcmToothApply.Size = new Size(80, 22);
            x += 80 + 12;

            btnHcmToothSelectAll.Location = new Point(x, y);
            btnHcmToothSelectAll.Size = new Size(90, 22);
            x += 90 + 4;

            btnHcmToothClearSel.Location = new Point(x, y);
            btnHcmToothClearSel.Size = new Size(80, 22);
            x += 80 + 12;

            lblHcmToothSelCount.Location = new Point(x, y + 4);
            lblHcmToothSelCount.Size = new Size(Math.Max(60, panelHcmTeeth.ClientSize.Width - x - HCM_TOOTH_PAD), 16);
        }

        /// <summary>Xếp 8 chiếc răng của 1 phần hàm, chia đều bề ngang.</summary>
        private void LayoutHcmToothRow(string[] teeth, int y, int panelWidth)
        {
            int usable = panelWidth - HCM_TOOTH_PAD * 2 - (teeth.Length - 1) * HCM_TOOTH_GAP;
            int tw = usable / teeth.Length;
            if (tw < 60) tw = 60;

            int x = HCM_TOOTH_PAD;
            for (int i = 0; i < teeth.Length; i++)
            {
                CheckButton btn;
                if (!hcmToothButtons.TryGetValue(teeth[i], out btn)) continue;
                btn.Location = new Point(x, y);
                btn.Size = new Size(tw, HCM_TOOTH_H);
                x += tw + HCM_TOOTH_GAP;
            }
        }

        #endregion

        #region ===== Thao tác =====

        private void btnHcmTooth_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckButton btn = sender as CheckButton;
                if (btn == null) return;
                string toothNo = btn.Name.Replace("btnClinicalHcmTooth_", "");
                PaintHcmTooth(toothNo);
                RefreshHcmToothSelCount();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Áp trạng thái đang chọn cho TẤT CẢ răng đang tích, rồi bỏ tích để làm tiếp nhóm khác.</summary>
        private void btnHcmToothApply_Click(object sender, EventArgs e)
        {
            try
            {
                HcmToothStatus st = GetHcmSelectedStatus();
                if (st == null)
                {
                    Inventec.Desktop.Common.Message.MessageManager.Show("Chọn trạng thái cần áp dụng.");
                    return;
                }

                List<string> selected = GetHcmSelectedTeeth();
                if (selected.Count == 0)
                {
                    Inventec.Desktop.Common.Message.MessageManager.Show(
                        "Chưa chọn chiếc răng nào. Bấm vào số răng để chọn, có thể chọn nhiều răng cùng lúc.");
                    return;
                }

                foreach (string toothNo in selected)
                {
                    hcmToothStatus[toothNo] = st.Code;
                    hcmToothButtons[toothNo].Checked = false;   // bỏ tích -> tự vẽ lại trong CheckedChanged
                    PaintHcmTooth(toothNo);
                }

                RefreshHcmToothSelCount();
                RefreshHcmToothLegend();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private void btnHcmToothSelectAll_Click(object sender, EventArgs e)
        {
            try { SetAllHcmToothChecked(true); }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private void btnHcmToothClearSel_Click(object sender, EventArgs e)
        {
            try { SetAllHcmToothChecked(false); }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private void SetAllHcmToothChecked(bool value)
        {
            foreach (KeyValuePair<string, CheckButton> kv in hcmToothButtons)
                kv.Value.Checked = value;
            RefreshHcmToothSelCount();
        }

        private HcmToothStatus GetHcmSelectedStatus()
        {
            int i = cboHcmToothStatus.SelectedIndex;
            if (i < 0 || i >= HCM_TOOTH_STATUSES.Count) return null;
            return HCM_TOOTH_STATUSES[i];
        }

        private List<string> GetHcmSelectedTeeth()
        {
            List<string> rs = new List<string>();
            foreach (KeyValuePair<string, CheckButton> kv in hcmToothButtons)
            {
                if (kv.Value.Checked) rs.Add(kv.Key);
            }
            return rs;
        }

        private static HcmToothStatus FindHcmToothStatus(int code)
        {
            var all = HCM_TOOTH_STATUSES;
            if (all == null || all.Count == 0) return null;
            foreach (HcmToothStatus st in all)
            {
                if (st.Code == code) return st;
            }
            return all[0];
        }

        /// <summary>Tô lại 1 chiếc răng theo trạng thái; đang tích thì làm đậm để thấy rõ.</summary>
        private void PaintHcmTooth(string toothNo)
        {
            CheckButton btn;
            if (!hcmToothButtons.TryGetValue(toothNo, out btn)) return;

            int code = hcmToothStatus.ContainsKey(toothNo) ? hcmToothStatus[toothNo] : HCM_TOOTH_STATUS_DEFAULT;
            HcmToothStatus st = FindHcmToothStatus(code);
            if (st == null) return;   // danh mục trạng thái rỗng -> để nguyên nút, không vẽ

            // Dòng trên: số răng. Dòng dưới: tên trạng thái -> không phải rê chuột mới biết.
            btn.Text = toothNo + Environment.NewLine + st.ShortName;
            btn.Appearance.BackColor = st.BackColor;
            btn.Appearance.ForeColor = st.ForeColor;
            btn.Appearance.Font = new Font(btn.Font.FontFamily, 8.25f,
                btn.Checked ? FontStyle.Bold : FontStyle.Regular);
            btn.ToolTip = "Răng " + toothNo + " — " + st.Name;
        }

        /// <summary>
        /// Thay bảng mã trạng thái răng bằng danh mục của cổng, rồi nạp lại ô chọn và vẽ lại sơ đồ.
        /// Danh mục rỗng thì GIỮ NGUYÊN bảng mã của HIS — không để bác sĩ mất chỗ chọn.
        /// </summary>
        private void SetHcmToothStatusSource(List<HcmToothStatus> statuses)
        {
            try
            {
                if (statuses == null || statuses.Count == 0) return;
                HCM_TOOTH_STATUSES = statuses;

                if (cboHcmToothStatus != null)
                {
                    cboHcmToothStatus.Properties.Items.Clear();
                    foreach (HcmToothStatus st in HCM_TOOTH_STATUSES)
                        cboHcmToothStatus.Properties.Items.Add(st.Name);
                    if (cboHcmToothStatus.Properties.Items.Count > 0) cboHcmToothStatus.SelectedIndex = 0;
                }

                // Răng nào đang mang mã không còn trong danh mục mới -> đưa về mã đầu danh sách.
                List<string> keys = new List<string>(hcmToothStatus.Keys);
                foreach (string toothNo in keys)
                {
                    if (FindHcmToothStatusOrNull(hcmToothStatus[toothNo]) == null)
                        hcmToothStatus[toothNo] = HCM_TOOTH_STATUSES[0].Code;
                    PaintHcmTooth(toothNo);
                }
                RefreshHcmToothLegend();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static HcmToothStatus FindHcmToothStatusOrNull(int code)
        {
            foreach (HcmToothStatus st in HCM_TOOTH_STATUSES)
            {
                if (st.Code == code) return st;
            }
            return null;
        }

        private void RefreshHcmToothSelCount()
        {
            if (lblHcmToothSelCount == null) return;
            int n = GetHcmSelectedTeeth().Count;
            lblHcmToothSelCount.Text = n == 0 ? "Chưa chọn răng nào" : ("Đang chọn " + n + " răng");
            lblHcmToothSelCount.Appearance.ForeColor = n == 0
                ? Color.FromArgb(120, 120, 120)
                : Color.FromArgb(21, 101, 192);
        }

        /// <summary>Chú giải: chỉ hiện trạng thái CÓ răng, kèm số lượng — nhìn là biết đã đánh đủ chưa.</summary>
        private void RefreshHcmToothLegend()
        {
            if (lblHcmToothLegend == null) return;

            List<string> parts = new List<string>();
            foreach (HcmToothStatus st in HCM_TOOTH_STATUSES)
            {
                int n = 0;
                foreach (KeyValuePair<string, int> kv in hcmToothStatus)
                {
                    if (kv.Value == st.Code) n++;
                }
                if (n > 0) parts.Add(st.Name + ": " + n);
            }
            lblHcmToothLegend.Text = string.Join("     ", parts.ToArray());
        }

        #endregion

        // TODO(BE): nạp/lưu sơ đồ răng khi bổ sung bảng dữ liệu riêng của mẫu M4:
        //   - lưu   : hcmToothStatus (32 cặp "số răng" -> "mã trạng thái nội bộ")
        //   - nạp   : gán lại hcmToothStatus rồi gọi PaintHcmTooth cho từng răng + RefreshHcmToothLegend
        //   - đẩy   : quy đổi mã trạng thái nội bộ -> mã định danh danh mục tình trạng răng của cổng,
        //             gói thành cặp "số răng": "mã của cổng" đúng như chỉ tiêu chi tiết khám răng của mẫu M4.
        //   - Răng để "Không ghi nhận" thì KHÔNG gửi lên cổng (quy tắc R15), không gửi trị rỗng.
    }
}
