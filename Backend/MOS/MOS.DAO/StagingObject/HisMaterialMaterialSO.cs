using MOS.DAO.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace MOS.DAO.StagingObject
{
    public class HisMaterialMaterialSO : StagingObjectBase
    {
        public HisMaterialMaterialSO()
        {
            
        }

        public List<System.Linq.Expressions.Expression<Func<HIS_MATERIAL_MATERIAL, bool>>> listHisMaterialMaterialExpression = new List<System.Linq.Expressions.Expression<Func<HIS_MATERIAL_MATERIAL, bool>>>();
    }
}
