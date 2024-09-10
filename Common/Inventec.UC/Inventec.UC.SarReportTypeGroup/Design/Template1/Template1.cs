using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;

namespace Inventec.UC.ListReportTypeGroup.Design.Template1
{

    internal partial class Template1 : UserControl
    {
        private RowCellClickDelegate _rowCellClick;
        private UpdateReportTypeGroup _updateData;
        int pageSize = 0;
        int rowCount = 0;
        int dataTotal = 0;

        public Template1(Data.InitData data)
        {
            InitializeComponent();
            try
            {
                this.pageSize = data.PageSize;
                this._updateData = data.updateData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

      
    }
}
