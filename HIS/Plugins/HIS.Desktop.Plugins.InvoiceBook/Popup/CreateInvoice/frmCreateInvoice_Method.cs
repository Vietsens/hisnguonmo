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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Entity.Model.Metadata;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Mapper;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using MOS.Filter;
using HIS.Desktop.Plugins.InvoiceBook.Base;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.InvoiceBook.Popup.CreateInvoice.Validation;
using HIS.Desktop.Plugins.InvoiceBook;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class frmCreateInvoice
    {
        V_HIS_INVOICE printInvoice;
        internal HIS.Desktop.Plugins.InvoiceBook.UCInvoiceBook ucInvoiceBook;
        #region Method_Form---------------------------------------------------------------------------------------------------

        private void LoadSellerInfo()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBranchFilter filter = new HisBranchFilter();
                filter.ID = WorkPlace.GetBranchId();

                var listBranch = new BackendAdapter(param).Get<List<HIS_BRANCH>>("api/HisBranch/Get", ApiConsumers.MosConsumer, filter, param);

                if (listBranch != null && listBranch.Count > 0)
                {
                    this.branch = listBranch.FirstOrDefault();
                }

                if (this.branch != null)
                {
                    txtSellerName.Text = this.branch.BRANCH_NAME;
                    txtSellerAccountNumber.Text = this.branch.ACCOUNT_NUMBER;
                    txtSellerAddress.Text = this.branch.ADDRESS;
                    txtSellerPhone.Text = this.branch.PHONE;
                    txtSellerTaxCode.Text = this.branch.TAX_CODE;
                }
                else
                {
                    txtSellerName.Text = "";
                    txtSellerAccountNumber.Text = "";
                    txtSellerAddress.Text = "";
                    txtSellerPhone.Text = "";
                    txtSellerTaxCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void LoadDefaultForm()
        {
            try
            {
                _lisPayForms = PayFormsGetData();
                cboPayForm.Properties.DataSource = _lisPayForms;
                SetDisplayCombooboxPayForm();
                setDefaultTotalFrom();
                LoadSellerInfo();
                dtInvoiceTime.EditValue = DateTime.Now;
                SetEnableButton(true);

                //txtTotalFromNumberOder.Text = string.Format("{0}/{1}/{2}", _invoiceBookWitchUcBook.TOTAL,
                //    _invoiceBookWitchUcBook.FORM, _invoiceBookWitchUcBook.CURRENT_NUM_ORDER);
                dtInvoiceTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDisplayCombooboxPayForm()
        {
            try
            {
                cboPayForm.Properties.DisplayMember = "PAY_FORM_NAME";
                cboPayForm.Properties.ValueMember = "ID";

                cboPayForm.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboPayForm.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboPayForm.Properties.ImmediatePopup = true;
                cboPayForm.ForceInitialize();
                cboPayForm.Properties.View.Columns.Clear();

                var aColumnCode = cboPayForm.Properties.View.Columns.AddField("PAY_FORM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                var aColumnName = cboPayForm.Properties.View.Columns.AddField("PAY_FORM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchPayForm()
        {
            try
            {
                if (string.IsNullOrEmpty(txtPayFormCode.Text))
                    cboPayForm.Properties.DataSource = _lisPayForms;
                else
                {
                    var listPayForm = _lisPayForms.Where(s => s.PAY_FORM_NAME.Contains(txtPayFormCode.Text) || s.PAY_FORM_CODE.Contains(txtPayFormCode.Text)).ToList();
                    if (listPayForm.Count == 1)
                    {
                        cboPayForm.Properties.DataSource = listPayForm;
                        cboPayForm.EditValue = listPayForm[0].ID;
                        txtPayFormCode.Text = listPayForm[0].PAY_FORM_CODE;

                        dtInvoiceTime.Focus();
                        dtInvoiceTime.ShowPopup();
                    }
                    else if (listPayForm.Count > 1)
                        cboPayForm.Properties.DataSource = listPayForm;
                    else
                    {
                        cboPayForm.EditValue = null;
                        cboPayForm.Properties.DataSource = _lisPayForms;
                        cboPayForm.Focus();
                        cboPayForm.ShowPopup();
                    }
                }
                SetDisplayCombooboxPayForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateControl()
        {
            try
            {
                ValidationControlTextEdit(txtPayFormCode, ValidTxtPayFormCode);//======

                ValidationControlTextEdit(dtInvoiceTime, ValidDtInvoiceTime);//=====

                ValidationControlTextEdit(txtBuyerName, ValidTxtBuyerName);//======

                ValidationControlTextEdit(txtSellerName, ValidTxtSellerName);//=====
                ValidVat();
                ValidChietKhau();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckValidInvoiceDetail(CommonParam param)
        {
            bool valid = true;
            bool valid_InvoiceDetail = true;
            try
            {
                if (_listInvoiceDetailNews == null || _listInvoiceDetailNews.Count <= 0)
                {
                    valid_InvoiceDetail = false;
                }
                foreach (var item in _listInvoiceDetailNews)
                {
                    if (item.AMOUNT < 0 || item.PRICE < 0 || item.DISCOUNT < 0)
                    {
                        valid_InvoiceDetail = false;
                        valid = false;
                        HIS.Desktop.LibraryMessage.MessageUtil.SetParam(param, LibraryMessage.Message.Enum.TruongDuLieuKhongNhanGiaTriAm);
                        return valid;
                    }
                }
                foreach (var item in _listInvoiceDetailNews)
                {
                    if (String.IsNullOrEmpty(item.GOODS_NAME))
                    {
                        valid_InvoiceDetail = false;
                        valid = false;
                        HIS.Desktop.LibraryMessage.MessageUtil.SetParam(param, LibraryMessage.Message.Enum.ThongBaoDuLieuTrong);
                        return valid;
                    }
                }

                valid = valid_InvoiceDetail;
                if (!valid)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(HIS.Desktop.Plugins.InvoiceBook.Resources.ResourceMessage.GiaTriLonHon0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }


        private void RefreshControl()
        {
            try
            {
                //_listInvoiceDetailNews = null;
                //gctCreateInvoiceDetail.DataSource = null;
                LoadDataNullSourceInvoiceDetail();
                CreateNewItemInvoiceDetail();

                //gctCreateInvoiceDetail_Load(null,null);
                txtBuyerName.Text = "";
                txtPayFormCode.Text = "";
                cboPayForm.EditValue = null;
                txtNumberOrder.Text = "";
                txtTotalFromNumberOder.Text = "";
                dtInvoiceTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                spinExemption.EditValue = 0;
                spinAmount.Value = 0;
                spinVatRatio.Value = 0;
                txtBuyerName.Text = "";
                txtBuyerTaxCode.Text = "";
                txtBuyerAccountNumber.Text = "";
                txtBuyerOrganization.Text = "";
                txtBuyerAddress.Text = "";
                txtDescription.Text = "";
                txtPayFormCode.Focus();
                txtPayFormCode.SelectAll();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButton(bool set)
        {
            try
            {
                btnSaveInvoiceBook.Enabled = set;
                btnPrint.Enabled = !set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SaveInvoice()
        {


            var param = new CommonParam();
            var success = false;
            this.positionHandleControl = -1;
            if (!dxValidationProvider1.Validate())
                return;
            bool valid = true;
            valid = valid && dxValidationProvider1.Validate();
            valid = valid && (CheckValidInvoiceDetail(param));
            WaitingManager.Show();
            try
            {
                if (valid)
                {

                    if (grvCreateInvoiceDetail == null || grvCreateInvoiceDetail.DataSource == null) return;
                    var listInvoiceDetail = new List<HIS_INVOICE_DETAIL>();
                    var listInvoiceDetailNew = (List<HIS_INVOICE_DETAIL_NEW>)gctCreateInvoiceDetail.DataSource;
                    foreach (var invoiceDetailNew in listInvoiceDetailNew)
                    {
                        AutoMapper.Mapper.CreateMap<HIS_INVOICE_DETAIL_NEW, HIS_INVOICE_DETAIL>();
                        var invoiceDetail = AutoMapper.Mapper.Map<HIS_INVOICE_DETAIL_NEW, HIS_INVOICE_DETAIL>(invoiceDetailNew);
                        listInvoiceDetail.Add(invoiceDetail);
                    }

                    var invoice = new HisInvoiceADO
                    {

                        INVOICE_BOOK_ID = _invoiceBookWitchUcBook.ID,
                        PAY_FORM_ID = (long)cboPayForm.EditValue,
                        INVOICE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInvoiceTime.DateTime) ?? 0,
                        SELLER_NAME = txtSellerName.Text,
                        SELLER_ADDRESS = txtSellerAddress.Text,
                        SELLER_PHONE = txtSellerPhone.Text,
                        SELLER_TAX_CODE = txtSellerTaxCode.Text,
                        SELLER_ACCOUNT_NUMBER = txtSellerAccountNumber.Text,
                        BUYER_NAME = txtBuyerName.Text,
                        BUYER_ADDRESS = txtBuyerAddress.Text,
                        BUYER_ACCOUNT_NUMBER = txtBuyerAccountNumber.Text,
                        BUYER_TAX_CODE = txtBuyerTaxCode.Text,
                        DISCOUNT = spinExemption.Value,
                        NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(txtNumberOrder.Text),
                        BUYER_ORGANIZATION = txtBuyerOrganization.Text,
                        DESCRIPTION = txtDescription.Text,
                        VAT_RATIO = spinVatRatio.Value / 100,
                        HIS_INVOICE_DETAIL = listInvoiceDetail,
                        TEMPLATE_CODE = _invoiceBookWitchUcBook.TEMPLATE_CODE,
                        SYMBOL_CODE = _invoiceBookWitchUcBook.SYMBOL_CODE

                    };

                    printInvoice = new BackendAdapter(param).Post<V_HIS_INVOICE>(ApiConsumer.HisRequestUriStore.HIS_INVOICE__CREATE, ApiConsumers.MosConsumer, invoice, param);
                    if (printInvoice != null)
                    {
                        success = true;
                        //ucInvoiceBook.FillDataToGridInvoiceBook(ucInvoiceBook);
                        //ucInvoiceBook.MeShow();
                        setDefaultTotalFrom();
                        txtNumberOrder.Text = printInvoice.VIR_NUM_ORDER.ToString();

                        //txtTotalFromNumberOder.Text = returnCreateInvoice.T + "/" + returnCreateInvoice.FROM_NUM_ORDER + "/" + returnCreateInvoice.CURRENT_NUM_ORDER;

                        grvCreateInvoiceDetail.ClearColumnErrors();
                        SetEnableButton(false);
                        //AddControlToBtnPrint();
                    }
                    WaitingManager.Hide();
                    UCInvoiceBook ucinvoicebook = new UCInvoiceBook(currentModule);
                    ucinvoicebook.FillDataToGridInvoiceBook(ucinvoicebook);
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
        }
        #endregion

        private void setDefaultTotalFrom()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (_invoiceBookWitchUcBook != null)
                {

                    MOS.Filter.HisInvoiceBookViewFilter invoiceBookFilter = new MOS.Filter.HisInvoiceBookViewFilter();
                    invoiceBookFilter.ID = _invoiceBookWitchUcBook.ID;
                    var invoiceBooks = new BackendAdapter(param).Get<List<V_HIS_INVOICE_BOOK>>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_BOOK_GET__VIEW, ApiConsumers.MosConsumer, invoiceBookFilter, null);
                    MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK aInvoiceBook = new MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK();
                    if (invoiceBooks != null && invoiceBooks.Count > 0)
                    {
                        aInvoiceBook = invoiceBooks.FirstOrDefault();
                    }

                    txtTotalFromNumberOder.Text = _invoiceBookWitchUcBook.TOTAL + "/" + _invoiceBookWitchUcBook.FROM_NUM_ORDER + "/" + aInvoiceBook.CURRENT_NUM_ORDER;
                }

                positionHandleControl = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                //grvCreateInvoiceDetail.
                //if (this.HisInvoice != null)
                //{
                // spinNumberOrder.Text = this.HisInvoice.VIR_NUM_ORDER;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Method_GridView-----------------------------------------------------------------------------------------------
        private void CreateNewItemInvoiceDetail()
        {
            var addTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            var newInvoiceDetail = new HIS_INVOICE_DETAIL_NEW
            {
                ADD_TIME = addTime
            };
            gctCreateInvoiceDetail.DataSource = null;
            _listInvoiceDetailNews.Add(newInvoiceDetail);
            LoadDataSourceInvoiceDetail();
        }

        private void DeleteNewItemInvoiceDetail(HIS_INVOICE_DETAIL_NEW invoiceDetailAddIndex)
        {
            _listInvoiceDetailNews.RemoveAll(s => s == invoiceDetailAddIndex);
            LoadDataSourceInvoiceDetail();
        }


        private void LoadDataSourceInvoiceDetail()
        {
            gctCreateInvoiceDetail.BeginUpdate();
            gctCreateInvoiceDetail.DataSource = _listInvoiceDetailNews.OrderBy(s => s.ADD_TIME).ToList();
            gctCreateInvoiceDetail.EndUpdate();
        }
        private void LoadDataNullSourceInvoiceDetail()
        {
            //_listInvoiceDetailNews.RemoveAll(o=>o==_listInvoiceDetailNews.Select());
            _listInvoiceDetailNews = null;
            gctCreateInvoiceDetail.BeginUpdate();
            _listInvoiceDetailNews = new List<HIS_INVOICE_DETAIL_NEW>();
            gctCreateInvoiceDetail.DataSource = null;
            gctCreateInvoiceDetail.EndUpdate();
        }


        #endregion


        #region Print method
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            var result = false;
            WaitingManager.Show();
            try
            {
                var invoice = (V_HIS_INVOICE)grvCreateInvoiceDetail.GetFocusedRow();
                var listInvoiceDetails = InvoiceDetailGetDatas(invoice, new CommonParam()).Data;
                var mps000115Rdo = new MPS.Processor.Mps000115.PDO.Mps000115PDO(_hisInvoiceFocusRowButtonPrint, listInvoiceDetails);
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000115Rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000115Rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                PrintData.EmrInputADO = inputADO;

                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            WaitingManager.Hide();
            return result;
        }

        internal ApiResultObject<List<HIS_INVOICE_DETAIL>> InvoiceDetailGetDatas(V_HIS_INVOICE invoiceDetail, object param)
        {
            var result = new ApiResultObject<List<HIS_INVOICE_DETAIL>>();
            try
            {
                var start = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                var paramCommon = new CommonParam();
                if (limit > 0)
                    paramCommon = new CommonParam(start, limit);

                result = new BackendAdapter(paramCommon).GetRO<List<HIS_INVOICE_DETAIL>>
                    (ApiConsumer.HisRequestUriStore.HIS_INVOICE_DETAIL, ApiConsumers.MosConsumer, new HisInvoiceDetailFilter { INVOICE_ID = invoiceDetail.ID }, paramCommon);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ClickPrintInvoice()
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__INVOICE_DETAIL, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RemoveControlError()
        {
            try
            {
                positionHandleControl = -1;
                dxValidationProvider1.RemoveControlError(txtNumberOrder);
                dxValidationProvider1.RemoveControlError(txtPayFormCode);
                dxValidationProvider1.RemoveControlError(cboPayForm);
                dxValidationProvider1.RemoveControlError(dtInvoiceTime);
                dxValidationProvider1.RemoveControlError(txtBuyerName);
                dxValidationProvider1.RemoveControlError(txtSellerName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
