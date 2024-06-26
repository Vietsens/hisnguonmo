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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ModuleExt
{
    public class WorkPlaceWorker
    {
        public WorkPlaceWorker() { }

        public static bool UpdateWorkingRoomByTreatmentRoom(long roomId)
        {
            bool success = false;
            try
            {
                bool existsRoom = WorkPlace.WorkPlaceSDO.Exists(o => o.RoomId == roomId);
                if (existsRoom)
                {
                    success = true;
                }
                else
                {
                    CommonParam param = new CommonParam();
                    WorkInfoSDO workInfoSDO = new WorkInfoSDO();
                    if (WorkPlace.WorkPlaceSDO != null && WorkPlace.WorkPlaceSDO.Count > 0)
                    {
                        workInfoSDO.NurseLoginName = WorkPlace.WorkInfoSDO.NurseLoginName;
                        workInfoSDO.NurseUserName = WorkPlace.WorkInfoSDO.NurseUserName;
                        workInfoSDO.WorkingShiftId = WorkPlace.WorkInfoSDO.WorkingShiftId;
                        workInfoSDO.Rooms = WorkPlace.WorkInfoSDO.Rooms;
                        workInfoSDO.Rooms.Add(new RoomSDO() { RoomId = roomId });
                    }

                    WorkPlace.WorkPlaceSDO = new BackendAdapter(param).Post<List<WorkPlaceSDO>>(HisRequestUriStore.TOKEN__UPDATE_WORK_PLACE_INFO, ApiConsumer.ApiConsumers.MosConsumer, workInfoSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (WorkPlace.WorkPlaceSDO != null && WorkPlace.WorkPlaceSDO.Count > 0)
                    {
                        WorkPlace.WorkInfoSDO = workInfoSDO;
                        GlobalVariables.CurrentRoomTypeIds = WorkPlace.WorkPlaceSDO.Select(o => o.RoomTypeId).ToList();
                        GlobalVariables.CurrentRoomTypeId = WorkPlace.WorkPlaceSDO.FirstOrDefault().RoomTypeId;
                        var roomTypes = BackendDataWorker.Get<HIS_ROOM_TYPE>().Where(o => GlobalVariables.CurrentRoomTypeIds.Contains(o.ID)).ToList();
                        if (roomTypes != null && roomTypes.Count > 0)
                        {
                            GlobalVariables.CurrentRoomTypeCode = roomTypes[0].ROOM_TYPE_CODE;
                            GlobalVariables.CurrentRoomTypeCodes = roomTypes.Select(o => o.ROOM_TYPE_CODE).ToList();
                        }
                        success = true;
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => WorkPlace.WorkPlaceSDO), WorkPlace.WorkPlaceSDO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
    }
}
