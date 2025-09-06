using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO
{
    public class OutPatientPresADO
    {
        public long MEDI_MATE_ID { get; set; }
        public long SERVICE_TYPE_ID { get; set; }
        public long USE_TIME { get; set; }
        public List<long> TAKE_BEAN_ID { get; set; }
        public string PrimaryKey { get; set; }
    }
}
