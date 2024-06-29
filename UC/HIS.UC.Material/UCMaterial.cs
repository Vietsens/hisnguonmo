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
using HIS.UC.Material.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using HIS.UC.Material.Popup;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.UC.Material
{
    public partial class UCMaterial : UserControl
    {
        MaterialInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click_Mate btn_Radio_Enable_Click_Mate = null;
        gridViewMaterial_MouseDownMate gridViewMaterial_MouseDownMate = null;
        Check__Enable_CheckedChanged Check__Enable_CheckedChanged = null;
        GridView_MouseRightClick gridView_MouseRightClick = null;
        HIS.UC.Material.MaterialADO currentAdo;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;

        Grid_RowCellClick gridView_RowCellClick = null;

        bool isShowSearchPanel;

        public UCMaterial(MaterialInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.MaterialGrid_CustomUnboundColumnData;
                this.gridView_RowCellClick = ado.ListMaterialGrid_RowCellClick;
                gridViewMaterial_MouseDownMate = ado.gridViewMaterial_MouseDownMate;
                Check__Enable_CheckedChanged = ado.Check__Enable_CheckedChanged;
                btn_Radio_Enable_Click_Mate = ado.btn_Radio_Enable_Click_Mate;
                this.gridView_MouseRightClick = ado.gridView_MouseRightClick;
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
                result = (List<HIS.UC.Material.MaterialADO>)gridControlMaterial.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCMaterial_Load(object sender, EventArgs e)
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
                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = this.initADO.ListMaterial;
                gridControlMaterial.EndUpdate();
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
                if (this.initADO.ListMaterialColumn != null && this.initADO.ListMaterialColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListMaterialColumn)
                    {
                        GridColumn col = gridViewMaterial.Columns.AddField(item.FieldName);
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

        private void gridViewMaterial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (V_HIS_MATERIAL_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<MaterialADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListMaterial = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ReloadRow(HIS_SERVICE_MATY data)
        {
            try
            {
                if (data != null)
                {
                    foreach (var item in this.initADO.ListMaterial)
                    {
                        if (item.ID == data.MATERIAL_TYPE_ID)
                        {
                            item.EXPEND_AMOUNT_STR = data.EXPEND_AMOUNT;
                            item.EXPEND_PRICE_STR = data.EXPEND_PRICE;
                            item.AMOUNT_BHYT_STR = data.AMOUNT_BHYT;
                            item.checkExpend = data.IS_NOT_EXPEND == 1;
                            break;
                        }
                    }

                    gridControlMaterial.RefreshDataSource();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (MaterialADO)gridViewMaterial.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "checkMate")
                        {
                            if (data.isKeyChooseMate)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }
                        if (e.Column.FieldName == "radioMate")
                        {
                            if (data.isKeyChooseMate)
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
                        if (e.Column.FieldName == "checkExpend")
                        {
                            e.RepositoryItem = repositoryItemCheck__Expend;
                        }
                        if (e.Column.FieldName == "AMOUNT_BHYT_STR")
                        {
                            e.RepositoryItem = repositoryItemSpinEdit__Amount_Bhyt;
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
                var row = (V_HIS_MATERIAL_TYPE)gridViewMaterial.GetFocusedRow();
                foreach (var item in this.initADO.ListMaterial)
                {
                    if (item.ID == row.ID)
                    {
                        item.radioMate = true;
                    }
                    else
                    {
                        item.radioMate = false;
                    }
                }

                gridControlMaterial.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click_Mate != null)
                {
                    this.btn_Radio_Enable_Click_Mate(row);
                }
                gridViewMaterial.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (V_HIS_MATERIAL_TYPE)gridViewMaterial.GetFocusedRow();
                if (row != null && this.gridViewMaterial_MouseDownMate != null)
                {
                    this.gridViewMaterial_MouseDownMate(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var data = (V_HIS_MATERIAL_TYPE)gridViewMaterial.GetFocusedRow();

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
                var row = (MaterialADO)gridViewMaterial.GetFocusedRow();
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
                currentAdo = (HIS.UC.Material.MaterialADO)gridViewMaterial.GetFocusedRow();

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
