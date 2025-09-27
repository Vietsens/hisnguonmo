using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientExpMest.ADO
{
    public class ExpMestCallADO
    {
        public List<ExpMestSttADO> expMestSttADOs { get; set; } = new List<ExpMestSttADO>();
        public List<ExpMestTypeADO> expMestTypeADOs { get; set; } = new List<ExpMestTypeADO>();
        public long sizeText { get; set; }
        public string colorText { get; set; }
        public string bgColorText { get; set; }

        public long sizeTextTitle { get; set; }
        public string colorTextTitle { get; set; }
        public string bgColorTextTitle { get; set; }

        public long sizeTextList { get; set; }
        public string colorTextList { get; set; }
        public string bgColorTextList { get; set; }

        public long sizeTextContent { get; set; }
        public string colorTextContent { get; set; }
        public string bgColorTextContent { get; set; }

        public long sizeTextCalling { get; set; }
        public string colorTextCalling { get; set; }
        public string bgColorTextCalling { get; set; }

        public string ContentTitle { get; set; }
        public string NoteTitle { get; set; }
    }
}
