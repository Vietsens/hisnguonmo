using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.ADOs
{
    public class TestIndexResultADO
    {
        public long SERE_SERV_TEIN_ID { get; set; }
        public string TEST_INDEX_CODE { get; set; }
        public string TEST_INDEX_NAME { get; set; }
        public string VALUE { get; set; }
        public long? TUBE_SLOT { get; set; }
        public long SERE_SERV_ID { get; set; }
        public long TREATMENT_ID { get; set; }

        // Thêm thuộc tính RESULT_TIME
        public long? RESULT_TIME { get; set; }

        // Thuộc tính hiển thị thời gian dạng chuỗi
        public string RESULT_TIME_STR
        {
            get
            {
                if (RESULT_TIME.HasValue)
                {
                    return Inventec.Common.DateTime.Convert.TimeNumberToTimeString(RESULT_TIME.Value);
                }
                return "";
            }
        }
    }
}