using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace MOS.DAO.HisTransactionExp
{
    public partial class HisTransactionExpDAO : EntityBase
    {
        public List<V_HIS_TRANSACTION_EXP> GetView(HisTransactionExpSO search, CommonParam param)
        {
            List<V_HIS_TRANSACTION_EXP> result = new List<V_HIS_TRANSACTION_EXP>();
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

        public V_HIS_TRANSACTION_EXP GetViewById(long id, HisTransactionExpSO search)
        {
            V_HIS_TRANSACTION_EXP result = null;

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
