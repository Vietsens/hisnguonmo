/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift
{
    /// <summary>
    /// 2891 - mục 4.1.3: Vùng giữa phải "BN theo lịch" — hiển thị bệnh nhân đã được
    /// Xếp lịch chạy thận (Xếp lịch MỚI) từ V_HIS_HEMODIALYSIS_SCHEDULE.
    /// Chọn 1 dòng -> nạp bối cảnh điều trị + pre-fill form "Đưa vào lịch" theo slot (y lệnh theo lịch).
    /// LƯU Ý: tên field cột (TDL_PATIENT_*, EXP_MEST_TEMPLATE_NAME, SCHEDULE_DATE, KIDNEY_SHIFT...)
    /// theo thiết kế view V_HIS_HEMODIALYSIS_SCHEDULE của backend (tài liệu 2891 mục 2.4) — DEV xác nhận khi merge lib.
    /// </summary>
    public partial class UCKidneyShift : UserControlBase
    {
        /// <summary>Dòng BN theo lịch đang chọn ở vùng phải</summary>
        private MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE currentHemoSchedule;

        /// <summary>Danh sách BN theo lịch (nguồn bind lưới vùng phải)</summary>
        private List<MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE> _hemoSchedules;

        /// <summary>Chặn FocusedRowChanged tự "cướp" selection khi đang bind dữ liệu grid phải</summary>
        private bool isLoadingHemoGrid = false;

        /// <summary>
        /// Nạp lưới BN theo lịch chạy thận theo phòng + khoảng ngày đang xem (tuần).
        /// </summary>
        private void FillDataToGridHemoSchedule()
        {
            try
            {
                WaitingManager.Show();
                this.isLoadingHemoGrid = true;
                gridControlHemoSchedule.DataSource = null;
                this._hemoSchedules = new List<MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE>();

                CommonParam paramCommon = new CommonParam();
                // Dùng ExpandoObject để không phụ thuộc tên lớp filter cụ thể của backend.
                // Lọc theo bộ lọc RIÊNG của vùng phải (Phòng chạy + Ngày + Ca + Từ khóa).
                dynamic filter = new System.Dynamic.ExpandoObject();
                if (cboExecuteRoomHemo.EditValue != null)
                    filter.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboExecuteRoomHemo.EditValue.ToString());
                if (cboShiftHemoSchedule.EditValue != null)
                    filter.KIDNEY_SHIFT = Inventec.Common.TypeConvert.Parse.ToInt64(cboShiftHemoSchedule.EditValue.ToString());
                if (!string.IsNullOrWhiteSpace(txtSearchForHemoSchedule.Text))
                    filter.KEY_WORD = txtSearchForHemoSchedule.Text.Trim();

                // SCHEDULE_DATE lưu dạng yyyyMMdd (8 chữ số) -> lọc theo 8 chữ số, KHÔNG thêm giờ
                DateTime scheduleDate = dtHemoScheduleDate.EditValue != null ? dtHemoScheduleDate.DateTime : DateTime.Now;
                long scheduleDateNumber = Inventec.Common.TypeConvert.Parse.ToInt64(scheduleDate.ToString("yyyyMMdd"));
                filter.SCHEDULE_DATE_FROM = scheduleDateNumber;
                filter.SCHEDULE_DATE_TO = scheduleDateNumber;

                var datas = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE>>(
                    RequestUriStore.HIS_HEMODIALYSIS_SCHEDULE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (datas != null && datas.Count > 0)
                    this._hemoSchedules = datas;

                gridControlHemoSchedule.BeginUpdate();
                gridControlHemoSchedule.DataSource = this._hemoSchedules;
                gridControlHemoSchedule.EndUpdate();
                // KHÔNG BestFitColumns -> giữ độ rộng cột cố định đã set (ColumnAutoWidth=false -> có scroll ngang)
                // Sau khi bind, bỏ focus mặc định để không tự chọn khi vừa load
                gridViewHemoSchedule.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                gridControlHemoSchedule.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.isLoadingHemoGrid = false;
            }
        }

        /// <summary>
        /// Chọn dòng bằng FocusedRowChanged để đảm bảo lấy đúng dòng ngay lần click đầu
        /// (gridControl.Click có thể trả GetFocusedRow null ở lần click đầu).
        /// </summary>
        private void gridViewHemoSchedule_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (this.isLoadingHemoGrid)
                return;
            this.RowHemoScheduleClick();
        }

        private void gridControlHemoSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                this.RowHemoScheduleClick();
                if (dateDateForAdd.EditValue == null)
                    dateDateForAdd.Focus();
                else if (cboCaForAdd.EditValue == null)
                    cboCaForAdd.Focus();
                else
                {
                    this.cboMarchineForAdd.Focus();
                    this.cboMarchineForAdd.SelectAll();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chọn BN vùng phải -> y lệnh theo lịch: nạp bối cảnh điều trị + pre-fill Ca/Gói/Máy/Ngày từ slot lịch.
        /// Người chỉ định vẫn là BS trực (xử lý ở InitComboUser - R8).
        /// </summary>
        private void RowHemoScheduleClick()
        {
            try
            {
                this.currentHemoSchedule = this.gridViewHemoSchedule.GetFocusedRow() as MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE;
                if (this.currentHemoSchedule == null)
                    return;

                // Mutual exclusivity (2891): đang chọn BN theo lịch (vùng phải)
                // -> bỏ chọn BN vùng trái để chỉ lưu 1 nguồn khi "Đưa vào lịch".
                this.currentTreatmentBedRoomADO = null;
                if (this.gridViewTreatmentBedRoom != null)
                {
                    this.gridViewTreatmentBedRoom.ClearSelection();
                    this.gridViewTreatmentBedRoom.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
                    this.gridControlTreatmentBedRoom.Invalidate();
                }

                this.treatmentId = this.currentHemoSchedule.TREATMENT_ID ?? 0;
                // Y lệnh theo lịch -> resolve đối tượng theo NGÀY chọn ở bộ lọc (ô Ngày), cuối ngày để phủ đối tượng trong ngày
                long intructionTime = dtHemoScheduleDate.EditValue != null
                    ? Inventec.Common.TypeConvert.Parse.ToInt64(dtHemoScheduleDate.DateTime.ToString("yyyyMMdd") + END_TIME)
                    : 0;
                this.LoadDataToCurrentTreatmentData(this.treatmentId, intructionTime);
                this.ProcessDataWithTreatmentWithPatientTypeInfo();
                this.LoadServicePaty();
                this.ResetStateControlForm();
                this.InitComboPatientType(this.currentPatientTypeWithPatientTypeAlter);

                // Pre-fill form "Đưa vào lịch" theo slot lịch (không áp dụng khi điều dưỡng - control đã khóa)
                if (!this.isNurseLoginBlocked)
                {
                    long schDate = this.currentHemoSchedule.SCHEDULE_DATE ?? 0;
                    if (schDate > 0)
                    {
                        // SCHEDULE_DATE lưu yyyyMMdd (8 số) -> đưa về yyyyMMddHHmmss (14 số) để convert
                        long schDateTimeNumber = schDate < 100000000 ? schDate * 1000000 : schDate;
                        var scheduleDateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(schDateTimeNumber);
                        if (scheduleDateTime.HasValue && scheduleDateTime.Value != DateTime.MinValue)
                            dateDateForAdd.DateTime = scheduleDateTime.Value;
                    }

                    cboCaForAdd.EditValue = this.currentHemoSchedule.KIDNEY_SHIFT;

                    if (this.currentHemoSchedule.EXP_MEST_TEMPLATE_ID.HasValue)
                        cboExpMestTemplateForAdd.EditValue = this.currentHemoSchedule.EXP_MEST_TEMPLATE_ID.Value;

                    if (this.currentHemoSchedule.MACHINE_ID.HasValue)
                        cboMarchineForAdd.EditValue = this.currentHemoSchedule.MACHINE_ID.Value;
                }

                if (cboServiceForAdd.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERVICE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>()
                        .Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboServiceForAdd.EditValue ?? "0").ToString())).FirstOrDefault();
                    if (data != null)
                        this.ProcessServiceChange(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHemoSchedule_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    IList source = (IList)((BaseView)sender).DataSource;
                    if (source != null && source.Count > 0)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE row =
                            (MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE)source[e.ListSourceRowIndex];
                        if (row != null)
                        {
                            if (e.Column.FieldName == "STT")
                                e.Value = e.ListSourceRowIndex + 1;
                            else if (e.Column.FieldName == "TDL_PATIENT_DOB_DISPLAY")
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(row.TDL_PATIENT_DOB ?? 0);
                        }
                        else
                            e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khởi tạo bộ lọc riêng vùng "BN theo lịch": Phòng chạy + Ca + Ngày (mặc định hôm nay).
        /// Gọi trong FillDataToControlsForm (sau khi requestRoom đã có).
        /// </summary>
        private void InitHemoScheduleFilter()
        {
            try
            {
                // Ca — tái dùng hàm sẵn có
                this.InitComboCa(this.cboShiftHemoSchedule);

                // Phòng chạy — mirror InitComboExecuteRoom nhưng gán vào txtExecuteRoomHemo
                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRooms =
                    BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                executeRooms = (executeRooms != null && executeRooms.Count > 0)
                    ? executeRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.DEPARTMENT_ID == requestRoom.DEPARTMENT_ID && ((o.IS_KIDNEY ?? -1) == 1)).ToList()
                    : executeRooms;

                if (executeRooms != null && executeRooms.Count > 0)
                {
                    var ordered = executeRooms.Where(u => u.NUM_ORDER != null).OrderBy(u => u.NUM_ORDER).ThenBy(o => o.EXECUTE_ROOM_NAME)
                        .Concat(executeRooms.Where(u => u.NUM_ORDER == null).OrderBy(o => o.EXECUTE_ROOM_NAME)).ToList();
                    cboExecuteRoomHemo.Properties.DataSource = ordered;
                }
                else
                    cboExecuteRoomHemo.Properties.DataSource = null;

                cboExecuteRoomHemo.Properties.ValueMember = "ROOM_ID";
                cboExecuteRoomHemo.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                DevExpress.XtraGrid.Columns.GridColumn colCode = cboExecuteRoomHemo.Properties.View.Columns.AddField("EXECUTE_ROOM_CODE");
                colCode.VisibleIndex = 1; colCode.Width = 100; colCode.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn colName = cboExecuteRoomHemo.Properties.View.Columns.AddField("EXECUTE_ROOM_NAME");
                colName.VisibleIndex = 2; colName.Width = 200; colName.Caption = "Tên";
                cboExecuteRoomHemo.Properties.PopupFormWidth = 320;
                cboExecuteRoomHemo.Properties.View.OptionsView.ShowColumnHeaders = false;

                if (executeRooms != null && executeRooms.Count > 0)
                {
                    cboExecuteRoomHemo.EditValue = executeRooms[0].ROOM_ID;
                    txtExecuteRoomHemo.Text = executeRooms[0].EXECUTE_ROOM_CODE;
                }

                // Ngày mặc định = hôm nay
                this.dtHemoScheduleDate.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gắn sự kiện cho các nút của bộ lọc vùng phải (gọi 1 lần trong Load).
        /// LƯU Ý: KHÔNG wire các nút này trong Designer để tránh chạy 2 lần.
        /// </summary>
        private void WireHemoScheduleFilterEvents()
        {
            // Wire CỨNG event grid phải TRƯỚC (quan trọng nhất — để RowHemoScheduleClick chạy).
            // Dùng -= rồi += để đúng 1 lần, không phụ thuộc Designer (grid dựng tay có thể mất wire).
            // Tách try riêng để 1 nút sai tên không làm hỏng wire grid.
            try
            {
                this.gridControlHemoSchedule.Click -= new EventHandler(this.gridControlHemoSchedule_Click);
                this.gridControlHemoSchedule.Click += new EventHandler(this.gridControlHemoSchedule_Click);
                this.gridViewHemoSchedule.FocusedRowChanged -= new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewHemoSchedule_FocusedRowChanged);
                this.gridViewHemoSchedule.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewHemoSchedule_FocusedRowChanged);
                this.gridViewHemoSchedule.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHemoSchedule_CustomUnboundColumnData);
                this.gridViewHemoSchedule.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHemoSchedule_CustomUnboundColumnData);

                // Mutual exclusivity - tô sáng: tắt appearance mặc định (tránh "bóng ma" ở grid mất focus),
                // tự tô dòng đang chọn qua RowStyle -> chỉ grid đang chọn mới sáng.
                this.gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceFocusedRow = false;
                this.gridViewTreatmentBedRoom.OptionsSelection.EnableAppearanceHideSelection = false;
                this.gridViewHemoSchedule.OptionsSelection.EnableAppearanceFocusedRow = false;
                this.gridViewHemoSchedule.OptionsSelection.EnableAppearanceHideSelection = false;
                this.gridViewTreatmentBedRoom.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewTreatmentBedRoom_RowStyle);
                this.gridViewHemoSchedule.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewHemoSchedule_RowStyle);

                // #1: cột lấp ĐẦY chiều ngang grid (không dư khoảng trắng ở cuối).
                // ColumnAutoWidth = true -> cột tự giãn theo tỉ lệ để phủ hết bề rộng.
                // MinWidth để cột không bị co quá nhỏ -> khi tổng min > bề rộng grid sẽ tự có scroll ngang.
                this.gridViewTreatmentBedRoom.OptionsView.ColumnAutoWidth = true;
                this.gridViewHemoSchedule.OptionsView.ColumnAutoWidth = true;
                // Tự giãn chiều cao dòng để cột word-wrap (Chẩn đoán chính) hiện đủ chữ
                this.gridViewTreatmentBedRoom.OptionsView.RowAutoHeight = true;
                this.gridViewHemoSchedule.OptionsView.RowAutoHeight = true;
                SetHemoGridColumnMinWidth();
                SetTreatmentGridColumnMinWidth();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            // Wire nút bộ lọc (nếu 1 nút thiếu -> chỉ mất nút đó, không ảnh hưởng chọn dòng)
            try
            {
                this.btnSearchForHemoSchedule.Click += new EventHandler(this.btnSearchForHemoSchedule_Click);
                this.btnDatePreviousHemo.Click += new EventHandler(this.btnDatePreviousHemo_Click);
                this.btnDateNextHemo.Click += new EventHandler(this.btnDateNextHemo_Click);
                this.btnShiftPreviousHemo.Click += new EventHandler(this.btnShiftPreviousHemo_Click);
                this.btnShiftNextHemo.Click += new EventHandler(this.btnShiftNextHemo_Click);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Chỉ tô sáng dòng grid trái khi đang chọn BN vùng trái (đột xuất).</summary>
        private void gridViewTreatmentBedRoom_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (this.currentTreatmentBedRoomADO != null && e.RowHandle >= 0
                    && e.RowHandle == this.gridViewTreatmentBedRoom.FocusedRowHandle)
                {
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(179, 217, 255);
                    e.Appearance.Options.UseBackColor = true;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Chỉ tô sáng dòng grid phải khi đang chọn BN theo lịch.</summary>
        private void gridViewHemoSchedule_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (this.currentHemoSchedule != null && e.RowHandle >= 0
                    && e.RowHandle == this.gridViewHemoSchedule.FocusedRowHandle)
                {
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(179, 217, 255);
                    e.Appearance.Options.UseBackColor = true;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đặt MinWidth cho các cột grid phải (theo FieldName) để cột không co quá nhỏ khi ColumnAutoWidth=true;
        /// khi tổng MinWidth vượt bề rộng grid -> tự có scroll ngang, không dư khoảng trắng cuối.
        /// </summary>
        private void SetHemoGridColumnMinWidth()
        {
            try
            {
                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewHemoSchedule.Columns)
                {
                    switch (col.FieldName)
                    {
                        case "STT": col.MinWidth = 40; col.Width = 40; break;
                        case "TDL_PATIENT_NAME": col.MinWidth = 140; col.Width = 160; break;
                        case "TDL_PATIENT_CODE": col.MinWidth = 100; col.Width = 110; break;
                        case "TDL_TREATMENT_CODE": col.MinWidth = 110; col.Width = 120; break;
                        case "TDL_PATIENT_DOB_DISPLAY": col.MinWidth = 80; col.Width = 90; break;
                        case "KIDNEY_SHIFT": col.MinWidth = 45; col.Width = 45; break;
                        case "EXP_MEST_TEMPLATE_NAME": col.MinWidth = 130; col.Width = 150; break;
                        case "NOTE": col.MinWidth = 120; col.Width = 150; break;
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// MinWidth cột grid trái (BN đang điều trị) — nhiều cột thì co tới min rồi tự có scroll ngang,
        /// KHÔNG cắt mất dữ liệu (khắc phục co cột khi ColumnAutoWidth=true + nhiều dòng).
        /// </summary>
        private void SetTreatmentGridColumnMinWidth()
        {
            try
            {
                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewTreatmentBedRoom.Columns)
                {
                    switch (col.FieldName)
                    {
                        case "STT": col.MinWidth = 40; col.Width = 40; break;
                        case "TDL_PATIENT_NAME": col.MinWidth = 160; col.Width = 180; break;
                        case "TDL_PATIENT_CODE": col.MinWidth = 100; col.Width = 110; break;
                        case "TREATMENT_CODE": col.MinWidth = 115; col.Width = 120; break;
                        case "TDL_PATIENT_DOB_DISPLAY": col.MinWidth = 85; col.Width = 90; break;
                        case "TDL_PATIENT_GENDER_NAME": col.MinWidth = 60; col.Width = 65; break;
                        case "IN_TIME_DISPLAY": col.MinWidth = 85; col.Width = 90; break;
                        case "TREATMENT_TYPE_NAME": col.MinWidth = 130; col.Width = 140; break;
                        case "PATIENT_TYPE_NAME": col.MinWidth = 110; col.Width = 120; break;
                        case "TDL_HEIN_CARD_NUMBER": col.MinWidth = 150; col.Width = 160; break;
                        case "BED_ROOM_NAME": col.MinWidth = 90; col.Width = 100; break;
                        case "BED_NAME": col.MinWidth = 80; col.Width = 90; break;
                        case "ICD_NAME":
                            col.MinWidth = 260; col.Width = 300;
                            // Word-wrap chắc chắn: gán MemoEdit (tự xuống dòng) + RowAutoHeight -> dòng tự cao
                            col.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            col.AppearanceCell.Options.UseTextOptions = true;
                            var memoIcd = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                            memoIcd.WordWrap = true;
                            gridControlTreatmentBedRoom.RepositoryItems.Add(memoIcd);
                            col.ColumnEdit = memoIcd;
                            break;
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnSearchForHemoSchedule_Click(object sender, EventArgs e)
        {
            try { this.FillDataToGridHemoSchedule(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnDatePreviousHemo_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime d = dtHemoScheduleDate.EditValue != null ? dtHemoScheduleDate.DateTime : DateTime.Now;
                dtHemoScheduleDate.DateTime = d.AddDays(-1);
                this.FillDataToGridHemoSchedule();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnDateNextHemo_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime d = dtHemoScheduleDate.EditValue != null ? dtHemoScheduleDate.DateTime : DateTime.Now;
                dtHemoScheduleDate.DateTime = d.AddDays(1);
                this.FillDataToGridHemoSchedule();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnShiftPreviousHemo_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeHemoShift(-1);
                this.FillDataToGridHemoSchedule();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnShiftNextHemo_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeHemoShift(1);
                this.FillDataToGridHemoSchedule();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Tăng/giảm Ca trong khoảng 1..5.</summary>
        private void ChangeHemoShift(int delta)
        {
            const long MIN_SHIFT = 1;
            const long MAX_SHIFT = 5;
            long current = cboShiftHemoSchedule.EditValue != null
                ? Inventec.Common.TypeConvert.Parse.ToInt64(cboShiftHemoSchedule.EditValue.ToString())
                : MIN_SHIFT;
            long next = current + delta;
            if (next < MIN_SHIFT) next = MIN_SHIFT;
            if (next > MAX_SHIFT) next = MAX_SHIFT;
            cboShiftHemoSchedule.EditValue = next;
        }
    }
}
