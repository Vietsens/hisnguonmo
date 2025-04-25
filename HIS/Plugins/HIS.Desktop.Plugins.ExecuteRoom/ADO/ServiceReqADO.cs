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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExecuteRoom.ADO
{
    public class ServiceReqADO : L_HIS_SERVICE_REQ
    {
        public string DOB_DISPLAY { get; set; }
        public string REQUEST_DEPARTMENT_DISPLAY { get; set; }
        public string REQUEST_ROOM_DISPLAY { get; set; }
        public string PATIENT_CLASSIFY_NAME { get; set; }
        public string DISPLAY_COLOR { get; set; }
        public long? SAMPLE_TIME { get; set; }
        public long status { get; set; }
        public ServiceReqADO() { }
        public ServiceReqADO(L_HIS_SERVICE_REQ data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqADO>(this, data);
                DOB_DISPLAY = data.TDL_PATIENT_DOB > 0 ? data.TDL_PATIENT_DOB.ToString().Substring(0, 4) : null;
                HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == data.REQUEST_DEPARTMENT_ID);
                REQUEST_DEPARTMENT_DISPLAY = department != null ? department.DEPARTMENT_NAME : null;

                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == data.REQUEST_ROOM_ID);
                REQUEST_ROOM_DISPLAY = room != null ? room.ROOM_NAME : null;

                if (data.TDL_PATIENT_CLASSIFY_ID.HasValue)
                {
                    HIS_PATIENT_CLASSIFY classify = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().FirstOrDefault(o => o.ID == data.TDL_PATIENT_CLASSIFY_ID);
                    PATIENT_CLASSIFY_NAME = classify != null ? classify.PATIENT_CLASSIFY_NAME : null;
                    DISPLAY_COLOR = classify != null ? classify.DISPLAY_COLOR : null;
                }
            }
        }

        public ServiceReqADO(HIS_SERVICE_REQ data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqADO>(this, data);
                DOB_DISPLAY = data.TDL_PATIENT_DOB > 0 ? data.TDL_PATIENT_DOB.ToString().Substring(0, 4) : null;
                SAMPLE_TIME = data.SAMPLE_TIME;
                HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == data.REQUEST_DEPARTMENT_ID);
                REQUEST_DEPARTMENT_DISPLAY = department != null ? department.DEPARTMENT_NAME : null;

                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == data.REQUEST_ROOM_ID);
                REQUEST_ROOM_DISPLAY = room != null ? room.ROOM_NAME : null;
                if (data.TDL_PATIENT_CLASSIFY_ID.HasValue)
                {
                    HIS_PATIENT_CLASSIFY classify = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().FirstOrDefault(o => o.ID == data.TDL_PATIENT_CLASSIFY_ID);
                    PATIENT_CLASSIFY_NAME = classify != null ? classify.PATIENT_CLASSIFY_NAME : null;
                    DISPLAY_COLOR = classify != null ? classify.DISPLAY_COLOR : null;
                }
            }
        }
    }
}
