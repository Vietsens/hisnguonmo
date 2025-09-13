using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHeinPatientType
{
    public class HisRequestUriStore
    {
        internal const string HIS_HEIN_PATIENT_TYPE_CREATE = "/api/HisHeinPatientType/Create";
        internal const string HIS_HEIN_PATIENT_TYPE_DELETE = "/api/HisHeinPatientType/Delete";
        internal const string HIS_HEIN_PATIENT_TYPE_UPDATE = "/api/HisHeinPatientType/Update";
        internal const string HIS_HEIN_PATIENT_TYPE_GET = "/api/HisHeinPatientType/Get";
        internal const string HIS_HEIN_PATIENT_TYPE_LOCK = "/api/HisHeinPatientType/ChangeLock";

        internal const string HIS_TREATMENT_TYPE_GET = "/api/HisTreatmentType/Get";
    }
}
