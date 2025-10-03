using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AdjustmentTransaction.AdjustmentTransaction
{
    public partial class frmAdjustmentTransaction : FormBase
    {

        V_HIS_TRANSACTION currentTransaction = new V_HIS_TRANSACTION();
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        public frmAdjustmentTransaction(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION tran) 
            : base(module)
        {
            InitializeComponent();
            this.currentTransaction = tran;
            this.currentModule = module;
        }

        private void SetDefaultValueTransaction()
        {
            try
            {
                if (currentTransaction != null)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
