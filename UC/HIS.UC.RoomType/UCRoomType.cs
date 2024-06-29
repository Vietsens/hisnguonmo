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
using HIS.UC.Room.ADO;
using HIS.UC.RoomType.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ADO;

namespace HIS.UC.RoomType
{
    public partial class UCRoomType : UserControl
    {
        RoomTypeInitADO roomtypeinit = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        Grid_RowCellClick gridView_RowCellClick = null;
        bool isShowSearchPanel;

        public UCRoomType(RoomTypeInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.roomtypeinit = ado;
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoomType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                
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
                if (this.roomtypeinit.ListRoomTypeColumn != null && this.roomtypeinit.ListRoomTypeColumn.Count > 0)
                {
                    foreach (var item in this.roomtypeinit.ListRoomTypeColumn)
                    {
                        GridColumn col = gridViewRoomType.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
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

        private void gridControlRoomType_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.roomtypeinit != null)
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
                gridControlRoomType.BeginUpdate();
                gridControlRoomType.DataSource = this.roomtypeinit.ListRoomType;
                gridControlRoomType.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoomType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                //if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                //{
                //    var data = (V_HIS_ROOM_TYPE_MODULE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                //    if (data != null && this.gridViewRoomType != null)
                //    {
                //        this.gridView_CustomUnboundColumnData(data, e);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal void Reload(List<RoomTypeADO> data)
        {
            try
            {
                if (this.roomtypeinit != null)
                {
                    this.roomtypeinit.listRoomTypeADO = data;
                    UpdateDataSource();
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
                result = (List<RoomTypeADO>)gridControlRoomType.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void gridViewRoomType_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var data = (V_HIS_ROOM_TYPE_MODULE)gridViewRoomType.GetFocusedRow();

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
    }
}
