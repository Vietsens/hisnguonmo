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
        internal V_HIS_SERVICE_CONSULT GetViewByCode(string code)
        {
            try
            {
                return GetViewByCode(code, new HisServiceConsultViewFilterQuery());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }

        internal V_HIS_SERVICE_CONSULT GetViewByCode(string code, HisServiceConsultViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisServiceConsultDAO.GetViewByCode(code, filter.Query());
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
