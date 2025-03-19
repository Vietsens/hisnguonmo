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
using Inventec.Common.ElectronicBillViettel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventec.Common.ElectronicBillViettel
{
    //tam thoi de 1 cho sau tach sau
    class ElectronicBillViettelProcessor : IRun
    {
        private DataInitApi DataApi;

        internal ElectronicBillViettelProcessor(DataInitApi DataApi)
        {
            this.DataApi = DataApi;
        }

        Response IRun.Run(object data)
        {
            Response result = new Response();
            try
            {
                if (data.GetType() == typeof(DataCreateInvoice))
                {
                    result = Create((DataCreateInvoice)data);
                }
                else if (data.GetType() == typeof(CancelInvoice))
                {
                    result = Cancel((CancelInvoice)data);
                }
                else if (data.GetType() == typeof(GetFile))
                {
                    result = GetFile((GetFile)data);
                }
                else if (data.GetType() == typeof(GetMetadata))
                {
                    result = GetCustomField((GetMetadata)data);
                }
                else if (data.GetType() == typeof(GetInvoiceInfoFilter))
                {
                    result = GetInvoiceInfo((GetInvoiceInfoFilter)data);
                }
                else if (data.GetType() == typeof(GetInvoiceRepresentationFileData))
                {
                    result = GetInvoiceFile((GetInvoiceRepresentationFileData)data);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
                throw ex;
            }
            return result;
        }

        private Response GetInvoiceFile(GetInvoiceRepresentationFileData invoices)
        {
            Inventec.Common.Logging.LogSystem.Info("GetInvoiceFile");
            Model.Response result = new Model.Response();

            if (this.CheckInvoiceGet(invoices, ref result))
            {
                if (DataApi.Version == Version.v2)
                {
                    Dictionary<string, string> access_token = null;

                    string accessToken = LoginProcess.Login(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass);

                    if (!String.IsNullOrWhiteSpace(accessToken))
                    {
                        access_token = new Dictionary<string, string>();
                        access_token[Base.Constants.keyHeaderToken] = string.Format(Base.Constants.valueHeaderToken, accessToken);
                    }
                    else
                    {
                        result.errorCode = Base.Constants.ErrorCode;
                        result.description = "Tài khoản hoặc mật khẩu người dùng không đúng";
                        return result;
                    }

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("___GetInvoiceData:", invoices));

                    string url = Base.RequestUriStore.CombileUrl(DataApi.VIETTEL_Address, Base.RequestUriStore.GetInvoiceRepresentationFileV2);

                    string strParam = Newtonsoft.Json.JsonConvert.SerializeObject(invoices);
                    Inventec.Common.Logging.LogSystem.Debug("_____sendJsonData : " + strParam);

                    result = Base.ApiConsumerV2.CallWebRequest<Model.Response>(System.Net.WebRequestMethods.Http.Post, url, DataApi.User, DataApi.Pass, access_token, "application/x-www-form-urlencoded", strParam);
                }
                else
                {
                    result = new Base.ApiConsumer(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass).CreateRequest<Model.Response>(Base.RequestUriStore.GetInvoiceRepresentationFileV1, invoices);
                }
            }

            return result;
        }

        private bool CheckInvoiceGet(GetInvoiceRepresentationFileData invoices, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (invoices == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(invoices.supplierTaxCode))
                {
                    mess = "Không có thông tin mã số thuế";
                }
                else if (String.IsNullOrWhiteSpace(invoices.templateCode))
                {
                    mess = "Không có thông tin mã mẫu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(invoices.invoiceNo))
                {
                    mess = "Không có thông tin ký hiệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(invoices.fileType) || (invoices.fileType.ToLower() != "pdf" && invoices.fileType.ToLower() != "zip"))
                {
                    mess = "Loại dữ liệu tải về phải là PDF hoặc ZIP";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;

                    if (data == null) data = new Model.Response();

                    data.errorCode = Base.Constants.ErrorCode;
                    data.description = mess;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private Response GetInvoiceInfo(GetInvoiceInfoFilter getInvoiceInfoFilter)
        {
            Response result = new Response();

            Inventec.Common.Logging.LogSystem.Info("--------------------------GetInvoiceInfo");
            if (DataApi.Version == Version.v2)
            {
                Dictionary<string, string> access_token = null;

                string accessToken = LoginProcess.Login(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass);

                if (!String.IsNullOrWhiteSpace(accessToken))
                {
                    access_token = new Dictionary<string, string>();
                    access_token[Base.Constants.keyHeaderToken] = string.Format(Base.Constants.valueHeaderToken, accessToken);
                }
                else
                {
                    result.errorCode = Base.Constants.ErrorCode;
                    result.description = "Tài khoản hoặc mật khẩu người dùng không đúng";
                    return result;
                }

                string url = Base.RequestUriStore.CombileUrl(DataApi.VIETTEL_Address, string.Format(Base.RequestUriStore.GetInvoicesV2, this.DataApi.SupplierTaxCode));

                string strParam = Newtonsoft.Json.JsonConvert.SerializeObject(getInvoiceInfoFilter);
                Inventec.Common.Logging.LogSystem.Debug("_____sendJsonData : " + strParam);
                result = Base.ApiConsumerV2.CallWebRequest<Model.Response>(System.Net.WebRequestMethods.Http.Post, url, DataApi.User, DataApi.Pass, access_token, "application/json", strParam);
            }
            else
            {
                string url = string.Format(Base.RequestUriStore.GetInvoicesV1, this.DataApi.SupplierTaxCode);
                result = new Base.ApiConsumer(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass).CreateRequest<Model.Response>(url, getInvoiceInfoFilter);
            }

            return result;
        }

        private Model.Response Create(Model.DataCreateInvoice invoices)
        {
            Inventec.Common.Logging.LogSystem.Info("Create");
            Model.Response result = new Model.Response();

            if (this.CheckCreate(invoices, ref result))
            {
                if (DataApi.Version == Version.v2)
                {
                    Dictionary<string, string> access_token = null;

                    string accessToken = LoginProcess.Login(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass);

                    if (!String.IsNullOrWhiteSpace(accessToken))
                    {
                        access_token = new Dictionary<string, string>();
                        access_token[Base.Constants.keyHeaderToken] = string.Format(Base.Constants.valueHeaderToken, accessToken);
                    }
                    else
                    {
                        result.errorCode = Base.Constants.ErrorCode;
                        result.description = "Tài khoản hoặc mật khẩu người dùng không đúng";
                        return result;
                    }

                    string url = Base.RequestUriStore.CombileUrl(DataApi.VIETTEL_Address, string.Format(Base.RequestUriStore.CreateInvoiceV2, this.DataApi.SupplierTaxCode));

                    string strParam = Newtonsoft.Json.JsonConvert.SerializeObject(invoices);
                    Inventec.Common.Logging.LogSystem.Debug("_____sendJsonData : " + strParam);
                    result = Base.ApiConsumerV2.CallWebRequest<Model.Response>(System.Net.WebRequestMethods.Http.Post, url, DataApi.User, DataApi.Pass, access_token, "application/json", strParam);
                }
                else
                {
                    string url = string.Format(Base.RequestUriStore.CreateInvoiceV1, this.DataApi.SupplierTaxCode);
                    result = new Base.ApiConsumer(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass).CreateRequest<Model.Response>(url, invoices);
                }
            }

            return result;
        }

        private bool CheckCreate(Model.DataCreateInvoice invoices, ref Model.Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (invoices == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (invoices.generalInvoiceInfo == null)
                {
                    mess = "Không có thông tin chung hóa đơn";
                }
                else if (invoices.buyerInfo == null)
                {
                    mess = "Không có thông tin người mua";
                }
                //else if (Invoices.sellerInfo == null)
                //{
                //    mess = "Không có thông tin người bán";
                //}
                else if (invoices.itemInfo == null || invoices.itemInfo.Count <= 0)
                {
                    mess = "Chi tiết hóa đơn không được để trống";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;

                    if (data == null) data = new Model.Response();

                    data.errorCode = Base.Constants.ErrorCode;
                    data.description = mess;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private Model.Response Cancel(Model.CancelInvoice invoicesCancel)
        {
            Inventec.Common.Logging.LogSystem.Info("Cancel");
            Model.Response result = new Model.Response();

            if (this.CheckCancel(invoicesCancel, ref result))
            {
                if (DataApi.Version == Version.v2)
                {
                    Dictionary<string, string> access_token = null;

                    string accessToken = LoginProcess.Login(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass);

                    if (!String.IsNullOrWhiteSpace(accessToken))
                    {
                        access_token = new Dictionary<string, string>();
                        access_token[Base.Constants.keyHeaderToken] = string.Format(Base.Constants.valueHeaderToken, accessToken);
                    }
                    else
                    {
                        result.errorCode = Base.Constants.ErrorCode;
                        result.description = "Tài khoản hoặc mật khẩu người dùng không đúng";
                        return result;
                    }

                    string url = Base.RequestUriStore.CombileUrl(DataApi.VIETTEL_Address, Base.RequestUriStore.CancelInvoiceV2);

                    string cancelData = "";
                    System.Reflection.PropertyInfo[] pi = invoicesCancel.GetType().GetProperties();
                    foreach (var item in pi)
                    {
                        var val = item.GetValue(invoicesCancel);
                        if (val != null)
                        {
                            cancelData += string.Format("{0}={1}&", item.Name, HttpUtility.UrlEncode(val.ToString()));
                        }
                    }

                    if (cancelData.EndsWith("&"))
                    {
                        cancelData = cancelData.Substring(0, cancelData.Length - 1);
                    }

                    result = Base.ApiConsumerV2.CallWebRequest<Model.Response>(System.Net.WebRequestMethods.Http.Post, url, DataApi.User, DataApi.Pass, access_token, "application/x-www-form-urlencoded", cancelData);
                }
                else
                {
                    result = new Base.ApiConsumer(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass).RequestFormData<Model.Response>(Base.RequestUriStore.CancelInvoiceV1, invoicesCancel);
                }
            }

            return result;
        }

        private bool CheckCancel(Model.CancelInvoice invoices, ref Model.Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (invoices == null)
                {
                    mess = "Không có dữ liệu hủy hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(invoices.supplierTaxCode))
                {
                    mess = "Không có thông tin mã số thuế";
                }
                else if (String.IsNullOrWhiteSpace(invoices.templateCode))
                {
                    mess = "Không có thông tin mã mẫu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(invoices.invoiceNo))
                {
                    mess = "Không có thông tin ký hiệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(invoices.additionalReferenceDesc))
                {
                    mess = "Không có thông tin lý do hủy";
                }
                else if (String.IsNullOrWhiteSpace(invoices.additionalReferenceDate))
                {
                    mess = "Không có thông tin thời gian hủy";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;

                    if (data == null) data = new Model.Response();

                    data.errorCode = Base.Constants.ErrorCode;
                    data.description = mess;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private Model.Response GetFile(Model.GetFile invoices)
        {
            Inventec.Common.Logging.LogSystem.Info("GetFile");
            Model.Response result = new Model.Response();

            if (this.CheckFile(invoices, ref result))
            {
                if (DataApi.Version == Version.v2)
                {
                    Dictionary<string, string> access_token = null;

                    string accessToken = LoginProcess.Login(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass);

                    if (!String.IsNullOrWhiteSpace(accessToken))
                    {
                        access_token = new Dictionary<string, string>();
                        access_token[Base.Constants.keyHeaderToken] = string.Format(Base.Constants.valueHeaderToken, accessToken);
                    }
                    else
                    {
                        result.errorCode = Base.Constants.ErrorCode;
                        result.description = "Tài khoản hoặc mật khẩu người dùng không đúng";
                        return result;
                    }

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("___GetInvoiceData:", invoices));

                    string getInvoicesData = "";
                    System.Reflection.PropertyInfo[] pi = invoices.GetType().GetProperties();
                    foreach (var item in pi)
                    {
                        var val = item.GetValue(invoices);
                        if (val != null)
                        {
                            getInvoicesData += string.Format("{0}={1}&", item.Name, HttpUtility.UrlEncode(val.ToString()));
                        }
                    }

                    if (getInvoicesData.EndsWith("&"))
                    {
                        getInvoicesData = getInvoicesData.Substring(0, getInvoicesData.Length - 1);
                    }

                    if (!String.IsNullOrWhiteSpace(invoices.transactionUuid))
                    {
                        string url = Base.RequestUriStore.CombileUrl(DataApi.VIETTEL_Address, Base.RequestUriStore.SearchInvoiceByTransactionUuidV2);
                        ResponseTransactionUuid apiRessult = Base.ApiConsumerV2.CallWebRequest<Model.ResponseTransactionUuid>(System.Net.WebRequestMethods.Http.Post, url, DataApi.User, DataApi.Pass, access_token, "application/x-www-form-urlencoded", getInvoicesData);
                        if (apiRessult != null)
                        {
                            result.description = apiRessult.description;
                            result.errorCode = apiRessult.errorCode;
                            if (apiRessult.result != null && apiRessult.result.Count > 0)
                            {
                                result.result = apiRessult.result.First();
                            }
                        }
                    }
                    else
                    {
                        string url = Base.RequestUriStore.CombileUrl(DataApi.VIETTEL_Address, Base.RequestUriStore.GetFileInvoiceV2);
                        result = Base.ApiConsumerV2.CallWebRequest<Model.Response>(System.Net.WebRequestMethods.Http.Post, url, DataApi.User, DataApi.Pass, access_token, "application/x-www-form-urlencoded", getInvoicesData);
                    }
                }
                else
                {
                    result = new Base.ApiConsumer(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass).RequestFormData<Model.Response>(Base.RequestUriStore.GetFileInvoiceV1, invoices);
                }
            }

            return result;
        }

        private bool CheckFile(Model.GetFile invoices, ref Model.Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (invoices == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(invoices.supplierTaxCode))
                {
                    mess = "Không có thông tin mã số thuế";
                }
                else if (String.IsNullOrWhiteSpace(invoices.transactionUuid) && String.IsNullOrWhiteSpace(invoices.templateCode))
                {
                    mess = "Không có thông tin mã mẫu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(invoices.transactionUuid) && String.IsNullOrWhiteSpace(invoices.invoiceNo))
                {
                    mess = "Không có thông tin ký hiệu hóa đơn";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;

                    if (data == null) data = new Model.Response();

                    data.errorCode = Base.Constants.ErrorCode;
                    data.description = mess;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private Model.Response GetCustomField(Model.GetMetadata data)
        {
            Inventec.Common.Logging.LogSystem.Info("GetCustomField");
            Model.Response result = new Model.Response();

            if (this.CheckCustomField(data, ref result))
            {
                if (DataApi.Version == Version.v2)
                {
                    Dictionary<string, string> access_token = null;

                    string accessToken = LoginProcess.Login(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass);

                    if (!String.IsNullOrWhiteSpace(accessToken))
                    {
                        access_token = new Dictionary<string, string>();
                        access_token[Base.Constants.keyHeaderToken] = string.Format(Base.Constants.valueHeaderToken, accessToken);
                    }
                    else
                    {
                        result.errorCode = Base.Constants.ErrorCode;
                        result.description = "Tài khoản hoặc mật khẩu người dùng không đúng";
                        return result;
                    }

                    string url = Base.RequestUriStore.CombileUrl(DataApi.VIETTEL_Address, Base.RequestUriStore.GetCustomFieldsV2);

                    url += "?";
                    System.Reflection.PropertyInfo[] pi = data.GetType().GetProperties();
                    foreach (var item in pi)
                    {
                        var val = item.GetValue(data);
                        if (val != null)
                        {
                            url += string.Format("{0}={1}&", item.Name, HttpUtility.UrlEncode(val.ToString()));
                        }
                    }

                    string getData = "";

                    result = Base.ApiConsumerV2.CallWebRequest<Model.Response>(System.Net.WebRequestMethods.Http.Get, url, DataApi.User, DataApi.Pass, access_token, null, getData);
                }
                else
                {
                    result = new Base.ApiConsumer(DataApi.VIETTEL_Address, DataApi.User, DataApi.Pass).RequestFormData<Model.Response>(Base.RequestUriStore.GetCustomFieldsV1, data);
                }
            }

            return result;
        }

        private bool CheckCustomField(Model.GetMetadata data, ref Model.Response customFields)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (data == null)
                {
                    mess = "Khong co du lieu mau hoa don";
                }
                else if (String.IsNullOrWhiteSpace(data.taxCode))
                {
                    mess = "Khong co thong tin ma so thue";
                }
                else if (String.IsNullOrWhiteSpace(data.templateCode))
                {
                    mess = "Khong co thong tin ma mau hoa don";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;

                    if (customFields == null) customFields = new Model.Response();

                    customFields.errorCode = Base.Constants.ErrorCode;
                    customFields.description = mess;
                    Inventec.Common.Logging.LogSystem.Error(mess);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
