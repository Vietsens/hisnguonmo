using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.FormMedicalRecord.Base
{
    class EmrMainADO : EMR_MAIN.ThongTinDieuTri
    {
        public string MaICD_BenhKemTheo {  get; set; }
        public string MaICD_ChanDoanVaoVien { get; set; }
    }
}
