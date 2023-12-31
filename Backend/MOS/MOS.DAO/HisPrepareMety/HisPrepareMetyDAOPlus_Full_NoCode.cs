using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace MOS.DAO.HisPrepareMety
{
    public partial class HisPrepareMetyDAO : EntityBase
    {
        public List<V_HIS_PREPARE_METY> GetView(HisPrepareMetySO search, CommonParam param)
        {
            List<V_HIS_PREPARE_METY> result = new List<V_HIS_PREPARE_METY>();
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

        public V_HIS_PREPARE_METY GetViewById(long id, HisPrepareMetySO search)
        {
            V_HIS_PREPARE_METY result = null;

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
