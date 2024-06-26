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
using MPS.Processor.Mps000046.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000046
{
    class ReqLoginADO
    {
        public string REQ_LOGINNAME { get; set; }
        public string REQ_USERNAME { get; set; }
        public long AMOUNT { get; set; }
        public long TYPE_ID { get; set; }

        public ReqLoginADO() { }

        public ReqLoginADO(Mps000046ADO data, List<Mps000046ADO> Mps000046ADOs) 
        {
            try
            {
                this.REQ_LOGINNAME = data.REQ_LOGINNAME;
                this.REQ_USERNAME = data.REQ_USERNAME;
                this.TYPE_ID = data.TYPE_ID;
                this.AMOUNT = Mps000046ADOs.Where(o => o.REQ_LOGINNAME == data.REQ_LOGINNAME).ToList().Count();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

    }
}
