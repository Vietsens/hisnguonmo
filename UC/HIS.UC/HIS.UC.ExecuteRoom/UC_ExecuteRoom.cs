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
using HIS.UC.ExecuteRoom.ADO;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.UC.ExecuteRoom
{
    public partial class UC_ExecuteRoom : UserControl
    {
        ExecuteRoomInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        Grid_CellValueChanged gridView_CellValueChanged = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        Grid_MouseDown gridView_MouseDown = null;

        public UC_ExecuteRoom(ExecuteRoomInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ExecuteRoomGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                this.gridView_MouseDown = ado.ExecuteRoomGrid_MouseDown;
                this.gridView_CellValueChanged = ado.ExecuteRoomGrid_CellValueChanged;
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
                result = (List<HIS.UC.ExecuteRoom.ExecuteRoomADO>)gridControlExecuteRoom.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UC_ExecuteRoom_Load(object sender, EventArgs e)
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

        public void SetIsKeyChooseTrue()
        {
            try
            {
                gridViewExecuteRoom.Columns[0].Visible = false;
                gridViewExecuteRoom.Columns[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetIsKeyChooseFalse()
        {
            try
            {
                gridViewExecuteRoom.Columns[1].Visible = false;
                gridViewExecuteRoom.Columns[0].Visible = true;

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
                gridControlExecuteRoom.BeginUpdate();
                gridControlExecuteRoom.DataSource = this.initADO.ListExecuteRoom;
                gridControlExecuteRoom.EndUpdate();
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
                if (this.initADO.ListExecuteRoomColumn != null && this.initADO.ListExecuteRoomColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListExecuteRoomColumn)
                    {
                        GridColumn col = gridViewExecuteRoom.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.Image = item.image;
                        col.ImageAlignment = item.imageAlignment;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
                        col.ToolTip = item.ToolTip;
                        //col.Visible=visible;
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

        private void gridViewRoom_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (HIS_EXECUTE_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ExecuteRoomADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListExecuteRoom = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewExecuteRoom_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ExecuteRoomADO)gridViewExecuteRoom.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        //if (data.isKeyChoose)
                        //{
                        //    CheckAll1.Enabled = false;
                        //}
                        //else
                        //{
                        //    CheckAll1.Enabled = true;
                        //}
                        if (e.Column.FieldName == "check1")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = CheckDisable;
                            }
                            else
                            {
                                e.RepositoryItem = CheckEnable;

                            }
                        }
                        if (e.Column.FieldName == "radio1")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = RadioEnable;
                            }
                            else
                            {
                                e.RepositoryItem = RadioDisable;
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

        private void RadioEnable_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_EXECUTE_ROOM)gridViewExecuteRoom.GetFocusedRow();
                foreach (var item in this.initADO.ListExecuteRoom)
                {
                    if (item.ID == row.ID)
                    {
                        item.radio1 = true;
                    }
                    else
                    {
                        item.radio1 = false;
                    }
                }

                gridControlExecuteRoom.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click != null)
                {
                    this.btn_Radio_Enable_Click(row);
                }
                gridViewExecuteRoom.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckEnable_Click(object sender, EventArgs e)
        {
        }

        private void CheckEnable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExecuteRoom_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                //state = Inventec.Common.TypeConvert.Parse.ToBoolean((gridViewExecuteRoom.GetRowCellValue(e.RowHandle, "check1") ?? "").ToString());
                //listState = new List<bool>();
                //listState.Add(state);
                //var data = (ExecuteRoomADO)gridViewExecuteRoom.GetFocusedRow();
                //if (data != null && data is ExecuteRoomADO)
                //{
                //    this.gridView_CellValueChanged(data, e);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExecuteRoom_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                try
                {
                    var row = (HIS_EXECUTE_ROOM)gridViewExecuteRoom.GetFocusedRow();
                    if (row != null && this.gridView_MouseDown != null)
                    {
                        this.gridView_MouseDown(sender, e);
                    }
                }
                catch (Exception ex)
                {

                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAll1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //var data = (ExecuteRoomADO)gridViewExecuteRoom.GetRow(0);
                //if (data != null && (bool)data.isKeyChoose == false)
                //{
                //    if (CheckAll1.Checked == true && CheckAll1.Enabled == true)
                //    {
                //        for (int i = 0; i < gridViewExecuteRoom.RowCount; i++)
                //        {

                //            gridViewExecuteRoom.SetRowCellValue(i, "check1", true);

                //        }
                //    }
                //    else if (CheckAll1.Checked == false)
                //    {
                //        for (int i = 0; i < gridViewExecuteRoom.RowCount; i++)
                //        {

                //            gridViewExecuteRoom.SetRowCellValue(i, "check1", false);

                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAll1_Click(object sender, EventArgs e)
        {
        }
    }
}
