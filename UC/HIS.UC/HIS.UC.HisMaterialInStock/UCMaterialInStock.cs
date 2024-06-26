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
using HIS.UC.HisMaterialInStock.ADO;
using HIS.UC.HisMaterialInStock.Run;
using HIS.UC.HisMaterialInStock.Reload;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.UC.HisMaterialInStock
{
    internal partial class UCMaterialInStock : UserControl
    {
        #region Declare
        HisMaterialInStockInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btnRadioEnable_Click = null;
        gridView_MouseDown gridView_MouseDownDelegate = null;
        GridView_Click gridView_Click = null;
        TxtKeyword_KeyDown txtKeyword_KeyDown = null;
        GridView_KeyDown gridView_KeyDown = null;
        #endregion

        public UCMaterialInStock(HisMaterialInStockInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.MaterialInStockGrid_CustomUnboundColumnData;
                this.btnRadioEnable_Click = ado.btnRadioEnable_Click;
                this.gridView_MouseDownDelegate = ado.gridView_MouseDown;
                this.gridView_Click = ado.gridView_Click;
                this.txtKeyword_KeyDown = ado.txtKeyword_KeyDown;
                this.gridView_KeyDown = ado.gridView_KeyDown;
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
                if (gridViewMaterialInStock.IsEditing)
                    gridViewMaterialInStock.CloseEditor();

                if (gridViewMaterialInStock.FocusedRowModified)
                    gridViewMaterialInStock.UpdateCurrentRow();

                result = (List<MOS.SDO.HisMaterialInStockSDO>)gridControlMaterialInStock.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void ProcessColumn()
        {
            try
            {
                if (this.initADO.MaterialInStockColumns != null && this.initADO.MaterialInStockColumns.Count > 0)
                {
                    foreach (var item in this.initADO.MaterialInStockColumns)
                    {
                        GridColumn col = gridViewMaterialInStock.Columns.AddField(item.FieldName);
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

                        col.OptionsColumn.ShowCaption = item.ShowCaption;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterialType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (MOS.SDO.HisMaterialInStockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<MOS.SDO.HisMaterialInStockSDO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.hisMaterialInStockSDOs = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDataSource()
        {
            try
            {
                gridControlMaterialInStock.BeginUpdate();
                gridControlMaterialInStock.DataSource = this.initADO.hisMaterialInStockSDOs;
                gridControlMaterialInStock.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterialType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (MOS.SDO.HisMaterialInStockSDO)gridViewMaterialInStock.GetRow(e.RowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UC_MaterialTypeGrid_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    ProcessColumn();
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (MOS.SDO.HisMaterialInStockSDO)gridViewMaterialInStock.GetFocusedRow();

                if (row != null && this.gridView_MouseDownDelegate != null)
                {
                    this.gridView_MouseDownDelegate(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterialInStock_Click(object sender, EventArgs e)
        {
            try
            {
                var materialInStock = (MOS.SDO.HisMaterialInStockSDO)gridViewMaterialInStock.GetFocusedRow();
                if (this.gridView_Click != null && materialInStock != null)
                {
                    gridView_Click(materialInStock);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FocusKeyword()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string txtKeydownText = txtKeyword.Text.Trim();
                    if (this.txtKeyword_KeyDown != null)
                    {
                        this.txtKeyword_KeyDown(txtKeydownText);
                    }
                    gridViewMaterialInStock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialInStock_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (gridView_KeyDown != null)
                {
                    var medicineInStock = (MOS.SDO.HisMaterialInStockSDO)gridViewMaterialInStock.GetFocusedRow();
                    gridView_KeyDown(sender, e, medicineInStock);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
