using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent
{
    public class WordContentADO
    {
        public WordContentADO() { }
        public string FileName { get; set; }
        public string TemplateFileName { get; set; }
        public Dictionary<string, object> TemplateKey { get; set; }
        public Inventec.Common.SignLibrary.ADO.InputADO EmrInputADO { get; set; }
        public Action<SAR.EFMODEL.DataModels.SAR_PRINT> ActUpdateReference { get; set; }
        public SAR.EFMODEL.DataModels.SAR_PRINT OldSarPrint { get; set; }
        public SAR.EFMODEL.DataModels.SAR_PRINT_TYPE SarPrintType { get; set; }
        public bool? IsViewOnly { get; set; }
    }
}
