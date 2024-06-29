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
using HIS.UC.ServiceRoomInfo.SetValue;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceRoomInfo
{
    partial class ServiceRoomInfoSetValue : BussinessBase, IAppDelegacy
    {
        object uc;
        V_HIS_PATIENT exam;

        internal ServiceRoomInfoSetValue(CommonParam param, object uc, V_HIS_PATIENT exam)
            : base(param)
        {
            this.uc = uc;
            this.exam = exam;
        }

        object IAppDelegacy.Execute()
        {
            object result = false;
            try
            {
                IServiceRoomInfoSetValue behavior = ServiceRoomInfoSetValueBehaviorFactory.MakeIServiceRequestRegister(param, uc, exam);
                if (behavior != null) behavior.Run();
                result = true;
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
