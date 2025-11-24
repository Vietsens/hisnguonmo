using DevExpress.XtraGrid.Views.Base;
using HIS.UC.TestSample.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TestSample
{
    public delegate void Grid_CustomUnboundColumnData(HIS_TEST_SAMPLE_TYPE data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e);
    public delegate void Grid_CustomRowCellEdit(HIS_TEST_SAMPLE_TYPE data, DevExpress.XtraGrid.Views.Base.CustomRowCellEventArgs e);
    public delegate void btn_Radio_Enable_Click(HIS_TEST_SAMPLE_TYPE data);
    public delegate void GridViewTestSample_MouseDown(object sender, MouseEventArgs e);

    public delegate void GridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e);
}
