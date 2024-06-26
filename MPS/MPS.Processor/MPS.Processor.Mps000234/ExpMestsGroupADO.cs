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
using MPS.Processor.Mps000234.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000234
{
    class ExpMestsGroupADO : ExpMestMedicineSDO
    {
        public string REQUEST_ROOM_CODE { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USER_TITLE { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string MOBILE { get; set; }
        public long MEDI_STOCK_TYPE { get; set; } // 1 - Trong kho; 2 - Ngoài Kho; 3 - Khác
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }

        public ExpMestsGroupADO()
        {

        }

        public ExpMestsGroupADO(ExpMestMedicineSDO data)
        {
            if (data != null)
                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestsGroupADO>(this, data);
        }

        public long GetTypeGroup()
        {
            if (this.Type > 0 && this.Type <= 2)
                return 1;
            if (this.Type > 2 && this.Type <= 4)
                return 2;
            return 3;
        }
    }
}
