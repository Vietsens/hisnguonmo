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

namespace MPS.Processor.Mps000099.PDO
{
    public partial class Mps000099PDO : RDOBase
    {
        public V_HIS_EXP_MEST _SaleExpMest = null;
        public List<V_HIS_EXP_MEST> _LstSaleExpMest = null;
        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        public List<V_HIS_EXP_MEST_MATERIAL> _Materials = null;
        
        public string GenderName = null;
        public long? DOB = null;

        public long expMesttSttId__Draft = 1;// trạng thái nháp
        public long expMesttSttId__Request = 2;// trạng thái yêu cầu
        public long expMesttSttId__Reject = 3;// không duyệt
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất

        public Mps000099PDO(V_HIS_EXP_MEST saleExpMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines, string genderName, long? dob, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export)
        {
            this._SaleExpMest = saleExpMest;
            this._Medicines = expMestMedicines;
            this.GenderName = genderName;
            this.DOB = dob;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
        }
        public Mps000099PDO(V_HIS_EXP_MEST saleExpMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines)
        {
            this._SaleExpMest = saleExpMest;
            this._Medicines = expMestMedicines;
        }
        public Mps000099PDO( List<V_HIS_EXP_MEST> lstSaleExpMest, List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines,List<V_HIS_EXP_MEST_MATERIAL> expMestMaterial)
        {
            this._LstSaleExpMest = lstSaleExpMest;
            this._Medicines = expMestMedicines;
            this._Materials = expMestMaterial;
        }
    }
}
