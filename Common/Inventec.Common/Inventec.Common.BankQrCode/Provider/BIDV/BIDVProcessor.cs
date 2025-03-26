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
using Inventec.Common.BankQrCode.ADO;
using Inventec.Common.BankQrCode.Provider.BIDV.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.Provider.BIDV
{
    class BIDVProcessor : IRun
    {
        private ADO.BankQrCodeInputADO InputData;

        public BIDVProcessor(ADO.BankQrCodeInputADO InputData)
        {
            // TODO: Complete member initialization
            this.InputData = InputData;
        }

        ResultQrCode IRun.Run()
        {
            ResultQrCode result = new ResultQrCode();
            try
            {
                if (CheckInputData(ref result))
                {
                    BidvInfoData configArr = Newtonsoft.Json.JsonConvert.DeserializeObject<BidvInfoData>(this.InputData.SystemConfig);
                    var qrcode = new Qrcode( configArr.PayLoad, 
                        configArr.PointOTMethod, 
                        configArr.Guid, 
                        configArr.MerchantCode, 
                        configArr.MCC, 
                        configArr.Ccy, 
                        QrCodeUtil.ProcessConvertAmount(this.InputData.Amount),
                        configArr.CountryCode,
                        configArr.MerchantName,
                        configArr.MerchantCity,
                        configArr.PostalCode,
                        this.InputData.TransactionCode,
                        configArr.StoreLabel,
                        configArr.TerminalLabel,
                        this.InputData.Purpose,
                        "ME"
                         );
                    result.Data = qrcode.create();
                }
            }
            catch (Exception ex)
            {
                result = new ResultQrCode();
                result.Message = "Run Exception: " + ex.Message;
            }
            return result;
        }

        ResultQrCode IRun.RunConsumer()
        {
            return null;
        }

        private bool CheckInputData(ref ResultQrCode result)
        {
            try
            {
                if (string.IsNullOrEmpty(this.InputData.SystemConfig) || Newtonsoft.Json.JsonConvert.DeserializeObject<BidvInfoData>(this.InputData.SystemConfig) == null)
                {
                    result = new ResultQrCode();
                    result.Message = "Sai định dạng cấu hình hệ thống.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = new ResultQrCode();
                result.Message = "Check data Exception: " + ex.Message;
                return false;
            }

            return true;
        }

        public static T CreateRequest<T>(string baseUri, string requestUri, object sendData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                //client.Timeout = new TimeSpan(0, 0, 90);

                HttpResponseMessage resp = null;
                string sendJsonData = JsonConvert.SerializeObject(sendData);

                string extension = baseUri.Substring(baseUri.IndexOf('/', baseUri.IndexOf("//") + 2));
                string fullrequestUri = requestUri;
                if (!requestUri.Contains(extension))
                {
                    fullrequestUri = extension + requestUri;
                }

                resp = client.PostAsync(fullrequestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", baseUri, requestUri, statusCode));
                }

                string responseData = resp.Content.ReadAsStringAsync().Result;

                T data = JsonConvert.DeserializeObject<T>(responseData);
                if (data == null)
                {
                    throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                }
                return data;
            }
        }
    }
}
