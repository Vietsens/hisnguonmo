/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// vCong60263: quy tắc lịch gửi XML 130 theo mốc giờ.
    /// Thuần logic, không đụng giao diện, không gọi API — để dễ đọc và dễ kiểm chứng.
    ///
    /// Quy ước:
    ///  - Mốc giờ: chuỗi "HH:mm" phân cách bởi ';'  (VD "10:00;16:00")
    ///  - Ngày áp dụng: số thứ tự phân cách bởi ',' với 1 = Thứ 2 ... 7 = Chủ nhật.
    ///    Rỗng/null = áp dụng TẤT CẢ các ngày.
    /// </summary>
    internal static class Xml130Schedule
    {
        internal const char TIME_SEPARATOR = ';';
        internal const char DAY_SEPARATOR = ',';
        internal const int MAX_LOOK_BACK_DAYS = 31;
        internal const int MIN_LOOK_BACK_DAYS = 0;
        //QT08: không khai thì quét lùi 1 ngày - đủ phủ hồ sơ khóa viện phí cuối ngày hôm trước.
        internal const int DEFAULT_LOOK_BACK_DAYS = 1;

        /// <summary>Đọc danh sách mốc giờ, bỏ mốc trùng, sắp xếp tăng dần. Mốc sai định dạng bị bỏ qua.</summary>
        internal static List<TimeSpan> ParseTimes(string raw)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            if (String.IsNullOrWhiteSpace(raw))
            {
                return result;
            }
            foreach (string part in raw.Split(TIME_SEPARATOR))
            {
                TimeSpan slot;
                if (TryParseTime(part, out slot) && !result.Contains(slot))
                {
                    result.Add(slot);
                }
            }
            result.Sort();
            return result;
        }

        /// <summary>Nhận "9:5", "09:05", " 09:05 " đều ra 09:05. Ngoài khoảng 00:00-23:59 thì trả false.</summary>
        internal static bool TryParseTime(string value, out TimeSpan slot)
        {
            slot = TimeSpan.Zero;
            if (String.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string[] parts = value.Trim().Split(':');
            if (parts.Length != 2)
            {
                return false;
            }
            int hour;
            int minute;
            if (!Int32.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out hour)
                || !Int32.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out minute))
            {
                return false;
            }
            if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
            {
                return false;
            }
            slot = new TimeSpan(hour, minute, 0);
            return true;
        }

        internal static string FormatTimes(IEnumerable<TimeSpan> slots)
        {
            if (slots == null)
            {
                return String.Empty;
            }
            return String.Join(TIME_SEPARATOR.ToString(),
                slots.Select(o => FormatTime(o)).ToArray());
        }

        internal static string FormatTime(TimeSpan slot)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", slot.Hours, slot.Minutes);
        }

        /// <summary>Đọc danh sách ngày áp dụng. Chuỗi rỗng trả về danh sách rỗng - xem IsApplyDay.</summary>
        internal static List<int> ParseDays(string raw)
        {
            List<int> result = new List<int>();
            if (String.IsNullOrWhiteSpace(raw))
            {
                return result;
            }
            foreach (string part in raw.Split(DAY_SEPARATOR))
            {
                int day;
                if (Int32.TryParse(part.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out day)
                    && day >= 1 && day <= 7 && !result.Contains(day))
                {
                    result.Add(day);
                }
            }
            result.Sort();
            return result;
        }

        internal static string FormatDays(IEnumerable<int> days)
        {
            if (days == null)
            {
                return String.Empty;
            }
            return String.Join(DAY_SEPARATOR.ToString(),
                days.Select(o => o.ToString(CultureInfo.InvariantCulture)).ToArray());
        }

        /// <summary>1 = Thứ 2 ... 7 = Chủ nhật.</summary>
        internal static int ToDayNumber(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        }

        /// <summary>
        /// Ngày truyền vào có nằm trong lịch không.
        /// QT04: bệnh viện bỏ chọn ngày nào thì ngày đó không chạy - bỏ chọn hết thì KHÔNG ngày nào chạy.
        /// Màn Thiết lập lưu đủ cả bảy ngày khi người dùng chọn hết, nên chuỗi rỗng ở đây
        /// chỉ xảy ra khi người dùng cố ý bỏ chọn toàn bộ.
        /// </summary>
        internal static bool IsApplyDay(DateTime date, string rawDays)
        {
            return ParseDays(rawDays).Contains(ToDayNumber(date.DayOfWeek));
        }

        /// <summary>Khóa nhận diện một lượt: ngày + mốc giờ. Dùng để mỗi mốc chỉ chạy một lần trong ngày.</summary>
        internal static string SlotKey(DateTime date, TimeSpan slot)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:yyyyMMdd}_{1}", date, FormatTime(slot));
        }

        /// <summary>
        /// Tìm mốc đến hạn mà hôm nay chưa chạy: mốc MUỘN NHẤT đã qua trong ngày.
        /// Lấy mốc muộn nhất để khi phần mềm mở muộn thì chạy bù ĐÚNG MỘT lượt,
        /// không chạy dồn tất cả các mốc đã qua.
        /// Trả về null khi chưa tới mốc nào, hoặc hôm nay không áp dụng lịch.
        /// </summary>
        internal static TimeSpan? FindDueSlot(DateTime now, string rawTimes, string rawDays, ICollection<string> ranSlotKeys)
        {
            if (!IsApplyDay(now, rawDays))
            {
                return null;
            }
            List<TimeSpan> slots = ParseTimes(rawTimes);
            TimeSpan nowTime = now.TimeOfDay;
            TimeSpan? due = null;
            foreach (TimeSpan slot in slots)
            {
                if (slot > nowTime)
                {
                    break; //danh sách đã sắp xếp tăng dần
                }
                if (ranSlotKeys != null && ranSlotKeys.Contains(SlotKey(now, slot)))
                {
                    continue;
                }
                due = slot; //giữ mốc muộn nhất đã qua và chưa chạy
            }
            return due;
        }

        /// <summary>
        /// Đánh dấu TẤT CẢ các mốc đã qua trong ngày là đã chạy.
        /// QT07 yêu cầu mở muộn thì chạy bù ĐÚNG MỘT lượt: nếu chỉ đánh dấu riêng mốc muộn nhất
        /// thì nhịp 30 giây sau đó sẽ lần lượt lấy các mốc còn lại và chạy dồn nhiều lượt liên tiếp.
        /// </summary>
        internal static void MarkPassedSlotsAsRun(DateTime now, string rawTimes, string rawDays,
            ICollection<string> ranSlotKeys)
        {
            if (ranSlotKeys == null || !IsApplyDay(now, rawDays))
            {
                return;
            }
            TimeSpan nowTime = now.TimeOfDay;
            foreach (TimeSpan slot in ParseTimes(rawTimes))
            {
                if (slot > nowTime)
                {
                    break; //danh sách đã sắp xếp tăng dần
                }
                string key = SlotKey(now, slot);
                if (!ranSlotKeys.Contains(key))
                {
                    ranSlotKeys.Add(key);
                }
            }
        }

        /// <summary>
        /// Mốc gửi kế tiếp kể từ thời điểm hiện tại, dùng để hiển thị trên dòng trạng thái.
        /// Tìm sang các ngày sau nếu hôm nay đã hết mốc; quét tối đa 8 ngày rồi dừng.
        /// </summary>
        internal static DateTime? FindNextSlot(DateTime now, string rawTimes, string rawDays, ICollection<string> ranSlotKeys)
        {
            List<TimeSpan> slots = ParseTimes(rawTimes);
            if (slots.Count == 0)
            {
                return null;
            }
            for (int offset = 0; offset <= 8; offset++)
            {
                DateTime date = now.Date.AddDays(offset);
                if (!IsApplyDay(date, rawDays))
                {
                    continue;
                }
                foreach (TimeSpan slot in slots)
                {
                    DateTime candidate = date.Add(slot);
                    if (candidate <= now)
                    {
                        continue;
                    }
                    if (ranSlotKeys != null && ranSlotKeys.Contains(SlotKey(date, slot)))
                    {
                        continue;
                    }
                    return candidate;
                }
            }
            return null;
        }

        /// <summary>Chuẩn hoá số ngày quét lùi về khoảng cho phép.</summary>
        internal static int NormalizeLookBackDays(int value)
        {
            if (value < MIN_LOOK_BACK_DAYS)
            {
                return MIN_LOOK_BACK_DAYS;
            }
            if (value > MAX_LOOK_BACK_DAYS)
            {
                return MAX_LOOK_BACK_DAYS;
            }
            return value;
        }
    }
}
