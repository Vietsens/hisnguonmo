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
using MOS.SDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000131.PDO
{
    public partial class Mps000131PDO : RDOBase
    {
        public Mps000131PDO() { }
        public Mps000131PDO(
            V_HIS_MEDI_STOCK mediStock,
            List<HisMedicineInStockSDO> lstMedicineInStockSDO,
            List<HisMaterialInStockSDO> lstMaterialInStockSDO,
            List<HisBloodTypeInStockSDO> lstBloodInStockSDO,
            bool isCheckMedicine,
            bool isCheckMaterial,
            bool isCheckBlood
            )
        {
            try
            {
                this.mediStock = mediStock;
                this.lstMedicineInStockSDO = lstMedicineInStockSDO;
                this.lstMaterialInStockSDO = lstMaterialInStockSDO;
                this.lstBloodInStockSDO = lstBloodInStockSDO;
                this.isCheckMedicine = isCheckMedicine;
                this.isCheckMaterial = isCheckMaterial;
                this.isCheckBlood = isCheckBlood;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
