using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignBed.ADO
{
    public class DataGridAdo : SereServADO
    {
        public DateTime? TIME_FROM { get; set; }
        public DateTime? TIME_TO { get; set; }
        public decimal? QUANTITY { get; set; }
        public string BED_CODE { get; set; }

        public string TIME_FROM_STR => TIME_FROM?.ToString("dd/MM/yyyy HH:mm");
        public string TIME_TO_STR => TIME_TO?.ToString("dd/MM/yyyy HH:mm");
        public string QUANTITY_STR => QUANTITY?.ToString();
    }
}
