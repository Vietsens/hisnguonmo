using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisSereServPtttTemp
{
    partial class HisSereServPtttTempGet : EntityBase
    {
        public List<HIS_SERE_SERV_PTTT_TEMP> Get(HisSereServPtttTempSO search, CommonParam param)
        {
            List<HIS_SERE_SERV_PTTT_TEMP> list = new List<HIS_SERE_SERV_PTTT_TEMP>();
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.HIS_SERE_SERV_PTTT_TEMP.AsNoTracking().AsQueryable();
                        if (search.listHisSereServPtttTempExpression != null && search.listHisSereServPtttTempExpression.Count > 0)
                        {
                            foreach (var item in search.listHisSereServPtttTempExpression)
                            {
                                query = query.Where(item);
                            }
                        }
                        
                        if (!string.IsNullOrWhiteSpace(search.OrderField) && !string.IsNullOrWhiteSpace(search.OrderDirection))
						{
							if (!param.Start.HasValue || !param.Limit.HasValue)
							{
								list = query.OrderByProperty(search.OrderField, search.OrderDirection).ToList(); 
							}
							else
							{
								param.Count = (from r in query select r).Count();
								query = query.OrderByProperty(search.OrderField, search.OrderDirection);
								if (param.Count <= param.Limit.Value && param.Start.Value == 0)
								{
									list = query.ToList();
								}
								else
								{
									list = query.Skip(param.Start.Value).Take(param.Limit.Value).ToList();
								}
							}
						}
						else
						{
							list = query.ToList();
						}
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

        public HIS_SERE_SERV_PTTT_TEMP GetById(long id, HisSereServPtttTempSO search)
        {
            HIS_SERE_SERV_PTTT_TEMP result = null;
            try
            {
                bool valid = true;
                valid = valid && IsGreaterThanZero(id);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.HIS_SERE_SERV_PTTT_TEMP.AsQueryable().Where(p => p.ID == id);
                        if (search.listHisSereServPtttTempExpression != null && search.listHisSereServPtttTempExpression.Count > 0)
                        {
                            foreach (var item in search.listHisSereServPtttTempExpression)
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
