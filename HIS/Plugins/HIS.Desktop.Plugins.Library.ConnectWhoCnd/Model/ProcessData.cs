using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd.Model
{
    internal class ProcessData
    {
        public HIS_TREATMENT Treatment { get; set; }
        public HIS_DHST Dhst { get; set; }
        public List<HIS_SERE_SERV> V_HIS_SERE_SERVs { get; set; }
        public List<HIS_SERE_SERV_TEIN> HIS_SERE_SERV_TEINs { get; set; }
    }
}
