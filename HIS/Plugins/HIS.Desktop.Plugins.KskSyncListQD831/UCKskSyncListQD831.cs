/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.KskSyncListQD831.Xml831;
using HIS.Desktop.Plugins.KskSyncListQD831.Sync;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.KskSyncListQD831
{
    /// <summary>
    /// Khám sức khỏe - Đồng bộ QĐ831. Danh sách hồ sơ lấy từ view V_HIS_KSK_PROFILE
    /// (khác chức năng QĐ1551: KHÔNG có cột/lọc "Loại KSK").
    ///
    /// Phạm vi bản này: tìm kiếm + hiển thị lưới + phân trang. CÁC nghiệp vụ đẩy cổng (đồng bộ),
    /// sinh XML và nút cài đặt theo cấu hình TẠM THỜI CHƯA XỬ LÝ — các nút giữ nguyên để đồng bộ
    /// giao diện với QĐ1551 nhưng chỉ báo "đang phát triển".
    /// </summary>
    public partial class UCKskSyncListQD831 : UserControlBase
    {
        #region Declare
        // Cau hinh lien thong HSSK QĐ831 (HIS_CONFIG).
        private const string CONFIG_KEY__HSSK_831 = "MOS.HIS_KSK_SYNC.HSSK_AREA_831_CONNECTION_INFO";

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }

        // Cai dat cong lien thong (btnSettings) — luu trang thai tich local qua ControlState (nhu HisKskSyncList).
        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private bool hsskConfigAvailable;   // MOS.HIS_KSK_SYNC.HSSK_AREA_831_CONNECTION_INFO co du lieu
        private bool syncHssk;              // trang thai tich "day cong HSSK QĐ831"
        private bool hasSavedSyncState;     // da co trang thai luu truoc do
        #endregion

        #region Constructor / Load
        public UCKskSyncListQD831(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
        }

        private void UCKskSyncListQD831_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultControl();
                InitControlState();
                FillDataToGrid();
                LoadDicRefresh();
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private Method
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.KskSyncListQD831.Resources.Lang", typeof(HIS.Desktop.Plugins.KskSyncListQD831.UCKskSyncListQD831).Assembly);
                SetLayoutText(lciConclusionFrom, "lblConclusionFrom.Text");
                SetLayoutText(lciConclusionTo, "lblConclusionTo.Text");
                SetLayoutText(lciSyncStatus, "lblSyncStatus.Text");
                SetText(btnSearch, "btnSearch.Text");
                SetText(btnRefresh, "btnRefresh.Text");
                SetText(btnPreview, "btnPreview.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetText(Control ctrl, string key)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                if (!string.IsNullOrEmpty(value)) ctrl.Text = value;
            }
            catch { }
        }

        private void SetLayoutText(DevExpress.XtraLayout.BaseLayoutItem item, string key)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                if (!string.IsNullOrEmpty(value)) item.Text = value;
            }
            catch { }
        }

        private void SetDefaultControl()
        {
            try
            {
                dtConclusionFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtConclusionTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtKeyWord.Text = "";
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                cboSyncStatus.SelectedIndex = 0;
                btnSync.Enabled = false;
                UpdateSyncBadge(0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null)
                    pageSize = ucPaging.pagingGrid.PageSize;
                else
                    pageSize = (int)ConfigApplications.NumPageSize;

                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                btnSync.Enabled = false;
                UpdateSyncBadge(0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisKskProfileViewFilter filter = new HisKskProfileViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                ApiResultObject<List<V_HIS_KSK_PROFILE>> apiResult =
                    new BackendAdapter(paramCommon).GetRO<List<V_HIS_KSK_PROFILE>>("api/HisKskProfile/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    gridControl1.DataSource = (data != null && data.Count > 0) ? data : null;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckDigit(string s)
        {
            try
            {
                if (string.IsNullOrEmpty(s)) return false;
                return s.All(char.IsDigit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Map control -> filter. KEY_WORD có sẵn trên HisKskProfileViewFilter nên gán trực tiếp.
        /// Các trường lọc còn lại (mã BN/mã điều trị chính xác, khoảng ngày kết luận, trạng thái đẩy)
        /// HIỆN CHƯA có trên HisKskProfileViewFilter — dùng TrySetProp (reflection) để CHỜ BACKEND bổ sung:
        /// khi backend thêm các property dưới đây vào HisKskProfileViewFilter, lọc sẽ tự động có hiệu lực,
        /// không cần sửa lại UI. Cần bổ sung backend: PATIENT_CODE__EXACT, TREATMENT_CODE__EXACT,
        /// CONCLUSION_TIME_FROM, CONCLUSION_TIME_TO, SYNC_RESULT_TYPE.
        /// </summary>
        private void SetFilter(ref HisKskProfileViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 10 && CheckDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    TrySetProp(filter, "TREATMENT_CODE__EXACT", txtTreatmentCode.Text);
                }
                else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && CheckDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    TrySetProp(filter, "PATIENT_CODE__EXACT", txtPatientCode.Text);
                }
                else
                {
                    filter.ORDER_FIELD = "CONCLUSION_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.KEY_WORD = txtKeyWord.Text.Trim();

                    if (dtConclusionFrom.EditValue != null && dtConclusionFrom.DateTime != DateTime.MinValue)
                        TrySetProp(filter, "CONCLUSION_TIME_FROM", Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtConclusionFrom.EditValue).ToString("yyyyMMdd") + "000000"));
                    if (dtConclusionTo.EditValue != null && dtConclusionTo.DateTime != DateTime.MinValue)
                        TrySetProp(filter, "CONCLUSION_TIME_TO", Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtConclusionTo.EditValue).ToString("yyyyMMdd") + "235959"));

                    switch (cboSyncStatus.SelectedIndex)
                    {
                        case 1: TrySetProp(filter, "SYNC_RESULT_TYPE", (short)1); break; // Chua dong bo
                        case 2: TrySetProp(filter, "SYNC_RESULT_TYPE", (short)2); break; // Da dong bo
                        case 3: TrySetProp(filter, "SYNC_RESULT_TYPE", (short)3); break; // That bai
                        default: break; // Tat ca
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Gán property theo tên (reflection), tự ép kiểu (kể cả Nullable). Không có property -> bỏ qua.</summary>
        private static void TrySetProp(object obj, string prop, object value)
        {
            try
            {
                if (obj == null) return;
                var p = obj.GetType().GetProperty(prop);
                if (p == null || !p.CanWrite) return;
                Type u = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                object v = (value == null) ? null : System.Convert.ChangeType(value, u);
                p.SetValue(obj, v, null);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion

        #region Trang thai dong bo (text + mau)
        private static int GetSyncType(V_HIS_KSK_PROFILE row)
        {
            int t = 0;
            try { t = System.Convert.ToInt32(row.SYNC_RESULT_TYPE); }
            catch { t = 0; }
            if (t == 0) t = 1; // Chua tung day (LEFT JOIN null) => Chua dong bo
            return t;
        }

        private static string GetSyncTypeText(int t)
        {
            switch (t)
            {
                case 2: return "Đã đồng bộ";
                case 3: return "Thất bại";
                case 4: return "Có chỉnh sửa";
                default: return "Chưa đồng bộ";
            }
        }
        #endregion

        #region Grid events
        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!(e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)) return;
                V_HIS_KSK_PROFILE data = (V_HIS_KSK_PROFILE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data == null) return;

                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * (ucPaging.pagingGrid.PageSize);
                }
                else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                {
                    try
                    {
                        string dobStr = data.TDL_PATIENT_DOB.ToString();
                        if (GetPropInt(data, "TDL_PATIENT_IS_HAS_NOT_DAY_DOB") == 1)
                            e.Value = dobStr.Length >= 4 ? dobStr.Substring(0, 4) : dobStr;
                        else
                            e.Value = FormatTimeNumber(data.TDL_PATIENT_DOB);
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                }
                else if (e.Column.FieldName == "CONCLUSION_TIME_STR")
                {
                    e.Value = FormatTimeNumber(data.CONCLUSION_TIME);
                }
                else if (e.Column.FieldName == "SYNC_RESULT_TYPE_STR")
                {
                    e.Value = GetSyncTypeText(GetSyncType(data));
                }
                else if (e.Column.FieldName == "SYNC_TIME_STR")
                {
                    e.Value = FormatTimeNumber(data.SYNC_TIME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Dinh dang so thoi gian (yyyyMMddHHmmss / yyyyMMdd) -> chuoi ngay. Null hoac 0 -> rong.
        private static string FormatTimeNumber(object o)
        {
            try
            {
                long v = (o == null) ? 0 : System.Convert.ToInt64(o);
                if (v <= 0) return "";
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(v);
            }
            catch { return ""; }
        }

        private static int GetPropInt(object obj, string name)
        {
            try
            {
                var p = obj.GetType().GetProperty(name);
                if (p == null) return 0;
                var v = p.GetValue(obj, null);
                return v == null ? 0 : System.Convert.ToInt32(v);
            }
            catch { return 0; }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column != colSyncStatus) return;
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_KSK_PROFILE data = (V_HIS_KSK_PROFILE)view.GetRow(e.RowHandle);
                if (data == null) return;
                int t = GetSyncType(data);
                e.Appearance.Options.UseForeColor = true;
                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                if (t == 2)
                    e.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 150, 60);    // xanh la - Da dong bo
                else if (t == 3)
                    e.Appearance.ForeColor = System.Drawing.Color.FromArgb(210, 40, 40);   // do - That bai
                else
                    e.Appearance.ForeColor = System.Drawing.Color.FromArgb(220, 140, 0);   // cam - Chua dong bo
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int count = gridView1.GetSelectedRows().Count(rh => rh >= 0);
                // Nút Đồng bộ ăn theo nút Cài đặt: chỉ bật khi có cấu hình + đã tích cổng.
                btnSync.Enabled = count > 0 && CanSync() && syncHssk;
                UpdateSyncBadge(count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateSyncBadge(int count)
        {
            try { btnSync.Text = "Đồng bộ lên cổng  (" + count + ")"; }
            catch { }
        }

        // Click cot "Xem" -> dung XML cho dong do va hien thi; cot "Đẩy" (day cong) tam thoi chua xu ly.
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var row = gridView1.GetRow(e.RowHandle) as V_HIS_KSK_PROFILE;
                if (row == null) return;
                if (colPreview != null && e.Column == colPreview) ShowPreview(row);
                else if (e.Column == colPush) SyncRecords(new List<V_HIS_KSK_PROFILE> { row });
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        // Xem truoc XML cua 1 ho so: nap du lieu (GetFull + API phu) -> serialize -> hien dialog.
        private void ShowPreview(V_HIS_KSK_PROFILE row)
        {
            try
            {
                WaitingManager.Show();
                Xml831.Data data = Ksk831DataLoader.BuildDataForRow(row);
                string xml = (data != null) ? Ksk831Serializer.ToXml(data) : null;
                WaitingManager.Hide();
                if (string.IsNullOrEmpty(xml))
                {
                    XtraMessageBox.Show("Không lấy được dữ liệu để tạo XML cho hồ sơ này.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (var frm = new Preview.frmKsk831Preview(row, xml))
                {
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Lỗi tạo dữ liệu xem trước." + Environment.NewLine + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Buttons / keys
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try { FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try { SetDefaultControl(); FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void txt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { btnSearch_Click(null, null); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { btnRefresh_Click(null, null); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        // Nut "Xuất XML": loc nhieu ho so (dong tich hoac toan bo tim kiem) -> tao file XML tung ho so.
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try { ExportXml(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void ExportXml()
        {
            string dir;
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Chọn thư mục xuất XML QĐ831";
                if (dlg.ShowDialog() != DialogResult.OK) return;
                dir = dlg.SelectedPath;
            }

            // Loc nhieu: uu tien cac dong TICH; neu khong tich -> toan bo theo dieu kien tim kiem.
            List<V_HIS_KSK_PROFILE> rows = gridView1.GetSelectedRows().Where(rh => rh >= 0)
                .Select(rh => gridView1.GetRow(rh) as V_HIS_KSK_PROFILE).Where(r => r != null).ToList();
            if (rows.Count == 0) rows = FetchAllRows();
            if (rows == null || rows.Count == 0)
            {
                XtraMessageBox.Show("Không có hồ sơ để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool sign = chkSign.Checked;
            if (sign)
                XtraMessageBox.Show("Chức năng ký số chưa được cấu hình trong chức năng này — file sẽ xuất KHÔNG kèm chữ ký (thẻ SIGNATURE để trống).",
                    "Ký số", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            WaitingManager.Show();
            int ok = 0, failed = 0;
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (var row in rows)
                {
                    try
                    {
                        Xml831.Data data = Ksk831DataLoader.BuildDataForRow(row);
                        if (data == null) { failed++; continue; }
                        // TODO: khi bật ký số -> chèn chữ ký vào data.Security.Signature trước khi serialize.
                        string xml = Ksk831Serializer.ToXml(data);
                        if (string.IsNullOrEmpty(xml)) { failed++; continue; }
                        string baseName = MakeFileName(row);
                        string name = baseName; int k = 1;
                        while (used.Contains(name)) name = baseName + "_" + (++k);
                        used.Add(name);
                        File.WriteAllText(Path.Combine(dir, name + ".xml"), xml, new System.Text.UTF8Encoding(false));
                        ok++;
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); failed++; }
                }
            }
            finally { WaitingManager.Hide(); }

            XtraMessageBox.Show(string.Format("Đã xuất {0}/{1} hồ sơ ra XML{2}.{3}Thư mục: {4}",
                ok, rows.Count, (failed > 0 ? (" (" + failed + " lỗi/thiếu dữ liệu)") : ""), Environment.NewLine, dir),
                "Xuất XML", MessageBoxButtons.OK, (failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information));
        }

        /// <summary>Lấy toàn bộ hồ sơ theo điều kiện tìm kiếm (không phân trang).</summary>
        private List<V_HIS_KSK_PROFILE> FetchAllRows()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisKskProfileViewFilter filter = new HisKskProfileViewFilter();
                SetFilter(ref filter);
                ApiResultObject<List<V_HIS_KSK_PROFILE>> rs =
                    new BackendAdapter(param).GetRO<List<V_HIS_KSK_PROFILE>>("api/HisKskProfile/GetView", ApiConsumers.MosConsumer, filter, param);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                return (rs != null && rs.Data != null) ? rs.Data : new List<V_HIS_KSK_PROFILE>();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return new List<V_HIS_KSK_PROFILE>(); }
        }

        private static string MakeFileName(V_HIS_KSK_PROFILE row)
        {
            string pat = row.TDL_PATIENT_CODE ?? "";
            string tre = row.TDL_TREATMENT_CODE ?? "";
            string s = string.Join("_", new[] { pat, tre }.Where(x => !string.IsNullOrEmpty(x)).ToArray());
            if (string.IsNullOrEmpty(s)) s = "KSK831";
            foreach (char c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s;
        }

        // Nut "Đồng bộ lên cổng": day cac dong da chon len cong HSSK QĐ831.
        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                var rows = gridView1.GetSelectedRows().Where(rh => rh >= 0)
                    .Select(rh => gridView1.GetRow(rh) as V_HIS_KSK_PROFILE).Where(r => r != null).ToList();
                if (rows.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn hồ sơ cần đồng bộ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SyncRecords(rows);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>
        /// Đồng bộ danh sách hồ sơ lên cổng HSSK QĐ831. Nạp dữ liệu THEO LÔ (BuildDataForRows), đẩy TỪNG hồ sơ
        /// qua 1 syncer (login 1 lần, tự làm mới token khi &gt;= 2h30). Có tích ký số -&gt; cảnh báo (chưa hỗ trợ ký).
        /// </summary>
        private void SyncRecords(List<V_HIS_KSK_PROFILE> rows)
        {
            if (rows == null || rows.Count == 0) return;

            // Thông tin liên thông LẤY THEO CẤU HÌNH HIS_CONFIG: MOS.HIS_KSK_SYNC.HSSK_AREA_831_CONNECTION_INFO
            // (định dạng <tài khoản>|<mật khẩu>|<địa chỉ gốc>|<api-login>|<api-push>).
            Ksk831SyncConfig cfg = Ksk831SyncConfig.Parse(GetConfigValue(CONFIG_KEY__HSSK_831));
            if (cfg == null)
            {
                XtraMessageBox.Show(
                    "Chưa cấu hình liên thông HSSK QĐ831." + Environment.NewLine +
                    "Vui lòng khai báo HIS_CONFIG: " + CONFIG_KEY__HSSK_831 + Environment.NewLine +
                    "= <tài khoản>|<mật khẩu>|<địa chỉ gốc>|<api-login>|<api-push>",
                    "Không thể đồng bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Chỉ đẩy khi đã TÍCH cổng ở nút Cài đặt (như HisKskSyncList).
            if (!syncHssk)
            {
                XtraMessageBox.Show(
                    "Chưa chọn cổng liên thông để đẩy dữ liệu." + Environment.NewLine +
                    "Vui lòng bấm nút Cài đặt và tích chọn cổng HSSK QĐ831 (đã được cấu hình).",
                    "Không thể đồng bộ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (chkSign.Checked)
                XtraMessageBox.Show("Ký số chưa được cấu hình trong chức năng này — hồ sơ sẽ đẩy KHÔNG kèm chữ ký (SIGNATURE trống).",
                    "Ký số", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            WaitingManager.Show();
            SetSyncUiBusy(true);
            int ok = 0, failed = 0;
            var errors = new List<string>();
            var results = new List<HIS_KSK_PROFILE>();
            int savedCount = 0; string saveError = null;
            long syncTime = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            try
            {
                // Nap du lieu theo lo (filter nhieu) roi day tung ho so.
                var built = Ksk831DataLoader.BuildDataForRows(rows);
                var syncer = new Ksk831Syncer(cfg);
                string nguoiGui = GetNguoiGui();

                foreach (var kv in built)
                {
                    HIS_KSK_PROFILE prof = NewSaveResult(kv.Key, syncTime);
                    try
                    {
                        Xml831.Data data = kv.Value;
                        if (data == null)
                        {
                            prof.SYNC_RESULT_TYPE = 3; prof.SYNC_FAILD_REASON = "Không lấy được dữ liệu";
                            failed++; errors.Add(RowCode(kv.Key) + ": không lấy được dữ liệu");
                        }
                        else
                        {
                            // TODO: khi bật ký số -> chèn chữ ký vào data.Security.Signature trước khi serialize.
                            string xml = Ksk831Serializer.ToXml(data);
                            Ksk831PushResult r = syncer.Push(xml, nguoiGui);
                            if (r != null && r.Success) { prof.SYNC_RESULT_TYPE = 2; ok++; }
                            else
                            {
                                prof.SYNC_RESULT_TYPE = 3;
                                prof.SYNC_FAILD_REASON = (r != null) ? r.Message : "Lỗi không xác định";
                                failed++; errors.Add(RowCode(kv.Key) + ": " + prof.SYNC_FAILD_REASON);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        prof.SYNC_RESULT_TYPE = 3; prof.SYNC_FAILD_REASON = ex.Message;
                        failed++; errors.Add(RowCode(kv.Key) + ": " + ex.Message);
                    }
                    results.Add(prof);

                    // LƯU NGAY trạng thái của TỪNG hồ sơ vừa đẩy (mục 3.3) — bền khi lỗi giữa chừng.
                    int s; string se;
                    if (PersistSyncResults(new List<HIS_KSK_PROFILE> { prof }, out s, out se)) savedCount += s;
                    else if (saveError == null) saveError = RowCode(kv.Key) + ": " + se;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Lỗi khi đồng bộ." + Environment.NewLine + ex.Message, "Đồng bộ thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                WaitingManager.Hide();
                SetSyncUiBusy(false);
            }

            string detail = (errors.Count > 0) ? (Environment.NewLine + string.Join(Environment.NewLine, errors.Take(15))) : "";
            if (!string.IsNullOrEmpty(saveError))
                detail += Environment.NewLine + "(Lưu trạng thái lỗi: " + saveError + ")";
            XtraMessageBox.Show(
                string.Format("Đã đồng bộ {0}/{1} hồ sơ (lưu trạng thái: {2} bản ghi).{3}", ok, rows.Count, savedCount, detail),
                "Kết quả đồng bộ", MessageBoxButtons.OK, (failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information));

            FillDataToGrid();
        }

        private void SetSyncUiBusy(bool busy)
        {
            try
            {
                btnSearch.Enabled = !busy;
                btnRefresh.Enabled = !busy;
                btnPreview.Enabled = !busy;
                gridControl1.Enabled = !busy;
                if (busy) btnSync.Enabled = false;
                else btnSync.Enabled = gridView1.GetSelectedRows().Count(rh => rh >= 0) > 0 && CanSync() && syncHssk;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private string GetConfigValue(string key)
        {
            try
            {
                var cfg = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY == key).FirstOrDefault();
                return cfg != null ? cfg.VALUE : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static string GetNguoiGui()
        {
            // Thong tin nguoi dong bo (ghi log). TODO: dung loginname HIS neu can chinh xac.
            try { return Environment.UserName; } catch { return ""; }
        }

        private static string RowCode(V_HIS_KSK_PROFILE row)
        {
            if (row == null) return "";
            return !string.IsNullOrEmpty(row.TDL_TREATMENT_CODE) ? row.TDL_TREATMENT_CODE
                : (row.TDL_PATIENT_CODE ?? "");
        }

        // Tạo item HIS_KSK_PROFILE để lưu trạng thái đẩy (khóa = ID hồ sơ). SYNC_ID/TRANSACTION_CODE để null
        // (cổng 831 không trả về); backend upsert theo ID.
        private static HIS_KSK_PROFILE NewSaveResult(V_HIS_KSK_PROFILE row, long syncTime)
        {
            return new HIS_KSK_PROFILE
            {
                ID = (row != null) ? row.ID : 0,
                SYNC_TIME = syncTime
            };
        }

        /// <summary>
        /// Lưu trạng thái đẩy vào HIS_KSK_PROFILE: POST api/HisKskProfile/SaveSyncResult, body ApiParam&lt;List&lt;HIS_KSK_PROFILE&gt;&gt;
        /// (BackendAdapter tự bọc CommonParam/ApiData + set header TokenCode). Trả về số bản ghi đã lưu.
        /// </summary>
        private bool PersistSyncResults(List<HIS_KSK_PROFILE> results, out int saved, out string error)
        {
            saved = 0; error = null;
            try
            {
                if (results == null || results.Count == 0) return true;
                CommonParam param = new CommonParam();
                saved = new BackendAdapter(param).Post<int>(
                    "api/HisKskProfile/SaveSyncResult", ApiConsumers.MosConsumer, results,
                    HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);

                bool hasErr = param != null && param.Messages != null && param.Messages.Count > 0;
                if (saved <= 0 || hasErr)
                {
                    error = hasErr ? string.Join(Environment.NewLine, param.Messages) : "Backend không lưu bản ghi nào.";
                    Inventec.Common.Logging.LogSystem.Warn("HSSK831 SAVE RESULT lỗi: " + error);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                error = ex.Message;
                return false;
            }
        }

        private void btnExportPath_Click(object sender, EventArgs e) { ShowNotImplemented(); }
        private void chkSign_CheckedChanged(object sender, EventArgs e) { /* Ky so: cau hinh chua xu ly */ }
        #endregion

        #region Cài đặt cổng liên thông (btnSettings — ControlState như HisKskSyncList)
        private DevExpress.XtraBars.PopupControlContainer popupSync;
        private DevExpress.XtraEditors.CheckEdit chkSyncHssk;

        /// <summary>Đọc trạng thái tích (local) + xác định cổng đã cấu hình chưa.</summary>
        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                if (this.currentModule != null)
                    this.currentControlStateRDO = controlStateWorker.GetData(this.currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == btnSettings.Name)
                        {
                            syncHssk = item.VALUE == "1";
                            hasSavedSyncState = true;
                        }
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            LoadHsskAvailability();
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        /// <summary>Cổng 831 có cấu hình -> auto tích (nếu chưa có trạng thái lưu). Config rỗng -> bỏ tích.</summary>
        private void LoadHsskAvailability()
        {
            try
            {
                hsskConfigAvailable = !string.IsNullOrWhiteSpace(GetConfigValue(CONFIG_KEY__HSSK_831));
                if (!hasSavedSyncState) syncHssk = hsskConfigAvailable;
                if (!hsskConfigAvailable) syncHssk = false;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (!hsskConfigAvailable)
                {
                    XtraMessageBox.Show(
                        "Chưa cấu hình cổng liên thông khám sức khỏe QĐ831." + Environment.NewLine +
                        "Vui lòng khai báo HIS_CONFIG: " + CONFIG_KEY__HSSK_831 + ".",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                EnsureSyncPopup();
                if (chkSyncHssk != null) chkSyncHssk.Checked = syncHssk;
                System.Drawing.Point p = btnSettings.PointToScreen(new System.Drawing.Point(0, btnSettings.Height + 2));
                popupSync.ShowPopup(p);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Popup chứa 1 checkbox chọn cổng HSSK QĐ831; caption hiển thị tên (kèm tài khoản cấu hình).</summary>
        private void EnsureSyncPopup()
        {
            if (popupSync != null) return;

            chkSyncHssk = new DevExpress.XtraEditors.CheckEdit();
            chkSyncHssk.Properties.Caption = BuildSyncCaption();
            chkSyncHssk.Checked = syncHssk;
            chkSyncHssk.CheckedChanged += SyncTarget_CheckedChanged;

            var lc = new DevExpress.XtraLayout.LayoutControl();
            lc.Dock = System.Windows.Forms.DockStyle.Fill;
            lc.BeginUpdate();
            var lci = (DevExpress.XtraLayout.LayoutControlItem)lc.Root.AddItem();
            lci.Control = chkSyncHssk;
            lci.TextVisible = false;
            lc.Root.GroupBordersVisible = false;
            lc.EndUpdate();

            popupSync = new DevExpress.XtraBars.PopupControlContainer();
            popupSync.Name = "popupControlContainerSync";
            popupSync.Manager = this.barManager1;
            popupSync.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            popupSync.Size = new System.Drawing.Size(340, 56);
            popupSync.Controls.Add(lc);
            popupSync.Visible = false;
            this.Controls.Add(popupSync);
        }

        /// <summary>Tên hiển thị cổng: "Liên thông HSSK QĐ831" + tài khoản (nếu đọc được từ cấu hình).</summary>
        private string BuildSyncCaption()
        {
            string caption = "Liên thông HSSK QĐ831";
            try
            {
                var cfg = Ksk831SyncConfig.Parse(GetConfigValue(CONFIG_KEY__HSSK_831));
                if (cfg != null && !string.IsNullOrEmpty(cfg.Username)) caption += " — " + cfg.Username;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return caption;
        }

        private void SyncTarget_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst) return;
                syncHssk = chkSyncHssk.Checked;
                SaveControlStateTarget();
                // Cập nhật ngay trạng thái nút Đồng bộ theo tích cổng.
                try { btnSync.Enabled = gridView1.GetSelectedRows().Count(rh => rh >= 0) > 0 && CanSync() && syncHssk; }
                catch { }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Lưu trạng thái tích cổng (VALUE "1"/"") qua ControlState, key = btnSettings.Name.</summary>
        private void SaveControlStateTarget()
        {
            try
            {
                if (this.controlStateWorker == null || this.currentModule == null) return;
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                var cs = this.currentControlStateRDO
                    .Where(o => o.KEY == btnSettings.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault();
                string val = syncHssk ? "1" : "";
                if (cs != null) cs.VALUE = val;
                else
                {
                    cs = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    cs.KEY = btnSettings.Name;
                    cs.VALUE = val;
                    cs.MODULE_LINK = this.currentModule.ModuleLink;
                    this.currentControlStateRDO.Add(cs);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Có cổng đã cấu hình -> mới cho phép Đồng bộ.</summary>
        private bool CanSync()
        {
            return hsskConfigAvailable;
        }

        private static void ShowNotImplemented()
        {
            try
            {
                XtraMessageBox.Show(
                    "Chức năng đang được phát triển (tạm thời chưa xử lý).",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
        #endregion

        #region Refresh dictionary
        private void LoadDicRefresh()
        {
            try
            {
                if (GlobalVariables.DicRefreshData == null)
                    GlobalVariables.DicRefreshData = new Dictionary<string, RefeshReference>();
                if (currentModule != null && !GlobalVariables.DicRefreshData.ContainsKey(currentModule.RoomId.ToString()))
                    GlobalVariables.DicRefreshData.Add(currentModule.RoomId.ToString(), (HIS.Desktop.Common.RefeshReference)FillDataToGrid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
