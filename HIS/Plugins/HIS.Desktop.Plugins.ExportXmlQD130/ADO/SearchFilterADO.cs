using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
{
    public class SearchFilterADO
    {
        public List<HIS_BRANCH> listBranch { get; set; }
        public List<HIS_PATIENT_TYPE> listPatientType { get; set; }
        public List<HIS_TREATMENT_TYPE> listPTreattmentType { get; set; }
        public FilterTypeADO prfileType { get; set; }
        public FilterTypeADO statusXml { get; set; }
        public List<HIS_PATIENT_TYPE> listDTTT { get; set; }
    }
}
