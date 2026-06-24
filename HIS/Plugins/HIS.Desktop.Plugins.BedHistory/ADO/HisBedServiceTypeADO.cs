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
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedHistory.ADO
{
    class HisBedServiceTypeADO : MOS.EFMODEL.DataModels.V_HIS_BED_LOG, IDXDataErrorInfo
    {
        public decimal AMOUNT { get; set; }
        //public string PATIENT_TYPE_CODE { get; set; }
        //public long PATIENT_TYPE_ID { get; set; }
        //public string PATIENT_TYPE_NAME { get; set; }
        public bool? IsExpend { get; set; }
        public bool? IsKHBHYT { get; set; }
        public bool? IsOutKtcFee { get; set; }
        public decimal TotalPrice { get; set; }
        public long? AmmoutNamGhep { get; set; }
        public string BED_SERVICE_TYPE_NAME { get; set; }
        //public long? PRIMARY_PATIENT_TYPE_ID { get; set; }
        //public string PRIMARY_PATIENT_TYPE_NAME { get; set; }
        public long? BILL_PATIENT_TYPE_ID { get; set; }
        public DateTime IntructionTime { get; set; }
        // Thời gian dự trù — nullable, default empty (null). Mapped to UseTime in HisBedServiceSDO when has value.
        public DateTime? UseTime { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public long? OTHER_PAY_SOURCE_ID { get; set; }
        public bool HasConfigOtherSourcePay { get; set; }
        public bool IsContainAppliedPatientType { get; set; }
        public bool IsBedStretcher { get; set; }
        public bool IsSplitDayOrResult { get; set; }

        #region IDXDataErrorInfo
        // Cảnh báo (tam giác vàng) khi Thời gian chỉ định rơi vào Thứ 7/Chủ nhật.
        // Grid tự hiển thị trên TẤT CẢ dòng vi phạm, tự re-check khi sửa. KHÔNG chặn lưu.
        // LƯU Ý: GetPropertyError được DevExpress gọi RẤT NHIỀU lần (mỗi cell, mỗi repaint).
        // -> Cache sẵn message (1 lần) để tránh đọc ResourceManager lặp lại gây lag/đơ.
        private static string _msgSaturday;
        private static string _msgSunday;

        // Cờ bật cột "Thời gian thực hiện" theo config HIS.Desktop.Plugins.BedHistory.UseTime.
        // Form gán giá trị khi Load. Chỉ khi = true mới hiện cảnh báo cuối tuần (gắn với ô Thời gian thực hiện).
        public static bool IsUseTimeConfigOn { get; set; }

        private static string GetWeekendWarningMessage(DayOfWeek dow)
        {
            if (dow == DayOfWeek.Saturday)
            {
                if (_msgSaturday == null)
                    _msgSaturday = String.Format(HIS.Desktop.Plugins.BedHistory.ResourceMessage.CanhBaoNgayChiDinhCuoiTuan, "Thứ 7");
                return _msgSaturday;
            }
            if (_msgSunday == null)
                _msgSunday = String.Format(HIS.Desktop.Plugins.BedHistory.ResourceMessage.CanhBaoNgayChiDinhCuoiTuan, "Chủ nhật");
            return _msgSunday;
        }

        public void GetPropertyError(string propertyName, ErrorInfo info)
        {
            try
            {
                // Chỉ cảnh báo cuối tuần khi config HIS.Desktop.Plugins.BedHistory.UseTime = 1
                if (!IsUseTimeConfigOn) return;
                if (propertyName == "IntructionTime")
                {
                    DayOfWeek dow = this.IntructionTime.DayOfWeek;
                    if (dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday)
                    {
                        info.ErrorType = ErrorType.Warning;
                        info.ErrorText = GetWeekendWarningMessage(dow);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void GetError(ErrorInfo info)
        {
        }
        #endregion
    }
}
