using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.Logging;

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
                            if (ElectronicBillDataInput.Transaction != null && ElectronicBillDataInput.Transaction.ORIGINAL_TRANSACTION_ID.HasValue)
                            {
                                ThayTheHoaDon(ref result, serviceUrl, adoLogin, loginResult, _templateType);
                            }
                            else
                            {
                                TaoHoaDonDienTu(ref result, serviceUrl, adoLogin, loginResult, _templateType);
                                if (result.InvoiceCode != null && configArr.Count() == 4 && configArr[3] == "1")
                                {
                                    KyHoaDonDienTu(ref result, serviceUrl, adoLogin, loginResult, _templateType);
                                }
                            }
                            break;

                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            InChuyenDoiHoaDon(ref result, serviceUrl, adoLogin, loginResult, _templateType);
                            break;
                        //case ElectronicBillType.ENUM.REPLACE_INVOICE:
                        //    ThayTheHoaDon(ref result, serviceUrl, adoLogin, loginResult, _templateType);
                        //    break;
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
        private void DeleteInvoice(ref ElectronicBillResult result, string serviceUrl, LoginDataMinvoice adoLogin, ApiDataResult loginResult)
        {
            try
            {
                DeleteInvoiceData deleteInvoiceData = new DeleteInvoiceData
                {
                    editmode = "3",
                    data = new List<DeleteDataItem>
                    {
                        new DeleteDataItem
                        {
                            inv_invoiceSeries = ElectronicBillDataInput.TemplateCode + ElectronicBillDataInput.SymbolCode,
                            inv_invoiceAuth_Id = result.hoadon68_id.ToString()
                        }
                    }
                };

                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(deleteInvoiceData);
                var deleteInvoiceResult = Base.ApiConsumerV2.CreateRequest<DeleteInvoiceResult>(System.Net.WebRequestMethods.Http.Post, serviceUrl, "api/InvoiceApi78/SaveV2", loginResult.token, adoLogin.ma_dvcs, sendJsonData
                );

                if (deleteInvoiceResult != null && deleteInvoiceResult.ok && deleteInvoiceResult.code == "00")
                {
                    Inventec.Common.Logging.LogSystem.Info(
                        string.Format("Xóa hóa đơn thành công: inv_invoiceSeries={0}, inv_invoiceAuth_Id={1}",
                            deleteInvoiceData.data[0].inv_invoiceSeries,
                            deleteInvoiceData.data[0].inv_invoiceAuth_Id)
                    );
                }
                else
                {
                    string message = deleteInvoiceResult != null ? deleteInvoiceResult.message ?? "Xóa hóa đơn thất bại" : "Xóa hóa đơn thất bại";
                    Inventec.Common.Logging.LogSystem.Error(
                        string.Format("Xóa hóa đơn thất bại: {0}", message)
                    );
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    string.Format("Lỗi khi gọi API xóa hóa đơn: {0}", ex.Message)
                );
            }
            ElectronicBillResultUtil.Set(ref result, false, "Ký hóa đơn thất bại và đã xóa hóa đơn");
        }

        private void KyHoaDonDienTu(ref ElectronicBillResult result, string serviceUrl, LoginDataMinvoice adoLogin, ApiDataResult loginResult, TemplateEnum.TYPE templateType)
        {
            try
            {
                var signData = new { hoadon68_id = result.InvoiceCode };
                string signJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(signData);
                ApiSignDataResult signResult = new ApiSignDataResult();
                try
                {
                    signResult = Base.ApiConsumerV2.CreateRequest<ApiSignDataResult>(System.Net.WebRequestMethods.Http.Post, serviceUrl, "api/InvoiceApi78/Sign", loginResult.token, adoLogin.ma_dvcs, signJsonData);
                }
                catch (Exception ex)
                {
                    LogSystem.Error("Lỗi khi gọi API ký hóa đơn: " + ex);
                    DeleteInvoice(ref result, serviceUrl, adoLogin, loginResult);
                    return;
                }
                if (signResult == null || signResult.code != "00" || signResult.data == null)
                {
                    string message = signResult != null ? signResult.message : "Ký hóa đơn thất bại";
                    ElectronicBillResultUtil.Set(ref result, false, message);
                    Inventec.Common.Logging.LogSystem.Error(string.Format("Ký hóa đơn thất bại: {0}", message));
                    DeleteInvoice(ref result, serviceUrl, adoLogin, loginResult);
                    return;
                }

                if (signResult.data.data == null || signResult.data.data.tthai != "Đã gửi")
                {
                    ElectronicBillResultUtil.Set(ref result, false, "Hóa đơn chưa được gửi CQT");
                    Inventec.Common.Logging.LogSystem.Error("Hóa đơn chưa được gửi CQT");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex.Message);
            }
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
                        result.hoadon68_id = Guid.Parse(createInvoiceResult.data.hoadon68_id);

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

                Dictionary<string, string> dicTreatmentValues = Base.General.ProcessDicValueString(ElectronicBillDataInput);
                data.thongtin_khoa = dicTreatmentValues["CURRENT_ROOM_DEPARTMENT"];

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
        #region in chuyển đổi hóa đơn
        private void InChuyenDoiHoaDon(ref ElectronicBillResult result, string serviceUrl, LoginDataMinvoice adoLogin, ApiDataResult loginResult, TemplateEnum.TYPE templateType)
        {
            try
            {
                string uri = string.Format("api/InvoiceApi78/PrintInvoice?id={0}", ElectronicBillDataInput.Transaction.INVOICE_CODE);
                var printResult = Base.ApiConsumerV2.CreateRequestGetByte(System.Net.WebRequestMethods.Http.Get, serviceUrl, uri, loginResult.token, adoLogin.ma_dvcs);

                if (printResult != null && printResult.Length > 0)
                {
                    result.Success = true;
                    result.InvoiceSys = ProviderType.MINVOICE;
                    result.PdfFileData = printResult;
                    string dic = Application.StartupPath + @"\temp";
                    if (!Directory.Exists(dic))
                        Directory.CreateDirectory(dic);
                    string fullName = dic + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    File.WriteAllBytes(fullName, printResult);
                    result.InvoiceLink = fullName;
                    Inventec.Common.Logging.LogSystem.Info("Nhận dữ liệu PDF thành công");
                }
                else
                {
                    ElectronicBillResultUtil.Set(ref result, false, "Không nhận được dữ liệu PDF từ API");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                ElectronicBillResultUtil.Set(ref result, false, "Lỗi khi gọi API in hóa đơn: " + ex.Message);
            }
        }
        #endregion
        #region thay thế hóa đơn
        private ReplaceInvoice ReplaceInvoice()
        {
            var inv = InvoiceInfo.InvoiceInfoProcessor.GetData(ElectronicBillDataInput);
            ReplaceInvoice replaceInvoice = new ReplaceInvoice();
            replaceInvoice.mode = 1;
            replaceInvoice.data = new List<RpInvoiceData>();
            RpInvoiceData data = new RpInvoiceData();
            data.inv_originalId = ElectronicBillDataInput.Transaction.TDL_ORIGINAL_EI_CODE;
            data.ngayvb = DateTime.Now.ToString("dd/MM/yyyy");
            //data.inv_originalId = ElectronicBillDataInput.InvoiceCode;
            data.inv_invoiceSeries = ElectronicBillDataInput.TemplateCode + ElectronicBillDataInput.SymbolCode;
            data.inv_invoiceIssuedDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(ElectronicBillDataInput.Transaction.TRANSACTION_DATE);
            data.inv_currencyCode = "VND";
            data.inv_exchangeRate = 1;
            data.inv_buyerAddressLine = inv.BuyerAddress;
            data.inv_paymentMethodName = ElectronicBillDataInput.PaymentMethod ?? "TM / CK";
            data.details = new List<RpInvoiceDetails>();
            RpInvoiceDetails RpDeatails = new RpInvoiceDetails();
            RpDeatails.data = new List<RpInvoiceItem>();
            RpInvoiceItem RpItem = new RpInvoiceItem();
            RpItem.stt = ElectronicBillDataInput.NumOrder.ToString();
            RpItem.inv_itemName = inv.BuyerOrganization;
            RpItem.ma_thue = ElectronicBillDataInput.Branch.TAX_CODE;
            RpDeatails.data.Add(RpItem);
            data.details.Add(RpDeatails);
            replaceInvoice.data.Add(data);
            return replaceInvoice;
        }

        private void ThayTheHoaDon(ref ElectronicBillResult result, string serviceUrl, LoginDataMinvoice adoLogin, ApiDataResult loginResult, TemplateEnum.TYPE templateType)
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
