/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Dựng UI + tìm kiếm/phân trang (V_HIS_TREATMENT) + đồng bộ hàng loạt + mở plugin chi tiết.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
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
    public partial class frmInfectiousDiseaseSyncList
    {
        #region Build UI
        private void BuildUi()
        {
            this.SuspendLayout();
            try
            {
                // --- Tìm kiếm ---
                pnlSearch = new PanelControl() { Dock = DockStyle.Top, Height = 76 };
                pnlSearch.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                AddLabel("Mã điều trị:", 6, 10);
                txtSearchTreatmentCode = new TextEdit() { Location = new Point(84, 8), Size = new Size(200, 22) };
                AddLabel("Tên bệnh nhân:", 300, 10);
                txtSearchPatientName = new TextEdit() { Location = new Point(392, 8), Size = new Size(200, 22) };
                AddLabel("Từ ngày:", 6, 40);
                dteSearchFrom = NewDate(); dteSearchFrom.Location = new Point(84, 38); dteSearchFrom.Size = new Size(110, 22); dteSearchFrom.DateTime = DateTime.Now;
                AddLabel("Đến ngày:", 200, 40);
                dteSearchTo = NewDate(); dteSearchTo.Location = new Point(268, 38); dteSearchTo.Size = new Size(110, 22); dteSearchTo.DateTime = DateTime.Now;
                btnSearch = new SimpleButton() { Text = "Tìm kiếm (Ctrl+F)", Location = new Point(392, 38), Size = new Size(130, 26) };
                btnSearch.Click += (s, e) => { try { SearchList(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };
                pnlSearch.Controls.AddRange(new Control[] { txtSearchTreatmentCode, txtSearchPatientName, dteSearchFrom, dteSearchTo, btnSearch });

                // --- Grid ---
                grdList = new GridControl() { Dock = DockStyle.Fill };
                gvList = new GridView(grdList);
                grdList.MainView = gvList;
                gvList.OptionsBehavior.Editable = false;
                gvList.OptionsView.ShowGroupPanel = false;
                gvList.OptionsView.ColumnAutoWidth = true;
                gvList.OptionsSelection.MultiSelect = true;
                gvList.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                gvList.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
                gvList.Columns.AddVisible("TREATMENT_CODE", "Mã điều trị");
                gvList.Columns.AddVisible("TDL_PATIENT_CODE", "Mã BN");
                gvList.Columns.AddVisible("TDL_PATIENT_NAME", "Bệnh nhân");
                gvList.Columns.AddVisible("ICD_CODE", "ICD");
                gvList.SelectionChanged += (s, e) => UpdateSyncBadge();
                gvList.DoubleClick += (s, e) => { try { OpenDetailForFocusedRow(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };

                // --- Đồng bộ + phân trang ---
                pnlSyncBar = new PanelControl() { Dock = DockStyle.Bottom, Height = 34 };
                pnlSyncBar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                btnSyncList = new SimpleButton() { Text = "Đồng bộ lên cổng (0)", Location = new Point(6, 4), Size = new Size(180, 26), Enabled = false };
                btnSyncList.Click += (s, e) => { try { SyncSelected(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); } };

                // --- Tự động đẩy (Timer) ---
                chkAutoPush = new CheckEdit() { Text = "Tự động đẩy mỗi", Location = new Point(196, 6), Size = new Size(112, 22) };
                spnAutoInterval = new SpinEdit() { Location = new Point(312, 4), Size = new Size(56, 22) };
                spnAutoInterval.Properties.IsFloatValue = false;
                spnAutoInterval.Properties.MinValue = 1;
                spnAutoInterval.Properties.MaxValue = 1440;
                spnAutoInterval.EditValue = 5;
                var lblPhut = new LabelControl() { Text = "phút", Location = new Point(372, 8), AutoSizeMode = LabelAutoSizeMode.None, Size = new Size(28, 16) };
                lblAutoStatus = new LabelControl() { Text = "", Location = new Point(408, 8), AutoSizeMode = LabelAutoSizeMode.None, Size = new Size(320, 16) };
                chkAutoPush.CheckedChanged += chkAutoPush_CheckedChanged;
                spnAutoInterval.EditValueChanged += spnAutoInterval_EditValueChanged;

                pnlSyncBar.Controls.AddRange(new Control[] { btnSyncList, chkAutoPush, spnAutoInterval, lblPhut, lblAutoStatus });

                ucPaging = new Inventec.UC.Paging.UcPaging() { Dock = DockStyle.Bottom };

                // --- Footer ---
                pnlFooter = new PanelControl() { Dock = DockStyle.Bottom, Height = 40 };
                pnlFooter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                btnEdit = new SimpleButton() { Text = "Xem/Sửa chi tiết", Location = new Point(6, 7), Size = new Size(140, 26) };
                btnEdit.Click += btnEdit_Click;
                btnReconcile = new SimpleButton() { Text = "Đối soát với cổng", Location = new Point(152, 7), Size = new Size(140, 26) };
                btnReconcile.Click += (s, e) => XtraMessageBox.Show("Chức năng đối soát đang phát triển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnClose = new SimpleButton() { Text = "Đóng", Location = new Point(300, 7), Size = new Size(90, 26) };
                btnClose.Click += btnClose_Click;
                pnlFooter.Controls.AddRange(new Control[] { btnEdit, btnReconcile, btnClose });

                this.Controls.Add(grdList);
                this.Controls.Add(pnlFooter);
                this.Controls.Add(ucPaging);
                this.Controls.Add(pnlSyncBar);
                this.Controls.Add(pnlSearch);
            }
            finally { this.ResumeLayout(false); }
        }

        private void AddLabel(string text, int x, int y)
        {
            pnlSearch.Controls.Add(new LabelControl() { Text = text, Location = new Point(x, y), AutoSizeMode = LabelAutoSizeMode.None, Size = new Size(84, 16) });
        }

        private DateEdit NewDate()
        {
            var d = new DateEdit();
            d.Properties.Mask.EditMask = "dd/MM/yyyy";
            d.Properties.Mask.UseMaskAsDisplayFormat = true;
            return d;
        }
        #endregion

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
                listData = new List<V_HIS_TREATMENT>();
                listStartPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(listStartPage, limit);

                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                SetListFilter(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "IN_TIME";

                grdList.BeginUpdate();
                grdList.DataSource = null;
                var apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null && apiResult.Data != null)
                {
                    listData = (List<V_HIS_TREATMENT>)apiResult.Data;
                    listRowCount = listData.Count;
                    listDataTotal = apiResult.Param != null ? (apiResult.Param.Count ?? 0) : 0;
                    grdList.DataSource = listData;
                }
                grdList.EndUpdate();
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
                if (!string.IsNullOrEmpty(txtSearchTreatmentCode.Text.Trim()))
                {
                    filter.TREATMENT_CODE__EXACT = txtSearchTreatmentCode.Text.Trim();
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
        #endregion

        #region Mở plugin chi tiết (inter-plugin)
        private void OpenDetailForFocusedRow()
        {
            try
            {
                var v = gvList.GetFocusedRow() as V_HIS_TREATMENT;
                if (v == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn 1 ca bệnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
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

        #region Đồng bộ hàng loạt
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
                var rows = gvList.GetSelectedRows().Where(rh => rh >= 0)
                    .Select(rh => gvList.GetRow(rh) as V_HIS_TREATMENT).Where(r => r != null).ToList();
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

        /// <summary>
        /// Chạy nền đẩy danh sách ca bệnh lên cổng (dùng chung đẩy tay + tự động).
        /// silent=true: chế độ tự động — không WaitingManager, không popup xác nhận/kết quả, chỉ cập nhật label + log.
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

        /// <summary>Lưu danh sách kết quả đẩy vào HIS_ECDS_DISEASE_CASE (§21). Chạy trong worker nền.</summary>
        private void PersistPushResults(List<EcdsSyncResultADO> results)
        {
            try
            {
                if (results == null || results.Count == 0) return;
                long now = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var list = results.Select(r => new HisEcdsPushResultADO
                {
                    TREATMENT_ID = r.TreatmentId,
                    ECDS_CASE_CODE = r.MaCaBenh,
                    PUSH_STATE = r.Success ? 1 : 2,
                    LAST_PUSH_TIME = now,
                    PUSH_MESSAGE = r.Message
                }).ToList();

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
