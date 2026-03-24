using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.ADOs
{
    public class TestHarmonyADO
    {
        public long SERE_SERV_ID { get; set; }
        public long? RESULT_TIME { get; set; }
        public string RESULT_TIME_STR
        {
            get
            {
                return RESULT_TIME.HasValue ?
                    Inventec.Common.DateTime.Convert.TimeNumberToTimeString(RESULT_TIME.Value) : "";
            }
        }

        // Cột "Túi máu" - VALUE từ danh sách A
        public string BLOOD_VALUE { get; set; }

        // Cột "MT muối" - VALUE từ danh sách B
        public string SALT_VALUE { get; set; }

        // Cột "Anti globulin" - VALUE từ danh sách C
        public string ANTI_GLOBULIN_VALUE { get; set; }
    }
}
