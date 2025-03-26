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
using Inventec.Common.ElectronicBill.Misa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inventec.Common.ElectronicBill.Misa.Processor
{
    class ReleaseInvoice : IRun
    {
        DataInit Data;

        public ReleaseInvoice()
        {

        }

        Response IRun.Run(DataInit data)
        {
            Response result = new Response();
            try
            {
                this.Data = data;

                if (data.DataRelease.GetType() == typeof(List<ReleaseInvoiceData>))
                {
                    result = DoReleaseInvoice((List<ReleaseInvoiceData>)data.DataRelease);
                }
                else if (data.DataRelease.GetType() == typeof(List<ReleaseInvoiceV2>))
                {
                    result = DoReleaseInvoiceV2((List<ReleaseInvoiceV2>)data.DataRelease);
                }
            }
            catch (Exception)
            {
                result = new Response();
                result.description = "Lỗi xử lý hóa đơn";
                throw;
            }
            return result;
        }

        private Response DoReleaseInvoice(List<ReleaseInvoiceData> dataRelease)
        {
            Response result = new Response();
            try
            {
                if (this.CheckListRelease(dataRelease, ref result))
                {
                    var apiResult = new Base.ApiConsumer(this.Data.BaseUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .CreateRequest<ApiResult>(Base.RequestUriStore.ReleaseInvoice, dataRelease);
                    if (apiResult == null || !apiResult.Success)
                    {
                        string error = apiResult != null && !String.IsNullOrWhiteSpace(apiResult.ErrorCode) ? (MappingError.DicMapping.ContainsKey(apiResult.ErrorCode) ? MappingError.DicMapping[apiResult.ErrorCode] : apiResult.ErrorCode) : "";
                        throw new Exception("Phát hành hóa đơn điện tử thất bại. " + error);
                    }
                    else if (apiResult != null && apiResult.Success)
                    {
                        if (apiResult.ErrorCode == "InvoiceDuplicated")
                        {
                            result.description = "Hóa đơn đã phát hành!";
                        }
                        else if (!string.IsNullOrWhiteSpace(apiResult.Data))
                        {
                            List<InvoiceResult> listInvoice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InvoiceResult>>(apiResult.Data);
                            if (listInvoice != null && listInvoice.Count > 0)
                            {
                                result.result = listInvoice;
                                List<string> errorCode = listInvoice.Where(o => !String.IsNullOrWhiteSpace(o.ErrorCode)).Select(s => s.ErrorCode).Distinct().ToList();
                                List<string> messError = new List<string>();
                                foreach (var item in errorCode)
                                {
                                    messError.Add(MappingError.DicMapping.ContainsKey(item) ? MappingError.DicMapping[item] : item);
                                }

                                result.description = string.Join(", ", messError);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Response();
                result.description = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckListRelease(List<ReleaseInvoiceData> list, ref Response data)
        {
            bool result = false;
            try
            {
                if (list != null && list.Count > 0)
                {
                    bool success = true;
                    foreach (var item in list)
                    {
                        success = success && this.CheckReleaseData(item, ref data);
                    }

                    result = success;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckReleaseData(ReleaseInvoiceData releaseInvoice, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (releaseInvoice == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(releaseInvoice.InvoiceData))
                {
                    mess = "Không có thông tin dữ liệu chi tiết hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(releaseInvoice.RefID))
                {
                    mess = "Không có thông tin mã giao dịch";
                }
                else if (String.IsNullOrWhiteSpace(releaseInvoice.TransactionID))
                {
                    mess = "Không có thông tin mã tra cứu";
                }
                else if (releaseInvoice.IsSendEmail && (String.IsNullOrWhiteSpace(releaseInvoice.ReceiverName) || String.IsNullOrWhiteSpace(releaseInvoice.ReceiverEmail)))
                {
                    if (String.IsNullOrWhiteSpace(releaseInvoice.ReceiverName))
                    {
                        mess = "Không có thông tin tên người nhận";
                    }
                    else
                    {
                        mess = "Không có thông tin địa chỉ email nhận";
                    }
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    if (data == null) data = new Model.Response();

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

        private Response DoReleaseInvoiceV2(List<ReleaseInvoiceV2> dataRelease)
        {
            Response result = new Response();
            try
            {
                if (this.CheckListReleaseV2(dataRelease, ref result))
                {
                    var apiResult = new Base.ApiConsumerV2(this.Data.BaseUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .CreateRequest<ApiResult>(Base.RequestUriStore.ReleaseInvoiceV2, dataRelease);
                    if (apiResult == null || !apiResult.Success)
                    {
                        string error = apiResult != null && !String.IsNullOrWhiteSpace(apiResult.ErrorCode) ? (MappingError.DicMapping.ContainsKey(apiResult.ErrorCode) ? MappingError.DicMapping[apiResult.ErrorCode] : apiResult.ErrorCode) : "";
                        throw new Exception("Phát hành hóa đơn điện tử thất bại. " + error);
                    }
                    else if (apiResult != null && apiResult.Success)
                    {
                        if (apiResult.ErrorCode == "InvoiceDuplicated")
                        {
                            result.description = "Hóa đơn đã phát hành!";
                        }
                        else if (!string.IsNullOrWhiteSpace(apiResult.Data))
                        {
                            List<InvoiceResultV2> listInvoice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InvoiceResultV2>>(apiResult.Data);
                            if (listInvoice != null && listInvoice.Count > 0)
                            {
                                result.resultV2 = listInvoice;
                                List<string> errorCode = listInvoice.Where(o => !String.IsNullOrWhiteSpace(o.ErrorCode)).Select(s => s.ErrorCode).Distinct().ToList();
                                List<string> messError = new List<string>();
                                foreach (var item in errorCode)
                                {
                                    messError.Add(MappingError.DicMapping.ContainsKey(item) ? MappingError.DicMapping[item] : item);
                                }

                                result.description = string.Join(", ", messError);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Response();
                result.description = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckListReleaseV2(List<ReleaseInvoiceV2> list, ref Response data)
        {
            bool result = false;
            try
            {
                if (list != null && list.Count > 0)
                {
                    bool success = true;
                    foreach (var item in list)
                    {
                        success = success && this.CheckReleaseDataV2(item, ref data);
                    }

                    result = success;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckReleaseDataV2(ReleaseInvoiceV2 releaseInvoice, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (releaseInvoice == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(releaseInvoice.InvoiceData))
                {
                    mess = "Không có thông tin dữ liệu chi tiết hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(releaseInvoice.RefID))
                {
                    mess = "Không có thông tin mã giao dịch";
                }
                else if (String.IsNullOrWhiteSpace(releaseInvoice.TransactionID))
                {
                    mess = "Không có thông tin mã tra cứu";
                }
                else if (releaseInvoice.IsSendEmail && (String.IsNullOrWhiteSpace(releaseInvoice.ReceiverName) || String.IsNullOrWhiteSpace(releaseInvoice.ReceiverEmail)))
                {
                    if (String.IsNullOrWhiteSpace(releaseInvoice.ReceiverName))
                    {
                        mess = "Không có thông tin tên người nhận";
                    }
                    else
                    {
                        mess = "Không có thông tin địa chỉ email nhận";
                    }
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    if (data == null) data = new Model.Response();

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
    }
}
