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

namespace MPS.Processor.Mps000394.PDO
{
    public class Mps000394PDO : RDOBase
    {
        public HIS_TREATMENT Treatment;
        public V_HIS_SERE_SERV_PTTT SereServPttt;
        public HIS_EYE_SURGRY_DESC EyeSurgryDesc;
        public HIS_SERE_SERV_EXT SereServExt;
        public List<V_HIS_EKIP_USER> ListUser;

        public Mps000394PDO(HIS_TREATMENT treatment,
            V_HIS_SERE_SERV_PTTT sereServPttt,
            HIS_EYE_SURGRY_DESC eyeSurgryDesc,
            HIS_SERE_SERV_EXT sereServExt,
            List<V_HIS_EKIP_USER> listUser)
        {
            this.Treatment = treatment;
            this.SereServPttt = sereServPttt;
            this.EyeSurgryDesc = eyeSurgryDesc;
            this.SereServExt = sereServExt;
            this.ListUser = listUser;
        }
    }
}
