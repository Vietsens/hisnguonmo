using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace MOS.DAO.HisBedLog
{
    public partial class HisBedLogDAO : EntityBase
    {
        public List<V_HIS_BED_LOG_3> GetView3(HisBedLogSO search, CommonParam param)
        {
            List<V_HIS_BED_LOG_3> result = new List<V_HIS_BED_LOG_3>();
            try
            {
                result = GetWorker.GetView3(search, param);
            }
            catch (Exception ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
                result.Clear();
            }
            return result;
        }

        public V_HIS_BED_LOG_3 GetView3ById(long id, HisBedLogSO search)
        {
            V_HIS_BED_LOG_3 result = null;

            try
            {
                result = GetWorker.GetView3ById(id, search);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }

            return result;
        }
    }
}
