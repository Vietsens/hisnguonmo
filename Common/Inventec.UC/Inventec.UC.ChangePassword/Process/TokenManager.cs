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
using Inventec.Common.WebApiClient;
using Inventec.Core;
using Inventec.UC.ChangePassword.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Process
{
    class TokenManager : BusinessBase
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
                //ApiConsumerStore.MosConsumer.SetTokenCode(tokenCode);
                ApiConsumerStore.SdaConsumer.SetTokenCode(tokenCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ChangePassword(string prePassword, string newPassword)
        {
            bool success = false;
            try
            {
                success = TokenClient.clientTokenManager.ChangePassword(param, prePassword, newPassword);
                if (success)
                {
                    ChangePasswordSuccessLog();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Doi mat khau that bai. ");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }

            TroubleCheck(success);
            return success;
        }

        private static void ChangePasswordSuccessLog()
        {
            try
            {
                string message = EventLogUtil.SetLog(EventLog.EventLog.Enum.Action_DoiMatKhauThanhCong);
                EventLogWorker.Creator.Create(null, true, message);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool Renew()
        {
            bool result = true;
            try
            {
                var token = TokenClient.clientTokenManager.Renew(param);
                if (token != null)
                {
                    SetConsunmer(token.TokenCode);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Goi api renew token that bai. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }

            return result;
        }
    }
}
