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
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;

namespace Inventec.Desktop.Plugins.ChangePassword
{
    class EventLogBehavior : BusinessBase, IEventLog
    {
        object[] entity;
        internal EventLogBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IEventLog.Run()
        {
            try
            {
                if (entity == null || entity.Count() <= 2)
                    throw new NullReferenceException("Du lieu truyen vao thieu tham so bat buoc. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity));

                Inventec.UC.ChangePassword.HasExceptionApi exceptionApi = null;
                ChangePasswordSuccessDelegate changePasswordSuccessDelegate = null;
                for (int i = 0; i < entity.Count(); i++)
                {
                    if (entity[i] is Inventec.Common.WebApiClient.ApiConsumer)
                    {
                        ChangePasswordConfig.SdaConsumer = (Inventec.Common.WebApiClient.ApiConsumer)entity[i];
                    }
                    else if (entity[i] is Inventec.UC.ChangePassword.HasExceptionApi)
                    {
                        exceptionApi = (Inventec.UC.ChangePassword.HasExceptionApi)entity[i];
                    }
                    else if (entity[i] is ChangePasswordSuccessDelegate)
                    {
                        changePasswordSuccessDelegate = (ChangePasswordSuccessDelegate)entity[i];
                    }
                    else if (entity[i] is string)
                    {
                        ChangePasswordConfig.Icon = entity[i].ToString();
                    }
                }

                return new frmChangePassword(exceptionApi, changePasswordSuccessDelegate);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
