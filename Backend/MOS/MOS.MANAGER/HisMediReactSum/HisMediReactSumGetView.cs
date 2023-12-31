using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisMediReactSum
{
    partial class HisMediReactSumGet : BusinessBase
    {
        internal List<V_HIS_MEDI_REACT_SUM> GetView(HisMediReactSumViewFilterQuery filter)
        {
            try
            {
                return DAOWorker.HisMediReactSumDAO.GetView(filter.Query(), param);
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
