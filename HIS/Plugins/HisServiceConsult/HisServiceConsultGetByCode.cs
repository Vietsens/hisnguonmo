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
        internal HIS_SERVICE_CONSULT GetByCode(string code)
        {
            try
            {
                return GetByCode(code, new HisServiceConsultFilterQuery());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }

        internal HIS_SERVICE_CONSULT GetByCode(string code, HisServiceConsultFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisServiceConsultDAO.GetByCode(code, filter.Query());
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
