using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientVer5.Class
{
    public class DisplayOptionADO
    {
        public DisplayOptionADO() { }
        public decimal? SizeTitle { get; set; }
        public Color ColorTittle { get; set; }
        public Color ColorBackround { get; set; }
        public string Title { get; set; }
        public decimal? SizeTitleSTT { get; set; }
        public decimal? SizeSTT { get; set; }
        public Color ColorSTT { get; set; }
        public decimal? SizeContentSTT { get; set; }
        public string TitleSTTNext { get; set; }
        public Color ColorSTTNext { get; set; }
        public string Content { get; set; }
        public decimal? SizeList { get; set; }
        public Color ColorList { get; set; }
        public Color ColorPriority { get; set; }
        public bool IsShowCol { get; set; }
        public bool IsNotInDebt { get;set; }
    }
}
