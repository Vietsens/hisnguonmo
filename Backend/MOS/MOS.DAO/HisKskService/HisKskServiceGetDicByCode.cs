using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisKskService
{
    partial class HisKskServiceGet : EntityBase
    {
        public Dictionary<string, HIS_KSK_SERVICE> GetDicByCode(HisKskServiceSO search, CommonParam param)
        {
            Dictionary<string, HIS_KSK_SERVICE> dic = new Dictionary<string, HIS_KSK_SERVICE>();
            try
            {
                List<HIS_KSK_SERVICE> listRecord = Get(search, param);
                if (listRecord != null)
                {
                    foreach (var item in listRecord)
                    {
                        if (!dic.ContainsKey(item.KSK_SERVICE_CODE))
                        {
                            dic.Add(item.KSK_SERVICE_CODE, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param), LogType.Error);
                LogSystem.Error(ex);
                dic.Clear();
            }
            return dic;
        }
    }
}
