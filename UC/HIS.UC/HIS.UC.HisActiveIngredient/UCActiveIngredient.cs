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
using HIS.UC.HisActiveIngredient.ADO;
using HIS.UC.HisActiveIngredient.Run;
using HIS.UC.HisActiveIngredient.Reload;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.UC.HisActiveIngredient
{
    internal partial class UCActiveIngredient : UserControl
    {
        #region Declare
        HisActiveIngredientInitADO initADO = null;
        GridView_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btnRadioEnable_Click = null;
        gridView_MouseDown gridViewService_MouseDownDelegate = null;
        #endregion

        public UCActiveIngredient(HisActiveIngredientInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ServiceGrid_CustomUnboundColumnData;
                this.btnRadioEnable_Click = ado.btnRadioEnable_Click;
                this.gridViewService_MouseDownDelegate = ado.gridViewService_MouseDown;
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
                if (gridViewActiveIngredient.IsEditing)
                    gridViewActiveIngredient.CloseEditor();

                if (gridViewActiveIngredient.FocusedRowModified)
                    gridViewActiveIngredient.UpdateCurrentRow();

                result = (List<ActiveIngredientADO>)gridControlActiveIngredient.DataSource;
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
                if (this.initADO.activeIngredientColumns != null && this.initADO.activeIngredientColumns.Count > 0)
                {
                    foreach (var item in this.initADO.activeIngredientColumns)
                    {
                        GridColumn col = gridViewActiveIngredient.Columns.AddField(item.FieldName);
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

        private void gridViewMedicineType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (ActiveIngredientADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ActiveIngredientADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.activeIngredientADOs = data;
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
                gridControlActiveIngredient.BeginUpdate();
                gridControlActiveIngredient.DataSource = this.initADO.activeIngredientADOs;
                gridControlActiveIngredient.EndUpdate();
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
                    var data = (ActiveIngredientADO)gridViewActiveIngredient.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "check2")
                        {
                            if (data.isKeyChoose1)
                            {
                                e.RepositoryItem = CheckD;
                            }
                            else
                            {
                                e.RepositoryItem = CheckE;
                            }
                        }
                        if (e.Column.FieldName == "radio2")
                        {
                            if (data.isKeyChoose1)
                            {
                                e.RepositoryItem = RadioE;
                            }
                            else
                            {
                                e.RepositoryItem = RadioD;
                            }

                        }
                    }
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

        private void RadioEnable_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_ACTIVE_INGREDIENT)gridViewActiveIngredient.GetFocusedRow();
                foreach (var item in this.initADO.activeIngredientADOs)
                {
                    if (item.ID == row.ID)
                    {
                        item.radio2 = true;
                    }
                    else
                    {
                        item.radio2 = false;
                    }
                }

                gridControlActiveIngredient.RefreshDataSource();

                if (row != null && this.btnRadioEnable_Click != null)
                {
                    this.btnRadioEnable_Click(row);
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
                var row = (HIS_ACTIVE_INGREDIENT)gridViewActiveIngredient.GetFocusedRow();
                if (row != null && this.gridViewService_MouseDownDelegate != null)
                {
                    this.gridViewService_MouseDownDelegate(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
