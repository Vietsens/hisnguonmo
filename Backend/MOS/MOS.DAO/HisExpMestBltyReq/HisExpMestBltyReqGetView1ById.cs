using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisExpMestBltyReq
{
    partial class HisExpMestBltyReqGet : EntityBase
    {
        public V_HIS_EXP_MEST_BLTY_REQ_1 GetView1ById(long id, HisExpMestBltyReqSO search)
        {
            V_HIS_EXP_MEST_BLTY_REQ_1 result = null;
            try
            {
                bool valid = true;
                valid = valid && IsGreaterThanZero(id);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_HIS_EXP_MEST_BLTY_REQ_1.AsQueryable().Where(p => p.ID == id);
                        if (search.listVHisExpMestBltyReq1Expression != null && search.listVHisExpMestBltyReq1Expression.Count > 0)
                        {
                            foreach (var item in search.listVHisExpMestBltyReq1Expression)
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
