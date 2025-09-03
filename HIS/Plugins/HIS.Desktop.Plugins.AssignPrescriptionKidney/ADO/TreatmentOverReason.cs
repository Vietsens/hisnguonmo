using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO
{
    public class TreatmentOverReason
    {
        public long treatmentId { get; set; }
        public string overReason { get; set; }
        public long overReasonId { get; set; }
    }
}
