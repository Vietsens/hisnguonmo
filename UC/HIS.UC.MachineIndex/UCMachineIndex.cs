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
using HIS.UC.MachineIndex.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.UC.MachineIndex
{
    public partial class UCMachineIndex : UserControl
    {
        MachineIndexInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 = null;
        gridViewMachineIndex_MouseDownMest gridViewMachineIndex_MouseDownMest = null;

        Grid_RowCellClick gridView_RowCellClick = null;
        List<LIS_MACHINE> listMachine;

        bool isShowSearchPanel;

        public UCMachineIndex(MachineIndexInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.MachineIndexGrid_CustomUnboundColumnData;
                this.gridView_RowCellClick = ado.ListMachineIndexGrid_RowCellClick;
                gridViewMachineIndex_MouseDownMest = ado.gridViewMachineIndex_MouseDownMest;
                btn_Radio_Enable_Click1 = ado.btn_Radio_Enable_Click1;
                if (ado.IsShowSearchPanel.HasValue)
                {
                    this.isShowSearchPanel = ado.IsShowSearchPanel.Value;
                }
                CommonParam param = new CommonParam();
                LisMachineFilter filter = new LisMachineFilter();

                listMachine = new BackendAdapter(param).Get<List<LIS_MACHINE>>("api/LisMachine/Get", ApiConsumers.LisConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Info("listMachine=" + (listMachine != null && listMachine.Count > 0 ? listMachine.Count.ToString() : ""));

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
                result = (List<HIS.UC.MachineIndex.MachineIndexADO>)gridControlMachineIndex.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCMachineIndex_Load(object sender, EventArgs e)
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

        private void UpdateDataSource()
        {
            try
            {
                gridControlMachineIndex.BeginUpdate();
                gridControlMachineIndex.DataSource = this.initADO.ListMachineIndex;
                gridControlMachineIndex.EndUpdate();
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
                if (this.initADO.ListMachineIndexColumn != null && this.initADO.ListMachineIndexColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListMachineIndexColumn)
                    {
                        GridColumn col = gridViewMachineIndex.Columns.AddField(item.FieldName);
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

        private void gridViewMachineIndex_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (MachineIndexADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (this.gridView_CustomUnboundColumnData != null)
                            this.gridView_CustomUnboundColumnData(data, e);
                        if (e.Column.FieldName == "MACHINE_NAME")
                        {
                            if (listMachine != null && listMachine.Count > 0)
                            {
                                var machine = listMachine.FirstOrDefault(o => o.ID == data.MACHINE_ID);
                                if (machine != null)
                                {
                                    e.Value = machine.MACHINE_NAME;
                                }
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

        internal void Reload(List<MachineIndexADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListMachineIndex = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMachineIndex_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (MachineIndexADO)gridViewMachineIndex.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "checkMachine")
                        {
                            if (data.isKeyChooseMachine)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }
                        if (e.Column.FieldName == "radioMachine")
                        {
                            if (data.isKeyChooseMachine)
                            {
                                e.RepositoryItem = repositoryItemRadio_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemRadio_Disable;
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

        private void repositoryItemRadio_Enable_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (LIS_MACHINE_INDEX)gridViewMachineIndex.GetFocusedRow();
                foreach (var item in this.initADO.ListMachineIndex)
                {
                    if (item.ID == row.ID)
                    {
                        item.radioMachine = true;
                    }
                    else
                    {
                        item.radioMachine = false;
                    }
                }

                gridControlMachineIndex.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click1 != null)
                {
                    this.btn_Radio_Enable_Click1(row);
                }
                gridViewMachineIndex.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMachineIndex_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (LIS_MACHINE_INDEX)gridViewMachineIndex.GetFocusedRow();
                if (row != null && this.gridViewMachineIndex_MouseDownMest != null)
                {
                    this.gridViewMachineIndex_MouseDownMest(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMachineIndex_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var data = (LIS_MACHINE_INDEX)gridViewMachineIndex.GetFocusedRow();

                if (this.gridView_RowCellClick != null)
                {
                    this.gridView_RowCellClick(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
