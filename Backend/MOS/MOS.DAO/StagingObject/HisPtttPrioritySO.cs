using MOS.DAO.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace MOS.DAO.StagingObject
{
    public class HisPtttPrioritySO : StagingObjectBase
    {
        public HisPtttPrioritySO()
        {
            
        }

        public List<System.Linq.Expressions.Expression<Func<HIS_PTTT_PRIORITY, bool>>> listHisPtttPriorityExpression = new List<System.Linq.Expressions.Expression<Func<HIS_PTTT_PRIORITY, bool>>>();
    }
}
