﻿using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Process
{
    internal abstract class BusinessBase : EntityBase
    {
        public BusinessBase()
            : base()
        {
            param = new CommonParam();
            try
            {
                UserName = TokenClient.clientTokenManager.GetLoginName();
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
                UserName = TokenClient.clientTokenManager.GetLoginName();
            }
            catch (Exception)
            {
            }
        }

        protected CommonParam param { get; set; }

        protected void TroubleCheck()
        {
            try
            {
                if (param.HasException || (param.BugCodes != null && param.BugCodes.Count > 0))
                {
                    MethodName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
                    TroubleCache.Add(GetInfoProcess() + (param.HasException ? "param.HasException." : "") + param.GetBugCode());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void TroubleCheck(object data)
        {
            try
            {
                MethodName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
                if (param.HasException || (data == null) || (data is Boolean && (bool)data == false))
                {
                    TroubleCache.Add(GetInfoProcess() + (param.HasException ? "param.HasException." : "") + param.GetBugCode());
                    //MessageUtil.SetParamFirstPostion(param, LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai);
                    Inventec.Common.Logging.LogUtil.LogActionFail(this.GetType().Name, MethodName, TokenClient.clientTokenManager.GetLoginName());
                }
                else
                {
                    //MessageUtil.SetParamFirstPostion(param, LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThanhCong);
                    Inventec.Common.Logging.LogUtil.LogActionSuccess(this.GetType().Name, MethodName, TokenClient.clientTokenManager.GetLoginName());
                }
                if ((param.BugCodes != null && param.BugCodes.Count > 0) || (param.Messages != null && param.Messages.Count > 0))
                {
                    Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param), LogType.Info);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ApiResultObject<T> PackCollectionResult<T>(T listData)
        {
            ApiResultObject<T> result = new ApiResultObject<T>();
            try
            {
                result.SetValue(listData, listData != null, param);
            }
            catch (Exception ex)
            {
                Logging("Co exception khi dong goi ApiResultObject.", LogType.Error);
                LogSystem.Error(ex);
            }
            return result;
        }

        public ApiResultObject<T> PackSingleResult<T>(T data)
        {
            ApiResultObject<T> result = new ApiResultObject<T>();
            try
            {
                result.SetValue(data, data != null, param);
            }
            catch (Exception ex)
            {
                Logging("Co exception khi dong goi ApiResultObject.", LogType.Error);
                LogSystem.Error(ex);
            }
            return result;
        }

        public static bool TokenCheck()
        {
            bool result = false;
            try
            {
                var tokenData = TokenClient.clientTokenManager.GetTokenData();
                if (tokenData != null && tokenData.ExpireTime <= DateTime.Now.AddMinutes(-1))
                {
                    result = new TokenManager().Renew();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }
    }
}
