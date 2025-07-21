using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO
{
    class LoaiPhieuInADO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Check { get; set; }

        public LoaiPhieuInADO() { }

        public LoaiPhieuInADO(string id, string name, bool check = false)
        {
            this.ID = id;
            this.Name = name;
            this.Check = check;
        }
    }
}
