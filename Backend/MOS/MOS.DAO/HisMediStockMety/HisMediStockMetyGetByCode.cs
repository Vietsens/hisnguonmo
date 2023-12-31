using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisMediStockMety
{
    partial class HisMediStockMetyGet : EntityBase
    {
        public HIS_MEDI_STOCK_METY GetByCode(string code, HisMediStockMetySO search)
        {
            HIS_MEDI_STOCK_METY result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.HIS_MEDI_STOCK_METY.AsQueryable().Where(p => p.MEDI_STOCK_METY_CODE == code);
                        if (search.listHisMediStockMetyExpression != null && search.listHisMediStockMetyExpression.Count > 0)
                        {
                            foreach (var item in search.listHisMediStockMetyExpression)
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
