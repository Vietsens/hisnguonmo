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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.InvoiceBook.Popup.AssignAuthorized
{
    public partial class frmAssignAuthorized : Form
    {
        public V_HIS_INVOICE_BOOK HisInvoiceBookInUc = new V_HIS_INVOICE_BOOK();
        List<V_HIS_USER_INVOICE_BOOK> _listUsersInvoiceBook = new List<V_HIS_USER_INVOICE_BOOK>();
        List<ACS_USER> _listUsers = new List<ACS_USER>();
        List<ACS_USER> _listUsersTemporary = new List<ACS_USER>();
        
        public frmAssignAuthorized()
        {
            InitializeComponent();
        }
    }
}
