using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MInvoice
{
    internal class MInvoiceBehavior : IRun
    {
        private string serviceConfig { get; set; }
        private string accountConfig { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }

        public MInvoiceBehavior(Base.ElectronicBillDataInput electronicBillDataInput, string serviceConfig, string accountConfig)
        {
            this.ElectronicBillDataInput = electronicBillDataInput;
            this.serviceConfig = serviceConfig;
            this.accountConfig = accountConfig;
        }

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
                    LoginDataMinvoice adoLogin = new LoginDataMinvoice();
                    adoLogin.username = accountConfigArr[0].Trim();
                    adoLogin.password = accountConfigArr[1].Trim();
                    adoLogin.ma_dvcs = ma_dvcs;
                    ApiDataResult loginResult = ProcessLogin(serviceUrl, adoLogin);

                    if (loginResult == null || loginResult.error != null)
                    {

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => loginResult), loginResult));
                        ElectronicBillResultUtil.Set(ref result, false, loginResult.error);
                        return result;
                    }
                    switch (electronicBillType)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            TaoHoaDonDienTu(ref result, serviceUrl, adoLogin, loginResult, _templateType);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                        //ChuyenDoiHoaDon(ref result);
                        //break;
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                        //break;
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                        //HuyHoaDon(ref result);
                        //break;
                        case ElectronicBillType.ENUM.CONVERT_INVOICE:
                        //CyberbillChuyenDoiHoaDon(ref result);
                        //break;
                        case ElectronicBillType.ENUM.CREATE_INVOICE_DATA:
                        //break;
                        case ElectronicBillType.ENUM.GET_INVOICE_INFO:
                        //break;
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

        private void TaoHoaDonDienTu(ref ElectronicBillResult result, string serviceUrl, LoginDataMinvoice adoLogin, ApiDataResult loginResult, TemplateEnum.TYPE templateType)
        {
            try
            {
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(CreateInvoice(templateType));
                var createInvoiceResult = Base.ApiConsumerV2.CreateRequest<ApiDataResult>(System.Net.WebRequestMethods.Http.Post, serviceUrl, "api/InvoiceApi78/SaveV2", loginResult.token, adoLogin.ma_dvcs, sendJsonData);
                result.InvoiceSys = ProviderType.MINVOICE;
                if (createInvoiceResult != null && createInvoiceResult.data != null)
                {
                    if (createInvoiceResult.code == "00")
                    {
                        result.Success = true;
                        result.InvoiceCode = createInvoiceResult.data.id;
                        result.InvoiceLookupCode = createInvoiceResult.data.sbmat;
                        result.InvoiceNumOrder = createInvoiceResult.data.shdon.ToString();
                        result.InvoiceLoginname = adoLogin.username;
                        result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(createInvoiceResult.data.tdlap);
                    }
                    else
                    {
                        result.Success = false;
                        string message = createInvoiceResult != null ? createInvoiceResult.error ?? createInvoiceResult.message : "Gửi hóa đơn gốc thất bại";
                        ElectronicBillResultUtil.Set(ref result, false, message);
                    }
                }
                else if (createInvoiceResult == null || !String.IsNullOrWhiteSpace(createInvoiceResult.error) || !String.IsNullOrWhiteSpace(createInvoiceResult.message))
                {
                    result.Success = false;
                    string message = createInvoiceResult != null ? createInvoiceResult.error ?? createInvoiceResult.message : "Gửi hóa đơn gốc thất bại";
                    ElectronicBillResultUtil.Set(ref result, false, message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private InvoiceADO CreateInvoice(TemplateEnum.TYPE templateType)
        {
            InvoiceADO result = new InvoiceADO();
            try
            {
                var inv = InvoiceInfo.InvoiceInfoProcessor.GetData(ElectronicBillDataInput);
                result.editmode = "1"; //1 - tạo mới, 2 - sửa, 3 - xóa
                result.data = new List<InvoiceData>();
                InvoiceData data = new InvoiceData();

                data.tdlap = DateTime.Now;
                data.khieu = ElectronicBillDataInput.TemplateCode + ElectronicBillDataInput.SymbolCode;//Ký hiệu hóa đơn
                data.shdon = "";//Số hoá đơn. Bắt buộc nếu là sửa hoá đơn
                data.sdhang = "";//Số đơn hàng
                data.dvtte = "VND";//Đơn vị tiền tệ
                data.htttoan = ElectronicBillDataInput.PaymentMethod ?? "TM/CK";//Hình thức thanh toán
                data.tnmua = inv.BuyerName;// Tên người mua
                data.mnmua = inv.BuyerCode;//Mã khách hàng
                data.ten = inv.BuyerOrganization;//Tên đơn vị mua
                data.mst = inv.BuyerTaxCode;//Mã số thuế đơn vị mua
                data.dchi = inv.BuyerAddress;//Địa chỉ người mua
                data.email = inv.BuyerEmail;
                data.stknmua = inv.BuyerAccountNumber;//Số tài khoản ngân hàng người mua
                data.cccdan = inv.BuyerCCCD;//Căn cước công dân người mua
                data.sdtnmua = inv.BuyerPhone;//Số điện thoại người mua
                data.details = GetDetail(templateType);
                data.tgtttbso = Math.Round(data.details.First().data.Sum(s => s.tgtien), 0);//Tổng tiền bằng số                
                data.tgtthue = Math.Round(data.details.First().data.Sum(s => s.tthue), 0);//Tổng tiền thuế GTGT
                data.tgtcthue = data.tgtttbso - data.tgtthue;//Tổng tiền trước thuế GTGT
                if (ElectronicBillDataInput.Discount.HasValue && ElectronicBillDataInput.Discount.Value > 0)
                {
                    data.ttcktmai = ElectronicBillDataInput.Discount ?? 0;//Tổng tiền chiết khấu
                }
                data.tgtttbchu = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(data.tgtttbso))) + "đồng"; ;//Tổng tiền bằng chữ
                data.nguoi_thu = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();//Người thu tiền

                result.data.Add(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<InvoiceDetails> GetDetail(TemplateEnum.TYPE templateType)
        {
            List<InvoiceDetails> result = new List<InvoiceDetails>();
            try
            {
                InvoiceDetails invoiceDetails = new InvoiceDetails();
                invoiceDetails.data = new List<InvoiceItem>();

                IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(templateType, ElectronicBillDataInput);
                var listProduct = iRunTemplate.Run();
                if (listProduct == null)
                {
                    throw new Exception("Loi phan tich listProductBase");
                }
                int count = 1;
                if (templateType == TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    var lstProductBasePlus = (List<ProductBasePlus>)listProduct;
                    foreach (var item in lstProductBasePlus)
                    {
                        InvoiceItem ddt = new InvoiceItem();
                        ddt.tchat = 1; //1 - Hàng hóa dịch vụ, 2 - Khuyến mại, 3 - Chiết khấu thương mại, 4 - Ghi chú/ diễn giải, 5 - Hàng hóa đăc trưng
                        ddt.stt = count++.ToString("D4");
                        ddt.ma = item.ProdCode;
                        ddt.ten = item.ProdName;
                        ddt.dvtinh = item.ProdUnit;
                        if (item.ProdQuantity.HasValue)
                        {
                            ddt.sluong = Math.Round(item.ProdQuantity.Value, 0, MidpointRounding.AwayFromZero);
                        }
                        if (item.ProdPrice.HasValue)
                        {
                            ddt.dgia = Math.Round(item.ProdPrice.Value, 0, MidpointRounding.AwayFromZero);
                        }
                        ddt.thtien = item.AmountWithoutTax ?? 0;
                        ddt.tthue = item.TaxAmount ?? 0;
                        ddt.tgtien = item.Amount;
                        if (!item.TaxPercentage.HasValue)
                        {
                            ddt.tsuat = -1;
                        }
                        else
                        {
                            ddt.tsuat = item.TaxConvert;
                        }
                        invoiceDetails.data.Add(ddt);
                    }
                }
                else
                {
                    var lstProductBase = (List<ProductBase>)listProduct;
                    foreach (var item in lstProductBase)
                    {
                        InvoiceItem ddt = new InvoiceItem();
                        ddt.tchat = 1; //1 - Hàng hóa dịch vụ, 2 - Khuyến mại, 3 - Chiết khấu thương mại, 4 - Ghi chú/ diễn giải, 5 - Hàng hóa đăc trưng
                        ddt.stt = count++.ToString("D4");
                        ddt.ma = item.ProdCode;
                        ddt.ten = item.ProdName;
                        ddt.dvtinh = item.ProdUnit;
                        if (item.ProdQuantity.HasValue)
                        {
                            ddt.sluong = Math.Round(item.ProdQuantity.Value, 0, MidpointRounding.AwayFromZero);
                        }
                        if (item.ProdPrice.HasValue)
                        {
                            ddt.dgia = Math.Round(item.ProdPrice.Value, 0, MidpointRounding.AwayFromZero);
                        }
                        ddt.thtien = item.Amount;
                        ddt.tthue = 0;
                        ddt.tgtien = item.Amount;
                        ddt.tsuat = -1;
                        invoiceDetails.data.Add(ddt);
                    }
                }

                result.Add(invoiceDetails);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ApiDataResult ProcessLogin(string serviceUrl, LoginDataMinvoice adoLogin)
        {
            ApiDataResult result = null;
            try
            {
                string uri = "api/Account/Login";
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(adoLogin);
                result = Base.ApiConsumerV2.CreateRequest<ApiDataResult>(System.Net.WebRequestMethods.Http.Post, serviceUrl, uri, null, null, sendJsonData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

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
    }

}
