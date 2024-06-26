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
using LIS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ConnectionTest.ADO
{
    public class LisMachineAdo : LIS_MACHINE
    {
        public long? MAX_SERVICE_PER_DAY { get; set; }
        public long? TOTAL_PROCESSED_SERVICE_TEIN { get; set; }
        public LisMachineAdo() { }


        public LisMachineAdo(LIS_MACHINE data, HisMachineCounterSDO sdo)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_MACHINE>(this, data);
                if (sdo != null)
                {
                    this.MAX_SERVICE_PER_DAY = sdo.MAX_SERVICE_PER_DAY;
                    this.TOTAL_PROCESSED_SERVICE_TEIN = sdo.TOTAL_PROCESSED_SERVICE_TEIN;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
