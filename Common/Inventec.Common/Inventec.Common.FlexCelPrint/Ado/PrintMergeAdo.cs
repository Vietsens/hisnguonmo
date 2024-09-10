using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint.Ado
{
    public class PrintMergeAdo
    {
        public PrintMergeAdo() { }

        public string printTypeCode { get; set; }
        public string fileName { get; set; }
        public object data { get; set; }
        public string printerName { get; set; }
        public int numCopy { get; set; }
        public string saveFilePath { get; set; }
        public MemoryStream saveMemoryStream { get; set; }
        public bool isAllowExport { get; set; }
        public Inventec.Common.FlexCelPrint.DelegateEventLog eventLog { get; set; }
        public Inventec.Common.FlexCelPrint.DelegatePrintLog PrintLog { get; set; }
        public Inventec.Common.FlexCelPrint.DelegateReturnEventPrint eventPrint { get; set; }
        public Inventec.Common.FlexCelPrint.DelegateShowPrintLog ShowPrintLog { get; set; }
        public Action<string, string> ActShowPrintLog { get; set; }
        public Action ShowTutorialModule { get; set; }
        public Dictionary<string, object> TemplateKey { get; set; }
        public bool IsSingleCopy { get; set; }
        public object EmrInputADO { get; set; }
        public bool IsAllowEditTemplateFile { get; set; }
        public bool IsPdfFile { get; set; }
    }
}
