/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.Core;
using System;

namespace His.UC.UCHein
{
    public abstract class BusinessBase : EntityBase
    {
        protected CommonParam param { get; set; }

        public BusinessBase()
            : base()
        {
            param = new CommonParam();
            try
            {
                
            }
            catch (Exception)
            {
            }
        }

        public BusinessBase(CommonParam paramBusiness)
            : base()
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
            try
            {
               
            }
            catch (Exception)
            {
            }
        }

        public bool HasException()
        {
            return param.HasException;
        }

        public void CopyCommonParamInfoGet(CommonParam paramSource)
        {
            try
            {
                param.Start = paramSource.Start;
                param.Limit = paramSource.Limit;
                param.Count = paramSource.Count;
                param.HasException = paramSource.HasException;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void CopyCommonParamInfo(CommonParam paramSource)
        {
            try
            {
                if (paramSource.BugCodes != null && paramSource.BugCodes.Count > 0) param.BugCodes.AddRange(paramSource.BugCodes);
                if (paramSource.Messages != null && paramSource.Messages.Count > 0) param.Messages.AddRange(paramSource.Messages);
                param.Start = paramSource.Start;
                param.Limit = paramSource.Limit;
                param.Count = paramSource.Count;
                param.HasException = paramSource.HasException;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void CopyCommonParamInfo(BusinessBase fromObject)
        {
            try
            {
                if (fromObject.param != null)
                {
                    if (fromObject.param.BugCodes != null && fromObject.param.BugCodes.Count > 0) param.BugCodes.AddRange(fromObject.param.BugCodes);
                    if (fromObject.param.Messages != null && fromObject.param.Messages.Count > 0) param.Messages.AddRange(fromObject.param.Messages);
                    param.Start = fromObject.param.Start;
                    param.Limit = fromObject.param.Limit;
                    param.Count = fromObject.param.Count;
                    param.HasException = fromObject.param.HasException;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected ApiResultObject<T> PackResult<T>(T resultData)
        {
            ApiResultObject<T> result = new ApiResultObject<T>();
            try
            {
                result.SetValue(resultData, Inventec.Core.Util.DecisionApiResult(resultData), param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                result = new ApiResultObject<T>(default(T), false);
            }
            return result;
        }
    }
}
