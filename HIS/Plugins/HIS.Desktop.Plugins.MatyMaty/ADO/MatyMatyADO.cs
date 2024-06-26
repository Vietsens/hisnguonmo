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

namespace HIS.Desktop.Plugins.MatyMaty.ADO
{
    public class MatyMatyADO : HIS_MATY_MATY
    {
        public string SERVICE_UNIT_NAME { get; set; }

        public MatyMatyADO() { }

        public MatyMatyADO(HIS_MATY_MATY matyMaty, HIS_MATERIAL_TYPE materialType, List<HIS_SERVICE_UNIT> ListServiceUnit)
        {
            this.ID = matyMaty.ID;
            this.MATERIAL_TYPE_ID = matyMaty.MATERIAL_TYPE_ID;
            this.PREPARATION_MATERIAL_TYPE_ID = matyMaty.PREPARATION_MATERIAL_TYPE_ID;
            this.PREPARATION_AMOUNT = matyMaty.PREPARATION_AMOUNT;
            if (materialType != null)
            {
                this.MATERIAL_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                this.MATERIAL_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                var serviceUnit = ListServiceUnit != null && ListServiceUnit.Count > 0 ? ListServiceUnit.FirstOrDefault(o => o.ID == materialType.TDL_SERVICE_UNIT_ID) : null;
                if (serviceUnit != null)
                {
                    this.SERVICE_UNIT_NAME = serviceUnit.SERVICE_UNIT_NAME;
                }

            }

        }

        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }
    }
}
