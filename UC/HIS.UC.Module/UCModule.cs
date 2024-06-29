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
using HIS.UC.Module.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using ACS.EFMODEL.DataModels;
using HIS.UC.Module.Popup;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.Module
{
    public partial class UCModule : UserControl
    {
        ModuleInitADO initADO = null;
        Inventec.Desktop.Common.Modules.Module moduleData;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 = null;
        gridViewModule_MouseDownModule gridViewModule_MouseDownModule = null;

        HIS.UC.Module.ModuleADO currentFocusAdo;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;
        GridView_MouseRightClick gridView_MouseRightClick = null;

        bool isShowSearchPanel;

        public UCModule(ModuleInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ModuleGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click1 = ado.btn_Radio_Enable_Click1;
                this.gridViewModule_MouseDownModule = ado.gridViewModule_MouseDownModule;
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
                result = (List<HIS.UC.Module.ModuleADO>)gridControlModule.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCModule_Load(object sender, EventArgs e)
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
                gridControlModule.BeginUpdate();
                gridControlModule.DataSource = this.initADO.ListModule;
                gridControlModule.EndUpdate();
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
                if (this.initADO.ListModuleColumn != null && this.initADO.ListModuleColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListModuleColumn)
                    {
                        GridColumn col = gridViewModule.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        //col.OptionsColumn.AllowSort = item.AllowSort;
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

        private void gridViewModule_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (ACS_MODULE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ModuleADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListModule = data;
                    UpdateDataSource();
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
                    var data = (ModuleADO)gridViewModule.GetRow(e.RowHandle);
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
                var row = (ACS_MODULE)gridViewModule.GetFocusedRow();
                foreach (var item in this.initADO.ListModule)
                {
                    if (item.ID == row.ID)
                    {
                        item.radio2 = true;
                    }
                    else
                    {
                        item.radio2 = false;
                    }
                }

                gridControlModule.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click1 != null)
                {
                    this.btn_Radio_Enable_Click1(row);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewModule_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (ACS_MODULE)gridViewModule.GetFocusedRow();
                if (row != null && this.gridViewModule_MouseDownModule != null)
                {
                    this.gridViewModule_MouseDownModule(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewListMedicineType_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentFocusAdo = (HIS.UC.Module.ModuleADO)gridViewModule.GetFocusedRow();

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
                if ((e.Item is BarButtonItem) && this.currentFocusAdo != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.Copy:
                            {
                                this.gridView_MouseRightClick(this.currentFocusAdo, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.Paste:
                            {
                                this.gridView_MouseRightClick(this.currentFocusAdo, e);
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
