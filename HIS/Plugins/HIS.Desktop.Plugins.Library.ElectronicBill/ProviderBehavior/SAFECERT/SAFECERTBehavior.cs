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
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SAFECERT.ADO;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.Logging;
using Inventec.Common.TypeConvert;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SAFECERT
{
    class SAFECERTBehavior : Base.IRun
    {
        private Base.ElectronicBillDataInput ElectronicBillDataInput;
        private string serviceConfigStr;
        private string accountConfigStr;
        private static ServiceConfig serviceConfig = new ServiceConfig();
        TemplateEnum.TYPE TempType;

        public SAFECERTBehavior(Base.ElectronicBillDataInput electronicBillDataInput, string serviceConfig, string accountConfig)
        {
            // TODO: Complete member initialization
            this.ElectronicBillDataInput = electronicBillDataInput;
            this.serviceConfigStr = serviceConfig;
            this.accountConfigStr = accountConfig;
        }

        Base.ElectronicBillResult Base.IRun.Run(Base.ElectronicBillType.ENUM _electronicBillTypeEnum, Template.TemplateEnum.TYPE templateType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                this.TempType = templateType;

                if (this.Check(ref result))
                {
                    switch (_electronicBillTypeEnum)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            ProcessCreateInvoice(ref result);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            ElectronicBillResultUtil.Set(ref result, false, "Chưa tích hợp tính năng này");
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

        private void ProcessCreateInvoice(ref ElectronicBillResult billResult)
        {
            try
            {
                if (this.ElectronicBillDataInput != null)
                {
                    string[] configArr = serviceConfigStr.Split('|');

                    string idMaster = "";
                    if (this.ElectronicBillDataInput.Transaction != null)
                    {
                        idMaster = this.ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                    }
                    else if (this.ElectronicBillDataInput.ListTransaction != null && this.ElectronicBillDataInput.ListTransaction.Count > 0)
                    {
                        List<string> trasactionCode = this.ElectronicBillDataInput.ListTransaction.Select(s => s.TRANSACTION_CODE).ToList();

                        idMaster = trasactionCode.OrderBy(o => o).First();
                    }

                    EInvoice invoice = new EInvoice();
                    invoice.idCompany = this.ElectronicBillDataInput.Branch.TAX_CODE.Replace("-", "");

                    invoice.idMaster = idMaster;
                    invoice.sodonhang = idMaster;

                    InvoiceInfo.InvoiceInfoADO ado = InvoiceInfo.InvoiceInfoProcessor.GetData(this.ElectronicBillDataInput);
                    //ado luôn trả ra khác null

                    invoice.diachi = ado.BuyerAddress ?? "";
                    invoice.dienthoainguoimua = ado.BuyerPhone ?? "";
                    invoice.tenkhachhang = ado.BuyerName;
                    invoice.tendonvi = ado.BuyerOrganization ?? "";
                    invoice.masothue = ado.BuyerTaxCode ?? "";
                    invoice.emailnguoimua = ado.BuyerEmail ?? "";
                    invoice.faxnguoimua = "";
                    invoice.maKhachHang = ado.BuyerCode;

                    invoice.hinhthuctt = ado.PaymentMethod ?? "TM/CK";

                    string ngayGiaoDich = GetDate(ado.TransactionTime);

                    invoice.ngaydonhang = ngayGiaoDich;
                    invoice.ngayhopdong = ngayGiaoDich;

                    invoice.noimotaikhoan = "";
                    invoice.socontainer = "";
                    invoice.sohopdong = "";
                    invoice.sotaikhoan = "";
                    invoice.sovandon = "";
                    if (this.ElectronicBillDataInput.Transaction != null)
                    {
                        V_HIS_CASHIER_ROOM room = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == ElectronicBillDataInput.Transaction.CASHIER_ROOM_ID);
                        if (room != null)
                        {
                            invoice.macnCh = room.EINVOICE_ROOM_CODE ?? "";
                            invoice.tencnCh = room.EINVOICE_ROOM_NAME ?? "";
                        }
                    }
                    invoice.hoaDonMoRong = "";

                    invoice.loaiTienTe = "VND";
                    invoice.tyGia = 1m;
                    invoice.soHieuBangKe = "";

                    invoice.listEinvoiceLine = this.GetEinvoiceLine(idMaster, serviceConfig.KeyTrans);

                    ProcessTongTien(invoice);

                    ApiObject apiData = new ApiObject();
                    apiData.einvoiceHeaderObj = invoice;
                    apiData.keyTrans = serviceConfig.KeyTrans;
                    apiData.masoThue = this.ElectronicBillDataInput.Branch.TAX_CODE.Replace("-", "");

                    try
                    {
                        var apiresult = ApiConsumer.CreateRequest<ApiResultObject>(serviceConfig.BaseUrl, serviceConfig.RequestUrl, apiData);
                        if (apiresult != null)
                        {
                            if (apiresult.maKetQua == "XHD_1111")
                            {
                                billResult.Success = true;
                                billResult.InvoiceSys = ProviderType.safecert;
                                billResult.InvoiceCode = string.Format("{0}|{1}", ProviderType.safecert, this.ElectronicBillDataInput.Transaction.NUM_ORDER);
                                billResult.InvoiceNumOrder = "";
                                billResult.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                            }
                            else
                            {
                                ElectronicBillResultUtil.Set(ref billResult, false, string.Format("{0}. {1}", apiresult.maKetQua, apiresult.moTaKetQua));
                                LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
                            }
                        }
                        else
                        {
                            ElectronicBillResultUtil.Set(ref billResult, false, "");
                            LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
                        }
                    }
                    catch (Exception ex)
                    {
                        ElectronicBillResultUtil.Set(ref billResult, false, ex.Message);
                        LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTongTien(EInvoice invoice)
        {
            try
            {
                if (invoice != null && invoice.listEinvoiceLine != null && invoice.listEinvoiceLine.Count > 0)
                {
                    invoice.tongsauvat05 = "";
                    invoice.tongtruocvat05 = "";
                    invoice.tongvat05 = "";
                    invoice.tongsauvat10 = "";
                    invoice.tongtruocvat10 = "";
                    invoice.tongvat10 = "";

                    foreach (var item in invoice.listEinvoiceLine)
                    {
                        if (!String.IsNullOrWhiteSpace(item.thuesuat))
                        {
                            if (String.IsNullOrWhiteSpace(invoice.tongsauvat05))
                                invoice.tongsauvat05 = "0";
                            if (String.IsNullOrWhiteSpace(invoice.tongtruocvat05))
                                invoice.tongtruocvat05 = "0";
                            if (String.IsNullOrWhiteSpace(invoice.tongvat05))
                                invoice.tongvat05 = "0";
                            if (String.IsNullOrWhiteSpace(invoice.tongsauvat10))
                                invoice.tongsauvat10 = "0";
                            if (String.IsNullOrWhiteSpace(invoice.tongtruocvat10))
                                invoice.tongtruocvat10 = "0";
                            if (String.IsNullOrWhiteSpace(invoice.tongvat10))
                                invoice.tongvat10 = "0";

                            switch (item.thuesuat)
                            {
                                case "5":
                                    invoice.tongsauvat05 = "" + Math.Round(Parse.ToDecimal(item.tongtien) + Parse.ToDecimal(invoice.tongsauvat05), 0);
                                    invoice.tongtruocvat05 = "" + Math.Round(Parse.ToDecimal(item.thanhtien) + Parse.ToDecimal(invoice.tongtruocvat05), 0);
                                    invoice.tongvat05 = "" + Math.Round(Parse.ToDecimal(item.tienthue) + Parse.ToDecimal(invoice.tongvat05), 0);
                                    break;
                                case "10":
                                    invoice.tongsauvat10 = "" + Math.Round(Parse.ToDecimal(item.tongtien) + Parse.ToDecimal(invoice.tongsauvat10), 0);
                                    invoice.tongtruocvat10 = "" + Math.Round(Parse.ToDecimal(item.thanhtien) + Parse.ToDecimal(invoice.tongtruocvat10), 0);
                                    invoice.tongvat10 = "" + Math.Round(Parse.ToDecimal(item.tienthue) + Parse.ToDecimal(invoice.tongvat10), 0);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            invoice.tongkhongchiuvat = "" + Math.Round(Parse.ToDecimal(item.tongtien) + Parse.ToDecimal(invoice.tongkhongchiuvat), 0);
                        }

                        invoice.tongtienchuathue = "" + Math.Round(Parse.ToDecimal(item.thanhtien) + Parse.ToDecimal(invoice.tongtienchuathue), 0);
                        invoice.tongtienthue = "" + Math.Round(Parse.ToDecimal(item.tienthue) + Parse.ToDecimal(invoice.tongtienthue), 0);
                        invoice.tongtientt = "" + Math.Round(Parse.ToDecimal(item.tongtien) + Parse.ToDecimal(invoice.tongtientt), 0);
                    }

                    invoice.tienchiphikhac = "";
                    invoice.tongtienckgg = "";

                    invoice.sotienbangchu = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", invoice.tongtientt)) + "đồng";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<EinvoiceLine> GetEinvoiceLine(string idMaster, string keytrans)
        {
            List<EinvoiceLine> result = new List<EinvoiceLine>();
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
                    int count = 1;
                    foreach (var item in listProductBase)
                    {
                        EinvoiceLine line = new EinvoiceLine();
                        line.idMaster = idMaster;
                        line.mahang = item.ProdCode;
                        line.tenhang = item.ProdName;
                        line.donvitinh = item.ProdUnit;
                        line.sothutu = count.ToString();
                        line.sothutuIdx = count;
                        if (item.ProdQuantity.HasValue)
                        {
                            line.soluong = "" + Math.Round(item.ProdQuantity.Value, 0, MidpointRounding.AwayFromZero);
                        }

                        if (item.ProdPrice.HasValue)
                        {
                            line.dongia = "" + Math.Round(item.ProdPrice.Value, 0, MidpointRounding.AwayFromZero);
                        }

                        line.thanhtien = "" + Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero);
                        line.tongtien = "" + Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero);

                        line.loaihhdv = item.Type == 1 ? "THUOC" : "KCB";

                        line.thuesuat = "KCT";
                        line.tienthue = "0";
                        line.thuettdb = "0";
                        line.tinhchat = 1;  //mặc định

                        result.Add(line);
                        count++;
                    }
                }
                else
                {
                    List<ProductBasePlus> listProductBase = (List<ProductBasePlus>)listProduct;
                    int count = 1;
                    foreach (var item in listProductBase)
                    {
                        EinvoiceLine line = new EinvoiceLine();
                        line.idMaster = idMaster;
                        line.mahang = item.ProdCode;
                        line.tenhang = item.ProdName;
                        line.donvitinh = item.ProdUnit;
                        line.sothutu = count.ToString();
                        line.sothutuIdx = count;
                        line.loaihhdv = "THUOC";
                        if (item.ProdQuantity.HasValue)
                        {
                            line.soluong = "" + Math.Round(item.ProdQuantity.Value, 0, MidpointRounding.AwayFromZero);
                        }

                        if (item.ProdPrice.HasValue)
                        {
                            line.dongia = "" + Math.Round(item.ProdPrice.Value, 0, MidpointRounding.AwayFromZero);
                        }

                        line.tongtien = "" + Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero);

                        line.thuettdb = "0";

                        line.tienthue = "" + Math.Round(item.TaxAmount ?? 0, 0, MidpointRounding.AwayFromZero);
                        line.thanhtien = "" + Math.Round(item.AmountWithoutTax ?? 0, 0, MidpointRounding.AwayFromZero);

                        if (!item.TaxPercentage.HasValue)
                        {
                            line.thuesuat = "KCT";
                        }
                        else if (item.TaxPercentage == 1)
                        {
                            line.thuesuat = "5";
                        }
                        else if (item.TaxPercentage == 2)
                        {
                            line.thuesuat = "10";
                        }
                        else if (item.TaxPercentage == 3)
                        {
                            line.thuesuat = "8";
                        }
                        else if (item.TaxPercentage == 0)
                        {
                            line.thuesuat = "0";
                        }

                        line.tinhchat = 1;  //mặc định

                        result.Add(line);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<EinvoiceLine>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string GetDate(long time)
        {
            string result = "";
            try
            {
                string temp = time.ToString();
                if (temp != null && temp.Length >= 8)
                {
                    result = new StringBuilder().Append(temp.Substring(0, 4)).Append("-").Append(temp.Substring(4, 2)).Append("-").Append(temp.Substring(6, 2)).ToString();
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        
        private bool Check(ref ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = serviceConfigStr.Split('|');
                if (configArr.Length != 3)
                    throw new Exception("Sai định dạng cấu hình hệ thống");
                if (configArr[0] != ProviderType.safecert)
                    throw new Exception("Không đúng cấu hình nhà cung cấp safecert");
                if (String.IsNullOrEmpty(configArr[1]))
                    throw new Exception("Chưa khai báo địa chỉ phát hành");

                string[] configArr_Account = accountConfigStr.Split('|');
                if (configArr_Account.Length != 2 || configArr_Account.Any(o => String.IsNullOrWhiteSpace(o)))
                    throw new Exception("Sai thông tin cấu hình tài khoản phát hành hóa đơn");

                if (ElectronicBillDataInput.Branch == null || string.IsNullOrWhiteSpace(ElectronicBillDataInput.Branch.TAX_CODE))
                {
                    throw new Exception("Chưa khai báo thông tin mã số thuế của viện");
                }
                string uriString = configArr[1].Replace("<#USER;>", configArr_Account[0])
                                                .Replace("<#PASS;>", configArr_Account[1])
                                                .Replace("<#TAX_CODE;>", this.ElectronicBillDataInput.Branch.TAX_CODE);
                Uri serviceUrl;
                if (!Uri.TryCreate(uriString, UriKind.Absolute, out serviceUrl))
                    throw new Exception("Sai định dạng cấu hình hệ thống địa chỉ phát hành");
                var baseUri = serviceUrl.GetLeftPart(System.UriPartial.Authority);

                if (String.IsNullOrEmpty(baseUri))
                    throw new Exception("Không tìm thấy địa chỉ Webservice URL");

                serviceConfig.BaseUrl = baseUri;
                serviceConfig.RequestUrl = uriString.Substring(baseUri.Length);
                serviceConfig.KeyTrans = configArr[2];

                if (ElectronicBillDataInput.ListTransaction != null && ElectronicBillDataInput.ListTransaction.Count > 0)
                {
                    var accountBook = ElectronicBillDataInput.ListTransaction.Where(o => String.IsNullOrWhiteSpace(o.SYMBOL_CODE) || String.IsNullOrWhiteSpace(o.TEMPLATE_CODE)).ToList();
                    if (accountBook != null && accountBook.Count > 0)
                    {
                        throw new Exception(string.Format("Các sổ {0} chưa có thông tin mẫu số, ký hiệu", string.Join(", ", accountBook.Select(s => s.ACCOUNT_BOOK_NAME).Distinct().ToList())));
                    }

                    var diffTemplate = ElectronicBillDataInput.ListTransaction.Where(o => o.TEMPLATE_CODE != ElectronicBillDataInput.TemplateCode).ToList();
                    if (diffTemplate != null && diffTemplate.Count > 0)
                    {
                        string accountBookDiff = "";
                        List<string> diff = new List<string>();
                        foreach (var item in diffTemplate)
                        {
                            diff.Add(string.Format("{0}({1})", item.ACCOUNT_BOOK_NAME, item.TEMPLATE_CODE));
                        }

                        accountBookDiff = string.Join(", ", diff.Distinct().ToList());

                        throw new Exception(string.Format("Các sổ {0} có thông tin mẫu số khác nhau. Vui lòng kiểm tra lại.", accountBookDiff));
                    }

                    var diffSymbol = ElectronicBillDataInput.ListTransaction.Where(o => o.SYMBOL_CODE != ElectronicBillDataInput.SymbolCode).ToList();
                    if (diffSymbol != null && diffSymbol.Count > 0)
                    {
                        string accountBookDiff = "";
                        List<string> diff = new List<string>();
                        foreach (var item in diffSymbol)
                        {
                            diff.Add(string.Format("{0}({1})", item.ACCOUNT_BOOK_NAME, item.SYMBOL_CODE));
                        }

                        accountBookDiff = string.Join(", ", diff.Distinct().ToList());

                        throw new Exception(string.Format("Các sổ {0} có thông tin ký hiệu khác nhau. Vui lòng kiểm tra lại.", accountBookDiff));
                    }
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

        private class ServiceConfig
        {
            public string BaseUrl { get; set; }
            public string RequestUrl { get; set; }
            public string KeyTrans  { get; set; }
        }
    }
}
