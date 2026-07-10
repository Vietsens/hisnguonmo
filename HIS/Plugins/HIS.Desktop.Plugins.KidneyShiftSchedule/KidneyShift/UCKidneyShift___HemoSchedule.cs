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

        /// <summary>
        /// Nạp lưới BN theo lịch chạy thận theo phòng + khoảng ngày đang xem (tuần).
        /// </summary>
        private void FillDataToGridHemoSchedule()
        {
            try
            {
                WaitingManager.Show();
                gridControlHemoSchedule.DataSource = null;
                this._hemoSchedules = new List<MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE>();

                CommonParam paramCommon = new CommonParam();
                // Dùng ExpandoObject để không phụ thuộc tên lớp filter cụ thể của backend
                dynamic filter = new System.Dynamic.ExpandoObject();
                if (cboExecuteRoom.EditValue != null)
                    filter.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboExecuteRoom.EditValue.ToString());
                if (cboCaForSearchServiceReqKidneyshift.EditValue != null)
                    filter.KIDNEY_SHIFT = Inventec.Common.TypeConvert.Parse.ToInt64(cboCaForSearchServiceReqKidneyshift.EditValue.ToString());
                if (dateDateForSearchServiceReqKidneyshift.EditValue != null)
                {
                    filter.SCHEDULE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dateDateForSearchServiceReqKidneyshift.DateTime.ToString("yyyyMMdd") + START_TIME);
                    filter.SCHEDULE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dateDateForSearchServiceReqKidneyshift.DateTime.ToString("yyyyMMdd") + END_TIME);
                }
                else
                {
                    if (dateWeekFrom.EditValue != null)
                        filter.SCHEDULE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dateWeekFrom.DateTime.ToString("yyyyMMdd") + START_TIME);
                    if (dateWeekTo.EditValue != null)
                        filter.SCHEDULE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dateWeekTo.DateTime.ToString("yyyyMMdd") + END_TIME);
                }

                var datas = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_HEMODIALYSIS_SCHEDULE>>(
                    RequestUriStore.HIS_HEMODIALYSIS_SCHEDULE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (datas != null && datas.Count > 0)
                    this._hemoSchedules = datas;

                gridControlHemoSchedule.BeginUpdate();
                gridControlHemoSchedule.DataSource = this._hemoSchedules;
                gridControlHemoSchedule.EndUpdate();
                gridViewHemoSchedule.BestFitColumns();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                gridControlHemoSchedule.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

                this.treatmentId = this.currentHemoSchedule.TREATMENT_ID ?? 0;
                this.LoadDataToCurrentTreatmentData(this.treatmentId);
                this.ProcessDataWithTreatmentWithPatientTypeInfo();
                this.LoadServicePaty();
                this.ResetStateControlForm();
                this.InitComboPatientType(this.currentPatientTypeWithPatientTypeAlter);

                // Pre-fill form "Đưa vào lịch" theo slot lịch (không áp dụng khi điều dưỡng - control đã khóa)
                if (!this.isNurseLoginBlocked)
                {
                    var scheduleDateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHemoSchedule.SCHEDULE_DATE ?? 0);
                    if (scheduleDateTime.HasValue && scheduleDateTime.Value != DateTime.MinValue)
                        dateDateForAdd.DateTime = scheduleDateTime.Value;

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
    }
}
