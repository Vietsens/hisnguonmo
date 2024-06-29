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
using HIS.UC.RehaServiceType.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;

namespace HIS.UC.RehaServiceType
{
    public partial class UCRehaServiceType : UserControl
    {
        RehaServiceTypeInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 = null;
        gridViewRehaServiceType_MouseDownRehaService gridViewRehaServiceType_MouseDownRehaService = null;

        bool isShowSearchPanel;

        public UCRehaServiceType(RehaServiceTypeInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.RehaServiceTypeGrid_CustomUnboundColumnData;
                gridViewRehaServiceType_MouseDownRehaService = ado.gridViewRehaServiceType_MouseDownRehaService;
                btn_Radio_Enable_Click1 = ado.btn_Radio_Enable_Click1;
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
                result = (List<HIS.UC.RehaServiceType.RehaServiceTypeADO>)gridControlRehaServiceType.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCRehaServiceType_Load(object sender, EventArgs e)
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
                gridControlRehaServiceType.BeginUpdate();
                gridControlRehaServiceType.DataSource = this.initADO.ListRehaServiceType;
                gridControlRehaServiceType.EndUpdate();
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
                if (this.initADO.ListRehaServiceTypeColumn != null && this.initADO.ListRehaServiceTypeColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListRehaServiceTypeColumn)
                    {
                        GridColumn col = gridViewRehaServiceType.Columns.AddField(item.FieldName);
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

        private void gridViewRehaServiceType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (V_HIS_REHA_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<RehaServiceTypeADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListRehaServiceType = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRehaServiceType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (RehaServiceTypeADO)gridViewRehaServiceType.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "checkRehaService")
                        {
                            if (data.isKeyChooseRehaService)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }
                        if (e.Column.FieldName == "radioRehaService")
                        {
                            if (data.isKeyChooseRehaService)
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
                var row = (V_HIS_REHA_SERVICE_TYPE)gridViewRehaServiceType.GetFocusedRow();
                foreach (var item in this.initADO.ListRehaServiceType)
                {
                    if (item.ID == row.ID)
                    {
                        item.radioRehaService = true;
                    }
                    else
                    {
                        item.radioRehaService = false;
                    }
                }

                gridControlRehaServiceType.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click1 != null)
                {
                    this.btn_Radio_Enable_Click1(row);
                }
                gridViewRehaServiceType.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRehaServiceType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (V_HIS_REHA_SERVICE_TYPE)gridViewRehaServiceType.GetFocusedRow();
                if (row != null && this.gridViewRehaServiceType_MouseDownRehaService != null)
                {
                    this.gridViewRehaServiceType_MouseDownRehaService(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
