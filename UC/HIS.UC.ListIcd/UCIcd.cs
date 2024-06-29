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
using HIS.UC.ListIcd.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.UC.ListIcd
{
    public partial class UCIcd : UserControl
    {
        IcdInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        gridViewIcd_MouseDownIcd gridViewIcd_MouseDownIcd;
        Delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick = null;
        Delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged = null;
        Delegate_repositoryItemButtonEdit_ContraindicationContent_Leave delegate_repositoryItemButtonEdit_ContraindicationContent_Leave = null;

        bool isShowSearchPanel;

        public UCIcd(IcdInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.IcdGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                gridViewIcd_MouseDownIcd = ado.gridViewIcd_MouseDownIcd;
                if (ado.IsShowSearchPanel.HasValue)
                {
                    this.isShowSearchPanel = ado.IsShowSearchPanel.Value;
                }
                this.delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick = ado.Delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick;
                this.delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged = ado.Delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged;
                this.delegate_repositoryItemButtonEdit_ContraindicationContent_Leave = ado.Delegate_repositoryItemButtonEdit_ContraindicationContent_Leave;
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
                result = (List<HIS.UC.ListIcd.IcdADO>)gridControlIcd.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCIcd_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    ProcessColumn();
                    UpdateDataSource();
                    SetCustomSizeForGridView(ref gridViewIcd);
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
                gridControlIcd.BeginUpdate();
                gridControlIcd.DataSource = this.initADO.ListIcd;
                gridControlIcd.EndUpdate();
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
                if (this.initADO.ListIcdColumn != null && this.initADO.ListIcdColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListIcdColumn)
                    {
                        GridColumn col = gridViewIcd.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
                        col.ToolTip = item.Tooltip;
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

        private void SetCustomSizeForGridView(ref DevExpress.XtraGrid.Views.Grid.GridView gridView)
        {
            try
            {
                if (this.initADO.ListIcdColumn != null && this.initADO.ListIcdColumn.Count > 0)
                {
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo info = gridView.GetViewInfo() as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo;
                    var listCols = this.initADO.ListIcdColumn;
                    if (info.Bounds.Width > listCols.Sum(o => o.ColumnWidth))
                    {
                        gridView.OptionsView.ColumnAutoWidth = true;
                    }
                    else
                    {
                        gridView.OptionsView.ColumnAutoWidth = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewIcd_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (HIS_ICD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<IcdADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListIcd = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewIcd_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (IcdADO)gridViewIcd.GetRow(e.RowHandle);
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
                        if (e.Column.FieldName == "check2")
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
                        if (e.Column.FieldName == "check3")
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
                        if (e.Column.FieldName == "CONTRAINDICATION_CONTENT")
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_ContraindicationContent;
                        }
                        if (e.Column.FieldName == "MIN_DURATION_STR2")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = repositoryItemSpinEdit__Min_Duration_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemSpinEdit__Min_Duration_Enable;
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
                var row = (HIS_ICD)gridViewIcd.GetFocusedRow();
                foreach (var item in this.initADO.ListIcd)
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

                gridControlIcd.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click != null)
                {
                    this.btn_Radio_Enable_Click(row);
                }

                gridViewIcd.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewIcd_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (HIS_ICD)gridViewIcd.GetFocusedRow();
                if (row != null && this.gridViewIcd_MouseDownIcd != null)
                {
                    this.gridViewIcd_MouseDownIcd(sender, e);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ContraindicationContent_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (IcdADO)gridViewIcd.GetFocusedRow();
                if (data != null && delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick != null)
                {
                    delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick(sender, e, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ContraindicationContent_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.ButtonEdit editor = sender as DevExpress.XtraEditors.ButtonEdit;
                var data = (IcdADO)gridViewIcd.GetFocusedRow();
                if (editor != null && editor.EditValue != null)
                {
                    data.CONTRAINDICATION_CONTENT = editor.EditValue.ToString();
                }
                if (data != null && delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged != null)
                {
                    delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged(sender, e, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ContraindicationContent_Leave(object sender, EventArgs e)
        {
            try
            {
                var data = (IcdADO)gridViewIcd.GetFocusedRow();
                if (data != null && delegate_repositoryItemButtonEdit_ContraindicationContent_Leave != null)
                {
                    delegate_repositoryItemButtonEdit_ContraindicationContent_Leave(sender, e, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewIcd_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (IcdADO)gridViewIcd.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "check1")
                        {
                            if (data.check1)
                            {
                                var item = this.initADO.ListIcd.FirstOrDefault(o => o.ID == data.ID);
                                item.check2 = false;
                                item.check3 = false;
                            }
                        }
                        else if (e.Column.FieldName == "check2")
                        {
                            if (data.check2)
                            {
                                var item = this.initADO.ListIcd.FirstOrDefault(o => o.ID == data.ID);
                                item.check1 = false;
                                item.check3 = false;
                            }
                        }
                        else if (e.Column.FieldName == "check3")
                        {
                            if (data.check3)
                            {
                                var item = this.initADO.ListIcd.FirstOrDefault(o => o.ID == data.ID);
                                item.check1 = false;
                                item.check2 = false;
                            }
                        }
                        gridControlIcd.RefreshDataSource();
                    }
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
                gridControlIcd.RefreshDataSource();
                gridViewIcd.PostEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void UCIcd_Resize(object sender, EventArgs e)
        {
            try
            {
                SetCustomSizeForGridView(ref gridViewIcd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

    }
}
