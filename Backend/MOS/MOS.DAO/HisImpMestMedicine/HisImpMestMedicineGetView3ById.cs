using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisImpMestMedicine
{
    partial class HisImpMestMedicineGet : EntityBase
    {
        public V_HIS_IMP_MEST_MEDICINE_3 GetView3ById(long id, HisImpMestMedicineSO search)
        {
            V_HIS_IMP_MEST_MEDICINE_3 result = null;
            try
            {
                bool valid = true;
                valid = valid && IsGreaterThanZero(id);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_HIS_IMP_MEST_MEDICINE_3.AsQueryable().Where(p => p.ID == id);
                        if (search.listVHisImpMestMedicine3Expression != null && search.listVHisImpMestMedicine3Expression.Count > 0)
                        {
                            foreach (var item in search.listVHisImpMestMedicine3Expression)
                            {
                                query = query.Where(item);
                            }
                        }
                        result = query.SingleOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search), LogType.Error);
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
