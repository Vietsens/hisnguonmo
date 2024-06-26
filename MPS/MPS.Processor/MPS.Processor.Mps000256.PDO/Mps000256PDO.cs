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

namespace MPS.Processor.Mps000256.PDO
{
    public class Mps000256PDO : RDOBase
    {
        public const string printTypeCode = "Mps000256";

        public List<HIS_PTTT_GROUP> ListPtttGroup { get; set; }
        public List<HIS_PTTT_METHOD> ListPtttMethod { get; set; }
        public List<HIS_PACKAGE> ListPackage { get; set; }
        public List<V_HIS_SERVICE> ListVService { get; set; }
        public List<V_HIS_SERVICE_PATY> ListVServicePaty { get; set; }
        public Mps000256ADO Mps000256ADO { get; set; }

        public Mps000256PDO(List<V_HIS_SERVICE> _ListVService, List<V_HIS_SERVICE_PATY> _ListVServicePaty,
            List<HIS_PACKAGE> _ListPackage, List<HIS_PTTT_METHOD> _ListPtttMethod, List<HIS_PTTT_GROUP> _ListPtttGroup,
            Mps000256ADO _Mps000256ADO)
        {
            try
            {
                this.ListPackage = _ListPackage;
                this.ListPtttGroup = _ListPtttGroup;
                this.ListPtttMethod = _ListPtttMethod;
                this.ListVService = _ListVService;
                this.ListVServicePaty = _ListVServicePaty;
                this.Mps000256ADO = _Mps000256ADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class Mps000256ADO
    {
        public long? PATIENT_TYPE_ID__BHYT { get; set; }
        public long? PATIENT_TYPE_ID__VP { get; set; }
    }
}
