using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisDataStore
{
    partial class HisDataStoreGet : EntityBase
    {
        public V_HIS_DATA_STORE GetViewByCode(string code, HisDataStoreSO search)
        {
            V_HIS_DATA_STORE result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_HIS_DATA_STORE.AsQueryable().Where(p => p.DATA_STORE_CODE == code);
                        if (search.listVHisDataStoreExpression != null && search.listVHisDataStoreExpression.Count > 0)
                        {
                            foreach (var item in search.listVHisDataStoreExpression)
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
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search), LogType.Error);
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
