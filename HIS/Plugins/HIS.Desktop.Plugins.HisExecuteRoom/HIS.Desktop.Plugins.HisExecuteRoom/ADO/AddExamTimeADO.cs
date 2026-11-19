using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisExecuteRoom.ADO
{
    /// <summary>
    /// Một khung giờ áp dụng cảnh báo chuyển khám (cột ADD_EXAM_TIMES: "thứ|HHmm|HHmm;...", 1 = Chủ nhật).
    /// </summary>
    public class AddExamTimeADO
    {
        public long? DAY { get; set; }
        public DateTime? FROM_TIME { get; set; }
        public DateTime? TO_TIME { get; set; }

        static readonly DateTime BaseDate = new DateTime(2000, 1, 1);

        public static List<AddExamTimeADO> Parse(string value)
        {
            List<AddExamTimeADO> result = new List<AddExamTimeADO>();
            if (String.IsNullOrWhiteSpace(value)) return result;
            foreach (string part in value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] items = part.Split('|');
                long day;
                DateTime? from, to;
                if (items.Length == 3 && long.TryParse(items[0], out day) && (from = ToTime(items[1])).HasValue && (to = ToTime(items[2])).HasValue)
                {
                    result.Add(new AddExamTimeADO() { DAY = day, FROM_TIME = from, TO_TIME = to });
                }
            }
            return result;
        }

        public static string Join(List<AddExamTimeADO> times)
        {
            if (times == null || times.Count == 0) return null;
            return String.Join(";", times.OrderBy(o => o.DAY == 1 ? 8 : o.DAY).ThenBy(o => o.FROM_TIME)
                .Select(o => o.DAY + "|" + o.FROM_TIME.Value.ToString("HHmm") + "|" + o.TO_TIME.Value.ToString("HHmm")));
        }

        static DateTime? ToTime(string hhmm)
        {
            int value;
            if (hhmm == null || hhmm.Length != 4 || !int.TryParse(hhmm, out value)) return null;
            int hour = value / 100, minute = value % 100;
            if (hour > 23 || minute > 59) return null;
            return BaseDate.AddHours(hour).AddMinutes(minute);
        }
    }
}
