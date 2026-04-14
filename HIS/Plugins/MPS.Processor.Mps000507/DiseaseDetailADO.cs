using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000507.ADO
{
    /// <summary>
    /// ADO cho từng disease detail result - dùng trong template Excel
    /// Mỗi dòng tương ứng 1 ô checkbox/textbox trên phiếu KSK cán bộ
    /// </summary>
    class DiseaseDetailADO
    {
        // Thông tin disease type (nhóm cha)
        public long? DISEASE_TYPE_ID { get; set; }
        public string DISEASE_TYPE_NAME { get; set; }
        public long? PARENT_TYPE { get; set; }
        public long? NUM_ORDER_TYPE { get; set; }

        // Thông tin disease detail (chi tiết)
        public long DISEASE_DETAIL_ID { get; set; }
        public string NAME { get; set; }
        public long? NUM_ORDER_DETAIL { get; set; }
        public long? IS_CHECKBOX { get; set; }
        public long? IS_OTHER { get; set; }

        // Kết quả - dùng cho template vlookup
        public string IS_CHECK_X { get; set; }  // "X" nếu checked, "" nếu không
        public string OTHER_VALUE { get; set; }  // Giá trị nhập khác
    }
}
