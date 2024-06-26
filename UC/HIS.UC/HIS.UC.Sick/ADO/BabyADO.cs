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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.Sick.ADO
{
    class BabyADO : HisBabySDO
    {
        public DateTime? BornTimeDt { get; set; }
        public long Stt { get; set; }
        public BabyADO()
        {
        }
        public BabyADO(HisBabySDO data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<BabyADO>(this, data);
            if (!string.IsNullOrEmpty(data.EthnicCode))
            {
                var ethnic = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == data.EthnicCode);
                if (ethnic != null)
                    this.EthnicName = ethnic.ETHNIC_NAME;
            }
            this.BornTimeDt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.BornTime ?? 0);
        }
        //public BabyADO(HIS_BABY data)
        //{
        //    Inventec.Common.Mapper.DataObjectMapper.Map<BabyADO>(this, data);
        //    this.BornTimeDt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.BornTime ?? 0);
        //}
    }
}
