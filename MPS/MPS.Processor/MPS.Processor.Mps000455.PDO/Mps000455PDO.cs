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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000455.PDO
{
    public class Mps000455PDO : RDOBase
    {
         public HIS_KSK_DRIVER_CAR HisKskDriverCar { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> examRank { get; set; }
        public Mps000455PDO(
            HIS_KSK_DRIVER_CAR HisKskDriverCar,
           V_HIS_SERVICE_REQ HisServiceReq,
            List<HIS_HEALTH_EXAM_RANK> examRank
            )
        {
            try
            {
                this.HisKskDriverCar = HisKskDriverCar;
                this.HisServiceReq = HisServiceReq;
                this.examRank = examRank;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
