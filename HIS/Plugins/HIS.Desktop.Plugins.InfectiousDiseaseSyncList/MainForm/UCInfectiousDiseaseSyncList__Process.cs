/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Dựng UI + tìm kiếm/phân trang (V_HIS_TREATMENT) + cột trạng thái đẩy (đối soát) + cột thao tác Xem/Đẩy
 * + đồng bộ hàng loạt/riêng lẻ + mở plugin chi tiết. Mô hình tham khảo KskSyncListQD831.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Config;
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Worker;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.MainForm
{
    public partial class UCInfectiousDiseaseSyncList
    {
        #region Search + Paging
        public void SearchList()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                try { pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE"); }
                catch { pageSize = 50; }
                if (pageSize <= 0) pageSize = 50;
                currentPageSize = pageSize;

                LoadListPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = listRowCount;
                param.Count = listDataTotal;
                ucPaging.Init(LoadListPaging, param, pageSize, this.grdList);
                UpdateSyncBadge();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListPaging(object param)
        {
            try
            {
                listData = new List<EcdsSyncGridRowADO>();
                listStartPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(listStartPage, limit);

                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                SetListFilter(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "IN_TIME";

                gvList.BeginUpdate();
                grdList.DataSource = null;
                var apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                var rows = new List<EcdsSyncGridRowADO>();
                if (apiResult != null && apiResult.Data != null)
                {
                    var data = (List<V_HIS_TREATMENT>)apiResult.Data;
                    listDataTotal = apiResult.Param != null ? (apiResult.Param.Count ?? 0) : 0;
                    int stt = listStartPage;
                    foreach (var v in data)
                    {
                        stt++;
                        rows.Add(new EcdsSyncGridRowADO
                        {
                            STT = stt,
                            TREATMENT_ID = v.ID,
                            PATIENT_ID = v.PATIENT_ID,
                            TREATMENT_CODE = v.TREATMENT_CODE,
                            PATIENT_CODE = v.TDL_PATIENT_CODE,
                            PATIENT_NAME = v.TDL_PATIENT_NAME,
                            ICD_CODE = v.ICD_CODE,
                            IN_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(v.IN_TIME),
                            PUSH_STATE = 0,
                            PUSH_STATE_STR = PushStateText(0),
                            Source = v
                        });
                    }
                }
                else listDataTotal = 0;

                // Đối soát trạng thái đẩy (best-effort — backend chưa sẵn thì để "Chưa đồng bộ").
                ReconcilePushState(rows);

                // Lọc theo trạng thái (client-side trên trang hiện tại).
                rows = ApplyStatusFilter(rows);

                listData = rows;
                listRowCount = rows.Count;
                grdList.DataSource = rows;
                gvList.EndUpdate();
                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetListFilter(ref HisTreatmentViewFilter filter)
        {
            try
            {
                string treatmentCode = (txtSearchTreatmentCode.Text ?? "").Trim();
                string patientCode = (txtSearchPatientCode.Text ?? "").Trim();

                if (!string.IsNullOrEmpty(treatmentCode))
                {
                    filter.TREATMENT_CODE__EXACT = treatmentCode;
                }
                else if (!string.IsNullOrEmpty(patientCode))
                {
                    filter.PATIENT_CODE__EXACT = patientCode;
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtSearchPatientName.Text.Trim()))
                        filter.PATIENT_NAME = txtSearchPatientName.Text.Trim();
                    if (dteSearchFrom.EditValue != null)
                        filter.IN_TIME_FROM = Int64.Parse(dteSearchFrom.DateTime.ToString("yyyyMMdd000000"));
                    if (dteSearchTo.EditValue != null)
                        filter.IN_TIME_TO = Int64.Parse(dteSearchTo.DateTime.ToString("yyyyMMdd235959"));
                }
                // TODO: lọc theo danh sách ICD bệnh truyền nhiễm.
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Lọc theo combo trạng thái (client-side trên trang hiện tại).</summary>
        private List<EcdsSyncGridRowADO> ApplyStatusFilter(List<EcdsSyncGridRowADO> rows)
        {
            try
            {
                if (rows == null || cboSyncStatus == null) return rows;
                switch (cboSyncStatus.SelectedIndex)
                {
                    case 1: return rows.Where(r => r.PUSH_STATE == 0).ToList();   // Chưa đồng bộ
                    case 2: return rows.Where(r => r.PUSH_STATE == 1).ToList();   // Đã đồng bộ
                    case 3: return rows.Where(r => r.PUSH_STATE == 2).ToList();   // Thất bại
                    default: return rows;                                          // Tất cả
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return rows; }
        }

        /// <summary>
        /// Đối soát trạng thái đẩy: GetView V_HIS_ECDS_DISEASE_CASE theo mã hồ sơ (HisEcdsDiseaseCaseViewFilter)
        /// → map PUSH_STATE + mã ca + ID bản ghi (dùng cho cập nhật kết quả đẩy). Best-effort.
        /// </summary>
        private void ReconcilePushState(List<EcdsSyncGridRowADO> rows)
        {
            try
            {
                caseIdByTreatment.Clear();
                if (rows == null || rows.Count == 0) return;
                var codes = rows.Select(r => r.TREATMENT_CODE).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                if (codes.Count == 0) return;

                CommonParam param = new CommonParam();
                var filter = new MOS.Filter.HisEcdsDiseaseCaseViewFilter { TREATMENT_CODES = codes };
                var recs = new BackendAdapter(param).Get<List<V_HIS_ECDS_DISEASE_CASE>>(
                    HisRequestUriStore.HIS_ECDS_GET_VIEW, ApiConsumers.MosConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);

                if (recs != null && recs.Count > 0)
                {
                    var map = recs.Where(o => o != null)
                        .GroupBy(o => o.TREATMENT_ID)
                        .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.LAST_PUSH_TIME ?? 0).First());

                    foreach (var r in rows)
                    {
                        V_HIS_ECDS_DISEASE_CASE rec;
                        if (map.TryGetValue(r.TREATMENT_ID, out rec) && rec != null)
                        {
                            r.PUSH_STATE = (int)(rec.PUSH_STATE ?? 0);
                            r.ECDS_CASE_CODE = rec.ECDS_CASE_CODE;
                            if (rec.ID > 0) caseIdByTreatment[r.TREATMENT_ID] = rec.ID;
                        }
                        r.PUSH_STATE_STR = PushStateText(r.PUSH_STATE);
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private static string PushStateText(int s)
        {
            switch (s)
            {
                case 1: return "Đã đồng bộ";
                case 2: return "Thất bại";
                default: return "Chưa đồng bộ";
            }
        }
        #endregion

        #region Grid events (màu trạng thái + cột thao tác)
        private void gvList_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var row = gvList.GetRow(e.RowHandle) as EcdsSyncGridRowADO;
                if (row == null) return;

                if (e.Column == colSyncStatus)
                {
                    e.Appearance.Options.UseForeColor = true;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    if (row.PUSH_STATE == 1) e.Appearance.ForeColor = Color.FromArgb(0, 150, 60);     // xanh — Đã đồng bộ
                    else if (row.PUSH_STATE == 2) e.Appearance.ForeColor = Color.FromArgb(210, 40, 40); // đỏ — Thất bại
                    else e.Appearance.ForeColor = Color.FromArgb(220, 140, 0);                          // cam — Chưa đồng bộ
                }
                else if (e.Column == colView || e.Column == colPush)
                {
                    e.Appearance.Options.UseForeColor = true;
                    e.Appearance.ForeColor = Color.Blue;
                    e.Appearance.Options.UseTextOptions = true;
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void gvList_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var row = gvList.GetRow(e.RowHandle) as EcdsSyncGridRowADO;
                if (row == null || row.Source == null) return;

                if (e.Column == colView) OpenDetailForTreatment(row.Source);
                else if (e.Column == colPush) SyncSingle(row);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
        #endregion

        #region Mở plugin chi tiết (inter-plugin)
        private void OpenDetailForFocusedRow()
        {
            try
            {
                var row = gvList.GetFocusedRow() as EcdsSyncGridRowADO;
                if (row == null || row.Source == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn 1 ca bệnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                OpenDetailForTreatment(row.Source);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void OpenDetailForTreatment(V_HIS_TREATMENT v)
        {
            try
            {
                if (v == null) return;
                var t = MapToTreatment(v);
                var args = new List<object>();
                args.Add(t);
                args.Add(new HIS.Desktop.Common.RefeshReference(OnDetailRefresh));

                long roomId = moduleData != null ? moduleData.RoomId : 0;
                long roomTypeId = moduleData != null ? moduleData.RoomTypeId : 0;
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                    ModuleLinkString.InfectiousDiseaseReport, roomId, roomTypeId, args);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void OnDetailRefresh()
        {
            try { SearchList(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private HIS_TREATMENT MapToTreatment(V_HIS_TREATMENT v)
        {
            var t = new HIS_TREATMENT();
            try
            {
                t.ID = v.ID;
                t.TREATMENT_CODE = v.TREATMENT_CODE;
                t.PATIENT_ID = v.PATIENT_ID;
                t.TDL_PATIENT_CODE = v.TDL_PATIENT_CODE;
                t.TDL_PATIENT_NAME = v.TDL_PATIENT_NAME;
                t.TDL_PATIENT_DOB = v.TDL_PATIENT_DOB;
                t.TDL_PATIENT_GENDER_ID = v.TDL_PATIENT_GENDER_ID;
                t.TDL_PATIENT_GENDER_NAME = v.TDL_PATIENT_GENDER_NAME;
                t.ICD_CODE = v.ICD_CODE;
                t.ICD_NAME = v.ICD_NAME;
                t.IN_TIME = v.IN_TIME;
                t.OUT_TIME = v.OUT_TIME;
                t.LAST_DEPARTMENT_ID = v.LAST_DEPARTMENT_ID;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return t;
        }
        #endregion

        #region Đồng bộ (hàng loạt + riêng lẻ)
        private void UpdateSyncBadge()
        {
            try
            {
                if (btnSyncList == null || gvList == null) return;
                int count = gvList.GetSelectedRows().Count(rh => rh >= 0);
                btnSyncList.Text = "Đồng bộ lên cổng (" + count + ")";
                btnSyncList.Enabled = count > 0;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void SetSyncBusy(bool busy)
        {
            try
            {
                isSyncing = busy;
                btnSearch.Enabled = !busy;
                grdList.Enabled = !busy;
                btnEdit.Enabled = !busy;
                if (busy) btnSyncList.Enabled = false; else UpdateSyncBadge();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Đẩy tay: lấy các dòng đang chọn, xác nhận, rồi đẩy (có popup kết quả).</summary>
        private void SyncSelected()
        {
            try
            {
                var rows = GetSelectedSources();
                if (rows.Count == 0) return;

                if (!EcdsConfigCFG.IsValid())
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.ChuaCauHinhKetNoiEcds,
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (XtraMessageBox.Show("Đồng bộ " + rows.Count + " ca bệnh đã chọn lên cổng ECDS?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                RunSyncForRows(rows, false);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                SetSyncBusy(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Đẩy riêng 1 ca (từ cột "Đẩy").</summary>
        private void SyncSingle(EcdsSyncGridRowADO row)
        {
            try
            {
                if (row == null || row.Source == null) return;
                if (!EcdsConfigCFG.IsValid())
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.ChuaCauHinhKetNoiEcds,
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (XtraMessageBox.Show("Đẩy ca bệnh " + (row.TREATMENT_CODE ?? "") + " lên cổng ECDS?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                RunSyncForRows(new List<V_HIS_TREATMENT> { row.Source }, false);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private List<V_HIS_TREATMENT> GetSelectedSources()
        {
            return gvList.GetSelectedRows().Where(rh => rh >= 0)
                .Select(rh => gvList.GetRow(rh) as EcdsSyncGridRowADO)
                .Where(r => r != null && r.Source != null)
                .Select(r => r.Source).ToList();
        }

        /// <summary>
        /// Chạy nền đẩy danh sách ca bệnh lên cổng (dùng chung đẩy tay + tự động).
        /// silent=true: chế độ tự động — không WaitingManager, không popup, chỉ cập nhật label + log.
        /// Mọi ID đã thử đều ghi vào autoAttemptedIds để Timer không đẩy lại (tránh trùng/spam).
        /// </summary>
        private void RunSyncForRows(List<V_HIS_TREATMENT> rows, bool silent)
        {
            try
            {
                if (rows == null || rows.Count == 0) return;
                if (isSyncing) return;

                string reporter = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                var rowsLocal = rows;

                if (!silent) WaitingManager.Show();
                SetSyncBusy(true);
                if (silent && lblAutoStatus != null)
                    lblAutoStatus.Text = "Tự động: đang đẩy " + rowsLocal.Count + " ca...";

                var worker = new BackgroundWorker();
                worker.DoWork += (s, e) =>
                {
                    var results = new List<EcdsSyncResultADO>();
                    int stt = 0;
                    apiWorker.EnsureLogin();
                    foreach (var v in rowsLocal)
                    {
                        stt++;
                        var r = new EcdsSyncResultADO { Stt = stt, TreatmentId = v.ID, TreatmentCode = v.TREATMENT_CODE, PatientName = v.TDL_PATIENT_NAME, IcdCode = v.ICD_CODE };
                        try
                        {
                            var dto = BuildDtoFromTreatment(v, reporter);
                            var result = apiWorker.DayCaBenh(dto);
                            if (result != null && result.thanhCong && result.duLieu != null)
                            {
                                r.Success = true; r.StatusText = "Đã đẩy"; r.MaCaBenh = result.duLieu.maCaBenh;
                            }
                            else
                            {
                                r.Success = false; r.StatusText = "Lỗi";
                                r.Message = result != null ? result.thongDiep : "Không có phản hồi từ cổng.";
                            }
                        }
                        catch (Exception exRow)
                        {
                            r.Success = false; r.StatusText = "Lỗi"; r.Message = exRow.Message;
                            Inventec.Common.Logging.LogSystem.Error(exRow);
                        }
                        results.Add(r);
                    }
                    // Lưu danh sách kết quả đẩy vào HIS (§21 UpdatePushResultList)
                    PersistPushResults(results);
                    e.Result = results;
                };
                worker.RunWorkerCompleted += (s, e) =>
                {
                    try
                    {
                        if (!silent) WaitingManager.Hide();

                        // Đánh dấu mọi ca đã thử -> Timer không auto-đẩy lại trong phiên.
                        foreach (var v in rowsLocal) autoAttemptedIds.Add(v.ID);

                        if (e.Error != null)
                        {
                            Inventec.Common.Logging.LogSystem.Error(e.Error);
                            if (silent)
                            {
                                if (lblAutoStatus != null) lblAutoStatus.Text = "Tự động: lỗi — " + e.Error.Message;
                            }
                            else
                            {
                                XtraMessageBox.Show("Lỗi khi đồng bộ: " + e.Error.Message, "Đồng bộ thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            return;
                        }

                        var results = e.Result as List<EcdsSyncResultADO> ?? new List<EcdsSyncResultADO>();
                        int ok = results.Count(o => o.Success);
                        int fail = results.Count - ok;
                        Inventec.Common.Logging.LogAction.Info(
                            "InfectiousDiseaseSyncList.Sync" + (silent ? "Auto" : "") + " OK=" + ok + " FAIL=" + fail);

                        if (silent)
                        {
                            if (lblAutoStatus != null)
                                lblAutoStatus.Text = "Tự động " + DateTime.Now.ToString("HH:mm")
                                    + ": đã đẩy " + ok + ", lỗi " + fail;
                        }
                        else
                        {
                            using (var frm = new SyncResult.frmEcdsSyncResult(results)) { frm.ShowDialog(); }
                            SearchList();
                        }
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
                    finally { SetSyncBusy(false); }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                if (!silent) WaitingManager.Hide();
                SetSyncBusy(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private EcdsDiseaseCaseDto BuildDtoFromTreatment(V_HIS_TREATMENT v, string reporter)
        {
            var dto = new EcdsDiseaseCaseDto();
            try
            {
                dto.HoTen = v.TDL_PATIENT_NAME;
                dto.NgaySinh = DiseaseCaseMapper.ToIsoDate(v.TDL_PATIENT_DOB);
                bool isMale = v.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE;
                dto.GioiTinh = isMale ? (int)EcdsGioiTinh.Nam : (int)EcdsGioiTinh.Nu;
                dto.NgayNhapVien = DiseaseCaseMapper.ToIsoDate(v.IN_TIME);
                dto.NgayRaVien = DiseaseCaseMapper.ToIsoDate(v.OUT_TIME);
                dto.ChanDoanRaVien = v.ICD_NAME;
                long? benhId = catalogCache.FindIdByMa(catalogCache.GetStatic(EcdsCatalogCache.DM_BENH), v.ICD_CODE);
                if (benhId.HasValue) dto.BenhChuanDoanId = benhId.Value;
                dto.PhanLoaiChuanDoan = (int)EcdsPhanLoaiChuanDoan.XacDinh;
                dto.TinhTrangHienNay = (int)EcdsTinhTrangHienNay.NoiTru;
                dto.LoaiPhatHien = (int)EcdsLoaiPhatHien.Khac;
                dto.NguoiBaoCao = reporter;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return dto;
        }

        /// <summary>
        /// Cập nhật kết quả đẩy vào HIS_ECDS_DISEASE_CASE (§21 UpdatePushResultList) — khóa theo ID bản ghi
        /// lấy từ đối soát (caseIdByTreatment). Ca CHƯA có bản ghi (chưa tạo qua form chi tiết) sẽ bỏ qua.
        /// Chạy trong worker nền.
        /// </summary>
        private void PersistPushResults(List<EcdsSyncResultADO> results)
        {
            try
            {
                if (results == null || results.Count == 0) return;
                long now = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                var list = new List<MOS.SDO.HisEcdsPushResultSDO>();
                int skipped = 0;
                foreach (var r in results)
                {
                    long caseId;
                    if (!caseIdByTreatment.TryGetValue(r.TreatmentId, out caseId) || caseId <= 0)
                    {
                        skipped++;   // chưa có bản ghi ca bệnh -> không cập nhật được (cần tạo qua form chi tiết)
                        continue;
                    }
                    list.Add(new MOS.SDO.HisEcdsPushResultSDO
                    {
                        ID = caseId,
                        ECDS_CASE_CODE = r.MaCaBenh,
                        PUSH_STATE = (short)(r.Success ? 1 : 2),
                        LAST_PUSH_TIME = now,
                        PUSH_MESSAGE = r.Message
                    });
                }

                if (skipped > 0)
                    Inventec.Common.Logging.LogSystem.Warn(
                        "InfectiousDiseaseSyncList: " + skipped + " ca chưa có bản ghi HIS_ECDS_DISEASE_CASE -> bỏ qua cập nhật kết quả đẩy.");

                if (list.Count == 0) return;

                CommonParam param = new CommonParam();
                new BackendAdapter(param).Post<bool>(
                    HisRequestUriStore.HIS_ECDS_UPDATE_PUSH_RESULT, ApiConsumers.MosConsumer, list, param);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                // Không chặn: đẩy cổng đã xong, chỉ lưu HIS lỗi -> ghi log.
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
