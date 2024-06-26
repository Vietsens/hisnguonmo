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
using HIS.UC.ServiceGroup.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.UC.ServiceGroup
{
    public partial class UCServiceGroup : UserControl
    {
        ServiceGroupInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 = null;
        gridViewServiceGroup_MouseDownMest gridViewServiceGroup_MouseDownMest = null;
        Spin_EditValueChanged spin_EditValueChanged = null;
        Check_CheckedChanged check_CheckedChanged = null;
        ServiceGroupGridView_Click serviceGroupGridView_Click = null;
        LockItem_Click lockItem_Click = null;
        UnLockItem_Click unLockItem_Click = null;
        DeleteItem_Click deleteItem_Click = null;

        bool isShowSearchPanel;

        public UCServiceGroup(ServiceGroupInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ServiceGroupGrid_CustomUnboundColumnData;
                gridViewServiceGroup_MouseDownMest = ado.gridViewServiceGroup_MouseDownMest;
                btn_Radio_Enable_Click1 = ado.btn_Radio_Enable_Click1;
                serviceGroupGridView_Click = ado.serviceGroupGridView_Click;
                spin_EditValueChanged = ado.spin_EditValueChanged;
                check_CheckedChanged = ado.check_CheckedChanged;
                lockItem_Click = ado.lockItem_Click;
                unLockItem_Click = ado.unLockItem_Click;
                deleteItem_Click = ado.deleteItem_Click;

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
                result = (List<HIS.UC.ServiceGroup.ServiceGroupADO>)gridControlServiceGroup.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCServiceGroup_Load(object sender, EventArgs e)
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
                result = this.gridControlServiceGroup;
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
                gridControlServiceGroup.BeginUpdate();
                gridControlServiceGroup.DataSource = this.initADO.ListServiceGroup;
                gridControlServiceGroup.EndUpdate();
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
                if (this.initADO.ListServiceGroupColumn != null && this.initADO.ListServiceGroupColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListServiceGroupColumn)
                    {
                        GridColumn col = gridViewServiceGroup.Columns.AddField(item.FieldName);
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

        private void gridViewServiceGroup_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (ServiceGroupADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ServiceGroupADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListServiceGroup = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceGroup_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ServiceGroupADO)gridViewServiceGroup.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "checkGroup")
                        {
                            if (data.isKeyChooseGroup)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }
                        else if (e.Column.FieldName == "radioGroup")
                        {
                            if (!data.isKeyChooseGroup)
                            {
                                e.RepositoryItem = repositoryItemRadio_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemRadio_Enable;
                            }

                        }
                        else if (e.Column.FieldName == "AMOUNT")
                        {
                            if (data.isKeyChooseGroup)
                            {
                                e.RepositoryItem = SpinEditAmountDisable;
                            }
                            else
                            {
                                e.RepositoryItem = SpinEditAmountEnable;
                            }
                        }
                        else if (e.Column.FieldName == "LOCK_UNLOCK")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = ButtonEditUnLock;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEditLock;
                            }
                        }
                        else if (e.Column.FieldName == "DELETE_ITEM")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = ButtonEditDeleteEnable;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEditDeleteDisable;
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
                var erRow = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
                foreach (var item in this.initADO.ListServiceGroup)
                {
                    if (item.ID == erRow.ID)
                    {
                        item.radioGroup = true;
                    }
                    else
                    {
                        item.radioGroup = false;
                    }
                }

                gridControlServiceGroup.RefreshDataSource();

                if (erRow != null && this.btn_Radio_Enable_Click1 != null)
                {
                    this.btn_Radio_Enable_Click1(erRow, erRow);
                }
                gridViewServiceGroup.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceGroup_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell && hi.Column.FieldName == "checkGroup")
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }

                var row = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
                if (row != null && this.gridViewServiceGroup_MouseDownMest != null)
                {
                    this.gridViewServiceGroup_MouseDownMest(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpinEditAmountEnable_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
                if (row != null && this.spin_EditValueChanged != null)
                {
                    this.spin_EditValueChanged(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck__Enable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gridViewServiceGroup_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
                if (row != null && serviceGroupGridView_Click != null)
                {
                    serviceGroupGridView_Click(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
                if (row != null && lockItem_Click != null)
                {
                    lockItem_Click(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
                if (row != null && unLockItem_Click != null)
                {
                    unLockItem_Click(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
                if (row != null && deleteItem_Click != null)
                {
                    deleteItem_Click(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceGroup_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "checkGroup")
                {
                    gridViewServiceGroup.BeginUpdate();
                    long serviceGroupId = Inventec.Common.TypeConvert.Parse.ToInt64(gridViewServiceGroup.GetRowCellValue(e.RowHandle, "ID").ToString());
                    bool checkGroup = Inventec.Common.TypeConvert.Parse.ToBoolean(gridViewServiceGroup.GetRowCellValue(e.RowHandle, "checkGroup").ToString());
                    var row = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
                    if (row != null && this.check_CheckedChanged != null)
                    {
                        row.checkGroup = checkGroup;
                        this.check_CheckedChanged(row);
                    }
                    gridViewServiceGroup.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck__Enable_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    var row = (ServiceGroupADO)gridViewServiceGroup.GetFocusedRow();
            //    if (row != null && this.check_CheckedChanged != null)
            //    {
            //        this.check_CheckedChanged(row);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }
    }
}
