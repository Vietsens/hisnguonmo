using System;

namespace Inventec.Common.IntegrationError
{
    /// <summary>
    /// Ghi lại lỗi bị "nuốt" (catch chỉ ghi log rồi trả null/rỗng) khi dựng dữ liệu gửi đối tác,
    /// để cuối luồng báo đúng là lỗi code HIS / máy trạm thay vì để người dùng đổ cho đối tác.
    /// ThreadStatic: mỗi luồng xử lý giữ lỗi riêng. Luồng chính gọi Reset() trước và Build/Reset sau mỗi thao tác.
    /// </summary>
    public static class SilentErrorTracker
    {
        [ThreadStatic]
        private static Exception silentError;
        [ThreadStatic]
        private static string silentStage;

        public static void Reset()
        {
            silentError = null;
            silentStage = null;
        }

        /// <summary>Gọi trong catch chỉ ghi log. Chỉ giữ lỗi đầu tiên (thường là gốc).</summary>
        public static void Note(Exception ex, string stage)
        {
            try
            {
                if (ex != null && silentError == null)
                {
                    silentError = ex;
                    silentStage = stage;
                }
            }
            catch { }
        }

        public static Exception Error
        {
            get { return silentError; }
        }

        public static string Stage
        {
            get { return silentStage; }
        }

        /// <summary>
        /// Thông báo cho lỗi bị nuốt (null nếu không có): máy trạm nếu là lỗi file/thiết bị, còn lại là lỗi code HIS.
        /// </summary>
        public static string BuildMessage(IntegrationPartner partner)
        {
            Exception ex = silentError;
            if (ex == null) return null;
            ErrorSource source = IntegrationErrorClassifier.Classify(ex);
            if (source != ErrorSource.Workstation) source = ErrorSource.Software;
            string stage = String.IsNullOrWhiteSpace(silentStage) ? "dựng dữ liệu gửi đi" : silentStage;
            string summary = source == ErrorSource.Workstation
                ? string.Format("{0} khi {1}", IntegrationErrorMessage.GetDefaultSummary(source, ex, partner), stage)
                : string.Format("Phần mềm HIS gặp lỗi khi {0} nên dữ liệu gửi sang {1} có thể thiếu hoặc sai. Lỗi phía đối tác (nếu có) có thể là hệ quả của lỗi này", stage, IntegrationErrorMessage.GetName(partner));
            return IntegrationErrorMessage.Build(source, partner, summary, IntegrationErrorClassifier.GetDetail(ex));
        }
    }
}
