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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MetyMaty.Change
{
    public class MetyMatyADO : HIS_METY_MATY
    {
        public MetyMatyADO() { }

        public MetyMatyADO(HIS_METY_MATY MetyMaty, HIS_MEDICINE_TYPE medicineType)
        {
            this.ID = MetyMaty.ID;
            this.METY_PRODUCT_ID = MetyMaty.METY_PRODUCT_ID;
            this.MATERIAL_TYPE_ID = MetyMaty.MATERIAL_TYPE_ID;
            this.MATERIAL_TYPE_AMOUNT = MetyMaty.MATERIAL_TYPE_AMOUNT;
            if (medicineType != null)
            {
                this.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                this.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
            }
        }

        public MetyMatyADO(HIS_METY_METY MetyMety, HIS_MEDICINE_TYPE medicineType)
        {
            this.ID = MetyMety.ID;
            this.METY_PRODUCT_ID = MetyMety.METY_PRODUCT_ID;
            this.MATERIAL_TYPE_ID = MetyMety.PREPARATION_MEDICINE_TYPE_ID;
            this.MATERIAL_TYPE_AMOUNT = MetyMety.PREPARATION_AMOUNT;
            if (medicineType != null)
            {
                this.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                this.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
            }
        }

        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
    }
}
