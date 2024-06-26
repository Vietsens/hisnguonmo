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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExpMestChmsCreate.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestChmsCreate
{
    public partial class UCExpMestChmsCreate : HIS.Desktop.Utility.UserControlBase
    {

        private void cboImpMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExpMediStock.Focus();
                    txtExpMediStock.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ResetValueControlCommon1();
                //ResetGridControlDetail();
                if (cboImpMediStock.EditValue != null)
                {

                    stock = listImpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue));
                    if (stock != null && stock.IS_GOODS_RESTRICT != null) 
                    {
                        if (stock.IS_GOODS_RESTRICT == 1)
                        {
                            List<V_HIS_MEDI_STOCK_MATY> material = new List<V_HIS_MEDI_STOCK_MATY>();
                            List<V_HIS_MEDI_STOCK_METY> medicine = new List<V_HIS_MEDI_STOCK_METY>();
                            HisMediStockMatyViewFilter matyFilter = new HisMediStockMatyViewFilter();
                            matyFilter.MEDI_STOCK_ID = stock.ID;
                            material = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_MATY>>(
                                "api/HisMediStockMaty/GetView", ApiConsumers.MosConsumer, matyFilter, null);
                            material = material.Where(o => o.IS_GOODS_RESTRICT == 1).ToList();
                            if (material != null && material.Count > 0)
                            {
                                materialTypeIds = material.Select(o => o.MATERIAL_TYPE_ID).ToList();
                            }

                            HisMediStockMetyViewFilter metyFilter = new HisMediStockMetyViewFilter();
                            metyFilter.MEDI_STOCK_ID = stock.ID;
                            medicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_METY>>(
                               "api/HisMediStockMety/GetView", ApiConsumers.MosConsumer, metyFilter, null);
                            medicine = medicine.Where(o => o.IS_GOODS_RESTRICT == 1).ToList();
                            if (medicine != null && medicine.Count > 0)
                            {
                                medicineTypeIds = medicine.Select(o => o.MEDICINE_TYPE_ID).ToList();
                            }
                        }
                        // LoadDataToTreeList(mestRoom);
                       
                    }
                    FillDataToTrees();
                }
                if (stock != null)
                {
                    txtImpMediStock.Properties.Buttons[1].Visible = true;
                    txtImpMediStock.Text = stock.MEDI_STOCK_NAME;
                    dxValidationProvider1.RemoveControlError(txtImpMediStock);
                }
                else
                {
                    txtImpMediStock.Properties.Buttons[1].Visible = false;
                    txtImpMediStock.Text = "";
                }
                //FillDataToGridExpMest();
                //if (cboExpMediStock.EditValue == null)
                //{
                //    //LoadDataToTreeList(null);
                //    FillDataToTrees();
                //}
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMediStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtImpMediStock.Text))
                    {
                        string key = txtImpMediStock.Text.ToLower();
                        var listData = listImpMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboImpMediStock.EditValue = listData.First().ID;
                        }
                    }
                    if (!valid)
                    {
                        cboImpMediStock.Focus();
                        cboImpMediStock.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMediStock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    cboImpMediStock.Focus();
                    cboImpMediStock.ShowPopup();
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImpMediStock.EditValue = null;
                    txtImpMediStock.Text = "";
                    //LoadDataToTreeList(null);
                    FillDataToTrees();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtImpMediStock.EditValue != txtImpMediStock.OldEditValue && String.IsNullOrEmpty(txtImpMediStock.Text) && cboImpMediStock.EditValue != null)
                {
                    cboImpMediStock.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //V_HIS_MEDI_STOCK mestRoom = null;              
                this.mestRoom = null;
                if (radioExport.Checked) 
                {
                    if (cboExpMediStock.EditValue != null)
                    {
                        mestRoom = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                    }
                    if (mestRoom != null)
                    {
                        txtExpMediStock.Properties.Buttons[1].Visible = true;
                        txtExpMediStock.Text = mestRoom.MEDI_STOCK_NAME;
                        dxValidationProvider1.RemoveControlError(txtExpMediStock);
                    }
                    else
                    {
                        txtExpMediStock.Properties.Buttons[1].Visible = false;
                        txtExpMediStock.Text = "";
                    }
                    //LoadDataToTreeList(mestRoom);

                    //ResetGridControlDetail();
                    FillDataToTrees();
                }
               

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMediStock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    cboExpMediStock.Focus();
                    cboExpMediStock.ShowPopup();
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExpMediStock.EditValue = null;
                    txtExpMediStock.Text = "";
                    //LoadDataToTreeList(null);
                    //if (chkPlanningExport.Checked == true)
                    //{
                    //    FillDataToTrees();
                    //}
                    //else
                    if (!chkPlanningExport.Checked)
                    {
                        GridCheckMarksSelection gridCheckMark = cboExpMediStock.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMark.ClearSelection(this.gridViewExpMestChmsDetail);

                    }
                    FillDataToTrees();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMediStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    if (chkPlanningExport.Checked)
                    {
                        bool valid = false;
                        if (!String.IsNullOrEmpty(txtExpMediStock.Text))
                        {
                            string key = txtExpMediStock.Text.ToLower();
                            var listData = listExpMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                            if (listData != null && listData.Count == 1)
                            {
                                valid = true;
                                cboExpMediStock.EditValue = listData.First().ID;
                            }
                        }
                        if (!valid)
                        {
                            cboExpMediStock.Focus();
                            cboExpMediStock.ShowPopup();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtExpMediStock.EditValue != txtExpMediStock.OldEditValue && String.IsNullOrEmpty(txtExpMediStock.Text) && cboExpMediStock.EditValue != null)
                {
                    cboExpMediStock.EditValue = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (xtraTabControlMain.SelectedTabPageIndex == 0)
                    {
                        txtSearchMedicine.Focus();
                    }
                    else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                    {
                        txtSearchMaterial.Focus();
                    }
                    else
                    {
                        txtSearch.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExpAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControlMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControlMain.SelectedTabPageIndex == 0)
                {
                    txtSearchMedicine.Focus();
                    lciABO.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciRH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                {
                    txtSearchMaterial.Focus();
                    lciABO.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciRH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else if (xtraTabControlMain.SelectedTabPageIndex == 2)
                {
                    txtSearch.Focus();
                    lciABO.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciRH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioImport_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioImport.Checked == false)
                {
                    gridColumnKHOX.GroupIndex = -1;
                    gridColumnKHOVatTu.GroupIndex = -1;
                    gridColumnKhoMau.GroupIndex = -1;
                    gridColumnKHOX.VisibleIndex = 18;
                    gridColumnKHOVatTu.VisibleIndex = 18;
                    gridColumnKhoMau.VisibleIndex = 18;
                    gridColumnKHOX.Visible = false;
                    gridColumnKHOVatTu.Visible = false;
                    gridColumnKhoMau.Visible = false;
                    GridCheckMarksSelection gridCheckMark = cboExpMediStock.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboExpMediStock.Properties.View);
                        mestRoom_.Clear();
                    }
                }
                else
                {
                    gridColumnKHOX.GroupIndex = 0;
                    gridColumnKHOVatTu.GroupIndex = 0;
                    gridColumnKhoMau.GroupIndex = 0;
                    gridColumnKHOX.Visible = true;
                    gridColumnKHOVatTu.Visible = true;
                    gridColumnKhoMau.Visible = true;
                }
                
                LoadDataToCboMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioExport_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioExport.CheckState == CheckState.Checked)
                {
                    chkPlanningExport.CheckState = CheckState.Unchecked;
                    SetDafaultPlanningExport(true);
                }
                LoadDataToCboMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToTrees();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_DeleteRowMedicine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                gridViewMedicine.DeleteRow(gridViewMedicine.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_DeleteRowMaterial_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                gridViewMaterial.DeleteRow(gridViewMaterial.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (gridViewMaterial.FocusedRowHandle < 0 || gridViewMaterial.FocusedColumn.FieldName != "EXP_AMOUNT")
                    return;
                var data = (HisMaterialInStockADO)((IList)((BaseView)sender).DataSource)[gridViewMaterial.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.EXP_AMOUNT <= 0)
                    {
                        valid = false;
                        message = Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong;
                    }
                    else if (data.EXP_AMOUNT > data.AvailableAmount)
                    {
                        valid = false;
                        message = Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;
                    }
                    if (!valid)
                    {
                        gridViewMaterial.SetColumnError(gridViewMaterial.FocusedColumn, message);
                    }
                    else
                    {
                        gridViewMaterial.SetColumnError(gridViewMaterial.FocusedColumn, "", ErrorType.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (gridViewMedicine.FocusedRowHandle < 0 || gridViewMedicine.FocusedColumn.FieldName != "EXP_AMOUNT")
                    return;
                var data = (HisMedicineInStockADO)((IList)((BaseView)sender).DataSource)[gridViewMedicine.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.EXP_AMOUNT <= 0)
                    {
                        valid = false;
                        message = Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong;
                    }
                    else if (data.EXP_AMOUNT > data.AvailableAmount)
                    {
                        valid = false;
                        message = Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;
                    }
                    if (!valid)
                    {
                        gridViewMedicine.SetColumnError(gridViewMedicine.FocusedColumn, message);
                    }
                    else
                    {
                        gridViewMedicine.SetColumnError(gridViewMedicine.FocusedColumn, "", ErrorType.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.currentMediMate = null;
                    var row = (HisMedicineInStockADO)gridViewMedicine.GetFocusedRow();
                    if (row != null)
                    {
                        this.currentMediMate = new ADO.MediMateTypeADO(row);
                        ResetValueControlDetail();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMedicine.Focus();
                    this.gridViewMedicine.FocusedRowHandle = this.gridViewMedicine.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.currentMediMate = null;
                    var row = (HisMaterialInStockADO)gridViewMaterial.GetFocusedRow();
                    if (row != null)
                    {
                        this.currentMediMate = new ADO.MediMateTypeADO(row);
                        ResetValueControlDetail();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMaterial.Focus();
                    this.gridViewMaterial.FocusedRowHandle = this.gridViewMaterial.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPlanningExport_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPlanningExport.CheckState == CheckState.Checked)
                {
                    SetDafaultPlanningExport(true);
                   
                    // btnNew_Click(null, null);
                   
                    ResetValueControlCommon();
                    spinExpAmount.Enabled = true;
                    txtNote.Enabled = true;
                     LoadDataToCboMediStock();
                    LoadKhoXuat();
                }
                else
                {
                    SetDafaultPlanningExport(false);
                }
                FillDataToTrees();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool isCheckAll = false;
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            Inventec.Common.Logging.LogSystem.Debug("this.listMediInStock count: " + this.listMediInStock.Count());
                            var lstCheckAll = this.listMediInStock;

                            List<HisMedicineInStockADO> lstChecks = new List<HisMedicineInStockADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstCheckAll.Where(o => o.IsCheck == true).Count();
                                var ServiceNum = lstCheckAll.Count();
                                if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionExpMest.Images[1];
                                }

                                if (ServiceCheckedNum == ServiceNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionExpMest.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        var min = Math.Min(item.AvailableAmount ?? 0, item.ExportedTotalAmount ?? 0);
                                        item.EXP_AMOUNT = min;
                                        if (item.ID != null)
                                        {
                                            item.IsCheck = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        item.EXP_AMOUNT = 0;
                                        if (item.ID != null)
                                        {
                                            item.IsCheck = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }
                                gridControlMedicine.BeginUpdate();
                                gridViewMedicine.PostEditor();
                                gridControlMedicine.DataSource = lstChecks;
                                gridControlMedicine.EndUpdate();
                            }
                        }
                    }

                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            int rowHandle = gridViewMedicine.GetVisibleRowHandle(hi.RowHandle);
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            if (checkEdit == null)
                                return;

                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo1 = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo1.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo1.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
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
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                bool isCheckAll = false;
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            var lstCheckAll = this.listMateInStock;

                            List<HisMaterialInStockADO> lstChecks = new List<HisMaterialInStockADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstCheckAll.Where(o => o.IsCheck == true).Count();
                                var ServiceNum = lstCheckAll.Count();
                                if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionExpMest.Images[1];
                                }

                                if (ServiceCheckedNum == ServiceNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionExpMest.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        var min = Math.Min(item.AvailableAmount ?? 0, item.ExportedTotalAmount ?? 0);
                                        item.EXP_AMOUNT = min;
                                        if (item.ID != null)
                                        {
                                            item.IsCheck = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        item.EXP_AMOUNT = 0;
                                        if (item.ID != null)
                                        {
                                            item.IsCheck = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }
                                gridControlMaterial.BeginUpdate();
                                gridControlMaterial.DataSource = lstChecks;
                                gridControlMaterial.EndUpdate();
                            }
                        }
                    }

                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            int rowHandle = gridViewMedicine.GetVisibleRowHandle(hi.RowHandle);
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            if (checkEdit == null)
                                return;

                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo1 = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo1.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo1.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
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
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || e.RowHandle < 0)
                    return;
                if (e.Column.FieldName != "IsCheck")
                    return;
                decimal slDat = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["EXP_AMOUNT"]) ?? "").ToString());
                bool isCheck = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(e.RowHandle, view.Columns["IsCheck"]) ?? "").ToString());
                if (!isCheck)
                {
                    view.SetRowCellValue(e.RowHandle, view.Columns["EXP_AMOUNT"], 0);
                }
                else
                {
                    decimal khaDung = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["AvailableAmount"]) ?? "").ToString());
                    decimal slDaXuat = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["ExportedTotalAmount"]) ?? "").ToString());
                    var min = Math.Min(khaDung, slDaXuat);
                    view.SetRowCellValue(e.RowHandle, view.Columns["EXP_AMOUNT"], min);
                }
                gridViewMedicine.UpdateCurrentRow();
                gridViewMedicine.PostEditor();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || e.RowHandle < 0)
                    return;
                bool isCheck = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(e.RowHandle, view.Columns["IsCheck"]) ?? "").ToString());
                decimal slDat = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["EXP_AMOUNT"]) ?? "").ToString());
                decimal slDaXuat = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["ExportedTotalAmount"]) ?? "").ToString());
                if (isCheck && slDat < slDaXuat)
                {
                    e.Appearance.BackColor = Color.Yellow;
                }
                else
                {
                    e.Appearance.BackColor = Color.Transparent;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || e.RowHandle < 0)
                    return;
                if (e.Column.FieldName != "IsCheck")
                    return;
                bool isCheck = Inventec.Common.TypeConvert.Parse.ToBoolean(view.GetRowCellValue(e.RowHandle, view.Columns["IsCheck"]).ToString());
                if (!isCheck)
                {
                    view.SetRowCellValue(e.RowHandle, view.Columns["EXP_AMOUNT"], 0);
                }
                else
                {
                    decimal khaDung = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["AvailableAmount"]) ?? "").ToString());
                    decimal slDaXuat = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["ExportedTotalAmount"]) ?? "").ToString());
                    var min = Math.Min(khaDung, slDaXuat);
                    view.SetRowCellValue(e.RowHandle, view.Columns["EXP_AMOUNT"], min);
                }
                gridViewMedicine.UpdateCurrentRow();
                gridViewMedicine.PostEditor();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || e.RowHandle < 0)
                    return;
                bool isCheck = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(e.RowHandle, view.Columns["IsCheck"]) ?? "").ToString());
                decimal slDat = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["EXP_AMOUNT"]) ?? "").ToString());
                decimal slDaXuat = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.RowHandle, view.Columns["ExportedTotalAmount"]) ?? "").ToString());
                if (isCheck && slDat < slDaXuat)
                {
                    e.Appearance.BackColor = Color.Yellow;
                }
                else
                {
                    e.Appearance.BackColor = Color.Transparent;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
