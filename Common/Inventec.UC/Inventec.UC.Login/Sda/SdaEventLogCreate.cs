using Inventec.Core;
using Inventec.UC.Login.Base;
using SDA.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Sda.SdaEventLogCreate
{
    internal partial class SdaEventLogCreate : BusinessBase
    {
        public SdaEventLogCreate()
            : base()
        {

        }

        public SdaEventLogCreate(CommonParam paramCreate)
            : base(paramCreate)
        {

        }

        public void Create(string loginName,long? eventLogTypeId, bool? isSuccess, string message)
        {
            try
            {
                SdaEventLogSDO data = new SdaEventLogSDO();
                data.EventLogTypeId = eventLogTypeId;
                data.IsSuccess = isSuccess;
                data.Description = message;
                data.Ip = Inventec.Common.WebApiClient.IpAddressUtils.GetIpAddressLocal();
                data.LogginName = loginName;
                Create(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
            }
        }

        private bool Create(SdaEventLogSDO data)
        {
            bool result = false;
            try
            {
                Inventec.Core.ApiResultObject<bool> aro = ApiConsumerStore.SdaConsumer.Post<Inventec.Core.ApiResultObject<bool>>("/api/SdaEventLog/Create", param, data);
                if (aro != null)
                {
                    if (aro.Param != null)
                    {
                        param.Messages.AddRange(aro.Param.Messages);
                        param.BugCodes.AddRange(aro.Param.BugCodes);
                    }
                    result = aro.Success;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
                result = false;
            }
            return result;
        }
    }
}
