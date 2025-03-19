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
using System.Web;

namespace Inventec.Common.ElectronicBill.Misa.Processor
{
    class ViewInvoice : IRun
    {
        DataInit Data;

        public ViewInvoice()
        {

        }

        Response IRun.Run(DataInit data)
        {
            Response result = new Response();
            try
            {
                this.Data = data;

                if (data.DataGet.GetType() == typeof(GetInvoice))
                {
                    result = DoGetInvoice((GetInvoice)data.DataGet);
                }
                else if (data.DataGet.GetType() == typeof(GetInvoiceV2))
                {
                    result = DoGetInvoiceV2((GetInvoiceV2)data.DataGet);
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

        private Response DoGetInvoice(GetInvoice dataGet)
        {
            Response result = new Response();
            try
            {
                if (this.CheckGetData(dataGet, ref result))
                {
                    var apiResult = new Base.ApiConsumer(this.Data.BaseUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .CreateRequest<ApiResult>(string.Format(Base.RequestUriStore.GetFileInvoice, HttpUtility.UrlEncode(dataGet.Converter)), new List<string>() { dataGet.TransactionID });
                    if (apiResult == null || !apiResult.Success)
                    {
                        string error = apiResult != null && !String.IsNullOrWhiteSpace(apiResult.ErrorCode) ? (MappingError.DicMapping.ContainsKey(apiResult.ErrorCode) ? MappingError.DicMapping[apiResult.ErrorCode] : apiResult.ErrorCode) : "";
                        throw new Exception("Tải hóa đơn thất bại. " + error);
                    }

                    if (!String.IsNullOrWhiteSpace(apiResult.Data))
                    {
                        List<GetFileResult> fileData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GetFileResult>>(apiResult.Data);
                        if (fileData != null && fileData.Count > 0)
                        {
                            List<string> errorCode = fileData.Where(o => !String.IsNullOrWhiteSpace(o.ErrorCode)).Select(s => s.ErrorCode).Distinct().ToList();
                            List<string> messError = new List<string>();
                            foreach (var item in errorCode)
                            {
                                messError.Add(MappingError.DicMapping.ContainsKey(item) ? MappingError.DicMapping[item] : item);
                            }

                            result.description = string.Join(", ", messError);
                            if (!String.IsNullOrWhiteSpace(fileData.First().Data))
                            {
                                result.fileToBytes = System.Convert.FromBase64String(fileData.First().Data);
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

        private bool CheckGetData(GetInvoice getInvoice, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (getInvoice == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(getInvoice.Converter))
                {
                    mess = "Không có thông tin người chuyển đổi";
                }
                else if (String.IsNullOrWhiteSpace(getInvoice.TransactionID))
                {
                    mess = "Không có thông tin hóa đơn cần chuyển đổi";
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

        private Response DoGetInvoiceV2(GetInvoiceV2 dataGet)
        {
            Response result = new Response();
            try
            {
                if (this.CheckGetDataV2(dataGet, ref result))
                {
                    result.fileDownload = string.Format("{0}/{1}", this.Data.DownloadUrl, string.Format(Base.RequestUriStore.GetFileInvoiceV2, HttpUtility.UrlEncode(dataGet.TransactionID)));
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

        private bool CheckGetDataV2(GetInvoiceV2 getInvoice, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (getInvoice == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(getInvoice.TransactionID))
                {
                    mess = "Không có thông tin hóa đơn cần chuyển đổi";
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
