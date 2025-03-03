using HIS.Desktop.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientV8.Class
{
    public class ServiceReqSttADO : ServiceReqSttSDO
    {
        public string DISPLAY_NAME { get; set; }
        public Color BackColor { get; set; }
    }
}
