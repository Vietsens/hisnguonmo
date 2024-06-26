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

namespace MPS.Processor.Mps000329.PDO
{
    public partial class Mps000329PDO : RDOBase
    {        
        public List<Mps000329ADO> listAdo = new List<Mps000329ADO>();
        public const string printTypeCode = "Mps000329";
        public Mps000329PDO(
            List<V_HIS_SERE_SERV_1> _vsereServs,
            List<HIS_SERE_SERV_EXT> _sereServExts,
            List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1> _ServiceReqs)
        {
            try
            {
                this._vSereServs = _vsereServs;
                this._SereServExts = _sereServExts;
                this._ServiceReqs = _ServiceReqs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }      
    }
}
