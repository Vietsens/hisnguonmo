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
using HIS.Desktop.Common;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.Register.Register
{
    public class ServiceRequestRegister : BusinessBase, IAppDelegacyT
    {
        object entity;
        object patient;
        internal ServiceRequestRegister(CommonParam param, object data, object patient)
            : base(param)
        {
            this.entity = data;
            this.patient = patient;
        }

        T IAppDelegacyT.Execute<T>()
        {
            T result = default(T);
            try
            {
                if (typeof(T) == typeof(HisServiceReqExamRegisterResultSDO))
                {
                    IServiceRequestRegisterExam behavior = ServiceRequestRegisterExamBehaviorFactory.MakeIServiceRequestRegister(param, entity, patient);
                    result = behavior != null ? (T)System.Convert.ChangeType(behavior.Run(), typeof(T)) : default(T);
                }
                else if (typeof(T) == typeof(HisPatientProfileSDO))
                {
                    IServiceRequestRegisterPatientProfile behavior = ServiceRequestRegisterPatientProfileBehaviorFactory.MakeIServiceRequestRegister(param, entity, patient);
                    result = behavior != null ? (T)System.Convert.ChangeType(behavior.Run(), typeof(T)) : default(T);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }
    }
}
