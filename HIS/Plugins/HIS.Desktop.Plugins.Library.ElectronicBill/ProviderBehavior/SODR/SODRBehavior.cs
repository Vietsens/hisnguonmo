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
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SODR.Process;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.EBillSoftDreams.Model;
using Inventec.Common.EBillSoftDreams.ModelXml;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SODR
{
    class SODRBehavior : IRun
    {
        string serviceConfig { get; set; }
        string accountConfig { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        TemplateEnum.TYPE TempType { get; set; }
        private SoftDreamsProcessor processV2 { get; set; }

        private bool IsV2 { get; set; }

        public SODRBehavior(ElectronicBillDataInput _electronicBillDataInput, string _serviceConfig, string _accountConfig, bool isV2 = false)
            : base()
        {
            this.serviceConfig = _serviceConfig;
            this.ElectronicBillDataInput = _electronicBillDataInput;
            this.accountConfig = _accountConfig;
            this.IsV2 = isV2;
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

                    string[] accountConfigArr = accountConfig.Split('|');

                    Inventec.Common.EBillSoftDreams.DataInitApi login = new Inventec.Common.EBillSoftDreams.DataInitApi();
                    login.Address = serviceUrl;
                    login.User = accountConfigArr[0].Trim();
                    login.Pass = accountConfigArr[1].Trim();

                    if (this.IsV2)
                    {
                        this.processV2 = new SoftDreamsProcessor(serviceUrl, accountConfigArr[0].Trim(), accountConfigArr[1].Trim());
                        if (processV2 != null)
                        {
                            switch (_electronicBillTypeEnum)
                            {
                                case ElectronicBillType.ENUM.CREATE_INVOICE:
                                    result = this.ProcessCreateInvoiceV2();
                                    break;
                                case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                                    result = this.ProcessGetInvoiceV2();
                                    break;
                                case ElectronicBillType.ENUM.DELETE_INVOICE:
                                case ElectronicBillType.ENUM.CANCEL_INVOICE:
                                    result = this.ProcessCancelInvoiceV2();
                                    break;
                            }
                        }
                        else
                        {
                            result.InvoiceSys = ProviderType.SoftDream;
                            ElectronicBillResultUtil.Set(ref result, false, "Lỗi khởi tạo");
                        }
                    }
                    else
                    {
                        switch (_electronicBillTypeEnum)
                        {
                            case ElectronicBillType.ENUM.CREATE_INVOICE:
                                ProcessCreateInvoice(login, ref result);
                                break;
                            case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                                //ProcessGetInvoice(login, ref result);
                                break;
                            case ElectronicBillType.ENUM.DELETE_INVOICE:
                            case ElectronicBillType.ENUM.CANCEL_INVOICE:
                                //ProcessCancelInvoice(login, ref result);
                                break;
                            default:
                                break;
                        }
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
                if (configArr.Length != 2)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");

                string[] accountArr = accountConfig.Split('|');
                if (accountArr.Length != 2)
                    throw new Exception("Sai định dạng cấu hình tài khoản.");
            }
            catch (Exception ex)
            {
                result = false;
                ElectronicBillResultUtil.Set(ref electronicBillResult, false, "");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #region MyRegion v1
        private void ProcessCreateInvoice(Inventec.Common.EBillSoftDreams.DataInitApi login, ref ElectronicBillResult result)
        {
            try
            {
                InvCreate invoices = this.GetInvoice(this.ElectronicBillDataInput);
                Inventec.Common.EBillSoftDreams.Response response = null;
                Inventec.Common.Logging.LogSystem.Info("SendData" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoices), invoices));

                var eMoit = new Inventec.Common.EBillSoftDreams.EBillSoftDreamsManager(login);
                if (eMoit != null)
                {
                    response = eMoit.Run(invoices);
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response));

                if (response != null && response.Success)
                {
                    //Thanh cong
                    result.InvoiceCode = response.Ikey;
                    result.InvoiceNumOrder = response.invoiceNo;
                    result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                    result.InvoiceLoginname = login.User;
                    result.InvoiceLookupCode = response.LookupCode;
                }

                result.InvoiceSys = ProviderType.SoftDream;
                ElectronicBillResultUtil.Set(ref result, response.Success, response.Messages);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private InvCreate GetInvoice(Base.ElectronicBillDataInput electronicBillDataInput)
        {
            InvCreate result = new InvCreate();
            try
            {
                if (electronicBillDataInput != null)
                {
                    InvCreate invoice = new InvCreate();

                    invoice.Pattern = electronicBillDataInput.TemplateCode;
                    invoice.Serial = electronicBillDataInput.SymbolCode;
                    invoice.Inv = GetDataInv(electronicBillDataInput);

                    result = invoice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private Inventec.Common.EBillSoftDreams.ModelXml.Inv GetDataInv(Base.ElectronicBillDataInput electronicBillDataInput)
        {
            Inventec.Common.EBillSoftDreams.ModelXml.Inv result = new Inventec.Common.EBillSoftDreams.ModelXml.Inv();
            try
            {
                Invoice invoice = new Invoice();
                if (electronicBillDataInput != null)
                {
                    string key = "";
                    if (ElectronicBillDataInput.Transaction != null)
                    {
                        key = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                    }
                    else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                    {
                        key = ElectronicBillDataInput.ListTransaction.Select(s => s.TRANSACTION_CODE).OrderBy(o => o).FirstOrDefault();
                    }
                    else
                    {
                        string time = electronicBillDataInput.TransactionTime > 0 ? electronicBillDataInput.TransactionTime.ToString() : Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now).ToString();
                        key = String.Format("{0}-{1}", time, Guid.NewGuid().ToString("N"));
                        if (key.Length > 20) key = key.Substring(0, 20);
                    }

                    invoice.Ikey = key;

                    invoice.ArisingDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(electronicBillDataInput.TransactionTime);

                    string paymentMethod = "T/M";
                    if (electronicBillDataInput.Transaction != null)
                    {
                        if (electronicBillDataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK)
                        {
                            paymentMethod = "C/K";
                        }
                        else if (electronicBillDataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                        {
                            paymentMethod = "TT/D";
                        }
                        else if (electronicBillDataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                        {
                            paymentMethod = "TM/CK";
                        }
                    }
                    else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                    {
                        if (electronicBillDataInput.ListTransaction.First().PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK)
                        {
                            paymentMethod = "C/K";
                        }
                        else if (electronicBillDataInput.ListTransaction.First().PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                        {
                            paymentMethod = "TT/D";
                        }
                        else if (electronicBillDataInput.ListTransaction.First().PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                        {
                            paymentMethod = "TM/CK";
                        }
                    }

                    invoice.PaymentMethod = paymentMethod;

                    InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(electronicBillDataInput);

                    invoice.CusCode = adoInfo.BuyerCode;
                    invoice.CusAddress = adoInfo.BuyerAddress ?? " ";
                    invoice.CusName = adoInfo.BuyerName;
                    invoice.CusTaxCode = adoInfo.BuyerTaxCode;
                    invoice.CusPhone = adoInfo.BuyerPhone;

                    string cusName = adoInfo.BuyerName;

                    if (Config.HisConfigCFG.IsSwapNameOption)
                    {
                        invoice.Buyer = cusName;
                        invoice.CusName = "";
                    }
                    else
                    {
                        invoice.Buyer = "";
                        invoice.CusName = cusName;
                    }

                    invoice.Total = Math.Round(electronicBillDataInput.Amount ?? 0, 0);
                    invoice.VATRate = -1;
                    invoice.VATAmount = 0;
                    invoice.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(electronicBillDataInput.Amount ?? 0);
                    invoice.AmountInWords = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", invoice.Total)) + "đồng";

                    invoice.Products = this.GetProductElectronicBill();

                    //•	Chiết khấu được hiển thị như 1 sản phẩm riêng biệt kế tiếp sản phẩm tương ứng với nó, khi đó ProdName thể hiện là dòng chiết khấu. Các trường Total, VATAmount, Amount mang giá trị âm.
                    if (electronicBillDataInput.Discount.HasValue)
                    {
                        Product discount = new Product();
                        discount.ProdName = "Chiết khấu";
                        discount.ProdPrice -= electronicBillDataInput.Discount.Value;
                        discount.Amount -= electronicBillDataInput.Discount.Value;
                        discount.Total -= electronicBillDataInput.Discount.Value;
                        discount.Extra = "{\"Pos\":\"\"}";
                        invoice.Products.Add(discount);
                    }
                }

                result.Invoice = invoice;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<Product> GetProductElectronicBill()
        {
            List<Product> result = new List<Product>();
            try
            {
                IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, ElectronicBillDataInput);
                var listProduct = iRunTemplate.Run();
                if (listProduct == null)
                {
                    throw new Exception("Loi phan tich listProductBase");
                }

                if (this.TempType != TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    List<ProductBase> listProductBase = (List<ProductBase>)listProduct;
                    if (listProductBase == null || listProductBase.Count == 0)
                    {
                        throw new Exception("Loi phan tich listProductBase");
                    }

                    foreach (var item in listProductBase)
                    {
                        Product product = new Product();
                        product.ProdName = item.ProdName;
                        product.ProdUnit = item.ProdUnit;
                        product.ProdQuantity = item.ProdQuantity;
                        product.Amount = item.Amount;//Tổng tiền
                        product.Total = item.Amount;//Tổng tiền trước thuế
                        product.ProdPrice = item.ProdPrice;

                        result.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion

        #region MyRegion v2
        private ElectronicBillResult ProcessCreateInvoiceV2()
        {
            ElectronicBillResult electronicBillResult = new ElectronicBillResult
            {
                InvoiceSys = ProviderType.SoftDream
            };
            try
            {
                string text = this.ElectronicBillDataInput.TemplateCode + this.ElectronicBillDataInput.SymbolCode;
                IssueCreateV2 invoice = this.GetInvoiceDetailV2();
                ResultDataV2 response = this.processV2.CreateInvoice(invoice);
                if (response != null && response.InvoiceResult != null && response.InvoiceResult.Count == 1)
                {
                    electronicBillResult.Success = true;
                    electronicBillResult.InvoiceCode = response.InvoiceResult.First().Ikey;
                    electronicBillResult.InvoiceNumOrder = response.InvoiceResult.First().No;
                }
                else
                {
                    LogSystem.Error("Tao va phat hanh hoa don that bai. " + LogUtil.TraceData(LogUtil.GetMemberName<ResultDataV2>(() => response), response) + LogUtil.TraceData(LogUtil.GetMemberName<IssueCreateV2>(() => invoice), invoice));
                    ElectronicBillResultUtil.Set(ref electronicBillResult, false, "Tạo và phát hành hóa thất bại");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                ElectronicBillResultUtil.Set(ref electronicBillResult, false, ex.Message);
            }
            return electronicBillResult;
        }

        private IssueCreateV2 GetInvoiceDetailV2()
        {
            IssueCreateV2 issueCreateV = new IssueCreateV2();
            try
            {
                bool flag = this.ElectronicBillDataInput != null;
                if (flag)
                {
                    issueCreateV.Pattern = this.ElectronicBillDataInput.TemplateCode + this.ElectronicBillDataInput.SymbolCode;
                    issueCreateV.Invoices = new List<InvoiceV2>();
                    InvoiceV2 invoiceV = new InvoiceV2();
                    InvoiceInfo.InvoiceInfoADO data = InvoiceInfo.InvoiceInfoProcessor.GetData(this.ElectronicBillDataInput, this.TempType != TemplateEnum.TYPE.Template10);
                    invoiceV.CusAddress = data.BuyerAddress;
                    invoiceV.CusBankName = "";
                    invoiceV.CusBankNo = data.BuyerAccountNumber;
                    invoiceV.CusPhone = data.BuyerPhone;
                    invoiceV.CusName = (data.BuyerName ?? data.BuyerOrganization);
                    invoiceV.CusTaxCode = data.BuyerTaxCode;
                    invoiceV.Email = data.BuyerEmail;
                    invoiceV.PaymentMethod = this.ElectronicBillDataInput.PaymentMethod;
                    if (ElectronicBillDataInput.Transaction != null && !String.IsNullOrEmpty(ElectronicBillDataInput.Transaction.TRANSACTION_CODE))
                    {
                        invoiceV.Ikey = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                    }
                    else if (ElectronicBillDataInput.ListTransaction != null && ElectronicBillDataInput.ListTransaction.Count > 0)
                    {
                        invoiceV.Ikey = ElectronicBillDataInput.ListTransaction.Select(s => s.TRANSACTION_CODE).OrderBy(o => o).FirstOrDefault();
                    }

                    invoiceV.Products = new List<ProductV2>();
                    IRunTemplate runTemplate = TemplateFactory.MakeIRun(this.TempType, this.ElectronicBillDataInput);
                    var obj = runTemplate.Run();
                    int num = 1;

                    if (obj == null)
                    {
                        throw new Exception("Không có thông tin chi tiết dịch vụ.");
                    }

                    if (this.TempType > TemplateEnum.TYPE.TemplateNhaThuoc)
                    {
                        List<ProductBase> list = (List<ProductBase>)obj;
                        if (list == null || list.Count == 0)
                        {
                            throw new Exception("Không có thông tin chi tiết dịch vụ.");
                        }
                        foreach (ProductBase current in list)
                        {
                            ProductV2 item = new ProductV2
                            {
                                No = (num.ToString() ?? ""),
                                Feature = "HHDV",
                                Code = current.ProdCode,
                                Name = current.ProdName,
                                Unit = current.ProdUnit,
                                Quantity = current.ProdQuantity ?? 0,
                                Price = current.ProdPrice ?? 0,
                                Total = current.Amount,
                                VATAmount = decimal.Zero,
                                VATRate = -1f,
                                Amount = current.Amount
                            };
                            invoiceV.Products.Add(item);
                            num++;
                        }
                    }
                    else
                    {
                        List<ProductBasePlus> list2 = (List<ProductBasePlus>)obj;
                        if (list2 == null || list2.Count == 0)
                        {
                            throw new Exception("Không có thông tin chi tiết dịch vụ.");
                        }

                        foreach (ProductBasePlus current2 in list2)
                        {
                            ProductV2 item2 = new ProductV2
                            {
                                No = (num.ToString() ?? ""),
                                Feature = "HHDV",
                                Code = current2.ProdCode,
                                Name = current2.ProdName,
                                Unit = current2.ProdUnit,
                                Quantity = current2.ProdQuantity ?? 0,
                                Price = current2.ProdPrice ?? 0,
                                Total = current2.AmountWithoutTax ?? 0,
                                VATAmount = current2.TaxAmount ?? 0,
                                VATRate = (float)current2.TaxConvert,
                                Amount = current2.Amount
                            };
                            invoiceV.Products.Add(item2);
                            num++;
                        }
                    }

                    if (this.ElectronicBillDataInput.Transaction != null && this.ElectronicBillDataInput.Transaction.EXEMPTION > 0 && this.TempType != TemplateEnum.TYPE.Template10)
                    {
                        ProductV2 item3 = new ProductV2
                        {
                            No = (num.ToString() ?? ""),
                            Feature = "CK",
                            Code = "",
                            Name = "Chiết khấu",
                            Unit = "",
                            Quantity = 0,
                            Price = 0,
                            Total = this.ElectronicBillDataInput.Transaction.EXEMPTION ?? 0,// hiển thị tổng tiền trên mẫu
                            VATAmount = 0,
                            VATRate = -1f,
                            Amount = this.ElectronicBillDataInput.Transaction.EXEMPTION ?? 0,
                            Discount = 0,
                            DiscountAmount = 0
                        };
                        invoiceV.Products.Add(item3);
                    }

                    issueCreateV.Invoices.Add(invoiceV);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return issueCreateV;
        }

        private ElectronicBillResult ProcessCancelInvoiceV2()
        {
            ElectronicBillResult electronicBillResult = new ElectronicBillResult
            {
                InvoiceSys = ProviderType.SoftDream
            };
            try
            {
                string pattern = this.ElectronicBillDataInput.TemplateCode + this.ElectronicBillDataInput.SymbolCode;
                bool response = this.processV2.DeleteInvoice(pattern, this.ElectronicBillDataInput.InvoiceCode);
                bool response2 = response;
                if (response2)
                {
                    electronicBillResult.Success = true;
                }
                else
                {
                    LogSystem.Error("Huy hoa don that bai. " + LogUtil.TraceData(LogUtil.GetMemberName<bool>(() => response), response) + LogUtil.TraceData(LogUtil.GetMemberName<ElectronicBillDataInput>(() => this.ElectronicBillDataInput), this.ElectronicBillDataInput));
                    ElectronicBillResultUtil.Set(ref electronicBillResult, false, "Hủy hóa đơn thất bại");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                ElectronicBillResultUtil.Set(ref electronicBillResult, false, ex.Message);
            }
            return electronicBillResult;
        }

        private ElectronicBillResult ProcessGetInvoiceV2()
        {
            ElectronicBillResult electronicBillResult = new ElectronicBillResult
            {
                InvoiceSys = ProviderType.SoftDream
            };
            try
            {
                string pattern = this.ElectronicBillDataInput.TemplateCode + this.ElectronicBillDataInput.SymbolCode;
                string response = this.processV2.GetInvoice(pattern, this.ElectronicBillDataInput.InvoiceCode);
                bool flag = !string.IsNullOrWhiteSpace(response);
                if (flag)
                {
                    string text = ElectronicBillResultUtil.ProcessPdfFileResult(response);
                    electronicBillResult.Success = true;
                    electronicBillResult.InvoiceLink = text;
                }
                else
                {
                    LogSystem.Error("lay file chuyen doi that bai. " + LogUtil.TraceData(LogUtil.GetMemberName<string>(() => response), response) + LogUtil.TraceData(LogUtil.GetMemberName<ElectronicBillDataInput>(() => this.ElectronicBillDataInput), this.ElectronicBillDataInput));
                    ElectronicBillResultUtil.Set(ref electronicBillResult, false, "Lấy file chuyển đổi thất bại");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                ElectronicBillResultUtil.Set(ref electronicBillResult, false, ex.Message);
            }
            return electronicBillResult;
        }
        #endregion
    }
}
