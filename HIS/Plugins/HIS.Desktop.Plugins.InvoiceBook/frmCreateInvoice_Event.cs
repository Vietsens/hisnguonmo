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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class frmCreateInvoice
    {
        private void gctCreateInvoice_Load(object sender, EventArgs e)
        {
            CreateNewItemInvoiceDetail();
        }

        private void grvCreateInvoice_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var data = (HIS_INVOICE_DETAIL_NEW)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data == null) return;
                var fistRowGridGiew = (HIS_INVOICE_DETAIL_NEW)((IList)((BaseView)sender).DataSource)[0];
                switch (e.Column.FieldName)
                {
                    case "ADD_NEW_ITEM":
                        e.RepositoryItem = data == fistRowGridGiew ? btnAddItem : btnDeleteItem;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvCreateInvoice_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound) return;
                var data = (HIS_INVOICE_DETAIL_NEW)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data == null) return;
                switch (e.Column.FieldName)
                {
                    case "NUMBER_ORDER":
                        e.Value = e.ListSourceRowIndex + 1;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            CreateNewItemInvoiceDetail();
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                var dataFocuseRow = (HIS_INVOICE_DETAIL_NEW)grvCreateInvoice.GetFocusedRow();
                DeleteNewItemInvoiceDetail(dataFocuseRow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCreateInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
                this.Close();
        }
    }
}
