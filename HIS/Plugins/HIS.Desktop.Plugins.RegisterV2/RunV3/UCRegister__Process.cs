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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private async Task SetDefaultCashierRoom()
        {
            try
            {
                List<HIS_CASHIER_ROOM> listCashier = new List<HIS_CASHIER_ROOM>();
                if (BackendDataWorker.IsExistsKey<HIS_CASHIER_ROOM>())
                {
                    listCashier = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_CASHIER_ROOM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    listCashier = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_CASHIER_ROOM>>("api/HisCashierRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (listCashier != null) BackendDataWorker.UpdateToRam(typeof(HIS_CASHIER_ROOM), listCashier, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                //Ci hien thi phong thu ngan ma ng dung chon lam viec
                var roomIds = WorkPlace.GetRoomIds();
                if (roomIds == null || roomIds.Count == 0)
                    throw new ArgumentNullException("Nguoi dung khong chon phong thu ngan nao");
                listCashier = listCashier.Where(o => roomIds.Contains(o.ROOM_ID)).ToList();

                this.InitComboCommon(this.cboCashierRoom, listCashier, "ID", "CASHIER_ROOM_NAME", "CASHIER_ROOM_CODE");

                if (listCashier != null && listCashier.Count == 1)
                {
                    this.cboCashierRoom.EditValue = listCashier.First().ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
