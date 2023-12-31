using MOS.DAO.Base;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOS.DAO.HisMedicineTypeTut
{
    partial class HisMedicineTypeTutGet : EntityBase
    {
        public Dictionary<string, HIS_MEDICINE_TYPE_TUT> GetDicByCode(HisMedicineTypeTutSO search, CommonParam param)
        {
            Dictionary<string, HIS_MEDICINE_TYPE_TUT> dic = new Dictionary<string, HIS_MEDICINE_TYPE_TUT>();
            try
            {
                List<HIS_MEDICINE_TYPE_TUT> listRecord = Get(search, param);
                if (listRecord != null)
                {
                    foreach (var item in listRecord)
                    {
                        if (!dic.ContainsKey(item.MEDICINE_TYPE_TUT_CODE))
                        {
                            dic.Add(item.MEDICINE_TYPE_TUT_CODE, item);
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
