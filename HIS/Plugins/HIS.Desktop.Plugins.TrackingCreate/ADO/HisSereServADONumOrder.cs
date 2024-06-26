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

namespace HIS.Desktop.Plugins.TrackingCreate.ADO
{
    public class HisSereServADONumOrder : MOS.EFMODEL.DataModels.HIS_SERE_SERV
    {
        public long? NUM_ORDER { get; set; }
        public string TDL_SERVICE_UNIT_NAME { get; set; }
        public string RATION_TIME_NAME { get; set; }
        public long? USE_TIME { get; set; }
        public HisSereServADONumOrder()
        {
        }
        public HisSereServADONumOrder(HIS_SERE_SERV data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisSereServADONumOrder>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public HisSereServADONumOrder(HIS_SERE_SERV data,HIS_SERVICE_REQ ser)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisSereServADONumOrder>(this, data);
                    this.USE_TIME = ser.USE_TIME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
