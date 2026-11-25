using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.ADO
{
    public class VenADO
    {
        public string VEN_CODE { get; set; }
        public string VEN_NAME { get; set; }

        public VenADO(string code, string name)
        {
            this.VEN_CODE = code;
            this.VEN_NAME = name;
        }
    }
}
