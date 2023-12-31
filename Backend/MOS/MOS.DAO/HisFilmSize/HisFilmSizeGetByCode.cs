using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisFilmSize
{
    partial class HisFilmSizeGet : EntityBase
    {
        public HIS_FILM_SIZE GetByCode(string code, HisFilmSizeSO search)
        {
            HIS_FILM_SIZE result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.HIS_FILM_SIZE.AsQueryable().Where(p => p.FILM_SIZE_CODE == code);
                        if (search.listHisFilmSizeExpression != null && search.listHisFilmSizeExpression.Count > 0)
                        {
                            foreach (var item in search.listHisFilmSizeExpression)
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
