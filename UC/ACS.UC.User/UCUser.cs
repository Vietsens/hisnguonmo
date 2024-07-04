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
using ACS.EFMODEL.DataModels;
using ACS.UC.User.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using ACS.UC.User.Popup;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;

namespace ACS.UC.User
{
    public partial class UCUser : UserControl
    {
        UserInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click_Medi  btn_Radio_Enable_Click_Medi = null;
        gridViewUser_MouseDownMedi gridViewUser_MouseDownMedi = null;
        Check__Enable_CheckedChanged Check__Enable_CheckedChanged = null;

        Grid_RowCellClick gridView_RowCellClick = null;

        bool isShowSearchPanel;

        GridView_MouseRightClick gridView_MouseRightClick = null;
        ACS.UC.User.UserADO currentAdo;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;

        public UCUser(UserInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.UserGrid_CustomUnboundColumnData;
                this.gridView_RowCellClick = ado.ListUserGrid_RowCellClick;
                this.gridView_MouseRightClick = ado.gridView_MouseRightClick;
                gridViewUser_MouseDownMedi = ado.gridViewUser_MouseDownMedi;
                btn_Radio_Enable_Click_Medi = ado.btn_Radio_Enable_Click_Medi;
                Check__Enable_CheckedChanged = ado.Check__Enable_CheckedChanged;
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
                result = (List<ACS.UC.User.UserADO>)gridControlUser.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCUser_Load(object sender, EventArgs e)
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
                gridControlUser.BeginUpdate();
                gridControlUser.DataSource = this.initADO.ListUser;
                gridControlUser.EndUpdate();
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
                if (this.initADO.ListUserColumn != null && this.initADO.ListUserColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListUserColumn)
                    {
                        GridColumn col = gridViewUser.Columns.AddField(item.FieldName);
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

        private void gridViewUser_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (ACS_USER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<UserADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListUser = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ReloadRow(ACS_USER data)
        {
            //try
            //{
            //    if (data != null)
            //    {
            //        foreach (var item in this.initADO.ListUser)
            //        {
            //            if (item.ID == data.USER_ID)
            //            {
            //                item.EXPEND_AMOUNT_STR = data.EXPEND_AMOUNT;
            //                item.EXPEND_PRICE_STR = data.EXPEND_PRICE;
            //                break;
            //            }
            //        }

            //        gridControlUser.RefreshDataSource();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void gridViewUser_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (UserADO)gridViewUser.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "checkMedi")
                        {
                            if (data.isKeyChooseMedi)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }
                        if (e.Column.FieldName == "radioMedi")
                        {
                            if (data.isKeyChooseMedi)
                            {
                                e.RepositoryItem = repositoryItemRadio_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemRadio_Disable;
                            }
                        }
                        if (e.Column.FieldName == "EXPEND_AMOUNT_STR")
                        {
                            e.RepositoryItem = repositoryItemSpinEdit_Amount;
                        }
                        if (e.Column.FieldName == "EXPEND_PRICE_STR")
                        {
                            e.RepositoryItem = repositoryItemSpinEdit_Price;
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
                var row = (ACS_USER)gridViewUser.GetFocusedRow();
                foreach (var item in this.initADO.ListUser)
                {
                    if (item.ID == row.ID)
                    {
                        item.radioMedi = true;
                    }
                    else
                    {
                        item.radioMedi = false;
                    }
                }

                gridControlUser.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click_Medi != null)
                {
                    this.btn_Radio_Enable_Click_Medi(row);
                }
                gridViewUser.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewUser_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (ACS_USER)gridViewUser.GetFocusedRow();
                if (row != null && this.gridViewUser_MouseDownMedi != null)
                {
                    this.gridViewUser_MouseDownMedi(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewUser_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var data = (ACS_USER)gridViewUser.GetFocusedRow();

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

        private void repositoryItemCheck__Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (UserADO)gridViewUser.GetFocusedRow();
                if (row != null && this.Check__Enable_CheckedChanged != null)
                {
                    this.Check__Enable_CheckedChanged(row);
                }
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
                currentAdo = (ACS.UC.User.UserADO)gridViewUser.GetFocusedRow();

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
    }
}
