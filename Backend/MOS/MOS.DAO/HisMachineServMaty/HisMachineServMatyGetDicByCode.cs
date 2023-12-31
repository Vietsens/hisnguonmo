using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisMachineServMaty
{
    partial class HisMachineServMatyGet : EntityBase
    {
        public Dictionary<string, HIS_MACHINE_SERV_MATY> GetDicByCode(HisMachineServMatySO search, CommonParam param)
        {
            Dictionary<string, HIS_MACHINE_SERV_MATY> dic = new Dictionary<string, HIS_MACHINE_SERV_MATY>();
            try
            {
                List<HIS_MACHINE_SERV_MATY> listRecord = Get(search, param);
                if (listRecord != null)
                {
                    foreach (var item in listRecord)
                    {
                        if (!dic.ContainsKey(item.MACHINE_SERV_MATY_CODE))
                        {
                            dic.Add(item.MACHINE_SERV_MATY_CODE, item);
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
