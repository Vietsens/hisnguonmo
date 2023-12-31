using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisExmeReasonCfg
{
    partial class HisExmeReasonCfgGet : BusinessBase
    {
        internal List<V_HIS_EXME_REASON_CFG> GetView(HisExmeReasonCfgViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisExmeReasonCfgDAO.GetView(filter.Query(), param);
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
