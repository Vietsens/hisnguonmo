﻿using Inventec.Core;
using System;
using Inventec.UC.Login.Base;
using Inventec.Common.Logging;

namespace Inventec.Desktop.Common.Token
{
    public sealed class TokenManager
    {
        public delegate void ChangePasswordLogDelegate();
        public delegate void LoginLogDelegate();
        public delegate void SetConsunmerDelegate(string tokenCoce);

        static ChangePasswordLogDelegate changePasswordLog;
        static LoginLogDelegate loginLog;
        static SetConsunmerDelegate setConsunmer;
        static CommonParam param;

        public static void SetDelegate(LoginLogDelegate _loginLog, ChangePasswordLogDelegate _changePasswordLog, SetConsunmerDelegate _setConsunmer)
        {
            loginLog = _loginLog;
            changePasswordLog = _changePasswordLog;
            setConsunmer = _setConsunmer;
        }

        public static bool ChangePassword(string prePassword, string newPassword)
        {
            bool success = false;
            try
            {
                param = new CommonParam();
                success = ClientTokenManagerStore.ClientTokenManager.ChangePassword(param, prePassword, newPassword);
                if (success)
                {
                    if (changePasswordLog != null)
                        changePasswordLog();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Doi mat khau that bai. ");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }

            return success;
        }

        public static bool Login(string loginName, string password)
        {
            Inventec.Token.Core.TokenData token = null;
            bool result = false;
            try
            {
                param = new CommonParam();
                token = ClientTokenManagerStore.ClientTokenManager.Login(param, loginName, password);
                if (token != null)
                {

                    if (loginLog != null) loginLog();
                    if (setConsunmer != null) setConsunmer(token.TokenCode);
                    result = true;
                }
                else
                {
                    token = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public static bool Logout()
        {
            bool result = true;
            try
            {
                param = new CommonParam();
                Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.Logout(param);
            }
            catch (Exception ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }

            return result;
        }

        public static bool Renew()
        {
            bool result = true;
            try
            {
                param = new CommonParam();
                var token = ClientTokenManagerStore.ClientTokenManager.Renew(param);
                if (token != null)
                {
                    if (setConsunmer != null) setConsunmer(token.TokenCode);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Goi api renew token that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }

            return result;
        }

        public static bool ValidExpriedTimeProcessor()
        {
            bool result = true;
            try
            {
                param = new CommonParam();
                var tokenData = ClientTokenManagerStore.ClientTokenManager.Init(param);
                if (tokenData != null)
                {
                }
            }
            catch (Exception ex)
            {
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }

            return result;
        }
    }
}
