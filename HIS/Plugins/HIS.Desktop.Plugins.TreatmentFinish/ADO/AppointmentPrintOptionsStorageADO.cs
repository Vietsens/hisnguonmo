using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.ADO
{
    class AppointmentPrintOptionsStorageADO
    {
        public static bool InPhieuHenKham { get; set; }
        public static bool XemTruocKhiIn { get; set; }
        public static bool KyPhieuHenKham { get; set; }

        public static bool CoTuyChonNaoDuocChon =>
            InPhieuHenKham || XemTruocKhiIn || KyPhieuHenKham;
    }
}
