using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisSereServ
{
    partial class HisSereServGet : EntityBase
    {
        public List<V_HIS_SERE_SERV_11> GetView11(HisSereServSO search, CommonParam param)
        {
            List<V_HIS_SERE_SERV_11> list = new List<V_HIS_SERE_SERV_11>();
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_HIS_SERE_SERV_11.AsNoTracking().AsQueryable();
                        if (search.listVHisSereServ11Expression != null && search.listVHisSereServ11Expression.Count > 0)
                        {
                            foreach (var item in search.listVHisSereServ11Expression)
                            {
                                query = query.Where(item);
                            }
                        }
                        
                        
                        if (!string.IsNullOrWhiteSpace(search.OrderField) && !string.IsNullOrWhiteSpace(search.OrderDirection)) { if (!param.Start.HasValue || !param.Limit.HasValue) { list = query.OrderByProperty(search.OrderField, search.OrderDirection).ToList(); } else { param.Count = (from r in query select r).Count(); query = query.OrderByProperty(search.OrderField, search.OrderDirection); if (param.Count <= param.Limit.Value && param.Start.Value == 0) { list = query.ToList(); } else { list = query.Skip(param.Start.Value).Take(param.Limit.Value).ToList(); } } } else { list = query.ToList(); }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param), LogType.Error);
                LogSystem.Error(ex);
                list.Clear();
            }
            return list;
        }
    }
}
