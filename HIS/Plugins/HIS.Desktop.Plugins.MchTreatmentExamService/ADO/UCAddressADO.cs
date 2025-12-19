using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.ADO
{
    public class UCAddressADO
    {
        public UCAddressADO() { }

        public string Province_Code { get; set; }
        public string Province_Name { get; set; }
        public string District_Code { get; set; }
        public string District_Name { get; set; }
        public string Commune_Code { get; set; }
        public string Commune_Name { get; set; }
        public string Address { get; set; }
        public string PatientId { get; set; }
        public bool IsNoDistrict { get; set; }
    }
}
