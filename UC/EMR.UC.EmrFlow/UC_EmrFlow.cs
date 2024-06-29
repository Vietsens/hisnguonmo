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
using EMR.EFMODEL.DataModels;
using EMR.UC.EmrFlow.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.UC.EmrFlow.Popup;
using DevExpress.XtraBars;

namespace EMR.UC.EmrFlow
{
    public partial class UCEmrFlow : UserControl
    {
        EmrFlowInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        gridViewEmrFlow_MouseDownEmrFlow gridViewEmrFlow_MouseDownEmrFlow;
        Rooom_MouseRightClick rooom_MouseRightClick;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;
        EMR.UC.EmrFlow.EmrFlowADO currentEmrFlow;
        PopupMenuProcessor.ItemType itemType;

        bool isShowSearchPanel;

        public UCEmrFlow(EmrFlowInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.EmrFlowGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                gridViewEmrFlow_MouseDownEmrFlow = ado.gridViewEmrFlow_MouseDownEmrFlow;
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
                result = (List<EMR.UC.EmrFlow.EmrFlowADO>)gridControlEmrFlow.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCEmrFlow_Load(object sender, EventArgs e)
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
                result = this.gridControlEmrFlow;
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
                gridControlEmrFlow.BeginUpdate();
                gridControlEmrFlow.DataSource = this.initADO.ListEmrFlow;
                gridControlEmrFlow.EndUpdate();
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
                if (this.initADO.ListEmrFlowColumn != null && this.initADO.ListEmrFlowColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListEmrFlowColumn)
                    {
                        GridColumn col = gridViewEmrFlow.Columns.AddField(item.FieldName);
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

        private void gridViewEmrFlow_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (V_EMR_FLOW)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<EmrFlowADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListEmrFlow = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewEmrFlow_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (EmrFlowADO)gridViewEmrFlow.GetRow(e.RowHandle);
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
                var row = (V_EMR_FLOW)gridViewEmrFlow.GetFocusedRow();
                foreach (var item in this.initADO.ListEmrFlow)
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

                gridControlEmrFlow.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click != null)
                {
                    this.btn_Radio_Enable_Click(row);
                }

                gridViewEmrFlow.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewEmrFlow_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (V_EMR_FLOW)gridViewEmrFlow.GetFocusedRow();
                if (row != null && this.gridViewEmrFlow_MouseDownEmrFlow != null)
                {
                    this.gridViewEmrFlow_MouseDownEmrFlow(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewEmrFlow_PopupMenuShowing_1(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentEmrFlow = (EMR.UC.EmrFlow.EmrFlowADO)gridViewEmrFlow.GetFocusedRow();

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
                                this.rooom_MouseRightClick(currentEmrFlow, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.PastePhongSangPhong:
                            {
                                this.rooom_MouseRightClick(currentEmrFlow, e);
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
