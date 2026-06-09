using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisServiceConsult
{
    partial class HisServiceConsultGet : BusinessBase
    {
        internal List<V_HIS_SERVICE_CONSULT> GetView(HisServiceConsultViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisServiceConsultDAO.GetView(filter.Query(), param);
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
