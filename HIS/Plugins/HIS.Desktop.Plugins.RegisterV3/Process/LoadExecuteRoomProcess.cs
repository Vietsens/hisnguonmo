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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.RegisterV3.ADO;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterV3.Process
{
    class LoadExecuteRoomProcess
    {        
        internal static List<ExecuteRoomADO> listAdo = new List<ExecuteRoomADO>();
        System.Windows.Forms.Timer timer;
        internal LoadExecuteRoomProcess()
        {
            try
            {
                timer = new System.Windows.Forms.Timer();
                int tgian = 300000;
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonThoiGianLoadDanhSachPhongKham > 0)
                {
                    tgian = (int)HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonThoiGianLoadDanhSachPhongKham;
                }
                timer.Interval = Convert.ToInt32(tgian);
                timer.Tick += timer_Tick;
                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.CreateThreadLoadExecuteRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExecuteRoomInfo()
        {
            try
            {
                HisExecuteRoomView1Filter exetuteFilter = new HisExecuteRoomView1Filter();
                exetuteFilter.IS_EXAM = true;
                exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                var listRoom = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXECUTE_ROOM_1>>("api/HisExecuteRoom/GetView1", ApiConsumers.MosConsumer, exetuteFilter, null);
                listRoom = GetListRoom(listRoom);
                if (listRoom != null && listRoom.Count > 0)
                {
                    listRoom = listRoom.OrderBy(o => o.EXECUTE_ROOM_CODE).ToList();
                    listAdo.Clear();
                    int max = 5;
                    int start = 0;
                    int count = listRoom.Count;
                    while (count > 0)
                    {
                        int limit = (count <= max) ? count : max;
                        var listSub = listRoom.Skip(start).Take(limit).ToList();
                        int num = 1;
                        ExecuteRoomADO ado = new ExecuteRoomADO();
                        foreach (var room in listSub)
                        {
                            ado.SetValueRoom(room, num);
                            num++;
                        }
                        listAdo.Add(ado);

                        start += max;
                        count -= max;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void CreateThreadLoadExecuteRoom()
        {
            try
            {
                Thread thread = new System.Threading.Thread(new ThreadStart(LoadDataExecuteRoomInfo));
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_EXECUTE_ROOM_1> GetListRoom(List<V_HIS_EXECUTE_ROOM_1> listData)
        {
            List<V_HIS_EXECUTE_ROOM_1> result = new List<V_HIS_EXECUTE_ROOM_1>();
            try
            {
                if (listData == null || listData.Count <= 0)
                {
                    return result;
                }
                if (HisConfigCFG.ExecuteRoomShow != null && HisConfigCFG.ExecuteRoomShow.Count > 0)
                {
                    foreach (var item in HisConfigCFG.ExecuteRoomShow)
                    {
                        if (String.IsNullOrWhiteSpace(item))
                            continue;
                        var room = listData.FirstOrDefault(o => o.EXECUTE_ROOM_CODE == item);
                        if (room != null)
                        {
                            result.Add(room);
                        }
                    }
                }
                else
                {
                    result = listData;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<V_HIS_EXECUTE_ROOM_1>();
            }
            return result;
        }
    }
}
