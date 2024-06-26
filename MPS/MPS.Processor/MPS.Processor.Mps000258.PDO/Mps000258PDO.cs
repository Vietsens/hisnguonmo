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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000258.PDO
{
    public class Mps000258PDO : RDOBase
    {
        public const string printTypeCode = "Mps000258";

        public List<V_HIS_SERVICE_PATY> ListServicePaty { get; set; }
        public List<V_HIS_SERVICE> ListService { get; set; }
        public List<HIS_PATIENT_TYPE> ListPatientType { get; set; }

        public Mps000258PDO(List<V_HIS_SERVICE_PATY> _ListServicePaty, List<V_HIS_SERVICE> _ListService, List<HIS_PATIENT_TYPE> _ListPatientType)
        {
            try
            {
                this.ListServicePaty = _ListServicePaty;
                this.ListService = _ListService;
                this.ListPatientType = _ListPatientType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
