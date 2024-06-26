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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.ServiceReqPatient.Printf
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        ADO.Printf printf;
        public XtraForm1(ADO.Printf _printf)
        { 
            InitializeComponent();
            printf = _printf;
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            OpenFileDialog open1 = new OpenFileDialog();
            open1.
        }
    }
}
