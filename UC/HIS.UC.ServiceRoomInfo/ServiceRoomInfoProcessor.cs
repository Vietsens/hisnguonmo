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
using HIS.UC.ServiceRoomInfo.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceRoomInfo
{
    public partial class ServiceRoomInfoProcessor : BussinessBase
    {
        public ServiceRoomInfoProcessor(CommonParam param)
            : base(param)
        {

        }

        public object Generate(object data)
        {
            object result = null;
            try
            {
                IAppDelegacy delegacy = new ServiceRoomInfoGenerate(param, data);
                result = delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public object GetDetailSDO(object data)
        {
            object result = null;
            try
            {
                IAppDelegacy delegacy = new ServiceRoomInfoGetDetailSDO(param, data);
                result = delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public object Remove(object deleteServiceRoomInfo, object uc)
        {
            object result = null;
            try
            {
                IAppDelegacy delegacy = new ServiceRoomInfoRemove(param, deleteServiceRoomInfo, uc);
                result = delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool ValidControl(object uc)
        {
            bool result = true;
            try
            {
                IAppDelegacy delegacy = new ServiceRoomInfoValid(param, uc);
                result = (bool)delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void FocusIn(object uc)
        {
            try
            {
                IAppDelegacy delegacy = new ServiceRoomInfoFocusIn(param, uc);
                delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValue(object uc, V_HIS_PATIENT exam)
        {
            try
            {
                IAppDelegacy delegacy = new ServiceRoomInfoSetValue(param, uc, exam);
                delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
