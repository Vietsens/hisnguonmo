using MOS.DAO.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace MOS.DAO.StagingObject
{
    public class HisMachineSO : StagingObjectBase
    {
        public HisMachineSO()
        {
            
        }

        public List<System.Linq.Expressions.Expression<Func<HIS_MACHINE, bool>>> listHisMachineExpression = new List<System.Linq.Expressions.Expression<Func<HIS_MACHINE, bool>>>();
        public List<System.Linq.Expressions.Expression<Func<V_HIS_MACHINE, bool>>> listVHisMachineExpression = new List<System.Linq.Expressions.Expression<Func<V_HIS_MACHINE, bool>>>();
    }
}
