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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.UC.BidBloodTypeGrid.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.UC.BidBloodTypeGrid
{
    public partial class UCBidBloodTypeGrid : UserControl
    {
        BidBloodTypeGridInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        Grid_Click gridView_Click = null;
        Grid_Enter gridView_Enter = null;

        bool isShowSearchPanel;

        public UCBidBloodTypeGrid(BidBloodTypeGridInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.BidBloodTypeGridGrid_CustomUnboundColumnData;
                this.gridView_Click = ado.BidBloodTypeGridGrid_Click;
                this.gridView_Enter = ado.BidBloodTypeGridGrid_Enter;

                if (ado.IsShowSearchPanel.HasValue)
                {
                    this.isShowSearchPanel = ado.IsShowSearchPanel.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public object GetDataGridView()
        {
            object result = null;
            try
            {
                result = (List<HIS.UC.BidBloodTypeGrid.BloodTypeADO>)gridControlBidBloodTypeGrid.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCBidBloodTypeGrid_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    ProcessColumn();
                    UpdateDataSource(this.initADO.ListBloodTypeAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ResetKeyWord()
        {
            try
            {
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FocusKeyword()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void UpdateDataSource(List<BloodTypeADO> data)
        {
            try
            {
                if (data != null)
                {
                    data = data.OrderBy(o => o.BLOOD_TYPE_CODE).ToList();
                }
                gridControlBidBloodTypeGrid.BeginUpdate();
                gridControlBidBloodTypeGrid.DataSource = data;
                gridControlBidBloodTypeGrid.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColumn()
        {
            try
            {
                if (this.initADO.ListBidBloodTypeGridColumn != null && this.initADO.ListBidBloodTypeGridColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListBidBloodTypeGridColumn)
                    {
                        GridColumn col = gridViewBidBloodTypeGrid.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
                        if (item.image != null)
                        {
                            col.Image = item.image;
                            col.ImageAlignment = StringAlignment.Center;
                        }

                        if (item.UnboundColumnType != null && item.UnboundColumnType != UnboundColumnType.Bound)
                            col.UnboundType = item.UnboundColumnType;
                        if (item.Format != null)
                        {
                            col.DisplayFormat.FormatString = item.Format.FormatString;
                            col.DisplayFormat.FormatType = item.Format.FormatType;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBidBloodTypeGrid_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (BloodTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null && this.gridView_CustomUnboundColumnData != null)
                    {
                        this.gridView_CustomUnboundColumnData(data, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload(List<BloodTypeADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListBloodTypeAdo = data;
                    UpdateDataSource(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBidBloodTypeGrid_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (BloodTypeADO)gridViewBidBloodTypeGrid.GetRow(e.RowHandle);
                    if (data != null)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBidBloodTypeGrid_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (BloodTypeADO)gridViewBidBloodTypeGrid.GetFocusedRow();
                if (row != null && this.gridView_Click != null)
                {
                    this.gridView_Click(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewBidBloodTypeGrid_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var row = (BloodTypeADO)gridViewBidBloodTypeGrid.GetFocusedRow();
                    if (row != null && this.gridView_Enter != null)
                    {
                        this.gridView_Enter(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        SearchByKeyWord(txtSearch.Text.Trim());
                    }
                    else
                    {
                        Reload(this.initADO.ListBloodTypeAdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SearchByKeyWord(string keyWord)
        {
            try
            {
                if (this.initADO != null && this.initADO.ListBloodTypeAdo != null)
                {
                    List<BloodTypeADO> blood = new List<BloodTypeADO>();
                    blood = this.initADO.ListBloodTypeAdo.Where(o =>
                        (o.BLOOD_TYPE_CODE ?? "").Contains(keyWord)
                        || (o.BLOOD_TYPE_NAME ?? "").Contains(keyWord)
                        || (o.SERVICE_UNIT_NAME ?? "").Contains(keyWord)
                        || (o.SUPPLIER_NAME ?? "").Contains(keyWord)
                        || (o.SERVICE_UNIT_CODE ?? "").Contains(keyWord)
                        || (o.SUPPLIER_CODE ?? "").Contains(keyWord)
                        ).ToList();

                    UpdateDataSource(blood);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }
    }
}
