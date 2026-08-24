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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.VlgPortalLookup.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.VlgPortalLookup
{
    /// <summary>
    /// Man "Tra cuu Cong tiep nhan KDLYT Vinh Long" — thay cho viec tra cuu bang Postman:
    /// (1) tra danh sach/chi tiet ho so KSK QD 2062 tren cong (loc theo NGAY KHAM, toi da 3 thang);
    /// (2) go ma dieu tri de xem chi tiet tung lan gui + loi tung truong;
    /// (3) DOI SOAT 2 chieu voi HIS_KSK_SYNC — lo ho so "xanh gia" (HIS bao Da dong bo nhung
    ///     khong co tren cong), ho so cong cham KHONG DAT, ho so lech trang thai;
    /// (4) xuat Excel danh sach de giao cac khoa sua du lieu; (5) kiem tra ket noi/tai khoan.
    /// Chi doc (GET) — khong day du lieu; dung chung khoa cau hinh voi man Dong bo KSK.
    /// </summary>
    public partial class UCVlgPortalLookup : UserControlBase
    {
        #region Declare
        /// <summary>Loai ho so dang tra cuu — index cua cboLoaiHoSo.</summary>
        private const int MODE_KSK = 0;    // Kham suc khoe QD 2062
        private const int MODE_KCB = 1;    // Kham chua benh
        private const int MODE_HSSK = 2;   // Ho so suc khoe QD 831/2017

        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        private VlgPortalClient client;
        private bool busy;

        private int CurrentMode
        {
            get { try { return Math.Max(0, cboLoaiHoSo.SelectedIndex); } catch { return MODE_KSK; } }
        }
        private const int MAX_RANGE_DAYS = 92;   // tai lieu: khoang tra cuu toi da 3 thang
        // Doi soat: so ma toi da duoc xac minh truc tiep qua /trang-thai khi vang mat trong danh sach cong
        // (chan viec ban hang tram GET len cong trong 1 lan doi soat).
        private const int MAX_DIRECT_VERIFY = 30;
        private const string HINT_DETAIL = "Nhấp đúp một dòng để xem chi tiết các lần gửi và lỗi từng trường."
            + "\r\nGõ mã điều trị vào ô lọc rồi bấm Tìm kiếm để tra thẳng một hồ sơ.";
        #endregion

        #region Constructor / Load
        public UCVlgPortalLookup(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
        }

        private void UCVlgPortalLookup_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                InitClient();
                // ApplyMode chay lan dau trong SetDefaultControl khi client CON NULL (nut bi khoa het)
                // -> ap lai sau khi da co client de mo dung nut theo mode + trang thai cau hinh.
                ApplyMode();
                memoDetail.Text = HINT_DETAIL;
                // Tu kiem tra ket noi khi mo man (nen, khong chan UI).
                if (ClientReady) CheckConnectionAsync();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                dtFrom.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtTo.EditValue = DateTime.Now.Date;
                cboError.Properties.Items.Clear();
                cboError.Properties.Items.Add("(Tất cả)");
                cboError.Properties.Items.Add("Chỉ hồ sơ CÓ lỗi");
                cboError.Properties.Items.Add("Chỉ hồ sơ KHÔNG lỗi");
                cboError.SelectedIndex = 0;
                cboLoaiHoSo.Properties.Items.Clear();
                cboLoaiHoSo.Properties.Items.Add("KSK 2062");
                cboLoaiHoSo.Properties.Items.Add("KCB");
                cboLoaiHoSo.Properties.Items.Add("HSSK 831");
                cboLoaiHoSo.SelectedIndex = MODE_KSK;   // fire SelectedIndexChanged -> ApplyMode
                txtMaLk.Text = "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void cboLoaiHoSo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { ApplyMode(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Doi giao dien theo loai ho so: caption cot/o nhap, cot hien/an, nut cho phep.
        /// Doi soat chi co o KSK (so voi HIS_KSK_SYNC); Huy/Khoi phuc chi co o KCB.
        /// </summary>
        private void ApplyMode()
        {
            try
            {
                int mode = CurrentMode;
                bool canUse = !this.busy && ClientReady;
                btnReconcile.Enabled = canUse && mode == MODE_KSK;
                btnHuy.Enabled = canUse && mode == MODE_KCB;
                btnKhoiPhuc.Enabled = canUse && mode == MODE_KCB;

                gridView1.BeginUpdate();
                try
                {
                    colMaBn.Visible = (mode == MODE_KCB);
                    colCccd.Visible = (mode == MODE_KSK);
                    colFormName.Visible = (mode == MODE_KSK);
                    colHisStatus.Visible = (mode == MODE_KSK);
                    colDoiSoat.Visible = (mode == MODE_KSK);
                    colNgayKham.Visible = (mode != MODE_HSSK);
                    colLatestReceived.Visible = (mode != MODE_KCB);
                    colLatestStatus.Visible = (mode != MODE_HSSK);   // HSSK khong co truong nay -> an han
                    switch (mode)
                    {
                        case MODE_KCB:
                            colMaLk.Caption = "Mã liên kết (mã ĐT)";
                            colNgayKham.Caption = "Ngày vào";
                            colValidation.Caption = "Trạng thái hồ sơ";
                            colLatestStatus.Caption = "Đã hủy?";
                            lciMaLk.Text = "Mã:";
                            txtMaLk.Properties.NullValuePrompt = "gõ mã điều trị để tra 1 hồ sơ KCB";
                            lciFrom.Text = "Ngày nhận từ:";
                            break;
                        case MODE_HSSK:
                            colMaLk.Caption = "Mã định danh";
                            colValidation.Caption = "Trạng thái request";
                            colLatestReceived.Caption = "Ngày nhận";
                            lciMaLk.Text = "Mã:";
                            txtMaLk.Properties.NullValuePrompt = "gõ mã điều trị / mã định danh để lọc";
                            lciFrom.Text = "Ngày nhận từ:";
                            break;
                        default:
                            colMaLk.Caption = "Mã điều trị";
                            colNgayKham.Caption = "Ngày khám";
                            colValidation.Caption = "KQ kiểm tra";
                            colLatestStatus.Caption = "Lần gửi cuối";
                            colLatestReceived.Caption = "Gửi lúc";
                            lciMaLk.Text = "Mã:";
                            txtMaLk.Properties.NullValuePrompt = "gõ mã điều trị để tra 1 hồ sơ";
                            lciFrom.Text = "Ngày khám từ:";
                            break;
                    }
                }
                finally { gridView1.EndUpdate(); }

                // Doi loai ho so -> xoa ket qua cu de khoi lan lon giua cac nhom.
                BindGrid(new List<VlgHoSoADO>());
                memoDetail.Text = HINT_DETAIL;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void InitClient()
        {
            try
            {
                string connectionInfo = null;
                var cfg = BackendDataWorker.Get<HIS_CONFIG>()
                    .Where(o => o.KEY == VlgPortalClient.CONFIG_KEY).FirstOrDefault();
                if (cfg != null) connectionInfo = cfg.VALUE;
                this.client = new VlgPortalClient(connectionInfo);
                if (!this.client.IsConfigured)
                {
                    lblConn.Text = "CHƯA CẤU HÌNH khóa " + VlgPortalClient.CONFIG_KEY + " — màn hình chỉ xem được sau khi cấu hình.";
                    lblConn.ForeColor = System.Drawing.Color.FromArgb(210, 40, 40);
                    ApplyNotReadyState();
                }
                else
                {
                    lblConn.Text = "Môi trường: " + this.client.MoiTruong + " — đang kiểm tra kết nối...";
                }
            }
            catch (Exception ex)
            {
                // Cache cau hinh loi luc mo man -> client co the null: khoa nut nhu vien chua cau hinh
                // (khong khoa thi moi click se NRE trong worker).
                Inventec.Common.Logging.LogSystem.Error(ex);
                lblConn.Text = "Không đọc được cấu hình — đóng và mở lại màn hình.";
                lblConn.ForeColor = System.Drawing.Color.FromArgb(210, 40, 40);
                ApplyNotReadyState();
            }
        }

        /// <summary>Vien chua cau hinh / init loi -> khoa cac nut goi cong.</summary>
        private void ApplyNotReadyState()
        {
            btnSearch.Enabled = false;
            btnReconcile.Enabled = false;
            btnCheckConn.Enabled = false;
        }

        private bool ClientReady
        {
            get { return this.client != null && this.client.IsConfigured; }
        }
        #endregion

        #region Busy state
        private void SetBusy(bool value)
        {
            try
            {
                this.busy = value;
                // Khong duoc mo khoa nut o vien chua cau hinh (Enter trong txtMaLk co the toi day).
                bool canUse = !value && ClientReady;
                int mode = CurrentMode;
                btnSearch.Enabled = canUse;
                btnReconcile.Enabled = canUse && mode == MODE_KSK;
                btnHuy.Enabled = canUse && mode == MODE_KCB;
                btnKhoiPhuc.Enabled = canUse && mode == MODE_KCB;
                btnCheckConn.Enabled = canUse;
                btnExcel.Enabled = !value;
                gridControl1.Enabled = !value;
                // Khong cho doi loai ho so giua chung — ket qua tra ve se do vao cot cua mode khac.
                cboLoaiHoSo.Enabled = !value;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion

        #region Kiem tra ket noi
        private void btnCheckConn_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.busy || !ClientReady) return;
                CheckConnectionAsync();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void CheckConnectionAsync()
        {
            try
            {
                var clientLocal = this.client;
                var worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += (s, ev) =>
                {
                    bool ok;
                    string msg = clientLocal.CheckConnection(out ok);
                    ev.Result = new object[] { ok, msg };
                };
                worker.RunWorkerCompleted += (s, ev) =>
                {
                    try
                    {
                        if (this.IsDisposed || this.Disposing) return;
                        if (ev.Error != null)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ev.Error);
                            lblConn.Text = "Lỗi kiểm tra kết nối — xem log.";
                            lblConn.ForeColor = System.Drawing.Color.FromArgb(210, 40, 40);
                            return;
                        }
                        var r = ev.Result as object[];
                        if (r == null) return;
                        bool ok = (bool)r[0];
                        lblConn.Text = (string)r[1];
                        lblConn.ForeColor = ok
                            ? System.Drawing.Color.FromArgb(0, 150, 60)
                            : System.Drawing.Color.FromArgb(210, 40, 40);
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
        #endregion

        #region Tim kiem
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.busy || !ClientReady) return;
                this.client.ResetBatchError();   // thao tac moi = lo moi, khong an loi mang cua lan truoc
                int mode = CurrentMode;
                string maLk = txtMaLk.Text.Trim();
                // KSK/KCB co API trang-thai theo ma -> tra thang 1 ho so; HSSK loc qua API danh sach.
                if (!string.IsNullOrEmpty(maLk) && mode != MODE_HSSK) { SearchSingle(mode, maLk); return; }

                DateTime from, to;
                if (!TryGetRange(out from, out to)) return;
                string errorStatus = GetErrorStatusFilter();

                var clientLocal = this.client;
                WaitingManager.Show();
                SetBusy(true);
                var worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += (s, ev) =>
                {
                    string error;
                    List<VlgHoSoADO> list;
                    switch (mode)
                    {
                        case MODE_KCB:
                            list = clientLocal.GetKcbHoSoList(from, to, errorStatus, true, out error);
                            break;
                        case MODE_HSSK:
                            list = clientLocal.GetHssk831List(from, to, errorStatus, maLk, null, out error);
                            // Nguoi dung nhap ma nhung loc theo ma_lk khong ra -> thu lai theo ma dinh danh.
                            if (error == null && list.Count == 0 && !string.IsNullOrEmpty(maLk))
                                list = clientLocal.GetHssk831List(from, to, errorStatus, null, maLk, out error);
                            break;
                        default:
                            list = clientLocal.GetKskHoSoList(from, to, errorStatus, out error);
                            break;
                    }
                    ev.Result = new object[] { list, error, clientLocal.LastListTruncated };
                };
                worker.RunWorkerCompleted += (s, ev) => FinishLoadList(ev, "Tra cứu");
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                SetBusy(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Tra thang 1 ho so theo ma (GET trang-thai KSK/KCB tuy mode) — do 1 dong + chi tiet vao memo.</summary>
        private void SearchSingle(int mode, string maLk)
        {
            var clientLocal = this.client;
            WaitingManager.Show();
            SetBusy(true);
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.DoWork += (s, ev) =>
            {
                bool found; string error;
                VlgHoSoADO ado = (mode == MODE_KCB)
                    ? clientLocal.GetKcbTrangThai(maLk, out found, out error)
                    : clientLocal.GetKskTrangThai(maLk, out found, out error);
                ev.Result = new object[] { ado, found, error };
            };
            worker.RunWorkerCompleted += (s, ev) =>
            {
                try
                {
                    WaitingManager.Hide();
                    if (this.IsDisposed || this.Disposing) return;
                    // PHAI check ev.Error TRUOC khi cham ev.Result — property Result nem lai exception cua DoWork.
                    if (ev.Error != null)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ev.Error);
                        XtraMessageBox.Show("Lỗi khi gọi cổng." + Environment.NewLine + ev.Error.Message,
                            "Tra cứu thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var r = ev.Result as object[];
                    if (r == null) return;
                    var ado = r[0] as VlgHoSoADO;
                    bool found = (bool)r[1];
                    string error = r[2] as string;
                    if (!string.IsNullOrEmpty(error))
                    {
                        XtraMessageBox.Show(error, "Tra cứu thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!found || ado == null)
                    {
                        BindGrid(new List<VlgHoSoADO>());
                        memoDetail.Text = "KHÔNG có hồ sơ mã " + maLk + " trên cổng (" + this.client.MoiTruong + ")."
                            + "\r\nHồ sơ chưa từng được cổng tiếp nhận thành công — kiểm tra lại việc đẩy từ màn Đồng bộ KSK.";
                        return;
                    }
                    BindGrid(new List<VlgHoSoADO> { ado });
                    memoDetail.Text = ado.DetailText;
                }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
                finally { SetBusy(false); }
            };
            worker.RunWorkerAsync();
        }

        private void FinishLoadList(System.ComponentModel.RunWorkerCompletedEventArgs ev, string actionName)
        {
            try
            {
                WaitingManager.Hide();
                if (this.IsDisposed || this.Disposing) return;
                if (ev.Error != null)
                {
                    Inventec.Common.Logging.LogSystem.Error(ev.Error);
                    XtraMessageBox.Show("Lỗi khi gọi cổng." + Environment.NewLine + ev.Error.Message,
                        actionName + " thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var r = ev.Result as object[];
                if (r == null) return;
                var list = r[0] as List<VlgHoSoADO>;
                string error = r[1] as string;
                bool truncated = r.Length > 2 && r[2] is bool && (bool)r[2];
                if (!string.IsNullOrEmpty(error))
                {
                    XtraMessageBox.Show(error, actionName + " thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                BindGrid(list ?? new List<VlgHoSoADO>());
                memoDetail.Text = actionName + ": " + ((list != null) ? list.Count : 0) + " hồ sơ."
                    + (truncated ? "\r\n⚠ Danh sách BỊ CẮT (quá " + (100 * 50) + " hồ sơ) — thu hẹp khoảng ngày để xem đủ." : "")
                    + "\r\n" + HINT_DETAIL;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
            finally { SetBusy(false); }
        }

        private bool TryGetRange(out DateTime from, out DateTime to)
        {
            from = DateTime.MinValue; to = DateTime.MinValue;
            try
            {
                if (dtFrom.EditValue == null || dtTo.EditValue == null)
                {
                    XtraMessageBox.Show("Chưa chọn khoảng ngày khám.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                from = Convert.ToDateTime(dtFrom.EditValue).Date;
                to = Convert.ToDateTime(dtTo.EditValue).Date;
                if (from > to)
                {
                    XtraMessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if ((to - from).TotalDays > MAX_RANGE_DAYS)
                {
                    XtraMessageBox.Show("Cổng chỉ cho tra cứu tối đa 3 tháng mỗi lần — thu hẹp khoảng ngày lại.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return false; }
        }

        private string GetErrorStatusFilter()
        {
            switch (cboError.SelectedIndex)
            {
                case 1: return "has_error";
                case 2: return "no_error";
                default: return "all";
            }
        }
        #endregion

        #region Doi soat voi HIS
        /// <summary>
        /// Doi soat 2 chieu: danh sach tren CONG (theo ngay kham) vs HIS_KSK_SYNC (theo ngay ket luan).
        /// 2 truc ngay LECH nhau (ket luan thuong sau kham vai ngay) nen ho so HIS-co/cong-vang duoc
        /// XAC MINH truc tiep qua /trang-thai truoc khi ket luan "xanh gia" (toi da MAX_DIRECT_VERIFY ma).
        /// </summary>
        private void btnReconcile_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.busy || !ClientReady) return;
                if (CurrentMode != MODE_KSK)
                {
                    XtraMessageBox.Show("Đối soát với HIS hiện chỉ áp dụng loại hồ sơ KSK 2062 — chọn lại Loại hồ sơ.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                this.client.ResetBatchError();
                DateTime from, to;
                if (!TryGetRange(out from, out to)) return;

                var clientLocal = this.client;
                WaitingManager.Show();
                SetBusy(true);
                var worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += (s, ev) =>
                {
                    string error;
                    List<VlgHoSoADO> portal = clientLocal.GetKskHoSoList(from, to, "all", out error);
                    bool truncated = clientLocal.LastListTruncated;
                    List<V_HIS_KSK_SYNC> his = null;
                    Dictionary<string, VlgHoSoADO> verified = null;
                    if (error == null)
                    {
                        his = FetchHisRows(from, to);   // null = API HIS loi (khac voi 0 dong!)
                        if (his != null)
                            verified = VerifyMissingOnPortal(clientLocal, portal, his);
                    }
                    ev.Result = new object[] { portal, his, error, verified, truncated };
                };
                worker.RunWorkerCompleted += (s, ev) =>
                {
                    try
                    {
                        WaitingManager.Hide();
                        if (this.IsDisposed || this.Disposing) return;
                        if (ev.Error != null)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ev.Error);
                            XtraMessageBox.Show("Lỗi khi đối soát." + Environment.NewLine + ev.Error.Message,
                                "Đối soát thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var r = ev.Result as object[];
                        if (r == null) return;
                        var portal = r[0] as List<VlgHoSoADO>;
                        var his = r[1] as List<V_HIS_KSK_SYNC>;
                        string error = r[2] as string;
                        var verified = r[3] as Dictionary<string, VlgHoSoADO>;
                        bool truncated = r[4] is bool && (bool)r[4];
                        if (!string.IsNullOrEmpty(error))
                        {
                            XtraMessageBox.Show(error, "Đối soát thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (his == null)
                        {
                            // API HIS loi: KHONG duoc doi soat voi danh sach rong — bao cao se sai toan bo.
                            XtraMessageBox.Show("Không lấy được dữ liệu KSK phía HIS (api/HisKskSync/GetView lỗi) — thử lại sau.",
                                "Đối soát thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        ShowReconcileResult(portal ?? new List<VlgHoSoADO>(), his, verified, from, to, truncated);
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
                    finally { SetBusy(false); }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                SetBusy(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ho so KSK phia HIS theo NGAY KET LUAN trong khoang chon.
        /// Tra NULL khi API loi — nguoi goi PHAI phan biet voi danh sach rong (0 dong that).
        /// </summary>
        private static List<V_HIS_KSK_SYNC> FetchHisRows(DateTime from, DateTime to)
        {
            try
            {
                var param = new CommonParam();
                var filter = new HisKskSyncViewFilter();
                filter.CONCLUSION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(from.ToString("yyyyMMdd") + "000000");
                filter.CONCLUSION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(to.ToString("yyyyMMdd") + "235959");
                var rs = new BackendAdapter(param).GetRO<List<V_HIS_KSK_SYNC>>(
                    "api/HisKskSync/GetView", ApiConsumers.MosConsumer, filter, param);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                return (rs != null && rs.Data != null) ? rs.Data : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return null; }
        }

        /// <summary>
        /// Xac minh truc tiep cac ma HIS bao "da day" (type 2/4) nhung VANG trong danh sach cong:
        /// goi /trang-thai tung ma (toi da MAX_DIRECT_VERIFY). Key co trong dict = da kiem tra;
        /// value != null = CO tren cong (chi lech truc ngay kham/ket luan); value == null = cong 404 (xanh gia that).
        /// Dung ngay khi gap loi mang de khong treo doi soat.
        /// </summary>
        private static Dictionary<string, VlgHoSoADO> VerifyMissingOnPortal(VlgPortalClient client,
            List<VlgHoSoADO> portal, List<V_HIS_KSK_SYNC> his)
        {
            var result = new Dictionary<string, VlgHoSoADO>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var onPortal = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (portal != null)
                    foreach (var p in portal)
                        if (!string.IsNullOrEmpty(p.MaLk)) onPortal.Add(p.MaLk);
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                int checkedCount = 0;
                foreach (var h in his)
                {
                    string maLk = SafeString(GetProp(h, "TDL_TREATMENT_CODE"));
                    int t = GetSyncType(h);
                    if (string.IsNullOrEmpty(maLk) || (t != 2 && t != 4)) continue;
                    if (onPortal.Contains(maLk) || !seen.Add(maLk)) continue;
                    if (checkedCount >= MAX_DIRECT_VERIFY) break;
                    checkedCount++;
                    bool found; string err;
                    VlgHoSoADO ado = client.GetKskTrangThai(maLk, out found, out err);
                    if (err != null) break;
                    result[maLk] = found ? ado : null;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return result;
        }

        /// <summary>Bo dem tung loai ket qua doi soat.</summary>
        private class ReconcileCount
        {
            public int Ok, GreenFake, Invalid, Reverse, Edited, Pending;
        }

        private void ShowReconcileResult(List<VlgHoSoADO> portal, List<V_HIS_KSK_SYNC> his,
            Dictionary<string, VlgHoSoADO> verified, DateTime from, DateTime to, bool portalTruncated)
        {
            var byMaLk = new Dictionary<string, VlgHoSoADO>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in portal)
                if (!string.IsNullOrEmpty(p.MaLk) && !byMaLk.ContainsKey(p.MaLk)) byMaLk[p.MaLk] = p;

            // HIS_KSK_SYNC hat theo y lenh — 1 ma dieu tri co the nhieu dong. Lay 1 dong dai dien,
            // uu tien trang thai can xu ly nhat: 4 (co chinh sua) > 2 (da dong bo) > 3 (that bai) > 1.
            var hisByMaLk = new Dictionary<string, V_HIS_KSK_SYNC>(StringComparer.OrdinalIgnoreCase);
            foreach (var h in his)
            {
                string maLk = SafeString(GetProp(h, "TDL_TREATMENT_CODE"));
                if (string.IsNullOrEmpty(maLk)) continue;
                V_HIS_KSK_SYNC cur;
                if (!hisByMaLk.TryGetValue(maLk, out cur)
                    || TypePriority(GetSyncType(h)) > TypePriority(GetSyncType(cur)))
                    hisByMaLk[maLk] = h;
            }

            var rows = new List<VlgHoSoADO>();
            var matchedMaLk = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var count = new ReconcileCount();

            foreach (var kv in hisByMaLk)
            {
                string maLk = kv.Key;
                int hisType = GetSyncType(kv.Value);
                VlgHoSoADO row;
                bool onPortal = byMaLk.TryGetValue(maLk, out row);
                if (hisType == 1 && !onPortal) continue;   // chua tung day + cong khong co -> khong lien quan
                matchedMaLk.Add(maLk);

                if (onPortal)
                {
                    row.HisStatusText = HisTypeText(hisType);
                    if (hisType == 1)
                    {
                        row.DoiSoatText = "⚠ Có trên cổng nhưng HIS ghi Chưa đồng bộ — kiểm tra trạng thái ở màn Đồng bộ KSK";
                        row.IsMismatch = true; count.Reverse++;
                    }
                    else ApplyMatchedVerdict(row, hisType, count);
                    rows.Add(row);
                    continue;
                }

                // HIS bao da day (2/3/4) nhung danh sach cong theo ngay kham KHONG co ma nay.
                VlgHoSoADO direct = null;
                bool checkedDirect = (verified != null) && verified.TryGetValue(maLk, out direct);
                if (checkedDirect && direct != null)
                {
                    // Co tren cong that — chi lech truc ngay (kham ngoai khoang, ket luan trong khoang).
                    direct.HisStatusText = HisTypeText(hisType);
                    ApplyMatchedVerdict(direct, hisType, count);
                    direct.DoiSoatText += " — ngày khám ngoài khoảng chọn";
                    rows.Add(direct);
                    continue;
                }
                row = new VlgHoSoADO
                {
                    MaLk = maLk,
                    HoTen = SafeString(GetProp(kv.Value, "TDL_PATIENT_NAME")),
                    HisStatusText = HisTypeText(hisType)
                };
                if (hisType == 3)
                {
                    row.DoiSoatText = "Khớp (thất bại, chưa lên cổng)"; count.Ok++;
                }
                else if (checkedDirect)   // direct == null: cong xac nhan 404 -> xanh gia that
                {
                    row.DoiSoatText = "⚠ XANH GIẢ: HIS " + HisTypeText(hisType)
                        + " nhưng cổng KHÔNG có hồ sơ — đẩy lại + báo cổng nếu tái diễn";
                    row.IsMismatch = true; count.GreenFake++;
                }
                else                      // qua han muc xac minh / mang loi giua chung -> chua ket luan duoc
                {
                    row.DoiSoatText = "⚠ Không thấy trong danh sách cổng theo ngày khám — nhấp đúp để kiểm tra trực tiếp";
                    row.IsMismatch = true; count.GreenFake++;
                }
                rows.Add(row);
            }

            // Ho so co tren cong nhung khong nam trong danh sach HIS cua khoang ngay (lech ngay kham/ket luan).
            foreach (var p in portal)
                if (!string.IsNullOrEmpty(p.MaLk) && !matchedMaLk.Contains(p.MaLk))
                {
                    p.HisStatusText = "(HIS không có trong khoảng ngày kết luận)";
                    p.DoiSoatText = "Chỉ thấy trên cổng — hồ sơ có ngày kết luận ngoài khoảng chọn";
                    rows.Add(p);
                }

            // Dong lech len dau cho de xu ly.
            rows = rows.OrderByDescending(o => o.IsMismatch).ThenBy(o => o.MaLk).ToList();
            BindGrid(rows);
            memoDetail.Text = string.Format(
                "KẾT QUẢ ĐỐI SOÁT {0:dd/MM/yyyy} - {1:dd/MM/yyyy} (cổng: theo ngày khám / HIS: theo ngày kết luận):"
                + "\r\n  ✓ Khớp: {2}"
                + "\r\n  ⚠ XANH GIẢ / không thấy trên cổng: {3}"
                + "\r\n  ⚠ Cổng chấm KHÔNG ĐẠT (HIS đang xanh): {4}"
                + "\r\n  ⚠ Lệch trạng thái (cổng ĐẠT nhưng HIS Thất bại / HIS Chưa đồng bộ): {5}"
                + "\r\n  ⚠ Có chỉnh sửa sau đồng bộ — cần đẩy lại: {6}"
                + "\r\n  Cổng đang xử lý (chưa có KQ kiểm tra): {7}"
                + (portalTruncated ? "\r\n⚠ Danh sách cổng BỊ CẮT (quá 5000 hồ sơ) — kết quả đối soát KHÔNG đầy đủ, thu hẹp khoảng ngày." : "")
                + "\r\nCác dòng lệch (⚠) đã đưa lên đầu — bấm Xuất Excel để giao xử lý.",
                from, to, count.Ok, count.GreenFake, count.Invalid, count.Reverse, count.Edited, count.Pending);
        }

        /// <summary>Phan loai khi ho so co mat CA hai ben (cong + HIS type 2/3/4).</summary>
        private static void ApplyMatchedVerdict(VlgHoSoADO row, int hisType, ReconcileCount count)
        {
            bool valid = string.Equals(row.ValidationStatus, "VALID", StringComparison.OrdinalIgnoreCase);
            bool invalid = string.Equals(row.ValidationStatus, "INVALID", StringComparison.OrdinalIgnoreCase);
            if (hisType == 4)
            {
                row.DoiSoatText = "⚠ Hồ sơ đã chỉnh sửa sau khi đồng bộ — đẩy lại lên cổng";
                row.IsMismatch = true; count.Edited++;
            }
            else if (!valid && !invalid)
            {
                // Cong tiep nhan bat dong bo — hs moi day chua co ket qua kiem tra, KHONG phai lech.
                row.DoiSoatText = "Cổng đang xử lý — chưa có kết quả kiểm tra, thử lại sau";
                count.Pending++;
            }
            else if (valid && hisType == 2) { row.DoiSoatText = "✓ Khớp (VALID)"; count.Ok++; }
            else if (valid && hisType == 3)
            {
                row.DoiSoatText = "⚠ Cổng ĐẠT nhưng HIS đang Thất bại — bấm 'Cập nhật KQ cổng VLg' ở màn Đồng bộ KSK";
                row.IsMismatch = true; count.Reverse++;
            }
            else if (invalid && hisType == 2)
            {
                row.DoiSoatText = "⚠ Cổng chấm KHÔNG ĐẠT nhưng HIS báo Đã đồng bộ — sửa dữ liệu, đẩy lại";
                row.IsMismatch = true; count.Invalid++;
            }
            else { row.DoiSoatText = "Khớp (cùng chưa đạt)"; count.Ok++; }
        }
        #endregion

        #region Grid + chi tiet + Excel
        private void BindGrid(List<VlgHoSoADO> rows)
        {
            try
            {
                gridView1.BeginUpdate();
                try { gridControl1.DataSource = rows; }
                finally { gridView1.EndUpdate(); }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!(e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)) return;
                if (e.Column.FieldName == "STT") e.Value = e.ListSourceRowIndex + 1;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var data = view.GetRow(e.RowHandle) as VlgHoSoADO;
                if (data == null) return;
                if (e.Column == colValidation)
                {
                    e.Appearance.Options.UseForeColor = true;
                    // FontStyleDelta: khong new Font moi cell repaint (ro GDI handle).
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    // Trang thai tot/xau cua ca 3 nhom: KSK (VALID/INVALID), KCB (PROCESSED/…_FAILED),
                    // HSSK (COMPLETED/FAILED). COMPLETED_WITH_WARNING de mau mac dinh.
                    string v = data.ValidationStatus ?? "";
                    bool good = v.Equals("VALID", StringComparison.OrdinalIgnoreCase)
                        || v.Equals("PROCESSED", StringComparison.OrdinalIgnoreCase)
                        || v.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase);
                    bool bad = v.Equals("INVALID", StringComparison.OrdinalIgnoreCase)
                        || v.IndexOf("FAILED", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (good) e.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 150, 60);
                    else if (bad) e.Appearance.ForeColor = System.Drawing.Color.FromArgb(210, 40, 40);
                }
                else if (e.Column == colDoiSoat && data.IsMismatch)
                {
                    e.Appearance.Options.UseForeColor = true;
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    e.Appearance.ForeColor = System.Drawing.Color.FromArgb(210, 40, 40);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Nhap dup 1 dong -> goi trang-thai lay chi tiet cac lan gui + loi vao memo.
        /// LUON goi lai cong (khong dung cache): quy trinh chuan la sua du lieu -> day lai ->
        /// quay lai day nhap dup kiem tra — cache cu se lam tuong day lai khong an thua.
        /// </summary>
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (this.busy || !ClientReady) return;
                var row = gridView1.GetFocusedRow() as VlgHoSoADO;
                if (row == null) return;
                int mode = CurrentMode;
                // HSSK tra chi tiet theo tracking_id; KSK/KCB theo ma lien ket.
                string key = (mode == MODE_HSSK) ? row.TrackingId : row.MaLk;
                if (string.IsNullOrEmpty(key)) return;

                this.client.ResetBatchError();
                var clientLocal = this.client;
                string maLk = key;
                memoDetail.Text = "Đang tải chi tiết hồ sơ " + maLk + " ...";
                SetBusy(true);
                var worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += (s, ev) =>
                {
                    bool found; string error;
                    VlgHoSoADO detail;
                    switch (mode)
                    {
                        case MODE_KCB: detail = clientLocal.GetKcbTrangThai(maLk, out found, out error); break;
                        case MODE_HSSK: detail = clientLocal.GetHssk831TrangThai(maLk, out found, out error); break;
                        default: detail = clientLocal.GetKskTrangThai(maLk, out found, out error); break;
                    }
                    ev.Result = new object[] { detail, found, error };
                };
                worker.RunWorkerCompleted += (s, ev) =>
                {
                    try
                    {
                        if (this.IsDisposed || this.Disposing) return;
                        if (ev.Error != null)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ev.Error);
                            memoDetail.Text = "Không tải được chi tiết.";
                            return;
                        }
                        var r = ev.Result as object[];
                        if (r == null) { memoDetail.Text = "Không tải được chi tiết."; return; }
                        var detail = r[0] as VlgHoSoADO;
                        bool found = (bool)r[1];
                        string error = r[2] as string;
                        if (!string.IsNullOrEmpty(error)) { memoDetail.Text = "Lỗi tra cứu: " + error; return; }
                        if (!found || detail == null)
                        {
                            memoDetail.Text = "KHÔNG có hồ sơ mã " + maLk + " trên cổng — chưa từng được tiếp nhận thành công.";
                            return;
                        }
                        row.DetailText = detail.DetailText;
                        memoDetail.Text = detail.DetailText;
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
                    finally { SetBusy(false); }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                SetBusy(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Huy / Khoi phuc ho so KCB
        private void btnHuy_Click(object sender, EventArgs e)
        {
            DoKcbAction(false);
        }

        private void btnKhoiPhuc_Click(object sender, EventArgs e)
        {
            DoKcbAction(true);
        }

        /// <summary>
        /// Huy (restore=false) / Khoi phuc (restore=true) 1 ho so KCB tren cong — chi mode KCB.
        /// Bat nhap ly do; xac nhan truoc khi goi; cong xu ly NGAY (PROCESSED).
        /// </summary>
        private void DoKcbAction(bool restore)
        {
            try
            {
                if (this.busy || !ClientReady || CurrentMode != MODE_KCB) return;
                var row = gridView1.GetFocusedRow() as VlgHoSoADO;
                if (row == null || string.IsNullOrEmpty(row.MaLk))
                {
                    XtraMessageBox.Show("Chọn một dòng hồ sơ KCB trước.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string actionName = restore ? "KHÔI PHỤC" : "HỦY";
                string lyDo = PromptLyDo(actionName + " hồ sơ KCB",
                    "Nhập lý do " + actionName.ToLower() + " hồ sơ " + row.MaLk + " trên cổng:");
                if (lyDo == null) return;   // bam Cancel
                lyDo = lyDo.Trim();
                if (lyDo.Length == 0)
                {
                    XtraMessageBox.Show("Lý do là bắt buộc theo yêu cầu của cổng.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (XtraMessageBox.Show(
                    "Xác nhận " + actionName + " hồ sơ " + row.MaLk + " (" + (row.HoTen ?? "") + ") trên cổng tiếp nhận?"
                    + Environment.NewLine + "Cổng sẽ xử lý NGAY, không qua hàng đợi.",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                this.client.ResetBatchError();
                var clientLocal = this.client;
                string maLk = row.MaLk;
                WaitingManager.Show();
                SetBusy(true);
                var worker = new System.ComponentModel.BackgroundWorker();
                worker.DoWork += (s, ev) =>
                {
                    string error;
                    string msg = clientLocal.PostKcbAction(restore, maLk, lyDo, out error);
                    ev.Result = new object[] { msg, error };
                };
                worker.RunWorkerCompleted += (s, ev) =>
                {
                    try
                    {
                        WaitingManager.Hide();
                        if (this.IsDisposed || this.Disposing) return;
                        if (ev.Error != null)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ev.Error);
                            XtraMessageBox.Show("Lỗi khi gọi cổng." + Environment.NewLine + ev.Error.Message,
                                actionName + " thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var r = ev.Result as object[];
                        if (r == null) return;
                        string msg = r[0] as string;
                        string error = r[1] as string;
                        if (!string.IsNullOrEmpty(error))
                        {
                            XtraMessageBox.Show(error, actionName + " thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        // Cap nhat ngay tren luoi de nguoi dung khoi tim lai.
                        row.LatestStatus = restore ? "" : "ĐÃ HỦY";
                        row.DetailText = null;
                        gridView1.RefreshData();
                        memoDetail.Text = msg + Environment.NewLine
                            + "Nhấp đúp dòng này để xem trạng thái mới nhất trên cổng.";
                        XtraMessageBox.Show(msg, actionName + " thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
                    finally { SetBusy(false); }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                SetBusy(false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>Hop thoai nhap ly do (DevExpress 15.2 chua co XtraInputBox). Tra null khi Cancel.</summary>
        private static string PromptLyDo(string title, string prompt)
        {
            try
            {
                using (var frm = new DevExpress.XtraEditors.XtraForm())
                {
                    frm.Text = title;
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    frm.MinimizeBox = false; frm.MaximizeBox = false; frm.ShowInTaskbar = false;
                    frm.ClientSize = new System.Drawing.Size(460, 110);

                    var lbl = new DevExpress.XtraEditors.LabelControl();
                    lbl.Text = prompt;
                    lbl.Location = new System.Drawing.Point(12, 12);
                    lbl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
                    lbl.Size = new System.Drawing.Size(436, 16);

                    var txt = new DevExpress.XtraEditors.TextEdit();
                    txt.Location = new System.Drawing.Point(12, 36);
                    txt.Size = new System.Drawing.Size(436, 20);

                    var btnOk = new DevExpress.XtraEditors.SimpleButton();
                    btnOk.Text = "Đồng ý";
                    btnOk.DialogResult = DialogResult.OK;
                    btnOk.Location = new System.Drawing.Point(272, 70);
                    btnOk.Size = new System.Drawing.Size(85, 26);

                    var btnCancel = new DevExpress.XtraEditors.SimpleButton();
                    btnCancel.Text = "Bỏ qua";
                    btnCancel.DialogResult = DialogResult.Cancel;
                    btnCancel.Location = new System.Drawing.Point(363, 70);
                    btnCancel.Size = new System.Drawing.Size(85, 26);

                    frm.Controls.Add(lbl);
                    frm.Controls.Add(txt);
                    frm.Controls.Add(btnOk);
                    frm.Controls.Add(btnCancel);
                    frm.AcceptButton = btnOk;
                    frm.CancelButton = btnCancel;

                    return (frm.ShowDialog() == DialogResult.OK) ? (txt.Text ?? "") : null;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return null; }
        }
        #endregion

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Excel xuat theo dong DANG HIEN THI (grid co the dang bi filter cot) — dem theo RowCount.
                int visibleRows = gridView1.RowCount;
                if (visibleRows == 0)
                {
                    XtraMessageBox.Show("Chưa có dữ liệu để xuất — bấm Tìm kiếm hoặc Đối soát trước.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Excel (*.xlsx)|*.xlsx";
                    dlg.FileName = "TraCuu_CongVLG_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
                    if (dlg.ShowDialog() != DialogResult.OK) return;
                    gridControl1.ExportToXlsx(dlg.FileName);
                    XtraMessageBox.Show("Đã xuất " + visibleRows + " dòng ra:" + Environment.NewLine + dlg.FileName,
                        "Xuất Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Xuất Excel thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtMaLk_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // Ton trong trang thai khoa nut (vien chua cau hinh / dang ban).
                if (e.KeyCode == Keys.Enter && btnSearch.Enabled) btnSearch_Click(null, null);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion

        #region helper
        private static int GetSyncType(V_HIS_KSK_SYNC row)
        {
            int t = 0;
            try { t = Convert.ToInt32(GetProp(row, "SYNC_RESULT_TYPE")); }
            catch { t = 0; }
            if (t == 0) t = 1;
            return t;
        }

        /// <summary>Do uu tien chon dong dai dien khi 1 ma dieu tri co nhieu dong HIS_KSK_SYNC.</summary>
        private static int TypePriority(int syncType)
        {
            switch (syncType)
            {
                case 4: return 3;   // co chinh sua — can xu ly nhat
                case 2: return 2;   // da dong bo
                case 3: return 1;   // that bai
                default: return 0;  // chua dong bo
            }
        }

        private static string HisTypeText(int syncType)
        {
            switch (syncType)
            {
                case 2: return "Đã đồng bộ";
                case 3: return "Thất bại";
                case 4: return "Có chỉnh sửa";
                default: return "Chưa đồng bộ";
            }
        }

        private static object GetProp(object obj, string name)
        {
            try
            {
                if (obj == null) return null;
                var p = obj.GetType().GetProperty(name);
                return p != null ? p.GetValue(obj, null) : null;
            }
            catch { return null; }
        }

        private static string SafeString(object o) { return o == null ? "" : o.ToString(); }
        #endregion
    }
}
