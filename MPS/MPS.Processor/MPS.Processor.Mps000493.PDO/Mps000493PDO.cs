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

namespace MPS.Processor.Mps000493.PDO
{
    public class Mps000493PDO : RDOBase
    {
        public HIS_PATIENT HisPatient { get; set; }
        public V_HIS_SERE_SERV_5 VHisSereServ5 { get; set; }
        public V_HIS_SERE_SERV_PTTT VHisSereServPttt { get; set; }
        public List<HIS_STENT_CONCLUDE> HisStentConcludes { get; set; }
        public List<V_HIS_EKIP_USER> VHisEkipUsers { get; set; }
        public V_HIS_SERVICE_REQ VHisServiceReq { get; set; }
        public HIS_SERE_SERV_EXT HisSereServExt { get; set; }
        public List<HIS_SERE_SERV_FILE> HisSereServFiles { get; set; }
        public Mps000493PDO()
        {

        }

        public Mps000493PDO(HIS_PATIENT hisPatient, V_HIS_SERE_SERV_5 vHisSereServ5, V_HIS_SERE_SERV_PTTT vHisSereServPttt, List<HIS_STENT_CONCLUDE> hisStentConcludes, List<V_HIS_EKIP_USER> vHisEkipUsers, V_HIS_SERVICE_REQ vHisServiceReq, HIS_SERE_SERV_EXT hisSereServExt, List<HIS_SERE_SERV_FILE> hisSereServFiles)
        {
            this.HisPatient = hisPatient;
            this.VHisSereServ5 = vHisSereServ5;
            this.VHisSereServPttt = vHisSereServPttt;
            this.HisStentConcludes = hisStentConcludes;
            this.VHisEkipUsers = vHisEkipUsers;
            this.VHisServiceReq = vHisServiceReq;
            this.HisSereServExt = hisSereServExt;
            this.HisSereServFiles = hisSereServFiles;
        }
    }
}
