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

namespace HIS.UC.Department
{
    public partial class UCDepartment : UserControl
    {
        GridCustomUnboundColumnDataDepartment GridCustomUnboundColumnDataDepartment = null;
        BtnRadioEnableClickDepartment BtnRadioEnableClickDepartment = null;
        GridViewDepartmentMouseDownDepartment GridViewDepartmentMouseDownDepartment = null;

        HIS.UC.Department.ADO.DepartmentInitADO initADO = null;

        bool IsShowSearchPanel;

        public UCDepartment(HIS.UC.Department.ADO.DepartmentInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.GridCustomUnboundColumnDataDepartment = ado.GridCustomUnboundColumnDataDepartment;
                this.GridViewDepartmentMouseDownDepartment = ado.GridViewDepartmentMouseDownDepartment;
                this.BtnRadioEnableClickDepartment = ado.BtnRadioEnableClickDepartment;
                if (ado.IsShowSearchPanel.HasValue)
                    this.IsShowSearchPanel = ado.IsShowSearchPanel.Value;
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
                if (gridViewDepartment.IsEditing)
                    gridViewDepartment.CloseEditor();

                if (gridViewDepartment.FocusedRowModified)
                    gridViewDepartment.UpdateCurrentRow();

                return result = (List<HIS.UC.Department.DepartmentADO>)gridControlDepartment.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        internal void Reload(List<DepartmentADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListDepartment = data;
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
                gridControlDepartment.BeginUpdate();
                gridControlDepartment.DataSource = this.initADO.ListDepartment;
                gridControlDepartment.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCDepartment_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    UpdateDataSource();
                    ProcessColumn();
                }
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
                if (this.initADO.ListDepartmentColumn != null && this.initADO.ListDepartmentColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListDepartmentColumn)
                    {
                        DevExpress.XtraGrid.Columns.GridColumn col = gridViewDepartment.Columns.AddField(item.FieldName);
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
                        if (item.UnboundColumnType != null && item.UnboundColumnType != DevExpress.Data.UnboundColumnType.Bound)
                        {
                            col.UnboundType = item.UnboundColumnType;
                        }
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

        private void gridViewDepartment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            //DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (DepartmentADO)gridViewDepartment.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "radioDepartment")
                        {
                            if (data.isKeyChooseDepartment)
                            {
                                e.RepositoryItem = repositoryItemRadio_Enable;

                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemRadio_Disable;
                            }
                        }
                        if (e.Column.FieldName == "checkDepartment")
                        {
                            if (data.isKeyChooseDepartment)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
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

        private void gridViewDepartment_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridViewDepartment.GetFocusedRow();
                if (row != null && this.GridViewDepartmentMouseDownDepartment != null)
                    this.GridViewDepartmentMouseDownDepartment(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemRadio_Enable_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridViewDepartment.GetFocusedRow();
                if (row != null)
                {
                    foreach (var item in this.initADO.ListDepartment)
                    {
                        if (item.ID == row.ID)
                        {
                            item.radioDepartment = true;
                        }
                        else
                            item.radioDepartment = false;
                    }

                    gridControlDepartment.RefreshDataSource();

                    if (this.BtnRadioEnableClickDepartment != null)
                        this.BtnRadioEnableClickDepartment(row);

                    gridViewDepartment.LayoutChanged();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
