using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisBidBloodType
{
    partial class HisBidBloodTypeGet : BusinessBase
    {
        internal List<V_HIS_BID_BLOOD_TYPE> GetView(HisBidBloodTypeViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisBidBloodTypeDAO.GetView(filter.Query(), param);
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
