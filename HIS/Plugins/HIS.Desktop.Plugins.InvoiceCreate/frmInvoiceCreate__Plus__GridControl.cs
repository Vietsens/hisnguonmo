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
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InvoiceCreate
{
    public partial class frmInvoiceCreate : HIS.Desktop.Utility.FormBase
    {
        private void gridViewInvoiceDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                gridViewInvoiceDetail.PostEditor();
                if (e.Column.FieldName == "PRICE" || e.Column.FieldName == "AMOUNT" || e.Column.FieldName == "DISCOUNT")
                {
                    try
                    {
                        decimal price = 0;
                        decimal amount = 0;
                        decimal discount = 0;
                        var priceData = gridViewInvoiceDetail.GetFocusedRowCellValue("PRICE");
                        if (priceData != null)
                        {
                            price = Convert.ToDecimal(priceData);
                        }
                        var amountData = gridViewInvoiceDetail.GetFocusedRowCellValue("AMOUNT");
                        if (amountData != null)
                        {
                            amount = Convert.ToDecimal(amountData);
                        }
                        var discountData = gridViewInvoiceDetail.GetFocusedRowCellValue("DISCOUNT");
                        if (discountData != null)
                        {
                            discount = Convert.ToDecimal(discountData);
                        }
                        gridViewInvoiceDetail.SetFocusedRowCellValue("TOTAL_PRICE", (amount * price) - discount);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewInvoiceDetail_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
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

        private void gridViewInvoiceDetail_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                try
                {
                    int count = 0;
                    GridView view = sender as GridView;
                    if (view.GetRowCellValue(e.RowHandle, view.Columns["GOODS_NAME"]) == null || String.IsNullOrEmpty(view.GetRowCellValue(e.RowHandle, view.Columns["GOODS_NAME"]).ToString()))
                    {
                        if (count == 0) count = 1;
                        e.Valid = false;
                        view.SetColumnError(view.Columns["GOODS_NAME"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__VALID_GOODS_NAME_NOT_FOUND", Base.ResourceLangManager.LanguageFrmInvoiceCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                    }

                    if (Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, view.Columns["PRICE"])) < 0)
                    {
                        if (count == 0) count = 2;
                        e.Valid = false;
                        view.SetColumnError(view.Columns["PRICE"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__VALID_PRICE_GREAT_THAN_ZERO", Base.ResourceLangManager.LanguageFrmInvoiceCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                    }

                    if (Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, view.Columns["AMOUNT"])) < 0)
                    {
                        if (count == 0) count = 3;
                        e.Valid = false;
                        view.SetColumnError(view.Columns["AMOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__VALID_AMOUNT_GREAT_THAN_ZERO", Base.ResourceLangManager.LanguageFrmInvoiceCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                    }

                    if (Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, view.Columns["DISCOUNT"])) < 0)
                    {
                        if (count == 0) count = 4;
                        e.Valid = false;
                        view.SetColumnError(view.Columns["DISCOUNT"], Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__VALID_DISCOUNT_GREAT_THAN_ZERO", Base.ResourceLangManager.LanguageFrmInvoiceCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                    }
                    if (count == 0)
                    {
                        CalcuTotalPrice();
                    }
                    else if (count == 1)
                    {
                        gridViewInvoiceDetail.FocusedColumn = view.Columns["GOODS_NAME"];
                    }
                    else if (count == 2)
                    {
                        gridViewInvoiceDetail.FocusedColumn = view.Columns["PRICE"];
                    }
                    else if (count == 3)
                    {
                        gridViewInvoiceDetail.FocusedColumn = view.Columns["AMOUNT"];
                    }
                    else if (count == 4)
                    {
                        gridViewInvoiceDetail.FocusedColumn = view.Columns["DISCOUNT"];
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridViewInvoiceDetail_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    CalcuTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
