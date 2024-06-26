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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000168.PDO
{
    public partial class Mps000168PDO : RDOBase
    {
        public Mps000168PDO() { }
        public Mps000168PDO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, V_HIS_EXP_MEST_1 expMest)
        {
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this._expMest = expMest;
        }

        public Mps000168PDO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, V_HIS_EXP_MEST_1 expMest, List<V_HIS_EXP_MEST_USER> expMestUser)
        {
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this._expMest = expMest;
            this._ExpMestUserPrint = expMestUser;
        }

        public Mps000168PDO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export, V_HIS_EXP_MEST_1 expMest)
        {
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
            this._expMest = expMest;
        }
    }
}
