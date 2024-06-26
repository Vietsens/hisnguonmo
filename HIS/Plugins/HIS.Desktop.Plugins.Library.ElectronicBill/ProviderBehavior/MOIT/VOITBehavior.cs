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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.ElectronicBillMoit;
using Inventec.Common.ElectronicBillMoit.ModelXml;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MOIT
{
    public class VOITBehavior : IRun
    {
        string serviceConfig { get; set; }
        string accountConfig { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        TemplateEnum.TYPE TempType { get; set; }
        int convert;
        string Version = "";

        public VOITBehavior(ElectronicBillDataInput _electronicBillDataInput, string _serviceConfig, string _accountConfig)
            : base()
        {
            this.serviceConfig = _serviceConfig;
            this.ElectronicBillDataInput = _electronicBillDataInput;
            this.accountConfig = _accountConfig;
        }

        ElectronicBillResult IRun.Run(ElectronicBillType.ENUM _electronicBillTypeEnum, TemplateEnum.TYPE _tempType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.Check(ref result))
                {
                    this.TempType = _tempType;

                    string[] configArr = serviceConfig.Split('|');

                    string serviceUrl = configArr[1]; //ConfigurationManager.AppSettings[AppConfigKey.WEBSERVICE_URL];
                    if (String.IsNullOrEmpty(serviceUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay dia chi Webservice URL");
                        ElectronicBillResultUtil.Set(ref result, false, "Không tìm thấy địa chỉ Webservice URL");
                        return result;
                    }

                    convert = int.Parse(configArr[2].Trim());
                    if (configArr.Count() > 3)
                    {
                        Version = configArr[3];
                    }

                    string[] accountConfigArr = accountConfig.Split('|');

                    Inventec.Common.ElectronicBillMoit.DataInitApi login = new Inventec.Common.ElectronicBillMoit.DataInitApi();
                    login.Address = serviceUrl;
                    login.User = accountConfigArr[0].Trim();
                    login.Pass = accountConfigArr[1].Trim();
                    login.SupplierTaxCode = ElectronicBillDataInput.Branch.TAX_CODE;

                    switch (_electronicBillTypeEnum)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            ProcessCreateInvoice(login, ref result);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            ProcessGetInvoice(login, ref result);
                            break;
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            ProcessCancelInvoice(login, ref result);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool Check(ref ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = serviceConfig.Split('|');
                if (configArr.Length < 3)
                    throw new Exception("Sai định dạng cấu hình .");
                if (configArr[0] != ProviderType.CongThuong)
                    throw new Exception("Khong dung cau hinh nha cung cap Bo Cong Thuong");

                string[] accountArr = accountConfig.Split('|');
                if (accountArr.Length != 2)
                    throw new Exception("Sai định dạng cấu hình tai khoan .");
            }
            catch (Exception ex)
            {
                result = false;
                ElectronicBillResultUtil.Set(ref electronicBillResult, false, "");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessCancelInvoice(Inventec.Common.ElectronicBillMoit.DataInitApi login, ref ElectronicBillResult result)
        {
            try
            {
                InvoiceResult response = null;

                var eMoit = new Inventec.Common.ElectronicBillMoit.BillMoitManager(login);
                if (eMoit != null)
                {
                    if (Version.Trim() == "2")
                    {
                        response = eMoit.CancelInvoiceTT78(ElectronicBillDataInput.TemplateCode, ElectronicBillDataInput.SymbolCode, ElectronicBillDataInput.InvoiceCode);
                    }
                    else
                    {
                        response = eMoit.CancelInvoice(ElectronicBillDataInput.TemplateCode, ElectronicBillDataInput.InvoiceCode);
                    }
                }

                if (response != null && response.Status)
                {
                    //Thanh cong
                    result.Success = true;
                    result.InvoiceSys = ProviderType.CongThuong;
                    result.Messages = new List<string>() { response.MessLog };
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.CongThuong;
                    result.Messages = new List<string>() { response.MessLog };
                    Inventec.Common.Logging.LogSystem.Error("Huy hoa don dien tu that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ElectronicBillDataInput), ElectronicBillDataInput));
                    ElectronicBillResultUtil.Set(ref result, false, response.MessLog);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetInvoice(Inventec.Common.ElectronicBillMoit.DataInitApi login, ref ElectronicBillResult result)
        {
            try
            {
                InvoiceResult response = null;

                var eMoit = new Inventec.Common.ElectronicBillMoit.BillMoitManager(login);
                if (eMoit != null)
                {
                    if (Version.Trim() == "2")
                    {
                        response = eMoit.GetFileInvoiceTT78(ElectronicBillDataInput.TemplateCode, ElectronicBillDataInput.SymbolCode, ElectronicBillDataInput.InvoiceCode);
                    }
                    else
                    {
                        response = eMoit.GetFileInvoice(ElectronicBillDataInput.TemplateCode, ElectronicBillDataInput.InvoiceCode);
                    }
                }

                if (response != null && response.Status)
                {
                    string fullFileName = ProcessPdfFileResult(response.PdfFile);

                    //Thanh cong
                    result.Success = true;
                    result.InvoiceSys = ProviderType.CongThuong;
                    result.Messages = new List<string>() { response.MessLog };
                    result.InvoiceLink = fullFileName;
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.CongThuong;
                    result.Messages = new List<string>() { response.MessLog };
                    Inventec.Common.Logging.LogSystem.Error("Lay file hoa don dien tu that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ElectronicBillDataInput), ElectronicBillDataInput));
                    ElectronicBillResultUtil.Set(ref result, false, response.MessLog);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessPdfFileResult(string base64string)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(base64string))
                {
                    byte[] data = System.Convert.FromBase64String(base64string);
                    result = ProcessPdfFileResult(data);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ProcessPdfFileResult(byte[] fileToBytes)
        {
            string result = "";
            try
            {
                string tempFileName = Path.GetTempFileName();
                tempFileName = tempFileName.Replace("tmp", "pdf");
                try
                {
                    File.WriteAllBytes(tempFileName, fileToBytes);
                    result = tempFileName;
                }
                catch (Exception ex)
                {
                    result = "";
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessCreateInvoice(Inventec.Common.ElectronicBillMoit.DataInitApi login, ref ElectronicBillResult result)
        {
            try
            {
                List<Invoice> invoices = this.GetInvoiceContrVietSens(this.ElectronicBillDataInput);
                string[] configArr = serviceConfig.Split('|');
                InvoiceResult response = null;

                var eMoit = new Inventec.Common.ElectronicBillMoit.BillMoitManager(login);
                if (eMoit != null)
                {
                    if (Version.Trim() == "2")
                    {
                        response = eMoit.CreateInvoiceTT78(invoices, ElectronicBillDataInput.TemplateCode, ElectronicBillDataInput.SymbolCode, convert);
                    }
                    else
                    {
                        response = eMoit.CreateInvoice(invoices, ElectronicBillDataInput.TemplateCode, ElectronicBillDataInput.SymbolCode, convert);
                    }
                }

                if (response != null && response.Status)
                {
                    //Thanh cong
                    result.Success = true;
                    result.InvoiceSys = ProviderType.CongThuong;
                    result.Messages = new List<string>() { response.MessLog };
                    result.InvoiceCode = response.fKey;
                    result.InvoiceNumOrder = response.NumOrder;
                    result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                    result.InvoiceLoginname = login.User;
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response));
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.CongThuong;
                    result.Messages = new List<string>() { response.MessLog };
                    Inventec.Common.Logging.LogSystem.Error("Tao hoa don dien tu that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ElectronicBillDataInput), ElectronicBillDataInput));
                    ElectronicBillResultUtil.Set(ref result, false, response.MessLog);
                }
            }
            catch (Exception ex)
            {
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<Invoice> GetInvoiceContrVietSens(ElectronicBillDataInput electronicBillDataInput)
        {
            List<Invoice> invoices = new List<Invoice>();

            if (electronicBillDataInput != null)
            {
                Invoice invoice = new Invoice();
                string key = "";
                if (electronicBillDataInput.Transaction != null)
                {
                    key = electronicBillDataInput.Transaction.TRANSACTION_CODE;
                }
                else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                {
                    key = ElectronicBillDataInput.ListTransaction.Select(s => s.TRANSACTION_CODE).OrderBy(o => o).FirstOrDefault();
                }
                else
                {
                    string time = electronicBillDataInput.TransactionTime > 0 ? electronicBillDataInput.TransactionTime.ToString() : Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now).ToString();
                    key = String.Format("{0}-{1}", time, Guid.NewGuid().ToString());
                    if (key.Length > 20) key = key.Substring(0, 20);
                }

                invoice.Key = key;
                invoice.InvoiceDetail = new InvoiceDetail();
                //invoice.InvoiceDetail.PaymentMethod = electronicBillDataInput.PaymentMethod;

                string paymentName = electronicBillDataInput.PaymentMethod;

                if (electronicBillDataInput.Transaction != null)
                {
                    HIS_PAY_FORM payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == electronicBillDataInput.Transaction.PAY_FORM_ID);
                    if (payForm != null)
                    {
                        paymentName = payForm.ELECTRONIC_PAY_FORM_NAME ?? payForm.PAY_FORM_NAME;
                    }
                }

                invoice.InvoiceDetail.PaymentMethod = paymentName;

                InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(electronicBillDataInput);

                invoice.InvoiceDetail.CusCode = adoInfo.BuyerCode;
                invoice.InvoiceDetail.CusAddress = adoInfo.BuyerAddress ?? " ";
                invoice.InvoiceDetail.CusName = adoInfo.BuyerName;
                invoice.InvoiceDetail.CusTaxCode = adoInfo.BuyerTaxCode;
                invoice.InvoiceDetail.CusPhone = adoInfo.BuyerPhone;

                string cusName = adoInfo.BuyerName;

                if (Config.HisConfigCFG.IsSwapNameOption)
                {
                    invoice.InvoiceDetail.Buyer = cusName;
                    invoice.InvoiceDetail.CusName = "";
                }
                else
                {
                    invoice.InvoiceDetail.Buyer = "";
                    invoice.InvoiceDetail.CusName = cusName;
                }

                decimal amount = 0;
                invoice.InvoiceDetail.Products = this.GetProductElectronicBill(ref amount);

                amount = Math.Round(amount - (electronicBillDataInput.Discount ?? 0), 0, MidpointRounding.AwayFromZero);
                invoice.InvoiceDetail.Total = String.Format("{0:0.####}", amount);

                invoice.InvoiceDetail.Amount = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amount));
                invoice.InvoiceDetail.AmountInWords = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amount))) + "đồng";

                //đưa thông tin chiết khấu vào chi tiết
                if (electronicBillDataInput.Discount.HasValue && electronicBillDataInput.Discount.Value > 0)
                {
                    Product pd = new Product();
                    pd.ProdName = "Tiền chiết khấu";
                    pd.ProdPrice = "0";
                    pd.ProdQuantity = "0";
                    pd.Amount = String.Format("{0:0.####}", electronicBillDataInput.Discount.Value);
                    pd.Total = Math.Round(electronicBillDataInput.Discount.Value, 0).ToString();
                    pd.VATAmount = "";
                    pd.VATRate = "-1";
                    invoice.InvoiceDetail.Products.Add(pd);
                }

                //Hien thi thong tin dich vu
                invoices.Add(invoice);
            }

            return invoices;
        }

        private List<Product> GetProductElectronicBill(ref decimal totalAmount)
        {
            List<Product> products = new List<Product>();

            IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, ElectronicBillDataInput);
            var listProduct = iRunTemplate.Run();
            if (listProduct == null)
            {
                throw new Exception("Không có thông tin chi tiết dịch vụ.");
            }

            if (this.TempType != TemplateEnum.TYPE.TemplateNhaThuoc)
            {
                List<ProductBase> listProductBase = (List<ProductBase>)listProduct;
                if (listProductBase == null || listProductBase.Count == 0)
                {
                    throw new Exception("Không có thông tin chi tiết dịch vụ.");
                }

                foreach (var item in listProductBase)
                {
                    Product product = new Product();
                    product.ProdName = item.ProdName;
                    product.ProdUnit = item.ProdUnit;
                    product.ProdQuantity = (item.ProdQuantity ?? 0).ToString();
                    product.Amount = String.Format("{0:0.####}", item.Amount);
                    product.ProdPrice = String.Format("{0:0.####}", item.ProdPrice ?? 0);
                    totalAmount += item.Amount;

                    products.Add(product);
                }
            }

            return products;
        }
    }
}
