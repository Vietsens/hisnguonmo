/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Ô chọn bệnh của tab Khám lâm sàng HCM — dùng DANH MỤC ICD CỦA CỔNG, không dùng của HIS.
 *
 * VÌ SAO KHÔNG DÙNG LẠI Ô CHỌN BỆNH CÓ SẴN:
 * Ô có sẵn của phần mềm đọc danh mục ICD của HIS (khoảng 16 nghìn mục). Danh mục của cổng chỉ có
 * 11.368 mục và ĐÁNH SỐ RIÊNG — cổng nhận Id của nó, không nhận mã bệnh. Cho người dùng chọn từ
 * danh mục của HIS thì có những bệnh cổng không có, đẩy lên bị từ chối hoặc mất chẩn đoán.
 *
 * CÁCH LƯU: cột `*_ICD_CODE` lưu DANH SÁCH Id của cổng, ngăn bởi dấu phẩy ("2381,5104"), đúng dạng
 * cổng nhận. Cột `*_ICD_NAME` lưu tên bệnh ngăn bởi "; " để người đọc và bản in hiểu được.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>Một mục trong danh mục ICD của cổng — giữ CẢ Id, vì cổng nhận Id chứ không nhận mã.</summary>
    public class SytIcdItem
    {
        /// <summary>Mã định danh của cổng — đây là thứ cổng nhận và là thứ được lưu.</summary>
        public long ID { get; set; }

        /// <summary>
        /// Tên bệnh NGUYÊN VĂN của cổng, đã gồm cả mã ở đầu: "V81.7 -- Hành khách đi tàu...".
        /// Không tách mã ra riêng: cột lưu là Id nên mã không dùng để làm gì.
        /// </summary>
        public string TEN_BENH { get; set; }
    }

    /// <summary>
    /// Bảng tìm chọn nhiều bệnh từ danh mục của cổng.
    ///
    /// Dựng bằng mã, không qua Designer: bảng chỉ có một lưới và hai nút, thêm tệp Designer vào dự án
    /// đổi lấy vài dòng bố cục là không đáng.
    /// </summary>
    public class frmSytIcdPicker : XtraForm
    {
        private readonly GridControl grid;
        private readonly GridView view;

        /// <summary>Các mục người dùng đã chọn khi bấm Đồng ý. Rỗng nếu bấm Bỏ qua.</summary>
        public List<SytIcdItem> SelectedItems { get; private set; }

        public frmSytIcdPicker(List<SytIcdItem> source, List<long> currentIds)
        {
            this.SelectedItems = new List<SytIcdItem>();
            this.Text = "Chọn bệnh — danh mục của Sở Y tế TP.HCM";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(760, 520);
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            view = new GridView();
            view.OptionsBehavior.Editable = false;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.ShowIndicator = false;
            // Hàng lọc ngay dưới tiêu đề cột — danh mục hơn 11 nghìn mục, không lọc thì không tìm nổi.
            view.OptionsView.ShowAutoFilterRow = true;
            view.OptionsSelection.MultiSelect = true;
            view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;

            grid = new GridControl();
            grid.Dock = DockStyle.Fill;
            grid.MainView = view;
            grid.ViewCollection.Add(view);
            grid.DataSource = source ?? new List<SytIcdItem>();

            // Đúng 3 cột: ô tích (lưới tự thêm khi chọn nhiều), Mã định danh, Tên bệnh.
            //
            // DỰNG CỘT TƯỜNG MINH, không dùng PopulateColumns rồi tìm cột để gán nhãn: lúc gọi thì
            // lưới chưa dựng cột nên tìm không ra, và cột sinh ra sau đó lấy nhãn mặc định là TÊN
            // TRƯỜNG ("ID", "TEN_BENH") — người dùng nhìn thấy tên trường thay vì nhãn tiếng Việt.
            view.OptionsBehavior.AutoPopulateColumns = false;

            DevExpress.XtraGrid.Columns.GridColumn colId = view.Columns.AddVisible("ID", "Mã định danh");
            DevExpress.XtraGrid.Columns.GridColumn colName = view.Columns.AddVisible("TEN_BENH", "Tên bệnh");

            // Ô tích và mã định danh chỉ cần vừa đủ; tên bệnh dài nên chiếm hết phần còn lại.
            view.OptionsSelection.CheckBoxSelectorColumnWidth = 32;
            view.OptionsView.ColumnAutoWidth = true;          // cột cuối giãn hết bề ngang còn lại
            colId.Width = 95;
            colId.OptionsColumn.FixedWidth = true;            // không giãn theo bề ngang bảng
            colId.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            colName.Width = 600;

            SimpleButton btnOk = new SimpleButton();
            btnOk.Text = "Đồng ý";
            btnOk.Width = 100;
            btnOk.Click += delegate { AcceptSelection(); };

            SimpleButton btnCancel = new SimpleButton();
            btnCancel.Text = "Bỏ qua";
            btnCancel.Width = 100;
            btnCancel.Click += delegate { this.DialogResult = DialogResult.Cancel; this.Close(); };

            PanelControl bottom = new PanelControl();
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 40;
            bottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            btnCancel.Dock = DockStyle.Right;
            btnOk.Dock = DockStyle.Right;
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnOk);

            this.Controls.Add(grid);
            this.Controls.Add(bottom);

            // Đóng bằng phím Enter / Esc như các bảng khác của phần mềm.
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            // Tích sẵn các bệnh đang chọn — làm SAU KHI bảng hiện ra, xem OnShown.
            this.pendingIds = currentIds;
            this.source = source;
        }

        private List<long> pendingIds;
        private List<SytIcdItem> source;

        /// <summary>
        /// Tích sẵn những bệnh đang chọn, để người dùng thấy và bỏ bớt được thay vì chọn lại từ đầu.
        ///
        /// PHẢI làm ở đây, KHÔNG làm trong hàm dựng: lúc dựng thì lưới chưa gắn vào bảng nên chưa
        /// tạo dòng nào, mọi lệnh tích đều rơi vào chỗ trống và bảng mở ra trắng trơn.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            PreselectRows(source, pendingIds);
        }


        private void PreselectRows(List<SytIcdItem> source, List<long> currentIds)
        {
            try
            {
                if (source == null || currentIds == null || currentIds.Count == 0) return;

                view.BeginSelection();
                try
                {
                    view.ClearSelection();
                    // Duyệt theo DÒNG ĐANG HIỆN của lưới, không theo thứ tự danh sách nguồn: lưới có
                    // sắp xếp và hàng lọc riêng nên số thứ tự trong danh sách nguồn KHÔNG trùng số
                    // hiệu dòng — tra theo số thứ tự sẽ tích nhầm dòng khác hoặc trượt hết.
                    int done = 0;
                    for (int rh = 0; rh < view.DataRowCount; rh++)
                    {
                        SytIcdItem it = view.GetRow(rh) as SytIcdItem;
                        if (it == null || !currentIds.Contains(it.ID)) continue;
                        view.SelectRow(rh);
                        done++;
                    }

                    if (done > 0) view.FocusedRowHandle = view.GetSelectedRows()[0];
                    if (done < currentIds.Count)
                        LogSystem.Warn("SytHcm: bang chon benh chi tich duoc " + done + "/"
                            + currentIds.Count + " benh da chon — so con lai khong co trong danh muc");
                }
                finally { view.EndSelection(); }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void AcceptSelection()
        {
            try
            {
                SelectedItems = new List<SytIcdItem>();
                int[] rows = view.GetSelectedRows();
                if (rows != null)
                {
                    foreach (int rh in rows)
                    {
                        SytIcdItem it = view.GetRow(rh) as SytIcdItem;
                        if (it != null) SelectedItems.Add(it);
                    }
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }
    }

    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>
        /// Danh mục ICD của cổng, GIỮ Id. Khác với `sytIcdSource` (kiểu HIS_ICD) vì kiểu đó không có
        /// chỗ chứa Id của cổng.
        /// </summary>
        private static List<SytIcdItem> sytIcdItems = null;

        /// <summary>Trạng thái của từng ô chọn bệnh: các mục đang chọn.</summary>
        private readonly Dictionary<Control, List<SytIcdItem>> sytIcdSelected
            = new Dictionary<Control, List<SytIcdItem>>();

        /// <summary>Dấu ngăn giữa các Id khi lưu — đúng dạng cổng nhận.</summary>
        private const string SYT_ICD_ID_SEP = ",";

        /// <summary>Dấu ngăn giữa các tên bệnh khi lưu — chỉ để người đọc.</summary>
        private const string SYT_ICD_NAME_SEP = "; ";

        /// <summary>
        /// Dựng một ô chọn bệnh: ô chọn (chọn nhanh 1 bệnh) + nút "..." (chọn nhiều bệnh).
        ///
        /// Chọn ở ô chọn là THAY cả danh sách bằng một bệnh — đúng với trường hợp hay gặp nhất là
        /// mỗi mục khám một chẩn đoán. Cần nhiều bệnh thì bấm "...".
        /// </summary>
        private UserControl BuildSytIcdEditor(PanelControl pnl)
        {
            try
            {
                if (pnl == null) return null;

                UserControl host = new UserControl();
                host.Dock = DockStyle.Fill;

                GridLookUpEdit cbo = new GridLookUpEdit();
                cbo.Name = pnl.Name + "_cbo";
                cbo.Dock = DockStyle.Fill;
                cbo.MenuManager = this.barManager1;
                cbo.Properties.NullText = "Chọn bệnh...";
                cbo.Properties.DataSource = sytIcdItems ?? new List<SytIcdItem>();
                cbo.Properties.DisplayMember = "TEN_BENH";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.View.OptionsView.ShowGroupPanel = false;
                cbo.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.PopupFormWidth = 620;
                cbo.EditValueChanged += CboSytIcd_EditValueChanged;

                SimpleButton btn = new SimpleButton();
                btn.Name = pnl.Name + "_btn";
                btn.Text = "...";
                btn.Width = HCM_ICD_BTN_W;
                btn.Dock = DockStyle.Right;
                btn.Tag = host;
                btn.Click += BtnSytIcdChoose_Click;

                host.Controls.Add(cbo);
                pnl.Controls.Add(host);
                pnl.Controls.Add(btn);
                btn.BringToFront();

                sytIcdSelected[host] = new List<SytIcdItem>();
                return host;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private GridLookUpEdit ComboOf(Control host)
        {
            if (host == null) return null;
            foreach (Control c in host.Controls)
            {
                GridLookUpEdit cbo = c as GridLookUpEdit;
                if (cbo != null) return cbo;
            }
            return null;
        }

        /// <summary>Chọn ở ô chọn -> danh sách chỉ còn một bệnh đó.</summary>
        private void CboSytIcd_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo == null || sytIcdSyncing) return;
                Control host = cbo.Parent;
                if (host == null) return;

                var list = new List<SytIcdItem>();
                if (cbo.EditValue != null && sytIcdItems != null)
                {
                    long id;
                    if (long.TryParse(cbo.EditValue.ToString(), out id))
                    {
                        SytIcdItem it = sytIcdItems.FirstOrDefault(o => o.ID == id);
                        if (it != null) list.Add(it);
                    }
                }
                sytIcdSelected[host] = list;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Chặn vòng lặp khi ta tự gán giá trị vào ô chọn.</summary>
        private bool sytIcdSyncing;

        /// <summary>Nút "..." — mở bảng chọn NHIỀU bệnh từ danh mục của cổng.</summary>
        private void BtnSytIcdChoose_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton btn = sender as SimpleButton;
                if (btn == null) return;
                Control host = btn.Tag as Control;
                if (host == null || !host.Enabled) return;

                if (sytIcdItems == null || sytIcdItems.Count == 0)
                {
                    XtraMessageBox.Show("Chưa tải được danh mục bệnh của Sở Y tế TP.HCM. "
                        + "Vui lòng thử lại sau ít phút.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<SytIcdItem> current = ListOfHost(host);
                var currentIds = current.Select(o => o.ID).ToList();

                using (frmSytIcdPicker frm = new frmSytIcdPicker(sytIcdItems, currentIds))
                {
                    if (frm.ShowDialog() != DialogResult.OK) return;
                    ApplySytIcdSelection(host, frm.SelectedItems);
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private List<SytIcdItem> ListOfHost(Control host)
        {
            List<SytIcdItem> rs;
            if (host != null && sytIcdSelected.TryGetValue(host, out rs) && rs != null) return rs;
            return new List<SytIcdItem>();
        }

        /// <summary>Ghi danh sách đã chọn vào ô và cập nhật phần hiển thị.</summary>
        private void ApplySytIcdSelection(Control host, List<SytIcdItem> items)
        {
            try
            {
                if (host == null) return;
                var list = items ?? new List<SytIcdItem>();
                sytIcdSelected[host] = list;

                GridLookUpEdit cbo = ComboOf(host);
                if (cbo == null) return;

                sytIcdSyncing = true;
                try
                {
                    // Một bệnh -> ô chọn sáng đúng mục đó. Nhiều bệnh -> ô chọn không thể hiện được
                    // nhiều mục, nên bỏ chọn và ghi cả danh sách vào phần chữ gợi ý để vẫn đọc được.
                    if (list.Count == 1)
                    {
                        cbo.EditValue = list[0].ID;
                        cbo.Properties.NullText = "Chọn bệnh...";
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Properties.NullText = (list.Count == 0)
                            ? "Chọn bệnh..."
                            : string.Join(SYT_ICD_NAME_SEP, list.Select(o => o.TEN_BENH).ToArray());
                    }
                }
                finally { sytIcdSyncing = false; }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }


        /// <summary>
        /// Đổi nguồn danh mục cho một ô chọn bệnh — dùng khi danh mục của cổng về sau lúc mở tab.
        /// Không dựng lại control nên không mất gì, chỉ nạp lại nguồn rồi vẽ lại phần đang chọn.
        /// </summary>
        private void RebindSytIcdSource(UserControl host)
        {
            try
            {
                GridLookUpEdit cbo = ComboOf(host);
                if (cbo == null) return;
                sytIcdSyncing = true;
                try { cbo.Properties.DataSource = sytIcdItems ?? new List<SytIcdItem>(); }
                finally { sytIcdSyncing = false; }
                ApplySytIcdSelection(host, ListOfHost(host));
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đọc giá trị để lưu: danh sách Id (ngăn bởi dấu phẩy) và danh sách tên bệnh.</summary>
        private void GetSytIcdValue(UserControl host, out string icdIds, out string icdNames)
        {
            icdIds = null;
            icdNames = null;
            try
            {
                List<SytIcdItem> list = ListOfHost(host);
                if (list.Count == 0) return;
                icdIds = string.Join(SYT_ICD_ID_SEP, list.Select(o => o.ID.ToString()).ToArray());
                icdNames = string.Join(SYT_ICD_NAME_SEP, list.Select(o => o.TEN_BENH ?? "").ToArray());
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đổ giá trị đã lưu vào ô. `icdIds` là danh sách Id của cổng ngăn bởi dấu phẩy.
        ///
        /// Dữ liệu lưu TRƯỚC bản này là MÃ BỆNH chứ không phải Id — mã đó không tra ra mục nào nên ô
        /// hiện trống, và ghi cảnh báo để người vận hành biết hồ sơ cũ cần nhập lại chẩn đoán.
        /// </summary>
        private void SetSytIcdValue(UserControl host, string icdIds, string icdNames)
        {
            try
            {
                if (host == null) return;
                var list = new List<SytIcdItem>();
                if (!string.IsNullOrWhiteSpace(icdIds) && sytIcdItems != null)
                {
                    foreach (string part in icdIds.Split(new char[] { ',', ';' }))
                    {
                        string t = (part ?? "").Trim();
                        if (t.Length == 0) continue;
                        long id;
                        if (!long.TryParse(t, out id))
                        {
                            LogSystem.Warn("SytHcm: gia tri ICD da luu \"" + t + "\" khong phai Id cua"
                                + " cong (du lieu cu luu ma benh) -> khong do duoc vao o chon benh");
                            continue;
                        }
                        SytIcdItem it = sytIcdItems.FirstOrDefault(o => o.ID == id);
                        if (it != null) list.Add(it);
                        else LogSystem.Warn("SytHcm: Id benh " + id + " khong co trong danh muc cua cong");
                    }
                }
                ApplySytIcdSelection(host, list);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Khóa hoặc mở một ô chọn bệnh, gồm cả nút "...".</summary>
        private void SetSytIcdEnabled(UserControl host, bool enabled)
        {
            try
            {
                if (host == null) return;
                host.Enabled = enabled;
                GridLookUpEdit cbo = ComboOf(host);
                if (cbo != null) cbo.Enabled = enabled;
                if (host.Parent != null)
                {
                    foreach (Control c in host.Parent.Controls)
                    {
                        if (c is SimpleButton) c.Enabled = enabled;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
    }
}
