using Inventec.Common.WebApiClient;
using Inventec.Core;
using Inventec.UC.Login.UCD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Base
{
    public sealed class TokenManager : BusinessBase
    {
        public TokenManager()
            : base()
        {

        }

        public TokenManager(CommonParam param)
            : base(param)
        {

        }

        public void SetConsunmer(string tokenCode)
        {
            try
            {
                ApiConsumerStore.SdaConsumer.SetTokenCode(tokenCode);
                if (ApiConsumerStore.MosConsumer != null)
                {
                    ApiConsumerStore.MosConsumer.SetTokenCode(tokenCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool Login(string loginName, string password)
        {
            Inventec.Token.Core.TokenData token = null;
            bool result = false;
            try
            {
                string appVersion = "";
                string appVersionFilePath = System.Windows.Forms.Application.StartupPath + "\\readme.txt";
                if (File.Exists(appVersionFilePath))
                {
                    appVersion = System.IO.File.ReadAllText(appVersionFilePath);
                }
                token = ClientTokenManagerStore.ClientTokenManager.Login(param, loginName, password, appVersion);
                if (token != null)
                {
                    SetConsunmer(token.TokenCode);
                    //LoginSuccessLog(loginName);
                    result = true;
                }
                else
                {
                    token = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool Logout()
        {
            bool result = true;
            try
            {
                ClientTokenManagerStore.ClientTokenManager.Logout(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }

            return result;
        }

        private static void LoginSuccessLog(string loginName)
        {
            try
            {
                //string message = EventLogUtil.SetLog(EventLog.EventLog.Enum.Action_DangNhapThanhCong);
                string message = string.Format(EventLogUtil.SetLog(EventLog.EventLog.Enum.Action_DangNhapThanhCong), AppConfig.APPLICATION_CODE);
                EventLogWorker.Creator.Create(loginName, null, true, message);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
