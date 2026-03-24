using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineIsUsedPatient.ADO
{
    public class TimeUsed
    {
        public long ID { get; set; }
        public long? TG_MORNING { get; set; }
        public long? TG_NOON { get; set; }
        public long? TG_EVENING { get; set; }
        public long? TG_AFTERNOON { get; set; }
        public decimal? SL_MORNING { get; set; }
        public decimal? SL_NOON { get; set; }
        public decimal? SL_EVENING { get; set; }
        public decimal? SL_AFTERNOON { get; set; }
        public bool IS_ADD { get; set; }
        public bool IS_DELETE { get; set; }

    }
}
