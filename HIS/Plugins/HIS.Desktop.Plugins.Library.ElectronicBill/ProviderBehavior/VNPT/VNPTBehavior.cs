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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.Adapter;
using Inventec.Common.ElectronicBill.Base;
using Inventec.Common.ElectronicBill.MD;
using Inventec.Common.ElectronicBill.ModelTT78;
using Inventec.Common.Logging;
using Inventec.Common.TypeConvert;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VNPT
{
    public class VNPTBehavior : IRun
    {
        string serviceConfig { get; set; }
        string accountConfig { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        Dictionary<SERE_SERV_TYPE, List<V_HIS_SERE_SERV_5>> dicSereServType { get; set; }
        TemplateEnum.TYPE TempType { get; set; }
        ElectronicBillType.ENUM ElectronicBillTypeEnum { get; set; }

        public enum SERE_SERV_TYPE
        {
            BHYT_NOT_SERVICE_CONFIG,
            NOT_BHYT_NOT_SERVICE_CONFIG,
            SERVICE_CONFIG,
            NOT_SERVICE_CONFIG
        }

        public VNPTBehavior(ElectronicBillDataInput _electronicBillDataInput, string _serviceConfig, string _accountConfig)
            : base()
        {
            this.serviceConfig = _serviceConfig;
            this.ElectronicBillDataInput = _electronicBillDataInput;
            this.accountConfig = _accountConfig;
        }

        HIS.Desktop.Plugins.Library.ElectronicBill.Base.ElectronicBillResult IRun.Run(ElectronicBillType.ENUM _electronicBillTypeEnum, TemplateEnum.TYPE templateType)
        {
            this.ElectronicBillTypeEnum = _electronicBillTypeEnum;
            HIS.Desktop.Plugins.Library.ElectronicBill.Base.ElectronicBillResult result = new HIS.Desktop.Plugins.Library.ElectronicBill.Base.ElectronicBillResult();
            try
            {
                this.TempType = templateType;

                if (this.Check(ref result))
                {
                    string[] configArr = serviceConfig.Split('|');
                    string serviceUrl = configArr[1].Trim();
                    string username = configArr[2].Trim();
                    string pass = configArr[3].Trim();
                    string pattern = configArr[4].Trim();
                    string serial = configArr[5].Trim();
                    int convert = Inventec.Common.TypeConvert.Parse.ToInt32(configArr[6].Trim());
                    string TypeStr = "";
                    string CancelFunc = "";
                    string version = "";
                    if (configArr.Length > 7)
                    {
                        TypeStr = configArr[7].Trim();
                    }

                    if (configArr.Length > 8)
                    {
                        CancelFunc = configArr[8].Trim();
                    }

                    if (configArr.Length > 9)
                    {
                        version = configArr[9].Trim();
                    }

                    //Lấy thông tin tài khoản người dùng
                    string[] accountConfigArr = accountConfig.Split('|');
                    string account = accountConfigArr[0].Trim();
                    string acPass = accountConfigArr[1].Trim();

                    int cmdType = GetCmdType(TypeStr, CancelFunc);

                    ElectronicBillInput electronicBillInput = new Inventec.Common.ElectronicBill.Base.ElectronicBillInput();
                    electronicBillInput.account = account;
                    electronicBillInput.acPass = acPass;
                    electronicBillInput.convert = convert;
                    if (cmdType != Inventec.Common.ElectronicBill.CmdType.ImportAndPublishInv && this.ElectronicBillDataInput != null && !String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.InvoiceCode))
                    {
                        electronicBillInput.fKey = this.ElectronicBillDataInput.InvoiceCode;
                    }
                    else
                    {
                        string patientCode = this.ElectronicBillDataInput.Treatment.TDL_PATIENT_CODE;
                        string treatmentCode = this.ElectronicBillDataInput.Treatment.TREATMENT_CODE;
                        string transactionCode = "";
                        if (this.ElectronicBillDataInput.Transaction != null)
                        {
                            transactionCode = this.ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                        }
                        else if (this.ElectronicBillDataInput.ListTransaction != null && this.ElectronicBillDataInput.ListTransaction.Count > 0)
                        {
                            transactionCode = this.ElectronicBillDataInput.ListTransaction.OrderBy(o => o.TRANSACTION_CODE).First().TRANSACTION_CODE;
                        }

                        electronicBillInput.fKey = string.Format("{0}_{1}_{2}", patientCode, treatmentCode, transactionCode);
                    }
                    electronicBillInput.passWord = pass;
                    electronicBillInput.pattern = pattern;
                    electronicBillInput.serial = serial;
                    electronicBillInput.serviceUrl = serviceUrl;
                    electronicBillInput.userName = username;

                    if (ElectronicBillTypeEnum == ElectronicBillType.ENUM.CREATE_INVOICE)
                    {
                        bool notShowTaxBreakdown = CheckHoaDonBanHang(pattern) ? true : false;
                        if (version == "2")
                        {
                            electronicBillInput.invoiceTT78s = this.GetInvoiceTT78(this.ElectronicBillDataInput, notShowTaxBreakdown); ;
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("_______ invoiceTT78s: ", electronicBillInput.invoiceTT78s));
                        }
                        else
                        {
                            List<Invoice> invoices = this.GetInvoiceContrVietSens(this.ElectronicBillDataInput, notShowTaxBreakdown);
                            if (TypeStr == "1")
                            {
                                Invoice_BM invoice = new Invoice_BM();
                                invoice.Key = invoices.First().Key;
                                InvoiceDetail_BM nvoiceDetail = new InvoiceDetail_BM();
                                Inventec.Common.Mapper.DataObjectMapper.Map<InvoiceDetail_BM>(nvoiceDetail, invoices.First().InvoiceDetail);
                                nvoiceDetail.Products = new List<ProductBm>();
                                int count = 1;
                                foreach (var item in invoices.First().InvoiceDetail.Products)
                                {
                                    ProductBm product = new ProductBm();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ProductBm>(product, item);
                                    product.Extra1 = count + "";
                                    product.Extra2 = "0";
                                    nvoiceDetail.Products.Add(product);
                                    count++;
                                }

                                if (notShowTaxBreakdown)
                                {
                                    nvoiceDetail.VATAmount = "";
                                    nvoiceDetail.VATRate = "-4";
                                }
                                else
                                {
                                    nvoiceDetail.VATAmount = "";
                                    nvoiceDetail.VATRate = "-1";
                                }
                                nvoiceDetail.KindOfService = "";
                                nvoiceDetail.PaymentStatus = "1";
                                nvoiceDetail.AmountValue = String.Format("{0:0.####}", ElectronicBillDataInput.Treatment.TOTAL_PATIENT_PRICE ?? 0);
                                nvoiceDetail.TamUng = String.Format("{0:0.####}", ElectronicBillDataInput.Treatment.TOTAL_DEPOSIT_AMOUNT ?? 0);

                                decimal chenhlech = (ElectronicBillDataInput.Treatment.TOTAL_PATIENT_PRICE ?? 0) - (ElectronicBillDataInput.Treatment.TOTAL_DEPOSIT_AMOUNT ?? 0);
                                string chenhlechStr = String.Format("{0:0.####}", chenhlech >= 0 ? chenhlech : -chenhlech);
                                nvoiceDetail.PayKH = chenhlech > 0 ? chenhlechStr : "0";
                                nvoiceDetail.RePayKH = chenhlech < 0 ? chenhlechStr : "0";
                                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == ElectronicBillDataInput.Treatment.TDL_PATIENT_TYPE_ID);
                                nvoiceDetail.NoteINV = data != null ? data.PATIENT_TYPE_NAME : "";
                                nvoiceDetail.CodeTNBA = ElectronicBillDataInput.Treatment.TDL_PATIENT_CODE;
                                nvoiceDetail.PaymentMethod = GetFirstCharUpper(this.ElectronicBillDataInput.PaymentMethod) ?? "TM";
                                nvoiceDetail.ArisingDate = DateTime.Now.ToString("dd/MM/yyyy");

                                //nvoiceDetail.CurrencyUnit = "VND";

                                invoice.InvoiceDetailBm = nvoiceDetail;

                                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("_______ nvoiceDetail: ", nvoiceDetail));
                                electronicBillInput.invoicesBm = new List<Invoice_BM> { invoice };
                            }
                            else
                            {
                                electronicBillInput.invoices = invoices;
                                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("_______ invoices: ", invoices));
                            }
                        }
                    }

                    Dictionary<string, string> dataReplate = new Dictionary<string, string>();

                    electronicBillInput.DataXmlStringPlus = General.GenarateXmlStringFromConfig(this.ElectronicBillDataInput, typeof(InvoiceDetail), dataReplate);

                    //Nếu có thay thế dữ liệu thì gán giá trị
                    if (dataReplate.Count > 0 && electronicBillInput.invoices != null && electronicBillInput.invoices.Count > 0)
                    {
                        System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<InvoiceDetail>();
                        foreach (var inv in electronicBillInput.invoices)
                        {
                            foreach (var item in pi)
                            {
                                if (dataReplate.ContainsKey(item.Name))
                                {
                                    item.SetValue(inv.InvoiceDetail, dataReplate[item.Name]);
                                }
                            }
                        }
                    }

                    Inventec.Common.ElectronicBill.ElectronicBillManager eHoaDon = new Inventec.Common.ElectronicBill.ElectronicBillManager(electronicBillInput);

                    Inventec.Common.Logging.LogSystem.Debug(String.Format("{0} ,{1}, {2}", serviceUrl, account, username));

                    Inventec.Common.ElectronicBill.Base.ElectronicBillResult billResult = eHoaDon.Run(cmdType);
                    if (billResult != null && billResult.Success)
                    {
                        result.Success = true;
                        if (electronicBillInput.invoices != null && electronicBillInput.invoices.Count > 0)
                        {
                            result.InvoiceCode = electronicBillInput.invoices.First().Key;
                        }
                        else if (electronicBillInput.invoicesBm != null && electronicBillInput.invoicesBm.Count > 0)
                        {
                            result.InvoiceCode = electronicBillInput.invoicesBm.First().Key;
                        }
                        else if (electronicBillInput.invoiceTT78s != null && electronicBillInput.invoiceTT78s.Count > 0)
                        {
                            result.InvoiceCode = electronicBillInput.invoiceTT78s.First().Key;
                        }
                        else if (ElectronicBillTypeEnum == ElectronicBillType.ENUM.GET_INVOICE_INFO)
                        {
                            result.InvoiceCode = this.ElectronicBillDataInput.InvoiceCode;
                        }

                        result.InvoiceSys = ProviderType.VNPT;
                        result.InvoiceLink = billResult.InvoiceLink;
                        result.InvoiceNumOrder = GetNumOrder(billResult.Data);
                        result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                        result.InvoiceLoginname = username;
                    }
                    else
                    {
                        if (cmdType == Inventec.Common.ElectronicBill.CmdType.ConvertForStoreFkey || cmdType == Inventec.Common.ElectronicBill.CmdType.downloadInvPDFFkeyNoPay)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(String.Format("{0} ,{1}, {2}", serviceUrl, account, username));

                            Inventec.Common.ElectronicBill.Base.ElectronicBillResult billResult1 = eHoaDon.Run(Inventec.Common.ElectronicBill.CmdType.GetInvErrorViewFkey);
                            if (billResult1 != null && billResult1.Success)
                            {
                                result.Success = true;
                                if (electronicBillInput.invoices != null && electronicBillInput.invoices.Count > 0)
                                {
                                    result.InvoiceCode = electronicBillInput.invoices.First().Key;
                                }
                                else if (electronicBillInput.invoicesBm != null && electronicBillInput.invoicesBm.Count > 0)
                                {
                                    result.InvoiceCode = electronicBillInput.invoicesBm.First().Key;
                                }
                                else if (electronicBillInput.invoiceTT78s != null && electronicBillInput.invoiceTT78s.Count > 0)
                                {
                                    result.InvoiceCode = electronicBillInput.invoiceTT78s.First().Key;
                                }
                                else if (ElectronicBillTypeEnum == ElectronicBillType.ENUM.GET_INVOICE_INFO)
                                {
                                    result.InvoiceCode = this.ElectronicBillDataInput.InvoiceCode;
                                }

                                result.InvoiceSys = ProviderType.VNPT;
                                result.InvoiceLink = billResult1.InvoiceLink;
                                result.InvoiceNumOrder = GetNumOrder(billResult1.Data);
                                result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                                result.InvoiceLoginname = username;
                            }
                            else
                            {
                                ElectronicBillResultUtil.Set(ref result, false, billResult1 != null ? billResult1.Messages : null);
                                LogSystem.Error("Gui thong tin hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => billResult1), billResult1) + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => electronicBillInput.invoices), electronicBillInput.invoices));
                            }
                        }
                        else
                        {
                            ElectronicBillResultUtil.Set(ref result, false, billResult != null ? billResult.Messages : null);
                            LogSystem.Error("Gui thong tin hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => billResult), billResult) + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => electronicBillInput.invoices), electronicBillInput.invoices));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Nếu trước ký tự “/” trong mẫu số có giá trị là 2 thì xử lý tạo thông tin hóa đơn là hóa đơn bán hàng
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        private bool CheckHoaDonBanHang(string serial)
        {
            bool result = false;
            try
            {
                if (String.IsNullOrWhiteSpace(serial)) return result;
                if (serial.Contains("2/")) result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string GetFirstCharUpper(string data)
        {
            string result = "";
            if (!String.IsNullOrWhiteSpace(data))
            {
                string[] sp = data.Split(' ');
                foreach (var item in sp)
                {
                    if (!String.IsNullOrWhiteSpace(item))
                    {
                        result += item.First().ToString().ToUpper();
                    }
                }
            }
            return result;
        }

        private string GetNumOrder(string p)
        {
            string result = "";
            try
            {
                //Ví dụ:OK:01GTKT3/001;AA/12E-key1_1,key2_2,key3_3,key4_4,key5_5
                //OK: pattern;serial1-key1_num1,key2_num12,key3_num3…
                if (!String.IsNullOrWhiteSpace(p))
                {
                    if (p.Substring(0, 2).ToLower() == "ok")
                    {
                        string pskn = p.Split(':').Last();
                        if (!String.IsNullOrWhiteSpace(pskn))
                        {
                            string skn = pskn.Split(';').Last();
                            if (!String.IsNullOrWhiteSpace(skn))
                            {
                                string kn = skn.Split('-').Last();
                                if (!String.IsNullOrWhiteSpace(kn))
                                {
                                    string n = kn.Split(',').Last();
                                    if (!String.IsNullOrWhiteSpace(n))
                                    {
                                        result = n.Split('_').Last();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private int GetCmdType(string TypeStr, string cancelFunc)
        {
            int result = -1;
            try
            {
                switch (this.ElectronicBillTypeEnum)
                {
                    case ElectronicBillType.ENUM.CREATE_INVOICE:
                        result = Inventec.Common.ElectronicBill.CmdType.ImportAndPublishInv;
                        break;
                    case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                        result = Inventec.Common.ElectronicBill.CmdType.downloadInvPDFFkeyNoPay;
                        if (TypeStr == "1" || TypeStr == "2")
                        {
                            result = Inventec.Common.ElectronicBill.CmdType.ConvertForStoreFkey;
                        }
                        break;
                    case ElectronicBillType.ENUM.CANCEL_INVOICE:
                    case ElectronicBillType.ENUM.DELETE_INVOICE:
                        result = Inventec.Common.ElectronicBill.CmdType.DeleteInvFkey;
                        if (TypeStr == "1" || cancelFunc == "1")
                            result = Inventec.Common.ElectronicBill.CmdType.CancelInvNoPay;
                        //else if (TypeStr == "2")
                        //    result = Inventec.Common.ElectronicBill.CmdType.CancelInv;
                        break;
                    case ElectronicBillType.ENUM.CONVERT_INVOICE:
                        result = Inventec.Common.ElectronicBill.CmdType.ConvertForVerifyFkey;
                        break;
                    case ElectronicBillType.ENUM.GET_INVOICE_INFO:
                        result = Inventec.Common.ElectronicBill.CmdType.listInvByCusFkey;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool Check(ref HIS.Desktop.Plugins.Library.ElectronicBill.Base.ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = serviceConfig.Split('|');
                if (configArr.Length < 7)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");
                if (String.IsNullOrEmpty(accountConfig))
                    throw new Exception("Không có cấu hình tài khoản");

                string[] accountConfigArr = accountConfig.Split('|');
                if (accountConfigArr.Length != 2)
                    throw new Exception("Sai định dạng cấu hình tài khoản.");

                if (this.ElectronicBillTypeEnum == ElectronicBillType.ENUM.CREATE_INVOICE)
                {
                    if (this.ElectronicBillDataInput == null)
                        throw new Exception("Không có dữ liệu phát hành hóa đơn.");
                    if (this.ElectronicBillDataInput.Treatment == null)
                        throw new Exception("Không có thông tin hồ sơ điều trị.");
                    if (this.ElectronicBillDataInput.Branch == null)
                        throw new Exception("Không có thông tin chi nhánh.");

                    //cấu hình theo thông tu 78 cần thêm mã số thuế và địa chỉ
                    if (configArr.Length > 9 && configArr[9] == "2")
                    {
                        if (String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.Branch.TAX_CODE))
                        {
                            throw new Exception("Không có thông tin mã số thuế của chi nhánh.");
                        }
                        if (String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.Branch.ADDRESS))
                        {
                            throw new Exception("Không có thông tin địa chỉ của chi nhánh.");
                        }
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

        private string BillDataToXmlData(List<Invoice> _invoices)
        {
            string result = "";
            try
            {
                if (_invoices != null && _invoices.Count > 0)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(_invoices.GetType(),
                        new XmlRootAttribute("Invoices"));

                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xmlSerializer.Serialize(writer, _invoices);
                            result = sww.ToString(); // Your XML
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<Invoice> GetInvoiceContrVietSens(ElectronicBillDataInput electronicBillDataInput, bool notShowTaxBreakdown)
        {
            List<Invoice> invoices = new List<Invoice>();

            if (electronicBillDataInput != null)
            {
                string[] configArr = serviceConfig.Split('|');
                if (configArr.Length < 7)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");
                if (String.IsNullOrEmpty(accountConfig))
                    throw new Exception("Không có cấu hình tài khoản");

                Invoice invoice = new Invoice();
                string key = "";
                if (electronicBillDataInput.Transaction != null && !String.IsNullOrWhiteSpace(electronicBillDataInput.Transaction.TRANSACTION_CODE))
                {
                    key = electronicBillDataInput.Transaction.TRANSACTION_CODE;
                }
                else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                {
                    key = this.ElectronicBillDataInput.ListTransaction.OrderBy(o => o.TRANSACTION_CODE).First().TRANSACTION_CODE;
                }
                else
                {
                    key = String.Format("{0}-{1}", Inventec.Common.DateTime.Get.Now(), Guid.NewGuid().ToString());
                    if (key.Length > 20) key = key.Substring(0, 20);
                }

                InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(this.ElectronicBillDataInput);

                invoice.Key = key;
                //invoice.Key = String.Format("{0}-{1}", Guid.NewGuid().ToString(), Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now));
                invoice.InvoiceDetail = new Inventec.Common.ElectronicBill.MD.InvoiceDetail();
                invoice.InvoiceDetail.Extra = invoice.Key;
                //invoice.InvoiceDetail.PaymentMethod = electronicBillDataInput.PaymentMethod ?? "";
                invoice.InvoiceDetail.CusCode = adoInfo.BuyerCode ?? (Inventec.Common.DateTime.Get.Now() ?? 0).ToString();
                invoice.InvoiceDetail.CusAddress = adoInfo.BuyerAddress ?? " ";
                invoice.InvoiceDetail.CusPhone = adoInfo.BuyerPhone ?? "";
                invoice.InvoiceDetail.CusTaxCode = adoInfo.BuyerTaxCode ?? "";

                string paymentName = electronicBillDataInput.PaymentMethod;

                if (electronicBillDataInput.Transaction != null)
                {
                    HIS_PAY_FORM payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == electronicBillDataInput.Transaction.PAY_FORM_ID);
                    if (payForm != null)
                    {
                        paymentName = payForm.ELECTRONIC_PAY_FORM_NAME ?? payForm.PAY_FORM_NAME;
                    }

                    if (configArr.Length > 9 && configArr[9] == "1")
                    {
                        invoice.InvoiceDetail.CusBankNo = electronicBillDataInput.Transaction.BUYER_ACCOUNT_NUMBER;
                    }
                }
                invoice.InvoiceDetail.PaymentMethod = paymentName ?? "";

                if (HisConfigCFG.IsSwapNameOption)
                {
                    invoice.InvoiceDetail.Buyer = adoInfo.BuyerName ?? "";
                    invoice.InvoiceDetail.CusName = adoInfo.BuyerOrganization ?? "";
                }
                else
                {
                    invoice.InvoiceDetail.Buyer = adoInfo.BuyerOrganization ?? "";
                    invoice.InvoiceDetail.CusName = adoInfo.BuyerName ?? "";
                }

                invoice.InvoiceDetail.CurrencyUnit = "VND";

                decimal amount = 0, VATRateMax = -1, SumVATAmount = -1;
                invoice.InvoiceDetail.Products = this.GetProductElectronicBill(electronicBillDataInput, notShowTaxBreakdown, ref amount, ref SumVATAmount, ref VATRateMax);

                amount = Math.Round(amount - (electronicBillDataInput.Discount ?? 0), 0, MidpointRounding.AwayFromZero);
                invoice.InvoiceDetail.Total = String.Format("{0:0.####}", amount);

                if (notShowTaxBreakdown)
                {
                    invoice.InvoiceDetail.VATAmount = "";
                    invoice.InvoiceDetail.VATRate = "-4";
                }
                else
                {
                    invoice.InvoiceDetail.VATAmount = SumVATAmount > 0 ? SumVATAmount.ToString() : "";
                    invoice.InvoiceDetail.VATRate = VATRateMax.ToString();
                }

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
                    if (notShowTaxBreakdown)
                    {
                        pd.VATAmount = "";
                        pd.VATRate = "-4";
                    }
                    else
                    {
                        pd.VATAmount = "";
                        pd.VATRate = "-1";
                    }
                    invoice.InvoiceDetail.Products.Add(pd);
                }

                //Hien thi thong tin dich vu
                invoices.Add(invoice);
            }

            return invoices;
        }

        private List<Inventec.Common.ElectronicBill.MD.Product> GetProductElectronicBill(ElectronicBillDataInput dataInput, bool notShowTaxBreakdown, ref decimal totalAmount, ref decimal SumVATAmount, ref decimal VATRateMax)
        {
            List<Inventec.Common.ElectronicBill.MD.Product> products = new List<Inventec.Common.ElectronicBill.MD.Product>();

            Base.ElectronicBillDataInput dataInputWithVat = null;

            if (HisConfigCFG.IsSplitServicesWithVat
                && this.ElectronicBillDataInput.SereServs.Exists(o => o.VAT_RATIO > 0))
            {
                dataInputWithVat = new ElectronicBillDataInput(ElectronicBillDataInput);
                dataInputWithVat.SereServs = TemplateFactory.GetSereServWithVAT(this.ElectronicBillDataInput);
            }
            else
            {
                dataInput.SereServs = this.ElectronicBillDataInput.SereServs;
                dataInput.SereServBill = this.ElectronicBillDataInput.SereServBill;
            }

            IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, dataInput);
            var listProduct = iRunTemplate.Run();

            var listProductVAT = dataInputWithVat != null ? TemplateFactory.MakeIRun(TemplateEnum.TYPE.TemplateNhaThuoc, dataInputWithVat).Run() : null;

            if (listProduct == null && listProductVAT == null)
            {
                throw new Exception("Không có thông tin chi tiết dịch vụ.");
            }

            if (listProduct != null)
            {
                if (this.TempType != TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    List<ProductBase> listProductBase = (List<ProductBase>)listProduct;
                    if (listProductBase == null || listProductBase.Count == 0)
                    {
                        throw new Exception("Không có thông tin chi tiết dịch vụ.");
                    }

                    foreach (var item in listProductBase)
                    {
                        Inventec.Common.ElectronicBill.MD.Product product = new Inventec.Common.ElectronicBill.MD.Product();
                        product.ProdName = item.ProdName;
                        product.ProdUnit = !String.IsNullOrWhiteSpace(item.ProdUnit) ? item.ProdUnit : "";
                        product.ProdQuantity = (item.ProdQuantity ?? 0).ToString();
                        product.Amount = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();// String.Format("{0:0.####}", item.Amount);
                        if (notShowTaxBreakdown)
                        {
                            product.VATAmount = "";
                            product.VATRate = "-4";
                        }
                        else
                        {
                            product.VATAmount = "";
                            product.VATRate = "-1";
                        }
                        product.Total = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                        totalAmount += item.Amount;
                        product.ProdPrice = String.Format("{0:0.####}", item.ProdPrice ?? 0);

                        products.Add(product);
                    }
                }
                else
                {
                    List<ProductBasePlus> listProductBasePlus = (List<ProductBasePlus>)listProduct;
                    if (listProductBasePlus == null || listProductBasePlus.Count == 0)
                    {
                        throw new Exception("Không có thông tin chi tiết dịch vụ.");
                    }

                    foreach (var item in listProductBasePlus)
                    {
                        Inventec.Common.ElectronicBill.MD.Product product = new Inventec.Common.ElectronicBill.MD.Product();
                        product.ProdName = item.ProdName;
                        product.ProdUnit = !String.IsNullOrWhiteSpace(item.ProdUnit) ? item.ProdUnit : "";
                        product.ProdQuantity = (item.ProdQuantity ?? 0).ToString();
                        product.Amount = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();

                        if (notShowTaxBreakdown)
                        {
                            product.ProdPrice = String.Format("{0:0.####}", Math.Round((item.ProdPrice ?? 0) + (item.ProdQuantity.HasValue && item.ProdQuantity.Value > 0 ? (item.TaxAmount ?? 0) / (item.ProdQuantity ?? 0) : 0), 4) + "");
                            product.VATAmount = "";
                            product.VATRate = "-4";
                        }
                        else
                        {
                            product.VATAmount = Math.Round(item.TaxAmount ?? 0, 0, MidpointRounding.AwayFromZero).ToString();

                            if (!item.TaxPercentage.HasValue)
                            {
                                product.VATRate = "-1";
                            }

                            else if (item.TaxPercentage == 0)
                            {
                                product.VATRate = "0";
                            }
                            else if (item.TaxPercentage == 1)
                            {
                                product.VATRate = "5";
                            }
                            else if (item.TaxPercentage == 2)
                            {
                                product.VATRate = "10";
                            }
                            else if (item.TaxPercentage == 3)
                            {
                                product.VATRate = "8";
                            }

                            if (String.IsNullOrWhiteSpace(product.VATRate))
                            {
                                product.VATRate = ((long)item.TaxConvert).ToString();
                            }

                            product.ProdPrice = String.Format("{0:0.####}", item.ProdPrice ?? 0);
                        }

                        product.Total = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                        totalAmount += item.Amount;
                        SumVATAmount += item.TaxAmount ?? 0;

                        products.Add(product);
                    }

                }
            }
            if (listProductVAT != null)
            {
                List<ProductBasePlus> listProductBasePlus = (List<ProductBasePlus>)listProductVAT;

                if (listProductBasePlus == null || listProductBasePlus.Count == 0)
                {
                    throw new Exception("Không có thông tin chi tiết dịch vụ.");
                }
                foreach (var item in listProductBasePlus)
                {
                    Inventec.Common.ElectronicBill.MD.Product product = new Inventec.Common.ElectronicBill.MD.Product();
                    product.ProdName = item.ProdName;
                    product.ProdUnit = !String.IsNullOrWhiteSpace(item.ProdUnit) ? item.ProdUnit : "";
                    product.ProdQuantity = (item.ProdQuantity ?? 0).ToString();
                    product.Amount = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();

                    if (notShowTaxBreakdown)
                    {
                        product.ProdPrice = String.Format("{0:0.####}", Math.Round((item.ProdPrice ?? 0) + (item.ProdQuantity.HasValue && item.ProdQuantity.Value > 0 ? (item.TaxAmount ?? 0) / (item.ProdQuantity ?? 0) : 0), 4) + "");
                        product.VATAmount = "";
                        product.VATRate = "-4";
                    }
                    else
                    {
                        product.VATAmount = Math.Round(item.TaxAmount ?? 0, 0, MidpointRounding.AwayFromZero).ToString();

                        if (!item.TaxPercentage.HasValue)
                        {
                            product.VATRate = "-1";
                        }

                        else if (item.TaxPercentage == 0)
                        {
                            product.VATRate = "0";
                        }
                        else if (item.TaxPercentage == 1)
                        {
                            product.VATRate = "5";
                        }
                        else if (item.TaxPercentage == 2)
                        {
                            product.VATRate = "10";
                        }
                        else if (item.TaxPercentage == 3)
                        {
                            product.VATRate = "8";
                        }

                        if (String.IsNullOrWhiteSpace(product.VATRate))
                        {
                            product.VATRate = ((long)item.TaxConvert).ToString();
                        }

                        product.ProdPrice = String.Format("{0:0.####}", item.ProdPrice ?? 0);
                    }

                    product.Total = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                    totalAmount += item.Amount;
                    SumVATAmount += item.TaxAmount ?? 0;

                    products.Add(product);
                }

            }

            totalAmount = Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero);
            SumVATAmount = Math.Round(SumVATAmount, 0, MidpointRounding.AwayFromZero);
            VATRateMax = (products != null && products.Count > 0) ? products.Max(o => decimal.Parse(o.VATRate)) : -1;

            return products;
        }

        private List<HDon> GetInvoiceTT78(Base.ElectronicBillDataInput electronicBillDataInput, bool notShowTaxBreakdown)
        {
            List<HDon> result = new List<HDon>();
            if (electronicBillDataInput != null)
            {
                HDon invoice = new HDon();
                string key = "";
                if (electronicBillDataInput.Transaction != null && !String.IsNullOrWhiteSpace(electronicBillDataInput.Transaction.TRANSACTION_CODE))
                {
                    key = electronicBillDataInput.Transaction.TRANSACTION_CODE;
                }
                else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                {
                    key = this.ElectronicBillDataInput.ListTransaction.OrderBy(o => o.TRANSACTION_CODE).First().TRANSACTION_CODE;
                }
                else
                {
                    key = String.Format("{0}-{1}", Inventec.Common.DateTime.Get.Now(), Guid.NewGuid().ToString());
                    if (key.Length > 20) key = key.Substring(0, 20);
                }

                invoice.Key = key;
                invoice.dLHDon = new DLHDon();
                invoice.dLHDon.tTChung = new TTChung();
                invoice.dLHDon.tTChung.DVTTe = "VND";
                invoice.dLHDon.tTChung.TGia = "1";
                //invoice.dLHDon.tTChung.HTTToan = electronicBillDataInput.PaymentMethod;

                string paymentName = electronicBillDataInput.PaymentMethod;

                if (electronicBillDataInput.Transaction != null)
                {
                    HIS_PAY_FORM payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == electronicBillDataInput.Transaction.PAY_FORM_ID);
                    if (payForm != null)
                    {
                        paymentName = payForm.ELECTRONIC_PAY_FORM_NAME ?? payForm.PAY_FORM_NAME;
                    }
                }
                invoice.dLHDon.tTChung.HTTToan = paymentName;

                invoice.dLHDon.nDHDon = new NDHDon();

                //người bán
                invoice.dLHDon.nDHDon.nBan = new NBan();
                invoice.dLHDon.nDHDon.nBan.Ten = electronicBillDataInput.Branch.BRANCH_NAME;
                invoice.dLHDon.nDHDon.nBan.DChi = electronicBillDataInput.Branch.ADDRESS ?? ".";
                invoice.dLHDon.nDHDon.nBan.MST = electronicBillDataInput.Branch.TAX_CODE;
                invoice.dLHDon.nDHDon.nBan.SDThoai = electronicBillDataInput.Branch.PHONE ?? "";
                invoice.dLHDon.nDHDon.nBan.STKNHang = electronicBillDataInput.Branch.ACCOUNT_NUMBER ?? "";
                invoice.dLHDon.nDHDon.nBan.TNHang = electronicBillDataInput.Branch.BANK_INFO ?? "";
                //invoice.dLHDon.nDHDon.nBan.DCTDTu = "";
                //invoice.dLHDon.nDHDon.nBan.Fax = "";

                //người mua
                InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(electronicBillDataInput);
                invoice.dLHDon.nDHDon.nMua = new NMua();

                if (HisConfigCFG.IsSwapNameOption)
                {
                    invoice.dLHDon.nDHDon.nMua.Ten = adoInfo.BuyerName;
                    invoice.dLHDon.nDHDon.nMua.HVTNMHang = adoInfo.BuyerOrganization;
                }
                else
                {
                    invoice.dLHDon.nDHDon.nMua.Ten = adoInfo.BuyerOrganization;
                    invoice.dLHDon.nDHDon.nMua.HVTNMHang = adoInfo.BuyerName;
                }

                //invoice.dLHDon.nDHDon.nMua.Ten = adoInfo.BuyerOrganization;
                //invoice.dLHDon.nDHDon.nMua.HVTNMHang = adoInfo.BuyerName;
                invoice.dLHDon.nDHDon.nMua.DChi = adoInfo.BuyerAddress;
                invoice.dLHDon.nDHDon.nMua.DCTDTu = adoInfo.BuyerEmail;
                invoice.dLHDon.nDHDon.nMua.MKHang = adoInfo.BuyerCode;
                invoice.dLHDon.nDHDon.nMua.MST = adoInfo.BuyerTaxCode;
                invoice.dLHDon.nDHDon.nMua.SDThoai = adoInfo.BuyerPhone;
                invoice.dLHDon.nDHDon.nMua.STKNHang = adoInfo.BuyerAccountNumber;
                //invoice.dLHDon.nDHDon.nMua.TNHang = "";

                //chi tiết hóa đơn
                invoice.dLHDon.nDHDon.hHDVu = new List<HHDVu>();

                decimal totalAmount = 0;
                Base.ElectronicBillDataInput dataInputWithVat = null;
                if (HisConfigCFG.IsSplitServicesWithVat
                 && this.ElectronicBillDataInput.SereServs.Exists(o => o.VAT_RATIO > 0))
                {
                    dataInputWithVat = new ElectronicBillDataInput(ElectronicBillDataInput);
                    dataInputWithVat.SereServs = TemplateFactory.GetSereServWithVAT(this.ElectronicBillDataInput);
                }
                else
                {
                    electronicBillDataInput.SereServs = this.ElectronicBillDataInput.SereServs;
                    electronicBillDataInput.SereServBill = this.ElectronicBillDataInput.SereServBill;
                }

                IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, electronicBillDataInput);
                var listProduct = iRunTemplate.Run();

                var listProductVAT = dataInputWithVat != null ? TemplateFactory.MakeIRun(TemplateEnum.TYPE.TemplateNhaThuoc, dataInputWithVat).Run() : null;

                if (listProduct == null && listProductVAT == null)
                {
                    throw new Exception("Không có thông tin chi tiết dịch vụ.");
                }


                int count = 1;
                if (listProductVAT != null)
                {
                    List<ProductBasePlus> listProductBasePlus = (List<ProductBasePlus>)listProductVAT;

                    if (listProductBasePlus == null || listProductBasePlus.Count == 0)
                    {
                        throw new Exception("Không có thông tin chi tiết dịch vụ.");
                    }

                    foreach (var item in listProductBasePlus)
                    {
                        HHDVu product = new HHDVu();
                        product.STT = count + "";
                        product.TChat = "1";
                        product.THHDVu = item.ProdName;
                        product.MHHDVu = item.ProdCode;
                        product.DVTinh = !String.IsNullOrWhiteSpace(item.ProdUnit) ? item.ProdUnit : "";
                        product.SLuong = (item.ProdQuantity ?? 0).ToString();
                        product.DGia = String.Format("{0:0.####}", item.ProdPrice ?? 0);

                        if (notShowTaxBreakdown)
                        {
                            product.ThTien = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                            product.TSThue = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                            product.TSuat = "";
                            product.TThue = "-4";
                        }
                        else
                        {
                            product.ThTien = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                            product.TSThue = Math.Round(item.AmountWithoutTax ?? 0, 0, MidpointRounding.AwayFromZero).ToString();
                            product.TSuat = Math.Round(item.TaxAmount ?? 0, 0, MidpointRounding.AwayFromZero).ToString();

                            if (!item.TaxPercentage.HasValue)
                            {
                                product.TThue = "-1";
                            }

                            else if (item.TaxPercentage == 0)
                            {
                                product.TThue = "0";
                            }
                            else if (item.TaxPercentage == 1)
                            {
                                product.TThue = "5";
                            }
                            else if (item.TaxPercentage == 2)
                            {
                                product.TThue = "10";
                            }
                            else if (item.TaxPercentage == 3)
                            {
                                product.TThue = "8";
                            }

                            if (String.IsNullOrWhiteSpace(product.TThue))
                            {
                                product.TThue = ((long)item.TaxConvert).ToString();
                            }
                        }

                        totalAmount += item.Amount;

                        invoice.dLHDon.nDHDon.hHDVu.Add(product);
                        count++;
                    }
                }
                if (listProduct != null)
                {
                    if (this.TempType != TemplateEnum.TYPE.TemplateNhaThuoc)
                    {
                        List<ProductBase> listProductBase = (List<ProductBase>)listProduct;
                        if (listProductBase == null || listProductBase.Count == 0)
                        {
                            throw new Exception("Không có thông tin chi tiết dịch vụ.");
                        }

                        foreach (var item in listProductBase)
                        {
                            HHDVu product = new HHDVu();
                            product.STT = count + "";
                            product.TChat = "1";
                            product.THHDVu = item.ProdName;
                            product.MHHDVu = item.ProdCode;
                            product.DVTinh = !String.IsNullOrWhiteSpace(item.ProdUnit) ? item.ProdUnit : "";
                            product.SLuong = (item.ProdQuantity ?? 0).ToString();
                            product.ThTien = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                            product.TSThue = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                            product.DGia = String.Format("{0:0.####}", item.ProdPrice ?? 0);
                            if (notShowTaxBreakdown)
                            {
                                product.TSuat = "";
                                product.TThue = "-4";
                            }
                            totalAmount += item.Amount;

                            invoice.dLHDon.nDHDon.hHDVu.Add(product);
                            count++;
                        }
                    }
                    else
                    {
                        List<ProductBasePlus> listProductBasePlus = (List<ProductBasePlus>)listProduct;
                        if (listProductBasePlus == null || listProductBasePlus.Count == 0)
                        {
                            throw new Exception("Không có thông tin chi tiết dịch vụ.");
                        }

                        foreach (var item in listProductBasePlus)
                        {
                            HHDVu product = new HHDVu();
                            product.STT = count + "";
                            product.TChat = "1";
                            product.THHDVu = item.ProdName;
                            product.MHHDVu = item.ProdCode;
                            product.DVTinh = !String.IsNullOrWhiteSpace(item.ProdUnit) ? item.ProdUnit : "";
                            product.SLuong = (item.ProdQuantity ?? 0).ToString();
                            product.DGia = String.Format("{0:0.####}", item.ProdPrice ?? 0);

                            if (notShowTaxBreakdown)
                            {
                                product.ThTien = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                                product.TSThue = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                                product.TSuat = "";
                                product.TThue = "-4";
                            }
                            else
                            {
                                product.ThTien = Math.Round(item.Amount, 0, MidpointRounding.AwayFromZero).ToString();
                                product.TSThue = Math.Round(item.AmountWithoutTax ?? 0, 0, MidpointRounding.AwayFromZero).ToString();
                                product.TSuat = Math.Round(item.TaxAmount ?? 0, 0, MidpointRounding.AwayFromZero).ToString();

                                if (!item.TaxPercentage.HasValue)
                                {
                                    product.TThue = "-1";
                                }

                                else if (item.TaxPercentage == 0)
                                {
                                    product.TThue = "0";
                                }
                                else if (item.TaxPercentage == 1)
                                {
                                    product.TThue = "5";
                                }
                                else if (item.TaxPercentage == 2)
                                {
                                    product.TThue = "10";
                                }
                                else if (item.TaxPercentage == 3)
                                {
                                    product.TThue = "8";
                                }

                                if (String.IsNullOrWhiteSpace(product.TThue))
                                {
                                    product.TThue = ((long)item.TaxConvert).ToString();
                                }
                            }

                            totalAmount += item.Amount;

                            invoice.dLHDon.nDHDon.hHDVu.Add(product);
                            count++;
                        }
                    }
                }
                invoice.dLHDon.nDHDon.tToan = new TToan();

                //hóa đơn không thuế không có thông tin thuế suất. Dựa theo thông tin tạo trên cổng.
                //invoice.dLHDon.nDHDon.tToan.lTSuat = new List<LTSuat>();
                //var grThue = invoice.dLHDon.nDHDon.hHDVu.GroupBy(o => o.TSuat).ToList();
                //foreach (var item in grThue)
                //{
                //    LTSuat lTSuat = new LTSuat();
                //    lTSuat.ThTien = item.Sum(s => Parse.ToDecimal(s.ThTien)) + "";
                //    lTSuat.TSuat = item.First().TSuat;
                //    lTSuat.TThue = item.Sum(s => Parse.ToDecimal(s.TThue)) + "";

                //    invoice.dLHDon.nDHDon.tToan.lTSuat.Add(lTSuat);
                //}
                totalAmount = Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero);

                invoice.dLHDon.nDHDon.tToan.TgTCThue = invoice.dLHDon.nDHDon.hHDVu.Sum(s => Parse.ToDecimal(s.ThTien)) + "";
                if (notShowTaxBreakdown)
                {
                    invoice.dLHDon.nDHDon.tToan.TgTThue = "-4";
                }
                else
                {
                    invoice.dLHDon.nDHDon.tToan.TgTThue = invoice.dLHDon.nDHDon.hHDVu.Sum(s => Parse.ToDecimal(s.TThue)) + "";
                }
                invoice.dLHDon.nDHDon.tToan.TgTTTBSo = totalAmount + "";
                invoice.dLHDon.nDHDon.tToan.TgTTTBChu = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalAmount))) + "đồng";

                if (electronicBillDataInput.Discount.HasValue && electronicBillDataInput.Discount.Value > 0)
                    invoice.dLHDon.nDHDon.tToan.TTCKTMai = electronicBillDataInput.Discount + "";

                result.Add(invoice);
            }
            return result;
        }
    }
}
