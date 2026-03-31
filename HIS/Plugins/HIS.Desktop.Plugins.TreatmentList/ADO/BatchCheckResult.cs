using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentList.ADO
{
    public class BatchCheckResult
    {
        public int ROWNUM { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_DOB { get; set; }
        public string TDL_HEIN_CARD_NUMBER { get; set; }
        public string Message { get; set; }
        public string Note { get; set; }
    }
}
