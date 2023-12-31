using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisSereServDebt
{
    partial class HisSereServDebtGet : EntityBase
    {
        public HIS_SERE_SERV_DEBT GetByCode(string code, HisSereServDebtSO search)
        {
            HIS_SERE_SERV_DEBT result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.HIS_SERE_SERV_DEBT.AsQueryable().Where(p => p.SERE_SERV_DEBT_CODE == code);
                        if (search.listHisSereServDebtExpression != null && search.listHisSereServDebtExpression.Count > 0)
                        {
                            foreach (var item in search.listHisSereServDebtExpression)
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
