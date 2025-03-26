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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    class Input
    {
        public static List<DataGet> Get(string _jsonOutPut)
        { 
            List<DataGet> datasuft = null;
            try
            {
                if (_jsonOutPut == null) return new List<DataGet>();
                //_jsonOutPut = "\"MRSINPUT_XXXX_IDs\":{0},\"DATA\":\"value 1,value 2\"";
                string toJson = "{" + (_jsonOutPut ?? "").Replace("{", "\"").Replace("}", "\"") + "}";

                Dictionary<string, string> dicFilter = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(toJson);
                var key = dicFilter.Keys.FirstOrDefault(o => o.StartsWith("MRSINPUT_"));
                if(key != null)
                {
                    SAR.Filter.SarReportTypeFilter filter = new SAR.Filter.SarReportTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.REPORT_TYPE_CODE = "MRSINPUT";
                    var mrsLoadingReportType = BackendDataWorker.Get<SAR_REPORT_TYPE>("api/SarReportType/Get", ApiConsumerStore.SarConsumer, filter);
                    if (mrsLoadingReportType != null && mrsLoadingReportType.Count>0)
                    {
                        return null;
                    }
                    ApiConsumerStore.MrsConsumer.SetTokenCode(ApiConsumerStore.SarConsumer.GetTokenCode());
                    datasuft = BackendDataWorker.Get<DataGet>(RequestUriStore.MRS_REPORT_GETINPUT, ApiConsumerStore.MrsConsumer, key);
                }    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return datasuft;
        }

    }
}
