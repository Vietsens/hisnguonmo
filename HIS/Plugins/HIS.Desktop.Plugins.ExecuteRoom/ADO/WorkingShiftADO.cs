using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.ExecuteRoom.ADO
{
    public class WorkingShiftADO : HIS_WORKING_SHIFT
    {
        public string TIME_DISPLAY { get; set; }

        public WorkingShiftADO()
        {
        }

        public WorkingShiftADO(HIS_WORKING_SHIFT data)
        {
            try
            {
                if (data != null)
                {
                    // Map tất cả properties từ HIS_WORKING_SHIFT
                    Inventec.Common.Mapper.DataObjectMapper.Map<WorkingShiftADO>(this, data);

                    if (!string.IsNullOrEmpty(this.FROM_TIME) && !string.IsNullOrEmpty(this.TO_TIME))
                    {
                        long? fromSeconds = ParseTimeString(this.FROM_TIME);
                        long? toSeconds = ParseTimeString(this.TO_TIME);

                        if (fromSeconds.HasValue && toSeconds.HasValue)
                        {
                            TimeSpan fromTime = TimeSpan.FromSeconds(fromSeconds.Value);
                            TimeSpan toTime = TimeSpan.FromSeconds(toSeconds.Value);

                            this.TIME_DISPLAY = string.Format("{0:D2}:{1:D2} - {2:D2}:{3:D2}",
                                fromTime.Hours, fromTime.Minutes,
                                toTime.Hours, toTime.Minutes);
                        }
                        else
                        {
                            this.TIME_DISPLAY = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? ParseTimeString(string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
                return null;

            // 1. Parse số giây (format cũ: "28800")
            long result;
            if (long.TryParse(timeString, out result))
                return result;

            // 2. Parse TimeSpan chuẩn ("HH:mm:ss" hoặc "HH:mm")
            TimeSpan timeSpan;
            if (TimeSpan.TryParse(timeString, out timeSpan))
                return (long)timeSpan.TotalSeconds;

            // 3. Parse format tùy chỉnh "H:mm"
            string[] parts = timeString.Split(':');
            if (parts.Length >= 2)
            {
                int hours, minutes;
                if (int.TryParse(parts[0], out hours) && int.TryParse(parts[1], out minutes))
                {
                    int seconds = 0;
                    if (parts.Length > 2)
                        int.TryParse(parts[2], out seconds);

                    if (hours >= 0 && hours < 24 && minutes >= 0 && minutes < 60 && seconds >= 0 && seconds < 60)
                        return hours * 3600 + minutes * 60 + seconds;
                }
            }

            Inventec.Common.Logging.LogSystem.Warn("ParseTimeString failed: " + timeString);
            return null;
        }
    }
}