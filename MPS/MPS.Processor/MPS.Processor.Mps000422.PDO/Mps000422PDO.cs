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

namespace MPS.Processor.Mps000422.PDO
{
    public partial class Mps000422PDO : RDOBase
    {
        public V_HIS_EXP_MEST_BLTY_REQ _MestBLTYReq = null;
        public V_HIS_EXP_MEST _SaleExpMest = null;
        public List<V_HIS_EXP_MEST_BLTY_REQ> _ListMestBLTYReq = null;
        public List<V_HIS_EXP_MEST> _ListSaleExpMest = null;


        public Mps000422PDO(List<V_HIS_EXP_MEST> listSaleExpMest, List<V_HIS_EXP_MEST_BLTY_REQ> listMestBLTYReq, V_HIS_EXP_MEST saleExpMest, V_HIS_EXP_MEST_BLTY_REQ mestBLTYReq)
        {
            this._ListSaleExpMest = listSaleExpMest;
            this._ListMestBLTYReq = listMestBLTYReq;
            this._SaleExpMest = saleExpMest;
            this._MestBLTYReq = mestBLTYReq;
        }

        public Mps000422PDO(V_HIS_EXP_MEST saleExpMest, List<V_HIS_EXP_MEST_BLTY_REQ> listMestBLTYReq)
        {
            this._SaleExpMest = saleExpMest;
            this._ListMestBLTYReq = listMestBLTYReq;
        }

    }
}
