/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.KskSyncList.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskSyncList.TestIndexMap
{
    /// <summary>
    /// Bang khai bao noi chi so can lam sang cua HIS voi chi tieu cua mau M4 (cong SYT TP.HCM).
    /// Ben trai: danh muc chi so xet nghiem cua HIS. Ben phai: 34 chi tieu co dinh cua mau M4.
    ///
    /// Form KHONG tu ghi ControlState: tra chuoi JSON ve cho UCKskSyncList qua onSave, vi
    /// ControlStateWorker.SetData XOA moi key khong nam trong danh sach truyen vao -> phai ghi
    /// bang danh sach day du cua man hinh (xem UCKskSyncList.SaveControlStateClsMap).
    /// </summary>
    public partial class frmKskSytClsMap : DevExpress.XtraEditors.XtraForm
    {
        private readonly Action<string> onSave;
        private List<KskSytClsFieldADO> fields;
        private List<V_HIS_TEST_INDEX> testIndexs;
        private Dictionary<string, V_HIS_TEST_INDEX> dicTestIndexByCode;
        private bool isDirty;

        public frmKskSytClsMap(string currentJson, Action<string> onSave)
        {
            InitializeComponent();
            this.onSave = onSave;
            SetIcon();
            InitTestConnectionButton();
            LoadTestIndexCatalog();
            BuildFieldsFromJson(currentJson);
        }

        #region ===== Thử kết nối cổng Sở Y tế TP.HCM =====

        private SimpleButton btnTestSyt;

        /// <summary>TAM THOI — nut day ho so gia, xoa cung KskSytHcmFakeData khi xong kiem thu.</summary>
        private SimpleButton btnPushFake;

        private const string CONFIG_KEY__SYT_HCM_CONNECTION_INFO = "MOS.HIS_KSK_SYNC.SYT_HCM_CONNECTION_INFO";

        /// <summary>
        /// Dựng nút "Thử kết nối" BẰNG MÃ và chèn vào bên trái nút Xuất JSON.
        /// Không sửa file Designer vì bố cục nút ở dạng bảng, thêm cột phải tính lại chỉ số cột.
        /// </summary>
        private void InitTestConnectionButton()
        {
            try
            {
                if (this.layoutControl1 == null || this.lciBtnExport == null) return;

                btnTestSyt = new SimpleButton();
                btnTestSyt.Name = "btnTestSyt";
                btnTestSyt.Text = "Thử kết nối cổng";
                btnTestSyt.ToolTip = "Kiểm tra phiếu truy cập, chữ ký số và danh sách IP cho phép "
                    + "của cổng Sở Y tế TP.HCM. Bản tin gửi đi RỖNG, không chứa dữ liệu bệnh nhân.";
                btnTestSyt.Click += btnTestSyt_Click;

                this.layoutControl1.BeginUpdate();
                try
                {
                    DevExpress.XtraLayout.LayoutControlItem lci =
                        (DevExpress.XtraLayout.LayoutControlItem)this.layoutControl1.AddItem(string.Empty, btnTestSyt);
                    lci.Name = "lciBtnTestSyt";
                    lci.TextVisible = false;
                    lci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                    lci.MinSize = new Size(130, 24);
                    lci.MaxSize = new Size(130, 24);
                    lci.Size = new Size(130, 24);
                    lci.Move(this.lciBtnExport, DevExpress.XtraLayout.Utils.InsertType.Left);

                    // TAM THOI — xoa cung KskSytHcmFakeData khi xong kiem thu.
                    btnPushFake = new SimpleButton();
                    btnPushFake.Name = "btnPushFake";
                    btnPushFake.Text = "Đẩy thử hồ sơ giả";
                    btnPushFake.ToolTip = "Gửi một hồ sơ DỮ LIỆU GIẢ đầy đủ 6 khối lên cổng Sở Y tế TP.HCM "
                        + "để kiểm thử. Không phải dữ liệu bệnh nhân thật.";
                    btnPushFake.Click += btnPushFake_Click;
                    DevExpress.XtraLayout.LayoutControlItem lciFake =
                        (DevExpress.XtraLayout.LayoutControlItem)this.layoutControl1.AddItem(string.Empty, btnPushFake);
                    lciFake.Name = "lciBtnPushFake";
                    lciFake.TextVisible = false;
                    lciFake.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                    lciFake.MinSize = new Size(140, 24);
                    lciFake.MaxSize = new Size(140, 24);
                    lciFake.Size = new Size(140, 24);
                    lciFake.Move(lci, DevExpress.XtraLayout.Utils.InsertType.Right);
                }
                finally { this.layoutControl1.EndUpdate(); }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnTestSyt_Click(object sender, EventArgs e)
        {
            try
            {
                string raw = GetSytConfigRaw();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    XtraMessageBox.Show(
                        "Chưa khai báo bản ghi cấu hình " + CONFIG_KEY__SYT_HCM_CONNECTION_INFO + ".",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                KskSytHcmConfig cfg = KskSytHcmConfig.Parse(raw);
                if (cfg == null || !cfg.CanPush)
                {
                    XtraMessageBox.Show(
                        "Cấu hình còn thiếu trường bắt buộc:\r\n\r\n"
                        + ((cfg != null) ? cfg.DescribeMissing() : "không đọc được cấu hình")
                        + "\r\n\r\nThứ tự các trường, cách nhau bằng dấu |:\r\n"
                        + "mã cơ sở | tài khoản | mật khẩu | địa chỉ xác thực | địa chỉ nghiệp vụ "
                        + "| mã đơn vị gọi | khóa riêng để ký",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Gọi mạng nên chạy trên luồng riêng, không để treo màn hình.
                btnTestSyt.Enabled = false;
                btnTestSyt.Text = "Đang thử...";
                System.Threading.Tasks.Task.Factory.StartNew(() => KskSytHcmPusher.TestConnection(cfg),
                        System.Threading.CancellationToken.None,
                        System.Threading.Tasks.TaskCreationOptions.LongRunning,
                        System.Threading.Tasks.TaskScheduler.Default)
                    .ContinueWith(t => ShowTestResult(t), System.Threading.Tasks.TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                RestoreTestButton();
            }
        }

        /// <summary>
        /// TẠM THỜI — đẩy một hồ sơ dữ liệu giả đầy đủ 6 khối để kiểm thử đường truyền.
        /// Xóa cùng KskSytHcmFakeData khi phần lấy dữ liệu thật xong.
        /// </summary>
        private void btnPushFake_Click(object sender, EventArgs e)
        {
            try
            {
                string raw = GetSytConfigRaw();
                KskSytHcmConfig cfg = KskSytHcmConfig.Parse(raw);
                if (cfg == null || !cfg.CanPush)
                {
                    XtraMessageBox.Show(
                        "Cấu hình còn thiếu trường bắt buộc:\r\n\r\n"
                        + ((cfg != null) ? cfg.DescribeMissing() : "chưa khai báo cấu hình"),
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (XtraMessageBox.Show(
                        "Gửi một hồ sơ DỮ LIỆU GIẢ lên cổng Sở Y tế TP.HCM để kiểm thử?\r\n\r\n"
                        + "Hồ sơ này không phải dữ liệu bệnh nhân thật, nhưng cổng sẽ ghi nhận nó "
                        + "trong danh sách hồ sơ đã nhận.",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                btnPushFake.Enabled = false;
                btnPushFake.Text = "Đang đẩy...";
                System.Threading.Tasks.Task.Factory.StartNew(
                        () => KskSytHcmPusher.Push(cfg, KskSytHcmFakeData.BuildFullBody()),
                        System.Threading.CancellationToken.None,
                        System.Threading.Tasks.TaskCreationOptions.LongRunning,
                        System.Threading.Tasks.TaskScheduler.Default)
                    .ContinueWith(t => ShowPushFakeResult(t), System.Threading.Tasks.TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                RestorePushFakeButton();
            }
        }

        private void ShowPushFakeResult(System.Threading.Tasks.Task<KskSytHcmPushResult> t)
        {
            try
            {
                if (this.IsDisposed || !this.IsHandleCreated) return;
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => ShowPushFakeResult(t)));
                    return;
                }

                RestorePushFakeButton();
                KskSytHcmPushResult r = (t != null && t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                    ? t.Result : null;
                if (r == null)
                {
                    XtraMessageBox.Show("Không thực hiện được. Xem nhật ký để biết chi tiết.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                XtraMessageBox.Show(
                    (r.Success ? "ĐÃ ĐẨY THÀNH CÔNG hồ sơ giả." : "Cổng KHÔNG nhận hồ sơ.")
                    + "\r\n\r\n" + r.ToString()
                    + (r.Success ? "" : "\r\n\r\nXem nhật ký để biết cổng báo thiếu/sai trường nào."),
                    "Kết quả đẩy thử", MessageBoxButtons.OK,
                    r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void RestorePushFakeButton()
        {
            try
            {
                if (btnPushFake == null) return;
                btnPushFake.Enabled = true;
                btnPushFake.Text = "Đẩy thử hồ sơ giả";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void ShowTestResult(System.Threading.Tasks.Task<KskSytHcmPushResult> t)
        {
            try
            {
                if (this.IsDisposed || !this.IsHandleCreated) return;
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => ShowTestResult(t)));
                    return;
                }

                RestoreTestButton();
                KskSytHcmPushResult r = (t != null && t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                    ? t.Result : null;
                if (r == null)
                {
                    XtraMessageBox.Show("Không thực hiện được phép thử. Xem nhật ký để biết chi tiết.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                XtraMessageBox.Show(DescribeTestResult(r), "Kết quả thử kết nối",
                    MessageBoxButtons.OK,
                    r.HttpStatus == 400 || r.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Giải nghĩa kết quả phép thử. Bản tin gửi đi RỖNG nên mã 400 là kết quả MONG ĐỢI:
        /// cổng đã nhận và xác thực được bản tin, chỉ thiếu dữ liệu nghiệp vụ.
        /// </summary>
        private static string DescribeTestResult(KskSytHcmPushResult r)
        {
            string head = "Kết quả: " + r.ToString() + "\r\n\r\n";
            if (r.HttpStatus == 400)
                return head + "HẠ TẦNG ĐÃ THÔNG.\r\n"
                    + "Cổng nhận được bản tin, phiếu truy cập và chữ ký số hợp lệ. Báo lỗi dữ liệu là "
                    + "đúng vì phép thử gửi bản tin rỗng.";
            if (r.Success)
                return head + "HẠ TẦNG ĐÃ THÔNG và cổng nhận cả bản tin rỗng.";
            if (r.HttpStatus == 401)
                return head + "XÁC THỰC THẤT BẠI. Kiểm tra tài khoản và mật khẩu trong cấu hình.";
            if (r.HttpStatus == 403)
                return head + "KHÔNG CÓ QUYỀN TRUY CẬP. Hai nguyên nhân thường gặp:\r\n"
                    + "  · địa chỉ IP của máy chưa được Sở Y tế cho phép;\r\n"
                    + "  · khóa riêng đang khai không khớp khóa công khai đã đăng ký với Sở.";
            if (r.HttpStatus == 0)
                return head + "KHÔNG GỌI TỚI ĐƯỢC cổng. Kiểm tra địa chỉ trong cấu hình và đường mạng. "
                    + "Xem nhật ký để biết lỗi cụ thể.";
            return head + "Xem nhật ký để biết chi tiết.";
        }

        private void RestoreTestButton()
        {
            try
            {
                if (btnTestSyt == null) return;
                btnTestSyt.Enabled = true;
                btnTestSyt.Text = "Thử kết nối cổng";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private static string GetSytConfigRaw()
        {
            try
            {
                // Đọc thẳng từ nguồn để sửa cấu hình là thấy ngay, không phải khởi động lại.
                var list = BackendDataWorker.Get<HIS_CONFIG>(false, true, false, false);
                if (list == null || list.Count == 0) list = BackendDataWorker.Get<HIS_CONFIG>();
                if (list == null) return null;
                foreach (var c in list)
                    if (c != null && c.KEY == CONFIG_KEY__SYT_HCM_CONNECTION_INFO) return c.VALUE;
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        #endregion

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Nap du lieu

        /// <summary>Nap danh muc chi so xet nghiem tu cache (chi lay ban con hieu luc).</summary>
        private void LoadTestIndexCatalog()
        {
            try
            {
                var all = BackendDataWorker.Get<V_HIS_TEST_INDEX>() ?? new List<V_HIS_TEST_INDEX>();
                this.testIndexs = all
                    .Where(o => o != null
                        && !string.IsNullOrWhiteSpace(o.TEST_INDEX_CODE)
                        && o.IS_ACTIVE == 1
                        && (o.IS_DELETE == null || o.IS_DELETE == 0))
                    .OrderBy(o => o.TEST_INDEX_GROUP_NAME)
                    .ThenBy(o => o.NUM_ORDER)
                    .ThenBy(o => o.TEST_INDEX_NAME)
                    .ToList();

                this.dicTestIndexByCode = new Dictionary<string, V_HIS_TEST_INDEX>();
                foreach (var ti in this.testIndexs)
                {
                    if (!this.dicTestIndexByCode.ContainsKey(ti.TEST_INDEX_CODE))
                        this.dicTestIndexByCode.Add(ti.TEST_INDEX_CODE, ti);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.testIndexs = new List<V_HIS_TEST_INDEX>();
                this.dicTestIndexByCode = new Dictionary<string, V_HIS_TEST_INDEX>();
            }
        }

        /// <summary>
        /// Dung 34 dong chi tieu roi do khai bao da luu vao. Chuoi JSON hong -> coi nhu chua khai bao,
        /// KHONG bao loi chan nguoi dung (quy tac R18).
        /// </summary>
        private void BuildFieldsFromJson(string json)
        {
            this.fields = KskSytClsFieldStore.BuildFields();
            if (string.IsNullOrWhiteSpace(json)) return;

            try
            {
                var file = Newtonsoft.Json.JsonConvert.DeserializeObject<KskSytClsMapFileADO>(json);
                if (file == null || file.Items == null) return;
                ApplyItems(file.Items, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Do danh sach cap noi vao luoi. Tra ten/don vi tu danh muc theo ma chi so.
        /// Tra ve so cap BI BO QUA (ma chi tieu la, hoac ma chi so khong co trong danh muc).
        /// </summary>
        private int ApplyItems(List<KskSytClsMapItemADO> items, bool clearBefore)
        {
            int skipped = 0;
            if (clearBefore)
            {
                foreach (var f in this.fields) ClearMap(f);
            }

            foreach (var item in items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.FieldCode)) { skipped++; continue; }

                var field = this.fields.FirstOrDefault(o => o.FieldCode == item.FieldCode);
                if (field == null) { skipped++; continue; }   // ma chi tieu khong thuoc mau M4

                field.Note = item.Note;
                if (string.IsNullOrWhiteSpace(item.TestIndexCode)) { ClearMapKeepNote(field); continue; }

                V_HIS_TEST_INDEX ti;
                if (!this.dicTestIndexByCode.TryGetValue(item.TestIndexCode, out ti))
                {
                    // Ma chi so khong con trong danh muc cua vien -> bo qua, khong gan bua.
                    ClearMapKeepNote(field);
                    skipped++;
                    continue;
                }
                SetMap(field, ti);
            }
            return skipped;
        }

        private void SetMap(KskSytClsFieldADO field, V_HIS_TEST_INDEX ti)
        {
            field.TestIndexCode = ti.TEST_INDEX_CODE;
            field.TestIndexName = ti.TEST_INDEX_NAME;
            field.TestIndexUnitName = ti.TEST_INDEX_UNIT_NAME;
        }

        private void ClearMap(KskSytClsFieldADO field)
        {
            ClearMapKeepNote(field);
            field.Note = null;
        }

        private void ClearMapKeepNote(KskSytClsFieldADO field)
        {
            field.TestIndexCode = null;
            field.TestIndexName = null;
            field.TestIndexUnitName = null;
        }

        #endregion

        #region Su kien form

        private void frmKskSytClsMap_Load(object sender, EventArgs e)
        {
            try
            {
                lblScopeHint.Text = "Khai báo này lưu tại MÁY TRẠM đang dùng, không dùng chung toàn viện."
                    + " Dùng Xuất JSON / Nhập JSON để nhân bản sang máy khác.";
                lblScopeHint.Appearance.ForeColor = Color.FromArgb(180, 95, 6);

                gridTestIndex.DataSource = this.testIndexs;
                gridField.DataSource = this.fields;

                gridViewTestIndex.DoubleClick += gridViewTestIndex_DoubleClick;
                gridViewField.CellValueChanged += gridViewField_CellValueChanged;
                btnAssign.Click += btnAssign_Click;
                btnUnassign.Click += btnUnassign_Click;
                btnExport.Click += btnExport_Click;
                btnImport.Click += btnImport_Click;
                btnSave.Click += btnSave_Click;
                btnClose.Click += btnClose_Click;

                if (this.testIndexs.Count == 0)
                {
                    XtraMessageBox.Show("Danh mục chỉ số xét nghiệm đang trống nên chưa nối được chỉ tiêu nào.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                RefreshSummary();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmKskSytClsMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!this.isDirty) return;
                var rs = XtraMessageBox.Show("Khai báo đã thay đổi nhưng chưa lưu. Bạn có muốn đóng và bỏ thay đổi?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rs != DialogResult.Yes) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestIndex_DoubleClick(object sender, EventArgs e)
        {
            try { AssignFocusedRows(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void gridViewField_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column != colFdNote) return;
                this.isDirty = true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            try { AssignFocusedRows(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnUnassign_Click(object sender, EventArgs e)
        {
            try
            {
                var field = gridViewField.GetFocusedRow() as KskSytClsFieldADO;
                if (field == null)
                {
                    XtraMessageBox.Show("Chọn chỉ tiêu ở lưới bên phải cần bỏ gán.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!field.IsMapped) return;

                ClearMapKeepNote(field);
                this.isDirty = true;
                gridViewField.RefreshData();
                RefreshSummary();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Gan chi so dang chon ben trai vao chi tieu dang chon ben phai.</summary>
        private void AssignFocusedRows()
        {
            var ti = gridViewTestIndex.GetFocusedRow() as V_HIS_TEST_INDEX;
            if (ti == null)
            {
                XtraMessageBox.Show("Chọn một chỉ số xét nghiệm ở lưới bên trái.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var field = gridViewField.GetFocusedRow() as KskSytClsFieldADO;
            if (field == null)
            {
                XtraMessageBox.Show("Chọn chỉ tiêu của cổng ở lưới bên phải cần nối vào.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Mot chi so chi duoc noi vao DUY NHAT 1 chi tieu -> tranh day trung gia tri.
            var used = this.fields.FirstOrDefault(o => o != field
                && !string.IsNullOrWhiteSpace(o.TestIndexCode)
                && o.TestIndexCode == ti.TEST_INDEX_CODE);
            if (used != null)
            {
                XtraMessageBox.Show(string.Format(
                    "Chỉ số \"{0}\" đã được nối vào chỉ tiêu \"{1}\".{2}Bỏ gán ở chỉ tiêu đó trước khi nối sang chỉ tiêu khác.",
                    ti.TEST_INDEX_NAME, used.FieldName, Environment.NewLine),
                    "Trùng chỉ số", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetMap(field, ti);
            this.isDirty = true;
            gridViewField.RefreshData();
            RefreshSummary();

            // Nhay xuong chi tieu ke tiep de khai bao lien tuc cho nhanh.
            if (gridViewField.FocusedRowHandle < gridViewField.RowCount - 1)
                gridViewField.FocusedRowHandle = gridViewField.FocusedRowHandle + 1;
        }

        private void RefreshSummary()
        {
            int total = this.fields.Count;
            int mapped = this.fields.Count(o => o.IsMapped);
            int missing = total - mapped;
            lblSummary.Text = missing == 0
                ? string.Format("Đã nối đủ {0}/{0} chỉ tiêu.", total)
                : string.Format("Đã nối {0}/{1} chỉ tiêu — còn {2} chỉ tiêu chưa nối (sẽ không đẩy lên cổng).",
                    mapped, total, missing);
            lblSummary.Appearance.ForeColor = missing == 0
                ? Color.FromArgb(0, 128, 0)
                : Color.FromArgb(180, 95, 6);
        }

        #endregion

        #region Luu / Xuat / Nhap

        /// <summary>Chi luu cap CO du lieu — chi tieu chua noi va khong ghi chu thi khong luu dong nao.</summary>
        private KskSytClsMapFileADO BuildFile()
        {
            var file = new KskSytClsMapFileADO();
            foreach (var f in this.fields)
            {
                if (!f.IsMapped && string.IsNullOrWhiteSpace(f.Note)) continue;
                file.Items.Add(new KskSytClsMapItemADO
                {
                    FieldCode = f.FieldCode,
                    TestIndexCode = f.TestIndexCode,
                    Note = f.Note
                });
            }
            return file;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewField.CloseEditor();
                gridViewField.UpdateCurrentRow();

                // Vung luu trang thai o may tram dung cau lenh noi chuoi -> dau nhay don lam hong cau lenh.
                var badNote = this.fields.FirstOrDefault(o => !string.IsNullOrEmpty(o.Note) && o.Note.Contains("'"));
                if (badNote != null)
                {
                    XtraMessageBox.Show(string.Format(
                        "Ghi chú của chỉ tiêu \"{0}\" có chứa dấu nháy đơn ('). Vui lòng bỏ ký tự này.",
                        badNote.FieldName),
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(BuildFile());
                if (this.onSave != null) this.onSave(json);

                this.isDirty = false;
                XtraMessageBox.Show("Đã lưu khai báo nối chỉ số cận lâm sàng cho máy trạm này.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Lưu khai báo không thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewField.CloseEditor();
                gridViewField.UpdateCurrentRow();

                var file = BuildFile();
                if (file.Items.Count == 0)
                {
                    XtraMessageBox.Show("Chưa nối chỉ tiêu nào nên không có gì để xuất.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Tệp JSON (*.json)|*.json";
                    dlg.FileName = "NoiChiSoCanLamSang_SYT_HCM_M4.json";
                    if (dlg.ShowDialog() != DialogResult.OK) return;

                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(file, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(dlg.FileName, json, System.Text.Encoding.UTF8);
                }

                XtraMessageBox.Show(string.Format("Đã xuất {0} cặp nối.", file.Items.Count),
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Xuất tệp không thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                string path;
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "Tệp JSON (*.json)|*.json";
                    if (dlg.ShowDialog() != DialogResult.OK) return;
                    path = dlg.FileName;
                }

                KskSytClsMapFileADO file;
                try
                {
                    string json = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
                    file = Newtonsoft.Json.JsonConvert.DeserializeObject<KskSytClsMapFileADO>(json);
                }
                catch (Exception exRead)
                {
                    Inventec.Common.Logging.LogSystem.Warn(exRead);
                    XtraMessageBox.Show("Tệp không đúng định dạng khai báo nối chỉ số.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (file == null || file.Items == null || file.Items.Count == 0)
                {
                    XtraMessageBox.Show("Tệp không có cặp nối nào.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirm = XtraMessageBox.Show(string.Format(
                    "Tệp có {0} cặp nối. Nhập vào sẽ GHI ĐÈ toàn bộ khai báo hiện tại trên máy này.{1}Bạn có chắc chắn?",
                    file.Items.Count, Environment.NewLine),
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                int skipped = ApplyItems(file.Items, true);
                this.isDirty = true;
                gridViewField.RefreshData();
                RefreshSummary();

                string msg = string.Format("Đã nhập khai báo từ tệp. Đã nối {0}/{1} chỉ tiêu.",
                    this.fields.Count(o => o.IsMapped), this.fields.Count);
                if (skipped > 0)
                {
                    msg += Environment.NewLine + string.Format(
                        "Bỏ qua {0} cặp vì mã chỉ tiêu không thuộc mẫu M4 hoặc mã chỉ số không có trong danh mục của viện.",
                        skipped);
                }
                msg += Environment.NewLine + "Bấm Lưu để ghi khai báo cho máy trạm này.";
                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Nhập tệp không thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try { this.Close(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        #endregion
    }
}
