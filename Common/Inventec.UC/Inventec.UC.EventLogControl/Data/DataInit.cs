
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
