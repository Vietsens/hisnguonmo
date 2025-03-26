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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Data
{
    public class DataInit
    {
        public Inventec.Common.WebApiClient.ApiConsumer sdaComsumer;
        public long pageNum;
        public string loginName;
        public string patientCode;
        public string treatmentCode;
        public string impMestCode;
        public string expMestCode;
        public string serviceRequestCode;
        public string bidNumber;
        public string UriElasticSearchServer { get; set; }

        public DataInit(Inventec.Common.WebApiClient.ApiConsumer SdaComsumer, long PagingNum, string loginName)
        {
            try
            {
                this.sdaComsumer = SdaComsumer;
                this.pageNum = PagingNum;
                this.loginName = loginName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public DataInit(long PagingNum, string loginName, string patientCode, string treatmentCode, string impMestCode, string expMestCode, string serviceRequestCode)
        {
            try
            {
                // this.sdaComsumer = SdaComsumer;
                this.pageNum = PagingNum;
                this.loginName = loginName;
                this.patientCode = patientCode;
                this.treatmentCode = treatmentCode;
                this.impMestCode = impMestCode;
                this.expMestCode = expMestCode;
                this.serviceRequestCode = serviceRequestCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public DataInit(long PagingNum, string loginName, string patientCode, string treatmentCode, string impMestCode, string expMestCode, string serviceRequestCode, string bidNumber)
        {
            try
            {
                // this.sdaComsumer = SdaComsumer;
                this.pageNum = PagingNum;
                this.loginName = loginName;
                this.patientCode = patientCode;
                this.treatmentCode = treatmentCode;
                this.impMestCode = impMestCode;
                this.expMestCode = expMestCode;
                this.serviceRequestCode = serviceRequestCode;
                this.bidNumber = bidNumber;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
