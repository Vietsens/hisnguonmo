using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.XtraReportExport
{
    public enum TemplateType
    {
        [Description("Excel")]
        Excel = 1,
        [Description("Docx")]
        Docx = 2,
        [Description("PowerPoint")]
        Pptx = 3,
        [Description("XtraReport")]
        Repx = 4,
    }
}
