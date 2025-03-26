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
using Inventec.Core;
using Inventec.Fss.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Fss.Client
{
    public class FileDelete
    {
        public static bool DeleteFile(string clientCode, string fileUrl)
        {
            try
            {
                return DeleteFile(clientCode, fileUrl, FssConstant.BASE_URI);
            }
            catch (FileDeleteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FileDeleteException("Exception when uploading file", ex);
            }
        }

        public static bool DeleteFile(string clientCode, string fileUrl, string baseUri)
        {
            bool result = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, FssConstant.TIME_OUT);
                    client.DefaultRequestHeaders.Add(FssConstant.HEADER_CLIENT_CODE, clientCode);

                    string requestedUrl = FssConstant.DELETE_URI;

                    ApiParam apiParam = new ApiParam();
                    CommonParam commonParam = new CommonParam();

                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = fileUrl;

                    Inventec.Core.ApiResultObject<bool> rs = null;

                    HttpResponseMessage resp = client.PostAsJsonAsync(requestedUrl, apiParam).Result;
                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    try
                    {
                        rs = Newtonsoft.Json.JsonConvert.DeserializeObject<Inventec.Core.ApiResultObject<bool>>(responseData);
                        if (rs != null && rs.Success)
                        {
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (FileDeleteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FileDeleteException("Exception when delete file", ex);
            }
            return result;
        }
    }
}
