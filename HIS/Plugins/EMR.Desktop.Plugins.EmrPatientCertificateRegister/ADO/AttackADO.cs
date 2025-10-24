using EMR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrPatientCertificateRegister.ADO
{
    class AttackADO : EMR_ATTACHMENT
    {
        public string FILE_NAME { get; set; }

        public string Base64Data { get; set; }
        public long DocumentId { get; set; }
        public string Extension { get; set; }
        public string FullName { get; set; }
        public int Dem { get; set; }

        public System.Drawing.Image image { get; set; }

        public AttackADO() { }
    }
}
