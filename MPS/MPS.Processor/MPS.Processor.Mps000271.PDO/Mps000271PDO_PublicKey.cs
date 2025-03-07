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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000271;
using MPS.ProcessorBase;
using HID.EFMODEL.DataModels;
using SCN.EFMODEL.DataModels;

namespace MPS.Processor.Mps000271.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000271PDO : RDOBase
    {
        public Mps000271ADO mps000271Ado { get; set; }
        public SingleKey singleKey { get; set; }
        public V_HIS_TREATMENT treatment { get; set; }
        public V_HIS_EXP_MEST_BLOOD expMestBlood { get; set; }
        public List<HIS_TRANSFUSION> transfusions { get; set; }
    }

    public class Mps000271ADO
    {
        public V_HIS_EXP_MEST_BLOOD expMestBlood { get; set; }
        public V_HIS_TREATMENT treatment { get; set; }
        public V_HIS_TRANSFUSION_SUM transfusionSum { get; set; }
        public List<HIS_TRANSFUSION> transfusions { get; set; }
    }

    public class SingleKey
    {
        public string CURRENT_ROOM_NAME { get; set; }
        public string CURRENT_DEPARTMENT_NAME { get; set; }
        public string TEST_SERVICE_NAME { get; set; }
    }

}
