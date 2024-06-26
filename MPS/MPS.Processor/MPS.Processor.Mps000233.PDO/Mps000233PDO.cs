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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000233.PDO
{
    public partial class Mps000233PDO
    {
        public Mps000233PDO(V_LIS_SAMPLE currentBarCode, V_HIS_SERVICE_REQ hisServiceReq, List<V_HIS_SERVICE> services,List<V_HIS_SERE_SERV_6>_sereServs)
        {
            try
            {
                this.CurrentBarCode = currentBarCode;
                this.HisServiceReq = hisServiceReq;
                this.Services = services;
                this.SereServs = _sereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
