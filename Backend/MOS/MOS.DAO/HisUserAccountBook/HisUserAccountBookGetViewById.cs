using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisUserAccountBook
{
    partial class HisUserAccountBookGet : EntityBase
    {
        public V_HIS_USER_ACCOUNT_BOOK GetViewById(long id, HisUserAccountBookSO search)
        {
            V_HIS_USER_ACCOUNT_BOOK result = null;
            try
            {
                bool valid = true;
                valid = valid && IsGreaterThanZero(id);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_HIS_USER_ACCOUNT_BOOK.AsQueryable().Where(p => p.ID == id);
                        if (search.listVHisUserAccountBookExpression != null && search.listVHisUserAccountBookExpression.Count > 0)
                        {
                            foreach (var item in search.listVHisUserAccountBookExpression)
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
