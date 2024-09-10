using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Design.Template1
{
    internal partial class Template1
    {
        private void Template1_Load(object sender, EventArgs e)
        {
            try
            {
                Language();
                SetDefaultControl();
                btnRefresh_Click(null, null);
                //btnSearch_Click(null, null);
                //txtKeyword.Focus();
                //txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Language()
        {
            try
            {
               
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
