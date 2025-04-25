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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
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
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.BKAV
{
    public class BKAVBehavior : IRun
    {
        string config { get; set; }
        string accountConfig { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        TemplateEnum.TYPE TempType { get; set; }
        ElectronicBillType.ENUM ElectronicBillTypeEnum { get; set; }

        public BKAVBehavior(ElectronicBillDataInput _electronicBillDataInput, string _config, string _accountConfig)
            : base()
        {
            this.config = _config;
            this.accountConfig = _accountConfig;
            this.ElectronicBillDataInput = _electronicBillDataInput;
        }

        ElectronicBillResult IRun.Run(ElectronicBillType.ENUM _electronicBillTypeEnum, TemplateEnum.TYPE _tempType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                this.TempType = _tempType;
                this.ElectronicBillTypeEnum = _electronicBillTypeEnum;

                if (this.Check(ref result))
                {
                    string[] configArr = config.Split('|');

                    string serviceUrl = configArr[1]; //ConfigurationManager.AppSettings[AppConfigKey.WEBSERVICE_URL];
                    if (String.IsNullOrEmpty(serviceUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay dia chi Webservice URL");
                        ElectronicBillResultUtil.Set(ref result, false, "Không tìm thấy địa chỉ Webservice URL");
                        return result;
                    }

                    string[] accountConfigArr = accountConfig.Split('|');

                    Inventec.Common.EHoaDon.BkavPartner bkavPartner = new Inventec.Common.EHoaDon.BkavPartner();
                    bkavPartner.BkavPartnerGUID = accountConfigArr[0].Trim();
                    bkavPartner.BkavPartnerToken = accountConfigArr[1].Trim();

                    if (accountConfigArr[2] != null)
                    {
                        uint Mode = 0;
                        uint.TryParse(accountConfigArr[2].Trim(), out Mode);
                        bkavPartner.Mode = Mode;
                    }

                    List<Inventec.Common.EHoaDon.InvoiceDataWS> invoiceDataWSs = this.GetInvoiceContrBKAV(this.ElectronicBillDataInput);

                    string cmdTypeCFG = "";
                    if (configArr.Length > 3)
                    {
                        cmdTypeCFG = configArr[3];
                    }

                    if (this.ElectronicBillTypeEnum == ElectronicBillType.ENUM.CREATE_INVOICE)
                    {
                        if (invoiceDataWSs.SelectMany(s => s.ListInvoiceDetailsWS).Sum(s => s.Amount) <= 0)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Lỗi dữ liệu không có thành tiền dịch vụ");
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceDataWSs), invoiceDataWSs));
                            ElectronicBillResultUtil.Set(ref result, false, "Tổng tiền hóa đơn phải lớn hơn 0");
                            return result;
                        }
                    }

                    int cmdType = GetCmdType(cmdTypeCFG);

                    Inventec.Common.EHoaDon.EHoaDonManager eHoaDon = new Inventec.Common.EHoaDon.EHoaDonManager(serviceUrl, bkavPartner, invoiceDataWSs);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceDataWSs), invoiceDataWSs));
                    List<Inventec.Common.EHoaDon.InvoiceResult> invoiceResults = eHoaDon.Run(cmdType);

                    bool IsError = false;

                    if (invoiceResults != null && invoiceResults.Count > 0)
                    {
                        foreach (var item in invoiceResults)
                        {
                            if (item.Status == 0)
                            {
                                //Thanh cong
                                result.Success = true;
                                result.InvoiceSys = "BKAV";
                                result.InvoiceCode = item.PartnerInvoiceID > 0 ? item.PartnerInvoiceID.ToString() : "";
                                result.InvoiceLink = configArr[2] + "/" + item.MessLog;
                                result.InvoiceNumOrder = item.InvoiceNo.ToString();
                                result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                                result.InvoiceLoginname = bkavPartner.BkavPartnerGUID;
                                result.InvoiceLookupCode = item.MTC;
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("___invoiceResults___", item));
                            }
                            else
                            {
                                if (this.ElectronicBillTypeEnum == ElectronicBillType.ENUM.GET_INVOICE_LINK)
                                {
                                    IsError = true;
                                    break;
                                }
                                ElectronicBillResultUtil.Set(ref result, false, item.MessLog);
                                LogSystem.Error("gui thong tin hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MessLog), item.MessLog) + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceDataWSs), invoiceDataWSs));
                                return result;
                            }
                        }
                    }

                    if (this.ElectronicBillTypeEnum == ElectronicBillType.ENUM.GET_INVOICE_LINK && IsError)
                    {
                        Inventec.Common.EHoaDon.EHoaDonManager eHoaDon_Show = new Inventec.Common.EHoaDon.EHoaDonManager(serviceUrl, bkavPartner, invoiceDataWSs);
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("invoiceDataWSs_SHOW: ", invoiceDataWSs));
                        List<Inventec.Common.EHoaDon.InvoiceResult> invoiceResults_Show = eHoaDon_Show.Run(Inventec.Common.EHoaDon.CmdType.GetInvoiceShow);

                        if (invoiceResults_Show != null && invoiceResults_Show.Count > 0)
                        {
                            foreach (var item in invoiceResults_Show)
                            {
                                if (item.Status == 0)
                                {
                                    //Thanh cong
                                    result.Success = true;
                                    result.InvoiceSys = "BKAV";
                                    result.InvoiceCode = item.PartnerInvoiceID > 0 ? item.PartnerInvoiceID.ToString() : "";
                                    result.InvoiceLink = configArr[2] + "/" + item.MessLog;
                                    result.InvoiceNumOrder = item.InvoiceNo.ToString();
                                    result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                                    result.InvoiceLoginname = bkavPartner.BkavPartnerGUID;
                                    result.InvoiceLookupCode = item.MTC;
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("___invoiceResults_Show___", item));
                                }
                                else
                                {
                                    ElectronicBillResultUtil.Set(ref result, false, item.MessLog);
                                    LogSystem.Error("gui thong tin hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MessLog), item.MessLog) + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceDataWSs), invoiceDataWSs));
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private int GetCmdType(string cmdTypeCFG)
        {
            int result = -1;
            try
            {
                switch (this.ElectronicBillTypeEnum)
                {
                    case ElectronicBillType.ENUM.CREATE_INVOICE:
                        if (cmdTypeCFG == "1")
                        {
                            result = Inventec.Common.EHoaDon.CmdType.CreateInvoiceWithFormSerialReturnNo;
                        }
                        else
                        {
                            result = Inventec.Common.EHoaDon.CmdType.CreateInvoiceTR;
                        }
                        break;
                    case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                        result = Inventec.Common.EHoaDon.CmdType.GetInvoiceLink;
                        break;
                    case ElectronicBillType.ENUM.GET_INVOICE_SHOW:
                        result = Inventec.Common.EHoaDon.CmdType.GetInvoiceShow;
                        break;
                    case ElectronicBillType.ENUM.CANCEL_INVOICE:
                        result = Inventec.Common.EHoaDon.CmdType.CancelInvoiceByPartnerInvoiceID;
                        break;
                    case ElectronicBillType.ENUM.DELETE_INVOICE:
                        result = Inventec.Common.EHoaDon.CmdType.DeleteInvoiceByPartnerInvoiceID;
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

        private bool Check(ref ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = config.Split('|');
                if (configArr.Length < 3)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");
                if (configArr[0] != ProviderType.BKAV)
                    throw new Exception("Không đúng cấu hình nhà cung cấp BKAV");

                string[] accountArr = accountConfig.Split('|');
                if (accountArr.Length != 3)
                    throw new Exception("Sai định dạng cấu hình tài khoản.");

                if (this.ElectronicBillTypeEnum == ElectronicBillType.ENUM.CREATE_INVOICE)
                {
                    if (this.ElectronicBillDataInput == null)
                        throw new Exception("Không có dữ liệu phát hành hóa đơn.");
                    if (this.ElectronicBillDataInput.Treatment == null)
                        throw new Exception("Không có thông tin hồ sơ điều trị.");
                    if (this.ElectronicBillDataInput.Branch == null)
                        throw new Exception("Không có thông tin chi nhánh.");

                    if (configArr.Length > 3)
                    {
                        string cmdTypeCFG = configArr[3];
                        if (cmdTypeCFG == "1" && (String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.TemplateCode) || String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.SymbolCode)))
                        {
                            List<string> messError = new List<string>();
                            if (String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.TemplateCode))
                            {
                                messError.Add("Mẫu số");
                            }

                            if (String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.SymbolCode))
                            {
                                messError.Add("Ký hiệu");
                            }

                            throw new Exception(string.Format("Không có thông tin {0}.", string.Join(",", messError)));
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

        private List<Inventec.Common.EHoaDon.InvoiceDataWS> GetInvoiceContrBKAV(ElectronicBillDataInput electronicBillDataInput)
        {
            List<Inventec.Common.EHoaDon.InvoiceDataWS> InvoiceDataWSs = new List<Inventec.Common.EHoaDon.InvoiceDataWS>();
            Inventec.Common.EHoaDon.InvoiceDataWS invoiceDataWS = new Inventec.Common.EHoaDon.InvoiceDataWS();
            if (electronicBillDataInput != null)
            {
                switch (ElectronicBillTypeEnum)
                {
                    case ElectronicBillType.ENUM.CREATE_INVOICE:
                        #region CREATE INVOICE
                        invoiceDataWS.Invoice = SetInvoice();
                        invoiceDataWS.ListInvoiceDetailsWS = this.GetProductElectronicBill(this.ElectronicBillDataInput);
                        //Hien thi thong tin dich vu
                        invoiceDataWS.PartnerInvoiceID = GetTransactionCode();
                        invoiceDataWS.PartnerInvoiceStringID = "";
                        InvoiceDataWSs.Add(invoiceDataWS);
                        #endregion
                        break;
                    case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                    case ElectronicBillType.ENUM.GET_INVOICE_SHOW:
                    case ElectronicBillType.ENUM.DELETE_INVOICE:
                    case ElectronicBillType.ENUM.CANCEL_INVOICE:
                        #region GET INVOICE LINK
                        invoiceDataWS.PartnerInvoiceID = electronicBillDataInput.PartnerInvoiceID;
                        invoiceDataWS.Invoice = new Inventec.Common.EHoaDon.InvoiceWS();
                        invoiceDataWS.ListInvoiceAttachFileWS = new List<Inventec.Common.EHoaDon.InvoiceAttachFileWS>();
                        invoiceDataWS.ListInvoiceDetailsWS = new List<Inventec.Common.EHoaDon.InvoiceDetailsWS>();
                        invoiceDataWS.PartnerInvoiceStringID = "";
                        invoiceDataWS.Reason = GetReason();
                        InvoiceDataWSs.Add(invoiceDataWS);
                        #endregion
                        break;
                    default:
                        break;
                }
            }
            return InvoiceDataWSs;
        }

        private string GetReason()
        {
            string result = "";
            try
            {
                if (this.ElectronicBillDataInput == null)
                    return result;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("__ElectronicBillDataInput", ElectronicBillDataInput));

                result = this.ElectronicBillDataInput.CancelReason;
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private long GetTransactionCode()
        {
            if (this.ElectronicBillDataInput.Transaction != null)
            {
                return long.Parse(this.ElectronicBillDataInput.Transaction.TRANSACTION_CODE);
            }
            else if (this.ElectronicBillDataInput.ListTransaction != null && this.ElectronicBillDataInput.ListTransaction.Count > 0)
            {
                return long.Parse(this.ElectronicBillDataInput.ListTransaction.OrderByDescending(o => o.TRANSACTION_CODE).First().TRANSACTION_CODE);
            }
            else
            {
                return long.Parse(this.ElectronicBillDataInput.Treatment.TREATMENT_CODE + DateTime.Now.ToString("yyyyMMdd"));
            }
        }

        private Inventec.Common.EHoaDon.InvoiceWS SetInvoice()
        {
            Inventec.Common.EHoaDon.InvoiceWS invoiceWS = new Inventec.Common.EHoaDon.InvoiceWS();
            invoiceWS.InvoiceTypeID = 1;
            invoiceWS.InvoiceDate = DateTime.Now;
            invoiceWS.InvoiceForm = this.ElectronicBillDataInput.TemplateCode;
            invoiceWS.InvoiceSerial = this.ElectronicBillDataInput.SymbolCode;
            int invoiceType = 1;
            if (int.TryParse(this.ElectronicBillDataInput.TemplateCode, out invoiceType))
            {
                invoiceWS.InvoiceTypeID = invoiceType;
            }

            InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(this.ElectronicBillDataInput);
            invoiceWS.BuyerName = adoInfo.BuyerName ?? "";
            invoiceWS.BuyerBankAccount = adoInfo.BuyerAccountNumber ?? "";
            invoiceWS.BuyerAddress = adoInfo.BuyerAddress ?? "";
            invoiceWS.BuyerUnitName = adoInfo.BuyerOrganization ?? "";
            invoiceWS.BuyerTaxCode = adoInfo.BuyerTaxCode ?? "";

            invoiceWS.PayMethodID = 1;

            int payFormId = 1;

            if (ElectronicBillDataInput.Transaction != null)
            {
                if (ElectronicBillDataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK)
                {
                    payFormId = 2;
                }
                else if (ElectronicBillDataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    payFormId = 3;
                }
            }
            else if (ElectronicBillDataInput.ListTransaction != null && ElectronicBillDataInput.ListTransaction.Count > 0)
            {
                if (ElectronicBillDataInput.ListTransaction.First().PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK)
                {
                    payFormId = 2;
                }
                else if (ElectronicBillDataInput.ListTransaction.First().PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    payFormId = 3;
                }
            }

            invoiceWS.PayMethodID = payFormId;
            invoiceWS.ReceiveTypeID = 3;

            invoiceWS.ReceiverEmail = adoInfo.BuyerEmail ?? "";
            invoiceWS.ReceiverMobile = adoInfo.BuyerPhone ?? "";
            invoiceWS.ReceiverAddress = adoInfo.BuyerAddress ?? "";
            invoiceWS.ReceiverName = adoInfo.BuyerName ?? "";

            invoiceWS.Note = "";
            invoiceWS.BillCode = "";
            invoiceWS.CurrencyID = !String.IsNullOrEmpty(this.ElectronicBillDataInput.Currency) ? this.ElectronicBillDataInput.Currency : "";
            invoiceWS.ExchangeRate = 1;
            invoiceWS.InvoiceStatusID = 1;
            invoiceWS.SignedDate = DateTime.Now;

            UserDefineADO ado = GetUserDefine(this.ElectronicBillDataInput.Treatment);
            if (ado != null)
            {
                invoiceWS.UserDefine = Newtonsoft.Json.JsonConvert.SerializeObject(ado);
                invoiceWS.Note = ado.DEPARTMENT_NAME;
            }
            return invoiceWS;
        }

        private UserDefineADO GetUserDefine(V_HIS_TREATMENT_FEE treatment)
        {
            UserDefineADO result = null;
            try
            {
                if (treatment != null)
                {
                    result = new UserDefineADO();
                    result.END_CODE = treatment.END_CODE;
                    result.EXTRA_END_CODE = treatment.EXTRA_END_CODE;
                    result.MAIN_CAUSE = treatment.MAIN_CAUSE;
                    result.OUT_CODE = treatment.OUT_CODE;
                    result.STORE_CODE = treatment.STORE_CODE;
                    result.TOTAL_BILL_AMOUNT = treatment.TOTAL_BILL_AMOUNT;
                    result.TOTAL_BILL_EXEMPTION = treatment.TOTAL_BILL_EXEMPTION;
                    result.TOTAL_BILL_FUND = treatment.TOTAL_BILL_FUND;
                    result.TOTAL_BILL_OTHER_AMOUNT = treatment.TOTAL_BILL_OTHER_AMOUNT;
                    result.TOTAL_BILL_TRANSFER_AMOUNT = treatment.TOTAL_BILL_TRANSFER_AMOUNT;
                    result.TOTAL_DEBT_AMOUNT = treatment.TOTAL_DEBT_AMOUNT;
                    result.TOTAL_DEPOSIT_AMOUNT = treatment.TOTAL_DEPOSIT_AMOUNT;
                    result.TOTAL_DISCOUNT = treatment.TOTAL_DISCOUNT;
                    result.TOTAL_HEIN_PRICE = treatment.TOTAL_HEIN_PRICE;
                    result.TOTAL_PATIENT_PRICE = treatment.TOTAL_PATIENT_PRICE;
                    result.TOTAL_PRICE = treatment.TOTAL_PRICE;
                    result.TOTAL_PRICE_EXPEND = treatment.TOTAL_PRICE_EXPEND;
                    result.TOTAL_REPAY_AMOUNT = treatment.TOTAL_REPAY_AMOUNT;
                    result.TREATMENT_CODE = treatment.TREATMENT_CODE;
                    result.TREATMENT_DAY_COUNT = treatment.TREATMENT_DAY_COUNT;

                    CommonParam param = new CommonParam();
                    HisDepartmentTranLastFilter depaFilter = new HisDepartmentTranLastFilter();
                    depaFilter.TREATMENT_ID = treatment.ID;
                    var lastDepartment = new BackendAdapter(param).Get<V_HIS_DEPARTMENT_TRAN>("/api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, depaFilter, param);
                    if (lastDepartment != null)
                    {
                        result.DEPARTMENT_CODE = lastDepartment.DEPARTMENT_CODE;
                        result.DEPARTMENT_NAME = lastDepartment.DEPARTMENT_NAME;
                        result.Khoa = lastDepartment.DEPARTMENT_NAME;
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

        private List<Inventec.Common.EHoaDon.InvoiceDetailsWS> GetProductElectronicBill(ElectronicBillDataInput dataInput)
        {
            List<Inventec.Common.EHoaDon.InvoiceDetailsWS> products = new List<Inventec.Common.EHoaDon.InvoiceDetailsWS>();
            IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, dataInput);
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
                    Inventec.Common.EHoaDon.InvoiceDetailsWS product = new Inventec.Common.EHoaDon.InvoiceDetailsWS();
                    product.Qty = (double)(item.ProdQuantity ?? 0);
                    product.ItemName = item.ProdName ?? "";
                    product.UnitName = item.ProdUnit ?? "";
                    product.TaxRateID = item.TaxRateID > 0 ? item.TaxRateID : Base.ProviderType.tax_KCT;
                    product.Amount = (double)item.Amount;
                    product.Price = (double)(item.ProdPrice ?? 0);

                    products.Add(product);
                }
            }
            else
            {
                List<ProductBasePlus> listProductBase = (List<ProductBasePlus>)listProduct;

                if (listProductBase == null || listProductBase.Count == 0)
                {
                    throw new Exception("Không có thông tin chi tiết dịch vụ.");
                }

                //"TaxRateID": 3,//ID Thuế suất: 1-0%, 2-5%, 3-10%, 4-Không chịu thuế, 5-Không kê khai thuế , 6-Khác
                //"TaxRate": 10.0, //Thuế suất: 0, 5, 10, -1 (Không chịu thuế), -2 (Không kê khai thuế), -4 (Thuế nhà thầu)

                foreach (var item in listProductBase)
                {
                    Inventec.Common.EHoaDon.InvoiceDetailsWS product = new Inventec.Common.EHoaDon.InvoiceDetailsWS();
                    product.Qty = (double)(item.ProdQuantity ?? 0);
                    product.ItemName = item.ProdName ?? "";
                    product.UnitName = item.ProdUnit ?? "";
                    product.Amount = (double)item.AmountWithoutTax;
                    product.TaxAmount = (double)item.TaxAmount;
                    product.Price = (double)(item.ProdPrice ?? 0);

                    if (!item.TaxPercentage.HasValue)
                    {
                        product.TaxRateID = 4;
                    }
                    else if (item.TaxPercentage == 0)
                    {
                        product.TaxRateID = 1;
                    }
                    else if (item.TaxPercentage == 1)
                    {
                        product.TaxRateID = 2;
                    }
                    else if (item.TaxPercentage == 2)
                    {
                        product.TaxRateID = 3;
                    }
                    else if (item.TaxPercentage == 3)
                    {
                        product.TaxRateID = 6;
                    }
                    else
                    {
                        product.TaxRateID = 6;
                    }

                    products.Add(product);
                }
            }

            return products;
        }
    }
}
