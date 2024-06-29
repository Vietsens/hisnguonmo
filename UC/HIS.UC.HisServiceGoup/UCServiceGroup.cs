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
using HIS.UC.HisServiceGoup.ADO;
using HIS.UC.HisServiceGoup.Run;
using HIS.UC.HisServiceGoup.Reload;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.UC.HisServiceGoup
{
    internal partial class UCServiceGroup : UserControl
    {
        #region Declare
        HisServiceGroupInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btnRadioEnable_Click = null;
        ServiceGroupGridView_Click serviceGroupGridView_Click = null;
        gridViewServiceGroup_MouseDown gridViewServiceGroup_MouseDownDeleteGate = null;
        #endregion

        public UCServiceGroup(HisServiceGroupInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ServiceGroupGrid_CustomUnboundColumnData;
                this.btnRadioEnable_Click = ado.btnRadioEnable_Click;
                this.serviceGroupGridView_Click = ado.serviceGroupGridView_Click;
                this.gridViewServiceGroup_MouseDownDeleteGate = ado.gridViewServiceGroup_MouseDownDelegate;
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
                if (gridViewServiceGroup.IsEditing)
                    gridViewServiceGroup.CloseEditor();

                if (gridViewServiceGroup.FocusedRowModified)
                    gridViewServiceGroup.UpdateCurrentRow();

                result = (List<ServiceGroupADO>)gridControlServiceGroup.DataSource;
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
                if (this.initADO.ServiceGroupColumns != null && this.initADO.ServiceGroupColumns.Count > 0)
                {
                    foreach (var item in this.initADO.ServiceGroupColumns)
                    {
                        GridColumn col = gridViewServiceGroup.Columns.AddField(item.FieldName);
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
                    var data = (ServiceGroupADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ServiceGroupADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ServiceGroupADOs = data;
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
                gridControlServiceGroup.BeginUpdate();
                gridControlServiceGroup.DataSource = this.initADO.ServiceGroupADOs;
                gridControlServiceGroup.EndUpdate();
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
                    var data = (ServiceGroupADO)gridViewServiceGroup.GetRow(e.RowHandle);
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
                        if (e.Column.FieldName == "isPublic")
                        {
                            e.RepositoryItem = CheckEditIsPublic;
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
                var row = (HIS_SERVICE_GROUP)gridViewServiceGroup.GetFocusedRow();
                foreach (var item in this.initADO.ServiceGroupADOs)
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

                gridControlServiceGroup.RefreshDataSource();

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

        private void gridViewMedicineType_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var serviceGroupFocus = (HIS_SERVICE_GROUP)gridViewServiceGroup.GetFocusedRow();
                if (serviceGroupGridView_Click != null)
                {
                    serviceGroupGridView_Click(serviceGroupFocus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceGroup_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var serviceGroupFocus = (HIS_SERVICE_GROUP)gridViewServiceGroup.GetFocusedRow();
                if (serviceGroupFocus != null && gridViewServiceGroup_MouseDownDeleteGate != null)
                {
                    gridViewServiceGroup_MouseDownDeleteGate(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
