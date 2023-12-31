using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace MOS.DAO.HisMedicineType
{
    public partial class HisMedicineTypeDAO : EntityBase
    {
        public List<V_HIS_MEDICINE_TYPE> GetView(HisMedicineTypeSO search, CommonParam param)
        {
            List<V_HIS_MEDICINE_TYPE> result = new List<V_HIS_MEDICINE_TYPE>();

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

        public HIS_MEDICINE_TYPE GetByCode(string code, HisMedicineTypeSO search)
        {
            HIS_MEDICINE_TYPE result = null;

            try
            {
                result = GetWorker.GetByCode(code, search);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }

            return result;
        }

        public V_HIS_MEDICINE_TYPE GetViewById(long id, HisMedicineTypeSO search)
        {
            V_HIS_MEDICINE_TYPE result = null;

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

        public V_HIS_MEDICINE_TYPE GetViewByCode(string code, HisMedicineTypeSO search)
        {
            V_HIS_MEDICINE_TYPE result = null;

            try
            {
                result = GetWorker.GetViewByCode(code, search);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public Dictionary<string, HIS_MEDICINE_TYPE> GetDicByCode(HisMedicineTypeSO search, CommonParam param)
        {
            Dictionary<string, HIS_MEDICINE_TYPE> result = new Dictionary<string, HIS_MEDICINE_TYPE>();
            try
            {
                result = GetWorker.GetDicByCode(search, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result.Clear();
            }

            return result;
        }

        public List<V_HIS_MEDICINE_TYPE_1> GetView1(HisMedicineTypeSO search, CommonParam param)
        {
            List<V_HIS_MEDICINE_TYPE_1> result = new List<V_HIS_MEDICINE_TYPE_1>();

            try
            {
                result = GetWorker.GetView1(search, param);
            }
            catch (Exception ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
                result.Clear();
            }

            return result;
        }

        public V_HIS_MEDICINE_TYPE_1 GetView1ById(long id, HisMedicineTypeSO search)
        {
            V_HIS_MEDICINE_TYPE_1 result = null;

            try
            {
                result = GetWorker.GetView1ById(id, search);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }

            return result;
        }

        public V_HIS_MEDICINE_TYPE_1 GetView1ByCode(string code, HisMedicineTypeSO search)
        {
            V_HIS_MEDICINE_TYPE_1 result = null;

            try
            {
                result = GetWorker.GetView1ByCode(code, search);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool ExistsCode(string code, long? id)
        {
            try
            {
                return CheckWorker.ExistsCode(code, id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                throw;
            }
        }
    }
}
