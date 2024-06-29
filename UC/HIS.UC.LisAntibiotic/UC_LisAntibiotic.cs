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
using HIS.UC.LisAntibiotic.ADO;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.UC.LisAntibiotic.Popup;
using DevExpress.XtraBars;
using LIS.EFMODEL.DataModels;

namespace HIS.UC.LisAntibiotic
{
    public partial class UC_LisAntibiotic : UserControl
    {
        LisAntibioticInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        Grid_CellValueChanged gridView_CellValueChanged = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        Grid_MouseDown gridView_MouseDown = null;

        GridView_MouseRightClick gridView_MouseRightClick = null;
        HIS.UC.LisAntibiotic.AntibioticADO currentAdo;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;

        public UC_LisAntibiotic(LisAntibioticInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ExecuteRoomGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                this.gridView_MouseDown = ado.ExecuteRoomGrid_MouseDown;
                this.gridView_CellValueChanged = ado.ExecuteRoomGrid_CellValueChanged;
                this.gridView_MouseRightClick = ado.gridView_MouseRightClick;
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
                result = (List<HIS.UC.LisAntibiotic.AntibioticADO>)gridControlExecuteRoom.DataSource;
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
                    var data = (LIS_ANTIBIOTIC)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<AntibioticADO> data)
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
                    var data = (AntibioticADO)gridViewExecuteRoom.GetRow(e.RowHandle);
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
                var row = (AntibioticADO)gridViewExecuteRoom.GetFocusedRow();
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
                    LIS_ANTIBIOTIC executeRoom = new LIS_ANTIBIOTIC();
                    Inventec.Common.Mapper.DataObjectMapper.Map<LIS_ANTIBIOTIC>(executeRoom, row);
                    this.btn_Radio_Enable_Click(executeRoom);
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
                gridViewExecuteRoom.PostEditor();
                var data = (AntibioticADO)gridViewExecuteRoom.GetFocusedRow();
                if (data != null && data is AntibioticADO && this.gridView_CellValueChanged != null)
                {
                    this.gridView_CellValueChanged(data, e);

                }
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

                //if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                //{
                //    GridView view = sender as GridView;
                //    GridHitInfo hi = view.CalcHitInfo(e.Location);
                //    if (hi.InRowCell)
                //    {
                //        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                //        {
                //            view.FocusedRowHandle = hi.RowHandle;
                //            view.FocusedColumn = hi.Column;
                //            view.ShowEditor();
                //            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                //            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                //            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                //            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                //            Rectangle gridGlyphRect =
                //                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                //                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                //                 glyphRect.Width,
                //                 glyphRect.Height);
                //            if (!gridGlyphRect.Contains(e.Location))
                //            {
                //                view.CloseEditor();
                //                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                //                {
                //                    view.SelectCell(hi.RowHandle, hi.Column);
                //                }
                //                else
                //                {
                //                    view.UnselectCell(hi.RowHandle, hi.Column);
                //                }
                //            }
                //            else
                //            {
                //                checkEdit.Checked = !checkEdit.Checked;
                //                view.CloseEditor();
                //            }
                //            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                //        }
                //    }
                //}

                var row = (AntibioticADO)gridViewExecuteRoom.GetFocusedRow();
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

        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentAdo = (HIS.UC.LisAntibiotic.AntibioticADO)gridViewExecuteRoom.GetFocusedRow();

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell && this.gridView_MouseRightClick != null)
                {
                    if (this.barManager1 == null && this.gridView_MouseRightClick != null)
                        this.barManager1 = new DevExpress.XtraBars.BarManager();
                    this.barManager1.Form = this;
                    popupMenuProcessor = new PopupMenuProcessor(this.barManager1, GridView_MouseRightClick);
                    this.popupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && this.currentAdo != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.Copy:
                            {
                                this.gridView_MouseRightClick(this.currentAdo, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.Paste:
                            {
                                this.gridView_MouseRightClick(this.currentAdo, e);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExecuteRoom_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
