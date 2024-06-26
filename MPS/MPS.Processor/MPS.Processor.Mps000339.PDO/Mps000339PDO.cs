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

namespace MPS.Processor.Mps000339.PDO
{
    public class Mps000339PDO : RDOBase
    {
        public V_HIS_TRANSACTION Transaction { get; set; }
        public List<HIS_BILL_GOODS> ListBillGoods { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> Medicines { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> Materials { get; set; }
        public List<V_HIS_EXP_MEST> ListExpMest { get; set; }
        public List<V_HIS_IMP_MEST> _HisImpMest  { get; set; }

        public Mps000339PDO() { }

        public Mps000339PDO(V_HIS_TRANSACTION transaction, List<HIS_BILL_GOODS> billGooDs, List<V_HIS_EXP_MEST_MEDICINE> _Medicines, List<V_HIS_EXP_MEST_MATERIAL> _Materials)
        {
            this.Transaction = transaction;
            this.ListBillGoods = billGooDs;
            this.Medicines = _Medicines;
            this.Materials = _Materials;
        }

        public Mps000339PDO(V_HIS_TRANSACTION transaction, List<HIS_BILL_GOODS> billGooDs, List<V_HIS_EXP_MEST_MEDICINE> _Medicines, List<V_HIS_EXP_MEST_MATERIAL> _Materials, List<V_HIS_EXP_MEST> listExpMest)
        {
            this.Transaction = transaction;
            this.ListBillGoods = billGooDs;
            this.Medicines = _Medicines;
            this.Materials = _Materials;
            this.ListExpMest = listExpMest;
        }

        public Mps000339PDO(V_HIS_TRANSACTION transaction, List<HIS_BILL_GOODS> billGooDs, List<V_HIS_EXP_MEST_MEDICINE> _Medicines, List<V_HIS_EXP_MEST_MATERIAL> _Materials, List<V_HIS_EXP_MEST> listExpMest, List<V_HIS_IMP_MEST> hisImpMest)
        {
            this.Transaction = transaction;
            this.ListBillGoods = billGooDs;
            this.Medicines = _Medicines;
            this.Materials = _Materials;
            this.ListExpMest = listExpMest;
            this._HisImpMest = hisImpMest;
        }
        public Mps000339PDO(V_HIS_TRANSACTION transaction, List<HIS_BILL_GOODS> billGooDs, List<V_HIS_EXP_MEST_MEDICINE> _Medicines, List<V_HIS_EXP_MEST_MATERIAL> _Materials, List<V_HIS_IMP_MEST> hisImpMest)
        {
            this.Transaction = transaction;
            this.ListBillGoods = billGooDs;
            this.Medicines = _Medicines;
            this.Materials = _Materials;
            this._HisImpMest = hisImpMest;
        }
    }
}
