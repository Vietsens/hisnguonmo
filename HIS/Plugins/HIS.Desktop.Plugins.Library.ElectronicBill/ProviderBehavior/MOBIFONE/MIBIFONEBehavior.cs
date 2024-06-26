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
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MOBIFONE.Model;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.EBillSoftDreams.Model;
using Inventec.Common.EBillSoftDreams.ModelXml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MOBIFONE
{
    class MIBIFONEBehavior : IRun
    {
        private string CurrencyCode = "VND";
        private DataReferences InvoiceBook { get; set; }
        private LoginResultData login { get; set; }
        private TemplateEnum.TYPE TempType { get; set; }
        private string serviceConfig { get; set; }
        private string accountConfig { get; set; }
        private string serviceUrl { get; set; }
        private string cer_serial { get; set; }
        private List<HoaDon78Result> HoaDonData { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        private LoginData adoLogin { get; set; }
        public MIBIFONEBehavior(Base.ElectronicBillDataInput electronicBillDataInput, string serviceConfig, string accountConfig)
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
                    this.TempType = _templateType;
                    string[] configArr = serviceConfig.Split('|');
                    serviceUrl = configArr[1];
                    cer_serial = configArr[2];
                    if (String.IsNullOrEmpty(serviceUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay dia chi Webservice URL");
                        ElectronicBillResultUtil.Set(ref result, false, "Không tìm thấy địa chỉ Webservice URL");
                        return result;
                    }
                    string[] accountConfigArr = accountConfig.Split('|');
                    adoLogin = new LoginData();
                    adoLogin.username = accountConfigArr[0].Trim();
                    adoLogin.password = accountConfigArr[1].Trim();
                    login = ProcessLogin(adoLogin);
                    if (login == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error(String.Format("{0}, {1}__{2}__{3}", login.error, serviceUrl, adoLogin.username, adoLogin.password));
                        ElectronicBillResultUtil.Set(ref result, false, login.error);
                        return result;
                    }
                    switch (electronicBillType)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            ProcessGetDataReferences(ref result);
                            ProcessCreateInvoice(ref result);
                            ProcessSignInvoiceCertFile(ref result);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            ProcessPrintInvoice(ref result);
                            break;
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                            break;
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            ProcessCancelInvoice(ref result);
                            break;
                        case ElectronicBillType.ENUM.CONVERT_INVOICE:
                            break;
                        case ElectronicBillType.ENUM.CREATE_INVOICE_DATA:
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_INFO:
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
        private void ProcessCancelInvoice(ref ElectronicBillResult result)
        {
            try
            {
                if (ElectronicBillDataInput == null || string.IsNullOrEmpty(ElectronicBillDataInput.InvoiceCode))
                    return;
                CancelInvResult data = HIS.Desktop.Plugins.Library.ElectronicBill.Base.ApiConsumerV2.CreateRequest<CancelInvResult>(System.Net.WebRequestMethods.Http.Get, serviceUrl, string.Format(RequestUriStore.MobifoneuploadCanceledInv, ElectronicBillDataInput.InvoiceCode), login.token, login.ma_dvcs, null);
                if (data != null && data.ok)
                {
                    result.Success = true;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                    Inventec.Common.Logging.LogSystem.Error("Tai hoa don file PDF that bai " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    ElectronicBillResultUtil.Set(ref result, false, data.error);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessPrintInvoice(ref ElectronicBillResult result, bool inchuyendoi = true)
        {
            try
            {
                if (ElectronicBillDataInput == null || string.IsNullOrEmpty(ElectronicBillDataInput.InvoiceCode))
                    return;
                byte[] data = HIS.Desktop.Plugins.Library.ElectronicBill.Base.ApiConsumerV2.CreateRequestGetByte(System.Net.WebRequestMethods.Http.Get, serviceUrl, string.Format(RequestUriStore.MobifoneInHoadon, ElectronicBillDataInput.InvoiceCode, inchuyendoi), login.token, login.ma_dvcs);
                if (data != null)
                {
                    string fullName = Application.StartupPath + @"\temp\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    System.IO.File.WriteAllBytes(fullName, data);
                    result.Success = true;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                    result.InvoiceLink = fullName;                  
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                    Inventec.Common.Logging.LogSystem.Error("Tai hoa don file PDF that bai " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    ElectronicBillResultUtil.Set(ref result, false, "Tải hóa đơn file PDF thất bại");
                    ProcessPrintInvoice(ref result, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessSignInvoiceCertFile(ref ElectronicBillResult result)
        {
            try
            {
                if (HoaDonData == null)
                    return;
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(DataSignInvoiceCertFile68Init());
                SignInvoiceCertFile68Result data = HIS.Desktop.Plugins.Library.ElectronicBill.Base.ApiConsumerV2.CreateRequest<SignInvoiceCertFile68Result>(System.Net.WebRequestMethods.Http.Post, serviceUrl, RequestUriStore.MobifoneSignInvoiceCertFile68, login.token, login.ma_dvcs, sendJsonData);
                if (data != null && data.ok && string.IsNullOrEmpty(data.error))
                {
                    result.Success = true;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                    result.InvoiceCode = HoaDonData.First().data.hdon_id;
                    result.InvoiceNumOrder = HoaDonData.First().data.khieu + HoaDonData.First().data.shdon;
                    result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(HoaDonData.First().data.nlap);
                    result.InvoiceLoginname = adoLogin.username;
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                    Inventec.Common.Logging.LogSystem.Error("Ky va gui hoa don toi CQT that bai " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    ElectronicBillResultUtil.Set(ref result, false, data.error);
                    ProcessCreateInvoice(ref result, (int)EditMode.Delete);                  
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private SignInvoiceCertFile68Init DataSignInvoiceCertFile68Init()
        {
            SignInvoiceCertFile68Init obj = null;
            try
            {
                obj = new SignInvoiceCertFile68Init();
                SignInvoiceCertFile68Data dt = new SignInvoiceCertFile68Data();
                dt.branch_code = login.ma_dvcs;
                dt.username = adoLogin.username;
                dt.lsthdon_id = HoaDonData.Select(o => o.data.hdon_id).ToList();
                dt.cer_serial = cer_serial;
                dt.type_cmd = HoaDonData.First().data.is_hdcma == 1 ? ((int)(TypeCmd.HasCode)).ToString() : ((int)(TypeCmd.NoCode)).ToString();
                dt.is_api = "1";
                obj.data = new List<SignInvoiceCertFile68Data>(){ dt };
            }
            catch (Exception ex)
            {
                obj = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return obj;
        }
        private void ProcessCreateInvoice(ref ElectronicBillResult result, int editMode = (int)EditMode.Create)
        {
            try
            {
                if (InvoiceBook == null)
                    return;
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(DataHoaDon78Init(editMode));
                HoaDonData = HIS.Desktop.Plugins.Library.ElectronicBill.Base.ApiConsumerV2.CreateRequest<List<HoaDon78Result>>(System.Net.WebRequestMethods.Http.Post, serviceUrl, RequestUriStore.MobifoneSaveListHoadon78, login.token, login.ma_dvcs, sendJsonData);
                if (HoaDonData != null && HoaDonData.Count > 0 && HoaDonData.FirstOrDefault(o=> !string.IsNullOrEmpty(o.error)) == null)
                {
                    if (editMode == (int)EditMode.Create)
                    {
                        result.Success = true;
                    }
                    result.InvoiceSys = ProviderType.MOBIFONE;
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                    Inventec.Common.Logging.LogSystem.Error("Tao hoa don that bai " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HoaDonData), HoaDonData));
                    ElectronicBillResultUtil.Set(ref result, false, HoaDonData != null && HoaDonData.Count > 0 ? string.Join(", ", HoaDonData.Select(o => o.error)) : "Tạo hóa đơn thất bại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private HoaDon78Init DataHoaDon78Init(int editMode)
        {
            HoaDon78Init obj = null;
            try
            {
                var inv = InvoiceInfo.InvoiceInfoProcessor.GetData(ElectronicBillDataInput);
                obj = new HoaDon78Init();
                obj.editmode = editMode;
                HoaDon78Data hd78data = new HoaDon78Data();
                hd78data.cctbao_id = InvoiceBook.qlkhsdung_id;
                hd78data.hdon_id = editMode == (int)EditMode.Create ? null : HoaDonData.First().data.hdon_id;
                hd78data.nlap = DateTime.Now.ToString("yyyy-MM-dd");
                hd78data.dvtte = CurrencyCode;
                hd78data.tgia = 1;
                hd78data.htttoan = "Tiền mặt/Chuyển khoản";
                hd78data.tnmua = inv.BuyerName;
                hd78data.mnmua = inv.BuyerCode;
                hd78data.mst = inv.BuyerTaxCode;
                hd78data.sdtnmua = inv.BuyerPhone;
                hd78data.email = inv.BuyerEmail;
                hd78data.ten = inv.BuyerOrganization;
                hd78data.dchi = inv.BuyerAddress;
                hd78data.details = new List<HoaDon78Details>();
                HoaDon78Details dt = new HoaDon78Details();
                dt.data = new List<HoaDon78DetailsData>();
                int count = 0;
                var lstProductBase = GetProductBaseElectronicBill();
                foreach (var item in lstProductBase)
                {
                    count++;
                    HoaDon78DetailsData ddt = new HoaDon78DetailsData();
                    ddt.stt = count;
                    ddt.ma = item.ProdCode;
                    ddt.ten = item.ProdName;
                    ddt.mdvtinh = item.ProdUnit;
                    ddt.dgia = item.ProdPrice;
                    ddt.sluong = item.ProdQuantity;
                    ddt.tlckhau = 0;
                    ddt.tthue = 0;
                    ddt.thtien = item.Amount;
                    ddt.tgtien = item.Amount;
                    ddt.kmai = 1;
                    ddt.tsuat = "-1";
                    dt.data.Add(ddt);
                }
                hd78data.details.Add(dt);

                hd78data.tgtcthue = (dt.data != null && dt.data.Count > 0) ? dt.data.Sum(o=>o.thtien ?? 0) : 0;
                hd78data.tgtthue = (dt.data != null && dt.data.Count > 0) ? dt.data.Sum(o => o.tthue ?? 0) : 0;
                hd78data.tgtttbso = (dt.data != null && dt.data.Count > 0) ? dt.data.Sum(o => o.tgtien ?? 0) : 0;
                hd78data.tgtttbso_last = (dt.data != null && dt.data.Count > 0) ? dt.data.Sum(o => o.tgtien ?? 0) : 0;
                hd78data.mdvi = login.ma_dvcs;
                hd78data.tthdon = 0;
                hd78data.is_hdcma = InvoiceBook.hthuc.Equals("K") ? 0 : 1;
                ///
                hd78data.hoadon68_khac = new List<HoaDon78Khac>();
                HoaDon78Khac kh = new HoaDon78Khac();
                kh.data = new List<HoaDon78KhacData>();
                //

                hd78data.hoadon68_phi = new List<HoaDon78Phi>();
                HoaDon78Phi ph = new HoaDon78Phi();
                ph.data = new List<HoaDon78PhiData>();
                obj.data = new List<HoaDon78Data>() { hd78data };
            }
            catch (Exception ex)
            {
                obj = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return obj;
        }
        private void ProcessGetDataReferences(ref ElectronicBillResult result)
        {
            try
            {
                List<DataReferences> data = Base.ApiConsumerV2.CreateRequest<List<DataReferences>>(System.Net.WebRequestMethods.Http.Get, serviceUrl, RequestUriStore.MobifoneGetDataReferencesByRefId, login.token, login.ma_dvcs, null);
                if (data != null)
                {
                    result.Success = true;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                    var checkSysbolCode = data.Where(o => o.khhdon.Equals(ElectronicBillDataInput.SymbolCode)).ToList();
                    if (checkSysbolCode == null || checkSysbolCode.Count == 0)
                    {
                        ElectronicBillResultUtil.Set(ref result, false, String.Format("Không tìm thấy thông tin ký hiệu hóa đơn {0} vui lòng kiểm tra lại hệ thống hóa đơn điện tử.", ElectronicBillDataInput.SymbolCode));
                        return;
                    }
                    else
                    {
                        InvoiceBook = checkSysbolCode.FirstOrDefault();
                    }
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MOBIFONE;
                    Inventec.Common.Logging.LogSystem.Error("Lay thong tin phat hanh hoa don that bai " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    ElectronicBillResultUtil.Set(ref result, false, "Lấy thông tin phát hành hóa đơn thất bại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Invoice
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
        private List<ProductBase> GetProductBaseElectronicBill()
        {
            List<ProductBase> result = new List<ProductBase>();
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
                    foreach (var item in lstProductBasePlus)
                    {
                        ProductBase pb = new ProductBase();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ProductBase>(pb, item);
                        result.Add(pb);
                    }
                }
                else
                {
                    result = (List<ProductBase>)listProduct;
                }
            }
            catch (Exception ex)
            {
                result = new List<ProductBase>();
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

        private LoginResultData ProcessLogin( LoginData obj)
        {
            LoginResultData result = null;
            try
            {
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                result = Base.ApiConsumerV2.CreateRequest<LoginResultData>(System.Net.WebRequestMethods.Http.Post, serviceUrl, Base.RequestUriStore.MobifoneLogin, null,null, sendJsonData);
            }
            catch (Exception ex)
            {
                result = null;
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
                if (configArr[0] != ProviderType.MOBIFONE)
                    throw new Exception("Không đúng cấu hình nhà cung cấp MOBIFONE");

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
