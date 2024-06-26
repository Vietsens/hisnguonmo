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

namespace HIS.Desktop.Plugins.SereServTein.ADO
{
    public class HisSereServTeinSDO : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_TEIN
    {
        public HisSereServTeinSDO()
        {

        }

        public HisSereServTeinSDO(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_TEIN data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisSereServTeinSDO>(this, data);
        }
        public bool checkMinMax { get; set; }
        public long IS_PARENT { get; set; }
        public decimal? MIN_VALUE { get; set; }
        public decimal? MAX_VALUE { get; set; }
        public string VALUE_RANGE { get; set; }
        public long? LEVEL { get; set; }
        public string NOTE { get; set; }
        public long? SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public long INTRUCTION_TIME { get; set; }
        public bool IS_NO_EXECUTE { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string EXECUTE_LOGINNAME { get; set; }
        public string SUBCLINICAL_RESULT_LOGINNAME { get; set; }
        public bool IsHasResultFrom_HIS_SERE_SERV_TEIN { get; set; }
        public long SERVICE_REQ_STT_ID { get; set; }
    }
}
