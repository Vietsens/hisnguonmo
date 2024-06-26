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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AggrApprove.ADO
{
    public class MediMateTypeADO
    {
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public decimal YCD_AMOUNT { get; set; }
        public decimal? SUM_IN_STOCK { get; set; }

        public MediMateTypeADO() { }
        public MediMateTypeADO(HisMedicineTypeInStockSDO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.MEDI_MATE_TYPE_ID = medicine.Id;
                    this.MEDI_MATE_TYPE_CODE = medicine.MedicineTypeCode;
                    this.MEDI_MATE_TYPE_NAME = medicine.MedicineTypeName;
                    this.SERVICE_UNIT_NAME = medicine.ServiceUnitName;
                    this.SUM_IN_STOCK = medicine.AvailableAmount;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public MediMateTypeADO(HisMaterialTypeInStockSDO material)
        {
            try
            {
                if (material != null)
                {
                    this.MEDI_MATE_TYPE_ID = material.Id;
                    this.MEDI_MATE_TYPE_CODE = material.MaterialTypeCode;
                    this.MEDI_MATE_TYPE_NAME = material.MaterialTypeName;
                    this.SERVICE_UNIT_NAME = material.ServiceUnitName;
                    this.SUM_IN_STOCK = material.AvailableAmount;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
