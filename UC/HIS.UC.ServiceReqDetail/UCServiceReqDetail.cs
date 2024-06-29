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
using HIS.UC.ServiceReqDetail.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using ACS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ADO;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.UC.ServiceReqDetail.Popup;

namespace HIS.UC.ServiceReqDetail
{
    public partial class UCServiceReqDetail : UserControl
    {
        ServiceReqDetailInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        GridView_RowCellStyle grid_RowCellStyle = null;
        btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 = null;
        gridView_MouseDownAccount gridViewAccount_MouseDownAccount = null;
        Check__Enable_CheckedChanged Check__Enable_CheckedChanged = null;
        GridView_MouseRightClick gridView_MouseRightClick = null;
        HIS.UC.ServiceReqDetail.ServiceReqDetailADO currentAccount;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;

        bool isShowSearchPanel;

        public UCServiceReqDetail(ServiceReqDetailInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.Grid_CustomUnboundColumnData;
                btn_Radio_Enable_Click1 = ado.btn_Radio_Enable_Click1;
                grid_RowCellStyle = ado.gridView_RowCellStyle;
                gridViewAccount_MouseDownAccount = ado.gridView_MouseDown;
                Check__Enable_CheckedChanged = ado.Check__Enable_CheckedChanged;
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
                if (gridView.IsEditing)
                    gridView.CloseEditor();

                if (gridView.FocusedRowModified)
                    gridView.UpdateCurrentRow();

                result = (List<HIS.UC.ServiceReqDetail.ServiceReqDetailADO>)gridControl.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCAccount_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    ProcessColumn();
                    //SetVisibleSearchPanel();
                    UpdateDataSource();
                    SetDataToCommonControl();
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
                gridControl.BeginUpdate();
                gridControl.DataSource = this.initADO.ListServiceReqDetailADO;
                gridControl.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToCommonControl()
        {
            try
            {
                if (this.initADO.ServiceReqDetailCommonADO != null)
                {
                    lblExpMestCode.Text = this.initADO.ServiceReqDetailCommonADO.EXP_MEST_CODE;
                    lblMediStockName.Text = this.initADO.ServiceReqDetailCommonADO.MEDI_STOCK_NAME;
                }
                else
                {
                    lblMediStockName.Text = "";
                    lblExpMestCode.Text = "";
                   
                }
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
                if (this.initADO.ListServiceReqDetailColumn != null && this.initADO.ListServiceReqDetailColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListServiceReqDetailColumn)
                    {
                        GridColumn col = gridView.Columns.AddField(item.FieldName);
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

        private void gridViewAccount_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (ServiceReqDetailADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null && this.gridView_CustomUnboundColumnData != null)
                    {
                        this.gridView_CustomUnboundColumnData(data, e);
                    }
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload(List<ServiceReqDetailADO> data, ServiceReqDetailCommonADO common)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListServiceReqDetailADO = data;
                    this.initADO.ServiceReqDetailCommonADO = common;
                    UpdateDataSource();
                    SetDataToCommonControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewCashCollect_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ServiceReqDetailADO)gridView.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "check2")
                        {
                            if (data.isKeyChoose1)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }
                        if (e.Column.FieldName == "radio2")
                        {
                            if (data.isKeyChoose1)
                            {
                                e.RepositoryItem = repositoryItemRadio_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemRadio_Disable;
                            }

                            //var countCheck = this.initADO.ListAccount.Where(o => o.radio2 == true).Count();
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
                var row = (ServiceReqDetailADO)gridView.GetFocusedRow();
                foreach (var item in this.initADO.ListServiceReqDetailADO)
                {
                }
                gridControl.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click1 != null)
                {
                    this.btn_Radio_Enable_Click1(row);
                }

                gridView.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAccount_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (ServiceReqDetailADO)gridView.GetFocusedRow();
                if (row != null && this.gridViewAccount_MouseDownAccount != null)
                {
                    this.gridViewAccount_MouseDownAccount(sender, e);
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
                var row = (ServiceReqDetailADO)gridView.GetFocusedRow();
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

        private void gridViewAccount_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentAccount = (HIS.UC.ServiceReqDetail.ServiceReqDetailADO)gridView.GetFocusedRow();

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell && (this.initADO.IsShowMenuPopup == true || this.gridView_MouseRightClick != null))
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
                if ((e.Item is BarButtonItem) && this.currentAccount != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.Copy:
                            {
                                this.gridView_MouseRightClick(this.currentAccount, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.Paste:
                            {
                                this.gridView_MouseRightClick(this.currentAccount, e);
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

        private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ServiceReqDetailADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null && this.grid_RowCellStyle != null)
                    {
                        grid_RowCellStyle(data, e);
                    }
                    if (data != null)
                    {
                        if (data.TYPE == 1)
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else if (data.TYPE == 2)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else if (data.TYPE == 3)
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
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
