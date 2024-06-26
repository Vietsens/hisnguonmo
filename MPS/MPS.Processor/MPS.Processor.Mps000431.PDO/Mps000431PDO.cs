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

namespace MPS.Processor.Mps000431.PDO
{
    public class Mps000431PDO : RDOBase
    {
        public const string printTypeCode = "Mps000431";

        public V_HIS_TRANSACTION HisTransaction { get; set; }

        public List<ProductADO> lstProductADO { get; set;}



        public Mps000431PDO(V_HIS_TRANSACTION _HisTransaction, List<ProductADO> _lstProductADO)
        {
            try
            {
                this.HisTransaction = _HisTransaction;
                this.lstProductADO = _lstProductADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ProductADO 
    {
        public decimal Amount { get; set; }
        public string ProdName { get; set; }
        public decimal ProdPrice { get; set; }
        public decimal ProdQuantity { get; set; }
        public string ProdUnit { get; set; }
    }
}
