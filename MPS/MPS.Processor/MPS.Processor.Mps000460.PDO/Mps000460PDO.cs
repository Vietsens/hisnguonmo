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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000460.PDO
{
    public partial class Mps000460PDO : RDOBase
    {
        public const string printTypeCode = "Mps000460";
        public List<V_LIS_SAMPLE> V_LIS_SAMPLE { get; set; }
        public LIS_DELIVERY_NOTE LIS_DELIVERY_NOTE { get; set; }

        public Mps000460PDO(List<V_LIS_SAMPLE> _V_LIS_SAMPLE, LIS_DELIVERY_NOTE _LIS_DELIVERY_NOTE)
        {
            try
            {
                this.V_LIS_SAMPLE = _V_LIS_SAMPLE;
                this.LIS_DELIVERY_NOTE = _LIS_DELIVERY_NOTE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
