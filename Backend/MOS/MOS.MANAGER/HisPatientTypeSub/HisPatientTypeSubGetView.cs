using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisPatientTypeSub
{
    partial class HisPatientTypeSubGet : BusinessBase
    {
        internal List<V_HIS_PATIENT_TYPE_SUB> GetView(HisPatientTypeSubViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisPatientTypeSubDAO.GetView(filter.Query(), param);
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
