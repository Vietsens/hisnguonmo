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
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.Adapter;
using Inventec.Common.ElectronicBillViettel.Model;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VIETTEL
{
    class VIETTELBehavior : IRun
    {
        private Base.ElectronicBillDataInput ElectronicBillDataInput;
        private string ServiceConfig;
        private string AccountConfig;
        private TemplateEnum.TYPE TempType;

        public VIETTELBehavior(Base.ElectronicBillDataInput electronicBillDataInput, string serviceConfig, string accountConfig)
        {
            // TODO: Complete member initialization
            this.ElectronicBillDataInput = electronicBillDataInput;
            this.ServiceConfig = serviceConfig;
            this.AccountConfig = accountConfig;
        }

        ElectronicBillResult IRun.Run(ElectronicBillType.ENUM _electronicBillTypeEnum, TemplateEnum.TYPE _tempType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.Check(ref result))
                {
                    this.TempType = _tempType;
                    string[] configArr = ServiceConfig.Split('|');

                    string serviceUrl = configArr[1]; //ConfigurationManager.AppSettings[AppConfigKey.WEBSERVICE_URL];
                    if (String.IsNullOrEmpty(serviceUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay dia chi Webservice URL");
                        ElectronicBillResultUtil.Set(ref result, false, "Không tìm thấy địa chỉ Webservice URL");
                        return result;
                    }

                    string version = "";
                    if (configArr.Count() > 2)
                    {
                        version = configArr[2];
                    }
                    bool IsTemp = false;
                    if (configArr.Count() > 3)
                    {
                        IsTemp = configArr[3] == "1";
                        if (IsTemp && _electronicBillTypeEnum != ElectronicBillType.ENUM.CREATE_INVOICE)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Tính năng không được hỗ trợ khi phát hành hóa đơn nháp!");
                            ElectronicBillResultUtil.Set(ref result, false, "Tính năng không được hỗ trợ khi phát hành hóa đơn nháp!");
                            return result;
                        }
                    }
                    string[] accountConfigArr = AccountConfig.Split('|');

                    Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin = new Inventec.Common.ElectronicBillViettel.DataInitApi();
                    viettelLogin.VIETTEL_Address = serviceUrl;
                    viettelLogin.User = accountConfigArr[0].Trim();
                    viettelLogin.Pass = accountConfigArr[1].Trim();
                    viettelLogin.IsTemp = IsTemp;
                    viettelLogin.SupplierTaxCode = ElectronicBillDataInput.Branch.TAX_CODE;
                    if (version == "2")
                    {
                        viettelLogin.Version = Inventec.Common.ElectronicBillViettel.Version.v2;
                    }

                    switch (_electronicBillTypeEnum)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            ProcessCreateInvoice(viettelLogin, ref result);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            if (HisConfigCFG.IsPrintNormal)
                            {
                                ProcessGetInvoiceNormal(viettelLogin, ref result);
                            }
                            else
                            {
                                ProcessGetInvoice(viettelLogin, ref result);
                            }
                            break;
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            ProcessCancelInvoice(viettelLogin, ref result);
                            break;
                        case ElectronicBillType.ENUM.CREATE_INVOICE_DATA:
                            ProcessCreateInvoiceSaveData(viettelLogin, ref result);
                            break;
                        case ElectronicBillType.ENUM.CONVERT_INVOICE:
                            ProcessGetInvoice(viettelLogin, ref result);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessCreateInvoiceSaveData(Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin, ref ElectronicBillResult result)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.SaveFileName))
                {
                    // Check if file already exists. If yes, delete it.     
                    if (File.Exists(this.ElectronicBillDataInput.SaveFileName))
                    {
                        File.Delete(this.ElectronicBillDataInput.SaveFileName);
                    }

                    DataCreateInvoice invoiceData = this.CreateInvoice(this.ElectronicBillDataInput, viettelLogin);

                    string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(invoiceData);

                    // Open the stream and read it back.    
                    using (var file = new StreamWriter(this.ElectronicBillDataInput.SaveFileName))
                    {
                        file.Write(sendJsonData);
                    }

                    ElectronicBillResultUtil.Set(ref result, true, "");
                }
                else
                {
                    ElectronicBillResultUtil.Set(ref result, false, "Không có đường dẫn lưu file");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCancelInvoice(Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin, ref ElectronicBillResult result)
        {
            try
            {
                CancelInvoice invoiceData = new CancelInvoice();
                invoiceData.additionalReferenceDesc = ElectronicBillDataInput.CancelReason;
                if (!String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.ENumOrder) && this.ElectronicBillDataInput.ENumOrder.Length >= 7)
                {
                    invoiceData.invoiceNo = this.ElectronicBillDataInput.ENumOrder;
                }
                else
                {
                    invoiceData.invoiceNo = this.ElectronicBillDataInput.SymbolCode + GetVirNumOrder(long.Parse(this.ElectronicBillDataInput.InvoiceCode.Split('|').Last()));
                }

                if (viettelLogin.Version == Inventec.Common.ElectronicBillViettel.Version.v2)
                {
                    invoiceData.strIssueDate = GetTimeMilliseconds(ElectronicBillDataInput.TransactionTime) + "";
                    invoiceData.additionalReferenceDate = GetTimeMilliseconds(ElectronicBillDataInput.CancelTime ?? 0) + "";
                }
                else
                {
                    invoiceData.additionalReferenceDate = ElectronicBillDataInput.CancelTime.ToString();
                    invoiceData.strIssueDate = ElectronicBillDataInput.TransactionTime.ToString();
                }

                //invoiceData.strIssueDate = ElectronicBillDataInput.TransactionTime.ToString();
                invoiceData.supplierTaxCode = ElectronicBillDataInput.Branch.TAX_CODE;
                invoiceData.templateCode = ElectronicBillDataInput.TemplateCode;
                Response response = null;

                var eViettel = new Inventec.Common.ElectronicBillViettel.ElectronicBillViettelManager(viettelLogin);
                if (eViettel != null)
                {
                    response = eViettel.Run(invoiceData);
                }

                if (response != null && String.IsNullOrWhiteSpace(response.errorCode))
                {
                    //Thanh cong
                    result.Success = true;
                    result.InvoiceSys = ProviderType.VIETTEL;
                    result.Messages = new List<string>() { response.description };
                    //result.InvoiceLink = configArr[2] + "/" + item.MessLog;
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.VIETTEL;
                    if (response != null)
                    {
                        result.Messages = new List<string>() { response.description };
                        Inventec.Common.Logging.LogSystem.Error("Huy hoa don dien tu that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceData), invoiceData));
                        ElectronicBillResultUtil.Set(ref result, false, response.description);
                    }
                    else
                    {
                        ElectronicBillResultUtil.Set(ref result, false, "");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Get Invoice PDF
        private void ProcessGetInvoice(Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin, ref ElectronicBillResult result)
        {
            try
            {
                #region tải từ FSS
                ////kiểm tra thông tin đường dẫn trước khi gọi sang viettel
                //bool hasData = false;
                //if (viettelLogin.Version == Inventec.Common.ElectronicBillViettel.Version.v2)
                //{
                //    MemoryStream stream = null;
                //    if (this.ElectronicBillDataInput.Transaction != null && !String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.Transaction.EINVOICE_URL))
                //    {
                //        try
                //        {
                //            stream = Inventec.Fss.Client.FileDownload.GetFile(this.ElectronicBillDataInput.Transaction.EINVOICE_URL);
                //        }
                //        catch (Exception ex)
                //        {
                //            Inventec.Common.Logging.LogSystem.Error(ex);
                //        }
                //    }
                //    else
                //    {
                //        CommonParam param = new CommonParam();
                //        HisTransactionFilter filter = new HisTransactionFilter();
                //        filter.INVOICE_CODE__EXACT = this.ElectronicBillDataInput.InvoiceCode;
                //        List<HIS_TRANSACTION> transactions = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("/api/HisTransaction/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                //        if (transactions != null && transactions.Count > 0)
                //        {
                //            try
                //            {
                //                var trans = transactions.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.EINVOICE_URL));
                //                if (trans != null)
                //                {
                //                    stream = Inventec.Fss.Client.FileDownload.GetFile(trans.EINVOICE_URL);
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                Inventec.Common.Logging.LogSystem.Error(ex);
                //            }
                //        }
                //    }

                //    if (stream != null)
                //    {
                //        stream.Position = 0;
                //        string fullFileName = ProcessPdfFileResult(stream.ToArray());

                //        //Thanh cong
                //        result.Success = true;
                //        result.InvoiceSys = ProviderType.VIETTEL;
                //        result.InvoiceLink = fullFileName;
                //        hasData = true;
                //    }
                //}

                //if (hasData)
                //{
                //    return;
                //}
                #endregion

                GetFile invoiceData = new GetFile();
                string converter = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                if (!String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.Converter))
                {
                    converter = this.ElectronicBillDataInput.Converter;
                }

                invoiceData.exchangeUser = converter;

                if (viettelLogin.Version == Inventec.Common.ElectronicBillViettel.Version.v2)
                {
                    invoiceData.strIssueDate = GetTimeMilliseconds(this.ElectronicBillDataInput.TransactionTime) + "";
                }
                else
                {
                    invoiceData.strIssueDate = this.ElectronicBillDataInput.TransactionTime.ToString();
                }

                invoiceData.supplierTaxCode = this.ElectronicBillDataInput.Branch.TAX_CODE;
                invoiceData.templateCode = this.ElectronicBillDataInput.TemplateCode;
                if (!String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.ENumOrder) && this.ElectronicBillDataInput.ENumOrder.Length >= 7)
                {
                    invoiceData.invoiceNo = this.ElectronicBillDataInput.ENumOrder;
                }
                else
                {
                    invoiceData.invoiceNo = this.ElectronicBillDataInput.SymbolCode + GetVirNumOrder(long.Parse(this.ElectronicBillDataInput.InvoiceCode.Split('|').Last()));
                }

                Response response = null;

                var eViettel = new Inventec.Common.ElectronicBillViettel.ElectronicBillViettelManager(viettelLogin);
                if (eViettel != null)
                {
                    response = eViettel.Run(invoiceData);
                }

                //dữ liệu pdf nhiều nên không log
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response));
                string updateloginname = "";
                if (response != null && String.IsNullOrWhiteSpace(response.errorCode))
                {
                    string fullFileName = ProcessPdfFileResult(response.fileToBytes);
                    Inventec.Common.Logging.LogSystem.Debug("_____PDF_FILE_NAME: " + fullFileName);

                    //Thanh cong
                    result.Success = true;
                    result.InvoiceSys = ProviderType.VIETTEL;
                    result.InvoiceLink = fullFileName;

                    //in chuyển đổi lần 1 thành công
                    if (this.ElectronicBillDataInput.Transaction != null && this.ElectronicBillDataInput.Transaction.EINVOICE_LOGINNAME != viettelLogin.User)
                    {
                        updateloginname = viettelLogin.User;
                    }
                }
                else if (viettelLogin.Version == Inventec.Common.ElectronicBillViettel.Version.v2)
                {
                    //nếu có thông tin thì lấy theo tài khoản tạo ko có thì lấy theo tài khoản thu ngân
                    if (this.ElectronicBillDataInput.Transaction != null && !String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.Transaction.EINVOICE_LOGINNAME))
                    {
                        invoiceData.exchangeUser = this.ElectronicBillDataInput.Transaction.EINVOICE_LOGINNAME;
                    }
                    else if (this.ElectronicBillDataInput.Transaction != null)
                    {
                        invoiceData.exchangeUser = GetExchangeUser(this.ElectronicBillDataInput.Transaction.CASHIER_LOGINNAME);
                    }

                    if (String.IsNullOrWhiteSpace(invoiceData.exchangeUser))
                    {
                        //luôn lấy theo cấu hình tài khoản đang đăng nhập nếu ko có thông tin
                        invoiceData.exchangeUser = viettelLogin.User;
                    }

                    response = eViettel.Run(invoiceData);
                    if (response != null && String.IsNullOrWhiteSpace(response.errorCode))
                    {
                        string fullFileName = ProcessPdfFileResult(response.fileToBytes);
                        Inventec.Common.Logging.LogSystem.Debug("_____PDF_FILE_NAME: " + fullFileName);

                        //Thanh cong
                        result.Success = true;
                        result.InvoiceSys = ProviderType.VIETTEL;
                        result.InvoiceLink = fullFileName;

                        if (this.ElectronicBillDataInput.Transaction == null || this.ElectronicBillDataInput.Transaction.EINVOICE_LOGINNAME != viettelLogin.User)
                        {
                            updateloginname = invoiceData.exchangeUser;
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.InvoiceSys = ProviderType.VIETTEL;
                        Inventec.Common.Logging.LogSystem.Error("lay file chuyen doi that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceData), invoiceData));
                        ElectronicBillResultUtil.Set(ref result, false, response.description);
                    }
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.VIETTEL;
                    Inventec.Common.Logging.LogSystem.Error("lay file chuyen doi that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceData), invoiceData));
                    ElectronicBillResultUtil.Set(ref result, false, response.description);
                }

                //update với V2
                if (result.Success = true && !String.IsNullOrWhiteSpace(updateloginname) && viettelLogin.Version == Inventec.Common.ElectronicBillViettel.Version.v2)
                {
                    HisTransactionInvoiceUrlSDO sdo = new HisTransactionInvoiceUrlSDO();
                    sdo.InvoiceCode = this.ElectronicBillDataInput.InvoiceCode;
                    sdo.Loginname = updateloginname;

                    CreatThreadUpdateDataLoginname(sdo);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetExchangeUser(string cashierLoginname)
        {
            string result = "";
            try
            {
                CommonParam param = new CommonParam();
                SdaConfigAppUserViewFilter filter = new SdaConfigAppUserViewFilter();
                filter.LOGINNAME_EXACT = cashierLoginname;
                if (this.ElectronicBillDataInput.EinvoiceTypeId == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VIETEL)
                {
                    filter.KEY__EXACT = SdaConfigKey.ACCOUNT_CONFIG_KEY__VIETEL;
                }
                else
                {
                    filter.KEY__EXACT = SdaConfigKey.ACCOUNT_CONFIG_KEY;
                }

                List<V_SDA_CONFIG_APP_USER> apiResult = new BackendAdapter(param).Get<List<V_SDA_CONFIG_APP_USER>>("/api/SdaConfigAppUser/GetView", ApiConsumer.ApiConsumers.SdaConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    foreach (var item in apiResult)
                    {
                        string value = item.VALUE ?? item.DEFAULT_VALUE;
                        if ((item.KEY == SdaConfigKey.ACCOUNT_CONFIG_KEY__VIETEL || item.KEY == SdaConfigKey.ACCOUNT_CONFIG_KEY) && !String.IsNullOrWhiteSpace(value))
                        {
                            string[] accountConfigArr = value.Split('|');
                            if (accountConfigArr.Length >= 2)
                            {
                                result = accountConfigArr[0].Trim();
                                break;
                            }
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

        private void ProcessGetInvoiceNormal(Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin, ref ElectronicBillResult result)
        {
            try
            {
                GetInvoiceRepresentationFileData invoiceData = new GetInvoiceRepresentationFileData();
                invoiceData.fileType = "PDF";
                invoiceData.supplierTaxCode = this.ElectronicBillDataInput.Branch.TAX_CODE;
                invoiceData.templateCode = this.ElectronicBillDataInput.TemplateCode;
                if (!String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.ENumOrder) && this.ElectronicBillDataInput.ENumOrder.Length >= 7)
                {
                    invoiceData.invoiceNo = this.ElectronicBillDataInput.ENumOrder;
                }
                else
                {
                    invoiceData.invoiceNo = this.ElectronicBillDataInput.SymbolCode + GetVirNumOrder(long.Parse(this.ElectronicBillDataInput.InvoiceCode.Split('|').Last()));
                }

                Response response = null;

                var eViettel = new Inventec.Common.ElectronicBillViettel.ElectronicBillViettelManager(viettelLogin);
                if (eViettel != null)
                {
                    response = eViettel.Run(invoiceData);
                }

                //dữ liệu pdf nhiều nên không log
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response));

                if (response != null && String.IsNullOrWhiteSpace(response.errorCode))
                {
                    string fullFileName = ProcessPdfFileResult(response.fileToBytes);
                    Inventec.Common.Logging.LogSystem.Debug("_____PDF_FILE_NAME: " + fullFileName);

                    //Thanh cong
                    result.Success = true;
                    result.InvoiceSys = ProviderType.VIETTEL;
                    result.InvoiceLink = fullFileName;
                }
                else
                {
                    result.Success = false;
                    result.InvoiceSys = ProviderType.VIETTEL;
                    Inventec.Common.Logging.LogSystem.Error("lay file that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceData), invoiceData));
                    ElectronicBillResultUtil.Set(ref result, false, response.description);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreatThreadUpdateDataLoginname(HisTransactionInvoiceUrlSDO sdo)
        {
            Thread saveFile = new Thread(ProcessUpdateDataLoginname);

            try
            {
                saveFile.Start(sdo);
            }
            catch (Exception ex)
            {
                saveFile.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUpdateDataLoginname(object obj)
        {
            try
            {
                if (obj != null)
                {
                    CommonParam param = new CommonParam();
                    var apiResult = new BackendAdapter(param).Post<bool>("/api/HisTransaction/UpdateEInvoiceUrl", ApiConsumer.ApiConsumers.MosConsumer, obj, param);
                    if (!apiResult)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => obj), obj));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreatThreadSaveData(string p, Response response)
        {
            Thread saveFile = new Thread(ProcessSaveFile);

            List<object> data = new List<object>();
            data.Add(p);
            data.Add(response);
            try
            {
                saveFile.Start(data);
            }
            catch (Exception ex)
            {
                saveFile.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSaveFile(object obj)
        {
            if (obj != null)
            {
                bool saveFileBackup = false;
                List<object> data = obj as List<object>;
                string invoiceCode = "";
                Response response = null;
                foreach (var item in data)
                {
                    if (item.GetType() == typeof(string))
                    {
                        invoiceCode = (string)item;
                    }
                    else if (item.GetType() == typeof(Response))
                    {
                        response = (Response)item;
                    }
                }

                try
                {
                    MemoryStream memo = new MemoryStream(response.fileToBytes);
                    Inventec.Fss.Utility.FileUploadInfo upload = Inventec.Fss.Client.FileUpload.UploadFile(HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_CODE, "INVOICE", memo, response.fileName, true);
                    if (upload != null)
                    {
                        CommonParam param = new CommonParam();
                        HisTransactionInvoiceUrlSDO sdo = new HisTransactionInvoiceUrlSDO();
                        sdo.InvoiceCode = invoiceCode;
                        sdo.Url = upload.Url;
                        var apiResult = new BackendAdapter(param).Post<bool>("/api/HisTransaction/UpdateEInvoiceUrl", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (!apiResult)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
                    }
                    else
                    {
                        saveFileBackup = true;
                    }
                }
                catch (Exception ex)
                {
                    saveFileBackup = true;
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                if (saveFileBackup)
                {
                    Inventec.Common.Logging.LogSystem.Error("Đẩy file lên fss thất bại. Lưu lại file trên máy người dùng");
                    string saveFile = ProcessPdfFileResult(response.fileToBytes);
                    Inventec.Common.Logging.LogSystem.Error(saveFile);
                }
            }
        }
        #endregion

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

        private void ProcessCreateInvoice(Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin, ref ElectronicBillResult result)
        {
            try
            {
                DataCreateInvoice invoiceData = this.CreateInvoice(this.ElectronicBillDataInput, viettelLogin);
                Response response = null;

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceData), invoiceData));
                var eViettel = new Inventec.Common.ElectronicBillViettel.ElectronicBillViettelManager(viettelLogin);
                if (eViettel != null)
                {
                    response = eViettel.Run(invoiceData);
                }

                if (response != null && String.IsNullOrWhiteSpace(response.errorCode) && response.result != null)
                {
                    //v2 có thể không trả vể invoiceNo nên cần get lại theo thông tin transactionUuid
                    if (viettelLogin != null && viettelLogin.Version == Inventec.Common.ElectronicBillViettel.Version.v2)
                    {
                        Thread.Sleep(500);
                        Inventec.Common.Logging.LogSystem.Debug("Gọi lại api lấy thông tin hóa đơn sau 0.5s");

                        GetFile getData = new GetFile();
                        getData.supplierTaxCode = this.ElectronicBillDataInput.Branch.TAX_CODE;
                        getData.transactionUuid = invoiceData.generalInvoiceInfo.transactionUuid;

                        Response responseGetData = null;
                        if (eViettel != null)
                        {
                            responseGetData = eViettel.Run(getData);
                        }

                        if (responseGetData != null && String.IsNullOrWhiteSpace(responseGetData.errorCode) && responseGetData.result != null)
                        {
                            long num = GetNumOrder(responseGetData.result.invoiceNo);
                            //Thanh cong
                            result.Success = true;
                            result.InvoiceSys = ProviderType.VIETTEL;
                            result.InvoiceCode = string.Format("{0}|{1}", responseGetData.result.reservationCode ?? response.result.reservationCode, num);
                            result.InvoiceNumOrder = responseGetData.result.invoiceNo;
                            result.InvoiceLoginname = viettelLogin.User;
                            if (responseGetData.result.issueDate > 0)
                            {
                                result.InvoiceTime = GetTimefromMilliseconds(responseGetData.result.issueDate);
                            }
                            else
                            {
                                result.InvoiceTime = GetInvoiceTimeByResult(eViettel, viettelLogin, responseGetData.result, invoiceData);
                            }
                        }
                        else
                        {
                            long num = GetNumOrder(response.result.invoiceNo);
                            //Thanh cong
                            result.Success = true;
                            result.InvoiceSys = ProviderType.VIETTEL;
                            result.InvoiceCode = string.Format("{0}|{1}", response.result.reservationCode ?? "", num);
                            result.InvoiceNumOrder = response.result.invoiceNo;
                            result.InvoiceTime = GetInvoiceTimeByResult(eViettel, viettelLogin, response.result, invoiceData);
                            result.InvoiceLoginname = viettelLogin.User;
                        }
                    }
                    else
                    {
                        long num = GetNumOrder(response.result.invoiceNo);
                        //Thanh cong
                        result.Success = true;
                        result.InvoiceSys = ProviderType.VIETTEL;
                        result.InvoiceCode = string.Format("{0}|{1}", response.result.reservationCode ?? "", num);
                        result.InvoiceNumOrder = response.result.invoiceNo;
                        result.InvoiceTime = GetInvoiceTimeByResult(eViettel, viettelLogin, response.result, invoiceData);
                        result.InvoiceLoginname = viettelLogin.User;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Tao hoa don dien tu that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoiceData), invoiceData));
                    ElectronicBillResultUtil.Set(ref result, false, response.description);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long GetInvoiceTimeByResult(Inventec.Common.ElectronicBillViettel.ElectronicBillViettelManager eViettel, Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin, ResponseResult responseResult, DataCreateInvoice invoiceData)
        {
            long result = Inventec.Common.DateTime.Get.Now() ?? 0;
            try
            {
                if (eViettel != null && viettelLogin != null && viettelLogin.Version == Inventec.Common.ElectronicBillViettel.Version.v2)
                {
                    //Thời gian phát hành hóa đơn được hướng dẫn truyền vào invoiceIssuedDate khi phát hành
                    //Convert ngược lại thời gian phát hành gửi sang HDDT.
                    if (invoiceData != null && invoiceData.generalInvoiceInfo != null && !String.IsNullOrWhiteSpace(invoiceData.generalInvoiceInfo.invoiceIssuedDate))
                    {
                        //cần lấy thời gian theo múi giờ
                        result = GetTimefromMilliseconds(double.Parse(invoiceData.generalInvoiceInfo.invoiceIssuedDate));
                    }
                    else
                    {
                        GetInvoiceInfoFilter filter = new GetInvoiceInfoFilter();
                        filter.invoiceNo = responseResult.invoiceNo;
                        filter.pageNum = 1;
                        filter.rowPerPage = 10;
                        filter.endDate = DateTime.Now;
                        filter.startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        var response = eViettel.Run(filter);
                        if (response != null && response.invoices != null && response.invoices.Count > 0)
                        {
                            if (response.invoices.First().issueDate.HasValue)
                            {
                                result = GetTimefromMilliseconds(response.invoices.First().issueDate ?? 0);
                            }
                            else if (!String.IsNullOrWhiteSpace(response.invoices.First().issueDateStr))
                            {
                                DateTime time = DateTime.Parse(response.invoices.First().issueDateStr);
                                result = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(time) ?? 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = Inventec.Common.DateTime.Get.Now() ?? 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private long GetNumOrder(string p)
        {
            long result = 0;
            try
            {
                if (!String.IsNullOrWhiteSpace(p))
                {
                    string numOrder = "";
                    for (int i = p.Length - 1; i > 0; i--)
                    {
                        if (char.IsDigit(p[i]))
                            numOrder = p[i] + numOrder;
                        else
                            break;
                    }

                    result = long.Parse(numOrder);
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #region CreateInvoice
        private DataCreateInvoice CreateInvoice(Base.ElectronicBillDataInput electronicBillDataInput, Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin)
        {
            DataCreateInvoice result = new DataCreateInvoice();
            try
            {
                if (electronicBillDataInput != null)
                {
                    result.buyerInfo = GetBuyerInfo(electronicBillDataInput);
                    result.generalInvoiceInfo = GetGeneralInvoiceInfo(viettelLogin);//tạo Transaction chưa có ID
                    result.payments = GetPayments(electronicBillDataInput);
                    result.sellerInfo = GetSellerInfo(electronicBillDataInput);
                    result.itemInfo = GetItemInfo();
                    result.summarizeInfo = GetSummarizeInfo(result.itemInfo);
                    result.taxBreakdowns = new List<TaxBreakdowns>();
                    if (HisConfigCFG.Viettel_TaxBreakdownOption == HisConfigCFG.TaxBreakdownOption.CoThongTinThue)
                    {
                        result.taxBreakdowns = GetTaxBreakdowns(result.itemInfo);
                    }
                    result.metadata = GetMetadata(result.summarizeInfo);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<Metadata> GetMetadata(SummarizeInfo summarizeInfo)
        {
            List<Metadata> result = new List<Metadata>();
            try
            {
                if (this.ElectronicBillDataInput != null)
                {
                    string keyTagS = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.HisConfigCFG.His_Desktop_plugins_ElectriconicBill_Viettel_Metadata);
                    if (String.IsNullOrWhiteSpace(keyTagS))//viện mới không cấu hình thì không thêm để không bị lỗi khi tạo hóa đơn
                    {
                        return result;
                    }

                    Dictionary<string, string> dicXmlValues = new Dictionary<string, string>();

                    List<string> lstKeyTag = keyTagS.Split('|').ToList();

                    if (lstKeyTag.Count > 5)
                    {
                        dicXmlValues = General.ProcessDicValueString(this.ElectronicBillDataInput);
                    }

                    string sotien0 = "sotien0";//"Tổng chi phí: "
                    string lable0 = "";//"Tổng chi phí: "
                    string sotien1 = "sotien1";//"Qũy BHYT thanh toán: "
                    string lable1 = "";//"Qũy BHYT thanh toán: "
                    string sotien2 = "sotien2";//"Số tiền BN cùng chi trả BHYT: "
                    string lable2 = "";//"Số tiền BN cùng chi trả BHYT: "
                    string sotien3 = "sotien3";//"Số tiền các quỹ tài trợ, khác: "
                    string lable3 = "";//"Số tiền các quỹ tài trợ, khác: "
                    string sotien4 = "sotien4";//"Số tiền BN thanh toán: "
                    string lable4 = "";//"Số tiền BN thanh toán: "
                    for (int i = 0; i < lstKeyTag.Count; i++)
                    {
                        if (i == 0)
                        {
                            sotien0 = lstKeyTag[0];
                            string[] split = lstKeyTag[i].Split('#');
                            if (split.Length > 2)
                            {
                                sotien0 = split[0];
                                lable0 = split[1];
                            }
                        }
                        else if (i == 1)
                        {
                            sotien1 = lstKeyTag[1];
                            string[] split = lstKeyTag[i].Split('#');
                            if (split.Length > 2)
                            {
                                sotien1 = split[0];
                                lable1 = split[1];
                            }
                        }
                        else if (i == 2)
                        {
                            sotien2 = lstKeyTag[2];
                            string[] split = lstKeyTag[i].Split('#');
                            if (split.Length > 2)
                            {
                                sotien2 = split[0];
                                lable2 = split[1];
                            }
                        }
                        else if (i == 3)
                        {
                            sotien3 = lstKeyTag[3];
                            string[] split = lstKeyTag[i].Split('#');
                            if (split.Length > 2)
                            {
                                sotien3 = split[0];
                                lable3 = split[1];
                            }
                        }
                        else if (i == 4)
                        {
                            sotien4 = lstKeyTag[4];
                            string[] split = lstKeyTag[i].Split('#');
                            if (split.Length > 2)
                            {
                                sotien4 = split[0];
                                lable4 = split[1];
                            }
                        }
                        else
                        {
                            string[] split = lstKeyTag[i].Split('#');
                            if (split.Length > 2)
                            {
                                Metadata metadata = new Metadata();
                                metadata.invoiceCustomFieldId = i + 1;
                                metadata.keyLabel = split[1];
                                metadata.keyTag = split[0];
                                if (dicXmlValues.ContainsKey(split[2]))
                                {
                                    metadata.stringValue = dicXmlValues[split[2]];
                                }
                                else
                                {
                                    metadata.stringValue = split[2];
                                }

                                metadata.valueType = "text";
                                result.Add(metadata);
                            }
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(sotien0))
                    {
                        Metadata totalPrice = new Metadata();
                        totalPrice.invoiceCustomFieldId = 1;
                        totalPrice.keyLabel = !string.IsNullOrWhiteSpace(lable0) ? lable0 : "Tổng chi phí";
                        totalPrice.keyTag = sotien0;
                        totalPrice.numberValue = (long)Math.Round(ElectronicBillDataInput.Treatment.TOTAL_PRICE ?? 0, 0);
                        totalPrice.valueType = "number";
                        result.Add(totalPrice);
                    }

                    if (!String.IsNullOrWhiteSpace(sotien1))
                    {
                        Metadata totalPriceBhyt = new Metadata();
                        totalPriceBhyt.invoiceCustomFieldId = 2;
                        totalPriceBhyt.keyLabel = !string.IsNullOrWhiteSpace(lable1) ? lable1 : "Số tiền BHYT";
                        totalPriceBhyt.keyTag = sotien1;
                        totalPriceBhyt.numberValue = (long)Math.Round(ElectronicBillDataInput.Treatment.TOTAL_HEIN_PRICE ?? 0, 0);
                        totalPriceBhyt.valueType = "number";
                        result.Add(totalPriceBhyt);
                    }

                    if (!String.IsNullOrWhiteSpace(sotien2))
                    {
                        Metadata patientPriceBhyt = new Metadata();
                        patientPriceBhyt.invoiceCustomFieldId = 3;
                        patientPriceBhyt.keyLabel = !string.IsNullOrWhiteSpace(lable2) ? lable2 : "Trong đó Số tiền BN cùng chi trả BHYT";
                        patientPriceBhyt.keyTag = sotien2;
                        var totalPatientPriceBhyt = Math.Round(ElectronicBillDataInput.Treatment.TOTAL_PATIENT_PRICE_BHYT ?? 0, 0);
                        patientPriceBhyt.numberValue = (long)totalPatientPriceBhyt;
                        patientPriceBhyt.valueType = "number";
                        result.Add(patientPriceBhyt);
                    }

                    if (!String.IsNullOrWhiteSpace(sotien3))
                    {
                        Metadata otherPrice = new Metadata();
                        otherPrice.invoiceCustomFieldId = 4;
                        otherPrice.keyLabel = !string.IsNullOrWhiteSpace(lable3) ? lable3 : "Số tiền các quỹ tài trợ khác";
                        otherPrice.keyTag = sotien3;
                        decimal billFund = 0;

                        //tại thời điểm tạo hóa đơn tại chức năng viện phí thì Treatment.TOTAL_BILL_FUND chưa có Transaction.TDL_BILL_FUND_AMOUNT
                        //tạo hóa đơn tại danh sách giao dịch thời gian tạo giao dịch sẽ nhỏ hơn thời gian hiện tại. khi đó thông tin quỹ đã được cập nhật vào hồ sơ nên sẽ không cộng
                        //tạo hóa đơn đt sau khi tạo giao dịch trên his thành công thì thông tin HIS_BILL_FUND không còn nữa thay vào đó là TDL_BILL_FUND_AMOUNT
                        if (!ElectronicBillDataInput.IsTransactionList)//ko phải xuất hóa đơn từ danh sách giao dịch
                        {
                            if (ElectronicBillDataInput.Transaction != null && ElectronicBillDataInput.Transaction.TDL_BILL_FUND_AMOUNT.HasValue)
                            {
                                billFund = ElectronicBillDataInput.Transaction.TDL_BILL_FUND_AMOUNT.Value;
                            }
                            else if (ElectronicBillDataInput.Transaction != null && ElectronicBillDataInput.Transaction.HIS_BILL_FUND != null && ElectronicBillDataInput.Transaction.HIS_BILL_FUND.Count > 0)
                            {
                                billFund = ElectronicBillDataInput.Transaction.HIS_BILL_FUND.Sum(s => s.AMOUNT);
                            }
                        }

                        otherPrice.numberValue = (long)Math.Round((ElectronicBillDataInput.Treatment.TOTAL_BILL_FUND ?? 0) + billFund, 0);
                        otherPrice.valueType = "number";
                        result.Add(otherPrice);
                    }

                    if (!String.IsNullOrWhiteSpace(sotien4))
                    {
                        Metadata patientPrice = new Metadata();
                        patientPrice.invoiceCustomFieldId = 5;
                        patientPrice.keyLabel = !string.IsNullOrWhiteSpace(lable4) ? lable4 : "Số tiền BN thanh toán";
                        patientPrice.keyTag = sotien4;
                        patientPrice.numberValue = (long)Math.Round(summarizeInfo.totalAmountWithTax, 0);
                        patientPrice.valueType = "number";
                        result.Add(patientPrice);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<Metadata>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// thời gian tạo giao dịch nhỏ hơn không quá 10s hoặc bằng thời gian hiện tại sẽ là tạo hóa đơn điện tử tại chức năng viện phí, xuất bán, thanh toán khác, thanh toán 2 sổ ...
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool CheckTransactionCreateTime(HIS_TRANSACTION transaction)
        {
            bool result = false;
            try
            {
                if (transaction != null && transaction.CREATE_TIME.HasValue && (Inventec.Common.DateTime.Get.Now() ?? 0) - transaction.CREATE_TIME.Value <= 10)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<TaxBreakdowns> GetTaxBreakdowns(List<ItemInfo> list)
        {
            List<TaxBreakdowns> result = new List<TaxBreakdowns>();
            try
            {
                if (list != null && list.Count > 0)
                {
                    var groupTax = list.GroupBy(o => o.taxPercentage).ToList();
                    foreach (var item in groupTax)
                    {
                        TaxBreakdowns tax = new TaxBreakdowns();
                        tax.taxPercentage = item.First().taxPercentage;
                        tax.taxAmount = item.Sum(s => s.taxAmount ?? 0);
                        tax.taxableAmount = item.Sum(s => s.itemTotalAmountWithoutTax ?? 0);
                        result.Add(tax);
                    }

                    if (this.TempType == TemplateEnum.TYPE.Template4)
                    {
                        result.First().taxPercentage = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<TaxBreakdowns>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private SummarizeInfo GetSummarizeInfo(List<ItemInfo> listItem)
        {
            SummarizeInfo result = null;
            try
            {
                if (listItem != null)
                {
                    result = new SummarizeInfo();
                    decimal amount = 0;
                    decimal amountWithoutTax = 0;

                    if (this.TempType == TemplateEnum.TYPE.Template10)
                    {
                        var sumData = listItem.Where(o => o.itemCode.Equals("TONG")).ToList();
                        if (sumData != null && sumData.Count > 0)
                        {
                            amount = Math.Round(sumData.Sum(s => s.itemTotalAmountWithTax ?? 0), 0);
                            amountWithoutTax = Math.Round(sumData.Sum(s => s.itemTotalAmountWithoutTax ?? 0), 0);
                        }
                    }
                    else
                    {
                        amount = Math.Round(listItem.Where(s => s.selection == 1).Sum(s => s.itemTotalAmountWithTax ?? 0), 0);
                        amountWithoutTax = Math.Round(listItem.Where(s => s.selection == 1).Sum(s => s.itemTotalAmountWithoutTax ?? 0), 0);
                    }

                    result.discountAmount = Math.Round(listItem.Sum(s => s.discount ?? 0) + (this.ElectronicBillDataInput.Transaction != null ? this.ElectronicBillDataInput.Transaction.EXEMPTION ?? 0 : 0), 0);
                    result.sumOfTotalLineAmountWithoutTax = amountWithoutTax;
                    result.totalAmountWithoutTax = amountWithoutTax;
                    result.totalAmountWithTax = amount;
                    result.totalAmountWithTaxInWords = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amount))) + "đồng";
                    if (HisConfigCFG.Viettel_TaxBreakdownOption == HisConfigCFG.TaxBreakdownOption.KhongHienThiThongTinThue)
                    {
                        result.totalTaxAmount = null;
                    }
                    else
                    {
                        result.totalTaxAmount = Math.Round(listItem.Sum(s => s.taxAmount ?? 0), 0);
                    }
                    result.totalAmountAfterDiscount = Math.Round(amountWithoutTax - result.discountAmount, 0);
                    //result.settlementDiscountAmount = 0;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private SellerInfo GetSellerInfo(Base.ElectronicBillDataInput electronicBillDataInput)
        {
            SellerInfo result = null;
            try
            {
                if (electronicBillDataInput != null)
                {
                    //Thông tin người bán trên hóa đơn, có thể được truyền sang hoặc lấy tự động trên hệ thống hóa đơn điện tử.
                    //Trong trường hợp sellerTaxCode không được truyền sang thì dữ liệu sẽ được lấy từ hệ thống hóa đơn điện tử
                    result = new SellerInfo();
                    result.sellerAddressLine = electronicBillDataInput.Branch.ADDRESS;
                    result.sellerBankAccount = electronicBillDataInput.Branch.ACCOUNT_NUMBER;
                    result.sellerBankName = null;
                    result.sellerCityName = null;
                    result.sellerCode = electronicBillDataInput.Branch.HEIN_MEDI_ORG_CODE;
                    result.sellerCountryCode = null;
                    result.sellerDistrictName = null;
                    result.sellerEmail = null;
                    result.sellerFaxNumber = null;
                    result.sellerLegalName = electronicBillDataInput.Branch.BRANCH_NAME;
                    result.sellerPhoneNumber = electronicBillDataInput.Branch.PHONE;
                    //result.sellerTaxCode = electronicBillDataInput.Branch.TAX_CODE;
                    result.sellerWebsite = null;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<Payments> GetPayments(Base.ElectronicBillDataInput electronicBillDataInput)
        {
            List<Payments> result = null;
            try
            {
                result = new List<Payments>();

                string paymentName = electronicBillDataInput.PaymentMethod;

                if (electronicBillDataInput.Transaction != null)
                {
                    HIS_PAY_FORM payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == electronicBillDataInput.Transaction.PAY_FORM_ID);
                    if (payForm != null)
                    {
                        paymentName = payForm.ELECTRONIC_PAY_FORM_NAME ?? payForm.PAY_FORM_NAME;
                    }
                }

                Payments p = new Payments() { paymentMethodName = paymentName };
                result.Add(p);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<ItemInfo> GetItemInfo()
        {
            List<ItemInfo> result = new List<ItemInfo>();

            //qua hàm này thì hóa đơn bán hàng cũng có thông tin sere_serv_bill
            TemplateFactory.ProcessDataSereServToSereServBill(this.TempType, ref this.ElectronicBillDataInput);

            int i = 1;

            Base.ElectronicBillDataInput dataInput = new Base.ElectronicBillDataInput(ElectronicBillDataInput);
            Base.ElectronicBillDataInput dataInputWithVat = null;

            //tách dịch vụ có VAT không áp dụng cho template nhà thuốc và template 199
            if (HisConfigCFG.IsSplitServicesWithVat
                && this.ElectronicBillDataInput.SereServBill.Exists(o => o.TDL_VAT_RATIO.HasValue && o.TDL_VAT_RATIO > 0)
                && this.TempType != TemplateEnum.TYPE.Template10
                && this.TempType != TemplateEnum.TYPE.TemplateNhaThuoc)
            {
                //danh sách dịch vụ gom bình thường chỉ gán sere_serv_bill
                dataInput.SereServBill = this.ElectronicBillDataInput.SereServBill.Where(o => !o.TDL_VAT_RATIO.HasValue || o.TDL_VAT_RATIO <= 0).ToList();

                //danh sách dịch vụ có VAT chỉ gán sere_serv
                dataInputWithVat = new ElectronicBillDataInput(ElectronicBillDataInput);
                dataInputWithVat.SereServs = TemplateFactory.GetSereServWithVAT(this.ElectronicBillDataInput);
            }
            else
            {
                dataInput.SereServs = this.ElectronicBillDataInput.SereServs;
                dataInput.SereServBill = this.ElectronicBillDataInput.SereServBill;
            }

            IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, dataInput);

            var listProduct = (dataInput.SereServs != null && dataInput.SereServs.Count > 0) || (dataInput.SereServBill != null && dataInput.SereServBill.Count > 0) ? iRunTemplate.Run() : null;

            var listProductVAT = dataInputWithVat != null ? TemplateFactory.MakeIRun(TemplateEnum.TYPE.TemplateNhaThuoc, dataInputWithVat).Run() : null;

            if (listProduct == null && listProductVAT == null)
            {
                throw new Exception("Không có thông tin chi tiết dịch vụ.");
            }

            if (listProduct != null)
            {
                if (listProduct.GetType() == typeof(List<ProductBase>))
                {
                    List<ProductBase> listProductBase = (List<ProductBase>)listProduct;

                    if (listProductBase == null || listProductBase.Count == 0)
                    {
                        throw new Exception("Không có thông tin chi tiết dịch vụ.");
                    }

                    result.AddRange(GetListProduct(listProductBase, ref i));
                }
                else if (listProduct.GetType() == typeof(List<ProductBasePlus>))
                {
                    List<ProductBasePlus> listProductBasePlus = (List<ProductBasePlus>)listProduct;

                    if (listProductBasePlus == null || listProductBasePlus.Count == 0)
                    {
                        throw new Exception("Không có thông tin chi tiết dịch vụ.");
                    }

                    result.AddRange(GetVatProduct(listProductBasePlus, ref i));
                }
                else
                {
                    throw new Exception("Không có thông tin chi tiết dịch vụ.");
                }
            }

            if (listProductVAT != null)
            {
                List<ProductBasePlus> listProductBasePlus = (List<ProductBasePlus>)listProductVAT;

                if (listProductBasePlus == null || listProductBasePlus.Count == 0)
                {
                    throw new Exception("Không có thông tin chi tiết dịch vụ.");
                }

                result.AddRange(GetVatProduct(listProductBasePlus, ref i));
            }

            return result;
        }

        private List<ItemInfo> GetListProduct(List<ProductBase> listProduct, ref int i)
        {
            List<ItemInfo> result = new List<ItemInfo>();

            foreach (var item in listProduct)
            {
                ItemInfo product = new ItemInfo();
                product.lineNumber = i;

                product.itemCode = item.ProdCode;
                product.itemName = item.ProdName;
                product.quantity = item.ProdQuantity;
                product.unitName = item.ProdUnit;
                product.discount = null;
                product.itemDiscount = null;
                product.selection = 1;
                if (this.TempType == TemplateEnum.TYPE.Template10 && i > 7)
                {
                    product.selection = 2;
                }

                product.itemTotalAmountWithTax = item.Amount;
                product.itemTotalAmountWithoutTax = item.Amount;
                product.unitPrice = item.ProdPrice;
                if (HisConfigCFG.Viettel_TaxBreakdownOption == HisConfigCFG.TaxBreakdownOption.KhongHienThiThongTinThue)
                {
                    product.taxPercentage = null;
                    product.taxAmount = null;
                }
                else
                {
                    product.taxPercentage = -2;
                    product.taxAmount = 0;
                }
                result.Add(product);
                i++;
            }
            if (this.ElectronicBillDataInput.Transaction != null && this.ElectronicBillDataInput.Transaction.EXEMPTION != null && this.ElectronicBillDataInput.Transaction.EXEMPTION > 0 && this.TempType != TemplateEnum.TYPE.Template10)
            {
                ItemInfo product = new ItemInfo();
                product.lineNumber = i;
                product.selection = 3; //Chiết khấu
                product.itemCode = "Chiet_Khau";
                product.itemName = "Miễn giảm";
                product.itemTotalAmountWithoutTax = this.ElectronicBillDataInput.Transaction.EXEMPTION;
                product.itemTotalAmountAfterDiscount = this.ElectronicBillDataInput.Transaction.EXEMPTION;
                product.itemTotalAmountWithTax = this.ElectronicBillDataInput.Transaction.EXEMPTION;
                result.Add(product);
                i++;
            }
            if ((this.ElectronicBillDataInput.Transaction == null || this.ElectronicBillDataInput.Transaction.EXEMPTION == null) && this.TempType == TemplateEnum.TYPE.Template3)
            {
                decimal billFund = 0;
                if (!ElectronicBillDataInput.IsTransactionList)//ko phải xuất hóa đơn từ danh sách giao dịch
                {
                    if (ElectronicBillDataInput.Transaction != null && ElectronicBillDataInput.Transaction.TDL_BILL_FUND_AMOUNT.HasValue)
                    {
                        billFund = ElectronicBillDataInput.Transaction.TDL_BILL_FUND_AMOUNT.Value;
                    }
                    else if (ElectronicBillDataInput.Transaction != null && ElectronicBillDataInput.Transaction.HIS_BILL_FUND != null && ElectronicBillDataInput.Transaction.HIS_BILL_FUND.Count > 0)
                    {
                        billFund = ElectronicBillDataInput.Transaction.HIS_BILL_FUND.Sum(s => s.AMOUNT);
                    }
                }

                billFund += (ElectronicBillDataInput.Treatment.TOTAL_BILL_FUND ?? 0);
                result.First().itemTotalAmountWithoutTax -= billFund;
                result.First().itemTotalAmountWithTax -= billFund;
                result.First().unitPrice -= billFund;
            }

            return result;
        }

        private List<ItemInfo> GetVatProduct(List<ProductBasePlus> listProduct, ref int i)
        {
            List<ItemInfo> result = new List<ItemInfo>();

            foreach (var item in listProduct)
            {
                ItemInfo product = new ItemInfo();
                product.lineNumber = i;
                product.itemName = item.ProdName;
                product.unitPrice = item.ProdPrice;
                product.quantity = item.ProdQuantity;
                product.unitName = item.ProdUnit;

                if (HisConfigCFG.Viettel_TaxBreakdownOption == HisConfigCFG.TaxBreakdownOption.KhongHienThiThongTinThue)
                {
                    product.taxPercentage = null;
                    product.itemTotalAmountWithTax = item.Amount;
                    product.itemTotalAmountWithoutTax = item.Amount;
                    product.taxAmount = null;
                }
                else
                {
                    if (item.TaxPercentage.HasValue)
                    {
                        product.taxPercentage = (long)Math.Round(item.TaxConvert, 0);
                    }
                    else
                    {
                        //không thuế
                        product.taxPercentage = -2;
                    }
                    product.itemTotalAmountWithTax = item.Amount;
                    product.itemTotalAmountWithoutTax = item.AmountWithoutTax;
                    product.taxAmount = item.TaxAmount;
                }

                product.discount = null;
                product.itemDiscount = null;
                product.selection = 1;
                result.Add(product);
                i++;
            }

            if (this.ElectronicBillDataInput.Transaction != null && this.ElectronicBillDataInput.Transaction.EXEMPTION != null && this.ElectronicBillDataInput.Transaction.EXEMPTION > 0 && this.TempType != TemplateEnum.TYPE.Template10)
            {
                ItemInfo product = new ItemInfo();
                product.lineNumber = i;
                product.selection = 3; //Chiết khấu
                product.itemCode = "Chiet_Khau";
                product.itemName = "Miễn giảm";
                product.itemTotalAmountWithoutTax = this.ElectronicBillDataInput.Transaction.EXEMPTION;
                product.itemTotalAmountAfterDiscount = this.ElectronicBillDataInput.Transaction.EXEMPTION;
                product.itemTotalAmountWithTax = this.ElectronicBillDataInput.Transaction.EXEMPTION;
                result.Add(product);
                i++;
            }
            return result;
        }

        //tạo Transaction chưa có ID
        private GeneralInvoiceInfo GetGeneralInvoiceInfo(Inventec.Common.ElectronicBillViettel.DataInitApi viettelLogin)
        {
            GeneralInvoiceInfo result = null;
            try
            {
                if (ElectronicBillDataInput != null)
                {
                    result = new GeneralInvoiceInfo();

                    result.additionalReferenceDate = null;
                    result.additionalReferenceDesc = "";
                    result.adjustmentInvoiceType = "";
                    result.adjustmentType = "1";
                    result.certificateSerial = "";//usb token
                    result.currencyCode = ElectronicBillDataInput.Currency;
                    result.cusGetInvoiceRight = true;
                    //result.invoiceNo = "";///numorder chưa có
                    //result.originalInvoiceId = "";
                    //result.originalInvoiceIssueDate = "";
                    result.paymentStatus = true;//đã thanh toán hay chưa
                    result.transactionUuid = "";//transaction.ID chưa có
                    if (ElectronicBillDataInput.Transaction != null && !String.IsNullOrEmpty(ElectronicBillDataInput.Transaction.TRANSACTION_CODE))
                    {
                        result.transactionUuid = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                    }
                    else if (ElectronicBillDataInput.ListTransaction != null && ElectronicBillDataInput.ListTransaction.Count > 0)
                    {
                        result.transactionUuid = ElectronicBillDataInput.ListTransaction.Select(s => s.TRANSACTION_CODE).OrderBy(o => o).FirstOrDefault();
                    }

                    if (ElectronicBillDataInput.Transaction != null)
                    {
                        result.userName = ElectronicBillDataInput.Transaction.CASHIER_USERNAME;
                    }
                    else if (ElectronicBillDataInput.ListTransaction != null && ElectronicBillDataInput.ListTransaction.Count > 0)
                    {
                        result.userName = string.Join(", ", ElectronicBillDataInput.ListTransaction.Select(s => s.CASHIER_USERNAME).Distinct());
                    }

                    result.templateCode = ElectronicBillDataInput.TemplateCode;
                    result.invoiceSeries = ElectronicBillDataInput.SymbolCode;
                    result.invoiceType = GetInvoiceType(ElectronicBillDataInput.TemplateCode);//Mã loại hóa đơn chỉ nhận các giá trị sau: 01GTKT, 02GTTT, 07KPTQ, 03XKNB, 04HGDL, 01BLP

                    string paymentName = ElectronicBillDataInput.PaymentMethod;

                    if (ElectronicBillDataInput.Transaction != null)
                    {
                        HIS_PAY_FORM payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == ElectronicBillDataInput.Transaction.PAY_FORM_ID);
                        if (payForm != null)
                        {
                            paymentName = payForm.ELECTRONIC_PAY_FORM_NAME ?? payForm.PAY_FORM_NAME;
                        }
                    }

                    result.paymentTypeName = paymentName;

                    if (viettelLogin != null && viettelLogin.Version == Inventec.Common.ElectronicBillViettel.Version.v2)
                    {
                        result.invoiceIssuedDate = GetTimeMilliseconds(Inventec.Common.DateTime.Get.Now() ?? 0) + "";
                    }

                    //thay thế hóa đơn
                    if (ElectronicBillDataInput.Transaction != null && ElectronicBillDataInput.Transaction.ORIGINAL_TRANSACTION_ID.HasValue)
                    {
                        result.adjustmentType = "3";
                        result.originalInvoiceId = ElectronicBillDataInput.Transaction.TDL_ORIGINAL_EI_NUM_ORDER;
                        result.originalInvoiceIssueDate = GetTimeMilliseconds(ElectronicBillDataInput.Transaction.TDL_ORIGINAL_EI_TIME ?? 0) + "";
                        result.additionalReferenceDesc = ElectronicBillDataInput.Transaction.REPLACE_REASON;
                        result.additionalReferenceDate = GetTimeMilliseconds(ElectronicBillDataInput.Transaction.REPLACE_TIME ?? 0) + "";
                        result.adjustedNote = ElectronicBillDataInput.Transaction.REPLACE_REASON;
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

        //Mã loại hóa đơn chỉ nhận các giá trị sau: 01GTKT, 02GTTT, 07KPTQ, 03XKNB, 04HGDL, 01BLP
        //"templateCode": "01GTKT0/342"  -> "invoiceType": "01GTKT"
        //"templateCode": "1/001" -> "invoiceType": "1"
        private string GetInvoiceType(string p)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(p))
                {
                    string[] split = p.Split('/');
                    if (split[0].Length > 6)
                    {
                        result = split[0].Substring(0, 6);
                    }
                    else
                    {
                        result = split[0];
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

        private BuyerInfo GetBuyerInfo(Base.ElectronicBillDataInput electronicBillDataInput)
        {
            BuyerInfo result = new BuyerInfo();
            try
            {
                InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(electronicBillDataInput, this.TempType != TemplateEnum.TYPE.Template10);
                result.buyerLegalName = adoInfo.BuyerOrganization;
                result.buyerTaxCode = adoInfo.BuyerTaxCode;
                result.buyerBankAccount = adoInfo.BuyerAccountNumber;
                result.buyerAddressLine = !String.IsNullOrWhiteSpace(adoInfo.BuyerAddress) ? adoInfo.BuyerAddress : ".";
                result.buyerPhoneNumber = adoInfo.BuyerPhone;
                result.buyerName = adoInfo.BuyerName;
                result.buyerBirthDay = adoInfo.BuyerDob;
                result.buyerCode = adoInfo.BuyerCode;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private V_HIS_PATIENT GetPatientbyId(long p)
        {
            V_HIS_PATIENT result = null;
            try
            {
                HisPatientViewFilter filter = new HisPatientViewFilter();
                filter.ID = p;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_PATIENT>>("/api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (apiResult != null)
                {
                    result = apiResult.FirstOrDefault();
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

        private bool Check(ref ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = ServiceConfig.Split('|');
                if (configArr.Length < 2)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");
                if (configArr[0] != ProviderType.VIETTEL)
                    throw new Exception("Không đúng cấu hình nhà cung cấp VIETTEL");

                string[] accountArr = AccountConfig.Split('|');
                if (accountArr.Length < 2)
                    throw new Exception("Sai định dạng cấu hình tai khoản.");

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

        // 7 số
        private string GetVirNumOrder(long numOrder)
        {
            return string.Format("{0:0000000}", numOrder);
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private double GetTimeMilliseconds(long time)
        {
            DateTime yourDateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(time) ?? DateTime.Now;
            return yourDateTime.ToUniversalTime().Subtract(UnixEpoch).TotalMilliseconds;
        }

        private long GetTimefromMilliseconds(double milisecondTime)
        {
            DateTime time = UnixEpoch.AddMilliseconds(milisecondTime);
            TimeZone localZone = TimeZone.CurrentTimeZone;
            time = localZone.ToLocalTime(time);

            return Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(time) ?? 0;
        }
    }
}
