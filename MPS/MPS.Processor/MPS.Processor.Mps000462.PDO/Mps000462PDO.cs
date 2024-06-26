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
using MPS.ProcessorBase.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000462.PDO
{
    public partial class Mps000462PDO : RDOBase
    {
        public const string printTypeCode = "Mps000462";
        public V_HIS_ANTIBIOTIC_REQUEST V_HIS_ANTIBIOTIC_REQUEST { get; set; }
        public HIS_DHST HIS_DHST { get; set; }
        public List<HIS_ANTIBIOTIC_MICROBI> HIS_ANTIBIOTIC_MICROBI { get; set; }
        public List<V_HIS_ANTIBIOTIC_NEW_REG> V_HIS_ANTIBIOTIC_NEW_REG { get; set; }
        public List<HIS_ANTIBIOTIC_OLD_REG> HIS_ANTIBIOTIC_OLD_REG { get; set; }

        public Mps000462PDO(V_HIS_ANTIBIOTIC_REQUEST _V_HIS_ANTIBIOTIC_REQUEST, List<HIS_ANTIBIOTIC_MICROBI> _HIS_ANTIBIOTIC_MICROBI, List<V_HIS_ANTIBIOTIC_NEW_REG> _V_HIS_ANTIBIOTIC_NEW_REG, List<HIS_ANTIBIOTIC_OLD_REG> _HIS_ANTIBIOTIC_OLD_REG, HIS_DHST _HIS_DHST)
        {
            try
            {
                this.V_HIS_ANTIBIOTIC_REQUEST = _V_HIS_ANTIBIOTIC_REQUEST;
                this.HIS_ANTIBIOTIC_MICROBI = _HIS_ANTIBIOTIC_MICROBI;
                this.V_HIS_ANTIBIOTIC_NEW_REG = _V_HIS_ANTIBIOTIC_NEW_REG;
                this.HIS_ANTIBIOTIC_OLD_REG = _HIS_ANTIBIOTIC_OLD_REG;
                this.HIS_DHST = _HIS_DHST;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
