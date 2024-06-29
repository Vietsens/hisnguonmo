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
using HIS.UC.ServiceRoom.FocusAndShow;
using HIS.UC.ServiceRoom.FocusService;
using HIS.UC.ServiceRoom.GetDetailSDO;
using HIS.UC.ServiceRoom.Run;
using HIS.UC.ServiceRoom.SetValueByPatient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceRoom
{
    public partial class RoomExamServiceProcessor
    {
        public RoomExamServiceProcessor() { }
        public object Run(object data)
        {
            object result = null;
            try
            {
                IRun behavior = RunBehaviorFactory.MakeIRoomExamService(data);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public object GetDetailSDO(object uc)
        {
            object result = null;
            try
            {
                IGetDetailSDO behavior = GetDetailSDOBehaviorFactory.MakeIGetDetailSDO(uc);
                result = behavior != null ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void FocusAndShow(object uc)
        {
            try
            {
                IFocusAndShow behavior = FocusAndShowBehaviorFactory.MakeIFocusAndShow(uc);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusService(object uc)
        {
            try
            {
                IFocusService behavior = FocusServiceBehaviorFactory.MakeIFocusService(uc);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValueByPatient(object uc, object patient)
        {
            try
            {
                ISetValueByPatient behavior = SetValueByPatientBehaviorFactory.MakeISetValueByPatient(uc, patient);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitComboRoom(object uc, List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER> executeRooms)
        {
            try
            {
                ((UCRoomExamService)uc).InitComboRoom(executeRooms);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitComboRoom(object uc, List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM_1> executeRooms)
        {
            try
            {
                ((UCRoomExamService)uc).InitComboRoom(executeRooms);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
