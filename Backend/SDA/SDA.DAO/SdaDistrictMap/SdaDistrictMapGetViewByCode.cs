using SDA.DAO.Base;
using SDA.DAO.StagingObject;
using SDA.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDA.DAO.SdaDistrictMap
{
    partial class SdaDistrictMapGet : EntityBase
    {
        public V_SDA_DISTRICT_MAP GetViewByCode(string code, SdaDistrictMapSO search)
        {
            V_SDA_DISTRICT_MAP result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_SDA_DISTRICT_MAP.AsQueryable().Where(p => p.DISTRICT_MAP_CODE == code);
                        if (search.listVSdaDistrictMapExpression != null && search.listVSdaDistrictMapExpression.Count > 0)
                        {
                            foreach (var item in search.listVSdaDistrictMapExpression)
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
