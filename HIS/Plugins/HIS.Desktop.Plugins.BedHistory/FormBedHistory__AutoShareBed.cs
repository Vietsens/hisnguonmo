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
using HIS.Desktop.Plugins.BedHistory.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.BedHistory
{
    /// <summary>
    /// Tu dong nam ghep - chi chay khi config HIS.Desktop.Plugins.BedHistory.IsAutoShareBed = "1".
    /// Moi khi mot ban ghi giuong duoc tao, doi giuong, doi gio bat dau/ket thuc hoac bi xoa thi dem lai so benh nhan
    /// tren giuong tai tung moc thoi gian bi anh huong. Ban ghi nao ghi sai so nguoi nam ghep thi cat tai moc do:
    /// ban ghi cu ket thuc dung moc, ban ghi moi bat dau dung moc voi so nguoi nam ghep moi. Nho vay lich su giuong cua
    /// tung benh nhan noi lien nhau (don -> ghep doi -> ghep 3 -> ...), khong ho va khong chong thoi gian.
    /// KHONG dung api/HisBedLog/UpdateShareCount nhu cach cu vi api do ghi de so nguoi nam ghep len CA ban ghi cu,
    /// lam khoang thoi gian benh nhan con nam mot minh cung bi tinh la nam ghep.
    /// </summary>
    public partial class FormBedHistory
    {
        private bool isReloadAfterAutoShareBedQueued = false;

        /// <summary>
        /// Lay ban ghi giuong tu server. Dung de biet giuong/gio cu truoc khi sua hoac xoa.
        /// Khong lay tu listCurrentBedLog vi su kien tich chon (IsChecked) da ghi de gio moi vao do truoc khi luu.
        /// </summary>
        private V_HIS_BED_LOG GetBedLogById(long bedLogId)
        {
            V_HIS_BED_LOG result = null;
            try
            {
                HisBedLogViewFilter filter = new HisBedLogViewFilter();
                filter.ID = bedLogId;
                var bedLogs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BED_LOG>>(Base.GlobalStore.HIS_BED_LOG_GETVIEW, ApiConsumers.MosConsumer, filter, null);
                result = bedLogs != null ? bedLogs.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                result = null;
                LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// So benh nhan KHAC dang nam giuong trong khoang thoi gian cua dong dang sua. Chi dung de dien tam so nguoi
        /// nam ghep len luoi khi chon giuong; so chinh xac duoc tinh lai sau khi luu (ProcessAutoShareBed).
        /// </summary>
        private long CountOtherPatientOnBed(HisBedADO dataBed)
        {
            long result = 0;
            try
            {
                if (dataBed != null && dataBed.TREATMENT_BED_ROOM_IDs != null)
                {
                    long currentTreatmentBedRoomId = this._TreatmentBedRoom != null ? this._TreatmentBedRoom.ID : 0;
                    result = dataBed.TREATMENT_BED_ROOM_IDs.Where(o => o != currentTreatmentBedRoomId).Distinct().Count();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Xu ly tu dong nam ghep sau khi mot ban ghi giuong duoc luu hoac xoa.
        /// </summary>
        /// <param name="before">Ban ghi truoc khi sua/xoa. Null khi tao moi.</param>
        /// <param name="after">Ban ghi sau khi luu. Null khi xoa.</param>
        /// <returns>true neu co ban ghi giuong cua chinh ho so dang mo bi cat/sua, can nap lai luoi.</returns>
        private bool ProcessAutoShareBed(V_HIS_BED_LOG before, HIS_BED_LOG after)
        {
            bool result = false;
            try
            {
                if (!this.IsAutoShareBedOn)
                    return false;

                // Chi sua dich vu giuong, doi tuong thanh toan, so nguoi nam ghep... thi so nguoi tren giuong khong doi,
                // khong xu ly de nguoi dung van sua tay duoc so nguoi nam ghep khi can.
                if (before != null && after != null
                    && before.BED_ID == after.BED_ID
                    && before.START_TIME == after.START_TIME
                    && before.FINISH_TIME == after.FINISH_TIME)
                    return false;

                // Moi giuong bi anh huong ung voi mot khoang thoi gian can dem lai. Gio ket thuc null la den hien tai.
                // Doi giuong thi ca giuong cu (benh nhan roi giuong) lan giuong moi (benh nhan vao giuong) deu phai dem lai.
                Dictionary<long, long> fromTimeByBed = new Dictionary<long, long>();
                Dictionary<long, long?> toTimeByBed = new Dictionary<long, long?>();
                if (before != null)
                    this.AddAutoShareBedRange(fromTimeByBed, toTimeByBed, before.BED_ID, before.START_TIME, before.FINISH_TIME);
                if (after != null)
                    this.AddAutoShareBedRange(fromTimeByBed, toTimeByBed, after.BED_ID, after.START_TIME, after.FINISH_TIME);

                List<HIS_BED_LOG> changedLogs = new List<HIS_BED_LOG>();
                List<AutoShareBedError> errors = new List<AutoShareBedError>();
                foreach (long bedId in fromTimeByBed.Keys)
                {
                    this.RecountShareCountOnBed(bedId, fromTimeByBed[bedId], toTimeByBed[bedId], changedLogs, errors);
                }

                if (changedLogs.Count > 0)
                {
                    List<long> ownTreatmentBedRoomIds = new List<long>();
                    if (this._TreatmentBedRoom != null)
                        ownTreatmentBedRoomIds.Add(this._TreatmentBedRoom.ID);
                    if (this.listCurrentBedLog != null)
                        ownTreatmentBedRoomIds.AddRange(this.listCurrentBedLog.Select(o => o.TREATMENT_BED_ROOM_ID));
                    if (before != null)
                        ownTreatmentBedRoomIds.Add(before.TREATMENT_BED_ROOM_ID);
                    if (after != null)
                        ownTreatmentBedRoomIds.Add(after.TREATMENT_BED_ROOM_ID);

                    result = changedLogs.Exists(o => ownTreatmentBedRoomIds.Contains(o.TREATMENT_BED_ROOM_ID));
                    LogSystem.Info("Tu dong nam ghep: da cap nhat/tao " + changedLogs.Count + " ban ghi giuong. " + LogUtil.TraceData("changedLogIds", changedLogs.Select(o => o.ID).ToList()));
                }

                if (errors.Count > 0)
                {
                    this.ShowAutoShareBedErrors(errors);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        private void AddAutoShareBedRange(Dictionary<long, long> fromTimeByBed, Dictionary<long, long?> toTimeByBed, long bedId, long startTime, long? finishTime)
        {
            if (bedId <= 0)
                return;

            if (!fromTimeByBed.ContainsKey(bedId))
            {
                fromTimeByBed[bedId] = startTime;
                toTimeByBed[bedId] = finishTime;
                return;
            }

            fromTimeByBed[bedId] = Math.Min(fromTimeByBed[bedId], startTime);
            if (!toTimeByBed[bedId].HasValue || !finishTime.HasValue)
                toTimeByBed[bedId] = null;
            else
                toTimeByBed[bedId] = Math.Max(toTimeByBed[bedId].Value, finishTime.Value);
        }

        /// <summary>
        /// Dem lai so benh nhan tren mot giuong trong khoang [fromTime, toTime) va cat ban ghi tai cac moc can thiet.
        /// So nguoi tren giuong chi doi o gio bat dau/ket thuc cua cac ban ghi, nen chi can xet: dau khoang va moi gio
        /// bat dau/ket thuc nam trong khoang. Tai moi moc, ban ghi dang nam giuong ([START_TIME, FINISH_TIME)) co so
        /// nguoi nam ghep khac so benh nhan thuc te thi:
        ///  - bat dau dung moc: chi sua so nguoi nam ghep;
        ///  - bat dau truoc moc: ket thuc ban ghi cu tai moc va tao ban ghi moi tu moc.
        /// Moc xet theo thu tu tang dan nen ban ghi moi tao ra se duoc xet tiep o cac moc sau.
        /// </summary>
        private void RecountShareCountOnBed(long bedId, long fromTime, long? toTime, List<HIS_BED_LOG> changedLogs, List<AutoShareBedError> errors)
        {
            TakeBedsInUseSDO sdo = new TakeBedsInUseSDO();
            sdo.BedIds = new List<long>() { bedId };
            sdo.StartTime = fromTime;
            sdo.FinishTime = toTime;
            LogSystem.Debug("Tu dong nam ghep: " + LogUtil.TraceData("TakeBedsInUseSDO", sdo));
            List<HIS_BED_LOG> bedLogs = new BackendAdapter(new CommonParam()).Post<List<HIS_BED_LOG>>("/api/HisBedLog/TakeBedsInUse", ApiConsumers.MosConsumer, sdo, null);
            if (bedLogs == null || bedLogs.Count == 0)
                return;

            List<long> points = new List<long>() { fromTime };
            foreach (var bedLog in bedLogs)
            {
                if (bedLog.START_TIME > fromTime && (!toTime.HasValue || bedLog.START_TIME < toTime.Value))
                    points.Add(bedLog.START_TIME);
                if (bedLog.FINISH_TIME.HasValue && bedLog.FINISH_TIME.Value > fromTime && (!toTime.HasValue || bedLog.FINISH_TIME.Value < toTime.Value))
                    points.Add(bedLog.FINISH_TIME.Value);
            }
            points = points.Distinct().OrderBy(o => o).ToList();

            foreach (long point in points)
            {
                List<HIS_BED_LOG> activeLogs = bedLogs.Where(o => o.START_TIME <= point && (!o.FINISH_TIME.HasValue || o.FINISH_TIME.Value > point)).ToList();
                long patientCount = activeLogs.Select(o => o.TREATMENT_BED_ROOM_ID).Distinct().Count();
                // Nam mot minh giu SHARE_COUNT = null nhu ban ghi nam don binh thuong
                long? shareCount = patientCount > 1 ? (long?)patientCount : null;

                foreach (var bedLog in activeLogs)
                {
                    if (this.IsSameShareCount(bedLog.SHARE_COUNT, shareCount))
                        continue;

                    // Khong dung tien giuong tam tinh thi server KHONG tu sua so luong/tien cua dich vu giuong da chi dinh
                    // khi ban ghi giuong doi gio hay doi so nguoi nam ghep (UsingBedTempProcessor chi chay khi
                    // IS_USING_BED_TEMP = 1). Cat ban ghi luc nay thi tien cu giu nguyen, doan moi lai bi chi dinh them
                    // -> tinh trung tien. De nguoi dung tu dieu chinh.
                    if (this.IsUsingBedTemp != "1" && (bedLog.SERVICE_REQ_ID.HasValue || bedLog.IS_SERVICE_REQ_ASSIGNED == 1))
                    {
                        errors.Add(new AutoShareBedError(bedLog, point, "Bản ghi giường đã được chỉ định dịch vụ giường, cần điều chỉnh tay lịch sử giường và dịch vụ giường."));
                        continue;
                    }

                    string error = null;
                    if (bedLog.START_TIME == point)
                    {
                        HIS_BED_LOG toUpdate = this.CloneBedLog(bedLog);
                        toUpdate.SHARE_COUNT = shareCount;
                        if (this.SaveBedLogByAutoShareBed(toUpdate, ref error) == null)
                        {
                            errors.Add(new AutoShareBedError(bedLog, point, error));
                            continue;
                        }

                        bedLog.SHARE_COUNT = shareCount;
                        changedLogs.Add(bedLog);
                    }
                    else
                    {
                        long? oldFinishTime = bedLog.FINISH_TIME;

                        HIS_BED_LOG toFinish = this.CloneBedLog(bedLog);
                        toFinish.FINISH_TIME = point;
                        if (this.SaveBedLogByAutoShareBed(toFinish, ref error) == null)
                        {
                            errors.Add(new AutoShareBedError(bedLog, point, error));
                            continue;
                        }

                        HIS_BED_LOG toCreate = this.CloneBedLog(bedLog);
                        toCreate.ID = 0;
                        toCreate.START_TIME = point;
                        toCreate.FINISH_TIME = oldFinishTime;
                        toCreate.SHARE_COUNT = shareCount;
                        HIS_BED_LOG created = this.SaveBedLogByAutoShareBed(toCreate, ref error);
                        if (created == null)
                        {
                            // Tao ban ghi moi loi thi tra lai gio ket thuc cu, tranh benh nhan bi ho thoi gian nam giuong
                            string rollbackError = null;
                            if (this.SaveBedLogByAutoShareBed(bedLog, ref rollbackError) == null)
                            {
                                LogSystem.Error("Tu dong nam ghep: tra lai gio ket thuc cu that bai. " + LogUtil.TraceData("bedLog", bedLog) + " " + rollbackError);
                                error = error + " Không trả lại được giờ kết thúc cũ, bản ghi đang kết thúc lúc " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(point) + ".";
                            }
                            errors.Add(new AutoShareBedError(bedLog, point, error));
                            continue;
                        }

                        bedLog.FINISH_TIME = point;
                        changedLogs.Add(bedLog);
                        changedLogs.Add(created);
                        bedLogs.Add(created);
                    }
                }
            }
        }

        private bool IsSameShareCount(long? shareCount1, long? shareCount2)
        {
            // null va 1 deu la nam mot minh
            if ((shareCount1 ?? 1) <= 1 && (shareCount2 ?? 1) <= 1)
                return true;
            return shareCount1 == shareCount2;
        }

        private HIS_BED_LOG CloneBedLog(HIS_BED_LOG bedLog)
        {
            HIS_BED_LOG result = new HIS_BED_LOG();
            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_BED_LOG>(result, bedLog);
            return result;
        }

        /// <summary>
        /// Goi api/HisBedLog/Create (ID = 0) hoac api/HisBedLog/Update giong het khi nguoi dung tu luu, de server xu ly
        /// chi phi giuong, kiem tra da thanh toan... nhu thao tac tay. Nguoi tao/sua la user dang dang nhap.
        /// </summary>
        private HIS_BED_LOG SaveBedLogByAutoShareBed(HIS_BED_LOG bedLog, ref string error)
        {
            HIS_BED_LOG result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisBedLogSDO sdo = new HisBedLogSDO();
                if (bedLog.ID > 0)
                {
                    sdo.Id = bedLog.ID;
                }
                sdo.TreatmentBedRoomId = bedLog.TREATMENT_BED_ROOM_ID;
                sdo.BedId = bedLog.BED_ID;
                sdo.StartTime = bedLog.START_TIME;
                sdo.FinishTime = bedLog.FINISH_TIME;
                sdo.BedServiceTypeId = bedLog.BED_SERVICE_TYPE_ID;
                sdo.ShareCount = bedLog.SHARE_COUNT;
                sdo.PatientTypeId = bedLog.PATIENT_TYPE_ID;
                sdo.PrimaryPatientTypeId = bedLog.PRIMARY_PATIENT_TYPE_ID;
                sdo.ServiceConditionId = bedLog.SERVICE_CONDITION_ID;
                sdo.WorkingRoomId = this.WorkPlaceSDO.RoomId;
                LogSystem.Debug("Tu dong nam ghep: " + LogUtil.TraceData("HisBedLogSDO", sdo));

                result = new BackendAdapter(param).Post<HIS_BED_LOG>(bedLog.ID > 0 ? Base.GlobalStore.HIS_BED_LOG_UPDATE : Base.GlobalStore.HIS_BED_LOG_CREATE, ApiConsumers.MosConsumer, sdo, param);
                if (result == null)
                {
                    List<string> messages = new List<string>();
                    if (param.Messages != null)
                        messages.AddRange(param.Messages);
                    if (param.BugCodes != null)
                        messages.AddRange(param.BugCodes);
                    error = string.Join(". ", messages.Distinct());
                    LogSystem.Error("Tu dong nam ghep: luu ban ghi giuong that bai. " + LogUtil.TraceData("sdo", sdo) + " " + error);
                }
            }
            catch (Exception ex)
            {
                result = null;
                LogSystem.Error(ex);
            }
            return result;
        }

        private void ShowAutoShareBedErrors(List<AutoShareBedError> errors)
        {
            try
            {
                Dictionary<long, string> patientByTreatmentBedRoom = new Dictionary<long, string>();
                HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                filter.IDs = errors.Select(o => o.BedLog.TREATMENT_BED_ROOM_ID).Distinct().ToList();
                var treatmentBedRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, null);
                if (treatmentBedRooms != null)
                {
                    foreach (var item in treatmentBedRooms)
                    {
                        patientByTreatmentBedRoom[item.ID] = item.TREATMENT_CODE + " - " + item.TDL_PATIENT_NAME;
                    }
                }

                List<string> lines = new List<string>();
                foreach (var item in errors)
                {
                    string patient = patientByTreatmentBedRoom.ContainsKey(item.BedLog.TREATMENT_BED_ROOM_ID) ? patientByTreatmentBedRoom[item.BedLog.TREATMENT_BED_ROOM_ID] : "";
                    var bed = this.dataBedADOs != null ? this.dataBedADOs.FirstOrDefault(o => o.ID == item.BedLog.BED_ID) : null;
                    string bedName = bed != null ? bed.BED_NAME : "";
                    lines.Add(string.Format("- {0}, giường {1}, thời điểm {2}: {3}", patient, bedName, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.Point), item.Error));
                }

                DevExpress.XtraEditors.XtraMessageBox.Show("Không tự động cập nhật được thông tin nằm ghép của các bệnh nhân sau, vui lòng kiểm tra và cập nhật lại lịch sử giường của các bệnh nhân này:\r\n" + string.Join("\r\n", lines), ResourceMessage.ThongBao);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nap lai luoi lich su giuong sau khi ban ghi cua chinh ho so dang mo bi cat/sua. Chay qua BeginInvoke vi luu
        /// thuong xay ra ngay trong su kien CellValueChanged/nut tren luoi, doi DataSource luc do de loi editor.
        /// </summary>
        private void QueueReloadAfterAutoShareBed()
        {
            try
            {
                if (this.isReloadAfterAutoShareBedQueued || !this.IsHandleCreated || this.IsDisposed)
                    return;

                this.isReloadAfterAutoShareBedQueued = true;
                this.BeginInvoke(new Action(this.ReloadBedLogAfterAutoShareBed));
            }
            catch (Exception ex)
            {
                this.isReloadAfterAutoShareBedQueued = false;
                LogSystem.Error(ex);
            }
        }

        private void ReloadBedLogAfterAutoShareBed()
        {
            try
            {
                this.isReloadAfterAutoShareBedQueued = false;

                // Giu lai cac dong nguoi dung da nhap nhung chua luu duoc
                List<HisBedHistoryADO> unsavedRows = this.bedLogChecks != null
                    ? this.bedLogChecks.Where(o => o.ID <= 0 && o.BED_ID > 0).ToList()
                    : new List<HisBedHistoryADO>();

                this.GetAllBedLog();
                this.FillDataToGridBedLog();

                if (unsavedRows.Count > 0 && this.bedLogChecks != null)
                {
                    this.bedLogChecks.AddRange(unsavedRows);
                    this.gridControlBedHistory.RefreshDataSource();
                }

                if (!this.IsDisable)
                {
                    this.ProcessAllBedLogChecks();
                    this.CountTimeBed();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private class AutoShareBedError
        {
            public HIS_BED_LOG BedLog { get; set; }
            public long Point { get; set; }
            public string Error { get; set; }

            public AutoShareBedError(HIS_BED_LOG bedLog, long point, string error)
            {
                this.BedLog = bedLog;
                this.Point = point;
                this.Error = error;
            }
        }
    }
}
