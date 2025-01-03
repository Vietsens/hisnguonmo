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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000233.PDO
{
    public partial class Mps000233PDO : RDOBase
    {
        public V_LIS_SAMPLE CurrentBarCode { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_SERE_SERV_6> SereServs { get; set; }
        public List<Mps000233ADO> ListRdo = new List<Mps000233ADO>();

        public class PrintTypeCode
        {
            public const string Mps000233 = "Mps000233";
        }

        public class Mps000233ADO : MOS.EFMODEL.DataModels.V_HIS_SERVICE
        {
            public long? APPOINTMENT_TIME { get; set; }
        }
    }
}
