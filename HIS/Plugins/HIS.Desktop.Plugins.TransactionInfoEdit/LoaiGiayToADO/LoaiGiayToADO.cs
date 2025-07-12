using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionInfoEdit.LoaiGiayToADO
{
    internal class LoaiGiayToADO
    {
        public long ID { get; set; }
        public string LoaiGiayTo { get; set; }
        public LoaiGiayToADO(long _ID, string _LoaiGiayTo)
        {
            this.ID = _ID;
            this.LoaiGiayTo = _LoaiGiayTo;
        }
    }
}
