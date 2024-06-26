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
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class frmCreateInvoice
    {
        private List<HIS_INVOICE_DETAIL_NEW> _listInvoiceDetailNews = new List<HIS_INVOICE_DETAIL_NEW>();

        private void CreateNewItemInvoiceDetail()
        {
            var addTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            var newInvoiceDetail = new HIS_INVOICE_DETAIL_NEW
            {
                ADD_TIME = addTime
            };
            _listInvoiceDetailNews.Add(newInvoiceDetail);
            gctCreateInvoice.DataSource = null;
            LoadDataSourceInvoiceDetail();
        }

        private void DeleteNewItemInvoiceDetail(HIS_INVOICE_DETAIL_NEW invoiceDetailAddIndex)
        {
            _listInvoiceDetailNews.RemoveAll(s => s == invoiceDetailAddIndex);
            LoadDataSourceInvoiceDetail();
        }

        private void LoadDataSourceInvoiceDetail()
        {
            gctCreateInvoice.BeginUpdate();
            gctCreateInvoice.DataSource = _listInvoiceDetailNews.OrderByDescending(s => s.ADD_TIME).ToList();
            gctCreateInvoice.EndUpdate();
        }
    }

    internal class HIS_INVOICE_DETAIL_NEW : HIS_INVOICE_DETAIL
    {
        public long ADD_TIME { get; set; }
    }
}
