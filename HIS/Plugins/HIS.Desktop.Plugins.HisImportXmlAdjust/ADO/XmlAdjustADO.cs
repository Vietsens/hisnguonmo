using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportXmlAdjust.ADO
{
    public class XmlAdjustADO
    {
        public long ID { get; set; }

        public string XML1_ID { get; set; }
        public string EXPENSE_ID { get; set; }
        public string XML_TABLE_NUMBER { get; set; }
        public string LINK_CODE { get; set; }
        public string XML_ORDER { get; set; }
        public string PATIENT_CODE { get; set; }
        public string PATIENT_NAME { get; set; }
        public string HEIN_CARD_NUMBER { get; set; }
        public string IN_DATE_STR { get; set; }
        public string OUT_DATE_STR { get; set; }
        public DateTime? IN_DATE { get; set; }
        public DateTime? OUT_DATE { get; set; }
        public string ORDER_DATE_STR { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        /// <summary>
        /// Cột "Trạng thái XML1" - cột (1) của file mẫu 09/BH, map thẳng vào TT_XML1/TRANGTHAI (trạng thái của cả hồ sơ).
        /// Tách riêng khỏi <see cref="STATUS"/> từ bản mẫu mới: trước đây 2 thẻ TRANGTHAI (hồ sơ và dòng chi phí)
        /// dùng chung một cột nên không khai báo được 2 trạng thái khác nhau trên cùng 1 hồ sơ.
        /// </summary>
        public string STATUS_XML1 { get; set; }
        public string ORIGINAL_FIELD { get; set; }
        public string ORIGINAL_VALUE { get; set; }
        public string ORIGINAL_REASON { get; set; }
        public string REJECT_REASON { get; set; }
        public string ADJUST_FIELD { get; set; }
        public string ADJUST_VALUE { get; set; }
        public string ADJUST_REASON { get; set; }
        /// <summary>
        /// Cột "Trạng thái" - cột (2) của file mẫu 09/BH, map vào CHIPHI/TRANGTHAI của từng dòng điều chỉnh chi phí.
        /// </summary>
        public string STATUS { get; set; }

        public string ERROR { get; set; }
    }
}
