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
using HIS.UC.CashCollect;
using HIS.UC.CashCollect.ADO;
using MOS.EFMODEL.DataModels;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Core;
using Inventec.Common.Adapter;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;

namespace HIS.UC.CashCollect
{
    public partial class UCCashCollect : UserControl
    {
        CashCollectInitADO initADO = null;

        Grid_CustomUnboundColumnData gridViewTransaction_CustomUnboundColumnData = null;
        btn_Un_Collect_Click btn_Un_Collect_Click = null;
        check_changed check_Changed = null;
        check_all_click checkAll_Click = null;

        bool isShowSearchPanel;
        bool isCheckAll;
        const string CHECK_FIELD_NAME = "check";

        public UCCashCollect(CashCollectInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridViewTransaction_CustomUnboundColumnData = ado.CashCollectGrid_CustomUnboundColumnData;
                if (ado.IsShowSearchPanel.HasValue)
                {
                    this.isShowSearchPanel = ado.IsShowSearchPanel.Value;
                }
                if (ado.btn_Un_Collect_Click != null)
                {
                    this.btn_Un_Collect_Click = ado.btn_Un_Collect_Click;
                }
                if (ado.check_Changed != null)
                {
                    this.check_Changed = ado.check_Changed;
                }
                if (ado.CheckAll_Click != null)
                {
                    this.checkAll_Click = ado.CheckAll_Click;
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
                result = (List<HIS.UC.CashCollect.CashCollectADO>)gridControlCashCollect.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCCashCollect_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    ProcessColumn();
                    //SetVisibleSearchPanel();
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
                gridControlCashCollect.BeginUpdate();
                gridControlCashCollect.DataSource = this.initADO.ListCashCollect;
                gridControlCashCollect.EndUpdate();
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
                if (this.initADO.ListCashCollectColumn != null && this.initADO.ListCashCollectColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListCashCollectColumn)
                    {
                        GridColumn col = gridViewCashCollect.Columns.AddField(item.FieldName);
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
                        if (this.checkAll_Click != null && item.FieldName == CHECK_FIELD_NAME)
                        {
                            // Header click toggles select-all, so it must not sort the grid
                            col.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCashCollect_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null && this.gridViewTransaction_CustomUnboundColumnData != null)
                    {
                        this.gridViewTransaction_CustomUnboundColumnData(data, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
        //        SearchClick(strValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void SearchClick(string strValue)
        //{
        //    try
        //    {
        //        if (this.initADO != null && this.initADO.ListCashCollect != null)
        //        {
        //            List<CashCollectADO> listData = new List<CashCollectADO>();
        //            gridControlCashCollect.DataSource = null;
        //            if (!String.IsNullOrEmpty(strValue.Trim()))
        //            {
        //                string key = strValue.Trim().ToLower();

        //                listData = this.initADO.ListCashCollect.Where(o => o.ACCOUNT_BOOK_CODE.ToLower().Contains(key) || o.ACCOUNT_BOOK_NAME.ToLower().Contains(key)).ToList();
        //            }
        //            else
        //            {
        //                listData = this.initADO.ListCashCollect;
        //            }
        //            gridControlCashCollect.BeginUpdate();
        //            gridControlCashCollect.DataSource = listData;
        //            gridControlCashCollect.EndUpdate();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        internal void Reload(List<CashCollectADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListCashCollect = data;
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
                    var data = (V_HIS_TRANSACTION)gridViewCashCollect.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "check")
                        {
                            if (data.CASHOUT_ID != null)
                            {
                                e.RepositoryItem = repositoryItemCheck__Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck__Enable;
                            }
                        }

                        if (e.Column.FieldName == "Delete")
                        {
                            if (data.CASHOUT_ID == null)
                            {
                                e.RepositoryItem = repositoryItemButton_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButton_Enable;
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

        private void repositoryItemButton_Enable_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (V_HIS_TRANSACTION)gridViewCashCollect.GetFocusedRow();
                if (row != null)
                {
                    this.btn_Un_Collect_Click(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCashCollect_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsCheckAllHeaderClicked(sender as GridView, e))
            {
                ProcessCheckAllClick();
                (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                return;
            }

            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
            {
                GridView view = sender as GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.InRowCell)
                {
                    if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                    {
                        view.FocusedRowHandle = hi.RowHandle;
                        view.FocusedColumn = hi.Column;
                        view.ShowEditor();
                        CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                        if (checkEdit.Properties.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style2)
                        {
                            return;
                        }

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
        }

        private void gridViewCashCollect_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                gridControlCashCollect.RefreshDataSource();
                var dataGrid = (List<HIS.UC.CashCollect.CashCollectADO>)gridControlCashCollect.DataSource;
                List<HIS.UC.CashCollect.CashCollectADO> dataCheck = new List<HIS.UC.CashCollect.CashCollectADO>();
                dataCheck = dataGrid.Where(p => p.check == true).ToList();
                if (dataCheck != null && dataCheck.Count >= 0)
                {
                    this.check_Changed(dataCheck);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsCheckAllHeaderClicked(GridView view, MouseEventArgs e)
        {
            bool result = false;
            try
            {
                if (this.checkAll_Click != null && view != null && e.Button == MouseButtons.Left)
                {
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    result = hi.HitTest == GridHitTest.Column && hi.Column != null && hi.Column.FieldName == CHECK_FIELD_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessCheckAllClick()
        {
            try
            {
                gridViewCashCollect.CloseEditor();
                SetCheckAll(!this.isCheckAll);
                this.checkAll_Click(this.isCheckAll);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Sets the header select-all state without raising CheckAll_Click.</summary>
        internal void SetCheckAll(bool isCheckAll)
        {
            try
            {
                this.isCheckAll = isCheckAll;
                gridViewCashCollect.InvalidateColumnHeader(gridViewCashCollect.Columns.ColumnByFieldName(CHECK_FIELD_NAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCashCollect_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (this.checkAll_Click != null && e.Column != null && e.Column.FieldName == CHECK_FIELD_NAME)
                {
                    e.Info.InnerElements.Clear();
                    e.Painter.DrawObject(e.Info);
                    DrawCheckBox(e.Cache, repositoryItemCheck__Enable, e.Bounds, this.isCheckAll);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DrawCheckBox(DevExpress.Utils.Drawing.GraphicsCache cache, DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit edit, Rectangle r, bool isChecked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = isChecked;
            info.Bounds = r;
            info.CalcViewInfo(cache.Graphics);
            painter.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, r));
        }
    }
}
