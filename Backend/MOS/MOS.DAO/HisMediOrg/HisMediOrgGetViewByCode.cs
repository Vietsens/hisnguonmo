using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisMediOrg
{
    partial class HisMediOrgGet : EntityBase
    {
        public V_HIS_MEDI_ORG GetViewByCode(string code, HisMediOrgSO search)
        {
            V_HIS_MEDI_ORG result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_HIS_MEDI_ORG.AsQueryable().Where(p => p.MEDI_ORG_CODE == code);
                        if (search.listVHisMediOrgExpression != null && search.listVHisMediOrgExpression.Count > 0)
                        {
                            foreach (var item in search.listVHisMediOrgExpression)
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
