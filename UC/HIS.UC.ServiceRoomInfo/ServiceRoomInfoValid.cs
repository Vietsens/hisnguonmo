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
using HIS.UC.ServiceRoomInfo.Valid;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceRoomInfo
{
    partial class ServiceRoomInfoValid : BussinessBase, IAppDelegacy
    {
        object uc;

        internal ServiceRoomInfoValid(CommonParam param, object uc)
            : base(param)
        {
            this.uc = uc;
        }

        object IAppDelegacy.Execute()
        {
            object result = false;
            try
            {
                IServiceRoomInfoValid behavior = ServiceRoomInfoValidBehaviorFactory.MakeIServiceRequestRegister(param, uc);
                result = behavior != null ? behavior.Run() : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
