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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.InvoiceCreate.ADO;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InvoiceCreate
{
    public partial class frmInvoiceCreate : HIS.Desktop.Utility.FormBase
    {
        private void btnCreateElectricInvoice_Click(object sender, EventArgs e)
        {
            try
            {

                if (gridViewInvoiceDetail.IsEditing)
                    gridViewInvoiceDetail.CloseEditor();

                if (gridViewInvoiceDetail.FocusedRowModified)
                    gridViewInvoiceDetail.UpdateCurrentRow();

                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap();
                if (electronicBillResult == null || !electronicBillResult.Success)
                {
                    param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                    return;
                }
                WaitingManager.Hide();
                if (electronicBillResult.Success)
                {
                    MessageManager.Show(this, param, success);
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap()
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                var listData = bindingSource1.DataSource as List<HisInvoiceDetailADO>;
                if (listData == null || listData.Count == 0)
                {
                    result.Success = false;
                    MessageBox.Show("Không tìm thấy dịch vụ nào được chọn!");
                    return result;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Amount = txtAmount.Value;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = txtDiscount.Value;
                dataInput.DiscountRatio = txtDiscount.Value / 100;
                dataInput.PaymentMethod = cboPayForm.Text;
                dataInput.Currency = "VND";

                var invoiceBook = listInvoiceBook.FirstOrDefault(o => o.INVOICE_BOOK_ID == Convert.ToInt64(cboInvoiceBook.EditValue));
                if (invoiceBook != null)
                {
                    dataInput.SymbolCode = invoiceBook.SYMBOL_CODE;
                    dataInput.TemplateCode = invoiceBook.TEMPLATE_CODE;
                }

                if (dtInvoiceTime.EditValue != null && dtInvoiceTime.DateTime != DateTime.MinValue)
                {
                    dataInput.TransactionTime = Convert.ToInt64(dtInvoiceTime.DateTime.ToString("yyyyMMddHHmmss"));
                }

                dataInput.Treatment = new V_HIS_TREATMENT_FEE()
                {
                    TDL_PATIENT_ACCOUNT_NUMBER = txtBuyerAccountNumber.Text,
                    TDL_PATIENT_ADDRESS = txtBuyerAddress.Text,
                    TDL_PATIENT_NAME = txtBuyerName.Text,
                    TDL_PATIENT_TAX_CODE = txtBuyerTaxCode.Text
                };

                WaitingManager.Show();
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput, TemplateEnum.TYPE.Template2);
                result = electronicBillProcessor.Run(ElectronicBillType.ENUM.CREATE_INVOICE);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool ValidElectronicBill(List<HisInvoiceDetailADO> listData, CommonParam param)
        {
            bool valid = true;
            try
            {
                if (listData == null || listData.Count == 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuaChonDichVuDeTaoHoaDon);
                    valid = false;
                }

                if (cboInvoiceBook.EditValue == null || cboPayForm.EditValue == null)
                {
                    param.Messages.Add(Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    valid = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewInvoiceDetail.RefreshData();
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                var listData = bindingSource1.DataSource as List<HisInvoiceDetailADO>;

                if (listData == null || listData.Count == 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuaChonDichVuDeTaoHoaDon);
                    goto End;
                }

                if (cboInvoiceBook.EditValue == null || cboPayForm.EditValue == null)
                {
                    param.Messages.Add(Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    goto End;
                }

                HisInvoiceSDO data = new HisInvoiceSDO();

                var invoiceBook = listInvoiceBook.FirstOrDefault(o => o.INVOICE_BOOK_ID == Convert.ToInt64(cboInvoiceBook.EditValue));
                if (invoiceBook != null)
                {
                    data.INVOICE_BOOK_ID = invoiceBook.INVOICE_BOOK_ID;
                }

                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                if (payForm != null)
                {
                    data.PAY_FORM_ID = payForm.ID;
                }

                if (dtInvoiceTime.EditValue != null && dtInvoiceTime.DateTime != DateTime.MinValue)
                {
                    data.INVOICE_TIME = Convert.ToInt64(dtInvoiceTime.DateTime.ToString("yyyyMMddHHmmss"));
                }
                data.DISCOUNT = txtDiscount.Value;
                data.VAT_RATIO = txtVatRatio.Value / 100;
                data.BUYER_ACCOUNT_NUMBER = txtBuyerAccountNumber.Text;
                data.BUYER_ADDRESS = txtBuyerAddress.Text;
                data.BUYER_NAME = txtBuyerName.Text;
                data.BUYER_ORGANIZATION = txtBuyerOrganization.Text;
                data.BUYER_TAX_CODE = txtBuyerTaxCode.Text;
                data.DESCRIPTION = txtBuyerDescription.Text;
                data.SELLER_ACCOUNT_NUMBER = txtSellerAccountNumber.Text;
                data.SELLER_ADDRESS = txtSellerAddress.Text;
                data.SELLER_NAME = txtSellerName.Text;
                data.SELLER_PHONE = txtSellerPhone.Text;
                data.SELLER_TAX_CODE = txtSellerTaxCode.Text;

                if (txtNumOrder.EditValue != null && txtNumOrder.Value > 0)
                {
                    data.NUM_ORDER = (long)txtNumOrder.Value;
                }

                data.HIS_INVOICE_DETAIL = listData.ToList<HIS_INVOICE_DETAIL>();

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_INVOICE>(HisRequestUriStore.HIS_INVOICE__CREATE, ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    resultInvoice = rs;
                    SetInfoCreateInvoiceSuccess();
                }
            End:
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultInvoice == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__InHoaDonDo_MPS000115, delegateRunPrintTemplte);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadInvoiceBook();
                LoadDataToComboInvoiceBook();
                ResetControlValue();
                LoadBuyerInfo();
                CalcuTotalPrice();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnRCNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewInvoiceDetail.FocusedRowHandle >= 0)
                {
                    gridViewInvoiceDetail.DeleteRow(gridViewInvoiceDetail.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetInfoCreateInvoiceSuccess()
        {
            try
            {
                if (this.resultInvoice != null)
                {
                    txtNumOrder.Value = this.resultInvoice.NUM_ORDER;
                    txtNumOrder.Enabled = false;
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrintTemplte(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultInvoice == null)
                    return false;
                CommonParam param = new CommonParam();
                V_HIS_INVOICE invoice = null;
                List<HIS_PAY_FORM> payForm = new List<HIS_PAY_FORM>();
                List<MPS.Processor.Mps000115.PDO.InvoiceDetailADO> invoiceADO = new List<MPS.Processor.Mps000115.PDO.InvoiceDetailADO>();
                HisInvoiceViewFilter invoiceFilter = new HisInvoiceViewFilter();
                invoiceFilter.ID = this.resultInvoice.ID;
                var hisInvoices = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_INVOICE>>(HisRequestUriStore.HIS_INVOICE_GET__VIEW, ApiConsumers.MosConsumer, invoiceFilter, null);
                if (hisInvoices != null && hisInvoices.Count == 1)
                {
                    invoice = hisInvoices.First();
                }
                HisInvoiceDetailFilter detailFiter = new HisInvoiceDetailFilter();
                detailFiter.INVOICE_ID = this.resultInvoice.ID;
                string creatorUserName = "";
                AcsUserFilter userFilter = new AcsUserFilter();
                userFilter.LOGINNAME = invoice.CREATOR;
                var getUser = new BackendAdapter(param).Get<List<ACS_USER>>(AcsRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, userFilter, null);
                if (getUser != null && getUser.Count > 0)
                {
                    creatorUserName = getUser.FirstOrDefault().USERNAME;
                }

                long rowCountGridFirstPage = HisConfigs.Get<long>("HIS.Desktop.Plugins.InvoiceBook.rowCountGridFirstPage"), rowCountGridNextPage = HisConfigs.Get<long>("HIS.Desktop.Plugins.InvoiceBook.rowCountGridNextPage"), totalNextPages = 0;

                List<string> titles = new List<string>();
                titles.Add("Liên 1: Lưu");
                titles.Add("Liên 2: Giao người mua");

                var listDetail = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_INVOICE_DETAIL>>(HisRequestUriStore.HIS_INVOICE_DETAIL, ApiConsumers.MosConsumer, detailFiter, null);
                if (invoice != null && listDetail != null)
                {

                    HisPayFormFilter filter = new HisPayFormFilter();
                    filter.ID = this.resultInvoice.PAY_FORM_ID;
                    payForm = new BackendAdapter(param).Get<List<HIS_PAY_FORM>>(HisRequestUriStore.HIS_PAY_FORM_GET, ApiConsumers.MosConsumer, filter, null);

                    totalNextPages = (listDetail.Count - rowCountGridFirstPage) / rowCountGridNextPage + 2;
                    for (int i = 0; i < listDetail.Count; i++)
                    {
                        MPS.Processor.Mps000115.PDO.InvoiceDetailADO _invoiceAdo = new MPS.Processor.Mps000115.PDO.InvoiceDetailADO();
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_INVOICE_DETAIL, MPS.Processor.Mps000115.PDO.InvoiceDetailADO>();
                        _invoiceAdo = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_INVOICE_DETAIL, MPS.Processor.Mps000115.PDO.InvoiceDetailADO>(listDetail[i]);
                        if (i < rowCountGridFirstPage)
                        {
                            _invoiceAdo.PageId = 0;
                        }
                        else
                        {
                            _invoiceAdo.PageId = ((i - rowCountGridFirstPage) < 0 ? 0 : (i - rowCountGridFirstPage)) / rowCountGridNextPage + 1;
                        }
                        _invoiceAdo.NUM_ORDER = i + 1;
                        invoiceADO.Add(_invoiceAdo);
                    }

                    List<MPS.Processor.Mps000115.PDO.TotalNextPage> totalADO = new List<MPS.Processor.Mps000115.PDO.TotalNextPage>();
                    List<long> pageIds = invoiceADO.Select(o => o.PageId ?? 0).ToList();


                    for (int i = 0; i < totalNextPages; i++)
                    {
                        MPS.Processor.Mps000115.PDO.TotalNextPage totalNextPage = new MPS.Processor.Mps000115.PDO.TotalNextPage();
                        if (i == 0)
                        {
                            totalNextPage.id = 0;
                            totalNextPage.Name = "";
                        }
                        else
                        {
                            totalNextPage.id = i;
                            totalNextPage.Name = "Tiep theo trang truoc - trang " + (i + 1) + "/" + (totalNextPages);
                        }

                        var containItem = pageIds.Contains(totalNextPage.id);
                        if (containItem != true)
                        {
                            totalNextPage.Name = "";
                        }
                        totalADO.Add(totalNextPage);

                    }


                    MPS.Processor.Mps000115.PDO.Mps000115PDO rdo = new MPS.Processor.Mps000115.PDO.Mps000115PDO(invoice, listDetail, invoiceADO, totalADO, payForm, creatorUserName);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null));
                }
                if (result)
                {
                    HIS_INVOICE_PRINT print = new HIS_INVOICE_PRINT();
                    print.INVOICE_ID = this.resultInvoice.ID;
                    print.PRINT_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    print.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_INVOICE_PRINT>(HisRequestUriStore.HIS_INVOICE_PRINT_CREATE, ApiConsumers.MosConsumer, print, null);
                    if (rs == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Tao du lieu HisInvoicePrint that bai khi in hoa don do: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
                    }
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

    }
}
