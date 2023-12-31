using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisExpMestMetyReq
{
    partial class HisExpMestMetyReqGet : BusinessBase
    {
        internal List<V_HIS_EXP_MEST_METY_REQ> GetView(HisExpMestMetyReqViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisExpMestMetyReqDAO.GetView(filter.Query(), param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
