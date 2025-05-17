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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.UCServiceRoomInfo
{
    public class ExecuteRoomGet1
    {
        public ExecuteRoomGet1() { }

        public List<V_HIS_EXECUTE_ROOM_1> Get1()
        {
            try
            {
                HisExecuteRoomView1Filter exetuteFilter = new HisExecuteRoomView1Filter();
                exetuteFilter.IS_EXAM = true;
                exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                return new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXECUTE_ROOM_1>>("api/HisExecuteRoom/GetView1", ApiConsumers.MosConsumer, exetuteFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        public List<L_HIS_ROOM_COUNTER_2> GetLCounter1()
        {
            try
            {
                HisRoomCounterLViewFilter exetuteFilter = new HisRoomCounterLViewFilter();
                exetuteFilter.IS_EXAM = true;
                exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                return new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<L_HIS_ROOM_COUNTER_2>>("api/HisRoom/GetCounterLView2", ApiConsumers.MosConsumer, exetuteFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        public async Task<List<L_HIS_ROOM_COUNTER_2>> GetLCounter1Async()
        {
            try
            {
                HisRoomCounterLViewFilter exetuteFilter = new HisRoomCounterLViewFilter();
                exetuteFilter.IS_EXAM = true;
                exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                return await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<L_HIS_ROOM_COUNTER_2>>("api/HisRoom/GetCounterLView2", ApiConsumers.MosConsumer, exetuteFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
