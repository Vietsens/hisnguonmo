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
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InvoiceCreateForTreatment
{
    public partial class frmInvoiceCreateForTreatment : HIS.Desktop.Utility.FormBase
    {
        private void btnCreateElectricInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.treatmentId <= 0)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                var listData = ssTreeProcessor.GetListCheck(ucSereServTree);
                //if (listData != null && listData.Count > 0)
                //{
                //    listData = listData.Where(o => o.BILL_ID.HasValue).ToList();
                //}

                if (Valid(listData, param))
                {
                    if (InvoiceCreateForTreatmentConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                    {
                        success = TaoHoaDonDienTuBenThu3CungCap(listData, param);
                    }
                    else
                    {
                        //Chế độ mặc định: sau khi tạo giao dịch trên hệ thống HIS thành công, tự tạo hóa đơn + ký điện tử trên hóa đơn lưu trên hệ thống HIS
                        string fileName = System.IO.Path.GetFullPath(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, "PrintTemplate")) + "\\Mps000115\\MPS000115_InHoaDon___2_Lien______A4_02.xlsx";
                        this.InPhieuThuThanhToanKyDienTu(PrintTypeCodeStore.PRINT_TYPE_CODE__InHoaDonDo_MPS000115, fileName);
                    }
                }

                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                    this.Hide();
                }
                else
                    MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool InPhieuThuThanhToanKyDienTu(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                //if (this.resultBill == null)
                //    return result;

                //CommonParam param = new CommonParam();
                //MemoryStream streamResult = new MemoryStream();

                //HisBillFundFilter billFundFilter = new HisBillFundFilter();
                //billFundFilter.BILL_ID = this.resultBill.ID;
                //var listBillFund = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_FUND>>("api/HisBillFund/Get", ApiConsumers.MosConsumer, billFundFilter, null);
                ////Gọi thư viện in truyền vào dữ liệu, kết quả trả về gán vào đối tượng streamResult
                //MPS.Processor.Mps000111.PDO.Mps000111PDO rdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(null, this.resultBill, null, listBillFund);
                //MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, streamResult);
                //result = MPS.MpsPrinter.Run(printData);
                //if (result && printData.saveMemoryStream != null && printData.saveMemoryStream.Length > 0)
                //{
                //    streamResult.Position = 0;
                //    MemoryStream outStream = new MemoryStream();
                //    //Gọi thư viện convert file excel đã qua xử lý về định dạng pdf
                //    if (Inventec.Common.FileConvert.Convert.ExcelToPdf(printData.saveMemoryStream, "", outStream, ""))
                //    {
                //        outStream.Position = 0;
                //        if (outStream != null && outStream.Length > 0)
                //        {
                //            //Gọi thư viện đọc chứng thư trên máy và thực hiện ký điện tử trên file pdf
                //            //Trước khi ký sẽ thực hiện các xử lý mã hóa,...
                //            Inventec.Ca.Processor processor = new Inventec.Ca.Processor();
                //            string pdfContentBase64 = Convert.ToBase64String(ReadFully(outStream));
                //            var pdfContentSigned = processor.SignPdfBase64(pdfContentBase64, "");

                //            //Chuyển đổi chuỗi base64 về mảng byte
                //            var base64EncodedBytes = System.Convert.FromBase64String(pdfContentSigned);
                //            //Chuyển đổi mảng byte của fiel kết quả về dạng MemoryStream
                //            MemoryStream outStreamResult = new MemoryStream(base64EncodedBytes);
                //            outStreamResult.Position = 0;
                //            //Gọi api fss upload file hóa đơn đã ký điện tử thành công
                //            string fileNameUpload = this.resultBill.ACCOUNT_BOOK_CODE + "__" + this.resultBill.TRANSACTION_CODE + SIGNED_EXTENSION;
                //            var fileUploadInfo = FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, "FILESIGNED", outStreamResult, fileNameUpload);
                //            if (fileUploadInfo != null)
                //            {
                //                //Cập nhật lại trường FILE_URL, FILE_NAME của bảng Bill
                //                this.resultBill.FILE_URL = fileUploadInfo.Url;
                //                this.resultBill.FILE_NAME = fileNameUpload;
                //                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_BILL>(RequestUriStore.HIS_BILL_UPDATE, ApiConsumers.MosConsumer, this.resultBill, param);
                //                if (rs != null && !String.IsNullOrEmpty(rs.FILE_URL))
                //                {
                //                    Inventec.Common.Logging.LogSystem.Debug("Ky dien tu cho giao dich hoa don thanh toan thanh cong. TRANSACTION_CODE = " + this.resultBill.TRANSACTION_CODE + ", Fss_Url_Signed_File = " + fileUploadInfo.Url);
                //                    result = true;
                //                    this.Close();
                //                }
                //                else
                //                {
                //                    Inventec.Common.Logging.LogSystem.Warn("Tao giao dich thanh toan thanh cong, tao va upload file pdf cho hoa don thanh toan thanh cong. Tuy nhien qua trinh cap nhat url cua file pdf vao bang BILL that bai.");
                //                }
                //            }
                //            else
                //            {
                //                Inventec.Common.Logging.LogSystem.Warn("Da thuc hien viec ky dien tu tren file pdf hoa don thanh toan xong, tuy nhien upload file ket qua len server that bai. Cac buoc xu ly tiep sau khong the thuc hien.");
                //            }
                //        }
                //        else
                //        {
                //            Inventec.Common.Logging.LogSystem.Warn("Convert file excel da xu ly về dinh dang pdf that bai. Ky dien tu that bai.");
                //        }
                //    }
                //    else
                //    {
                //        Inventec.Common.Logging.LogSystem.Warn("Xu ly ExcelToPdf that bai. Tao file pdf convert tu file excel da qua xu ly that bai, cac buoc xu ly tiep sau khong the thuc hien");
                //    }
                //}
                //else
                //{
                //    Inventec.Common.Logging.LogSystem.Warn("Tao giao dich thanh toan thanh cong, tuy nhien xu ly tao file excel hoa don thanh toan that bai. Khong the thuc hien ky dien tu tren hoa don thanh toan.");
                //}
                //if (!result)
                //{
                //    param.Messages.Add(Base.ResourceMessageLang.TaoThanhToanThanhCong_TuyNhienThucHienKyDienTuThatBai);
                //    MessageManager.Show(param, result);
                //    this.Close();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private bool TaoHoaDonDienTuBenThu3CungCap(List<HIS.UC.SereServTree.SereServADO> listData, CommonParam param)
        {
            bool success = false;
            try
            {
                ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap();
                if (electronicBillResult == null || !electronicBillResult.Success)
                {
                    param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                    return false;
                }
                success = electronicBillResult.Success;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap()
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                List<V_HIS_SERE_SERV_5> sereServBills = new List<V_HIS_SERE_SERV_5>();
                var sereServBillADOs = ssTreeProcessor.GetListCheck(this.ucSereServTree);
                if (sereServBillADOs == null)
                {
                    result.Success = false;
                    LogSystem.Debug("Khong co dich vu thanh toan nao duoc chon!");
                    return result;
                }
                foreach (var item in sereServBillADOs)
                {
                    V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServBill, item);
                    sereServBills.Add(sereServBill);
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Amount = txtAmount.Value;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = txtDiscount.Value;
                if (txtDiscount.EditValue != null && txtDiscount.Value > 0)
                {
                    dataInput.DiscountRatio = (txtAmount.Value / txtDiscount.Value) * 100;
                }

                dataInput.PaymentMethod = cboPayForm.Text;
                dataInput.SereServs = sereServBills;
                V_HIS_TREATMENT_FEE treatmentFee = new V_HIS_TREATMENT_FEE();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_FEE>(treatmentFee, this.treatment);
                dataInput.Treatment = treatmentFee;
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
                WaitingManager.Show();
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
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

        private bool Valid(List<HIS.UC.SereServTree.SereServADO> listData, CommonParam param)
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
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.treatmentId <= 0)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                var listData = ssTreeProcessor.GetListCheck(ucSereServTree);
                //if (listData != null && listData.Count > 0)
                //{
                //    listData = listData.Where(o => o.BILL_ID.HasValue).ToList();
                //}

                if (Valid(listData, param))
                {
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

                    data.SereServIds = listData.Select(s => s.ID).ToList();

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_INVOICE>(HisRequestUriStore.HIS_INVOICE__CREATE, ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        resultInvoice = rs;
                        SetInfoCreateInvoiceSuccess();
                    }
                }

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
                cboInvoiceBook.Properties.BeginUpdate();
                cboInvoiceBook.Properties.DataSource = listInvoiceBook;
                if (listInvoiceBook.Count == 1)
                {
                    txtTemplateCode.Text = listInvoiceBook.First().TEMPLATE_CODE;
                    cboInvoiceBook.EditValue = listInvoiceBook.FirstOrDefault().INVOICE_BOOK_ID;
                }
                cboInvoiceBook.Properties.EndUpdate();


                ResetControlValue();
                LoadDataToTreeSereServ();
                CalcuTotalPrice();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnCreateElectrictionBill_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnCreateElectricInvoice.Enabled)
                    btnCreateElectricInvoice_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
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
                if (bbtnRCPrint.Enabled)
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

                long rowCountGridFirstPage = HisConfigs.Get<long>("HIS.Desktop.Plugins.InvoiceBook.rowCountGridFirstPage"), rowCountGridNextPage = HisConfigs.Get<long>("HIS.Desktop.Plugins.InvoiceBook.rowCountGridNextPage"), totalNextPages = 0;

                string creatorUserName = "";
                AcsUserFilter userFilter = new AcsUserFilter();
                userFilter.LOGINNAME = invoice.CREATOR;
                var getUser = new BackendAdapter(param).Get<List<ACS_USER>>(AcsRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, userFilter, null);
                if (getUser != null && getUser.Count > 0)
                {
                    creatorUserName = getUser.FirstOrDefault().USERNAME;
                }
                List<string> titles = new List<string>();
                titles.Add("Liên 1: Lưu");
                titles.Add("Liên 2: Giao người mua");

                HisInvoiceDetailFilter detailFiter = new HisInvoiceDetailFilter();
                detailFiter.INVOICE_ID = this.resultInvoice.ID;
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
