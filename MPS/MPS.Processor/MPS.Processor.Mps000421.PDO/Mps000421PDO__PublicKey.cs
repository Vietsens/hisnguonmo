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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000421.PDO
{
    /// <summary>
    /// Chia se key
    /// </summary>
    public partial class Mps000421PDO : RDOBase
    {
        public V_HIS_TREATMENT Treatment { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_EXP_MEST ExpMest { get; set; }
        public List<V_HIS_EXP_MEST_BLOOD> ExpMestBlood { get; set; }
        public List<V_HIS_EXP_BLTY_SERVICE> ExpBltyService { get; set; }
        public List<HIS_EXP_MEST> ListExpMest { get; set; }
        public List<HIS_TRANSFUSION> TransFusions { get; set; }
    }

}
