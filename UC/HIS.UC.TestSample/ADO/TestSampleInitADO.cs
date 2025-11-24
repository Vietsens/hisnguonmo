using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TestSample.ADO
{
    public class TestSampleInitADO
    {
        public List<TestSampleADO> ListTestSample { get; set; }
        public List<TestSampleColumn> ListTestSampleColumn { get; set; }

        public bool? IsShowSearchPanel { get; set; }

        public Grid_CustomUnboundColumnData TestSampleGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click btn_Radio_Enable_Click { get; set; }
        public GridViewTestSample_MouseDown GridViewTestSample_MouseDown { get; set; }

        public GridView_MouseRightClick gridView_MouseRightClick { get; set; }
    }
}
