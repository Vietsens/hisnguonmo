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

namespace MPS.Processor.Mps000371.ADO
{
    class SereServADO : V_HIS_SERE_SERV_15
    {
        public string TDL_SERVICE_UNIT_NAME { get; set; }
        public string BED_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string RATION_TIME_NAME { get; set; }
        public string RATION_TIME_CODE { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }

        public SereServADO(V_HIS_SERE_SERV_15 data, List<V_HIS_SERVICE_REQ> _ListServiceReq, List<HIS_SERVICE_UNIT> ListServiceUnit, List<HIS_TREATMENT_BED_ROOM> TreatmentBedRoom, List<V_HIS_BED_LOG> HisBedLog)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);

                if (_ListServiceReq != null && _ListServiceReq.Count > 0)
                {
                    var req = _ListServiceReq.FirstOrDefault(o => o.ID == data.SERVICE_REQ_ID);
                    if (req != null)
                    {
                        this.REQUEST_ROOM_NAME = req.REQUEST_ROOM_NAME;
                        this.DESCRIPTION = req.DESCRIPTION;
                        this.RATION_TIME_NAME = req.RATION_TIME_NAME;
                        this.RATION_TIME_CODE = req.RATION_TIME_CODE;
                    }
                }

                if (ListServiceUnit != null && ListServiceUnit.Count > 0)
                {
                    var serviceUnit = ListServiceUnit.FirstOrDefault(o => o.ID == data.TDL_SERVICE_UNIT_ID);
                    if (serviceUnit != null)
                        this.TDL_SERVICE_UNIT_NAME = serviceUnit.SERVICE_UNIT_NAME;
                }

                if (TreatmentBedRoom != null && TreatmentBedRoom.Count > 0 && HisBedLog != null && HisBedLog.Count > 0)
                {
                    var treatmentBedRoom = TreatmentBedRoom.FirstOrDefault(o => o.TREATMENT_ID == (data.TDL_TREATMENT_ID ?? 0));
                    if (treatmentBedRoom != null)
                    {
                        var bedLog = HisBedLog.Where(o => o.TREATMENT_BED_ROOM_ID == treatmentBedRoom.ID && o.FINISH_TIME != null).ToList();
                        if (bedLog != null && bedLog.Count > 0)
                        {
                            this.BED_NAME = bedLog.OrderByDescending(o => o.START_TIME).FirstOrDefault().BED_NAME;
                        }
                    }
                }
            }
        }
    }
}
