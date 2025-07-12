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
using MPS.ProcessorBase;
using System.Runtime.InteropServices;

namespace MPS.Processor.Mps000503.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000503PDO : RDOBase
    {
        public const string printTypeCode = "Mps000503";

        public V_HIS_TREATMENT Treatment { get; set; }
        public V_HIS_SERVICE_REQ ServiceReq { get; set; }
        public V_HIS_SERE_SERV Sere_serv { get; set; }

        public Mps000503PDO() { }

        public Mps000503PDO(
            V_HIS_TREATMENT treatment,
            V_HIS_SERVICE_REQ serviceReq,
            V_HIS_SERE_SERV sere_serv
        )
        {
            try
            {
                this.Treatment = treatment;
                this.ServiceReq = serviceReq;
                this.Sere_serv = sere_serv;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
