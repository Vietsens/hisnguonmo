using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.DocumentViewer
{
    public class InputADO
    {
        public InputADO() { }
        public string URL { get; set; }
        public int? NumberOfCopy { get; set; }
        public bool DeleteWhenClose { get; set; }
        public Action ActPrintSuccess { get; set; }
        public string PrintPageSize { get; set; }
    }
}
