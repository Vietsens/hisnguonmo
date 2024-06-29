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
using HIS.UC.HisServiceType.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.UC.HisServiceType
{
    public partial class UCHisServiceType : UserControl
    {
        HisServiceTypeInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        gridViewHisServiceType_MouseDownHisServiceType gridViewHisServiceType_MouseDownHisServiceType = null;

        bool isShowSearchPanel;

        public UCHisServiceType(HisServiceTypeInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.HisServiceTypeGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                this.gridViewHisServiceType_MouseDownHisServiceType = ado.gridViewHisServiceType_MouseDownHisServiceType;
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
                result = (List<HIS.UC.HisServiceType.HisServiceTypeADO>)gridControlHisServiceType.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCHisServiceType_Load(object sender, EventArgs e)
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
                gridControlHisServiceType.BeginUpdate();
                gridControlHisServiceType.DataSource = this.initADO.ListHisServiceType;
                gridControlHisServiceType.EndUpdate();
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
                if (this.initADO.ListHisServiceTypeColumn != null && this.initADO.ListHisServiceTypeColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListHisServiceTypeColumn)
                    {
                        GridColumn col = gridViewHisServiceType.Columns.AddField(item.FieldName);
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

        private void gridViewHisServiceType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (HIS_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<HisServiceTypeADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListHisServiceType = data;
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
                    var data = (HisServiceTypeADO)gridViewHisServiceType.GetRow(e.RowHandle);
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
                var row = (HIS_SERVICE_TYPE)gridViewHisServiceType.GetFocusedRow();
                foreach (var item in this.initADO.ListHisServiceType)
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

                gridControlHisServiceType.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click != null)
                {
                    this.btn_Radio_Enable_Click(row);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisServiceType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (HIS_SERVICE_TYPE)gridViewHisServiceType.GetFocusedRow();
                if (row != null && this.gridViewHisServiceType_MouseDownHisServiceType != null)
                {
                    this.gridViewHisServiceType_MouseDownHisServiceType(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
