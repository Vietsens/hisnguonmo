/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.HisPatientPackage.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    public partial class UcHisPatientPackage
    {
        #region Load grid + paging

        /// <summary>Phím tắt Ctrl+F.</summary>
        public void Search()
        {
            try { FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Phím tắt Ctrl+R.</summary>
        public void Refesh()
        {
            try { SetDefaultControl(); FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    try { pageSize = (int)ConfigApplications.NumPageSize; }
                    catch (Exception exCfg) { Inventec.Common.Logging.LogSystem.Warn(exCfg); pageSize = 50; }
                }
                if (pageSize <= 0) pageSize = 50;

                LoadGridData(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                HisPatientPackageViewFilter filter = new HisPatientPackageViewFilter();
                SetFilter(ref filter);

                gridView.BeginUpdate();
                ApiResultObject<List<V_HIS_PATIENT_PACKAGE>> apiResult =
                    new BackendAdapter(paramCommon).GetRO<List<V_HIS_PATIENT_PACKAGE>>(
                        HisRequestUriStore.MOSHIS_HIS_PATIENT_PACKAGE_GET_VIEW,
                        ApiConsumers.MosConsumer, filter, paramCommon);

                if (apiResult != null && apiResult.Data != null)
                {
                    List<PatientPackageADO> adoList = BuildAdoList(apiResult.Data);
                    gridControl.DataSource = adoList;
                    rowCount = adoList.Count;
                    dataTotal = apiResult.Param == null ? 0 : (apiResult.Param.Count ?? 0);
                }
                else
                {
                    gridControl.DataSource = null;
                    rowCount = 0;
                    dataTotal = 0;
                }
                gridView.EndUpdate();

                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dựng filter từ panel lọc trái. HisPatientPackageViewFilter CHỈ có KEY_WORD cho text
        /// (KHÔNG có field PATIENT_ID/PATIENT_CODE), nên text search đi qua KEY_WORD:
        /// - Mã bệnh nhân -> field riêng filter.PATIENT_CODE (backend lọc đúng cột mã BN — KHÔNG dùng KEY_WORD).
        ///   Nhập chữ/tên không khớp mã -> grid rỗng tự nhiên. KHÔNG áp filter thời gian (tìm xuyên ngày).
        /// - Từ khóa     -> KEY_WORD (khớp tên BN/mã BN/tên gói) + VẪN áp filter thời gian (lọc trong khoảng đang chọn).
        /// - Nhập cả hai -> ưu tiên Mã BN, bỏ qua Từ khóa.
        /// - KHÔNG có search nào -> chỉ filter theo thời gian tạo.
        /// </summary>
        private void SetFilter(ref HisPatientPackageViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                string code = (txtPatientCode.Text ?? "").Trim();
                string kw = (txtKeyword.Text ?? "").Trim();

                if (!string.IsNullOrEmpty(code))
                {
                    // Chèn 0 cho đủ 10 số nếu nhập toàn số và ngắn hơn 10 — chuẩn hóa mã BN giống PatientRaw.
                    if (code.Length < 10 && code.All(char.IsDigit))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    // Mã BN -> field riêng PATIENT_CODE (backend lọc đúng cột mã BN), KHÔNG dùng KEY_WORD
                    // (tránh khớp nhầm theo tên BN/tên gói). Nhập chữ/tên không khớp mã -> grid rỗng.
                    // KHÔNG áp filter thời gian -> tìm xuyên ngày.
                    filter.PATIENT_CODE = code;
                    return;
                }

                if (!string.IsNullOrEmpty(kw))
                {
                    // Từ khóa -> KEY_WORD (backend khớp tên BN/mã BN/tên gói); KHÔNG return -> VẪN áp filter
                    // thời gian phía dưới để tìm kiếm theo từ khóa trong khoảng thời gian đang chọn.
                    filter.KEY_WORD = kw;
                }

                // Áp filter thời gian khi: KHÔNG search, hoặc tìm theo Từ khóa. (Mã BN đã return ở trên.)
                long from, to;
                if (GetDateRange(out from, out to))
                {
                    filter.CREATE_TIME_FROM = from;
                    filter.CREATE_TIME_TO = to;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tính khoảng thời gian tạo theo cboTimeType (giống pattern Hồ sơ điều trị):
        ///   0 Trong ngày  : range = 00:00 -> 23:59 ngày dteDate.
        ///   1 Trong tháng : range = ngày đầu 00:00 -> ngày cuối 23:59 tháng dteDate.
        ///   2 Trong năm   : range = 01/01 00:00 -> 31/12 23:59 năm dteDate.
        ///   3 Khoảng ngày : range = dteDate -> dteToDate (GIỮ NGUYÊN giờ phút user nhập),
        ///                   auto swap nếu ngược.
        /// </summary>
        private bool GetDateRange(out long from, out long to)
        {
            from = 0; to = 0;
            try
            {
                if (dteDate.EditValue == null) return false;
                DateTime fromDt, toDt;
                switch (cboTimeType.SelectedIndex)
                {
                    case 0: // Trong ngày
                    {
                        DateTime d = Convert.ToDateTime(dteDate.EditValue).Date;
                        fromDt = d;
                        toDt = d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        break;
                    }
                    case 1: // Trong tháng
                    {
                        DateTime d = Convert.ToDateTime(dteDate.EditValue).Date;
                        fromDt = new DateTime(d.Year, d.Month, 1);
                        toDt = fromDt.AddMonths(1).AddSeconds(-1);
                        break;
                    }
                    case 2: // Trong năm
                    {
                        DateTime d = Convert.ToDateTime(dteDate.EditValue).Date;
                        fromDt = new DateTime(d.Year, 1, 1);
                        toDt = new DateTime(d.Year, 12, 31, 23, 59, 59);
                        break;
                    }
                    case 3: // Khoảng ngày -> giữ giờ phút user nhập (HH:mm), auto swap nếu ngược.
                    {
                        if (dteToDate == null || dteToDate.EditValue == null) return false;
                        fromDt = Convert.ToDateTime(dteDate.EditValue);
                        toDt = Convert.ToDateTime(dteToDate.EditValue);
                        if (toDt < fromDt) { var tmp = fromDt; fromDt = toDt; toDt = tmp; }
                        break;
                    }
                    default:
                        return false;
                }
                from = Inventec.Common.TypeConvert.Parse.ToInt64(fromDt.ToString("yyyyMMddHHmmss"));
                to = Inventec.Common.TypeConvert.Parse.ToInt64(toDt.ToString("yyyyMMddHHmmss"));
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Tính sẵn (pre-compute) các cột hiển thị vào ADO — KHÔNG tính trong CustomUnboundColumnData
        /// để grid render nhanh (xem performance.md).
        /// Bộ lọc keyword đã chuyển hết về backend KEY_WORD, KHÔNG còn client-side filter.
        /// </summary>
        private List<PatientPackageADO> BuildAdoList(List<V_HIS_PATIENT_PACKAGE> data)
        {
            var result = new List<PatientPackageADO>();
            try
            {
                EnsureGenderDict();
                IEnumerable<V_HIS_PATIENT_PACKAGE> source = data;

                int idx = 0;
                foreach (var o in source)
                {
                    if (o == null) continue;
                    var ado = new PatientPackageADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<PatientPackageADO>(ado, o);
                    ado.STT = ++idx + startPage;
                    ado.DobDisplay = FormatDob(o.PATIENT_DOB);
                    ado.GenderName = GetGenderName(o.PATIENT_GENDER_ID);
                    ado.STATUS_CODE = MapDisplayStatusCode(ado.STATUS_CODE);
                    ado.StatusName = GetStatusName(ado.STATUS_CODE);
                    ado.CreateTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(o.CREATE_TIME ?? 0);
                    ado.ModifyTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(o.MODIFY_TIME ?? 0);
                    result.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void EnsureGenderDict()
        {
            try
            {
                if (genderDict != null) return;
                genderDict = BackendDataWorker.Get<HIS_GENDER>()
                    .GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First().GENDER_NAME);
            }
            catch (Exception ex)
            {
                genderDict = new Dictionary<long, string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetGenderName(long genderId)
        {
            string name;
            return (genderDict != null && genderDict.TryGetValue(genderId, out name)) ? name : "";
        }

        private string FormatDob(long dob)
        {
            try
            {
                if (dob <= 0) return "";
                string s = dob.ToString();
                if (s.Length >= 8) return s.Substring(6, 2) + "/" + s.Substring(4, 2) + "/" + s.Substring(0, 4);
                if (s.Length >= 4) return s.Substring(0, 4);
                return s;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>
        /// Map mã trạng thái CSDL gốc (theo §3.1 - bảng HIS_PATIENT_PACKAGE): REGISTERED / IN_USE / LOCKED
        /// sang mã hiển thị của màn 6.2 (theo bảng spec §5.2 - 4 nhãn). Áp dụng cho dữ liệu thật từ view.
        /// TODO: khi backend chốt cách phát hiện "Đã hoàn tiền" (vd: TOTAL_REFUNDED >= TOTAL_PAID),
        /// bổ sung nhánh trả REFUNDED tại đây.
        /// </summary>
        private string MapDisplayStatusCode(string dbCode)
        {
            if (string.IsNullOrEmpty(dbCode)) return dbCode;
            switch (dbCode)
            {
                case "REGISTERED": return PatientPackageStatusCode.WAITING_PAYMENT;
                case "IN_USE": return PatientPackageStatusCode.PAID;
                case "LOCKED": return PatientPackageStatusCode.CANCELED;
                default: return dbCode;
            }
        }

        private string GetStatusName(string statusCode)
        {
            switch (statusCode)
            {
                case PatientPackageStatusCode.WAITING_PAYMENT: return Lang("UcHisPatientPackage.Status.WaitingPayment");
                case PatientPackageStatusCode.PAID: return Lang("UcHisPatientPackage.Status.Paid");
                case PatientPackageStatusCode.REFUNDED: return Lang("UcHisPatientPackage.Status.Refunded");
                case PatientPackageStatusCode.CANCELED: return Lang("UcHisPatientPackage.Status.Canceled");
                default: return statusCode ?? "";
            }
        }

        #endregion

        #region Status -> button visibility

        /// <summary>
        /// Cache cờ "phòng đang vào là phòng thu ngân?" — quyết định enable Thanh toán/Hoàn tiền.
        /// null = chưa tính, true/false = đã tính. Tính 1 lần qua BackendDataWorker.
        /// </summary>
        private bool? isInCashierRoomCache;

        /// <summary>
        /// Kiểm tra phòng hiện tại (currentModule.RoomId) có nằm trong V_HIS_CASHIER_ROOM không.
        /// Thanh toán/Hoàn tiền CHỈ enable khi user vào màn này từ phòng thu ngân.
        /// </summary>
        private bool IsInCashierRoom()
        {
            try
            {
                if (isInCashierRoomCache.HasValue) return isInCashierRoomCache.Value;
                long roomId = currentModule != null ? currentModule.RoomId : 0;
                if (roomId <= 0) { isInCashierRoomCache = false; return false; }
                var cashier = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                    .Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>()
                    .FirstOrDefault(o => o.ROOM_ID == roomId);
                isInCashierRoomCache = cashier != null;
                return isInCashierRoomCache.Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                isInCashierRoomCache = false;
                return false;
            }
        }

        /// <summary>
        /// Ma trận ẩn/hiện nút theo 4 trạng thái (bảng màn 6.2):
        ///   Chờ thanh toán : Sửa, Xóa, In, Thanh toán.
        ///   Đã thanh toán  : In, Hoàn tiền (+ Khóa).
        ///   Đã hoàn tiền   : In.
        ///   Đã hủy         : Xóa (+ Mở khóa).
        /// Bổ sung: Thanh toán + Hoàn tiền CHỈ enable khi đang ở phòng thu ngân.
        /// </summary>
        private bool IsActionAllowed(string status, PatientPackageRowAction action)
        {
            switch (action)
            {
                case PatientPackageRowAction.Edit:
                    return status == PatientPackageStatusCode.WAITING_PAYMENT;
                case PatientPackageRowAction.Delete:
                    return status == PatientPackageStatusCode.WAITING_PAYMENT
                        || status == PatientPackageStatusCode.CANCELED;
                case PatientPackageRowAction.Print:
                    return status == PatientPackageStatusCode.WAITING_PAYMENT
                        || status == PatientPackageStatusCode.PAID
                        || status == PatientPackageStatusCode.REFUNDED;
                case PatientPackageRowAction.Pay:
                    return status == PatientPackageStatusCode.WAITING_PAYMENT && IsInCashierRoom();
                case PatientPackageRowAction.Refund:
                    return status == PatientPackageStatusCode.PAID && IsInCashierRoom();
                case PatientPackageRowAction.Lock:
                    // Khóa/Mở khóa KHÔNG phụ thuộc trạng thái (spec §5.2 không có cột "Khóa").
                    // Việc routing icon "khóa mở" vs "khóa đóng" theo CANCELED đảm bảo click luôn hợp lệ.
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Thêm cột "mũi tên thể hiện trạng thái" ngay sau STT (vẽ tam giác đổi màu theo STATUS_CODE).
        /// Tạo bằng code để KHÔNG phải dịch lại VisibleIndex của toàn bộ cột trong Designer.
        /// </summary>
        private void InitStatusArrowColumn()
        {
            try
            {
                if (colStatusArrow != null) return;
                colStatusArrow = new DevExpress.XtraGrid.Columns.GridColumn();
                colStatusArrow.Name = "colStatusArrow";
                colStatusArrow.FieldName = "STATUS_ARROW";
                colStatusArrow.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                colStatusArrow.OptionsColumn.ShowCaption = false;
                colStatusArrow.OptionsColumn.AllowEdit = false;
                colStatusArrow.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                colStatusArrow.OptionsColumn.FixedWidth = true;
                colStatusArrow.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                colStatusArrow.Width = 24;
                gridView.Columns.Add(colStatusArrow);
                colStatusArrow.Visible = true;
                colStatusArrow.VisibleIndex = 1; // ngay sau STT

                // Hover tooltip — hiện tên trạng thái khi di chuột qua mũi tên.
                gridView.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
                if (gridControl.ToolTipController == null)
                    gridControl.ToolTipController = new DevExpress.Utils.ToolTipController();
                gridControl.ToolTipController.GetActiveObjectInfo -= StatusArrowToolTip_GetActiveObjectInfo;
                gridControl.ToolTipController.GetActiveObjectInfo += StatusArrowToolTip_GetActiveObjectInfo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Tooltip hover trên cột mũi tên: hiện tên trạng thái (Chờ thanh toán/Đã thanh toán/Đã hoàn tiền/Đã hủy).</summary>
        private void StatusArrowToolTip_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl != gridControl) return;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = gridView.CalcHitInfo(e.ControlMousePosition);
                if (!info.InRowCell || info.Column != colStatusArrow || info.RowHandle < 0) return;

                PatientPackageADO row = gridView.GetRow(info.RowHandle) as PatientPackageADO;
                if (row == null) return;

                string tip = GetStatusName(row.STATUS_CODE);
                if (string.IsNullOrEmpty(tip)) return;

                // Key duy nhất theo (row, column) để DevExpress không show lặp.
                object key = "PkgStatus_" + info.RowHandle + "_" + info.Column.AbsoluteIndex;
                e.Info = new DevExpress.Utils.ToolTipControlInfo(key, tip);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Vẽ mũi tên (tam giác) màu theo trạng thái cho cột colStatusArrow.</summary>
        private void gridView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (colStatusArrow == null || e.Column != colStatusArrow || e.RowHandle < 0) return;
                PatientPackageADO row = gridView.GetRow(e.RowHandle) as PatientPackageADO;
                if (row == null) return;

                // Nền theo màu dòng (giữ màu trắng/vàng/đỏ)
                e.Appearance.FillRectangle(e.Cache, e.Bounds);

                Color c;
                switch (row.STATUS_CODE)
                {
                    case PatientPackageStatusCode.WAITING_PAYMENT: c = Color.FromArgb(240, 163, 10); break; // cam (chờ TT)
                    case PatientPackageStatusCode.PAID: c = Color.FromArgb(46, 158, 79); break;             // xanh lá (đã TT)
                    case PatientPackageStatusCode.REFUNDED: c = Color.FromArgb(0, 120, 215); break;         // xanh dương (đã hoàn)
                    case PatientPackageStatusCode.CANCELED: c = Color.FromArgb(128, 128, 128); break;        // xám (đã hủy)
                    default: c = Color.Gray; break;
                }

                System.Drawing.Rectangle b = e.Bounds;
                int cx = b.X + b.Width / 2;
                int cy = b.Y + b.Height / 2;
                int s = 5;
                System.Drawing.Point[] tri = new System.Drawing.Point[]
                {
                    new System.Drawing.Point(cx - s, cy - s - 1),
                    new System.Drawing.Point(cx - s, cy + s + 1),
                    new System.Drawing.Point(cx + s + 1, cy)
                };

                System.Drawing.Drawing2D.SmoothingMode old = e.Cache.Graphics.SmoothingMode;
                e.Cache.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (System.Drawing.SolidBrush br = new System.Drawing.SolidBrush(c))
                {
                    e.Cache.Graphics.FillPolygon(br, tri);
                }
                e.Cache.Graphics.SmoothingMode = old;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                PatientPackageADO row = gridView.GetRow(e.RowHandle) as PatientPackageADO;
                if (row == null) return;
                string status = row.STATUS_CODE;

                // DISABLE icon theo ma trận trạng thái (spec §5.2 - Grid danh sách - Logic hiển thị nút theo trạng thái):
                //   Cho phép        -> repo enabled (icon màu, click fire xử lý).
                //   Không cho phép  -> repo disabled (CÙNG icon nhưng grey-out, KHÔNG click được, KHÔNG popup cảnh báo).
                if (e.Column == colEdit)        e.RepositoryItem = IsActionAllowed(status, PatientPackageRowAction.Edit)   ? repoEdit   : repoEditDis;
                else if (e.Column == colDelete) e.RepositoryItem = IsActionAllowed(status, PatientPackageRowAction.Delete) ? repoDelete : repoDeleteDis;
                else if (e.Column == colPrint)  e.RepositoryItem = IsActionAllowed(status, PatientPackageRowAction.Print)  ? repoPrint  : repoPrintDis;
                else if (e.Column == colPay)    e.RepositoryItem = IsActionAllowed(status, PatientPackageRowAction.Pay)    ? repoPay    : repoPayDis;
                else if (e.Column == colRefund) e.RepositoryItem = IsActionAllowed(status, PatientPackageRowAction.Refund) ? repoRefund : repoRefundDis;
                else if (e.Column == colLock)
                {
                    // DEBUG: log IS_ACTIVE từng row khi grid render icon (gọi lần đầu sau refresh)
                    Inventec.Common.Logging.LogSystem.Info(string.Format(
                        "[Lock-Render] row.ID={0} | IS_ACTIVE={1} -> icon={2}",
                        row.ID,
                        row.IS_ACTIVE.HasValue ? row.IS_ACTIVE.Value.ToString() : "null",
                        (row.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE) ? "repoLock(khóa đóng)" : "repoUnlock(khóa mở)"));

                    e.RepositoryItem = (row.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE) ? repoLock : repoUnlock;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Tô màu dòng theo trạng thái (trắng/vàng/đỏ — mục 2.2 tài liệu).</summary>
        private void gridView_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                GridView view = sender as GridView;
                PatientPackageADO row = view.GetRow(e.RowHandle) as PatientPackageADO;
                if (row == null) return;
                switch (row.STATUS_CODE)
                {
                    case PatientPackageStatusCode.PAID: e.Appearance.BackColor = Color.FromArgb(234, 247, 238); break;      // xanh lá nhạt
                    case PatientPackageStatusCode.REFUNDED: e.Appearance.BackColor = Color.FromArgb(234, 242, 251); break;  // xanh dương nhạt
                    case PatientPackageStatusCode.CANCELED: e.Appearance.BackColor = Color.FromArgb(242, 242, 242); break;  // xám nhạt
                    // Chờ thanh toán: nền trắng mặc định
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private PatientPackageADO GetFocusedPackage()
        {
            try
            {
                int handle = gridView.FocusedRowHandle;
                if (handle < 0) return null;
                return gridView.GetRow(handle) as PatientPackageADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        #endregion

        #region Repository button clicks

        private void repoEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            EditProcess(GetFocusedPackage());
        }

        private void repoDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DeleteProcess(GetFocusedPackage());
        }

        /// <summary>
        /// Icon "khóa đóng" chỉ hiển thị trên dòng đang KHÓA (CANCELED) -> click thực hiện MỞ khóa.
        /// (Đảo so với tên repo: tên repoLock dùng cho icon hiển thị trạng thái KHÓA hiện tại của dòng.)
        /// </summary>
        private void repoLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            UnlockProcess(GetFocusedPackage());
        }

        /// <summary>
        /// Icon "khóa mở" chỉ hiển thị trên dòng đang MỞ -> click thực hiện KHÓA.
        /// </summary>
        private void repoUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            LockProcess(GetFocusedPackage());
        }

        private void repoPay_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            PayProcess(GetFocusedPackage());
        }

        private void repoRefund_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            RefundProcess(GetFocusedPackage());
        }

        private void repoPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            PrintProcess(GetFocusedPackage());
        }

        /// <summary>
        /// Dispatch action theo column - dùng gridControl.MouseClick (WinForms native) thay vì
        /// RowCellClick (DevExpress event có thể bị repository absorb).
        /// Hit-test bằng CalcHitInfo để biết click vào cột nào, row nào.
        /// </summary>
        private void gridControl_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info =
                    gridView.CalcHitInfo(new System.Drawing.Point(e.X, e.Y));
                if (!info.InRowCell || info.RowHandle < 0) return;

                PatientPackageADO row = gridView.GetRow(info.RowHandle) as PatientPackageADO;
                if (row == null) return;

                string status = row.STATUS_CODE;
                Action action = null;

                if (info.Column == colEdit && IsActionAllowed(status, PatientPackageRowAction.Edit))
                    action = () => EditProcess(row);
                else if (info.Column == colDelete && IsActionAllowed(status, PatientPackageRowAction.Delete))
                    action = () => DeleteProcess(row);
                else if (info.Column == colPrint && IsActionAllowed(status, PatientPackageRowAction.Print))
                    action = () => PrintProcess(row);
                else if (info.Column == colPay && IsActionAllowed(status, PatientPackageRowAction.Pay))
                    action = () => PayProcess(row);
                else if (info.Column == colRefund && IsActionAllowed(status, PatientPackageRowAction.Refund))
                    action = () => RefundProcess(row);
                else if (info.Column == colLock)
                {
                    // DEBUG: log state khi click vào icon Lock
                    Inventec.Common.Logging.LogSystem.Info(string.Format(
                        "[Lock-Click] row.ID={0} | row.IS_ACTIVE={1} | row.STATUS_CODE='{2}' | row.LOCKED_REASON='{3}' | route={4}",
                        row.ID,
                        row.IS_ACTIVE.HasValue ? row.IS_ACTIVE.Value.ToString() : "null",
                        row.STATUS_CODE ?? "null",
                        row.LOCKED_REASON ?? "null",
                        (row.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE) ? "UnlockProcess" : "LockProcess"));

                    action = (row.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        ? new Action(() => UnlockProcess(row))
                        : new Action(() => LockProcess(row));
                }

                if (action != null)
                {
                    gridView.FocusedRowHandle = info.RowHandle;
                    action();  // gọi trực tiếp — Process methods ShowDialog block nên không cần BeginInvoke
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #endregion
    }
}
