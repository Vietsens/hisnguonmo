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
using Inventec.Common.Logging;
using Inventec.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Inventec.Common.Adapter
{
    public abstract class AdapterBase : EntityBaseAdapter
    {

        public AdapterBase()
            : base()
        {
            param = new CommonParam();
        }

        public AdapterBase(CommonParam paramBusiness)
            : base()
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public static void SetDelegate(Action _actProcessSessionLogout)
        {
            actProcessSessionLogout = _actProcessSessionLogout;
        }

        #region Get
        public T Get<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Get<T>(requestUri, consumer, commonParam, filter, 0, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Get<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, int userTimeout, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Get<T>(requestUri, consumer, commonParam, filter, userTimeout, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Get<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Get<T>(requestUri, consumer, commonParam, filter, 0, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Get<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, int userTimeout, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Get<T>(requestUri, consumer, commonParam, filter, userTimeout, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Get<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Get<T>(requestUri, consumer, commonParam, filter, 0, action, listParam);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Get<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, int userTimeout, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                Inventec.Core.ApiResultObject<T> rs = null;
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = consumer.Get<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout, listParam);
                }
                else
                {
                    rs = consumer.Get<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout);
                }
                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(requestUri + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
                       + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commonParam), commonParam));
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                StatusCode = ex.StatusCode.GetHashCode().ToString();
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(requestUri + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
                       + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commonParam), commonParam));
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(requestUri + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commonParam), commonParam));
                LogSystem.Error(ex);
            }
            return result;
        }

        public T GetWithBug<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                Inventec.Core.ApiResultObject<T> rs = null;
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, 0, null, null);

                rs = consumer.Get<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, 0);
                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(requestUri + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
                       + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commonParam), commonParam));
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                StatusCode = ex.StatusCode.GetHashCode().ToString();
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(requestUri + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
                       + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commonParam), commonParam));
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                param.BugCodes.Add(AdapterConfig.BUG_CODE__CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(requestUri + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commonParam), commonParam));
                LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region GetAsync
        public async Task<T> GetAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await GetAsync<T>(requestUri, consumer, commonParam, filter, 0, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> GetAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, int userTimeout, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await GetAsync<T>(requestUri, consumer, commonParam, filter, userTimeout, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> GetAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await GetAsync<T>(requestUri, consumer, commonParam, filter, 0, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> GetAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, int userTimeout, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await GetAsync<T>(requestUri, consumer, commonParam, filter, userTimeout, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> GetAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await GetAsync<T>(requestUri, consumer, commonParam, filter, 0, action, listParam);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> GetAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, int userTimeout, Action action, params object[] listParam)
        {
            T result = default(T);
            Inventec.Core.ApiResultObject<T> rs = null;
            try
            {
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = await consumer.GetAsync<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout, listParam).ConfigureAwait(false);
                }
                else
                {
                    rs = await consumer.GetAsync<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout).ConfigureAwait(false);
                }

                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                    else
                    {
                        LogInOut("", LogType.Error);
                    }
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<Inventec.Core.ApiResultObject<T>> GetAsyncRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, int userTimeout, Action action, params object[] listParam)
        {
            Inventec.Core.ApiResultObject<T> rs = null;
            try
            {
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = await consumer.GetAsync<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout, listParam).ConfigureAwait(false);
                }
                else
                {
                    rs = await consumer.GetAsync<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout).ConfigureAwait(false);
                }

                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return rs;
        }
        #endregion

        #region GetStrong
        public T GetStrong<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, int userTimeout, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                Inventec.Core.ApiResultObject<T> rs = null;
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, userTimeout, action, listParam);

                rs = consumer.GetStrong<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout, listParam);
                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> GetStrongRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, int userTimeout, Action action, params object[] listParam)
        {
            Inventec.Core.ApiResultObject<T> rs = null;
            try
            {
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, userTimeout, action, listParam);

                rs = consumer.GetStrong<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout, listParam);
                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return rs;
        }

        public async Task<T> GetStrongAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, int userTimeout, Action action, params object[] listParam)
        {
            T result = default(T);
            Inventec.Core.ApiResultObject<T> rs = null;
            try
            {
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = await consumer.GetStrongAsync<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout, listParam).ConfigureAwait(false);
                }
                else
                {
                    rs = await consumer.GetStrongAsync<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout).ConfigureAwait(false);
                }

                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
        #endregion

        #region GetRO
        public Inventec.Core.ApiResultObject<T> GetRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, CommonParam commonParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = GetRO<T>(requestUri, consumer, commonParam, filter, 0, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> GetRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, int userTimeout, CommonParam commonParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = GetRO<T>(requestUri, consumer, commonParam, filter, userTimeout, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> GetRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, Action action, CommonParam commonParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = GetRO<T>(requestUri, consumer, commonParam, filter, 0, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> GetRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, int userTimeout, Action action, CommonParam commonParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = GetRO<T>(requestUri, consumer, commonParam, filter, userTimeout, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> GetRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, Action action, params object[] listParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = GetRO<T>(requestUri, consumer, commonParam, filter, 0, action, listParam);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> GetRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, int userTimeout, Action action, params object[] listParam)
        {
            Inventec.Core.ApiResultObject<T> rs = null;
            try
            {
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = consumer.Get<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout, listParam);
                }
                else
                {
                    rs = consumer.Get<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout);
                }

                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return rs;
        }

        #endregion

        #region GetZip
        public T GetZip<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = GetZip<T>(requestUri, consumer, commonParam, filter, 0, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T GetZip<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object filter, int userTimeout, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = GetZip<T>(requestUri, consumer, commonParam, filter, userTimeout, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T GetZip<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object filter, int userTimeout, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                Inventec.Core.ApiResultObject<T> rs = null;
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, filter, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = consumer.GetZip<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout, listParam);
                }
                else
                {
                    rs = consumer.GetZip<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, filter, userTimeout);
                }
                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
        #endregion

        #region Post
        public T Post<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Post<T>(requestUri, consumer, commonParam, data, 0, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Post<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, int userTimeout, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Post<T>(requestUri, consumer, commonParam, data, userTimeout, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Post<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Post<T>(requestUri, consumer, commonParam, data, 0, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Post<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, int userTimeout, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Post<T>(requestUri, consumer, commonParam, data, userTimeout, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Post<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = Post<T>(requestUri, consumer, commonParam, data, 0, action, listParam);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T Post<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, int userTimeout, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                Inventec.Core.ApiResultObject<T> rs = null;
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, data, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = consumer.Post<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, data, userTimeout, listParam);
                }
                else
                {
                    rs = consumer.Post<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, data, userTimeout);
                }

                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T PostWithBug<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                Inventec.Core.ApiResultObject<T> rs = null;
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, data, 0, null, null);

                rs = consumer.Post<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, data, 0);

                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                param.BugCodes.Add(AdapterConfig.BUG_CODE__CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region PostRO
        public Inventec.Core.ApiResultObject<T> PostRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, CommonParam commonParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = PostRO<T>(requestUri, consumer, commonParam, data, 0, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> PostRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, int userTimeout, CommonParam commonParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = PostRO<T>(requestUri, consumer, commonParam, data, userTimeout, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> PostRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, Action action, CommonParam commonParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = PostRO<T>(requestUri, consumer, commonParam, data, 0, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> PostRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, Action action, params object[] listParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = PostRO<T>(requestUri, consumer, commonParam, data, 0, action, listParam);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> PostRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, int userTimeout, Action action, CommonParam commonParam)
        {
            Inventec.Core.ApiResultObject<T> result = null;
            try
            {
                FrameIndex = 1;
                result = PostRO<T>(requestUri, consumer, commonParam, data, userTimeout, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public Inventec.Core.ApiResultObject<T> PostRO<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, int userTimeout, Action action, params object[] listParam)
        {
            Inventec.Core.ApiResultObject<T> rs = null;
            try
            {
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, data, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = consumer.Post<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, data, userTimeout, listParam);
                }
                else
                {
                    rs = consumer.Post<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, data, userTimeout);
                }

                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return rs;
        }

        #endregion

        #region PostAsync
        public async Task<T> PostAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await PostAsync<T>(requestUri, consumer, commonParam, data, 0, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> PostAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, int userTimeout, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await PostAsync<T>(requestUri, consumer, commonParam, data, userTimeout, null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> PostAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await PostAsync<T>(requestUri, consumer, commonParam, data, 0, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> PostAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, int userTimeout, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await PostAsync<T>(requestUri, consumer, commonParam, data, userTimeout, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> PostAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = await PostAsync<T>(requestUri, consumer, commonParam, data, 0, action, listParam);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<T> PostAsync<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, int userTimeout, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                Inventec.Core.ApiResultObject<T> rs = null;
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, data, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    rs = await consumer.PostAsync<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, data, userTimeout, listParam);
                }
                else
                {
                    rs = await consumer.PostAsync<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, data, userTimeout);
                }

                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region PostWithouApiParam
        public T PostWithouApiParam<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, Action action, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = PostWithouApiParam<T>(requestUri, consumer, commonParam, data, 0, action, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T PostWithouApiParam<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, int userTimeout, Action action, params object[] listParam)
        {
            T result = default(T);
            try
            {
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, data, userTimeout, action, listParam);

                if (listParam != null && listParam.Length > 0)
                {
                    result = consumer.PostWithouApiParam<T>(requestUri, data, userTimeout, listParam);
                }
                else
                {
                    result = consumer.PostWithouApiParam<T>(requestUri, data, userTimeout);
                }

                if (result == null)
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
        #endregion

        #region PostWithFile
        public T PostWithFile<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, List<FileHolder> Files, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = PostWithFile<T>(requestUri, consumer, commonParam, data, 0, null, Files);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T PostWithFile<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, object data, Action action, List<FileHolder> Files, CommonParam commonParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = PostWithFile<T>(requestUri, consumer, commonParam, data, 0, action, Files);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T PostWithFile<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, Action action, List<FileHolder> listParam)
        {
            T result = default(T);
            try
            {
                FrameIndex = 1;
                result = PostWithFile<T>(requestUri, consumer, commonParam, data, 0, action, listParam);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        public T PostWithFile<T>(string requestUri, Inventec.Common.WebApiClient.ApiConsumer consumer, CommonParam commonParam, object data, int userTimeout, Action action, List<FileHolder> listParam)
        {
            T result = default(T);
            try
            {
                Inventec.Core.ApiResultObject<T> rs = null;
                if (commonParam != null)
                    commonParam.LanguageCode = AdapterConfig.LanguageCode;

                SetLogInputWithParam(requestUri, consumer, commonParam, data, userTimeout, action, listParam);

                rs = consumer.PostWithFile<Inventec.Core.ApiResultObject<T>>(requestUri, commonParam, data, listParam, userTimeout);
                if (rs != null)
                {
                    if (rs.Param != null)
                    {
                        SetCommonParam(rs.Param);
                    }
                    result = (rs.Data);
                }

                LogInOut<T>(rs);
            }
            catch (Inventec.Common.WebApiClient.ApiException ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex.StatusCode), ex.StatusCode));
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    param.Messages.Add(AdapterConfig.STR_SESSION_TIMEOUT);
                    param.HasException = true;
                    if (action != null)
                    {
                        action();
                    }
                    else if (actProcessSessionLogout != null)
                    {
                        actProcessSessionLogout();
                    }
                }
                else
                {
                    LogInOut("", LogType.Error);
                }
            }
            catch (AggregateException ex)
            {
                LogSystem.Error(ex);
                param.Messages.Add(AdapterConfig.STR_CANNOT_CONNECT_TO_SERVER);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
        #endregion

    }
}
