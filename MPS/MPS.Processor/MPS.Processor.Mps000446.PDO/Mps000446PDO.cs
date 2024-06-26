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

namespace MPS.Processor.Mps000446.PDO
{
    public class Mps000446PDO : RDOBase
    {
        public V_HIS_TREATMENT_FEE Treatment { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }
        public List<HIS_SERE_SERV> HisSereServ { get; set; }
        public List<V_HIS_ROOM> HisRoom { get; set; }
        public List<V_HIS_SERVICE> HisService { get; set; }
        public List<HIS_SERVICE_TYPE> HisServiceType { get; set; }
        public List<HIS_SESE_TRANS_REQ> HisSeseTransReqs { get; set; }
        public List<V_HIS_SERE_SERV> VHisSereServs { get; set; }
        public V_HIS_TREATMENT HisTreatment { get; set; }
        public List<HIS_CONFIG> ListHisConfigPaymentQrCode { get; set; }

        public Mps000446PDO(V_HIS_TREATMENT_FEE treatment, HIS_TRANS_REQ transReq, List<HIS_SERE_SERV> hisSereServ, List<V_HIS_ROOM> hisRoom, List<V_HIS_SERVICE> hisService, List<HIS_SERVICE_TYPE> hisServiceType)
        {
            this.HisSereServ = hisSereServ;
            this.TransReq = transReq;
            this.Treatment = treatment;
            this.HisRoom = hisRoom;
            this.HisService = hisService;
            this.HisServiceType = hisServiceType;
        }
        public Mps000446PDO(List<HIS_SESE_TRANS_REQ> hisSeseTransReqs, List<V_HIS_SERE_SERV> vHisSereServs, V_HIS_TREATMENT hisTreatment, HIS_TRANS_REQ transReq, List<HIS_CONFIG> listHisConfigPaymentQrCode)
        {
            this.HisSeseTransReqs = hisSeseTransReqs;
            this.VHisSereServs = vHisSereServs;
            this.HisTreatment = hisTreatment;
            this.TransReq = transReq;
            this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
        }
    }
}
