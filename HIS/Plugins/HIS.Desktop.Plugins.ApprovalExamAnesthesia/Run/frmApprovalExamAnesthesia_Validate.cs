using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalExamAnesthesia.Run
{
    public partial class frmApprovalExamAnesthesia : HIS.Desktop.Utility.FormBase
    {
        internal void ValidationControl()
        {
            try
            {
                ValidationCboDoctor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
    }
}
