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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventec.Common.ElectronicBill.Misa.Processor
{
    class ConvertInvoice : IRun
    {
        DataInit Data;

        public ConvertInvoice()
        {

        }

        Response IRun.Run(DataInit data)
        {
            Response result = new Response();
            try
            {
                this.Data = data;

                if (data.DataGet.GetType() == typeof(GetInvoiceV2))
                {
                    result = DoConvertInvoice((GetInvoiceV2)data.DataGet);
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

        private Response DoConvertInvoice(GetInvoiceV2 data)
        {
            Response result = new Response();
            try
            {
                if (this.CheckConvertInvoiceDataV2(data, ref result))
                {
                    VoucherPaperADO requestData = new VoucherPaperADO();
                    requestData.Converter = data.Converter;
                    requestData.ConvertDate = DateTime.Today;
                    requestData.TransactionIDList = new List<string>() { data.TransactionID };
                    var apiResult = new Base.ApiConsumerV2(this.Data.BaseUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .CreateRequest<ApiResult>(Base.RequestUriStore.ConvertFileInvoiceV2, requestData);
                    if (apiResult == null || !apiResult.Success)
                    {
                        string error = apiResult != null && !String.IsNullOrWhiteSpace(apiResult.ErrorCode) ? (MappingError.DicMapping.ContainsKey(apiResult.ErrorCode) ? MappingError.DicMapping[apiResult.ErrorCode] : apiResult.ErrorCode) : "";
                        throw new Exception(error);
                    }

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("______DoConvertInvoice data: ", apiResult));

                    if (!String.IsNullOrWhiteSpace(apiResult.Data))
                    {
                        List<InvoiceResultV2> listInvoice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InvoiceResultV2>>(apiResult.Data);
                        if (listInvoice != null && listInvoice.Count > 0)
                        {
                            if (String.IsNullOrWhiteSpace(listInvoice[0].Data))
                            {
                                string error = String.Format("Vui lòng thực hiện in hóa đơn điện tử sau vài phút. (Mã lỗi : {0})",listInvoice[0].ErrorCode);
                                throw new Exception(error);
                            }
                            string fileName = ProcessFile.GenerateTempFile();
                            byte[] fileData = Convert.FromBase64String(listInvoice[0].Data);
                            File.WriteAllBytes(fileName, fileData);
                            result.fileDownload = fileName;

                            List<string> errorCode = listInvoice.Where(o => !String.IsNullOrWhiteSpace(o.ErrorCode)).Select(s => s.ErrorCode).Distinct().ToList();
                            List<string> messError = new List<string>();
                            if (errorCode != null)
                            {
                                foreach (var item in errorCode)
                                {
                                    messError.Add(MappingError.DicMapping.ContainsKey(item) ? MappingError.DicMapping[item] : item);
                                }
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

        private bool CheckConvertInvoiceDataV2(GetInvoiceV2 data, ref Response dataResult)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (data == null)
                {
                    mess = "Không có dữ liệu hóa đơn";
                }
                else if (String.IsNullOrWhiteSpace(data.TransactionID))
                {
                    mess = "Không có thông tin hóa đơn cần chuyển đổi";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    if (dataResult == null) dataResult = new Model.Response();

                    dataResult.description = mess;
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
