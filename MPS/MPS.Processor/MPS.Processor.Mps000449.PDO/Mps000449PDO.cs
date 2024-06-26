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

namespace MPS.Processor.Mps000449.PDO
{
    public class Mps000449PDO : RDOBase
    {
        public const string PrintTypeCode = "Mps000449";

        public List<V_HIS_SERVICE_REQ> ServiceReq = null;
        public List<HIS_KSK_GENERAL> KSKGeneral = null;
        public List<HIS_SERE_SERV> _ListSereServ = null;
        public List<HIS_SERE_SERV_TEIN> _ListSereServTein = null;
        public List<HIS_SERE_SERV> _ListSereServ02 = null;
        public List<HIS_SERE_SERV> _ListSereServXetNghiem = null;
        public List<HIS_HEALTH_EXAM_RANK> _ListHealthExamRank = null;
        public List<HIS_DHST> _ListDhst = null;
        public List<HIS_TEST_INDEX> _ListTestIndex = null;
        public List<HIS_SERE_SERV_EXT> _ListSereServExt = null;
        public Mps000449PDO() 
        { 
        }

        public Mps000449PDO(List<V_HIS_SERVICE_REQ> serviceReq, 
            List<HIS_KSK_GENERAL> kSKGeneral, 
            List<HIS_SERE_SERV> listSereServ, 
            List<HIS_SERE_SERV_TEIN> listSereServTein,
            List<HIS_HEALTH_EXAM_RANK> listHealthExamRank,
            List<HIS_DHST> listDhst,
            List<HIS_TEST_INDEX> listTestIndex,
            List<HIS_SERE_SERV_EXT> listSereServExt)
        {
            this.ServiceReq = serviceReq;
            this.KSKGeneral = kSKGeneral;
            this._ListSereServ = listSereServ;
            this._ListSereServTein = listSereServTein;
            this._ListHealthExamRank = listHealthExamRank;
            this._ListDhst = listDhst;
            this._ListTestIndex = listTestIndex;
            this._ListSereServExt = listSereServExt;
        }
    }
}
