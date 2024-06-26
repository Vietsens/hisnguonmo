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

namespace MPS.Processor.Mps000442.PDO
{
    class Mps000442ADOExt
    {
        public Mps000442ADOExt() { }

        public List<Mps000442ADOExt> Mps000111ADOs { get; set; }
    }
    public class SereServADO : HIS_EXP_MEST
    {
        public string MEDICINE_TYPE_NAME { get; set; }
       public string SERVICE_UNIT_NAME { get; set; }
        public string AMOUNT { get; set; }
        public SereServADO() { }
        public SereServADO(HIS_EXP_MEST data)
        {
            try
            {
                if (data != null)
                {
                 
                    //this.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                    //this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    //this.AMOUNT = data.AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    //ĐOạn này là đoạn lấy tất cả các trường của HISss
    //Đây là đoạn ở PDO này, check log của MPS 111 xem trước khi a sửa nó ntn là biết ngay ấy mào oke, 
}
