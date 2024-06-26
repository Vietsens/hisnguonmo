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

namespace MPS.Processor.Mps000130.PDO
{
    public partial class Mps000130PDO : RDOBase
    {
        public Mps000130PDO() { }
        public Mps000130PDO(
            HIS_EXP_MEST expMest,
            List<V_HIS_EXP_MEST_MEDICINE> hisImpMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> hisImpMestMaterials,
            List<V_HIS_EXP_MEST_BLOOD> hisImpMestBloods,
            List<HIS_SUPPLIER> _suppliers,
            List<V_HIS_MEDI_STOCK> _mediStocks
            )
        {
            try
            {
                this.expMest = expMest;
                this._Medicines = hisImpMestMedicines;
                this._Materials = hisImpMestMaterials;
                this._Bloods = hisImpMestBloods;
                this._Suppliers = _suppliers;
                this._MediStocks = _mediStocks;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
