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
using HIS.UC.ExecuteRole.ADO;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.UC.ExecuteRole
{
    public partial class UC_ExecuteRole : UserControl
    {
        ExecuteRoleInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        Grid_CellValueChanged gridView_CellValueChanged = null;
        check_CheckedChanged checkEnable_CheckedChanged = null;
        Spin_EditValueChanged spinE_EditValueChanged = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        Grid_MouseDown gridView_MouseDown = null;

        public UC_ExecuteRole(ExecuteRoleInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ExecuteRoleGrid_CustomUnboundColumnData;
                this.btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                this.gridView_MouseDown = ado.ExecuteRoleGrid_MouseDown;
                this.gridView_CellValueChanged = ado.ExecuteRoleGrid_CellValueChanged;
                this.checkEnable_CheckedChanged = ado.checkEnable_CheckedChanged;
                this.spinE_EditValueChanged = ado.spin_EditValueChanged;
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
                result = (List<HIS.UC.ExecuteRole.ExecuteRoleADO>)gridControlExecuteRole.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        public object GetGridControl()
        {
            object result = null;
            try
            {
                result = this.gridControlExecuteRole;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UC_ExecuteRole_Load(object sender, EventArgs e)
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
                gridViewExecuteRole.Columns[0].Visible = false;
                gridViewExecuteRole.Columns[1].Visible = true;
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
                gridViewExecuteRole.Columns[1].Visible = false;
                gridViewExecuteRole.Columns[0].Visible = true;

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
                gridControlExecuteRole.BeginUpdate();
                gridControlExecuteRole.DataSource = this.initADO.ListExecuteRole;
                gridControlExecuteRole.EndUpdate();
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
                if (this.initADO.ListExecuteRoleColumn != null && this.initADO.ListExecuteRoleColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListExecuteRoleColumn)
                    {
                        GridColumn col = gridViewExecuteRole.Columns.AddField(item.FieldName);
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
                    var data = (HIS_EXECUTE_ROLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ExecuteRoleADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListExecuteRole = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewExecuteRole_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ExecuteRoleADO)gridViewExecuteRole.GetRow(e.RowHandle);
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

                        if (e.Column.FieldName == "PRICE")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = SpinPriceD;
                            }
                            else
                            {
                                e.RepositoryItem = SpinPriceE;
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
                var erAdo = (ExecuteRoleADO)gridViewExecuteRole.GetFocusedRow();
                var row = (HIS_EXECUTE_ROLE)gridViewExecuteRole.GetFocusedRow();
                foreach (var item in this.initADO.ListExecuteRole)
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

                gridControlExecuteRole.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click != null && erAdo != null)
                {
                    this.btn_Radio_Enable_Click(row, erAdo);
                }
                gridViewExecuteRole.LayoutChanged();
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
                var row = (ExecuteRoleADO)gridViewExecuteRole.GetFocusedRow();
                if (row != null && this.checkEnable_CheckedChanged != null)
                {
                    this.checkEnable_CheckedChanged(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExecuteRole_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                //state = Inventec.Common.TypeConvert.Parse.ToBoolean((gridViewExecuteRole.GetRowCellValue(e.RowHandle, "check1") ?? "").ToString());
                //listState = new List<bool>();
                //listState.Add(state);
                //var data = (ExecuteRoleADO)gridViewExecuteRole.GetFocusedRow();
                //if (data != null && data is ExecuteRoleADO)
                //{
                //    this.gridView_CellValueChanged(data, e);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExecuteRole_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                try
                {
                    var row = (HIS_EXECUTE_ROLE)gridViewExecuteRole.GetFocusedRow();
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
                //var data = (ExecuteRoleADO)gridViewExecuteRole.GetRow(0);
                //if (data != null && (bool)data.isKeyChoose == false)
                //{
                //    if (CheckAll1.Checked == true && CheckAll1.Enabled == true)
                //    {
                //        for (int i = 0; i < gridViewExecuteRole.RowCount; i++)
                //        {

                //            gridViewExecuteRole.SetRowCellValue(i, "check1", true);

                //        }
                //    }
                //    else if (CheckAll1.Checked == false)
                //    {
                //        for (int i = 0; i < gridViewExecuteRole.RowCount; i++)
                //        {

                //            gridViewExecuteRole.SetRowCellValue(i, "check1", false);

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

        private void SpinPriceE_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ExecuteRoleADO)gridViewExecuteRole.GetFocusedRow();
                if (row != null && this.spinE_EditValueChanged != null)
                {
                    this.spinE_EditValueChanged(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
