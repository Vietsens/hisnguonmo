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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.UC.CashierRoom.Popup;
using DevExpress.XtraBars;

namespace HIS.UC.CashierRoom
{
    public partial class UCCashierRoom : UserControl
    {
        public UCCashierRoom(HIS.UC.CashierRoom.ADO.CashierRoomInitADO ado)
        {
            InitializeComponent();
            this.initADO = ado;
            this.GridCustomUnboundColumnDataCashierRoom = ado.GridCustomUnboundColumnDataCashierRoom;
            this.BtnRadioEnableClickCashierRoom = ado.BtnRadioEnableClickCashierRoom;
            this.GridViewCashierRoomMouseDownCashierRoom = ado.GridViewCashierRoomMouseDownCashierRoom;
            this.GridViewCashierRoomMouseRightClick = ado.gridView_MouseRightClick;

        }

        bool IsShowSearchPanel;

        HIS.UC.CashierRoom.ADO.CashierRoomInitADO initADO = null;
        HIS.UC.CashierRoom.CashierRoomADO currentAdo;

        GridCustomUnboundColumnDataCashierRoom GridCustomUnboundColumnDataCashierRoom = null;
        BtnRadioEnableClickCashierRoom BtnRadioEnableClickCashierRoom = null;
        GridViewCashierRoomMouseDownCashierRoom GridViewCashierRoomMouseDownCashierRoom = null;
        GridView_MouseRightClick GridViewCashierRoomMouseRightClick = null;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;

        public object GetDataGridView()
        {
            try
            {
                object result = null;
                if (gridViewCashierRoom.IsEditing)
                    gridViewCashierRoom.CloseEditor();

                if (gridViewCashierRoom.FocusedRowModified)
                    gridViewCashierRoom.UpdateCurrentRow();

                return result = (List<HIS.UC.CashierRoom.CashierRoomADO>)gridControlCashierRoom.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        internal void Reload(List<CashierRoomADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListCashierRoom = data;
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
                gridControlCashierRoom.BeginUpdate();
                gridControlCashierRoom.DataSource = this.initADO.ListCashierRoom;
                gridControlCashierRoom.EndUpdate();
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
                if (this.initADO.ListCashierRoomColumn != null && this.initADO.ListCashierRoomColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListCashierRoomColumn)
                    {
                        DevExpress.XtraGrid.Columns.GridColumn col = gridViewCashierRoom.Columns.AddField(item.FieldName);
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

        private void UCCashierRoom_Load(object sender, EventArgs e)
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

        private void gridViewCashierRoom_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (CashierRoomADO)gridViewCashierRoom.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "checkCashierRoom")
                            e.RepositoryItem = data.iskeyChooseCashierRoom ? repositoryItemCheck__Disable : repositoryItemCheck__Enable;
                        if (e.Column.FieldName == "radioCashierRoom")
                            e.RepositoryItem = data.iskeyChooseCashierRoom ? repositoryItemRadio_Enable : repositoryItemRadio_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCashierRoom_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM)gridViewCashierRoom.GetFocusedRow();
                if (row != null && this.GridViewCashierRoomMouseDownCashierRoom != null)
                    this.GridViewCashierRoomMouseDownCashierRoom(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentAdo = (CashierRoomADO)gridViewCashierRoom.GetFocusedRow();

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell && this.GridViewCashierRoomMouseRightClick != null)
                {
                    if (this.barManager1 == null && this.GridViewCashierRoomMouseRightClick != null)
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
                                this.GridViewCashierRoomMouseRightClick(this.currentAdo, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.Paste:
                            {
                                this.GridViewCashierRoomMouseRightClick(this.currentAdo, e);
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

        private void repositoryItemRadio_Enable_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM)gridViewCashierRoom.GetFocusedRow();
                if (row != null)
                {
                    foreach (var item in this.initADO.ListCashierRoom)
                    {
                        item.radioCashierRoom = item.ID == row.ID ? true : false;
                    }
                    gridControlCashierRoom.RefreshDataSource();

                    if (this.BtnRadioEnableClickCashierRoom != null)
                        this.BtnRadioEnableClickCashierRoom(row);
                    gridViewCashierRoom.LayoutChanged();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
