using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.ADOs
{
    /// <summary>
    /// Một dòng dropdown "XN hòa hợp" — tương ứng 1 túi máu trong 1 y lệnh.
    /// Mỗi bộ cấu hình (A|B|C) sinh tối đa 1 dòng cho mỗi y lệnh, với điều kiện
    /// tồn tại bản ghi chỉ số mã túi (A).
    /// </summary>
    public class TestHarmonyADO
    {
        // Khóa duy nhất cho cboXNHH.EditValue — phân biệt các dòng dropdown
        public long ROW_ID { get; set; }

        // Y lệnh nguồn (TDL_SERVICE_REQ_ID của V_HIS_SERE_SERV_TEIN)
        public long? SERVICE_REQ_ID { get; set; }

        // Thời gian sửa của chỉ số mã túi (A) — dùng để sắp xếp + hiển thị
        public long? MODIFY_TIME { get; set; }

        public string MODIFY_TIME_STR
        {
            get
            {
                return MODIFY_TIME.HasValue
                    ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(MODIFY_TIME.Value)
                    : "";
            }
        }

        // Giá trị của chỉ số mã túi (A) — đây là MÃ TÚI thực tế, dùng để match khi click túi máu
        public string BLOOD_VALUE { get; set; }

        // Giá trị của chỉ số hòa hợp muối (B) — có thể rỗng nếu chưa có
        public string SALT_VALUE { get; set; }

        // Giá trị của chỉ số hòa hợp anti-globulin (C) — có thể rỗng nếu chưa có
        public string ANTI_GLOBULIN_VALUE { get; set; }
    }
}
