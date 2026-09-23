using System.Collections.Generic;

namespace HIS.Desktop.Plugins.KskServiceEditList.ADO
{
    /// <summary>
    /// Kết quả gộp của tất cả các lô gọi API ServiceEdit
    /// </summary>
    public class KskServiceEditBatchResultADO
    {
        public List<KskServiceEditResultSDO> Results { get; set; }
        public KskServiceEditSummarySDO Summary { get; set; }
        /// <summary>Số hồ sơ chưa được xử lý do mất phiên đăng nhập</summary>
        public int NotProcessedCount { get; set; }
        /// <summary>Thông báo lỗi chung (lỗi kết nối, lỗi dữ liệu đầu vào) theo từng lô</summary>
        public List<string> Messages { get; set; }

        public KskServiceEditBatchResultADO()
        {
            this.Results = new List<KskServiceEditResultSDO>();
            this.Summary = new KskServiceEditSummarySDO();
            this.Messages = new List<string>();
        }
    }
}
