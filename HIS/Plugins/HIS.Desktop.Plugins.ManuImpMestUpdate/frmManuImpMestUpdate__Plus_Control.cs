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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ManuImpMestUpdate.ADO;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Config;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Resources;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Validation;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType;
using HIS.UC.MedicineType;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ManuImpMestUpdate
{
    public partial class frmManuImpMestUpdate : HIS.Desktop.Utility.FormBase
    {
        private void chkImprice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkImprice.Checked == true)
                {
                    SetValueByServiceAdo();
                }
                else
                {
                    SetValueByServiceAdo();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDocumentNumber.Focus();
                    txtDocumentNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentSupplier = null;
                if (cboSupplier.EditValue != null)
                {
                    this.currentSupplier = this.listSupplier.FirstOrDefault(o => o.ID == Convert.ToInt64(cboSupplier.EditValue));
                    CommonParam param = new CommonParam();

                    var supplier = new BackendAdapter(param).Get<List<V_HIS_BID>>("api/HisBid/GetViewBySupplier", ApiConsumer.ApiConsumers.MosConsumer, this.currentSupplier.ID, param);

                    medicineProcessor.ReloadBid(this.ucMedicineTypeTree, supplier);
                    materialProcessor.ReloadBid(this.ucMaterialTypeTree, supplier);
                }
                //if (cboBid.EditValue != null)
                //{
                //    SetDataSourceGridControlMediMate();
                //}
                //cboBid.Enabled = true;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBid_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    checkOutBid.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkOutBid.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkOutBid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDocumentNumber.Focus();
                    txtDocumentNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void dtDocumentDate_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtDeliver.Focus();
                    txtDeliver.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDeliver_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDiscountRatio.Focus();
                    spinDiscountRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinDiscountRatio.Value > 0)
                    {
                        var totalPrice = listServiceADO.Sum(o => (o.IMP_AMOUNT * o.IMP_PRICE));
                        spinDiscountPrice.Value = totalPrice * (spinDiscountRatio.Value / 100);
                        spinDocumentPrice.Focus();
                        spinDocumentPrice.SelectAll();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var totalPrice = listServiceADO.Sum(o => (o.IMP_AMOUNT * o.IMP_PRICE));
                var discount = spinDiscountPrice.Value;
                if (totalPrice > 0)
                {
                    spinDiscountRatio.Value = (discount / totalPrice) * 100;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscountPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var totalPrice = listServiceADO.Sum(o => (o.IMP_AMOUNT * o.IMP_PRICE));
                    var discount = spinDiscountPrice.Value;
                    if (totalPrice > 0)
                    {
                        spinDiscountRatio.Value = (discount / totalPrice) * 100;
                    }
                    spinDocumentPrice.Focus();
                    spinDocumentPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDocumentPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
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

        private void cboSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                cboSupplier.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderLeft_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServicePaty_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                string error = GetError(gridViewServicePaty.FocusedRowHandle, gridViewServicePaty.FocusedColumn);
                if (error == string.Empty) return;
                gridViewServicePaty.SetColumnError(gridViewServicePaty.FocusedColumn, error, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServicePaty_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsNotSell)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string GetError(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                if (column.FieldName == "ExpVatRatio")
                {

                    VHisServicePatyADO data = (VHisServicePatyADO)gridViewServicePaty.GetRow(rowHandle);
                    if (data != null && (data.ExpVatRatio > 100) || data.ExpVatRatio < 0)
                        return "Giá trị không hợp lệ.";
                }
                else if (column.FieldName == "PRICE")
                {
                    VHisServicePatyADO data = (VHisServicePatyADO)gridViewServicePaty.GetRow(rowHandle);
                    if (data != null && data.PRICE < 0)
                        return "Giá trị không hợp lệ.";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return string.Empty;
        }

        public string GetErrorGridDetail(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                if (column.FieldName == "IMP_AMOUNT")
                {

                    VHisServiceADO data = (VHisServiceADO)gridViewImpMestDetail.GetRow(rowHandle);
                    if (data != null && (data.IMP_AMOUNT <= 0))
                        return "Giá trị không hợp lệ.";
                }
                //else if (column.FieldName == "IMP_PRICE")
                //{
                //    VHisServiceADO data = (VHisServiceADO)gridViewImpMestDetail.GetRow(rowHandle);
                //    if (data != null && data.IMP_PRICE < 0)
                //        return "Giá trị không hợp lệ.";
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return string.Empty;
        }

        private void gridViewServicePaty_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                BaseEditViewInfo info = ((DevExpress.XtraGrid.Views.Grid.ViewInfo.GridCellInfo)e.Cell).ViewInfo;
                string error = GetError(e.RowHandle, e.Column);
                SetError(info, error);
                info.CalcViewInfo(e.Graphics);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetError(BaseEditViewInfo cellInfo, string errorIconText)
        {
            try
            {
                if (errorIconText == string.Empty)
                {
                    cellInfo.ErrorIconText = null;
                    cellInfo.ShowErrorIcon = false;
                    return;
                }
                cellInfo.ErrorIconText = errorIconText;
                cellInfo.ShowErrorIcon = true;
                cellInfo.FillBackground = true;
                cellInfo.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServicePaty_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestDetail_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                BaseEditViewInfo info = ((DevExpress.XtraGrid.Views.Grid.ViewInfo.GridCellInfo)e.Cell).ViewInfo;
                string error = GetErrorGridDetail(e.RowHandle, e.Column);
                SetError(info, error);
                info.CalcViewInfo(e.Graphics);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestDetail_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                string error = GetErrorGridDetail(gridViewImpMestDetail.FocusedRowHandle, gridViewImpMestDetail.FocusedColumn);
                if (error == string.Empty) return;
                gridViewImpMestDetail.SetColumnError(gridViewImpMestDetail.FocusedColumn, error, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region shotcut
        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnUpdate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region left
        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpPrice.Focus();
                    spinImpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpVatRatio.Focus();
                    spinImpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinImpPriceVAT.Value = (spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));
                //UpdateServicePatyByImpPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExpiredDate.Focus();
                    txtExpiredDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVatRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinImpPriceVAT.Value = (spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));
                // UpdateServicePatyByImpPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Hạn Sử Dụng
        private void txtExpiredDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    //DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    //if (dt != null && dt.Value != DateTime.MinValue)
                    //{
                    //    dtExpiredDate.EditValue = dt;
                    //    dtExpiredDate.Update();
                    //}
                    //else
                    //{
                    //    dtExpiredDate.Visible = true;
                    //    dtExpiredDate.Focus();
                    //    dtExpiredDate.ShowPopup();
                    //}
                    dtExpiredDate.Visible = true;
                    dtExpiredDate.Focus();
                    dtExpiredDate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = Resources.ResourceMessage.NguoiDungNhapNgayKhongHopLe;
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_Leave(object sender, EventArgs e)
        {
            try
            {
                dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpiredDate.Text))
                    {
                        dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);

                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtExpiredDate.EditValue = dt;
                        dtExpiredDate.Update();

                    }
                    else
                    {
                        dtExpiredDate.Visible = true;
                        dtExpiredDate.Focus();
                        dtExpiredDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();

                if (!String.IsNullOrEmpty(currentValue))
                {
                    int day = Int16.Parse(currentValue.Substring(0, 2));
                    int month = Int16.Parse(currentValue.Substring(3, 2));
                    int year = Int16.Parse(currentValue.Substring(6, 4));
                    if (day < 0 || day > 31 || month < 0 || month > 12 || year < 1000 || year > DateTime.Now.Year)
                    {
                        //e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtExpiredDate.Visible = false;
                    txtExpiredDate.Text = dtExpiredDate.DateTime.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.Visible = false;
                    dtExpiredDate.Update();
                    txtExpiredDate.Text = dtExpiredDate.DateTime.ToString("dd/MM/yyyy");

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkSellByImpPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (chkSellByImpPrice.Checked)
                //{
                //    if (listServicePatyAdo != null && listServicePatyAdo.Count > 0)
                //    {
                //        foreach (var item in listServicePatyAdo)
                //        {
                //            if (item.IsNotSell)
                //                continue;
                //            item.PRICE = spinImpPrice.Value;
                //            item.ExpVatRatio = spinImpVatRatio.Value;
                //            item.VAT_RATIO = item.ExpVatRatio / 100;
                //        }
                //        gridControlServicePaty.DataSource = listServicePatyAdo;
                //        gridControlServicePaty.RefreshDataSource();
                //    }
                //}
                //else
                //{
                //    if (listServicePatyAdoFixed != null && listServicePatyAdoFixed.Count > 0)
                //    {
                //        AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                //        listServicePatyAdo = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(listServicePatyAdoFixed);
                //        gridControlServicePaty.DataSource = listServicePatyAdo;
                //        gridControlServicePaty.RefreshDataSource();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSellByImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPackageNumber.Focus();
                    txtPackageNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackageNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void gridViewServicePaty_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;

                var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "ExpVatRatio")
                    {
                        data.VAT_RATIO = data.ExpVatRatio / 100;
                        data.ExpPriceVat = data.ExpPriceVat * (1 + data.VAT_RATIO);
                        data.PRICE = (1 + data.PercentProfit / 100) * data.ExpPriceVat;
                        //data.PRICE = data.ExpPriceVat * (1 + data.PercentProfit / 100);
                    }
                    else if (e.Column.FieldName == "PercentProfit")
                    {
                        data.PRICE = (1 + data.PercentProfit / 100) * data.ExpPriceVat;
                    }
                    else if (e.Column.FieldName == "PRICE")
                    {
                        if (data.PRICE < data.ExpPriceVat)
                        {
                            data.PRICE = data.ExpPriceVat;
                        }
                        data.PercentProfit = 100 * (data.PRICE - data.ExpPriceVat) / data.ExpPriceVat;

                    }
                    else if (e.Column.FieldName == "IsNotSell")
                    {
                        gridControlServicePaty.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "PercentProfit")
                        {
                            try
                            {
                                if (data.IsNotSell)
                                {
                                    e.RepositoryItem = repositoryItemSpinPercentProfitD;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemSpinPercentProfitE;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        //else if (e.Column.FieldName == "ExpVatRatio")
                        //{
                        //    try
                        //    {
                        //        if (data.IsNotSell)
                        //        {
                        //            e.RepositoryItem = repositoryItemSpinExpVatRatioDisable;
                        //        }
                        //        else
                        //        {
                        //            e.RepositoryItem = repositoryItemSpinExpVatRatio;
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Inventec.Common.Logging.LogSystem.Error(ex);
                        //    }
                        //}

                        else if (e.Column.FieldName == "PRICE")
                        {
                            try
                            {
                                if (data.IsNotSell)
                                {
                                    e.RepositoryItem = repositoryItemSpinExpPriceE;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemSpinExpPriceE;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IsNotSell")
                        {
                            try
                            {
                                if (data.IsNotEdit)
                                {
                                    e.RepositoryItem = repositoryItemCheckIsNotSellDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemCheckIsNotSell;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridViewServicePaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "ExpPriceVat_str")
                        {
                            try
                            {
                                //e.Value = data.PRICE * (1 + data.VAT_RATIO);//nambg
                                e.Value = ConvertNumberToString(data.ExpPriceVat);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "PRICE_STR")
                        {
                            e.Value = ConvertNumberToString(data.PRICE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsNotSell)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (cboSupplier.EditValue != null && !checkOutBid.Checked)
                {
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                    this.currentSupplier = this.listSupplier.FirstOrDefault(o => o.ID == Convert.ToInt64(cboSupplier.EditValue));
                    CommonParam param = new CommonParam();

                    var supplier = new BackendAdapter(param).Get<List<V_HIS_BID>>("api/HisBid/GetViewBySupplier", ApiConsumer.ApiConsumers.MosConsumer, this.currentSupplier.ID, param);

                    medicineProcessor.ReloadBid(this.ucMedicineTypeTree, supplier);
                    materialProcessor.ReloadBid(this.ucMaterialTypeTree, supplier);
                }
                else
                {
                    medicineProcessor.ReloadBid(this.ucMedicineTypeTree, null);
                    materialProcessor.ReloadBid(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, false);
                    materialProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                }

                SetFocuTreeMediMate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region right
        private void cboImpMestType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(cboImpMestType.Text))
                    {
                        this.currentImpMestType = null;
                        listMediStock = new List<V_HIS_MEDI_STOCK>();
                        var key = cboImpMestType.Text.ToLower();
                        var listData = listImpMestType.Where(o => o.IMP_MEST_TYPE_CODE.ToLower().Contains(key) || o.IMP_MEST_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                            this.currentImpMestType = listData.First();
                            cboImpMestType.EditValue = this.currentImpMestType.ID;
                        }
                    }

                    if (this.currentImpMestType != null)
                    {
                        if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                        {
                            listMediStock = listMediStock.Where(o => o.IS_ALLOW_IMP_SUPPLIER == 1).ToList();
                        }
                        cboMediStock.Properties.DataSource = listMediStock;
                        cboMediStock.Focus();
                        cboMediStock.ShowPopup();
                    }
                    else
                    {
                        cboMediStock.Properties.DataSource = listMediStock;
                        cboMediStock.Focus();
                        cboMediStock.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediStock.Properties.DataSource == null)
                    {
                        cboImpSource.Focus();
                        cboImpSource.ShowPopup();
                    }
                    else
                    {
                        bool valid = false;
                        if (!String.IsNullOrEmpty(cboMediStock.Text))
                        {
                            var key = cboMediStock.Text.ToLower();
                            var listData = listMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                            if (listData != null && listData.Count == 1)
                            {
                                valid = true;
                                cboMediStock.EditValue = listData.First().ID;
                                cboImpSource.Focus();
                                cboImpSource.ShowPopup();
                            }
                        }
                        if (!valid)
                        {
                            cboMediStock.Focus();
                            cboMediStock.ShowPopup();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboImpSource.Focus();
                    cboImpSource.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboSupplier.Focus();
                    if (cboSupplier.Enabled == true)
                        cboSupplier.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboSupplier.Focus();
                    if (cboSupplier.Enabled == true)
                        cboSupplier.ShowPopup();
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
                    SetFocuTreeMediMate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestDetail_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                {
                    return;
                }
                var data = (VHisServiceADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "IMP_PRICE")
                    {
                        if (data.IsMedicine)
                        {
                            data.HisMedicine.IMP_PRICE = data.IMP_PRICE;
                        }
                        else
                        {
                            data.HisMaterial.IMP_PRICE = data.IMP_PRICE;
                        }
                    }
                    else if (e.Column.FieldName == "ImpVatRatio")
                    {
                        data.IMP_VAT_RATIO = data.ImpVatRatio / 100;
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        if (gridViewImpMestDetail.EditingValue is DateTime)
                        {
                            var dt = (DateTime)gridViewImpMestDetail.EditingValue;
                            if (dt == null || dt == DateTime.MinValue)
                            {
                                data.EXPIRED_DATE = null;
                            }
                            else if ((Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959")) < (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Hạn sử dụng không được nhỏ hơn ngày hiện tại");
                                return;
                            }
                            else
                            {
                                data.EXPIRED_DATE = Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959");
                            }
                            if (data.IsMedicine)
                            {
                                data.HisMedicine.EXPIRED_DATE = data.EXPIRED_DATE;
                            }
                            else
                            {
                                data.HisMaterial.EXPIRED_DATE = data.EXPIRED_DATE;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "PACKAGE_NUMBER")
                    {
                        if (data.IsMedicine)
                        {
                            data.HisMedicine.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                        }
                        else
                        {
                            data.HisMaterial.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                        }
                    }
                    gridControlImpMestDetail.RefreshDataSource();
                    CalculTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_STR")
                        {
                            e.Value = ConvertNumberToString(data.IMP_PRICE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ConvertNumberToString(decimal number)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Number.Convert.NumberToString(number, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
        #endregion
    }
}
