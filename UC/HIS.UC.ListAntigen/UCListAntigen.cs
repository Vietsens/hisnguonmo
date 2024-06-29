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
using HIS.UC.ListAntigen.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.UC.ListAntigen.Popup;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;

namespace HIS.UC.ListAntigen
{
    public partial class UCListAntigen : UserControl
    {
        ListAntigenInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        gridView_MouseDown gridViewMety_MouseDownMety = null;
        Grid_RowCellClick gridView_RowCellClick = null;
        bool isShowSearchPanel;

       
        HIS.UC.ListAntigen.ListAntigenADO currentMedicineType;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;
        GridView_MouseRightClick gridView_MouseRightClick = null;

        public UCListAntigen(ListAntigenInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ListAntigenGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                this.gridViewMety_MouseDownMety = ado.gridViewMety_MouseDownMety;
                this.gridView_MouseRightClick = ado.GridView_MouseRightClick;
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
                result = (List<HIS.UC.ListAntigen.ListAntigenADO>)gridControlListAntigen.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCListAntigen_Load(object sender, EventArgs e)
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
                gridControlListAntigen.BeginUpdate();
                gridControlListAntigen.DataSource = this.initADO.ListAntigen;
                gridControlListAntigen.EndUpdate();
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
                if (this.initADO.ListAntigenColumn != null && this.initADO.ListAntigenColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListAntigenColumn)
                    {
                        GridColumn col = gridViewListAntigen.Columns.AddField(item.FieldName);
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

        private void gridViewListAntigen_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (HIS_ANTIGEN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ListAntigenADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListAntigen = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ReloadRow(HIS_ANTIGEN_METY data)
        {
            try
            {
                if (data != null)
                {
                    foreach (var item in this.initADO.ListAntigen)
                    {
                        if (item.ID == data.MEDICINE_TYPE_ID)
                        {
                            //item.ALERT_MIN_IN_STOCK_STR = data.ALERT_MIN_IN_STOCK;
                            //item.ALERT_MAX_IN_STOCK = data.ALERT_MAX_IN_STOCK;
                            //if (data.IS_PREVENT_EXP.HasValue)
                            //    item.IS_PREVENT_EXP = data.IS_PREVENT_EXP.Value == 1 ? true : false;
                            //if (data.IS_PREVENT_MAX.HasValue)
                            //    item.IS_PREVENT_MAX = data.IS_PREVENT_MAX.Value == 1 ? true : false;
                            //if (data.IS_GOODS_RESTRICT.HasValue)
                            //    item.IsGoodsRetrict = data.IS_GOODS_RESTRICT.Value == 1 ? true : false;
                            break;
                        }
                    }

                    gridControlListAntigen.RefreshDataSource();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCashCollect_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ListAntigenADO)gridViewListAntigen.GetRow(e.RowHandle);
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

                        if (e.Column.FieldName == "IsGoodsRetrict")
                        {
                            e.RepositoryItem = repositoryItemCheckEditIsGoodsRetrict;
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
                        if (e.Column.FieldName == "ALERT_MIN_IN_STOCK_STR")
                        {
                            e.RepositoryItem = repositoryItemSpinEdit1;
                        }
                        if (e.Column.FieldName == "ALERT_MAX_IN_STOCK")
                        {
                            e.RepositoryItem = repositoryItemSpinEdit2;
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
                var row = (HIS_ANTIGEN)gridViewListAntigen.GetFocusedRow();
                foreach (var item in this.initADO.ListAntigen)
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

                gridControlListAntigen.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click != null)
                {
                    this.btn_Radio_Enable_Click(row);
                }
                gridViewListAntigen.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCListAntigen_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (HIS_ANTIGEN)gridViewListAntigen.GetFocusedRow();
                if (row != null && this.gridViewMety_MouseDownMety != null)
                {
                    this.gridViewMety_MouseDownMety(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewListAntigen_ShowingEditor(object sender, CancelEventArgs e)
        {

        }
        private void gridViewListAntigen_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var data = (HIS_ANTIGEN)gridViewListAntigen.GetFocusedRow();

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

        private void gridViewListAntigen_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentMedicineType = (HIS.UC.ListAntigen.ListAntigenADO)gridViewListAntigen.GetFocusedRow();

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell && this.gridView_MouseRightClick != null)
                {
                    if (this.barManager1 == null)
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
                if ((e.Item is BarButtonItem) && this.currentMedicineType != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.Copy:
                            {
                                this.gridView_MouseRightClick(this.currentMedicineType, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.Paste:
                            {
                                this.gridView_MouseRightClick(this.currentMedicineType, e);
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
