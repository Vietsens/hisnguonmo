using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisBidMedicineType
{
    partial class HisBidMedicineTypeGet : BusinessBase
    {
        internal List<V_HIS_BID_MEDICINE_TYPE> GetView(HisBidMedicineTypeViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisBidMedicineTypeDAO.GetView(filter.Query(), param);
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
