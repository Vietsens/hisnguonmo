/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
