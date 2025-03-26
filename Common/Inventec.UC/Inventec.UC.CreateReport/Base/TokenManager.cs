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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Base
{
    internal sealed class TokenManager : BusinessBase
    {
        public static int RowMessageCount { get; set; }

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
                ApiConsumerStore.SarConsumer.SetTokenCode(tokenCode);
                ApiConsumerStore.SdaConsumer.SetTokenCode(tokenCode);
                ApiConsumerStore.AcsConsumer.SetTokenCode(tokenCode);
                ApiConsumerStore.MrsConsumer.SetTokenCode(tokenCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void ResetConsunmer()
        {
            try
            {
                ApiConsumerStore.SarConsumer = null;
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
                var token = TokenClientStore.ClientTokenManager.Renew(param);
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

        public bool ValidExpriedTimeProcessor()
        {
            bool result = true;
            try
            {
                var tokenData = TokenClientStore.ClientTokenManager.Init(param);
                if (tokenData != null)
                {
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
