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
using Inventec.Common.WebApiClient;
using Inventec.Core;
using Inventec.Token.ClientSystem;
using Inventec.Token.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WebApiClient
{
    public class ApiConsumerWrapper
    {
        private ApiConsumer consumer;
        private ClientTokenManager clientTokenManager;
        private string loginName;
        private string password;
        private string appCode;
        private string acsAdress;
        private string baseAdress;
        private bool reAuthorizeAttempt;
        private bool loginSuccess = false;

        public ApiConsumerWrapper(bool reAuthorizeAttempt, string baseUri, string applicationCode, ClientTokenManager clientTokenManager)
        {
            this.clientTokenManager = clientTokenManager;
            this.baseAdress = baseUri;
            this.appCode = applicationCode;
            this.loginSuccess = true;
            this.InitApiConsumer();
        }

        public void UseRegistry(bool useRegistry)
        {
            if (this.clientTokenManager != null)
            {
                this.clientTokenManager.UseRegistry(useRegistry);
            }
        }

        public ApiConsumerWrapper(bool reAuthorizeAttempt, string applicationCode, CommonParam commonParam, string baseUri, string acsUri, string loginName, string password)
        {
            this.Init(reAuthorizeAttempt, applicationCode, commonParam, baseUri, acsUri, loginName, password);
        }

        public ApiConsumerWrapper(bool reAuthorizeAttempt, string applicationCode, string baseUri, string acsUri, string loginName, string password)
        {
            this.Init(reAuthorizeAttempt, applicationCode, new CommonParam(), baseUri, acsUri, loginName, password);
        }

        private void Init(bool reAuthorizeAttempt, string applicationCode, CommonParam commonParam, string baseUri, string acsUri, string loginName, string password)
        {
            this.reAuthorizeAttempt = reAuthorizeAttempt;
            this.loginName = loginName;
            this.password = password;
            this.acsAdress = acsUri;
            this.baseAdress = baseUri;
            this.appCode = applicationCode;
            this.Login();
        }

        private void InitApiConsumer()
        {
            if (this.clientTokenManager.GetTokenData() != null)
            {
                this.consumer = new ApiConsumer(this.baseAdress, this.clientTokenManager.GetTokenData().TokenCode, this.appCode);
                this.loginSuccess = true;
                LogSystem.Info(string.Format("Login Sucess. Uri: {0}", this.baseAdress));
            }
            else
            {
                LogSystem.Warn(string.Format("Login fail. Uri: {0}, loginName: {1}, password: ****", this.baseAdress, this.loginName));
            }
        }

        private void Login()
        {
            if (!this.loginSuccess)
            {
                this.clientTokenManager = new ClientTokenManager(this.appCode, this.acsAdress);
                this.clientTokenManager.UseRegistry(false);
                this.clientTokenManager.Login(new CommonParam(), this.loginName, this.password);
                this.InitApiConsumer();
            }
        }

        public T Post<T>(bool firstAttempt, string uri, CommonParam param, object filter, params object[] listParam)
        {
            try
            {
                this.Login();
                ApiResultObject<T> result = this.consumer.Post<ApiResultObject<T>>(uri, param, filter, listParam);
                if (result != null)
                {
                    this.AddParam(result.Param, param);
                    return result.Data;
                }
            }
            catch (ApiException ex)
            {
                LogSystem.Warn(ex);
                if (this.ExceptionHandler(ex, param, ref firstAttempt))
                {
                    return this.Post<T>(firstAttempt, uri, param, filter, listParam);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return default(T);
        }

        public T Get<T>(bool firstAttempt, string uri, CommonParam param, object filter, params object[] listParam)
        {
            try
            {
                this.Login();
                ApiResultObject<T> result = this.consumer.Get<ApiResultObject<T>>(uri, param, filter, listParam);
                if (result != null)
                {
                    this.AddParam(result.Param, param);
                    return result.Data;
                }
            }
            catch (ApiException ex)
            {
                LogSystem.Warn(ex);
                if (this.ExceptionHandler(ex, param, ref firstAttempt))
                {
                    return this.Get<T>(firstAttempt, uri, param, filter, listParam);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return default(T);
        }

        public void SetBaseUri(string baseUri)
        {
            if (this.consumer != null)
            {
                this.consumer.SetBaseUri(baseUri);
            }
        }

        private void AddParam(CommonParam resultParam, CommonParam param)
        {
            if (resultParam != null)
            {
                param = param != null ? param : new CommonParam();
                if (resultParam.BugCodes != null && param.BugCodes != null)
                {
                    param.BugCodes.AddRange(resultParam.BugCodes);
                }
                else if (resultParam.BugCodes != null && param.BugCodes == null)
                {
                    param.BugCodes = resultParam.BugCodes;
                }
                if (resultParam.Messages != null && param.Messages != null)
                {
                    param.Messages.AddRange(resultParam.Messages);
                }
                else if (resultParam.Messages != null && param.Messages == null)
                {
                    param.Messages = resultParam.Messages;
                }
            }
        }

        private bool ExceptionHandler(ApiException ex, CommonParam param, ref bool firstAttempt)
        {
            if (ex != null && firstAttempt && reAuthorizeAttempt)
            {
                firstAttempt = false;//tranh vong lap vo han
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    lock (this.clientTokenManager)
                    {
                        Inventec.Token.Core.TokenData token = null;
                        if (!string.IsNullOrEmpty(this.loginName) && !string.IsNullOrEmpty(this.password))
                        {
                            token = this.clientTokenManager.Login(param, this.loginName, this.password);
                        }
                        if (token != null)
                        {
                            this.consumer.SetTokenCode(token.TokenCode);
                            this.loginSuccess = true;
                            return true;
                        }
                    }
                }
            }
            this.loginSuccess = false;
            return false;
        }

        public bool LoginSuccess { get { return this.loginSuccess; } }

        public string GetTokenCode()
        {
            return (this.loginSuccess ? this.clientTokenManager.GetTokenData().TokenCode : null);
        }
    }
}
