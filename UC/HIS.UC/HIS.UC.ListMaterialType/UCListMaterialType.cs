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
using HIS.UC.ListMaterialType.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using HIS.UC.ListMaterialType.Popup;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.UC.ListMaterialType
{
    public partial class UCListMaterialType : UserControl
    {
        ListMaterialTypeInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        gridViewMaty_MouseDownMaty gridViewMaty_MouseDownMaty = null;
        bool isShowSearchPanel;

        HIS.UC.ListMaterialType.ListMaterialTypeADO currentFocusAdo;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;
        GridView_MouseRightClick gridView_MouseRightClick = null;
        ProcessUpdateTrustAmount processUpdateTrustAmount = null;

        public UCListMaterialType(ListMaterialTypeInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ListMaterialTypeGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                this.gridViewMaty_MouseDownMaty = ado.gridViewMaty_MouseDownMaty;
                this.gridView_MouseRightClick = ado.gridView_MouseRightClick;
                this.processUpdateTrustAmount = ado.processUpdateTrustAmount;
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
                result = (List<HIS.UC.ListMaterialType.ListMaterialTypeADO>)gridControlListMaterialType.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UCListMaterialType_Load(object sender, EventArgs e)
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
                gridControlListMaterialType.BeginUpdate();
                gridControlListMaterialType.DataSource = this.initADO.ListMaterialType;
                gridControlListMaterialType.EndUpdate();
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
                if (this.initADO.ListMaterialTypeColumn != null && this.initADO.ListMaterialTypeColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListMaterialTypeColumn)
                    {
                        GridColumn col = gridViewListMaterialType.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
                        col.ToolTip = item.ToolTip;
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

        private void gridViewListMaterialType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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

        internal void Reload(List<ListMaterialTypeADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListMaterialType = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal void ReloadRow(HIS_MEDI_STOCK_MATY data)
        {
            try
            {
                if (data != null)
                {
                    foreach (var item in this.initADO.ListMaterialType)
                    {
                        if (item.ID == data.MATERIAL_TYPE_ID)
                        {
                            item.ALERT_MIN_IN_STOCK_STR = data.ALERT_MIN_IN_STOCK;
                            item.ALERT_MAX_IN_STOCK_STR = data.ALERT_MAX_IN_STOCK;
                            if (data.IS_GOODS_RESTRICT.HasValue)
                                item.IS_GOODS_RESTRICT = data.IS_GOODS_RESTRICT.Value == 1 ? true : false;
                            if (data.IS_PREVENT_MAX.HasValue)
                                item.IS_PREVENT_MAX = data.IS_PREVENT_MAX.Value == 1 ? true : false;
                            break;
                        }
                    }

                    gridControlListMaterialType.RefreshDataSource();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewCashCollect_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ListMaterialTypeADO)gridViewListMaterialType.GetRow(e.RowHandle);
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

                        if (e.Column.FieldName == "updateTrustAmount")
                        {
                            e.RepositoryItem = repositoryItemButtonEditUpdateTrustAmount;
                        }
                        if (e.Column.FieldName == "IS_GOODS_RESTRICT")
                        {
                            e.RepositoryItem = repositoryItemCheckEditIsGoodsRetrict;

                        }
                        if (e.Column.FieldName == "ALERT_MIN_IN_STOCK_STR")
                        {
                            e.RepositoryItem = repositoryItemSpinEdit1;
                        }
                        if (e.Column.FieldName == "ALERT_MAX_IN_STOCK_STR")
                        {
                            e.RepositoryItem = repositoryItemSpinEdit2;
                        }
                        if (e.Column.FieldName == "EXP_MEDI_STOCK_ID")
                        {
                            e.RepositoryItem = cboKhoXuat;
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
                var row = (V_HIS_MATERIAL_TYPE)gridViewListMaterialType.GetFocusedRow();
                foreach (var item in this.initADO.ListMaterialType)
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

                gridControlListMaterialType.RefreshDataSource();

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

        private void UCListMaterialType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var row = (V_HIS_MATERIAL_TYPE)gridViewListMaterialType.GetFocusedRow();
                if (row != null && this.gridViewMaty_MouseDownMaty != null)
                {
                    this.gridViewMaty_MouseDownMaty(sender, e);
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
                currentFocusAdo = (HIS.UC.ListMaterialType.ListMaterialTypeADO)gridViewListMaterialType.GetFocusedRow();

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell && this.gridView_MouseRightClick != null)
                {
                    if (this.barManager1 == null)
                        this.barManager1 = new DevExpress.XtraBars.BarManager();
                    this.barManager1.Form = this;
                    popupMenuProcessor = new PopupMenuProcessor(this.barManager1, GridView_MouseRightClick);
                    if (currentFocusAdo.isKeyChoose)
                    {
                        this.popupMenuProcessor.InitMenu();
                    }
                    else
                    {
                        this.popupMenuProcessor.InitMenuU();
                    }
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
                        case PopupMenuProcessor.ItemType.UpdateTrustAmount:
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

        private void gridViewListMaterialType_ValidateRow(object sender,
DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
              /*  GridView view = sender as GridView;
                GridColumn inStockCol = view.Columns["ALERT_MAX_IN_STOCK_STR"];
                GridColumn onOrderCol = view.Columns["TRUST_AMOUNT_IN_STOCK_STR"];
                //Get the value of the first column 
                decimal? inSt = (decimal?)view.GetRowCellValue(e.RowHandle, inStockCol);
                //Get the value of the second column 
                decimal? onOrd = (decimal?)view.GetRowCellValue(e.RowHandle, onOrderCol);

                ListMaterialTypeADO row = (ListMaterialTypeADO)view.GetRow(e.RowHandle);
                //Validity criterion 
                if (inSt != null && onOrd != null)
                {
                    if (inSt < onOrd)
                    {
                        //view.SetRowCellValue(e.RowHandle, inStockCol, null);
                        e.Valid = false;
                        //Set errors with specific descriptions for the columns 
                        view.SetColumnError(inStockCol, "Không cho phép nhập \"Cơ số\" nhỏ hơn \"Cơ số thực tế\"", ErrorType.Warning);
                        foreach (var item in this.initADO.ListMaterialType)
                        {
                            if (item.ID == row.ID)
                            {
                                item.TRUST_AMOUNT_IN_STOCK_STR = null;
                                break;
                            }
                        }
                        gridControlListMaterialType.RefreshDataSource();

                    }
                }
                else if (inSt != null && onOrd == null)
                {
                    //view.SetRowCellValue(e.RowHandle, inStockCol, null);
                    e.Valid = false;
                    //Set errors with specific descriptions for the columns 
                    view.SetColumnError(inStockCol, "Bạn phải thực hiện cập nhật \"Cơ số thực tế\" trước khi nhập \"Cơ số\"", ErrorType.Warning);
                    foreach (var item in this.initADO.ListMaterialType)
                    {
                        if (item.ID == row.ID)
                        {
                            item.TRUST_AMOUNT_IN_STOCK_STR = null;
                            break;
                        }
                    }
                    gridControlListMaterialType.RefreshDataSource();

                }*/
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        private void gridViewListMaterialType_InvalidRowException(object sender,
DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {

                GridView view = sender as GridView;
                //Suppress displaying the error message box 
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.Ignore;
                if (XtraMessageBox.Show("Dữ liệu không hợp lệ. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
                GridColumn inStockCol = view.Columns["ALERT_MAX_IN_STOCK_STR"];
                view.SetRowCellValue(e.RowHandle, inStockCol, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void repositoryItemButtonEditUpdateTrustAmount_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HIS.UC.ListMaterialType.ListMaterialTypeADO curentType = (HIS.UC.ListMaterialType.ListMaterialTypeADO)this.gridViewListMaterialType.GetFocusedRow();
                if (curentType != null)
                {
                    if (processUpdateTrustAmount != null)
                    {
                        this.processUpdateTrustAmount(curentType.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }

        }


        private void gridViewListMaterialType_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewListMaterialType.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlListMaterialType.DataSource as List<HIS.UC.ListMaterialType.ListMaterialTypeADO>;
                var row = listDatas[index];
                //Get the value of the first column 
                decimal? inSt = (decimal?)row.ALERT_MAX_IN_STOCK_STR;
                //Get the value of the second column 
                decimal? onOrd = (decimal?)row.TRUST_AMOUNT_IN_STOCK_STR;
                /*if (e.ColumnName == "ALERT_MAX_IN_STOCK_STR")
                {
                    if (inSt != null && onOrd != null)
                    {
                        if (inSt < onOrd)
                        {
                            e.Info.ErrorType = ErrorType.Warning;
                            e.Info.ErrorText = "Không cho phép nhập \"Cơ số\" nhỏ hơn \"Cơ số thực tế\"";

                        }
                    }
                    else if (inSt != null && onOrd == null)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Bạn phải thực hiện cập nhật \"Cơ số thực tế\" trước khi nhập \"Cơ số\"";

                    }

                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }*/

            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKhoXuat_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    var row = (V_HIS_MATERIAL_TYPE)gridViewListMaterialType.GetFocusedRow();
                    foreach (var item in this.initADO.ListMaterialType)
                    {

                        if (row.ID == item.ID)
                        {
                            item.EXP_MEDI_STOCK_ID = null;
                            break;
                        }
                    }

                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void LoaCboMediStock(List<V_HIS_MEST_ROOM> data, bool readonl)
        {
            try
            {
                cboKhoXuat.ReadOnly = readonl;
                this.initADO.LisMestRoom = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "Mã kho xuất", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "Tên kho xuất", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "MEDI_STOCK_ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboKhoXuat, this.initADO.LisMestRoom, controlEditorADO);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
