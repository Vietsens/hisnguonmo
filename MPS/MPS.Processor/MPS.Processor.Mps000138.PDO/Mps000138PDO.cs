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
using MOS.SDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000138.PDO
{
    public partial class Mps000138PDO : RDOBase
    {
        public V_HIS_REGISTER_REQ _RegisterReq { get; set; }
        public V_HIS_REGISTER_REQ _RegisterReqLastCalled { get; set; }

        public Mps000138PDO(V_HIS_REGISTER_REQ registerReq)
        {
            this._RegisterReq = registerReq;
        }

        public Mps000138PDO(V_HIS_REGISTER_REQ registerReq, V_HIS_REGISTER_REQ registerReqLastCalled)
        {
            this._RegisterReq = registerReq;
            this._RegisterReqLastCalled = registerReqLastCalled;
        }
    }
}
