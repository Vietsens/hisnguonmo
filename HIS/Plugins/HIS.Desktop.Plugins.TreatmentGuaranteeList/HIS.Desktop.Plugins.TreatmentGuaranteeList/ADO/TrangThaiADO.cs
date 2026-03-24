using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentGuaranteeList.ADO
{
    public class TrangThaiADO
    {
        public long ID { get; set; }
        public string TrangThai { get; set; }
        public TrangThaiADO(long _ID, string _TrangThai)
        {
            this.ID = _ID;
            this.TrangThai = _TrangThai;
        }
    }
}
