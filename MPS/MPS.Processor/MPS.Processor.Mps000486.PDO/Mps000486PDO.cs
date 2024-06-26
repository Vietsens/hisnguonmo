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

namespace MPS.Processor.Mps000486.PDO
{
    public partial class Mps000486PDO : RDOBase
    {
        public List<HIS_SERVICE_REQ> listServiceReq { get; set; }
        public List<V_HIS_SERE_SERV_2> listVSereServ2 { get; set; }
        public List<HIS_SERVICE_REQ_METY> listServiceReqMety { get; set; }
        public List<HIS_SERVICE_REQ_MATY> listServiceReqMaty { get; set; }
        public List<V_HIS_MEDICINE_TYPE> listVMedicineType { get; set; }
        public List<V_HIS_MATERIAL_TYPE> listVMaterialType { get; set; }
        public Mps000486ADO ado486 { get; set; }
        public Mps000486PDO() { }

        public Mps000486PDO(List<HIS_SERVICE_REQ> listServiceReq, List<V_HIS_SERE_SERV_2> listVSereServ2, List<HIS_SERVICE_REQ_METY> listServiceReqMety, List<HIS_SERVICE_REQ_MATY> listServiceReqMaty, List<V_HIS_MEDICINE_TYPE> listVMedicineType, List<V_HIS_MATERIAL_TYPE> listVMaterialType, Mps000486ADO ado486)
        {
            this.listServiceReq = listServiceReq;
            this.listVSereServ2 = listVSereServ2;
            this.listServiceReqMety = listServiceReqMety;
            this.listServiceReqMaty = listServiceReqMaty;
            this.listVMedicineType = listVMedicineType;
            this.listVMaterialType = listVMaterialType;
            this.ado486 = ado486;
        }
    }
    public class Mps000486ADO
    {
        public long ADO_TIME_FROM { get; set; }
        public long ADO_TIME_TO { get; set; }
    }
}

