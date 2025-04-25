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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedRoomPartial.ADO
{
    public class SereServADO : DHisSereServ2
    {
       
        public bool IsEnableEdit { get; set; }
        public bool IsEnableDelete { get; set; }
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public string NOTE_ADO { get; set; }
        public string AMOUNT_SER { get; set; }
        public long SERVICE_REQ_TYPE_ID { get; set; }
        public long child { get; set; }
        public long REQUEST_DEPARTMENT_ID { get; set; }
        public long EXECUTE_DEPARTMENT_ID { get; set; }
        //ServiceReq
        public long? SAMPLE_TIME { get; set; }
        public long? RECEIVE_SAMPLE_TIME { get; set; }
        public short? IS_TEMPORARY_PRES { get; set; }

        public SereServADO() { }

        public SereServADO(DHisSereServ2 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);
                    this.PARENT_ID__IN_SETY = data.SERVICE_REQ_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
