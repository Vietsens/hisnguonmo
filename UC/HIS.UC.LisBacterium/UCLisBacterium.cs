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
using HIS.UC.LisBacterium.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.UC.LisBacterium.Popup;
using DevExpress.XtraBars;
using LIS.EFMODEL.DataModels;

namespace HIS.UC.LisBacterium
{
    public partial class UCLisBacterium : UserControl
    {
        RoomInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        gridViewRoom_MouseDownRoom gridViewRoom_MouseDownRoom;
        Rooom_MouseRightClick rooom_MouseRightClick;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;
        HIS.UC.LisBacterium.LisBacteriumADO currentRoom;
        PopupMenuProcessor.ItemType itemType;

        bool isShowSearchPanel;

        public UCLisBacterium(RoomInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.RoomGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                gridViewRoom_MouseDownRoom = ado.gridViewRoom_MouseDownRoom;
                this.rooom_MouseRightClick = ado.rooom_MouseRightClick;

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
                result = (List<HIS.UC.LisBacterium.LisBacteriumADO>)gridControlRoom.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCRoom_Load(object sender, EventArgs e)
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

        public object GetGridControl()
        {
            object result = null;
            try
            {
                result = this.gridControlRoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UpdateDataSource()
        {
            try
            {
                gridControlRoom.BeginUpdate();
                gridControlRoom.DataSource = this.initADO.ListRoom;
                gridControlRoom.EndUpdate();
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
                if (this.initADO.ListRoomColumn != null && this.initADO.ListRoomColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListRoomColumn)
                    {
                        GridColumn col = gridViewRoom.Columns.AddField(item.FieldName);
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

        private void gridViewRoom_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (LIS_BACTERIUM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<LisBacteriumADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListRoom = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoom_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (LisBacteriumADO)gridViewRoom.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "check1")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }
                        if (e.Column.FieldName == "radio1")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = repositoryItemRadio_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemRadio_Disable;
                            }
                        }
                        if (e.Column.FieldName == "checkPriority")
                        {
                            if (data.IsEnablePriority)
                            {
                                e.RepositoryItem = repositoryItemCheckPriority_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheckPriority_Disable;
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
                var row = (LIS_BACTERIUM)gridViewRoom.GetFocusedRow();
                foreach (var item in this.initADO.ListRoom)
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

                gridControlRoom.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click != null)
                {
                    this.btn_Radio_Enable_Click(row);
                }

                gridViewRoom.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoom_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (LIS_BACTERIUM)gridViewRoom.GetFocusedRow();
                if (row != null && this.gridViewRoom_MouseDownRoom != null)
                {
                    this.gridViewRoom_MouseDownRoom(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoom_PopupMenuShowing_1(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentRoom = (HIS.UC.LisBacterium.LisBacteriumADO)gridViewRoom.GetFocusedRow();

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell && (this.initADO.IsShowMenuPopup == true || this.rooom_MouseRightClick != null))
                {
                    if (this.barManager1 == null)
                        this.barManager1 = new DevExpress.XtraBars.BarManager();
                    this.barManager1.Form = this;
                    popupMenuProcessor = new PopupMenuProcessor(this.barManager1, Rooom_MouseRightClick);
                    this.popupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Rooom_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && this.rooom_MouseRightClick != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.CopyPhongSangPhong:
                            {
                                this.rooom_MouseRightClick(currentRoom, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.PastePhongSangPhong:
                            {
                                this.rooom_MouseRightClick(currentRoom, e);
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
    }
}
