using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisRehaTrainUnit
{
    partial class HisRehaTrainUnitGet : EntityBase
    {
        public HIS_REHA_TRAIN_UNIT GetByCode(string code, HisRehaTrainUnitSO search)
        {
            HIS_REHA_TRAIN_UNIT result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.HIS_REHA_TRAIN_UNIT.AsQueryable().Where(p => p.REHA_TRAIN_UNIT_CODE == code);
                        if (search.listHisRehaTrainUnitExpression != null && search.listHisRehaTrainUnitExpression.Count > 0)
                        {
                            foreach (var item in search.listHisRehaTrainUnitExpression)
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
