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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class frmCreateInvoice
    {
        #region Event_Form----------------------------------------------------------------------------------------------------
        private void frmCreateInvoice_Load(object sender, EventArgs e)
        {
            try
            {
                //InvoicePrintPageCFG.LoadConfig();
                SetCaptionByLanguageKey();
                ValidateControl();
                LoadDefaultForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InvoiceBook.Resources.Lang", typeof(HIS.Desktop.Plugins.InvoiceBook.Popup.CreateInvoice.frmCreateInvoice).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreateInvoice.cboPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmCreateInvoice.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefreshControl.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.btnRefreshControl.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveInvoiceBook.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.btnSaveInvoiceBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCreateInvoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {

        }

        private void frmCreateInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
                this.Close();
        }

        private void txtPayFormCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SearchPayForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboPayForm.EditValue == null) return;
                txtPayFormCode.Text = _lisPayForms.First(s => s.ID == (long)cboPayForm.EditValue).PAY_FORM_CODE;

                dtInvoiceTime.Focus();
                dtInvoiceTime.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtInvoiceTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                spinExemption.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveInvoiceBook_Click(object sender, EventArgs e)
        {
            try
            {
                SaveInvoice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefreshControl_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshControl();
                RemoveControlError();
                LoadSellerInfo();
                SetEnableButton(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExemption_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (_totalPrice != 0)
                    spinAmount.Value = _totalPrice - spinExemption.Value >= 0 ? _totalPrice - spinExemption.Value : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event_GridView------------------------------------------------------------------------------------------------
        private void gctCreateInvoiceDetail_Load(object sender, EventArgs e)
        {
            try
            {
                CreateNewItemInvoiceDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvCreateInvoiceDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var data = (HIS_INVOICE_DETAIL_NEW)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data == null) return;
                var fistRowGridGiew = (HIS_INVOICE_DETAIL_NEW)((IList)((BaseView)sender).DataSource)[0];
                switch (e.Column.FieldName)
                {
                    case "ADD_NEW_ITEM":
                        e.RepositoryItem = data == fistRowGridGiew ? btnAddItem : btnDeleteItem;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvCreateInvoiceDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound) return;
                var data = (HIS_INVOICE_DETAIL_NEW)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data == null) return;
                switch (e.Column.FieldName)
                {
                    case "NUMBER_ORDER":
                        e.Value = e.ListSourceRowIndex + 1;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            CreateNewItemInvoiceDetail();
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                var dataFocuseRow = (HIS_INVOICE_DETAIL_NEW)grvCreateInvoiceDetail.GetFocusedRow();
                DeleteNewItemInvoiceDetail(dataFocuseRow);
                _totalPrice = ((List<HIS_INVOICE_DETAIL_NEW>)gctCreateInvoiceDetail.DataSource).ToList().Sum(s => s.SUM_PRICE_STR);
                spinAmount.Value = _totalPrice - spinExemption.Value >= 0 ? _totalPrice - spinExemption.Value : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gctCreateInvoiceDetail_ForeColorChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvCreateInvoiceDetail_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (_isCalculatorPrice == false)
                    _isCalculatorPrice = true;
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvCreateInvoiceDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (_isCalculatorPrice == false) return;
                _isCalculatorPrice = false;
                var view = gctCreateInvoiceDetail.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                var row = (HIS_INVOICE_DETAIL_NEW)grvCreateInvoiceDetail.GetRow(grvCreateInvoiceDetail.FocusedRowHandle);
                if (row == null) return;
                if (view == null) return;
                var calculatorPrice = row.AMOUNT * row.PRICE - (row.DISCOUNT ?? 0);
                view.SetRowCellValue(view.FocusedRowHandle, view.Columns["SUM_PRICE_STR"],
                    Inventec.Common.Number.Convert.NumberToString(calculatorPrice, ConfigApplications.NumberSeperator));
                _totalPrice = ((List<HIS_INVOICE_DETAIL_NEW>)gctCreateInvoiceDetail.DataSource).ToList().Sum(s => s.SUM_PRICE_STR);
                spinAmount.Value = _totalPrice - spinExemption.Value >= 0 ? _totalPrice - spinExemption.Value : 0;
            }
            catch (Exception ex)
            {
                _isCalculatorPrice = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        public string GetError(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                HIS_INVOICE_DETAIL_NEW data = (HIS_INVOICE_DETAIL_NEW)grvCreateInvoiceDetail.GetRow(rowHandle);
                if (column.FieldName == "AMOUNT")
                {

                    if (data == null)
                        return string.Empty;
                    if (data.AMOUNT < 0)
                    {
                        return "Số lượng phải lớn hơn hoặc bằng 0";
                    }
                }
                if (column.FieldName == "PRICE")
                {
                    if (data == null)
                        return string.Empty;
                    if (data.PRICE < 0)
                    {
                        return "Giá phải lớn hơn 0 hoặc bằng 0";
                    }
                }
                if (column.FieldName == "DISCOUNT")
                {
                    if (data == null)
                        return string.Empty;
                    if (data.DISCOUNT < 0)
                    {
                        return "Chiết khấu phải lớn hơn 0 hoặc bằng 0";
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return string.Empty;
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
                cellInfo.FillBackground = false;
                cellInfo.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
