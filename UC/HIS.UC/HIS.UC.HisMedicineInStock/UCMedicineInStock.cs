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
using HIS.UC.HisMedicineInStock.ADO;
using HIS.UC.HisMedicineInStock.Run;
using HIS.UC.HisMedicineInStock.Reload;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;

namespace HIS.UC.HisMedicineInStock
{
    internal partial class UCMedicineInStock : UserControl
    {
        #region Declare
        HisMedicineInStockInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btnRadioEnable_Click = null;
        gridViewService_MouseDown gridView_MouseDownDelegate = null;
        GridView_Click gridView_Click = null;
        TxtKeyword_KeyDown txtKeyword_KeyDown = null;
        GridView_KeyDown gridView_KeyDown = null;
        #endregion

        public UCMedicineInStock(HisMedicineInStockInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.MedicineInStockGrid_CustomUnboundColumnData;
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
                if (gridViewMedicineInStock.IsEditing)
                    gridViewMedicineInStock.CloseEditor();

                if (gridViewMedicineInStock.FocusedRowModified)
                    gridViewMedicineInStock.UpdateCurrentRow();

                result = (List<MOS.SDO.HisMedicineInStockSDO>)gridControlMedicineInStock.DataSource;
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
                if (this.initADO.MedicineInStockColumns != null && this.initADO.MedicineInStockColumns.Count > 0)
                {
                    foreach (var item in this.initADO.MedicineInStockColumns)
                    {
                        GridColumn col = gridViewMedicineInStock.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.OptionsColumn.ReadOnly = item.ReadOnly;
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

        private void gridViewMedicineType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (MOS.SDO.HisMedicineInStockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<MOS.SDO.HisMedicineInStockSDO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.hisMedicineInStockSDOs = data;
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
                gridControlMedicineInStock.BeginUpdate();
                gridControlMedicineInStock.DataSource = this.initADO.hisMedicineInStockSDOs;
                gridControlMedicineInStock.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (MOS.SDO.HisMedicineInStockSDO)gridViewMedicineInStock.GetRow(e.RowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UC_MedicineTypeGrid_Load(object sender, EventArgs e)
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
                var row = (MOS.SDO.HisMedicineInStockSDO)gridViewMedicineInStock.GetFocusedRow();

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

        private void gridViewService_Click(object sender, EventArgs e)
        {
            try
            {
                var medicineInStock = (MOS.SDO.HisMedicineInStockSDO)gridViewMedicineInStock.GetFocusedRow();
                if (this.gridView_Click != null && medicineInStock != null)
                {
                    gridView_Click(medicineInStock);
                }
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
                txtKeyword.Focus();
                txtKeyword.SelectAll();
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
                    string KeywordValue = txtKeyword.Text.Trim();
                    if (this.txtKeyword_KeyDown != null)
                    {
                        this.txtKeyword_KeyDown(KeywordValue);
                    }
                    gridViewMedicineInStock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineInStock_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (gridView_KeyDown != null)
                {
                    var medicineInStock = (MOS.SDO.HisMedicineInStockSDO)gridViewMedicineInStock.GetFocusedRow();
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
