using Inventec.Core;
using Inventec.UC.Login.Base;
using THE.Filter;
using SDA.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Mos
{
    internal partial class MosBranchGet : BusinessBase
    {
        public MosBranchGet()
            : base()
        {

        }

        public MosBranchGet(CommonParam paramCreate)
            : base(paramCreate)
        {

        }

        public List<THE.EFMODEL.DataModels.THE_BRANCH> Get(TheBranchFilter branchFilter)
        {
            List<THE.EFMODEL.DataModels.THE_BRANCH> result = null;
            try
            {
                if (ApiConsumerStore.MosConsumer != null)
                {
                    Inventec.Core.ApiResultObject<List<THE.EFMODEL.DataModels.THE_BRANCH>> aro = ApiConsumerStore.MosConsumer.Get<Inventec.Core.ApiResultObject<List<THE.EFMODEL.DataModels.THE_BRANCH>>>("/api/TheBranch/Get", param, branchFilter);
                    if (aro != null)
                    {
                        if (aro.Param != null)
                        {
                            param.Messages.AddRange(aro.Param.Messages);
                            param.BugCodes.AddRange(aro.Param.BugCodes);
                        }
                        result = aro.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
                result = null;
            }
            return result;
        }
    }
}
