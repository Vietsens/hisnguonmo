using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisInfusionSum
{
    partial class HisInfusionSumGet : BusinessBase
    {
        internal List<V_HIS_INFUSION_SUM> GetView(HisInfusionSumViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisInfusionSumDAO.GetView(filter.Query(), param);
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
