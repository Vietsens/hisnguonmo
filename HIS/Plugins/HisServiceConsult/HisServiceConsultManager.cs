using Inventec.Core;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.MANAGER.Base;
using MOS.SDO;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisServiceConsult
{
    public partial class HisServiceConsultManager : BusinessBase
    {
        public HisServiceConsultManager()
            : base()
        {

        }
        
        public HisServiceConsultManager(CommonParam param)
            : base(param)
        {

        }
		
		[Logger]
        public ApiResultObject<List<HIS_SERVICE_CONSULT>> Get(HisServiceConsultFilterQuery filter)
        {
            ApiResultObject<List<HIS_SERVICE_CONSULT>> result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                valid = valid && IsNotNull(filter);
                List<HIS_SERVICE_CONSULT> resultData = null;
                if (valid)
                {
                    resultData = new HisServiceConsultGet(param).Get(filter);
                }
                result = this.PackSuccess(resultData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                result = null;
            }
            return result;
        }

		[Logger]
        public ApiResultObject<HisServiceConsultSDO> GetByTreatment(long treatmentId)
        {
            ApiResultObject<HisServiceConsultSDO> result = new ApiResultObject<HisServiceConsultSDO>(null);
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                HisServiceConsultSDO resultData = null;
                if (valid)
                {
                    resultData = new HisServiceConsultGetByTreatment(param).GetByTreatment(treatmentId);
                }
                result = this.PackSuccess(resultData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                result = null;
            }
            return result;
        }

		[Logger]
        public ApiResultObject<HIS_SERVICE_CONSULT> Create(HIS_SERVICE_CONSULT data)
        {
            ApiResultObject<HIS_SERVICE_CONSULT> result = new ApiResultObject<HIS_SERVICE_CONSULT>(null);
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                valid = valid && IsNotNull(data);
                HIS_SERVICE_CONSULT resultData = null;
				bool isSuccess = false;
                if (valid)
                {
					isSuccess = new HisServiceConsultCreate(param).Create(data);
                    resultData = isSuccess ? data : null;
                }
                result = this.PackResult(resultData, isSuccess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
            }
            return result;
        }

		[Logger]
        public ApiResultObject<HIS_SERVICE_CONSULT> Update(HIS_SERVICE_CONSULT data)
        {
            ApiResultObject<HIS_SERVICE_CONSULT> result = new ApiResultObject<HIS_SERVICE_CONSULT>(null);
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                valid = valid && IsNotNull(data);
                HIS_SERVICE_CONSULT resultData = null;
				bool isSuccess = false;
                if (valid)
                {
					isSuccess = new HisServiceConsultUpdate(param).Update(data);
                    resultData = isSuccess ? data : null;
                }
                result = this.PackResult(resultData, isSuccess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
            }
            
            return result;
        }

		[Logger]
        public ApiResultObject<HIS_SERVICE_CONSULT> ChangeLock(long id)
        {
            ApiResultObject<HIS_SERVICE_CONSULT> result = new ApiResultObject<HIS_SERVICE_CONSULT>(null);
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                HIS_SERVICE_CONSULT resultData = null;
				bool isSuccess = false;
                if (valid)
                {
                    isSuccess = new HisServiceConsultLock(param).ChangeLock(id, ref resultData);
                }
                result = this.PackResult(resultData, isSuccess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
            }
            
            return result;
        }
		
		[Logger]
        public ApiResultObject<HIS_SERVICE_CONSULT> Lock(long id)
        {
            ApiResultObject<HIS_SERVICE_CONSULT> result = null;
            
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                HIS_SERVICE_CONSULT resultData = null;
				bool isSuccess = false;
                if (valid)
                {
                    isSuccess = new HisServiceConsultLock(param).Lock(id, ref resultData);
                }
                result = this.PackResult(resultData, isSuccess);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
            }

            return result;
        }
		
		[Logger]
        public ApiResultObject<HIS_SERVICE_CONSULT> Unlock(long id)
        {
            ApiResultObject<HIS_SERVICE_CONSULT> result = null;
            
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                HIS_SERVICE_CONSULT resultData = null;
				bool isSuccess = false;
                if (valid)
                {
                    isSuccess = new HisServiceConsultLock(param).Unlock(id, ref resultData);
                }
                result = this.PackResult(resultData, isSuccess);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
            }

            return result;
        }

		[Logger]
        public ApiResultObject<bool> Delete(long id)
        {
            ApiResultObject<bool> result = new ApiResultObject<bool>(false);

            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                bool resultData = false;
                if (valid)
                {
                    resultData = new HisServiceConsultTruncate(param).Truncate(id);
                }
                result = this.PackSingleResult(resultData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
            }
            
            return result;
        }
    }
}
