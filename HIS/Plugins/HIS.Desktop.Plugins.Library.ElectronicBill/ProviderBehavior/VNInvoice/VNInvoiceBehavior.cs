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
        private const string SUCCESS_CODE = "01";
        private string serviceConfig { get; set; }
        private string accountConfig { get; set; }
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
                    string[] configArr = serviceConfig.Split('|');
                    string serviceUrl = configArr[1];
                    string ma_dvcs = configArr[2];
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
                    //inputLogin.taxCode =
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
                                ThayTheHoaDon(ref result, serviceUrl, inputLogin, outputLogin, _templateType);
                            }
                            else
                            {
                                TaoHoaDonDienTu(ref result, serviceUrl, inputLogin, outputLogin, _templateType);
                                if (result.InvoiceCode != null && configArr.Count() == 4 && configArr[3] == "1")
                                {
                                    KyHoaDonDienTu(ref result, serviceUrl, inputLogin, outputLogin, _templateType);
                                }
                            }
                            break;

                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            TaiHoaDonChuyenDoi(ref result, serviceUrl, inputLogin, outputLogin, _templateType);
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

        private void TaiHoaDonChuyenDoi(ref ElectronicBillResult result, string serviceUrl, object adoLogin, object loginResult, TemplateEnum.TYPE templateType)
        {
            throw new NotImplementedException();
        }

        private void KyHoaDonDienTu(ref ElectronicBillResult result, string serviceUrl, object adoLogin, object loginResult, TemplateEnum.TYPE templateType)
        {
            throw new NotImplementedException();
        }

        private void TaoHoaDonDienTu(ref ElectronicBillResult result, string serviceUrl, object adoLogin, object loginResult, TemplateEnum.TYPE templateType)
        {
            throw new NotImplementedException();
        }
        #region thay thế hóa đơn
        private InputReplaceInvoice ReplaceInvoice()
        {
            //var inv = InvoiceInfo.InvoiceInfoProcessor.GetData(ElectronicBillDataInput);
            InputReplaceInvoice replaceInvoice = new InputReplaceInvoice();
            replaceInvoice.erpIdReference = ElectronicBillDataInput.Transaction.ORIGINAL_TRANSACTION_ID.ToString();
            replaceInvoice.templateNo = Convert.ToInt32(ElectronicBillDataInput.TemplateCode);
            replaceInvoice.serialNo = ElectronicBillDataInput.SymbolCode;
            replaceInvoice.invoiceNo = ElectronicBillDataInput.NumOrder.ToString(); // cần xem lại
            replaceInvoice.erpId = ElectronicBillDataInput.Transaction.TRANSACTION_CODE; // a nampp bảo thế 
            replaceInvoice.creatorErp = inputLogin.username; // cần xem lại
            replaceInvoice.invoiceDate = DateTime.Now;
            replaceInvoice.note = "";
            replaceInvoice.paymentMethod = ElectronicBillDataInput.PaymentMethod ?? "TM / CK";
            replaceInvoice.currency = "VND";
            replaceInvoice.exchangeRate = 1; // cần xem lại
            replaceInvoice.totalAmount = ElectronicBill;
            replaceInvoice.totalVatAmount = ;
            List<InvoiceTaxBreakdown> dsthuesuat = new List<InvoiceTaxBreakdown>();
            if (replaceInvoice.dschitiet != null && hd.dschitiet.Count > 0)
            {
                hd.tongtien_chuavat = hd.dschitiet.Sum(o => o.tongtien_chuathue ?? 0);
                hd.tongtien_covat = hd.dschitiet.Sum(o => o.tongtien_cothue ?? 0);
                var groupByMaThue = hd.dschitiet.GroupBy(o => o.mathue).ToList();
                foreach (var item in groupByMaThue)
                {
                    DanhSachThue st = new DanhSachThue();
                    st.mathue = item.First().mathue;
                    st.tongtien_chiuthue = item.Sum(o => o.tongtien_chuathue ?? 0);
                    st.tongtien_thue = item.Sum(o => o.tongtien_cothue ?? 0) - item.Sum(o => o.tongtien_chuathue ?? 0);
                    dsthuesuat.Add(st);
                }
            }
            return replaceInvoice;
        }
        private void ThayTheHoaDon(ref ElectronicBillResult result, string serviceUrl, InputLoginVNInvoice inputLogin, OutputLoginVNInvoice outputLogin, TemplateEnum.TYPE templateType)
        {
            try
            {
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ReplaceInvoice());
                var ReplaceInvoiceResult = Base.ApiConsumerV2.CreateRequest<ApiDataResult>(System.Net.WebRequestMethods.Http.Post, serviceUrl, "api/InvoiceApi78/ThayTheSaveSign", loginResult.token, adoLogin.ma_dvcs, sendJsonData);
                result.InvoiceSys = ProviderType.MINVOICE;
                if (ReplaceInvoiceResult != null && ReplaceInvoiceResult.data != null)
                {
                    if (ReplaceInvoiceResult.code == "00")
                    {
                        result.Success = true;
                        result.InvoiceCode = ReplaceInvoiceResult.data.id;
                        result.InvoiceLookupCode = ReplaceInvoiceResult.data.sbmat;
                        result.InvoiceNumOrder = ReplaceInvoiceResult.data.shdon.ToString();
                        result.InvoiceLoginname = adoLogin.username;
                        result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ReplaceInvoiceResult.data.tdlap);
                        result.hoadon68_id = Guid.Parse(ReplaceInvoiceResult.data.hoadon68_id);

                    }
                    else
                    {
                        result.Success = false;
                        string message = ReplaceInvoiceResult != null ? ReplaceInvoiceResult.error ?? ReplaceInvoiceResult.message : "Gửi hóa đơn gốc thất bại";
                        ElectronicBillResultUtil.Set(ref result, false, message);
                    }
                }
                else if (ReplaceInvoiceResult == null || !String.IsNullOrWhiteSpace(ReplaceInvoiceResult.error) || !String.IsNullOrWhiteSpace(ReplaceInvoiceResult.message))
                {
                    result.Success = false;
                    string message = ReplaceInvoiceResult != null ? ReplaceInvoiceResult.error ?? ReplaceInvoiceResult.message : "Gửi hóa đơn gốc thất bại";
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
                if (configArr[0] != ProviderType.MINVOICE)
                    throw new Exception("Không đúng cấu hình nhà cung cấp M-invoice");

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

        private OutputLoginVNInvoice ProcessLogin(string serviceUrl, InputLoginVNInvoice adoLogin)
        {
            OutputLoginVNInvoice result = null;
            try
            {
                string uri = "api/system/account/login";
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(adoLogin);
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
