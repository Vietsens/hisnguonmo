using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace MOS.DAO.HisRehaSum
{
    public partial class HisRehaSumDAO : EntityBase
    {
        public List<V_HIS_REHA_SUM> GetView(HisRehaSumSO search, CommonParam param)
        {
            List<V_HIS_REHA_SUM> result = new List<V_HIS_REHA_SUM>();
            try
            {
                result = GetWorker.GetView(search, param);
            }
            catch (Exception ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
                result.Clear();
            }
            return result;
        }

        public V_HIS_REHA_SUM GetViewById(long id, HisRehaSumSO search)
        {
            V_HIS_REHA_SUM result = null;

            try
            {
                result = GetWorker.GetViewById(id, search);
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
