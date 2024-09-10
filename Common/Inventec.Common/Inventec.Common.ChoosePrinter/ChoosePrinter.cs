using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.ChoosePrinter
{
    internal partial class ChoosePrinter : Form
    {
        internal string PrinterName;

        public ChoosePrinter()
        {
            InitializeComponent();
        }

        private void ChoosePrinter_Load(object sender, EventArgs e)
        {
            try
            {
                List<string> printers = new List<string>();
                foreach (String printer in PrinterSettings.InstalledPrinters)
                {
                    printers.Add(printer);
                }

                CboPrinter.Properties.DataSource = printers;

                using (PrintDocument printDoc = new PrintDocument())
                {
                    CboPrinter.EditValue = printDoc.PrinterSettings.PrinterName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                PrinterName = CboPrinter.Text;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
