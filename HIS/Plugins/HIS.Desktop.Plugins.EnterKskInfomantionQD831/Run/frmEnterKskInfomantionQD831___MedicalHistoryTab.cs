/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    /// <summary>
    /// Tab "B. Tiền sử" — mục 3 "Tiền sử bệnh tật, dị ứng (bản thân)".
    /// Lấy từ 2 bảng: HIS_DISEASE_TYPE (lọc DISEASE_TYPE_CODE) -> ID -> móc HIS_DISEASE_DETAIL
    /// theo DISEASE_TYPE_ID, sắp xếp theo NUM_ORDER.
    ///  - Dị ứng (code "50"): GridControl gcTsDiUngBanThan. Cột check chỉ hiện khi có ít nhất 1 IS_CHECKBOX
    ///    (tiêu đề ẩn, nhỏ); còn lại: Loại + Mô tả rõ.
    ///  - Bệnh tật (code "49"): KHÔNG grid — sinh control động vào panel1 (IS_CHECKBOX->CheckEdit,
    ///    IS_OTHER->TextEdit+nhãn), DISEASE_TYPE_NAME làm tiêu đề.
    /// Dữ liệu danh mục lấy từ BackendDataWorker (cache RAM).
    /// </summary>
    public partial class frmEnterKskInfomantionQD831
    {
        private const string DISEASE_TYPE_CODE__DI_UNG_BAN_THAN = "50";   // grid mục 3 (Dị ứng bản thân)
        private const string DISEASE_TYPE_CODE__BENH_TAT_BAN_THAN = "49"; // control động mục 3 (Bệnh tật bản thân)
        private const string DISEASE_TYPE_CODE__KHUYET_TAT = "53";        // grid mục 4 (Khuyết tật)
        private const string DISEASE_TYPE_CODE__DI_UNG_GIA_DINH = "52";   // grid mục 6 (Dị ứng gia đình)
        private const string DISEASE_TYPE_CODE__BENH_TAT_GIA_DINH = "51"; // grid mục 6 (Bệnh tật gia đình)

        /// <summary>Row model chung cho các grid bệnh tật/dị ứng (mỗi HIS_DISEASE_DETAIL = 1 dòng).</summary>
        internal class DiseaseGridRow
        {
            public long DetailId { get; set; }
            public string TenLoai { get; set; }   // HIS_DISEASE_DETAIL.NAME
            public bool? Chon { get; set; }        // null = dòng không có checkbox (ô để trống)
            public string MoTa { get; set; }
            public string NguoiMac { get; set; }   // chỉ dùng cho grid tiền sử gia đình
            public bool IsCheckbox { get; set; }   // dòng này có ô check hay không
        }

        // Map để đọc/lưu về sau: DISEASE_DETAIL_ID -> control (TextEdit inline hoặc MemoEdit full-width).
        private Dictionary<long, CheckEdit> benhTatCheckMap;
        private Dictionary<long, BaseEdit> benhTatTextMap;
        // Dòng của từng grid theo DISEASE_TYPE_CODE (cho lưu về sau).
        private readonly Dictionary<string, BindingList<DiseaseGridRow>> diseaseGridRows = new Dictionary<string, BindingList<DiseaseGridRow>>();

        /// <summary>Lấy HIS_DISEASE_TYPE theo DISEASE_TYPE_CODE (bỏ soft delete).</summary>
        private HIS_DISEASE_TYPE GetDiseaseTypeByCode(string code)
        {
            try
            {
                var list = BackendDataWorker.Get<HIS_DISEASE_TYPE>();
                if (list == null) return null;
                return list.FirstOrDefault(t => (t.DISEASE_TYPE_CODE ?? "").Trim() == code
                    && (t.IS_DELETE == null || t.IS_DELETE == 0));
            }
            catch (Exception ex) { LogSystem.Error(ex); return null; }
        }

        /// <summary>Móc HIS_DISEASE_DETAIL theo DISEASE_TYPE_ID, sắp xếp theo NUM_ORDER.</summary>
        private List<HIS_DISEASE_DETAIL> GetDiseaseDetailsByType(long typeId)
        {
            try
            {
                var list = BackendDataWorker.Get<HIS_DISEASE_DETAIL>();
                if (list == null) return new List<HIS_DISEASE_DETAIL>();
                return list
                    .Where(d => d.DISEASE_TYPE_ID == typeId && (d.IS_DELETE == null || d.IS_DELETE == 0))
                    .OrderBy(d => d.NUM_ORDER ?? 0)
                    .ToList();
            }
            catch (Exception ex) { LogSystem.Error(ex); return new List<HIS_DISEASE_DETAIL>(); }
        }

        /// <summary>Khởi tạo mục 3 tab Tiền sử: grid Dị ứng (code 50) + control động Bệnh tật (code 49).</summary>
        private void InitDiseaseHistoryTienSu()
        {
            try
            {
                // Mục 3 — Dị ứng bản thân (grid, code 50)
                LoadDiseaseGrid(this.gcTsDiUngBanThan, DISEASE_TYPE_CODE__DI_UNG_BAN_THAN, "Loại", false);
                // Mục 4 — Khuyết tật (grid, code 53)
                LoadDiseaseGrid(this.gcTsKhuyetTat, DISEASE_TYPE_CODE__KHUYET_TAT, "Bộ phận/cơ quan", false);
                // Mục 6 — Tiền sử gia đình: Dị ứng (52) + Bệnh tật (51), có thêm cột "Người mắc"
                LoadDiseaseGrid(this.gcTsDiUngGiaDinh, DISEASE_TYPE_CODE__DI_UNG_GIA_DINH, "Loại", true);
                LoadDiseaseGrid(this.gcTsBenhTatGiaDinh, DISEASE_TYPE_CODE__BENH_TAT_GIA_DINH, "Tên bệnh", true);
                // Mục 6 — Bệnh tật gia đình: tích "Chọn" mới cho nhập "Mô tả" + "Người mắc (quan hệ)".
                WireDiseaseGridGating(this.gcTsBenhTatGiaDinh);

                // Mục 3 — Bệnh tật bản thân (control động, code 49)
                var typeBenhTat = GetDiseaseTypeByCode(DISEASE_TYPE_CODE__BENH_TAT_BAN_THAN);
                GenBenhTatBanThanControls(typeBenhTat, typeBenhTat != null ? GetDiseaseDetailsByType(typeBenhTat.ID) : new List<HIS_DISEASE_DETAIL>());

                // Mục 2 — Loại hố xí (GridLookUpEdit)
                InitHoXiLookup();

                // Mục 7 — bỏ nút spin trên các ô số + chặn lăn chuột đổi giá trị.
                RemoveMuc7SpinButtons();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        internal class HoXiItem
        {
            public string TEN { get; set; }
            public HoXiItem(string ten) { TEN = ten; }
        }

        /// <summary>Nạp danh sách "Loại hố xí" vào GridLookUpEdit cboTsHoXi.</summary>
        private void InitHoXiLookup()
        {
            try
            {
                if (this.cboTsHoXi == null) return;
                var list = new List<HoXiItem>
                {
                    new HoXiItem("Xả nước"),
                    new HoXiItem("Hai ngăn"),
                    new HoXiItem("Hố xí thùng"),
                    new HoXiItem("Không có"),
                };
                this.cboTsHoXi.Properties.DataSource = list;
                this.cboTsHoXi.Properties.DisplayMember = "TEN";
                this.cboTsHoXi.Properties.ValueMember = "TEN";
                this.cboTsHoXi.Properties.NullText = "";
                this.cboTsHoXi.Properties.PopulateViewColumns();
                if (this.gvTsHoXiView != null)
                {
                    this.gvTsHoXiView.OptionsView.ShowColumnHeaders = false;
                    foreach (GridColumn c in this.gvTsHoXiView.Columns) c.Caption = "Loại hố xí";
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        // ===================== Grid dùng chung (Dị ứng/Khuyết tật/Gia đình) =====================
        /// <summary>
        /// Dựng cột + nạp dữ liệu 1 grid theo DISEASE_TYPE_CODE.
        /// Cột: [check — chỉ khi có ít nhất 1 IS_CHECKBOX, tiêu đề ẩn/nhỏ] + Tên (khóa) + Mô tả rõ + [Người mắc nếu gia đình].
        /// </summary>
        private void LoadDiseaseGrid(GridControl grid, string typeCode, string nameCaption, bool includeNguoiMac)
        {
            try
            {
                if (grid == null) return;
                var type = GetDiseaseTypeByCode(typeCode);
                var details = type != null ? GetDiseaseDetailsByType(type.ID) : new List<HIS_DISEASE_DETAIL>();
                bool anyCheckbox = details.Any(d => (d.IS_CHECKBOX ?? 0) == 1);

                GridView gv = grid.MainView as GridView;
                if (gv == null) { gv = new GridView(grid); grid.MainView = gv; }
                gv.Columns.Clear();
                grid.RepositoryItems.Clear();

                int vi = 0;
                if (anyCheckbox)
                {
                    var repoChk = new RepositoryItemCheckEdit();
                    grid.RepositoryItems.Add(repoChk);
                    // Editor rỗng (readonly, không viền) cho dòng KHÔNG phải checkbox -> ô check để trống.
                    var repoEmpty = new RepositoryItemTextEdit();
                    repoEmpty.ReadOnly = true;
                    repoEmpty.NullText = "";
                    repoEmpty.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    grid.RepositoryItems.Add(repoEmpty);

                    GridColumn cChk = gv.Columns.AddVisible("Chon");
                    cChk.Caption = " ";
                    cChk.ColumnEdit = repoChk;
                    cChk.Width = 26; cChk.MinWidth = 24; cChk.MaxWidth = 30;
                    cChk.OptionsColumn.FixedWidth = true;
                    cChk.VisibleIndex = vi++;

                    // Chỉ dòng IsCheckbox mới dùng CheckEdit; dòng khác dùng editor rỗng (không hiện checkbox).
                    gv.CustomRowCellEdit += (s, e) =>
                    {
                        if (e.Column != cChk) return;
                        var r = gv.GetRow(e.RowHandle) as DiseaseGridRow;
                        e.RepositoryItem = (r != null && r.IsCheckbox) ? (DevExpress.XtraEditors.Repository.RepositoryItem)repoChk : repoEmpty;
                    };
                }
                GridColumn cName = gv.Columns.AddVisible("TenLoai");
                cName.Caption = nameCaption;
                cName.OptionsColumn.AllowEdit = false;
                cName.Width = 120; cName.OptionsColumn.FixedWidth = true;
                cName.VisibleIndex = vi++;

                GridColumn cMoTa = gv.Columns.AddVisible("MoTa");
                cMoTa.Caption = "Mô tả rõ";
                cMoTa.VisibleIndex = vi++;

                if (includeNguoiMac)
                {
                    GridColumn cNM = gv.Columns.AddVisible("NguoiMac");
                    cNM.Caption = "Người mắc (quan hệ huyết thống)";
                    cNM.Width = 170; cNM.OptionsColumn.FixedWidth = true;
                    cNM.VisibleIndex = vi++;
                }

                gv.OptionsView.ShowGroupPanel = false;
                gv.OptionsView.ColumnAutoWidth = true;
                gv.OptionsBehavior.Editable = true;

                var rows = new BindingList<DiseaseGridRow>();
                foreach (var d in details)
                {
                    bool isChk = (d.IS_CHECKBOX ?? 0) == 1;
                    rows.Add(new DiseaseGridRow
                    {
                        DetailId = d.ID,
                        TenLoai = (d.NAME ?? "").Trim(),
                        IsCheckbox = isChk,
                        Chon = isChk ? (bool?)false : null,  // dòng không có checkbox -> null -> ô trống
                        MoTa = "",
                        NguoiMac = ""
                    });
                }
                grid.DataSource = rows;
                this.diseaseGridRows[typeCode] = rows;
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        // ===================== Logic tương tác grid (check "Chọn" -> Mô tả + Người mắc) =====================
        private bool suppressDiseaseGatingEvent = false;

        /// <summary>
        /// Grid bệnh tật/dị ứng có cột "Chọn": chỉ khi tích Chọn (dòng IsCheckbox) mới cho nhập
        /// "Mô tả" + "Người mắc (quan hệ)"; bỏ tích -> khóa (tô xám) + xóa. Dòng không có checkbox: nhập tự do.
        /// </summary>
        private void WireDiseaseGridGating(GridControl grid)
        {
            try
            {
                if (grid == null) return;
                var gv = grid.MainView as GridView;
                if (gv == null || gv.Columns["Chon"] == null) return;
                string[] gated = { "MoTa", "NguoiMac" };

                gv.CellValueChanged += (s, e) =>
                {
                    if (suppressDiseaseGatingEvent) return;
                    if (e.Column.FieldName != "Chon") return;
                    var r = gv.GetRow(e.RowHandle) as DiseaseGridRow;
                    if (r == null || !r.IsCheckbox) return;
                    bool on = e.Value != null && Convert.ToBoolean(e.Value);
                    if (!on)
                    {
                        try
                        {
                            suppressDiseaseGatingEvent = true;
                            foreach (var f in gated)
                                if (gv.Columns[f] != null) gv.SetRowCellValue(e.RowHandle, gv.Columns[f], null);
                        }
                        finally { suppressDiseaseGatingEvent = false; try { gv.RefreshRow(e.RowHandle); } catch { } }
                    }
                };

                gv.ShowingEditor += (s, e) =>
                {
                    var view = s as GridView;
                    if (view == null || view.FocusedColumn == null) return;
                    if (Array.IndexOf(gated, view.FocusedColumn.FieldName) < 0) return;
                    var r = view.GetRow(view.FocusedRowHandle) as DiseaseGridRow;
                    if (r != null && r.IsCheckbox && !(r.Chon.HasValue && r.Chon.Value))
                        e.Cancel = true; // chưa tích Chọn -> khóa ô Mô tả/Người mắc
                };

                gv.RowCellStyle += (s, e) =>
                {
                    if (Array.IndexOf(gated, e.Column.FieldName) < 0) return;
                    var r = gv.GetRow(e.RowHandle) as DiseaseGridRow;
                    if (r != null && r.IsCheckbox && !(r.Chon.HasValue && r.Chon.Value))
                    {
                        e.Appearance.BackColor = Color.FromArgb(238, 238, 238);
                        e.Appearance.Options.UseBackColor = true;
                    }
                };
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        // ===================== Bệnh tật (control động, code 49) =====================
        private void GenBenhTatBanThanControls(HIS_DISEASE_TYPE type, List<HIS_DISEASE_DETAIL> details)
        {
            try
            {
                if (this.panel1 == null) return;
                this.benhTatCheckMap = new Dictionary<long, CheckEdit>();
                this.benhTatTextMap = new Dictionary<long, BaseEdit>();

                this.panel1.SuspendLayout();
                this.panel1.Controls.Clear();
                this.panel1.AutoScroll = true;
                SetDoubleBuffered(this.panel1);

                const int COLS = 3;
                var table = new TableLayoutPanel();
                table.SuspendLayout();
                table.Dock = DockStyle.Top;
                table.ColumnCount = COLS;
                table.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                table.ColumnStyles.Clear();
                for (int i = 0; i < COLS; i++)
                    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / COLS));
                this.panel1.Controls.Add(table);
                SetDoubleBuffered(table);

                int row = 0, col = 0;

                // Tiêu đề = DISEASE_TYPE_NAME, chiếm cả dòng (span 3 cột).
                string typeName = (type != null ? (type.DISEASE_TYPE_NAME ?? "") : "").Trim();
                if (!string.IsNullOrEmpty(typeName))
                {
                    var lblH = new LabelControl();
                    lblH.Text = typeName;
                    lblH.Appearance.FontStyleDelta = FontStyle.Bold;
                    lblH.Appearance.Options.UseFont = true;
                    lblH.AutoSizeMode = LabelAutoSizeMode.None;
                    lblH.Dock = DockStyle.Fill;
                    if (row >= table.RowCount) table.RowCount = row + 1;
                    table.Controls.Add(lblH, 0, row);
                    table.SetColumnSpan(lblH, COLS);
                    row++; col = 0;
                }

                foreach (var d in details)
                {
                    bool isChk = (d.IS_CHECKBOX ?? 0) == 1;
                    bool isOther = (d.IS_OTHER ?? 0) == 1;
                    string name = (d.NAME ?? "").Trim();

                    if (isChk && !isOther)
                    {
                        // checkbox: 1 ô/cột, tự xuống dòng sau COLS cột.
                        var chk = new CheckEdit(); chk.Properties.Caption = name; chk.Dock = DockStyle.Fill; chk.Tag = d.ID;
                        if (row >= table.RowCount) table.RowCount = row + 1;
                        table.Controls.Add(chk, col, row);
                        this.benhTatCheckMap[d.ID] = chk;
                        col++;
                        if (col >= COLS) { col = 0; row++; }
                    }
                    else
                    {
                        // is_other (và checkbox+other): chiếm CẢ DÒNG — cột 0 = tiêu đề, cột 1..COLS-1 = ô nhập.
                        if (col != 0) { col = 0; row++; }
                        if (row >= table.RowCount) table.RowCount = row + 1;
                        if (isChk && isOther)
                        {
                            var chk = new CheckEdit(); chk.Properties.Caption = name; chk.Dock = DockStyle.Fill; chk.Tag = d.ID;
                            table.Controls.Add(chk, 0, row);
                            var txt = new TextEdit(); txt.Dock = DockStyle.Fill; txt.Properties.MaxLength = 500; txt.Tag = d.ID;
                            table.Controls.Add(txt, 1, row); table.SetColumnSpan(txt, COLS - 1);
                            this.benhTatCheckMap[d.ID] = chk;
                            this.benhTatTextMap[d.ID] = txt;
                        }
                        else // isOther only -> MemoEdit chiếm 2 cột
                        {
                            var lbl = new LabelControl(); lbl.Text = name; lbl.AutoSizeMode = LabelAutoSizeMode.None; lbl.Dock = DockStyle.Fill;
                            table.Controls.Add(lbl, 0, row);
                            var mem = new MemoEdit(); mem.Dock = DockStyle.Fill; mem.Properties.MaxLength = 500; mem.Tag = d.ID;
                            table.Controls.Add(mem, 1, row); table.SetColumnSpan(mem, COLS - 1);
                            this.benhTatTextMap[d.ID] = mem;
                        }
                        row++; col = 0;
                    }
                }

                // Cao mỗi dòng cố định + Height CỐ ĐỊNH (bỏ AutoSize) -> tránh vòng lặp AutoSize+AutoScroll gây nháy.
                for (int r = 0; r < row; r++)
                {
                    if (r < table.RowStyles.Count) table.RowStyles[r] = new RowStyle(SizeType.Absolute, 26);
                    else table.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
                }
                table.Height = row * 26 + 2;
                table.ResumeLayout(false);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            finally { try { this.panel1.ResumeLayout(true); } catch { } }
        }

        /// <summary>Bật double-buffer cho control (giảm nháy khi repaint/scroll) — qua reflection vì thuộc tính protected.</summary>
        private static void SetDoubleBuffered(Control c)
        {
            try
            {
                if (c == null) return;
                typeof(Control).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .SetValue(c, true, null);
            }
            catch { }
        }

        /// <summary>Nối logic enable-theo-radio cho Mục 2 (gọi 1 lần lúc Load, sau khi control đã có).</summary>
        private void InitMuc2RiskFactorLogic()
        {
            try
            {
                WireRiskGroup(this.rdoTsHutThuoc, new Control[] { this.chkTsHutThuongXuyen, this.chkTsHutDaBo });
                WireRiskGroup(this.rdoTsRuouBia, new Control[] { this.memoTsSoLyRuou, this.chkTsRuouDaBo });
                WireRiskGroup(this.rdoTsMaTuy, new Control[] { this.chkTsMaTuyThuongXuyen, this.chkTsMaTuyDaBo });
                WireRiskGroup(this.rdoTsTheLuc, new Control[] { this.chkTsTheLucThuongXuyen });
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private void WireRiskGroup(RadioGroup radio, Control[] children)
        {
            if (radio == null) return;
            radio.Tag = children; // lưu nhóm con để handler dùng lại
            radio.EditValueChanged -= RiskRadio_EditValueChanged;
            radio.EditValueChanged += RiskRadio_EditValueChanged;
            DisableMouseWheel(radio); // chặn lăn chuột tự đổi giá trị radio
            ApplyRiskGroupState(radio, children); // set trạng thái ban đầu
        }

        /// <summary>Chặn lăn chuột làm đổi giá trị editor (radio/spin) — đánh dấu Handled để không xử lý wheel.</summary>
        private static void DisableMouseWheel(System.Windows.Forms.Control c)
        {
            if (c == null) return;
            c.MouseWheel += (s, e) =>
            {
                var h = e as System.Windows.Forms.HandledMouseEventArgs;
                if (h != null) h.Handled = true;
            };
        }

        /// <summary>Mục 7: bỏ nút spin (up/down) trên các ô SpinEdit — chỉ nhập số bằng bàn phím.</summary>
        private void RemoveMuc7SpinButtons()
        {
            try
            {
                var spins = new DevExpress.XtraEditors.SpinEdit[]
                {
                    this.memoTsSoLanCoThai, this.memoTsSoLanSayThai, this.memoTsSoLanSinhDe, this.memoTsSoConHienSong,
                    this.memoTsSoLanPhaThai, this.memoTsSoLanDeDuThang, this.memoTsSoLanDeNon
                };
                foreach (var s in spins)
                    if (s != null) { s.Properties.Buttons.Clear(); DisableMouseWheel(s); }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void RiskRadio_EditValueChanged(object sender, EventArgs e)
        {
            var radio = sender as RadioGroup;
            if (radio == null) return;
            ApplyRiskGroupState(radio, radio.Tag as Control[]);
        }

        /// <summary>Radio = "Có" (1) -> enable con; ngược lại disable + xóa giá trị con.</summary>
        private void ApplyRiskGroupState(RadioGroup radio, Control[] children)
        {
            try
            {
                if (radio == null || children == null) return;
                bool isYes = radio.EditValue != null && Convert.ToInt32(radio.EditValue) == 1;
                foreach (var c in children)
                {
                    if (c == null) continue;
                    c.Enabled = isYes;
                    if (!isYes) ClearRiskChild(c);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void ClearRiskChild(Control c)
        {
            var chk = c as CheckEdit;
            if (chk != null) { chk.Checked = false; return; }
            var edit = c as BaseEdit;
            if (edit != null) edit.EditValue = null;
        }

    }
}
