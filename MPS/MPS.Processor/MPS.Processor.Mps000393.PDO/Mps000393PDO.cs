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

namespace MPS.Processor.Mps000393.PDO
{
    public class Mps000393PDO : RDOBase
    {
        public HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_SERE_SERV_PTTT _SereServPttt { get; set; }
        public HIS_SERE_SERV_EXT _SereServExt { get; set; }
        public HIS_EYE_SURGRY_DESC _EyeSurgryDesc { get; set; }
        public List<V_HIS_EKIP_USER> _EkipUser { get; set; }

        public Mps000393PDO(HIS_TREATMENT treatment,
            V_HIS_SERE_SERV_PTTT sereServPttt,
            HIS_EYE_SURGRY_DESC eyeSurgryDesc,
            HIS_SERE_SERV_EXT sereServExt,
            List<V_HIS_EKIP_USER> listUser)
        {
            this._Treatment = treatment;
            this._SereServPttt = sereServPttt;
            this._EyeSurgryDesc = eyeSurgryDesc;
            this._SereServExt = sereServExt;
            this._EkipUser = listUser;
        }
    }
}
