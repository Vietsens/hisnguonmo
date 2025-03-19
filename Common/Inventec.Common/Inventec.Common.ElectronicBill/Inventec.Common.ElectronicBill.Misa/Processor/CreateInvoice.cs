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
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Processor
{
    class CreateInvoice : IRun
    {
        DataInit Data;

        public CreateInvoice() { }

        Response IRun.Run(DataInit data)
        {
            Response result = new Response();
            try
            {
                this.Data = data;

                if (data.DataCreate.GetType() == typeof(List<CreateInvoiceData>))
                {
                    result = DoCreateInvoice((List<CreateInvoiceData>)data.DataCreate);
                }
                else if (data.DataCreate.GetType() == typeof(List<CreateInvoiceV2>))
                {
                    result = DoCreateInvoiceV2((List<CreateInvoiceV2>)data.DataCreate);
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

        private Response DoCreateInvoice(List<CreateInvoiceData> createData)
        {
            Response result = new Response();
            try
            {
                if (this.CheckListData(createData, ref result))
                {
                    var apiResult = new Base.ApiConsumer(this.Data.BaseUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .CreateRequest<ApiResult>(Base.RequestUriStore.CreateInvoice, createData);
                    if (apiResult == null || !apiResult.Success)
                    {
                        string error = apiResult != null && !String.IsNullOrWhiteSpace(apiResult.ErrorCode) ? (MappingError.DicMapping.ContainsKey(apiResult.ErrorCode) ? MappingError.DicMapping[apiResult.ErrorCode] : apiResult.ErrorCode) : "";
                        throw new Exception("Tạo hóa đơn điện tử thất bại. " + error);
                    }

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("______DoCreateInvoice data: ", apiResult));

                    if (!String.IsNullOrWhiteSpace(apiResult.Data))
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
            catch (Exception ex)
            {
                result = new Response();
                result.description = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckListData(List<CreateInvoiceData> list, ref Response data)
        {
            bool result = false;
            try
            {
                if (list != null && list.Count > 0)
                {
                    bool success = true;
                    foreach (var item in list)
                    {
                        success = success && this.CheckCreateData(item, ref data);
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

        private bool CheckCreateData(CreateInvoiceData createInvoice, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (createInvoice == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.RefID))
                {
                    mess = "Không có thông tin mã giao dịch";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.InvoiceType))
                {
                    mess = "Không có thông tin loại hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.TemplateCode))
                {
                    mess = "Không có thông tin mẫu số";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.InvoiceSeries))
                {
                    mess = "Không có thông tin ký hiệu";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.CurrencyCode))
                {
                    mess = "Không có thông tin loại tiền tệ";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.PaymentMethodName))
                {
                    mess = "Không có thông tin phương thức thanh toán";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.SellerTaxCode))
                {
                    mess = "Không có thông tin mã số thuế người bán";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.SellerLegalName))
                {
                    mess = "Không có thông tin tên người bán";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.SellerAddressLine))
                {
                    mess = "Không có thông tin địa chỉ người bán";
                }
                else if (createInvoice.OriginalInvoiceDetail == null || createInvoice.OriginalInvoiceDetail.Count == 0)
                {
                    mess = "Không có thông tin chi tiết hóa đơn";
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

        private Response DoCreateInvoiceV2(List<CreateInvoiceV2> createData)
        {
            Response result = new Response();
            try
            {
                if (this.CheckListDataV2(createData, ref result))
                {
                    var apiResult = new Base.ApiConsumerV2(this.Data.BaseUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .CreateRequest<ApiResult>(Base.RequestUriStore.CreateInvoiceV2, createData);
                    if (apiResult == null || !apiResult.Success)
                    {
                        string error = apiResult != null && !String.IsNullOrWhiteSpace(apiResult.ErrorCode) ? (MappingError.DicMapping.ContainsKey(apiResult.ErrorCode) ? MappingError.DicMapping[apiResult.ErrorCode] : apiResult.ErrorCode) : "";
                        throw new Exception(error);
                    }

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("______DoCreateInvoice data: ", apiResult));

                    if (!String.IsNullOrWhiteSpace(apiResult.Data))
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
            catch (Exception ex)
            {
                result = new Response();
                result.description = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckCreateDataV2(CreateInvoiceV2 createInvoice, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (createInvoice == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.RefID))
                {
                    mess = "Không có thông tin mã giao dịch";
                }
                //else if (String.IsNullOrWhiteSpace(createInvoice.InvoiceType))
                //{
                //    mess = "Không có thông tin loại hóa đơn";
                //}
                //else if (String.IsNullOrWhiteSpace(createInvoice.TemplateCode))
                //{
                //    mess = "Không có thông tin mẫu số";
                //}
                else if (String.IsNullOrWhiteSpace(createInvoice.InvSeries))
                {
                    mess = "Không có thông tin ký hiệu";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.CurrencyCode))
                {
                    mess = "Không có thông tin loại tiền tệ";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.PaymentMethodName))
                {
                    mess = "Không có thông tin phương thức thanh toán";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.SellerTaxCode))
                {
                    mess = "Không có thông tin mã số thuế người bán";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.SellerLegalName))
                {
                    mess = "Không có thông tin tên người bán";
                }
                else if (String.IsNullOrWhiteSpace(createInvoice.SellerAddress))
                {
                    mess = "Không có thông tin địa chỉ người bán";
                }
                else if (createInvoice.OriginalInvoiceDetail == null || createInvoice.OriginalInvoiceDetail.Count == 0)
                {
                    mess = "Không có thông tin chi tiết hóa đơn";
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

        private bool CheckListDataV2(List<CreateInvoiceV2> list, ref Response data)
        {
            bool result = false;
            try
            {
                if (list != null && list.Count > 0)
                {
                    bool success = true;
                    foreach (var item in list)
                    {
                        success = success && this.CheckCreateDataV2(item, ref data);
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
    }
}
