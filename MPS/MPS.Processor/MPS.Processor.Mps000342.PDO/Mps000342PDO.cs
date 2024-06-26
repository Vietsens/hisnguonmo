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

namespace MPS.Processor.Mps000342.PDO
{
    public class Mps000342PDO : RDOBase
    {
        public HIS_TRANSACTION _Transaction { get; set; }
        public List<HIS_EXP_MEST> _ExpMests { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> _Materials { get; set; }
        public List<HIS_BILL_GOODS> _BillGoods { get; set; }


        public Mps000342PDO(HIS_TRANSACTION transaction, List<HIS_EXP_MEST> expMests, List<V_HIS_EXP_MEST_MEDICINE> medicines, List<V_HIS_EXP_MEST_MATERIAL> materials,List<HIS_BILL_GOODS> billGoods)
        {
            this._Transaction = transaction;
            this._ExpMests = expMests;
            this._Medicines = medicines;
            this._Materials = materials;
            this._BillGoods = billGoods;
        }
    }
}
