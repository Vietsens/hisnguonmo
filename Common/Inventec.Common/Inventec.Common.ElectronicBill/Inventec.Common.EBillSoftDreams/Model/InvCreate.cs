using Inventec.Common.EBillSoftDreams.ModelXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EBillSoftDreams.Model
{
    public class InvCreate
    {
        public Inv Inv { get; set; }
        public string Pattern { get; set; }
        public string Serial { get; set; }
    }
}
