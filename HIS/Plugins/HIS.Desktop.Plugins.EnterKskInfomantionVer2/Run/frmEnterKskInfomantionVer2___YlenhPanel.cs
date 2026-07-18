/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Logic panel "Danh sách y lệnh khám" (UI dựng trong Designer: grpYlenh thu về trái kiểu EmrDocument,
 * chứa pnlYlenh = combo phòng khám + nút Tìm + ô từ khóa + grid). File này chỉ đổ dữ liệu & xử lý sự kiện:
 * nạp combo phòng khám (V_HIS_ROOM có IS_EXAM=1), nạp danh sách y lệnh KHÁM chưa kết thúc theo phòng +
 * từ khóa, tô màu dòng như ExecuteRoom, click 1 dòng -> nạp lại form KSK theo y lệnh đó.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private bool ylenhPanelInited = false;

        /// <summary>Dòng danh sách y lệnh khám (đã format sẵn để bind + màu).</summary>
        private class YlenhRowADO
        {
            public int STT { get; set; }
            public long? NUM_ORDER { get; set; }
            public string SERVICE_REQ_CODE { get; set; }
            public string PATIENT_NAME { get; set; }
            public string INTRUCTION_TIME_STR { get; set; }
            public long ID { get; set; }
            public long? PRIORITY { get; set; }
            public string HEIN_CARD_NUMBER { get; set; }
        }

        /// <summary>Đổ dữ liệu cho panel y lệnh (UI đã có sẵn trong Designer). Gọi 1 lần ở Load.</summary>
        private void InitYlenhData()
        {
            if (ylenhPanelInited) return;
            ylenhPanelInited = true;
            try
            {
                InitYlenhGridColumns();
                // Enter ở ô Từ khóa = bấm nút Tìm.
                this.txtYlenhKeyword.KeyDown -= txtYlenhKeyword_KeyDown;
                this.txtYlenhKeyword.KeyDown += txtYlenhKeyword_KeyDown;
                LoadYlenhRoomCombo();                 // mặc định = phòng khám hiện tại
                InitYlenhDateDefault();               // mặc định = đầu/cuối ngày hôm nay
                LoadYlenhListInBackground();          // tìm bằng luồng ngoài, không ảnh hưởng load KSK
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Định dạng dd/MM/yyyy HH:mm:ss + mặc định Từ = đầu ngày hôm nay (00:00:00), Đến = cuối ngày (23:59:59).</summary>
        private void InitYlenhDateDefault()
        {
            try
            {
                SetYlenhDateFormat(this.dteYlenhFrom);
                SetYlenhDateFormat(this.dteYlenhTo);
                DateTime today = DateTime.Now.Date;
                this.dteYlenhFrom.EditValue = today;                                 // 00:00:00
                this.dteYlenhTo.EditValue = today.AddDays(1).AddSeconds(-1);          // 23:59:59
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void SetYlenhDateFormat(DevExpress.XtraEditors.DateEdit dte)
        {
            dte.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dte.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            dte.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dte.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            dte.Properties.Mask.UseMaskAsDisplayFormat = true;
            dte.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";
        }

        private void InitYlenhGridColumns()
        {
            try
            {
                this.gridViewYlenh.OptionsBehavior.Editable = false;         // không cho sửa
                this.gridViewYlenh.OptionsView.ShowGroupPanel = false;
                this.gridViewYlenh.OptionsView.ColumnAutoWidth = false;
                this.gridViewYlenh.Columns.Clear();

                GridColumn cStt = this.gridViewYlenh.Columns.AddVisible("STT"); cStt.Caption = "STT"; cStt.Width = 40;
                GridColumn cNum = this.gridViewYlenh.Columns.AddVisible("NUM_ORDER"); cNum.Caption = "#"; cNum.Width = 40;
                GridColumn cCode = this.gridViewYlenh.Columns.AddVisible("SERVICE_REQ_CODE"); cCode.Caption = "Mã y lệnh"; cCode.Width = 110;
                GridColumn cName = this.gridViewYlenh.Columns.AddVisible("PATIENT_NAME"); cName.Caption = "Tên bệnh nhân"; cName.Width = 160;
                GridColumn cTime = this.gridViewYlenh.Columns.AddVisible("INTRUCTION_TIME_STR"); cTime.Caption = "Thời gian y lệnh"; cTime.Width = 110;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Combo phòng: V_HIS_ROOM có IS_EXAM=1 (phòng khám). Mặc định = phòng của y lệnh đang mở.</summary>
        private void LoadYlenhRoomCombo()
        {
            try
            {
                var examRoomIds = new HashSet<long>(
                    BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
                        .Where(o => o.IS_EXAM == 1)
                        .Select(o => o.ROOM_ID));
                var rooms = BackendDataWorker.Get<V_HIS_ROOM>()
                    .Where(o => o.IS_ACTIVE == 1 && examRoomIds.Contains(o.ID))
                    .OrderBy(o => o.ROOM_NAME).ToList();

                this.cboYlenhRoom.Properties.DataSource = rooms;
                this.cboYlenhRoom.Properties.DisplayMember = "ROOM_NAME";
                this.cboYlenhRoom.Properties.ValueMember = "ID";
                this.cboYlenhRoom.Properties.NullText = "";
                this.cboYlenhRoom.Properties.View.Columns.Clear();
                var colCode = this.cboYlenhRoom.Properties.View.Columns.AddField("ROOM_CODE");
                colCode.VisibleIndex = 0; colCode.Caption = "Mã phòng"; colCode.Width = 80;
                var colName = this.cboYlenhRoom.Properties.View.Columns.AddField("ROOM_NAME");
                colName.VisibleIndex = 1; colName.Caption = "Tên phòng"; colName.Width = 220;
                this.cboYlenhRoom.Properties.View.OptionsView.ShowColumnHeaders = true;
                this.cboYlenhRoom.Properties.PopupFormWidth = 320;

                // Mặc định = phòng thực hiện của y lệnh đang mở.
                if (currentServiceReq != null && currentServiceReq.EXECUTE_ROOM_ID != null && currentServiceReq.EXECUTE_ROOM_ID > 0)
                    this.cboYlenhRoom.EditValue = currentServiceReq.EXECUTE_ROOM_ID;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnYlenhSearch_Click(object sender, EventArgs e)
        {
            LoadYlenhList();
        }

        /// <summary>Enter ở ô Từ khóa -> tìm luôn (như bấm nút Tìm).</summary>
        private void txtYlenhKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                LoadYlenhList();
            }
        }

        /// <summary>Nạp danh sách (đồng bộ) — dùng cho nút Tìm.</summary>
        private void LoadYlenhList()
        {
            try
            {
                long roomId; string kw; long tfrom, tto;
                ReadYlenhFilterValues(out roomId, out kw, out tfrom, out tto);
                var rows = FetchYlenhRows(roomId, kw, tfrom, tto);
                BindYlenhRows(rows);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>
        /// Nạp danh sách bằng LUỒNG NGOÀI khi mới mở form — đọc tham số trên UI thread, gọi API + xử lý
        /// trên thread nền (không chặn/ảnh hưởng luồng load dữ liệu khám sức khỏe), rồi bind lại qua BeginInvoke.
        /// </summary>
        private void LoadYlenhListInBackground()
        {
            try
            {
                long roomId; string kw; long tfrom, tto;
                ReadYlenhFilterValues(out roomId, out kw, out tfrom, out tto);   // đọc trên UI thread
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var rows = FetchYlenhRows(roomId, kw, tfrom, tto);       // API + build trên thread nền
                        if (this.IsHandleCreated && !this.IsDisposed)
                            this.BeginInvoke(new Action(() => BindYlenhRows(rows)));
                    }
                    catch (Exception exBg) { Inventec.Common.Logging.LogSystem.Error(exBg); }
                });
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Đọc tham số lọc từ control (BẮT BUỘC gọi trên UI thread). tfrom/tto=0 nghĩa là không lọc.</summary>
        private void ReadYlenhFilterValues(out long roomId, out string kw, out long tfrom, out long tto)
        {
            roomId = 0;
            if (this.cboYlenhRoom.EditValue != null) long.TryParse(this.cboYlenhRoom.EditValue.ToString(), out roomId);
            kw = (this.txtYlenhKeyword.Text ?? "").Trim();
            tfrom = 0; tto = 0;
            // Ô "Từ" = đầu ngày (000000), ô "Đến" = cuối ngày (235959).
            if (this.dteYlenhFrom.EditValue != null && this.dteYlenhFrom.DateTime != DateTime.MinValue)
                tfrom = Inventec.Common.TypeConvert.Parse.ToInt64(this.dteYlenhFrom.DateTime.ToString("yyyyMMdd") + "000000");
            if (this.dteYlenhTo.EditValue != null && this.dteYlenhTo.DateTime != DateTime.MinValue)
                tto = Inventec.Common.TypeConvert.Parse.ToInt64(this.dteYlenhTo.DateTime.ToString("yyyyMMdd") + "235959");
        }

        /// <summary>Gọi API + lọc + dựng danh sách dòng (KHÔNG chạm UI — an toàn gọi ở thread nền).</summary>
        private List<YlenhRowADO> FetchYlenhRows(long roomId, string kw, long tfrom, long tto)
        {
            var filter = new HisServiceReqViewFilter();
            if (roomId > 0) filter.EXECUTE_ROOM_ID = roomId;
            if (tfrom > 0) filter.INTRUCTION_TIME_FROM = tfrom;
            if (tto > 0) filter.INTRUCTION_TIME_TO = tto;

            var param = new CommonParam();
            var data = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(
                "api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param) ?? new List<V_HIS_SERVICE_REQ>();

            // Chỉ y lệnh KHÁM (KH) + bỏ y lệnh ĐÃ KẾT THÚC (STT hoàn thành).
            long typeKh = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
            long sttHt = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
            data = data.Where(o => o.SERVICE_REQ_TYPE_ID == typeKh && o.SERVICE_REQ_STT_ID != sttHt).ToList();

            // Lọc từ khóa (mã y lệnh / tên bệnh nhân) — không dấu.
            if (!string.IsNullOrEmpty(kw))
            {
                string kwu = Inventec.Common.String.Convert.UnSignVNese(kw.ToLower());
                data = data.Where(o =>
                    Inventec.Common.String.Convert.UnSignVNese((o.SERVICE_REQ_CODE ?? "").ToLower()).Contains(kwu)
                    || Inventec.Common.String.Convert.UnSignVNese((o.TDL_PATIENT_NAME ?? "").ToLower()).Contains(kwu)).ToList();
            }

            data = data.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.INTRUCTION_TIME).ToList();

            var rows = new List<YlenhRowADO>();
            int stt = 0;
            foreach (var o in data)
            {
                stt++;
                rows.Add(new YlenhRowADO
                {
                    STT = stt,
                    NUM_ORDER = o.NUM_ORDER,
                    SERVICE_REQ_CODE = o.SERVICE_REQ_CODE,
                    PATIENT_NAME = o.TDL_PATIENT_NAME,
                    INTRUCTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(o.INTRUCTION_TIME),
                    ID = o.ID,
                    PRIORITY = o.PRIORITY,
                    HEIN_CARD_NUMBER = o.TDL_HEIN_CARD_NUMBER
                });
            }
            return rows;
        }

        /// <summary>Gán datasource cho grid (UI thread).</summary>
        private void BindYlenhRows(List<YlenhRowADO> rows)
        {
            this.gridControlYlenh.DataSource = null;
            this.gridControlYlenh.DataSource = rows;
        }

        /// <summary>Màu dòng tương tự ExecuteRoom: ưu tiên -> đậm; có thẻ BHYT -> xanh dương.</summary>
        private void gridViewYlenh_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                var row = this.gridViewYlenh.GetRow(e.RowHandle) as YlenhRowADO;
                if (row == null) return;
                if (row.PRIORITY != null && row.PRIORITY == 1)
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                if (!string.IsNullOrEmpty(row.HEIN_CARD_NUMBER))
                    e.Appearance.ForeColor = System.Drawing.Color.Blue;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>1 click trúng dòng dữ liệu -> nạp lại form theo y lệnh đó.</summary>
        private void gridViewYlenh_Click(object sender, EventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;
                System.Drawing.Point pt = view.GridControl.PointToClient(Control.MousePosition);
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = view.CalcHitInfo(pt);
                if (hit == null || !hit.InRow || hit.RowHandle < 0) return; // bỏ qua header/trống

                var row = view.GetRow(hit.RowHandle) as YlenhRowADO;
                if (row == null || row.ID <= 0) return;
                if (currentServiceReq != null && currentServiceReq.ID == row.ID) return; // đang mở rồi
                ReloadForServiceReq(row.ID);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>
        /// Đồng bộ phòng/loại phòng của currentModule theo y lệnh được chọn (ưu tiên phòng thực hiện, fallback
        /// phòng yêu cầu). Ảnh hưởng: sdo.RequestRoomId khi Lưu + RoomId/RoomTypeId khi mở module con.
        /// </summary>
        private void UpdateModuleRoomByServiceReq(V_HIS_SERVICE_REQ sreq)
        {
            try
            {
                if (currentModule == null || sreq == null) return;

                long roomIdSel = 0;
                if (sreq.EXECUTE_ROOM_ID > 0) roomIdSel = sreq.EXECUTE_ROOM_ID;
                else if (sreq.REQUEST_ROOM_ID > 0) roomIdSel = sreq.REQUEST_ROOM_ID;
                if (roomIdSel <= 0) return;

                currentModule.RoomId = roomIdSel;
                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomIdSel);
                if (room != null)
                    currentModule.RoomTypeId = Convert.ToInt64(room.ROOM_TYPE_ID);

                // Đồng bộ luôn combo phòng trên panel để nhất quán.
                this.cboYlenhRoom.EditValue = roomIdSel;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Double-click: load y lệnh như click 1 lần, KHÁC BIỆT là tự thu gọn panel (Expanded=false).</summary>
        private void gridViewYlenh_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;
                System.Drawing.Point pt = view.GridControl.PointToClient(Control.MousePosition);
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = view.CalcHitInfo(pt);
                if (hit == null || !hit.InRow || hit.RowHandle < 0) return;

                var row = view.GetRow(hit.RowHandle) as YlenhRowADO;
                if (row == null || row.ID <= 0) return;

                // Load thông tin (nếu chưa phải y lệnh đang mở — click đơn có thể đã nạp ở nhịp đầu double-click).
                if (currentServiceReq == null || currentServiceReq.ID != row.ID)
                    ReloadForServiceReq(row.ID);

                // Tự thu gọn panel danh sách y lệnh về trái.
                if (grpYlenh != null) grpYlenh.Expanded = false;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Nạp lại toàn form theo 1 y lệnh khác (không mở form mới).</summary>
        private void ReloadForServiceReq(long serviceReqId)
        {
            try
            {
                var param = new CommonParam();
                var filter = new HisServiceReqViewFilter();
                filter.ID = serviceReqId;
                var list = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(
                    "api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
                var sreq = (list != null) ? list.FirstOrDefault() : null;
                if (sreq == null) return;

                this.SuspendLayout();
                this.currentServiceReq = sreq;

                // Cập nhật phòng/loại phòng của module theo y lệnh mới (dùng cho sdo.RequestRoomId khi Lưu
                // và khi mở các module con). Nếu không đổi, sẽ lưu nhầm theo phòng của y lệnh mở ban đầu.
                UpdateModuleRoomByServiceReq(sreq);

                // Reset state để fill lại sạch.
                currentKskGeneral = null;
                currentKskOverEight = null;
                currentKskUnderEight = null;
                currentKskPeriodDriver = null;
                currentKskDriverCar = null;
                currentKskOther = null;
                currentKsKOccupational = null;
                currentKskUnderSixEf = null;   // tránh Lưu tab <6t ghi đè ID bản ghi y lệnh trước
                // Reset các đối tượng DHST của TỪNG tab: chúng chỉ được set khi FillData có dữ liệu, nếu không
                // reset thì giữ object y lệnh TRƯỚC -> khi Lưu obj.ID = dhst*.ID (cũ) sẽ ghi nhầm record BN khác,
                // và SetEnableControl (dùng dhstGeneral.EXECUTE_LOGINNAME) đánh giá enable/disable sai.
                dhstGeneral = null; dhstOverEighteen = null; dhstUnderEighteen = null;
                dhstUnderSix = null; dhstOccupational = null;
                // List tiền sử bệnh (lái xe) + tiêm chủng: cũng gán trong FillData if-data -> reset tránh dùng lại của y lệnh trước.
                lstDataDriverDity = null; lstDataDriverDityOverE = null; lstDataUneiVaty = null;
                for (int i = 0; i < tabFilled.Length; i++) tabFilled[i] = false;
                preKskGenerals = null; preKskOccupationals = null;
                preKskOverEighteens = null; preKskUnderEighteens = null; preKskDriverCars = null;
                preKskPeriodDrivers = null; preKskOthers = null; preKskUnderSixes = null;
                preTreatments = null; preBabies = null; preKskContracts = null;
                preDhstById = null; preUneiVatys = null; preDitysOverE = null; preDitysPeriodDriver = null;
                preVaccineTypes = null; preDiseaseTypesOverE = null; preDiseaseTypesPeriodDriver = null;

                // Fill lại theo y lệnh mới.
                PrefetchFormData();
                ShowInformationPatient();
                int defaultTab = ResolveDefaultTab();
                EnsureTabLoaded(defaultTab);
                LoadKskHistoryIcdToUc();
                LoadConcluderComboExt();
                LoadConclusionTimeExt();
                SetTabDefault();
                UpdateFinishButtonEnable();   // bật/tắt nút Kết thúc theo trạng thái y lệnh mới
                // Fill lại ĐÚNG tab đang hiển thị: nếu SetTabDefault không đổi tab (y lệnh mới chưa có bản ghi
                // KSK, hoặc tab đang xem khác tab mặc định) thì tab đang xem đã bị reset (tabFilled=false) nhưng
                // CHƯA được fill -> vẫn hiện dữ liệu cũ. EnsureTabLoaded idempotent nên gọi lại an toàn.
                EnsureTabLoaded(xtraTabControl1.SelectedTabPageIndex);
                SetEnableControl();

                this.ResumeLayout(false);
            }
            catch (Exception ex)
            {
                try { this.ResumeLayout(false); } catch { }
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
