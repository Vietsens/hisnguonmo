using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace MOS.DAO.HisLocationStore
{
    public partial class HisLocationStoreDAO : EntityBase
    {
        public List<V_HIS_LOCATION_STORE> GetView(HisLocationStoreSO search, CommonParam param)
        {
            List<V_HIS_LOCATION_STORE> result = new List<V_HIS_LOCATION_STORE>();
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

        public V_HIS_LOCATION_STORE GetViewById(long id, HisLocationStoreSO search)
        {
            V_HIS_LOCATION_STORE result = null;

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
