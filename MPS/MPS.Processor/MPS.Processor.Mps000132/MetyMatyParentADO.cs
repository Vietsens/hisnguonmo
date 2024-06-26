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
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000132.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000132
{
    class MetyMatyParentADO
    {
        //public MetyMatyParentADO()
        //{

        //}
        public long ID { get; set; }
        public string METY_MATY_CODE { get; set; }
        public string METY_MATY_NAME { get; set; }
        public bool IsMedicineType { get; set; }
        public long? MEDICINE_GROUP_ID { get; set; }
        public string MEDICINE_GROUP_CODE { get; set; }
        public MetyMatyParentADO(V_HIS_MEDICINE_TYPE medicine)
        {
            try
            {
                this.ID = medicine.ID;
                this.METY_MATY_CODE = medicine.MEDICINE_TYPE_CODE;
                this.METY_MATY_NAME = medicine.MEDICINE_TYPE_NAME;
                this.IsMedicineType = true;
                this.MEDICINE_GROUP_CODE = medicine.MEDICINE_GROUP_CODE;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        } 
        public MetyMatyParentADO(V_HIS_MATERIAL_TYPE medicine)
        {
            try
            {
                this.ID = medicine.ID;
                this.METY_MATY_CODE = medicine.MATERIAL_TYPE_CODE;
                this.METY_MATY_NAME = medicine.MATERIAL_TYPE_NAME;
                this.IsMedicineType = false;
                this.MEDICINE_GROUP_ID = null;
                this.MEDICINE_GROUP_CODE = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }

}
