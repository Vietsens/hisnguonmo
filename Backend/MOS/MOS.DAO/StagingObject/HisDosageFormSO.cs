using MOS.DAO.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace MOS.DAO.StagingObject
{
    public class HisDosageFormSO : StagingObjectBase
    {
        public HisDosageFormSO()
        {
            
        }

        public List<System.Linq.Expressions.Expression<Func<HIS_DOSAGE_FORM, bool>>> listHisDosageFormExpression = new List<System.Linq.Expressions.Expression<Func<HIS_DOSAGE_FORM, bool>>>();
    }
}
