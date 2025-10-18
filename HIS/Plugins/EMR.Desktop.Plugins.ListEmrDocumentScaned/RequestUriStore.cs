using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.ListEmrDocumentScaned
{
    class RequestUriStore
    {
        internal const string EMR_GET = "api/EmrDocumentScaned/Get";
        internal const string EMR_CREATE = "api/EmrDocumentScaned/Create";
        internal const string EMR_UPDATE = "api/EmrDocumentScaned/Update";
        internal const string EMR_DELETE = "api/EmrDocumentScaned/Delete";

        internal const string EMR_SCAN = "api/EmrDocumentScaned/ScanByTreatment";
    }
}
