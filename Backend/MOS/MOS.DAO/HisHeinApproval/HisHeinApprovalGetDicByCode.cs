using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisHeinApproval
{
    partial class HisHeinApprovalGet : EntityBase
    {
        public Dictionary<string, HIS_HEIN_APPROVAL> GetDicByCode(HisHeinApprovalSO search, CommonParam param)
        {
            Dictionary<string, HIS_HEIN_APPROVAL> dic = new Dictionary<string, HIS_HEIN_APPROVAL>();
            try
            {
                List<HIS_HEIN_APPROVAL> listRecord = Get(search, param);
                if (listRecord != null)
                {
                    foreach (var item in listRecord)
                    {
                        if (!dic.ContainsKey(item.HEIN_APPROVAL_CODE))
                        {
                            dic.Add(item.HEIN_APPROVAL_CODE, item);
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
