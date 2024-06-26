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
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.ExecuteRoom.ADO
{
    class SereServ6ADO : V_HIS_SERE_SERV_6
    {
        public HIS_SERE_SERV_EXT SereServExt { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public string MACHINE_NAME { get; set; }

        public SereServ6ADO(HIS_SERE_SERV data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServ6ADO>(this, data);
                HIS_SERVICE_UNIT serviceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == data.TDL_SERVICE_UNIT_ID);
                if (serviceUnit != null)
                {
                    this.SERVICE_UNIT_CODE = serviceUnit.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = serviceUnit.SERVICE_UNIT_NAME;
                }

                HIS_SERVICE_TYPE serviceType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == data.TDL_SERVICE_TYPE_ID);
                if (serviceType != null)
                {
                    this.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                    this.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                }
                if (SereServExt != null)
                {
                    this.INSTRUCTION_NOTE = SereServExt.INSTRUCTION_NOTE;
                }

                this.BLOCK = GetBlockFromSereServ6(data);
            }
        }

        private string GetBlockFromSereServ6(HIS_SERE_SERV data)
        {
            string blockStr = "";
            try
            {
                CommonParam param = new CommonParam();
                HisSereServView6Filter filter = new HisSereServView6Filter();
                filter.ID = data.ID;
                var listSereServ6 = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_6>>("api/HisSereServ/GetView6", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("list__V_HIS_SERE_SERV_6:__", listSereServ6));
                if (listSereServ6 != null && listSereServ6.Count == 1)
                {
                    blockStr = listSereServ6.First().BLOCK;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return blockStr;
        }
    }
}
