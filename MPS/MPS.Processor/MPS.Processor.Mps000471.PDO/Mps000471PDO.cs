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

namespace MPS.Processor.Mps000471.PDO
{
    public partial class Mps000471PDO : RDOBase
    {
        public List<V_HIS_SERVICE_REQ> serviceReq { get; set; }
        public List<V_HIS_SERE_SERV> sereServ { get; set; }
        public List<HIS_SERE_SERV_EXT> sereServExt { get; set; }

        public Mps000471PDO(List<V_HIS_SERVICE_REQ> _serviceReq, List<V_HIS_SERE_SERV> _sereServ, List<HIS_SERE_SERV_EXT> _sereServExt)
        {
            this.serviceReq = _serviceReq;
            this.sereServ = _sereServ;
            this.sereServExt = _sereServExt;
        }
    }
}
