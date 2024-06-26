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

namespace MPS.Processor.Mps000205.PDO
{
    public partial class Mps000205PDO : RDOBase
    {
        public List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = null;
        public List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = null;
        public List<V_HIS_EXP_MEST_USER> _ExpMestUserPrint = null;
        public List<Mps000205ADO> listAdo = new List<Mps000205ADO>();
        public List<RoleADO> roleAdo = new List<RoleADO>();

        public long expMesttSttId__Draft = 1;// trạng thái nháp
        public long expMesttSttId__Request = 2;// trạng thái yêu cầu
        public long expMesttSttId__Reject = 3;// không duyệt
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất

        public Mps000205PDO(V_HIS_EXP_MEST expMest,V_HIS_EXP_MEST lostExpMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterials)
        {
            this._ExpMest = expMest;
            this._ExpMestMaterials = ExpMestMaterials;
            this._ExpMestMedicines = expMestMedicines;
        }

        public Mps000205PDO(V_HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterials, List<V_HIS_EXP_MEST_USER> listExpMestUserPrint)
        {
            this._ExpMest = expMest;
            this._ExpMestMaterials = ExpMestMaterials;
            this._ExpMestMedicines = expMestMedicines;
            this._ExpMestUserPrint = listExpMestUserPrint;
        }

        public Mps000205PDO(V_HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterials, List<V_HIS_EXP_MEST_USER> listExpMestUserPrint, long expMesttSttId__Draft, long expMesttSttId__Request, long expMesttSttId__Reject, long expMesttSttId__Approval, long expMesttSttId__Export)
        {
            this._ExpMest = expMest;
            this._ExpMestMaterials = ExpMestMaterials;
            this._ExpMestMedicines = expMestMedicines;
            this._ExpMestUserPrint = listExpMestUserPrint;
            this.expMesttSttId__Approval = expMesttSttId__Approval;
            this.expMesttSttId__Draft = expMesttSttId__Draft;
            this.expMesttSttId__Export = expMesttSttId__Export;
            this.expMesttSttId__Reject = expMesttSttId__Reject;
            this.expMesttSttId__Request = expMesttSttId__Request;
        }


    }
}
