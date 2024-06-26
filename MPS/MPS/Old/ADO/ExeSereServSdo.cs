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

namespace MPS.ADO
{
    public class ExeSereServSdo : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV
    {
        public ExeSereServSdo()
        {

        }
        public ExeSereServSdo(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereserv,MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReq)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<ExeSereServSdo>(this, sereserv);
            this.NUM_ORDER_EXAM = serviceReq.NUM_ORDER;
        }
        public string INSTRUCTION_TIME_STR { get; set; }
        public long? NUM_ORDER_EXAM { get; set; }
    }
}
