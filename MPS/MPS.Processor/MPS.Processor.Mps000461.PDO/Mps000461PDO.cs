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

namespace MPS.Processor.Mps000461.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000461PDO : RDOBase
    {
        public List<V_HIS_SERE_SERV> SereServs { get; set; }
        public HIS_TREATMENT CurrentHisTreatment { get; set; }  // được phép ghi đè
        public List<V_HIS_SERVICE_REQ> ListServiceReqPrint { get; set; }    // k ghi đè đc

        public Mps000461PDO(
            HIS_TREATMENT currentHisTreatment,
            List<V_HIS_SERVICE_REQ> ServiceReqPrint,
            List<V_HIS_SERE_SERV> _listSereServs
            )
        {
            try
            {
                this.ListServiceReqPrint = ServiceReqPrint;
                this.SereServs = _listSereServs.OrderByDescending(o => o.TDL_SERVICE_REQ_TYPE_ID).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                this.CurrentHisTreatment = currentHisTreatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
