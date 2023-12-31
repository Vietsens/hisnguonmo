using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisAssessmentObject
{
    partial class HisAssessmentObjectGet : EntityBase
    {
        public HIS_ASSESSMENT_OBJECT GetByCode(string code, HisAssessmentObjectSO search)
        {
            HIS_ASSESSMENT_OBJECT result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.HIS_ASSESSMENT_OBJECT.AsQueryable().Where(p => p.ASSESSMENT_OBJECT_CODE == code);
                        if (search.listHisAssessmentObjectExpression != null && search.listHisAssessmentObjectExpression.Count > 0)
                        {
                            foreach (var item in search.listHisAssessmentObjectExpression)
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
