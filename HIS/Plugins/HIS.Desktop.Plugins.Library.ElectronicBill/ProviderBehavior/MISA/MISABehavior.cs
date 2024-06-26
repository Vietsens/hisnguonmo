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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.Adapter;
using Inventec.Common.ElectronicBill.Misa.Model;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MISA
{
    class MISABehavior : IRun
    {
        private Base.ElectronicBillDataInput ElectronicBillDataInput;
        private string serviceConfig;
        private string accountConfig;
        private TemplateEnum.TYPE TempType;

        Inventec.Common.ElectronicBill.Misa.DataInit DataInit;
        private bool IsRelease;
        bool InChuyenDoi = false;
        private Response InvoiceRelease;
        private Response InvoiceCreate;

        private bool IsAutoSign;

        public MISABehavior(Base.ElectronicBillDataInput electronicBillDataInput, string serviceConfig, string accountConfig)
        {
            // TODO: Complete member initialization
            this.ElectronicBillDataInput = electronicBillDataInput;
            this.serviceConfig = serviceConfig;
            this.accountConfig = accountConfig;
        }

        ElectronicBillResult IRun.Run(ElectronicBillType.ENUM _electronicBillTypeEnum, TemplateEnum.TYPE _tempType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.Check(_electronicBillTypeEnum, ref result))
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

                    string appId = configArr[2];
                    if (String.IsNullOrEmpty(appId))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong co thong tin id ung dung");
                        ElectronicBillResultUtil.Set(ref result, false, "Không có thông tin id ứng dụng");
                        return result;
                    }

                    string signUrl = configArr[3];
                    if (String.IsNullOrEmpty(signUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong co thong tin dia chi may chu ky so");
                        ElectronicBillResultUtil.Set(ref result, false, "Không có thông tin địa chỉ máy chủ ký số");
                        return result;
                    }

                    if (configArr.Count() > 5)
                    {
                        string autoSign = configArr[5];
                        this.IsAutoSign = autoSign == "1";
                    }

                    string ConfigV3 = "";
                    if (configArr.Count() > 7)
                    {
                        ConfigV3 = configArr[7];
                    }

                    if (configArr.Count() > 8)
                    {
                        this.InChuyenDoi = configArr[8].Trim() == "1";
                    }

                    if (ConfigV3.Trim() == "1")
                    {
                        IRun iRun = new MISABehaviorV2(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                        result = iRun != null ? iRun.Run(_electronicBillTypeEnum, TempType) : null;
                    }
                    else
                    {
                        string[] accountConfigArr = accountConfig.Split('|');
                        DataInit = new Inventec.Common.ElectronicBill.Misa.DataInit();
                        DataInit.BaseUrl = serviceUrl;
                        DataInit.AppID = appId;
                        DataInit.SignUrl = signUrl;
                        DataInit.TaxCode = this.ElectronicBillDataInput.Branch.TAX_CODE;
                        DataInit.User = accountConfigArr[0].Trim();
                        DataInit.Pass = accountConfigArr[1].Trim();

                        switch (_electronicBillTypeEnum)
                        {
                            case ElectronicBillType.ENUM.CREATE_INVOICE:
                                ProcessCreateInvoice(ref result);
                                break;
                            case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                                ProcessGetInvoice(ref result);
                                break;
                            case ElectronicBillType.ENUM.DELETE_INVOICE:
                            case ElectronicBillType.ENUM.CANCEL_INVOICE:
                                ProcessCancelInvoice(ref result);
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

        private void ProcessCancelInvoice(ref ElectronicBillResult result)
        {
            try
            {
                DeleteInvoiceData dInvoice = new DeleteInvoiceData();

                dInvoice.DeletedReason = this.ElectronicBillDataInput.CancelReason;
                dInvoice.RefDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.ElectronicBillDataInput.CancelTime ?? 0) ?? DateTime.Now;
                dInvoice.RefNo = this.ElectronicBillDataInput.ENumOrder;
                dInvoice.TransactionID = this.ElectronicBillDataInput.InvoiceCode;

                Inventec.Common.ElectronicBill.Misa.Model.Response response = null;
                DataInit.DataDelete = dInvoice;

                var eMoit = new Inventec.Common.ElectronicBill.Misa.ElectronicBillMisaManager(DataInit);
                if (eMoit != null)
                {
                    response = eMoit.Run(Inventec.Common.ElectronicBill.Misa.Type.DeleteInvoice);
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response));

                if (response != null && String.IsNullOrWhiteSpace(response.description))
                {
                    //Thanh cong
                    result.Success = true;
                    result.InvoiceSys = ProviderType.MISA;
                    result.Messages = new List<string>() { response.description };
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MISA;
                    result.Messages = new List<string>() { response.description };
                    Inventec.Common.Logging.LogSystem.Error("Huy hoa don dien tu that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dInvoice), dInvoice));
                    ElectronicBillResultUtil.Set(ref result, false, response.description);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetInvoice(ref ElectronicBillResult result)
        {
            try
            {
                GetInvoice invoices = new GetInvoice();
                string converter = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                if (!String.IsNullOrWhiteSpace(ElectronicBillDataInput.Converter))
                {
                    converter = ElectronicBillDataInput.Converter;
                }

                invoices.Converter = converter;
                invoices.TransactionID = ElectronicBillDataInput.InvoiceCode;

                Inventec.Common.ElectronicBill.Misa.Model.Response response = null;
                DataInit.DataGet = invoices;

                var eMoit = new Inventec.Common.ElectronicBill.Misa.ElectronicBillMisaManager(DataInit);
                if (eMoit != null)
                {
                    if (this.InChuyenDoi)
                        response = eMoit.Run(Inventec.Common.ElectronicBill.Misa.Type.ConvertInvoice);
                    else
                        response = eMoit.Run(Inventec.Common.ElectronicBill.Misa.Type.ViewInvoice);
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response));

                if (response != null && response.fileToBytes != null)
                {
                    string fullFileName = ProcessPdfFileResult(response.fileToBytes);

                    //Thanh cong
                    result.Success = true;
                    result.InvoiceSys = ProviderType.MISA;
                    result.InvoiceLink = fullFileName;

                    Inventec.Common.Logging.LogSystem.Debug("_____PDF_FILE_NAME: " + fullFileName);
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MISA;
                    Inventec.Common.Logging.LogSystem.Error("lay file chuyen doi that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoices), invoices));
                    ElectronicBillResultUtil.Set(ref result, false, response.description);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool Check(ElectronicBillType.ENUM _electronicBillTypeEnum, ref ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = serviceConfig.Split('|');
                if (configArr.Length < 5)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");

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

        private void ProcessCreateInvoice(ref ElectronicBillResult result)
        {
            try
            {
                if (this.ElectronicBillDataInput.Transaction == null || ElectronicBillDataInput.Transaction.ID <= 0)
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MISA;
                    ElectronicBillResultUtil.Set(ref result, false, "Chưa hỗ trợ xuất hóa đơn gộp!");
                    throw new Exception("Chưa hỗ trợ xuất hóa đơn gộp!");
                }

                Inventec.Common.ElectronicBill.Misa.DataInit login = new Inventec.Common.ElectronicBill.Misa.DataInit();
                Inventec.Common.Mapper.DataObjectMapper.Map<Inventec.Common.ElectronicBill.Misa.DataInit>(login, DataInit);

                CreateInvoiceData invoices = this.GetInvoice(this.ElectronicBillDataInput, ref result);
                Inventec.Common.ElectronicBill.Misa.Model.Response responsePreview = null;
                login.DataCreate = new List<CreateInvoiceData>() { invoices };
                login.DataPreview = invoices;

                var eMoit = new Inventec.Common.ElectronicBill.Misa.ElectronicBillMisaManager(login);
                if (eMoit != null)
                {
                    this.InvoiceCreate = eMoit.Run(Inventec.Common.ElectronicBill.Misa.Type.CreateInvoice);
                    if (!IsAutoSign)
                    {
                        responsePreview = eMoit.Run(Inventec.Common.ElectronicBill.Misa.Type.PreviewInvoice);
                    }
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.InvoiceCreate), this.InvoiceCreate));

                // tạo thành công hóa đơn
                if (this.InvoiceCreate != null && String.IsNullOrWhiteSpace(this.InvoiceCreate.description) && this.InvoiceCreate.result != null)
                {
                    if (IsAutoSign)//tự động ký
                    {
                        string error = "";
                        if (DoSignAndReleaseInvoice(null, ref error) && InvoiceRelease != null)
                        {
                            result.InvoiceSys = ProviderType.MISA;
                            result.InvoiceCode = InvoiceRelease.result.First().TransactionID;
                            result.InvoiceNumOrder = InvoiceRelease.result.First().InvoiceNumber;
                            result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(InvoiceRelease.result.First().InvoiceIssuedDate);
                            result.InvoiceLoginname = login.User;
                            ElectronicBillResultUtil.Set(ref result, String.IsNullOrWhiteSpace(InvoiceRelease.description), InvoiceRelease.description);
                        }
                        else
                        {
                            ElectronicBillResultUtil.Set(ref result, false, error);
                        }
                    }
                    else//không cấu hình thì hiển thị cửa sổ ký
                    {
                        //Thanh cong
                        SignInitData init = new SignInitData();
                        init.ContentSign = this.InvoiceCreate.result.First().InvoiceData;
                        init.fileToBytes = responsePreview.fileToBytes;
                        init.SignAndRelease = (DelegateSignAndRelease)DoSignAndReleaseInvoice;

                        SignInvoice.FormSignInvoice form = new SignInvoice.FormSignInvoice(init);
                        form.ShowDialog();
                        if (IsRelease && InvoiceRelease != null)
                        {
                            result.InvoiceSys = ProviderType.MISA;
                            result.InvoiceCode = InvoiceRelease.result.First().TransactionID;
                            result.InvoiceNumOrder = InvoiceRelease.result.First().InvoiceNumber;
                            result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(InvoiceRelease.result.First().InvoiceIssuedDate);
                            result.InvoiceLoginname = login.User;
                            ElectronicBillResultUtil.Set(ref result, String.IsNullOrWhiteSpace(InvoiceRelease.description), InvoiceRelease.description);
                        }
                    }
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.MISA;
                    Inventec.Common.Logging.LogSystem.Error("tao hoa doi dien tu that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.InvoiceCreate), this.InvoiceCreate) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoices), invoices));
                    ElectronicBillResultUtil.Set(ref result, false, this.InvoiceCreate != null ? this.InvoiceCreate.description : "");
                }
            }
            catch (Exception ex)
            {
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DoSignAndReleaseInvoice(SignDelegate data, ref string errorMessage)
        {
            bool result = true;
            try
            {
                if (this.InvoiceCreate != null && String.IsNullOrWhiteSpace(this.InvoiceCreate.description) && this.InvoiceCreate.result != null)
                {
                    string[] configArr = serviceConfig.Split('|');

                    Inventec.Common.ElectronicBill.Misa.DataInit login = new Inventec.Common.ElectronicBill.Misa.DataInit();
                    Inventec.Common.Mapper.DataObjectMapper.Map<Inventec.Common.ElectronicBill.Misa.DataInit>(login, DataInit);

                    Inventec.Common.ElectronicBill.Misa.Model.Response signXnlData = null;
                    SignXml dataSign = new SignXml();
                    dataSign.PinCode = configArr[4];
                    dataSign.XmlContent = this.InvoiceCreate.result.First().InvoiceData;
                    login.DataSign = dataSign;

                    var eMoit = new Inventec.Common.ElectronicBill.Misa.ElectronicBillMisaManager(login);
                    if (eMoit != null)
                    {
                        signXnlData = eMoit.Run(Inventec.Common.ElectronicBill.Misa.Type.SignInvoice);
                        if (signXnlData != null && !String.IsNullOrWhiteSpace(signXnlData.XmlData))
                        {
                            ReleaseInvoiceData dataRelease = new ReleaseInvoiceData();
                            dataRelease.InvoiceData = signXnlData.XmlData;
                            dataRelease.RefID = this.InvoiceCreate.result.First().RefID;
                            dataRelease.TransactionID = this.InvoiceCreate.result.First().TransactionID;

                            if (data != null)
                            {
                                dataRelease.ReceiverEmail = data.Email;
                                dataRelease.ReceiverName = data.Name;
                                dataRelease.IsSendEmail = !string.IsNullOrWhiteSpace(data.Name) && !String.IsNullOrWhiteSpace(data.Email);
                            }

                            login.DataRelease = new List<ReleaseInvoiceData>() { dataRelease };
                            var release = new Inventec.Common.ElectronicBill.Misa.ElectronicBillMisaManager(login);
                            if (release != null)
                            {
                                InvoiceRelease = release.Run(Inventec.Common.ElectronicBill.Misa.Type.ReleaseInvoice);
                                if (InvoiceRelease != null && String.IsNullOrWhiteSpace(InvoiceRelease.description) && InvoiceRelease.result != null)
                                {
                                    IsRelease = true;
                                }
                                else
                                {
                                    errorMessage = "Phát hành thất bại. " + (InvoiceRelease != null ? InvoiceRelease.description : "");
                                    result = false;
                                }
                            }
                        }
                        else
                        {
                            errorMessage = "Ký số thất bại. " + (signXnlData != null ? signXnlData.description : "");
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private CreateInvoiceData GetInvoice(Base.ElectronicBillDataInput electronicBillDataInput, ref ElectronicBillResult dataResult)
        {
            CreateInvoiceData result = null;
            if (electronicBillDataInput != null)
            {
                result = new CreateInvoiceData();
                result.AdjustmentType = 1;

                InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(this.ElectronicBillDataInput, false);

                result.BuyerAddressLine = adoInfo.BuyerAddress;
                result.BuyerBankAccount = adoInfo.BuyerAccountNumber;
                result.BuyerBankName = "";
                result.BuyerDisplayName = adoInfo.BuyerName;
                result.BuyerEmail = adoInfo.BuyerEmail;
                result.BuyerLegalName = adoInfo.BuyerOrganization;
                result.BuyerPhoneNumber = adoInfo.BuyerPhone;
                result.BuyerTaxCode = adoInfo.BuyerTaxCode;

                if (electronicBillDataInput.Transaction != null)
                {
                    result.RefID = electronicBillDataInput.Transaction.TRANSACTION_CODE;
                }
                else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                {
                    result.RefID = ElectronicBillDataInput.ListTransaction.Select(s => s.TRANSACTION_CODE).OrderBy(o => o).FirstOrDefault();
                }

                result.CurrencyCode = "VND";
                if (TempType != TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    result.InvoiceNote = "Thanh toán viện phí";
                }
                else
                    result.InvoiceNote = "Hóa đơn bán lẻ";

                result.InvoiceSeries = electronicBillDataInput.SymbolCode;
                result.TemplateCode = electronicBillDataInput.TemplateCode;
                //result.PaymentMethodName = electronicBillDataInput.PaymentMethod;
                result.InvoiceType = GetInvoiceType(ElectronicBillDataInput.TemplateCode);

                string paymentName = electronicBillDataInput.PaymentMethod;

                if (electronicBillDataInput.Transaction != null)
                {
                    HIS_PAY_FORM payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == electronicBillDataInput.Transaction.PAY_FORM_ID);
                    if (payForm != null)
                    {
                        paymentName = payForm.ELECTRONIC_PAY_FORM_NAME ?? payForm.PAY_FORM_NAME;
                    }
                }

                result.PaymentMethodName = paymentName;

                if (ElectronicBillDataInput.Branch != null)
                {
                    result.SellerAddressLine = ElectronicBillDataInput.Branch.ADDRESS;
                    result.SellerBankAccount = ElectronicBillDataInput.Branch.ACCOUNT_NUMBER;
                    result.SellerBankName = ElectronicBillDataInput.Branch.BANK_INFO;
                    result.SellerLegalName = ElectronicBillDataInput.Branch.BRANCH_NAME;
                    result.SellerTaxCode = ElectronicBillDataInput.Branch.TAX_CODE;
                }

                result.ExchangeRate = 1;
                //result.DiscountAmount = ElectronicBillDataInput.Discount ?? 0;

                result.OptionUserDefined = ProcessUserDefined();


                if (electronicBillDataInput.Transaction != null)
                {
                    result.CustomField1 = this.ElectronicBillDataInput.Transaction.TDL_TREATMENT_CODE;
                }
                else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                {
                    result.CustomField1 = this.ElectronicBillDataInput.ListTransaction.Where(o => !String.IsNullOrWhiteSpace(o.TDL_TREATMENT_CODE)).Select(s => s.TDL_TREATMENT_CODE).FirstOrDefault();
                }

                if (!String.IsNullOrWhiteSpace(HisConfigCFG.ElectronicBillXmlInvoicePlus))
                {
                    string[] xmlKeys = HisConfigCFG.ElectronicBillXmlInvoicePlus.Split('|');
                    Dictionary<string, string> dicXmlKey = new Dictionary<string, string>();
                    foreach (var item in xmlKeys)
                    {
                        string[] keys = item.Split(':');
                        if (keys.Count() > 1)
                        {
                            string value = item.Replace(keys[0] + ":", "");
                            //không có dữ liệu thì gán null để mất thẻ trong XML
                            //dùng Empty để cho phép nhập khoảng trắng trong cấu hình
                            if (!String.IsNullOrEmpty(value))
                            {
                                dicXmlKey[keys[0]] = value;
                            }
                            else
                            {
                                dicXmlKey[keys[0]] = null;
                            }
                        }
                    }

                    Dictionary<string, string> dicXmlValues = General.ProcessDicValueString(this.ElectronicBillDataInput);
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(typeof(CreateInvoiceV2));

                    foreach (var keys in dicXmlKey)
                    {
                        string value = keys.Value;
                        if (!String.IsNullOrWhiteSpace(value))
                        {
                            foreach (var values in dicXmlValues)
                            {
                                value = value.Replace(values.Key, values.Value);
                            }
                        }

                        foreach (var item in pi)
                        {
                            if (item.Name == keys.Key)
                            {
                                item.SetValue(result, value);
                                break;
                            }
                        }
                    }
                }

                var detail = GetProductElectronicBill();
                if (detail != null && detail.Count > 0)
                {
                    result.OriginalInvoiceDetail = detail;

                    if (detail != null && detail.Count > 0)
                    {
                        decimal amount = detail.Sum(s => s.Amount + s.VatAmount);

                        result.TotalAmountWithoutVAT = detail.Sum(s => s.Amount);
                        result.TotalAmountWithVAT = amount;
                        result.TotalAmountWithVATFrn = amount;
                        result.TotalVATAmount = detail.Sum(s => s.VatAmount);
                        result.VatPercentage = -1;
                        result.TotalAmountWithVATInWords = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", amount)) + "đồng";
                    }
                }
            }

            return result;
        }

        private UserDefined ProcessUserDefined()
        {
            UserDefined result = new UserDefined();

            result.MainCurrency = "VND";
            result.AmountDecimalDigits = "0";
            result.AmountOCDecimalDigits = "0";
            result.CoefficientDecimalDigits = "0";
            result.ExchangRateDecimalDigits = "0";
            result.QuantityDecimalDigits = "0";
            result.UnitPriceDecimalDigits = "0";
            result.UnitPriceOCDecimalDigits = "0";

            return result;
        }

        private List<InvoiceDetail> GetProductElectronicBill()
        {
            List<InvoiceDetail> result = new List<InvoiceDetail>();
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

                int count = 1;
                foreach (var item in listProductBase)
                {
                    InvoiceDetail product = new InvoiceDetail();
                    product.LineNumber = count;
                    product.ItemCode = GetFirstWord(item.ProdName) ?? " ";
                    product.ItemName = item.ProdName;
                    product.UnitName = item.ProdUnit;
                    product.Quantity = item.ProdQuantity ?? 0;
                    product.VatAmount = 0;
                    product.VatPercentage = -1;
                    product.Amount = item.Amount;
                    product.UnitPrice = item.ProdPrice ?? 0;

                    result.Add(product);
                    count++;
                }
            }
            return result;
        }

        private string GetFirstWord(string name)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    var spl = name.Split(' ', ',', '.').ToList();
                    foreach (var item in spl)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            result += item[0].ToString().ToUpper();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = name.Split(' ', ',', '.').FirstOrDefault();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //Mã loại hóa đơn chỉ nhận các giá trị sau: 01GTKT, 02GTTT, 07KPTQ, 03XKNB, 04HGDL, 01BLP
        private string GetInvoiceType(string p)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(p))
                {
                    string num = p.Substring(0, 2);
                    if (num == "01")
                    {
                        string tp = p.Substring(0, 5);
                        if (tp == "01BLP")
                        {
                            result = "01BLP";
                        }
                        else
                        {
                            result = "01GTKT";
                        }
                    }
                    else if (num == "02")
                    {
                        result = "02GTTT";
                    }
                    else if (num == "03")
                    {
                        result = "03XKNB";
                    }
                    else if (num == "04")
                    {
                        result = "04HGDL";
                    }
                    else if (num == "07")
                    {
                        result = "07KPTQ";
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

        #region file
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

        private bool WriteByteArrayToFile(byte[] byData, string fileName)
        {
            bool result = true;
            try
            {
                using (FileStream file_stream = File.Open(fileName, FileMode.CreateNew))
                {
                    file_stream.Write(byData, 0, byData.Length);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion
    }
}
