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

        // Thời gian sửa (MODIFY_TIME từ V_HIS_SERE_SERV_TEIN)
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
    }
}
