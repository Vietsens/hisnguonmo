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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
using HIS.Desktop.Plugins.TreatmentFinish.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MOS.UTILITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        enum ValidationDataType
        {
            PopupMessage,
            GetListMessage
        }
        private bool CheckAssignServiceBed_ForSave(ValidationDataType validationDataType,ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.TREATMENT_FINISH.CHECK_ASSIGN_SERVICE_BED") == "2")
                {
                    decimal amountBed = 0;
                    decimal amountTreat = Inventec.Common.TypeConvert.Parse.ToDecimal(txtDaysBedTreatment.Text);
                    List<HIS_SERE_SERV> listSereServBed = null;
                    if (this.SereServCheck != null && this.SereServCheck.Count > 0)
                    {
                        listSereServBed = this.SereServCheck.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                    }
                    if (listSereServBed != null && listSereServBed.Count > 0)
                    {
                        amountBed = listSereServBed.Sum(s => s.AMOUNT);
                    }
                    if (amountBed > amountTreat)
                    {
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.SoNgayGiuongLonHonSoNgayGiuongToiDa, amountBed, amountTreat), ResourceMessage.ThongBao);
                            return false;
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = false;
                            warning.Description = String.Format(ResourceMessage.SoNgayGiuongLonHonSoNgayGiuongToiDa, amountBed, amountTreat);
                            listWarningADO.Add(warning);
                        }
                    }
                    else if (amountBed < amountTreat)
                    {
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.SoNgayGiuongNhoHonSoNgayGiuongToiDa, amountBed, amountTreat), ResourceMessage.ThongBao);
                            return false;
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = false;
                            warning.Description = String.Format(ResourceMessage.SoNgayGiuongNhoHonSoNgayGiuongToiDa, amountBed, amountTreat);
                            listWarningADO.Add(warning);
                        }
                        
                    }
                }

                if (Config.CheckFinishTimeCFG.isCheckBedService)
                {
                    GetPatientTypeAlter();
                    if (patientTypeAlter != null && patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        List<HIS_SERE_SERV> listSereServBed = null;

                        if (this.SereServCheck != null && this.SereServCheck.Count > 0)
                        {
                            listSereServBed = this.SereServCheck.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                        }

                        if (listSereServBed == null || listSereServBed.Count <= 0)
                        {
                            if (validationDataType == ValidationDataType.PopupMessage)
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.BanCoMuonTiepTuc, ResourceMessage.BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    return false;
                                }
                            }
                            else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                            {
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = true;
                                warning.Description = ResourceMessage.BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong;
                                listWarningADO.Add(warning);
                            }
                        }
                        else
                        {
                            var amountBed = listSereServBed.Sum(s => s.AMOUNT);
                            decimal amountTreat = Inventec.Common.TypeConvert.Parse.ToDecimal(txtDaysBedTreatment.Text);
                            if (amountBed < amountTreat)
                            {
                                if (validationDataType == ValidationDataType.PopupMessage)
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, String.Format(ResourceMessage.SoNgayGiuongNhoHonSoNgayGiuongToiDa, amountBed, amountTreat)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                    {
                                        return false;
                                    }
                                }
                                else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                                {
                                    WarningADO warning = new WarningADO();
                                    warning.IsSkippable = true;
                                    warning.Description = String.Format(ResourceMessage.SoNgayGiuongNhoHonSoNgayGiuongToiDa, amountBed, amountTreat);
                                    listWarningADO.Add(warning);
                                }
                            }
                            else if (amountBed > amountTreat)
                            {
                                if (validationDataType == ValidationDataType.PopupMessage)
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, String.Format(ResourceMessage.SoNgayGiuongLonHonSoNgayGiuongToiDa, amountBed, amountTreat)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                    {
                                        return false;
                                    }
                                }
                                else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                                {
                                    WarningADO warning = new WarningADO();
                                    warning.IsSkippable = true;
                                    warning.Description = String.Format(ResourceMessage.SoNgayGiuongLonHonSoNgayGiuongToiDa, amountBed, amountTreat);
                                    listWarningADO.Add(warning);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        /// <summary>
        /// Tiền tố log của check "thời gian y lệnh lớn hơn thời gian ra khoa". 
        /// </summary>
        private const string LOG_PREFIX_CHECK_OUT_TIME = "[CheckYLenhVsThoiGianRaKhoa]";

        /// <summary>
        /// Danh sách điều trị kết hợp (khoa phối hợp điều trị) của bệnh án. Lazy load, chỉ dùng cho check thời gian ra khoa.
        /// </summary>
        private List<HIS_CO_TREATMENT> ListCoTreatmentCheckTime = null;

        /// <summary>
        /// Một khoảng thời gian bệnh nhân thuộc quản lý của 1 khoa (nằm khoa hoặc điều trị kết hợp).
        /// </summary>
        private class DepartmentTimeRangeADO
        {
            public long DepartmentId { get; set; }
            public long FromTime { get; set; }
            public long ToTime { get; set; }
            public string Source { get; set; }
        }

        /// <summary> 000000376331
        /// Load danh sách điều trị kết hợp của bệnh án. Khoa điều trị kết hợp KHÔNG sinh bản ghi chuyển khoa
        /// nên nếu không lấy dữ liệu này thì mọi y lệnh của khoa phối hợp đều bị coi là "sau thời gian ra khoa". 
        /// </summary>
        private void LoadCoTreatmentForCheckOutTime()
        {
            try
            {
                if (this.ListCoTreatmentCheckTime != null)
                {
                    return;
                }

                CommonParam param = new CommonParam();
                HisCoTreatmentFilter filter = new HisCoTreatmentFilter();
                filter.TDL_TREATMENT_ID = this.treatmentId;
                this.ListCoTreatmentCheckTime = new BackendAdapter(param).Get<List<HIS_CO_TREATMENT>>("api/HisCoTreatment/Get", ApiConsumers.MosConsumer, filter, param);

                if (this.ListCoTreatmentCheckTime == null)
                {
                    this.ListCoTreatmentCheckTime = new List<HIS_CO_TREATMENT>();
                }
            }
            catch (Exception ex)
            {
                this.ListCoTreatmentCheckTime = new List<HIS_CO_TREATMENT>();
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tên khoa theo ID, dựng 1 lần để log không phải quét lại danh mục khoa trên từng dòng.
        /// </summary>
        private Dictionary<long, string> dicDepartmentNameForLog = null;

        /// <summary>
        /// Tên khoa (dùng cho log).
        /// </summary>
        private string GetDepartmentNameForLog(long departmentId)
        {
            try
            {
                if (this.dicDepartmentNameForLog == null)
                {
                    this.dicDepartmentNameForLog = new Dictionary<long, string>();
                    foreach (var department in BackendDataWorker.Get<HIS_DEPARTMENT>())
                    {
                        this.dicDepartmentNameForLog[department.ID] = department.DEPARTMENT_NAME;
                    }
                }

                string name = null;
                return this.dicDepartmentNameForLog.TryGetValue(departmentId, out name) ? name : "?";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return "?";
            }
        }

        /// <summary>
        /// Dựng danh sách khoảng thời gian bệnh nhân thuộc từng khoa.
        /// - Mỗi bản ghi chuyển khoa là 1 khoảng: [thời gian vào khoa, thời gian vào khoa của bản ghi chuyển khoa tiếp theo].
        ///   Nếu không có bản ghi tiếp theo (khoa cuối cùng / đứt chuỗi PREVIOUS_ID) thì lấy thời gian kết thúc điều trị.
        /// - Mỗi bản ghi điều trị kết hợp là 1 khoảng bổ sung cho khoa phối hợp: [START_TIME, FINISH_TIME].
        ///   Chỉ bổ sung cho khoa đã có bản ghi chuyển khoa để không mở rộng phạm vi kiểm tra so với logic cũ.
        /// KHÔNG gom nhóm/không sửa đổi ListDepartmentTran: bệnh nhân ra khoa rồi vào lại thì mỗi lần nằm là 1 khoảng riêng.
        /// </summary>
        private List<DepartmentTimeRangeADO> BuildDepartmentTimeRangesForCheckOutTime(long treatmentEndTime)
        {
            List<DepartmentTimeRangeADO> ranges = new List<DepartmentTimeRangeADO>();
            try
            {
                foreach (var tran in this.ListDepartmentTran)
                {
                    if (!tran.DEPARTMENT_IN_TIME.HasValue)
                    {
                        //bản ghi chuyển khoa mới yêu cầu, khoa nhận chưa tiếp nhận -> chưa xác định được khoảng thời gian
                        LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " Bo qua ban ghi chuyen khoa chua co thoi gian vao khoa. TRAN_ID=" + tran.ID
                            + "; DEPARTMENT_ID=" + tran.DEPARTMENT_ID + "; REQUEST_TIME=" + (tran.REQUEST_TIME.HasValue ? tran.REQUEST_TIME.Value.ToString() : "null"));
                        continue;
                    }

                    long fromTime = tran.DEPARTMENT_IN_TIME.Value;
                    long toTime = treatmentEndTime;
                    string source = "TRAN_" + tran.ID + "_KET_THUC_DT";

                    //bản ghi chuyển khoa tiếp theo: lấy thời gian vào khoa lớn nhất để khoảng thời gian nằm khoa rộng nhất
                    var nextTrans = this.ListDepartmentTran.Where(o => o.PREVIOUS_ID.HasValue && o.PREVIOUS_ID.Value == tran.ID && o.DEPARTMENT_IN_TIME.HasValue).ToList();
                    if (nextTrans.Count > 0)
                    {
                        toTime = nextTrans.Max(o => o.DEPARTMENT_IN_TIME.Value);
                        source = "TRAN_" + tran.ID + "_NEXT_" + string.Join("|", nextTrans.Select(o => o.ID.ToString()).ToArray());
                    }

                    if (toTime < fromTime)
                    {
                        //dữ liệu chuỗi chuyển khoa bị lệch: bản ghi tiếp theo có thời gian vào khoa nhỏ hơn bản ghi hiện tại
                        LogSystem.Warn(LOG_PREFIX_CHECK_OUT_TIME + " Thoi gian ra khoa nho hon thoi gian vao khoa, lay theo thoi gian ket thuc dieu tri."
                            + " TRAN_ID=" + tran.ID + "; DEPARTMENT_ID=" + tran.DEPARTMENT_ID + "; VAO_KHOA=" + fromTime + "; RA_KHOA=" + toTime);
                        toTime = treatmentEndTime > fromTime ? treatmentEndTime : fromTime;
                        source = "TRAN_" + tran.ID + "_DU_LIEU_LECH";
                    }

                    DepartmentTimeRangeADO range = new DepartmentTimeRangeADO();
                    range.DepartmentId = tran.DEPARTMENT_ID;
                    range.FromTime = fromTime;
                    range.ToTime = toTime;
                    range.Source = source;
                    ranges.Add(range);
                }

                //bổ sung khoảng thời gian điều trị kết hợp
                if (this.ListCoTreatmentCheckTime != null && this.ListCoTreatmentCheckTime.Count > 0)
                {
                    List<long> departmentIdTrans = this.ListDepartmentTran.Select(o => o.DEPARTMENT_ID).Distinct().ToList();
                    foreach (var co in this.ListCoTreatmentCheckTime)
                    {
                        if (!departmentIdTrans.Contains(co.DEPARTMENT_ID))
                        {
                            //khoa chỉ điều trị kết hợp, không có bản ghi chuyển khoa -> vốn không nằm trong phạm vi kiểm tra
                            LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " Bo qua dieu tri ket hop cua khoa khong co ban ghi chuyen khoa. CO_TREATMENT_ID=" + co.ID
                                + "; DEPARTMENT_ID=" + co.DEPARTMENT_ID);
                            continue;
                        }

                        if (!co.START_TIME.HasValue)
                        {
                            LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " Bo qua dieu tri ket hop chua co thoi gian bat dau. CO_TREATMENT_ID=" + co.ID
                                + "; DEPARTMENT_ID=" + co.DEPARTMENT_ID);
                            continue;
                        }

                        DepartmentTimeRangeADO range = new DepartmentTimeRangeADO();
                        range.DepartmentId = co.DEPARTMENT_ID;
                        range.FromTime = co.START_TIME.Value;
                        range.ToTime = co.FINISH_TIME.HasValue ? co.FINISH_TIME.Value : treatmentEndTime;
                        range.Source = "CO_TREATMENT_" + co.ID;
                        if (range.ToTime < range.FromTime)
                        {
                            range.ToTime = range.FromTime;
                        }
                        ranges.Add(range);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return ranges;
        }

        /// <summary>
        /// Ghi log toàn bộ dữ liệu đầu vào và kết quả của check thời gian y lệnh - thời gian ra khoa.
        /// </summary>
        private void LogDataCheckOutTime(long treatmentEndTime, List<DepartmentTimeRangeADO> ranges, Dictionary<long, long> dicLastOutTime,
            List<HIS_SERE_SERV> lstSereServOutTime, List<HIS_SERE_SERV> lstSereServInGap)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(LOG_PREFIX_CHECK_OUT_TIME + " BEGIN. TREATMENT_ID=" + this.treatmentId
                    + "; TREATMENT_CODE=" + (this.currentHisTreatment != null ? this.currentHisTreatment.TREATMENT_CODE : "null")
                    + "; THOI_GIAN_KET_THUC=" + treatmentEndTime
                    + "; SO_CHUYEN_KHOA=" + this.ListDepartmentTran.Count
                    + "; SO_DIEU_TRI_KET_HOP=" + (this.ListCoTreatmentCheckTime == null ? 0 : this.ListCoTreatmentCheckTime.Count)
                    + "; SO_DICH_VU_KIEM_TRA=" + this.SereServCheck.Count
                    + "; SO_DICH_VU_CANH_BAO=" + lstSereServOutTime.Count
                    + "; SO_DICH_VU_KHOANG_TRONG=" + lstSereServInGap.Count
                    + " (so Y LENH tuong ung xem 2 dong log ben duoi, 1 y lenh gom nhieu dich vu)");

                sb.AppendLine("--- DS chuyen khoa (TRAN_ID | DEPARTMENT_ID | TEN_KHOA | PREVIOUS_ID | VAO_KHOA) ---");
                foreach (var tran in this.ListDepartmentTran.OrderBy(o => o.DEPARTMENT_IN_TIME ?? long.MaxValue).ThenBy(o => o.ID))
                {
                    sb.AppendLine(string.Format("{0} | {1} | {2} | {3} | {4}", tran.ID, tran.DEPARTMENT_ID, this.GetDepartmentNameForLog(tran.DEPARTMENT_ID),
                        tran.PREVIOUS_ID.HasValue ? tran.PREVIOUS_ID.Value.ToString() : "null",
                        tran.DEPARTMENT_IN_TIME.HasValue ? tran.DEPARTMENT_IN_TIME.Value.ToString() : "null"));
                }

                sb.AppendLine("--- DS dieu tri ket hop (CO_ID | DEPARTMENT_ID | TEN_KHOA | BAT_DAU | KET_THUC | IS_ACTIVE) ---");
                if (this.ListCoTreatmentCheckTime != null)
                {
                    foreach (var co in this.ListCoTreatmentCheckTime.OrderBy(o => o.START_TIME ?? long.MaxValue).ThenBy(o => o.ID))
                    {
                        sb.AppendLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5}", co.ID, co.DEPARTMENT_ID, this.GetDepartmentNameForLog(co.DEPARTMENT_ID),
                            co.START_TIME.HasValue ? co.START_TIME.Value.ToString() : "null",
                            co.FINISH_TIME.HasValue ? co.FINISH_TIME.Value.ToString() : "null",
                            co.IS_ACTIVE.HasValue ? co.IS_ACTIVE.Value.ToString() : "null"));
                    }
                }

                sb.AppendLine("--- Khoang thoi gian theo khoa (DEPARTMENT_ID | TEN_KHOA | TU | DEN | NGUON) ---");
                foreach (var range in ranges.OrderBy(o => o.DepartmentId).ThenBy(o => o.FromTime))
                {
                    sb.AppendLine(string.Format("{0} | {1} | {2} | {3} | {4}", range.DepartmentId, this.GetDepartmentNameForLog(range.DepartmentId),
                        range.FromTime, range.ToTime, range.Source));
                }

                sb.AppendLine("--- Thoi gian ra khoa cuoi cung theo khoa (DEPARTMENT_ID | TEN_KHOA | RA_KHOA_CUOI) ---");
                foreach (var key in dicLastOutTime.Keys.OrderBy(o => o))
                {
                    sb.AppendLine(string.Format("{0} | {1} | {2}", key, this.GetDepartmentNameForLog(key), dicLastOutTime[key]));
                }

                LogSystem.Info(sb.ToString());

                this.LogSereServCheckOutTime("Y lenh CANH BAO (sau thoi gian ra khoa cuoi cung cua khoa chi dinh)", lstSereServOutTime, dicLastOutTime);
                this.LogSereServCheckOutTime("Y lenh nam ngoai khoang nam khoa nhung truoc thoi gian ra khoa cuoi cung (KHONG canh bao)", lstSereServInGap, dicLastOutTime);

                LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " END. TREATMENT_ID=" + this.treatmentId);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ghi log chi tiết từng y lệnh, chia lô 100 dòng để tránh 1 bản ghi log quá lớn.
        /// </summary>
        private void LogSereServCheckOutTime(string title, List<HIS_SERE_SERV> datas, Dictionary<long, long> dicLastOutTime)
        {
            try
            {
                if (datas == null || datas.Count == 0)
                {
                    LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " " + title + ": 0 y lenh.");
                    return;
                }

                var groups = datas.GroupBy(o => new { o.TDL_SERVICE_REQ_CODE, o.TDL_REQUEST_DEPARTMENT_ID, o.TDL_INTRUCTION_TIME })
                    .Select(g => g.Key).OrderBy(o => o.TDL_INTRUCTION_TIME).ThenBy(o => o.TDL_SERVICE_REQ_CODE).ToList();

                LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " " + title + ": " + groups.Count + " y lenh (MA_YL | KHOA_CHI_DINH | TEN_KHOA | TG_Y_LENH | RA_KHOA_CUOI | LECH).");

                StringBuilder sb = new StringBuilder();
                int index = 0;
                foreach (var g in groups)
                {
                    long lastOutTime = 0;
                    dicLastOutTime.TryGetValue(g.TDL_REQUEST_DEPARTMENT_ID, out lastOutTime);
                    sb.AppendLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5}", g.TDL_SERVICE_REQ_CODE, g.TDL_REQUEST_DEPARTMENT_ID,
                        this.GetDepartmentNameForLog(g.TDL_REQUEST_DEPARTMENT_ID), g.TDL_INTRUCTION_TIME, lastOutTime,
                        g.TDL_INTRUCTION_TIME > lastOutTime ? "SAU_RA_KHOA" : "TRONG_KHOANG_TRONG"));
                    index++;
                    if (index % 100 == 0)
                    {
                        LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " " + title + " (den dong " + index + "):" + Environment.NewLine + sb.ToString());
                        sb.Clear();
                    }
                }
                if (sb.Length > 0)
                {
                    LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " " + title + " (den dong " + index + "):" + Environment.NewLine + sb.ToString());
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private bool Check_INTRUCTION_TIME_and_DEPARTMENT_IN_TIME_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }

                if (this.SereServCheck == null || this.SereServCheck.Count == 0 || this.ListDepartmentTran == null || this.ListDepartmentTran.Count == 0)
                {
                    LogSystem.Info(LOG_PREFIX_CHECK_OUT_TIME + " Bo qua check. TREATMENT_ID=" + this.treatmentId
                        + "; SereServCheck=" + (this.SereServCheck == null ? "null" : this.SereServCheck.Count.ToString())
                        + "; ListDepartmentTran=" + (this.ListDepartmentTran == null ? "null" : this.ListDepartmentTran.Count.ToString()));
                    return valid;
                }

                long treatmentEndTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;

                //chưa nhập thời gian kết thúc điều trị thì không xác định được thời gian ra khoa của khoa cuối cùng -> mọi y lệnh sẽ bị cảnh báo sai
                if (dtEndTime.EditValue == null || treatmentEndTime <= 0
                    || (this.currentHisTreatment != null && treatmentEndTime < this.currentHisTreatment.IN_TIME))
                {
                    LogSystem.Warn(LOG_PREFIX_CHECK_OUT_TIME + " Bo qua check vi thoi gian ket thuc dieu tri khong hop le. TREATMENT_ID=" + this.treatmentId
                        + "; THOI_GIAN_KET_THUC=" + treatmentEndTime
                        + "; TREATMENT_IN_TIME=" + (this.currentHisTreatment != null ? this.currentHisTreatment.IN_TIME.ToString() : "null"));
                    return valid;
                }

                this.LoadCoTreatmentForCheckOutTime();

                //các khoảng thời gian bệnh nhân thuộc từng khoa (nhiều lần vào/ra cùng 1 khoa là nhiều khoảng riêng biệt)
                List<DepartmentTimeRangeADO> ranges = this.BuildDepartmentTimeRangesForCheckOutTime(treatmentEndTime);

                //thời gian ra khoa lần cuối của từng khoa
                Dictionary<long, long> dicLastOutTime = new Dictionary<long, long>();
                foreach (var range in ranges)
                {
                    if (!dicLastOutTime.ContainsKey(range.DepartmentId) || dicLastOutTime[range.DepartmentId] < range.ToTime)
                    {
                        dicLastOutTime[range.DepartmentId] = range.ToTime;
                    }
                }

                //y lệnh có thời gian chỉ định lớn hơn thời gian ra khoa lần cuối của khoa chỉ định
                List<HIS_SERE_SERV> lstSereServOutTime = new List<HIS_SERE_SERV>();
                //y lệnh không nằm trong khoảng nào nhưng vẫn trước thời gian ra khoa lần cuối -> chỉ ghi log, không cảnh báo
                List<HIS_SERE_SERV> lstSereServInGap = new List<HIS_SERE_SERV>();

                foreach (var ss in this.SereServCheck)
                {
                    if (ss.AMOUNT == 0)
                    {
                        continue;
                    }

                    long lastOutTime = 0;
                    if (!dicLastOutTime.TryGetValue(ss.TDL_REQUEST_DEPARTMENT_ID, out lastOutTime))
                    {
                        //khoa chỉ định không có bản ghi chuyển khoa nào -> giữ nguyên logic cũ là không kiểm tra
                        continue;
                    }

                    if (ss.TDL_INTRUCTION_TIME > lastOutTime)
                    {
                        lstSereServOutTime.Add(ss);
                    }
                    else if (!ranges.Exists(o => o.DepartmentId == ss.TDL_REQUEST_DEPARTMENT_ID
                        && o.FromTime <= ss.TDL_INTRUCTION_TIME && ss.TDL_INTRUCTION_TIME <= o.ToTime))
                    {
                        lstSereServInGap.Add(ss);
                    }
                }

                this.LogDataCheckOutTime(treatmentEndTime, ranges, dicLastOutTime, lstSereServOutTime, lstSereServInGap);

                //tồn tại dịch vụ có thời gian chỉ định lớn hơn thời gian ra khoa
                if (lstSereServOutTime.Count > 0)
                {
                    string codes = string.Join(", ", lstSereServOutTime.Select(s => s.TDL_SERVICE_REQ_CODE).Distinct().OrderBy(o => o));

                    if (validationDataType == ValidationDataType.PopupMessage)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.YLenhCoThoiGianChiDinhLonHonThoiGianRaKhoa, codes) + " Bạn có muốn kết thúc điều trị không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            return false;
                        }
                    }
                    else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                    {
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = true;
                        warning.Description = String.Format(ResourceMessage.YLenhCoThoiGianChiDinhLonHonThoiGianRaKhoa, codes);
                        listWarningADO.Add(warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckSameHein_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.CheckFinishTimeCFG.CHECK_SAME_HEIN) == "1" || HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.CheckFinishTimeCFG.CHECK_SAME_HEIN) == "2")
                {
                    bool checkSameHein = false;
                    CommonParam param = new CommonParam();

                    HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                    patientTypeAlterFilter.TREATMENT_ID = currentHisTreatment.ID;

                    var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                    if (patientTypeAlter != null && patientTypeAlter.Count >= 2)
                    {
                        foreach (var item in patientTypeAlter)
                        {
                            var sameHein = patientTypeAlter.Where(o => o.HEIN_CARD_NUMBER == item.HEIN_CARD_NUMBER).ToList();
                            if (sameHein != null && sameHein.Count >= 2)
                            {
                                var checkHeinOrg = sameHein.Select(o => o.HEIN_MEDI_ORG_CODE).Distinct().ToList();
                                if (checkHeinOrg.Count > 1)
                                {
                                    //Mã cskcb khác nhau
                                    checkSameHein = true;
                                    break;
                                }
                                else
                                {
                                    var checkRightRoute = sameHein.Select(o => o.RIGHT_ROUTE_CODE).Distinct().ToList();
                                    if (checkRightRoute.Count == 1)
                                    {
                                        //Đúng tuyến và lý do đúng tuyến khác nhau
                                        if (checkRightRoute.FirstOrDefault() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                                        {
                                            var checkRightRouteType = sameHein.Select(o => o.RIGHT_ROUTE_TYPE_CODE).Distinct().ToList();
                                            if (checkRightRouteType.Count > 1)
                                            {
                                                checkSameHein = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Trái tuyến
                                        checkSameHein = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Info("Save treatmentFinish 4");

                    //issue 13722
                    if (checkSameHein && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.CheckFinishTimeCFG.CHECK_SAME_HEIN) == "1")
                    {
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BenhNhanCoThongTinBhytChuaDung, ResourceMessage.ThongBao);
                            return false;
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = false;
                            warning.Description = ResourceMessage.BenhNhanCoThongTinBhytChuaDung;
                            listWarningADO.Add(warning);
                        }
                    }
                    else if (checkSameHein && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.CheckFinishTimeCFG.CHECK_SAME_HEIN) == "2")
                    {
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.BanCoMuonTiepTuc, ResourceMessage.BenhNhanCoThongTinBhytChuaDung), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                return false;
                            }
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = true;
                            warning.Description = ResourceMessage.BenhNhanCoThongTinBhytChuaDung;
                            listWarningADO.Add(warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckRation_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (!CheckFinishTimeCFG.isWarningApproveRation || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    return true;
                }

                CommonParam param = new CommonParam();
                HisTreatmentRationNotApproveFilter rationFilter = new HisTreatmentRationNotApproveFilter();
                rationFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                List<HisTreatmentRationNotApproveSDO> notApproves = new BackendAdapter(param).Get<List<HisTreatmentRationNotApproveSDO>>("api/HisTreatment/GetRationNotApprove", ApiConsumers.MosConsumer, rationFilter, param);

                if (notApproves != null && notApproves.Count > 0)
                {
                    var Groups = notApproves.GroupBy(g => g.RationSumCode).ToList();
                    List<string> msgs = new List<string>();

                    string notHasRationSum = "";

                    foreach (var item in Groups)
                    {
                        if (!String.IsNullOrWhiteSpace(item.Key))
                        {
                            string maYLenhs = string.Join(",", item.Select(s => s.ServiceReqCode).ToList());
                            msgs.Add(String.Format(ResourceMessage.MaPhieuTongHopSuatAnMaYLenh, item.Key, maYLenhs));
                        }
                        else
                        {
                            notHasRationSum = string.Join(",", item.Select(s => s.ServiceReqCode).ToList());
                        }
                    }

                    //De cho cau thong bao chu tong hop nam cuoi cung
                    if (!String.IsNullOrWhiteSpace(notHasRationSum))
                    {
                        msgs.Add(String.Format(ResourceMessage.ChuaTongHopSuatAnMaYLenh, notHasRationSum));
                    }

                    string messages = String.Join(".\n", msgs);

                    if (validationDataType == ValidationDataType.PopupMessage)
                    {
                        if (XtraMessageBox.Show(String.Format("\r\n" + ResourceMessage.BanCoMuonTiepTuc, String.Format(ResourceMessage.YLenhChuaTongHopHoacDuyetSuatAn, messages)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return false;
                        }
                    }
                    else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                    {
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = true;
                        warning.Description = String.Format(ResourceMessage.YLenhChuaTongHopHoacDuyetSuatAn, messages);
                        listWarningADO.Add(warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool Check_UNSIGN_DOC_FINISH_OPTION_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (this.currentHisTreatment != null)
                {
                    var treatmentType = this.hisTreatmentTypes.FirstOrDefault(o => o.ID == this.currentHisTreatment.TDL_TREATMENT_TYPE_ID);
                    //Hồ sơ có diện điều trị được khai báo cảnh báo(UNSIGN_DOC_FINISH_OPTION = 1) hoặc chặn(UNSIGN_DOC_FINISH_OPTION = 2) khi có văn bản chưa hoàn thiện ký
                    if (treatmentType != null && (treatmentType.UNSIGN_DOC_FINISH_OPTION == 1
                                                || treatmentType.UNSIGN_DOC_FINISH_OPTION == 2))
                    {
                        if (CheckEmrDocumentData(treatmentType, validationDataType, ref listWarningADO))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckDHST_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                saveDHST = null;

                if (TinhTuoi(this.currentHisTreatment.TDL_PATIENT_DOB, dtEndTime.DateTime) <= 1 && !CheckDHST(this.currentHisTreatment.ID, ref saveDHST))
                {
                    WaitingManager.Hide();
                    if (validationDataType == ValidationDataType.PopupMessage)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BenhNhanChuaCoDHST, ResourceMessage.ThongBao);
                        btnDHST.Enabled = true;
                        return false;
                    }
                    else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                    {
                        btnDHST.Enabled = true;
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = false;
                        warning.Description = ResourceMessage.BenhNhanChuaCoDHST;
                        listWarningADO.Add(warning);
                    }
                }

                if (TinhTuoi(this.currentHisTreatment.TDL_PATIENT_DOB, dtEndTime.DateTime) <= 1 && CheckDHST(this.currentHisTreatment.ID, ref saveDHST) && saveDHST != null && !saveDHST.WEIGHT.HasValue)
                {
                    WaitingManager.Hide();
                    if (validationDataType == ValidationDataType.PopupMessage)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BenhNhanThieuCanNang, ResourceMessage.ThongBao);
                        btnDHST.Enabled = true;
                        return false;
                    }
                    else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                    {
                        btnDHST.Enabled = true;
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = false;
                        warning.Description = ResourceMessage.BenhNhanThieuCanNang;
                        listWarningADO.Add(warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckWarnNotRequiredCompleteHasNoSample(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                string serviceReqCode = "";

                if (ConfigKey.WarnNotRequiredCompleteHasNoSample == "1")
                {
                    HisServiceReqFilter srFilter = new HisServiceReqFilter();

                    srFilter.TREATMENT_ID = currentHisTreatment.ID;
                    srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN;
                    srFilter.IS_NOT_REQUIRED_COMPLETE = true;
                    srFilter.ORDER_DIRECTION = "DESC";
                    srFilter.ORDER_FIELD = "CREATE_TIME";

                    var examServiceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);

                    if (examServiceReqs != null && examServiceReqs.Count > 0)
                    {
                        var serviceReqs = examServiceReqs.Where(o => o.SAMPLE_TIME == null).ToList();
                        if (serviceReqs != null && serviceReqs.Count > 0)
                        {
                            serviceReqCode = string.Join(", ", serviceReqs.Select(o => o.SERVICE_REQ_CODE));

                            if (validationDataType == ValidationDataType.PopupMessage)
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Y lệnh {0} chưa có thông tin lấy mẫu.Bạn có muốn tiếp tục?", serviceReqCode),
                               "Thông báo",
                              MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    return false;
                                }
                            }
                            else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                            {
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = true;
                                warning.Description = String.Format(ResourceMessage.YLenhChuaCoThongTinLayMau, serviceReqCode);
                                listWarningADO.Add(warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckIsCheckServiceFollowWhenOut()
        {
            bool valid = true;
            try
            {
                if (ConfigKey.IsCheckServiceFollowWhenOut == "1" && this.currentHisTreatment.TDL_PATIENT_TYPE_ID == Config.ConfigKey.PatientTypeId__BHYT)
                {
                    CommonParam param = new CommonParam();
                    var result = new BackendAdapter(param).Post<bool>("api/HisTreatment/CheckServiceFollow", ApiConsumers.MosConsumer, currentHisTreatment.ID, param);
                    
                    if (result == false)
                    {
                        if (XtraMessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, param.GetMessage()), ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
        private bool CheckBedEndForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (ConfigKey.CheckBedEnd == "1" && this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    //danh sach vao buong benh
                    HisTreatmentBedRoomViewFilter tbrFilter = new HisTreatmentBedRoomViewFilter();
                    tbrFilter.TREATMENT_ID = treatmentId;
                    var TreatmentBerRoom = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, tbrFilter, null);

                    //danh sach y lenh giuong
                    HisServiceReqFilter srFilter = new HisServiceReqFilter();
                    srFilter.TREATMENT_ID = treatmentId;
                    srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G;
                    List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);

                    //danh sach lich su giuong theo TREATMENT_ID
                    HisBedLogViewFilter filter = new HisBedLogViewFilter();
                    filter.TREATMENT_ID = currentHisTreatment.ID;
                    BedLogs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, filter, null);

                    if (serviceReqs != null && serviceReqs.Count > 0 && TreatmentBerRoom != null && TreatmentBerRoom.Count > 0 && BedLogs != null && BedLogs.Count > 0)
                    {
                        var bedRoomDict = TreatmentBerRoom.ToDictionary(b => b.ID, b => b);
                        var bedLogDict = BedLogs.ToDictionary(b => b.ID, b => b);
                        long endTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                        var listServiceReqCode = new List<string>();

                        foreach (var item in serviceReqs)
                        {
                            if (item.BED_LOG_ID.HasValue && bedLogDict.ContainsKey(item.BED_LOG_ID.Value))
                            {
                                var bedLog = bedLogDict[item.BED_LOG_ID.Value];
                                if (bedLog.TREATMENT_BED_ROOM_ID != 0 && bedRoomDict.ContainsKey(bedLog.TREATMENT_BED_ROOM_ID))
                                {
                                    if (bedLog.FINISH_TIME.HasValue && bedLog.FINISH_TIME.Value > endTime)
                                    {
                                        string finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bedLog.FINISH_TIME.Value)?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                                        listServiceReqCode.Add($"Y lệnh {item.SERVICE_REQ_CODE} có thời gian {finishTimeStr}");
                                    }
                                }
                            }
                        }

                        if (listServiceReqCode.Count > 0)
                        {
                            string endTimeStr = dtEndTime.DateTime.ToString("dd/MM/yyyy HH:mm:ss");
                            string message = $"Tồn tại y lệnh giường có thời gian kết thúc lớn hơn thời gian ra viện {endTimeStr}{Environment.NewLine}";
                            message += string.Join(Environment.NewLine, listServiceReqCode);

                            if (validationDataType == ValidationDataType.PopupMessage)
                            {
                                var result = DevExpress.XtraEditors.XtraMessageBox.Show(message, "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (result != DialogResult.Yes)
                                {
                                    return false;
                                }
                            }
                            else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                            {
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = false;
                                warning.Description = message;
                                listWarningADO.Add(warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
        private bool CheckPrescriptionForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (ConfigKey.CheckPrescriptionEnd == "1" && this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    //danh sach dich vu thuoc vat tu
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.TREATMENT_ID = treatmentId;
                    ssFilter.TDL_SERVICE_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT };
                    var sereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                    List<long> listServiceReqId = sereServs.Select(p => p.SERVICE_REQ_ID ?? 0).Distinct().ToList();

                    if (listServiceReqId.Count > 0)
                    {
                        //danh sach y lenh thuoc/vat tu 
                        HisServiceReqFilter srPreFilter = new HisServiceReqFilter();
                        srPreFilter.IDs = listServiceReqId;
                        List<HIS_SERVICE_REQ> serviceReqsPre = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srPreFilter, null);

                        if (serviceReqsPre != null && serviceReqsPre.Count > 0)
                        {
                            long endTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                            var listServiceReqCode = new List<string>();
                            foreach (var item in serviceReqsPre)
                            {
                                if (item.FINISH_TIME.HasValue && item.FINISH_TIME > endTime)
                                {
                                    string finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.FINISH_TIME.Value)?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                                    listServiceReqCode.Add($"Y lệnh {item.SERVICE_REQ_CODE} có thời gian {finishTimeStr}");
                                }
                            }

                            if (listServiceReqCode.Count > 0)
                            {
                                string endTimeStr = dtEndTime.DateTime.ToString("dd/MM/yyyy HH:mm:ss");
                                string message = $"Tồn tại y lệnh thuốc/vật tư có thời gian kết thúc lớn hơn thời gian ra viện {endTimeStr}{Environment.NewLine}";
                                message += string.Join(Environment.NewLine, listServiceReqCode);


                                if (validationDataType == ValidationDataType.PopupMessage)
                                {
                                    var result = DevExpress.XtraEditors.XtraMessageBox.Show(message, "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (result != DialogResult.Yes)
                                    {
                                        return false;
                                    }
                                }
                                else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                                {
                                    WarningADO warning = new WarningADO();
                                    warning.IsSkippable = false;
                                    warning.Description = message;
                                    listWarningADO.Add(warning);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckUnassignTrackingServiceReq_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "1"
                    || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "2" 
                    || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3")
                {
                    List<V_HIS_SERVICE_REQ_12> listServiceReq = new List<V_HIS_SERVICE_REQ_12>();

                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqView12Filter filter = new MOS.Filter.HisServiceReqView12Filter();
                    filter.TREATMENT_ID = this.treatmentId;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "CREATE_TIME";
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_12>>("api/HisServiceReq/GetView12", ApiConsumers.MosConsumer, filter, param);

                    if ((Config.CheckFinishTimeCFG.IsNotShowOutMediAndMate == "1" || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3") && apiResult != null)
                    {
                        listServiceReq = apiResult.Where(o => o.TRACKING_ID == null
                                                    && o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                                                    && o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN
                                                    && o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G).ToList();
                        if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3")
                        {
                            List<V_HIS_SERVICE_REQ_12> lstTmp = new List<V_HIS_SERVICE_REQ_12>();
                            if (SereServTreatment != null && SereServTreatment.Count > 0)
                            {
                                foreach (var item in listServiceReq)
                                {
                                    if (item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK || item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT || item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                                    {
                                        if (SereServTreatment.Exists(o => o.SERVICE_REQ_ID == item.ID && o.EXP_MEST_MEDICINE_ID != null))
                                        {
                                            lstTmp.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        lstTmp.Add(item);
                                    }
                                }
                            }
                            listServiceReq = lstTmp;
                        }
                    }
                    else if (apiResult != null)
                    {
                        listServiceReq = apiResult.Where(o => o.TRACKING_ID == null
                                                    && o.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                                                    && o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN
                                                    && o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G
                                                    && ((o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && o.EXP_MEST_ID != null) || o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)).ToList();
                    }
                    if (listServiceReq != null && listServiceReq.Count > 0)
                    {
                        string message = "";
                        var groupServiceReq = listServiceReq.GroupBy(o => o.REQUEST_DEPARTMENT_NAME).ToList();
                        foreach (var group in groupServiceReq)
                        {
                            string str = "";
                            foreach (var item in group)
                            {
                                str += item.SERVICE_REQ_CODE + "; ";
                            }
                            if (!String.IsNullOrEmpty(str) && str.Length > 2)
                                str = str.Remove(str.Length - 2, 2);
                            str += " (khoa chỉ định: " + group.Key + ")";
                            str += ",";
                            message += str;
                        }
                        if (!String.IsNullOrEmpty(message) && message.Length > 1)
                        {
                            message = message.Remove(message.Length - 1, 1);
                            message = "Y lệnh " + message + " chưa gắn tờ điều trị.";
                        }
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "1")
                            {
                                XtraMessageBox.Show(message, ResourceMessage.ThongBao);
                                return false;
                            }
                            else if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "2" || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3")
                            {
                                if (XtraMessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, message), ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != System.Windows.Forms.DialogResult.Yes)
                                {
                                    return false;
                                }
                            }
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "1")
                            {
                                btnDHST.Enabled = true;
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = false;
                                warning.Description = message;
                                listWarningADO.Add(warning);
                            }
                            else if (Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "2"
                                || Config.CheckFinishTimeCFG.WarningOptionInCaseOfUnassignTrackingServiceReq == "3")
                            {
                                btnDHST.Enabled = true;
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = true;
                                warning.Description = message;
                                listWarningADO.Add(warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool CheckUsedDrugQuantityMismatch_ForSave(ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool valid = true;
            try
            {
                if (validationDataType == ValidationDataType.PopupMessage && this._isSkipWarningForSave == true)
                {
                    return valid;
                }
                if (ConfigKey.CheckUsedDrugQuantityMismatch == "2")
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.TREATMENT_ID = treatmentId;
                    ssFilter.TDL_SERVICE_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC };
                    var sereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                    if (sereServs != null && sereServs.Count > 0)
                    {
                        var validSereServs = sereServs.Where(s => s.IS_DELETE != 1 && s.EXP_MEST_MEDICINE_ID.HasValue).ToList();

                        if (validSereServs.Count > 0)
                        {
                            List<long> expMestMedicineIds = validSereServs.Select(s => s.EXP_MEST_MEDICINE_ID.Value).Distinct().ToList();

                            HisExpMedimateUsedFilter usedFilter = new HisExpMedimateUsedFilter();
                            usedFilter.TDL_TREATMENT_ID = treatmentId;
                            usedFilter.EXP_MEST_MEDICINE_IDs = expMestMedicineIds;
                            var expUsedList = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEDIMATE_USED>>("api/HisExpMedimateUsed/Get", ApiConsumers.MosConsumer, usedFilter, null);

                            if (expUsedList != null && expUsedList.Count > 0)
                            {
                                var usedGrouped = expUsedList
                                    .Where(u => u.EXP_MEST_MEDICINE_ID.HasValue)
                                    .GroupBy(u => u.EXP_MEST_MEDICINE_ID.Value)
                                    .ToDictionary(g => g.Key, g => g.Sum(u => u.AMOUNT) ?? 0);

                                List<string> warningMessages = new List<string>();
                                foreach (var sereServ in validSereServs)
                                {
                                    decimal usedAmount = 0;
                                    if (usedGrouped.ContainsKey(sereServ.EXP_MEST_MEDICINE_ID.Value))
                                    {
                                        usedAmount = usedGrouped[sereServ.EXP_MEST_MEDICINE_ID.Value];
                                    }

                                    if (usedAmount < sereServ.AMOUNT)
                                    {
                                        string msg = String.Format("Tổng số lượng thuốc bệnh nhân đã dùng {0} nhỏ hơn tổng số lượng thuốc đã kê {1} của y lệnh {2}",
                                            usedAmount,
                                            sereServ.AMOUNT,
                                            sereServ.TDL_SERVICE_REQ_CODE);
                                        warningMessages.Add(msg);
                                    }
                                }

                                if (warningMessages.Count > 0)
                                {
                                    string fullMessage = string.Join(Environment.NewLine, warningMessages) + Environment.NewLine + "Bạn có muốn tiếp tục không?";

                                    if (validationDataType == ValidationDataType.PopupMessage)
                                    {
                                        var result = DevExpress.XtraEditors.XtraMessageBox.Show(fullMessage, "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                        if (result != DialogResult.Yes)
                                        {
                                            return false;
                                        }
                                    }
                                    else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                                    {
                                        WarningADO warning = new WarningADO();
                                        warning.IsSkippable = true;
                                        warning.Description = fullMessage;
                                        listWarningADO.Add(warning);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private bool Check_IsAllowTreatmentFinishDepartmentIsActiveFee_ForSave()
        {
            bool valid = true;
            try
            {
                if (this.currentHisTreatment != null)
                {
                    if (Config.ConfigKey.IsAllowTreatmentFinishDepartmentIsActiveFee == "1")
                    {
                        CommonParam param = new CommonParam();
                        var departments = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == Constant.IS_TRUE).ToList();
                        if (departments != null && departments.Count > 0)
                        {
                            List<long> departmentIds = departments.Select(o => o.ID).ToList();
                            HisDepartmentTranViewFilter filter = new HisDepartmentTranViewFilter();
                            filter.TREATMENT_ID = this.currentHisTreatment.ID;
                            filter.DEPARTMENT_IDs = departmentIds;
                            filter.IS_ACTIVE = Constant.IS_TRUE;
                            var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, filter, param);
                            if (departmentTrans != null && departmentTrans.Count > 0)
                            {
                                List<string> departmentNames = departmentTrans.Select(o => o.DEPARTMENT_NAME).Distinct().ToList();
                                XtraMessageBox.Show(String.Format("Hồ sơ {0} có khoa {1} chưa được khóa chi phí. Không cho phép kết thúc điều trị", this.currentHisTreatment.TREATMENT_CODE, string.Join(",", departmentNames), ResourceMessage.ThongBao));
                                if (departmentNames != null && departmentNames.Count > 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
