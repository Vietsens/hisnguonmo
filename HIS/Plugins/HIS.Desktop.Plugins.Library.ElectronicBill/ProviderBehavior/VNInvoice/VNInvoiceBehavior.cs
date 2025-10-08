using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.Adapter;
using Inventec.Common.ElectronicBill.MD;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
using System.Net;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VNInvoice.Model;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VNInvoice
{
    class VNInvoiceBehavior : IRun
    {
        private const int SUCCESS_CODE = 0;
        private string serviceConfig { get; set; }
        private string accountConfig { get; set; }
        private TemplateEnum.TYPE TempType { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }

        public VNInvoiceBehavior(Base.ElectronicBillDataInput electronicBillDataInput, string serviceConfig, string accountConfig)
        {
            this.ElectronicBillDataInput = electronicBillDataInput;
            this.serviceConfig = serviceConfig;
            this.accountConfig = accountConfig;
        }
        InputLoginVNInvoice inputLogin = new InputLoginVNInvoice();
        public ElectronicBillResult Run(ElectronicBillType.ENUM electronicBillType, TemplateEnum.TYPE _templateType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.Check(electronicBillType, ref result))
                {
                    this.TempType = _templateType;
                    string[] configArr = serviceConfig.Split('|');
                    string serviceUrl = configArr[1];
                    //string ma_dvcs = configArr[2];
                    if (String.IsNullOrEmpty(serviceUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay dia chi Webservice URL");
                        ElectronicBillResultUtil.Set(ref result, false, "Không tìm thấy địa chỉ Webservice URL");
                        return result;
                    }
                    string[] accountConfigArr = accountConfig.Split('|');
                    //InputLoginVNInvoice inputLogin = new InputLoginVNInvoice();
                    inputLogin.username = accountConfigArr[0].Trim();
                    inputLogin.password = accountConfigArr[1].Trim();
                    inputLogin.taxCode = this.ElectronicBillDataInput.Branch.TAX_CODE; // cần xem lại
                    //adoLogin.ma_dvcs = ma_dvcs;
                    //ApiDataResult loginResult = ProcessLogin(serviceUrl, adoLogin);
                    OutputLoginVNInvoice outputLogin = new OutputLoginVNInvoice();
                    outputLogin = ProcessLogin(serviceUrl, inputLogin);
                    if (outputLogin == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => outputLogin), outputLogin));
                        //ElectronicBillResultUtil.Set(ref result, false, outputLogin.error);
                        return result;
                    }
                    switch (electronicBillType)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            if (ElectronicBillDataInput.Transaction != null && ElectronicBillDataInput.Transaction.ORIGINAL_TRANSACTION_ID.HasValue)
                            {
                                if(ElectronicBillDataInput.TemplateCode == "1")
                                {
                                    ThayTheHoaDonGTGT(ref result, serviceUrl, inputLogin, outputLogin, _templateType);
                                }
                                else if(ElectronicBillDataInput.TemplateCode == "2")
                                {
                                    ThayTheHoaDonBH(ref result, serviceUrl, inputLogin, outputLogin, _templateType);
                                }
                            }
                            else
                            {
                                if (ElectronicBillDataInput.TemplateCode == "1")
                                {
                                    TaoVaKyHoaDonDienTuGTGT(ref result, serviceUrl, inputLogin, outputLogin, _templateType); // hóa đơn giá trị gia tăng
                                }
                                else if (ElectronicBillDataInput.TemplateCode == "2")
                                {
                                    TaoVaKyHoaDonDienTuBH(ref result, serviceUrl, inputLogin, outputLogin, _templateType); //hóa đơn bán hàng
                                }
                            }
                            break;

                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            if (ElectronicBillDataInput.TemplateCode == "1")
                            {
                                TaiHoaDonChuyenDoiGTGT(ref result, serviceUrl, inputLogin, outputLogin, _templateType);
                            }
                            else if(ElectronicBillDataInput.TemplateCode == "2")
                            {
                                TaiHoaDonChuyenDoiBH(ref result, serviceUrl, inputLogin, outputLogin, _templateType);
                            }
                            break;
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            break;
                        case ElectronicBillType.ENUM.CONVERT_INVOICE:
                            break;
                        case ElectronicBillType.ENUM.CREATE_INVOICE_DATA:
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_INFO:
                            break;
                        default:
                            ElectronicBillResultUtil.Set(ref result, false, "Chưa tích hợp tính năng");
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


        #region tạo và ký hóa đơn điện tử
        private InputCreateAndSignInvoice IEBill()
        {
            InputCreateAndSignInvoice data = new InputCreateAndSignInvoice();
            try
            {
                data.TemplateNo = Convert.ToInt32(ElectronicBillDataInput.TemplateCode);
                data.SerialNo = ElectronicBillDataInput.SymbolCode;
                data.erpId = ElectronicBillDataInput.Transaction.TRANSACTION_CODE; // a nampp bảo thế 
                data.creatorErp = inputLogin.username; // cần xem lại
                data.transactionId = ElectronicBillDataInput.Transaction.ID.ToString();
                data.invoiceDate = DateTime.Now.ToString("yyyy-MM-dd");
                data.paymentMethod = ElectronicBillDataInput.PaymentMethod ?? "TM / CK";
                data.currency = "VND";
                data.exchangeRate = 1;
                data.totalAmount = 0; //tổng tiền hàng chưa vat, sẽ được set lại ở dưới
                data.totalVatAmount = 0; // tổng tiền thuế
                data.totalPaymentAmount = 0;//tổng tiền thanh toán có VAT, sẽ được set lại ở dưới
                data.buyerCode = ElectronicBillDataInput.Transaction.TDL_PATIENT_CODE;
                data.buyerEmail = ElectronicBillDataInput.Transaction.BUYER_EMAIL;
                data.buyerFullName = ElectronicBillDataInput.Transaction.BUYER_NAME;
                data.buyerAddressLine = ElectronicBillDataInput.Transaction.BUYER_ADDRESS;
                data.invoiceDetails = DSCT();
                List<InvoiceTaxBreakdown> dsthuesuat = new List<InvoiceTaxBreakdown>(); // sẽ tự động tính nếu bật cấu hình(với tạo hóa đơn riêng lẻ, còn thằng tạo và ký này không thấy có)
                data.invoiceTaxBreakdowns = dsthuesuat;
                if (data.invoiceDetails != null && data.invoiceDetails.Count > 0)
                {
                    data.totalAmount = data.invoiceDetails.Sum(o => o.amount ?? 0);
                    data.totalPaymentAmount = data.invoiceDetails.Sum(o => o.paymentAmount ?? 0);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return data;
        }
        private void TaoVaKyHoaDonDienTuGTGT(ref ElectronicBillResult result, string serviceUrl, InputLoginVNInvoice inputLogin, OutputLoginVNInvoice outputLogin, TemplateEnum.TYPE templateType)
        {
            try
            {
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(IEBill());
                var createAndSignInvoiceResult = Base.ApiConsumerV2.CreateRequest<OutputCreateAndSignInvoice>(System.Net.WebRequestMethods.Http.Post, serviceUrl, string.Format("/01gtkt/create-batch-and-sign?TemplateNo={0}&serialNo={1}", IEBill().TemplateNo, IEBill().SerialNo), outputLogin.accessToken, null, sendJsonData);
                result.InvoiceSys = ProviderType.VNINVOICE;
                if (createAndSignInvoiceResult != null && createAndSignInvoiceResult.data != null)
                {
                    if (createAndSignInvoiceResult.code == SUCCESS_CODE)
                    {
                        result.Success = true;
                        result.InvoiceCode = createAndSignInvoiceResult.data[0].transactionId;
                        result.InvoiceLoginname = inputLogin.username;
                        result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                    else
                    {
                        result.Success = false;
                        ElectronicBillResultUtil.Set(ref result, false, createAndSignInvoiceResult.message != null ? createAndSignInvoiceResult.message : "Tạo và ký hóa đơn giá trị gia tăng thất bại");
                    }
                }
                else if (createAndSignInvoiceResult == null || (createAndSignInvoiceResult != null && createAndSignInvoiceResult.errors != null))
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, createAndSignInvoiceResult != null && createAndSignInvoiceResult.errors != null ? createAndSignInvoiceResult.errors : "Tạo và ký hóa đơn giá trị gia tăng thất bại");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void TaoVaKyHoaDonDienTuBH(ref ElectronicBillResult result, string serviceUrl, InputLoginVNInvoice inputLogin, OutputLoginVNInvoice outputLogin, TemplateEnum.TYPE templateType)
        {
            try
            {
                //string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(IEBill());
                var data = new List<InputCreateAndSignInvoice> { IEBill() };
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var createAndSignInvoiceResult = Base.ApiConsumerV2.CreateRequest<OutputCreateAndSignInvoice>(System.Net.WebRequestMethods.Http.Post, serviceUrl, string.Format("/02gttt/create-batch-and-sign?TemplateNo={0}&serialNo={1}", IEBill().TemplateNo, IEBill().SerialNo), outputLogin.accessToken, null, sendJsonData);
                result.InvoiceSys = ProviderType.VNINVOICE;
                if (createAndSignInvoiceResult != null)
                {
                    if (createAndSignInvoiceResult.code == SUCCESS_CODE)
                    {
                        result.Success = true;
                        result.InvoiceCode = createAndSignInvoiceResult.data[0].transactionId;
                        result.InvoiceLoginname = inputLogin.username;
                        result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                    else
                    {
                        result.Success = false;
                        ElectronicBillResultUtil.Set(ref result, false, createAndSignInvoiceResult.message != null ? createAndSignInvoiceResult.message : "Tạo và ký hóa đơn bán hàng thất bại");
                    }
                }
                else if (createAndSignInvoiceResult == null || (createAndSignInvoiceResult != null && createAndSignInvoiceResult.errors != null))
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, createAndSignInvoiceResult != null && createAndSignInvoiceResult.errors != null ? createAndSignInvoiceResult.errors : "Tạo và ký hóa đơn bán hàng thất bại");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region chuyển đổi hóa đơn
        private void TaiHoaDonChuyenDoiGTGT(ref ElectronicBillResult result, string serviceUrl, InputLoginVNInvoice inputLogin, OutputLoginVNInvoice outputLogin, TemplateEnum.TYPE templateType)
        {
            try
            {
                if (ElectronicBillDataInput == null || string.IsNullOrEmpty(ElectronicBillDataInput.InvoiceCode))
                    return;
                //string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ICoEBill());
                string erpid = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                var convertInvoiceResult = Base.ApiConsumerV2.CreateRequest<OutputConvertInvoice>(System.Net.WebRequestMethods.Http.Post, serviceUrl, string.Format("/01gtkt/official/{0}", erpid), outputLogin.accessToken, null);
                result.InvoiceSys = ProviderType.VNINVOICE;
                if (convertInvoiceResult != null && convertInvoiceResult.data != null && convertInvoiceResult.id != null)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, "Chuyển đổi hóa đơn thất bại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void TaiHoaDonChuyenDoiBH(ref ElectronicBillResult result, string serviceUrl, InputLoginVNInvoice inputLogin, OutputLoginVNInvoice outputLogin, TemplateEnum.TYPE templateType)
        {
            try
            {
                if (ElectronicBillDataInput == null || string.IsNullOrEmpty(ElectronicBillDataInput.InvoiceCode))
                    return;
                string erpid = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                var convertInvoiceResult = Base.ApiConsumerV2.CreateRequest<OutputConvertInvoice>(System.Net.WebRequestMethods.Http.Post, serviceUrl, string.Format("/02gttt/official/{0}", erpid), outputLogin.accessToken, null);
                result.InvoiceSys = ProviderType.VNINVOICE;
                if (convertInvoiceResult != null && convertInvoiceResult.data != null && convertInvoiceResult.id != null)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, "Chuyển đổi hóa đơn thất bại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        
        #region thay thế hóa đơn
        private InputReplaceInvoice ReplaceInvoice()
        {
            //var inv = InvoiceInfo.InvoiceInfoProcessor.GetData(ElectronicBillDataInput);
            InputReplaceInvoice replaceInvoice = new InputReplaceInvoice();
            try
            {
                replaceInvoice.erpIdReference = ElectronicBillDataInput.Transaction.ORIGINAL_TRANSACTION_ID.ToString();
                replaceInvoice.templateNo = Convert.ToInt32(ElectronicBillDataInput.TemplateCode);
                replaceInvoice.serialNo = ElectronicBillDataInput.SymbolCode;
                replaceInvoice.invoiceNo = ElectronicBillDataInput.NumOrder.ToString(); // cần xem lại
                replaceInvoice.erpId = ElectronicBillDataInput.Transaction.TRANSACTION_CODE; // a nampp bảo thế 
                replaceInvoice.creatorErp = inputLogin.username; // cần xem lại
                replaceInvoice.invoiceDate = DateTime.Now.ToString("yyyy-MM-dd");
                replaceInvoice.note = "";
                replaceInvoice.paymentMethod = ElectronicBillDataInput.PaymentMethod ?? "TM / CK";
                replaceInvoice.currency = "VND";
                replaceInvoice.exchangeRate = 1; // cần xem lại
                replaceInvoice.totalAmount = 0; // sẽ set lại ở dsct, tổng tiền chưa thuế
                replaceInvoice.totalVatAmount = 0;
                replaceInvoice.totalPaymentAmount = 0; //sẽ set lại ở dsct, tổng tiền có thuế 
                replaceInvoice.buyerCode = ElectronicBillDataInput.Transaction.TDL_PATIENT_CODE;
                replaceInvoice.buyerEmail = ElectronicBillDataInput.Transaction.BUYER_EMAIL;
                replaceInvoice.buyerFullName = ElectronicBillDataInput.Transaction.BUYER_NAME;
                replaceInvoice.buyerAddressLine = ElectronicBillDataInput.Transaction.BUYER_ADDRESS;
                replaceInvoice.invoiceDetails = DSCT();
                List<InvoiceTaxBreakdown> dsthuesuat = new List<InvoiceTaxBreakdown>(); // sẽ tự động tính nếu bật cấu hình
                if (replaceInvoice.invoiceDetails != null && replaceInvoice.invoiceDetails.Count > 0)
                {
                    replaceInvoice.totalAmount = replaceInvoice.invoiceDetails.Sum(o => o.amount ?? 0);
                    replaceInvoice.totalPaymentAmount = replaceInvoice.invoiceDetails.Sum(o => o.paymentAmount ?? 0);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return replaceInvoice;
        }

        private List<VNInvoiceDetail> DSCT()
        {
            List<VNInvoiceDetail> dschitiet = new List<VNInvoiceDetail>();
            try
            {
                IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, ElectronicBillDataInput);
                var listProduct = iRunTemplate.Run();
                if (listProduct == null)
                {
                    throw new Exception("Loi phan tich listProductBase");
                }
                if (this.TempType == TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    var lstProductBasePlus = (List<ProductBasePlus>)listProduct;
                    int count = 1;
                    foreach (var item in lstProductBasePlus)
                    {
                        VNInvoiceDetail ddt = new VNInvoiceDetail();
                        ddt.index = count++;
                        ddt.productType = 0;
                        //ddt.khuyenmai = 0;
                        ddt.productCode = item.ProdCode;
                        ddt.productName = item.ProdName;
                        ddt.unitName = item.ProdUnit;
                        if (item.ProdQuantity.HasValue)
                        {
                            ddt.quantity = (double)Math.Round(item.ProdQuantity.Value, 0, MidpointRounding.AwayFromZero);
                        }
                        if (item.ProdPrice.HasValue)
                        {
                            ddt.unitPrice = Math.Round(item.ProdPrice.Value, 0, MidpointRounding.AwayFromZero);
                        }
                        ddt.discountPercentBeforeTax = 0;
                        ddt.discountAmountBeforeTax = 0;
                        //ddt.phikhac_tyle = 0;
                        //ddt.phikhac_sotien = 0;
                        ddt.paymentAmount = item.Amount; //thành tiền sau vat
                        ddt.amount = item.AmountWithoutTax; // thành tiền chưa thuế
                        ddt.vatPercent = 0; // giống cyber
                        ddt.vatAmount = 0;
                        dschitiet.Add(ddt);
                    }
                }
                else
                {
                    int count = 1;
                    var result = (List<ProductBase>)listProduct;
                    foreach (var item in result)
                    {
                        VNInvoiceDetail ddt = new VNInvoiceDetail();
                        ddt.index = count++;
                        ddt.productType = 1;
                        //ddt.khuyenmai = 0;
                        ddt.productCode = item.ProdCode;
                        ddt.productName = item.ProdName;
                        ddt.unitName = item.ProdUnit;
                        ddt.quantity = (double)item.ProdQuantity;
                        ddt.unitPrice = item.ProdPrice;
                        ddt.discountPercentBeforeTax = 0;
                        ddt.discountAmountBeforeTax = 0;
                        //ddt.phikhac_tyle = 0;
                        //ddt.phikhac_sotien = 0;
                        ddt.amount = item.Amount;
                        //ddt.mathue = "-1";
                        ddt.paymentAmount = item.Amount;
                        ddt.vatPercent = 0;
                        dschitiet.Add(ddt);
                    }
                }
            }
            catch (Exception ex)
            {
                dschitiet = new List<VNInvoiceDetail>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return dschitiet;
        }

        private void ThayTheHoaDonGTGT(ref ElectronicBillResult result, string serviceUrl, InputLoginVNInvoice inputLogin, OutputLoginVNInvoice outputLogin, TemplateEnum.TYPE templateType)
        {
            try
            {
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ReplaceInvoice());
                var ReplaceInvoiceResult = Base.ApiConsumerV2.CreateRequest<OutputReplaceInvoice>(System.Net.WebRequestMethods.Http.Post, serviceUrl, "/01gtkt/replace", outputLogin.accessToken, null, sendJsonData);
                result.InvoiceSys = ProviderType.VNINVOICE;
                if (ReplaceInvoiceResult != null && ReplaceInvoiceResult.data != null)
                {
                    if (ReplaceInvoiceResult.code == SUCCESS_CODE)
                    {
                        result.Success = true;
                        result.InvoiceCode = ReplaceInvoiceResult.data[0].erpId;
                        result.InvoiceNumOrder = ReplaceInvoiceResult.data[0].invoiceNo.ToString();

                    }
                    else
                    {
                        result.Success = false;
                        string message = ReplaceInvoiceResult != null ? ReplaceInvoiceResult.errors ?? ReplaceInvoiceResult.message : "Thay thế hóa đơn GTGT thất bại";
                        ElectronicBillResultUtil.Set(ref result, false, message);
                    }
                }
                else if (ReplaceInvoiceResult == null || !String.IsNullOrWhiteSpace(ReplaceInvoiceResult.errors) || !String.IsNullOrWhiteSpace(ReplaceInvoiceResult.message))
                {
                    result.Success = false;
                    string message = ReplaceInvoiceResult != null ? ReplaceInvoiceResult.errors ?? ReplaceInvoiceResult.message : "Thay thế hóa đơn GTGT thất bại";
                    ElectronicBillResultUtil.Set(ref result, false, message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThayTheHoaDonBH(ref ElectronicBillResult result, string serviceUrl, InputLoginVNInvoice inputLogin, OutputLoginVNInvoice outputLogin, TemplateEnum.TYPE templateType)
        {
            try
            {
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ReplaceInvoice());
                var ReplaceInvoiceResult = Base.ApiConsumerV2.CreateRequest<OutputReplaceInvoice>(System.Net.WebRequestMethods.Http.Post, serviceUrl, "/02gttt/replace", outputLogin.accessToken, null, sendJsonData);
                result.InvoiceSys = ProviderType.VNINVOICE;
                if (ReplaceInvoiceResult != null && ReplaceInvoiceResult.data != null)
                {
                    if (ReplaceInvoiceResult.code == SUCCESS_CODE)
                    {
                        result.Success = true;
                        result.InvoiceCode = ReplaceInvoiceResult.data[0].erpId;
                        result.InvoiceNumOrder = ReplaceInvoiceResult.data[0].invoiceNo.ToString();
                    }
                    else
                    {
                        result.Success = false;
                        string message = ReplaceInvoiceResult != null ? ReplaceInvoiceResult.errors ?? ReplaceInvoiceResult.message : "Thay thế hóa đơn bán hàng thất bại";
                        ElectronicBillResultUtil.Set(ref result, false, message);
                    }
                }
                else if (ReplaceInvoiceResult == null || !String.IsNullOrWhiteSpace(ReplaceInvoiceResult.errors) || !String.IsNullOrWhiteSpace(ReplaceInvoiceResult.message))
                {
                    result.Success = false;
                    string message = ReplaceInvoiceResult != null ? ReplaceInvoiceResult.errors ?? ReplaceInvoiceResult.message : "Thay thế hóa đơn bán hàng thất bại";
                    ElectronicBillResultUtil.Set(ref result, false, message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion 
        private bool Check(ElectronicBillType.ENUM _electronicBillTypeEnum, ref ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = serviceConfig.Split('|');
                if (configArr.Length < 3)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");
                if (configArr[0] != ProviderType.VNINVOICE)
                    throw new Exception("Không đúng cấu hình nhà cung cấp VN-invoice");

                string[] accountArr = accountConfig.Split('|');
                if (accountArr.Length != 2)
                    throw new Exception("Sai định dạng cấu hình tài khoản.");

                if (_electronicBillTypeEnum == ElectronicBillType.ENUM.CREATE_INVOICE)
                {
                    if (this.ElectronicBillDataInput == null)
                        throw new Exception("Không có dữ liệu phát hành hóa đơn.");
                    if (this.ElectronicBillDataInput.Treatment == null)
                        throw new Exception("Không có thông tin hồ sơ điều trị.");
                    if (this.ElectronicBillDataInput.Branch == null)
                        throw new Exception("Không có thông tin chi nhánh.");
                }
            }
            catch (Exception ex)
            {
                result = false;
                ElectronicBillResultUtil.Set(ref electronicBillResult, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private OutputLoginVNInvoice ProcessLogin(string serviceUrl, InputLoginVNInvoice inputLogin)
        {
            OutputLoginVNInvoice result = null;
            try
            {
                string uri = "/system/account/login";
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(inputLogin);
                result = Base.ApiConsumerV2.CreateRequest<OutputLoginVNInvoice>(System.Net.WebRequestMethods.Http.Post, serviceUrl, uri, null, null, sendJsonData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
