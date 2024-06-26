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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.AllocateExecuteRoom.ADO
{
    class ServiceReq1ADO : L_HIS_SERVICE_REQ_1
    {
        public string DOB_DISPLAY { get; set; }
        public long EXECUTE_DEPARTMENT_ID { get; set; }
        public long REQUEST_DEPARTMENT_ID { get; set; }
        public long REQUEST_ROOM_ID { get; set; }
        public string EXECUTE_DEPARTMENT_CODE { get; set; }
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string EXECUTE_ROOM_ADDRESS { get; set; }
        public string REQUEST_DEPARTMENT_CODE { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_ROOM_CODE { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string SERVICE_REQ_STT_CODE { get; set; }
        public string SERVICE_REQ_STT_NAME { get; set; }
        public string SERVICE_REQ_TYPE_CODE { get; set; }
        public string SERVICE_REQ_TYPE_NAME { get; set; }
        public bool isCheckRow { get; set; }

        public ServiceReq1ADO() { this.isCheckRow = false; }

        public ServiceReq1ADO(L_HIS_SERVICE_REQ_1 data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReq1ADO>(this, data);
                DOB_DISPLAY = data.TDL_PATIENT_DOB > 0 ? data.TDL_PATIENT_DOB.ToString().Substring(0, 4) : null;
                this.isCheckRow = false;
            }
        }

        public ServiceReq1ADO(HIS_SERVICE_REQ data)
        {
            if (data != null)
            {
               
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == data.EXECUTE_ROOM_ID);
                if (executeRoom != null)
                {
                    this.EXECUTE_DEPARTMENT_CODE = executeRoom.DEPARTMENT_CODE;
                    this.EXECUTE_DEPARTMENT_NAME = executeRoom.DEPARTMENT_NAME;
                    this.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                    this.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                    this.EXECUTE_ROOM_ADDRESS = executeRoom.ADDRESS;
                }
                else
                {
                    this.EXECUTE_DEPARTMENT_ID = data.EXECUTE_DEPARTMENT_ID;
                    this.EXECUTE_ROOM_ID = data.EXECUTE_ROOM_ID;
                }
                var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == data.REQUEST_ROOM_ID);
                if (reqRoom != null)
                {
                    this.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                    this.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                    this.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                    this.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                }
                else
                {
                    this.REQUEST_DEPARTMENT_ID = data.REQUEST_DEPARTMENT_ID;
                    this.REQUEST_ROOM_ID = data.REQUEST_ROOM_ID;
                }
                var serviceReqStt = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().FirstOrDefault(o => o.ID == data.SERVICE_REQ_STT_ID);
                if (serviceReqStt != null)
                {
                    this.SERVICE_REQ_STT_CODE = serviceReqStt.SERVICE_REQ_STT_CODE;
                    this.SERVICE_REQ_STT_NAME = serviceReqStt.SERVICE_REQ_STT_NAME;
                }
                var serviceReqType = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(o => o.ID == data.SERVICE_REQ_TYPE_ID);
                if (serviceReqType != null)
                {
                    this.SERVICE_REQ_TYPE_CODE = serviceReqType.SERVICE_REQ_TYPE_CODE;
                    this.SERVICE_REQ_TYPE_NAME = serviceReqType.SERVICE_REQ_TYPE_NAME;
                }


                this.isCheckRow = false;
            }
        }
    }
}
