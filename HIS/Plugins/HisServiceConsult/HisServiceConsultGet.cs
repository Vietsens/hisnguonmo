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
        internal HisServiceConsultGet()
            : base()
        {

        }

        internal HisServiceConsultGet(CommonParam paramGet)
            : base(paramGet)
        {

        }

        internal List<HIS_SERVICE_CONSULT> Get(HisServiceConsultFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisServiceConsultDAO.Get(filter.Query(), param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }

        internal HIS_SERVICE_CONSULT GetById(long id)
        {
            try
            {
                return GetById(id, new HisServiceConsultFilterQuery());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }

        internal HIS_SERVICE_CONSULT GetById(long id, HisServiceConsultFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisServiceConsultDAO.GetById(id, filter.Query());
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
