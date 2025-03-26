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
    class DeleteInvoice : IRun
    {
        DataInit Data;

        public DeleteInvoice()
        {

        }

        Response IRun.Run(DataInit data)
        {
            Response result = new Response();
            try
            {
                this.Data = data;

                if (data.DataDelete.GetType() == typeof(DeleteInvoiceData))
                {
                    result = DoDeleteInvoice((DeleteInvoiceData)data.DataDelete);
                }
                else if (data.DataDelete.GetType() == typeof(DeleteInvoiceV2))
                {
                    result = DoDeleteInvoiceV2((DeleteInvoiceV2)data.DataDelete);
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

        private Response DoDeleteInvoice(DeleteInvoiceData dataDelete)
        {
            Response result = new Response();
            try
            {
                if (this.CheckDelete(dataDelete, ref result))
                {
                    var apiResult = new Base.ApiConsumer(this.Data.BaseUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .CreateRequest<ApiResult>(Base.RequestUriStore.CancelInvoice, dataDelete);
                    if (apiResult == null || !apiResult.Success)
                    {
                        string error = apiResult != null && !String.IsNullOrWhiteSpace(apiResult.ErrorCode) ? (MappingError.DicMapping.ContainsKey(apiResult.ErrorCode) ? MappingError.DicMapping[apiResult.ErrorCode] : apiResult.ErrorCode) : "";
                        result.description = error;
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

        private bool CheckDelete(DeleteInvoiceData dataDelete, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (dataDelete == null)
                {
                    mess = "Không có dữ liệu hủy hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(dataDelete.DeletedReason))
                {
                    mess = "Không có thông tin lý do xóa";
                }
                else if (String.IsNullOrWhiteSpace(dataDelete.RefNo))
                {
                    mess = "Không có thông tin số chứng từ";
                }
                else if (String.IsNullOrWhiteSpace(dataDelete.TransactionID))
                {
                    mess = "Không có thông tin mã tra cứu hóa đơn";
                }
                else if (dataDelete.RefDate == null)
                {
                    mess = "Không có thông tin ngày hủy";
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

        private Response DoDeleteInvoiceV2(DeleteInvoiceV2 dataDelete)
        {
            Response result = new Response();
            try
            {
                if (this.CheckDeleteV2(dataDelete, ref result))
                {
                    List<DeleteInvoiceV2> listDelete = new List<DeleteInvoiceV2> { dataDelete };

                    var apiResult = new Base.ApiConsumerV2(this.Data.BaseUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .CreateRequest<ApiResult>(Base.RequestUriStore.CancelInvoiceV2, listDelete);
                    if (apiResult == null || !apiResult.Success)
                    {
                        string error = apiResult != null && !String.IsNullOrWhiteSpace(apiResult.ErrorCode) ? (MappingError.DicMapping.ContainsKey(apiResult.ErrorCode) ? MappingError.DicMapping[apiResult.ErrorCode] : apiResult.ErrorCode) : "";
                        result.description = error;
                        if (!String.IsNullOrEmpty(error))
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
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

        private bool CheckDeleteV2(DeleteInvoiceV2 dataDelete, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (dataDelete == null)
                {
                    mess = "Không có dữ liệu hủy hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(dataDelete.CancelReason))
                {
                    mess = "Không có thông tin lý do xóa";
                }
                else if (String.IsNullOrWhiteSpace(dataDelete.InvNo))
                {
                    mess = "Không có thông tin số chứng từ";
                }
                else if (String.IsNullOrWhiteSpace(dataDelete.TransactionID))
                {
                    mess = "Không có thông tin mã tra cứu hóa đơn";
                }
                else if (dataDelete.RefDate == null)
                {
                    mess = "Không có thông tin ngày hủy";
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
