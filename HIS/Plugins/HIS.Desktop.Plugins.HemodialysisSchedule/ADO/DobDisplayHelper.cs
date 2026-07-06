/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System;

namespace HIS.Desktop.Plugins.HemodialysisSchedule.ADO
{
    /// <summary>
    /// Format ngày sinh theo cờ IS_HAS_NOT_DAY_DOB của từng bản ghi:
    /// = 1 → chỉ hiển thị năm; ngược lại → đủ ngày/tháng/năm.
    /// Tách riêng để cả 2 ADO (lịch + bệnh nhân) dùng chung, tránh lặp logic.
    /// </summary>
    internal static class DobDisplayHelper
    {
        public static string Format(long? dob, short? isHasNotDayDob)
        {
            try
            {
                long num = dob ?? 0;
                if (num <= 0) return "";
                if ((isHasNotDayDob ?? 0) == 1)
                {
                    string s = num.ToString();
                    return s.Length >= 4 ? s.Substring(0, 4) : s;
                }
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(num) ?? "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}
