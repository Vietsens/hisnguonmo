using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.FlexCelPrint.FormPrintReason
{
    public partial class frmPrintReason : DevExpress.XtraEditors.XtraForm
    {
        public string ReturnValue { get; set; }

        public frmPrintReason()
        {
            InitializeComponent();
        }

        private void frmPrintReason_Load(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPrintReason_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string printReason = txtPrintReason.Text;
                if (String.IsNullOrWhiteSpace(printReason))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa nhập lý do in!");
                    return;
                }
                this.ReturnValue = printReason;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
